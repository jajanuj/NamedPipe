using System;
using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace NamedPipe.Library
{
   /// <summary>
   /// 具名管道客戶端（支援 raw 訊息、Request/Response、處理器註冊、診斷事件）。
   /// </summary>
   public sealed class PipeClient : IDisposable
   {
      #region Fields

      private readonly ConcurrentDictionary<string, Func<string, Task<string>>> _handlers = new ConcurrentDictionary<string, Func<string, Task<string>>>();
      private readonly ConcurrentDictionary<string, Func<string, EventContext, Task>> _eventHandlers = new ConcurrentDictionary<string, Func<string, EventContext, Task>>();
      private readonly ConcurrentDictionary<string, PendingEntry> _pending = new ConcurrentDictionary<string, PendingEntry>();
      private readonly string _pipeName;
      private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);
      private readonly string _serverName;
      private NamedPipeClientStream _client;
      private CancellationTokenSource _cts;
      private Task _pumpTask;

      #endregion

      #region Constructors

      /// <summary>建立客戶端。</summary>
      public PipeClient(string pipeName, string serverName = ".")
      {
         if (string.IsNullOrEmpty(pipeName))
         {
            throw new ArgumentNullException(nameof(pipeName));
         }

         _pipeName = pipeName;
         _serverName = string.IsNullOrEmpty(serverName) ? "." : serverName;
      }

      #endregion

      #region Delegates, Events

      /// <summary>當接收迴圈遇到未處理例外時觸發。</summary>
      public event Action<Exception> Faulted;

      /// <summary>接收非 Envelope 的原始訊息時觸發。</summary>
      public event Action<string> MessageReceived;

      /// <summary>收到遲到/動作不符的回覆時觸發（診斷用途）。</summary>
      public event Action<UnmatchedResponseInfo> UnmatchedResponse;

      #endregion

      #region Properties

      /// <summary>是否已連線。</summary>
      public bool IsConnected
      {
         get { return _client != null && _client.IsConnected; }
      }

      #endregion

      #region Public Methods

      /// <summary>連線到伺服器。</summary>
      public async Task ConnectAsync(int timeoutMs = 30000, CancellationToken ct = default(CancellationToken))
      {
         _client = new NamedPipeClientStream(_serverName, _pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
         await Task.Run(() => _client.Connect(timeoutMs), ct).ConfigureAwait(false);
         _cts = new CancellationTokenSource();
         _pumpTask = Task.Run(() => PumpAsync(_cts.Token), ct);
      }

      /// <summary>傳送原始文字訊息。</summary>
      public async Task SendAsync(string message, CancellationToken ct = default(CancellationToken))
      {
         if (_client == null || !_client.IsConnected)
         {
            throw new InvalidOperationException("Not connected");
         }

         await _sendLock.WaitAsync(ct).ConfigureAwait(false);
         try
         {
            await Framing.WriteAsync(_client, message ?? string.Empty, ct).ConfigureAwait(false);
         }
         finally
         {
            _sendLock.Release();
         }
      }

      /// <summary>註冊可供伺服器呼叫的處理器（雙向 RPC）。</summary>
      public void RegisterHandler(string action, Func<string, Task<string>> handler)
      {
         if (string.IsNullOrEmpty(action))
         {
            throw new ArgumentNullException(nameof(action));
         }

         if (handler == null)
         {
            throw new ArgumentNullException(nameof(handler));
         }

         _handlers[action] = handler;
      }

      /// <summary>註冊事件處理器。事件處理器負責自行發送回覆，可發送多次。</summary>
      public void RegisterEventHandler(string eventAction, Func<string, EventContext, Task> eventHandler)
      {
         if (string.IsNullOrEmpty(eventAction))
         {
            throw new ArgumentNullException(nameof(eventAction));
         }

         if (eventHandler == null)
         {
            throw new ArgumentNullException(nameof(eventHandler));
         }

         _eventHandlers[eventAction] = eventHandler;
      }

      /// <summary>對伺服器發出 RPC 呼叫並等待回覆。</summary>
      public async Task<string> CallAsync(string action, string payload, int timeoutMs = 10000, CancellationToken ct = default(CancellationToken))
      {
         if (_client == null || !_client.IsConnected)
         {
            throw new InvalidOperationException("Not connected");
         }

         var cid = Guid.NewGuid().ToString("N");
         var tcs = new TaskCompletionSource<string>();
         _pending.TryAdd(cid, new PendingEntry { Tcs = tcs, Action = action ?? string.Empty, CreatedUtc = DateTime.UtcNow });
         var msg = EnvelopeOptions.AddPrefix(JsonUtil.Serialize(new Envelope
            { Type = action, CorrelationId = cid, Payload = payload ?? string.Empty, IsResponse = false }));
         try
         {
            await SendAsync(msg, ct).ConfigureAwait(false);
         }
         catch (Exception ex)
         {
            if (_pending.TryRemove(cid, out var removed))
            {
               removed.Tcs.TrySetException(new InvalidOperationException("Send failed (" + removed.Action + ")", ex));
            }

            throw;
         }

         using (var timeoutCts = new CancellationTokenSource(timeoutMs))
         using (timeoutCts.Token.Register(() =>
                {
                   if (_pending.TryRemove(cid, out var removed))
                   {
                      removed.Tcs.TrySetException(new TimeoutException("CallAsync timeout: " + removed.Action));
                      OnUnmatchedResponse(cid, removed.Action, removed.Action, DateTime.UtcNow - removed.CreatedUtc);
                   }
                }))
         {
            return await tcs.Task.ConfigureAwait(false);
         }
      }

      /// <summary>取得目前等待回覆的請求清單（快照）。</summary>
      public PendingInfo[] GetPendingSnapshot()
      {
         var now = DateTime.UtcNow;
         var list = new System.Collections.Generic.List<PendingInfo>();
         foreach (var kv in _pending)
         {
            var e = kv.Value;
            list.Add(new PendingInfo { CorrelationId = kv.Key, Action = e.Action, CreatedUtc = e.CreatedUtc, Age = now - e.CreatedUtc });
         }

         return list.ToArray();
      }

      #endregion

      #region Private Methods

      private void OnUnmatchedResponse(string cid, string type, string expectedAction, TimeSpan? age)
      {
         var h = UnmatchedResponse;
         if (h != null)
         {
            h(new UnmatchedResponseInfo { CorrelationId = cid, Type = type, ExpectedAction = expectedAction, Age = age, Side = "Client" });
         }
      }

      // 背景接收迴圈
      private async Task PumpAsync(CancellationToken ct)
      {
         try
         {
            while (!ct.IsCancellationRequested && _client != null && _client.IsConnected)
            {
               string raw = await Framing.ReadAsync(_client, ct).ConfigureAwait(false);
               Envelope env = null;
               if (EnvelopeOptions.LooksLikeEnvelope(raw))
               {
                  var json = EnvelopeOptions.StripPrefix(raw);
                  try
                  {
                     env = JsonUtil.Deserialize<Envelope>(json);
                  }
                  catch
                  {
                  }
               }

               if (env != null)
               {
                  if (env.IsResponse && !string.IsNullOrEmpty(env.CorrelationId))
                  {
                     if (_pending.TryRemove(env.CorrelationId, out var ent))
                     {
                        if (!string.IsNullOrEmpty(ent.Action) && !string.Equals(ent.Action, env.Type, StringComparison.Ordinal))
                        {
                           ent.Tcs.TrySetException(new InvalidOperationException("Response type mismatch. cid=" + env.CorrelationId + ", expected=" +
                                                                                 ent.Action + ", got=" + env.Type));
                           OnUnmatchedResponse(env.CorrelationId, env.Type, ent.Action, DateTime.UtcNow - ent.CreatedUtc);
                           continue;
                        }

                        ent.Tcs.TrySetResult(env.Payload ?? string.Empty);
                        continue;
                     }

                     OnUnmatchedResponse(env.CorrelationId, env.Type, null, null);
                     continue;
                  }

                  if (!env.IsResponse && !string.IsNullOrEmpty(env.Type))
                  {
                     // Check if this is an Event command (starts with "Event")
                     bool isEventCommand = env.Type.StartsWith("Event", StringComparison.OrdinalIgnoreCase);
                     
                     if (isEventCommand && _eventHandlers.TryGetValue(env.Type, out var eventHandler))
                     {
                        // For Event commands, create an event context that allows multiple responses
                        var eventContext = new EventContext(this, env.Type, env.CorrelationId);
                        
                        _ = Task.Run(async () =>
                        {
                           try
                           {
                              // Pass the event context to the handler
                              await eventHandler(env.Payload ?? string.Empty, eventContext).ConfigureAwait(false);
                           }
                           catch (Exception ex)
                           {
                              // Send error response for Event commands
                              try
                              {
                                 await eventContext.SendResponseAsync("ERROR: " + ex.Message).ConfigureAwait(false);
                              }
                              catch
                              {
                                 var f = Faulted;
                                 if (f != null)
                                 {
                                    f(ex);
                                 }
                              }
                           }
                        }, ct);
                        continue;
                     }
                     else if (_handlers.TryGetValue(env.Type, out var handler))
                     {
                        // Regular RPC handling (existing behavior)
                        _ = Task.Run(async () =>
                        {
                           string payloadOut = string.Empty;
                           try
                           {
                              payloadOut = await handler(env.Payload ?? string.Empty).ConfigureAwait(false);
                           }
                           catch (Exception ex)
                           {
                              payloadOut = "ERROR: " + ex.Message;
                           }

                           var resp = new Envelope { Type = env.Type, CorrelationId = env.CorrelationId, Payload = payloadOut, IsResponse = true };
                           try
                           {
                              await SendAsync(EnvelopeOptions.AddPrefix(JsonUtil.Serialize(resp)), CancellationToken.None).ConfigureAwait(false);
                           }
                           catch (Exception ex)
                           {
                              var f = Faulted;
                              if (f != null)
                              {
                                 f(ex);
                              }
                           }
                        }, ct);
                        continue;
                     }
                  }
               }

               var h = MessageReceived;
               if (h != null)
               {
                  h(raw);
               }
            }
         }
         catch (Exception ex)
         {
            var f = Faulted;
            if (f != null)
            {
               f(ex);
            }
         }
      }

      #endregion

      /// <summary>釋放連線與背景接收資源。</summary>
      public void Dispose()
      {
         try
         {
            _cts?.Cancel();
            if (_pumpTask != null)
            {
               Task.WaitAny(new[] { _pumpTask }, 200);
            }
         }
         catch
         {
         }
         finally
         {
            _client?.Dispose();
            _cts?.Dispose();
            try
            {
               _sendLock.Dispose();
            }
            catch
            {
            }
         }
      }
   }
}