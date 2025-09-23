using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace NamedPipe.Library
{
   /// <summary>
   /// 具名管道伺服器（支援多客戶端、廣播、雙向 RPC 與診斷事件）。
   /// </summary>
   public sealed class PipeServer : IDisposable
   {
      #region Fields

      private readonly ConcurrentDictionary<int, ClientConnection> _clients = new ConcurrentDictionary<int, ClientConnection>();
      internal readonly ConcurrentDictionary<string, Func<string, Task<string>>> _handlers = new ConcurrentDictionary<string, Func<string, Task<string>>>();
      private readonly SemaphoreSlim _instanceSlots;
      private readonly int _maxInstances;
      private readonly string _pipeName;
      private Task _acceptLoop;
      private CancellationTokenSource _cts;
      private int _nextId = 0;

      #endregion

      #region Constructors

      /// <summary>建立伺服器。</summary>
      public PipeServer(string pipeName, int maxInstances = 5)
      {
         if (string.IsNullOrEmpty(pipeName))
         {
            throw new ArgumentNullException(nameof(pipeName));
         }

         _pipeName = pipeName;
         _maxInstances = Math.Max(1, maxInstances);
         _instanceSlots = new SemaphoreSlim(_maxInstances, _maxInstances);
      }

      #endregion

      #region Delegates, Events

      /// <summary>有客戶端連線時觸發。</summary>
      public event Action<ClientConnection> ClientConnected;

      /// <summary>有客戶端斷線時觸發。</summary>
      public event Action<ClientConnection> ClientDisconnected;

      /// <summary>伺服器背景接收/接受連線出錯時觸發。</summary>
      public event Action<Exception> Faulted;

      /// <summary>收到非 Envelope 的原始訊息時觸發。</summary>
      public event Action<ClientConnection, string> MessageReceived;

      /// <summary>收到未匹配之回覆（遲到或動作不符）時觸發。</summary>
      public event Action<ClientConnection, UnmatchedResponseInfo> UnmatchedResponse;

      #endregion

      #region Public Methods

      /// <summary>啟動接受連線迴圈。</summary>
      public void Start()
      {
         if (_cts != null)
         {
            throw new InvalidOperationException("Server already started");
         }

         _cts = new CancellationTokenSource();
         _acceptLoop = Task.Run(() => AcceptLoopAsync(_cts.Token));
      }

      /// <summary>註冊伺服器端可處理的動作。</summary>
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

      /// <summary>向所有已連線客戶端廣播原始訊息。</summary>
      public async Task BroadcastAsync(string message, CancellationToken ct = default(CancellationToken))
      {
         foreach (var kv in _clients)
         {
            await kv.Value.SendAsync(message, ct).ConfigureAwait(false);
         }
      }

      #endregion

      #region Private Methods

      private async Task AcceptLoopAsync(CancellationToken ct)
      {
         try
         {
            while (!ct.IsCancellationRequested)
            {
               await _instanceSlots.WaitAsync(ct).ConfigureAwait(false);
               NamedPipeServerStream server = null;
               try
               {
                  server = new NamedPipeServerStream(_pipeName, PipeDirection.InOut, _maxInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
               }
               catch
               {
                  _instanceSlots.Release();
                  throw;
               }

               await WaitForConnectionAsync(server, ct).ConfigureAwait(false);
               int id = Interlocked.Increment(ref _nextId);
               var conn = new ClientConnection(id, server, this);
               if (!_clients.TryAdd(id, conn))
               {
                  try
                  {
                     server.Dispose();
                  }
                  finally
                  {
                     _instanceSlots.Release();
                  }

                  continue;
               }

               var cc = ClientConnected;
               if (cc != null)
               {
                  cc(conn);
               }

               _ = Task.Run(async () =>
               {
                  try
                  {
                     await conn.RunAsync(ct).ConfigureAwait(false);
                  }
                  finally
                  {
                     _instanceSlots.Release();
                  }
               }, ct);
            }
         }
         catch (OperationCanceledException)
         {
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

      private static Task WaitForConnectionAsync(NamedPipeServerStream s, CancellationToken ct)
      {
         var tcs = new TaskCompletionSource<object>();
         var reg = ct.Register(() =>
         {
            try
            {
               s.Dispose();
            }
            catch
            {
            }

            tcs.TrySetCanceled();
         });
         try
         {
            s.BeginWaitForConnection(ar =>
            {
               try
               {
                  s.EndWaitForConnection(ar);
                  reg.Dispose();
                  tcs.TrySetResult(null);
               }
               catch (ObjectDisposedException)
               {
                  tcs.TrySetCanceled();
               }
               catch (Exception ex)
               {
                  tcs.TrySetException(ex);
               }
            }, null);
         }
         catch (Exception ex)
         {
            reg.Dispose();
            tcs.TrySetException(ex);
         }

         return tcs.Task;
      }

      #endregion

      /// <summary>釋放伺服器與連線資源。</summary>
      public void Dispose()
      {
         try
         {
            if (_cts != null)
            {
               _cts.Cancel();
            }

            if (_acceptLoop != null)
            {
               Task.WaitAny(new[] { _acceptLoop }, 200);
            }
         }
         catch
         {
         }
         finally
         {
            foreach (var c in _clients.Values)
            {
               c.Dispose();
            }

            if (_cts != null)
            {
               _cts.Dispose();
            }

            try
            {
               _instanceSlots.Dispose();
            }
            catch
            {
            }
         }
      }

      internal void OnClientDisconnected(ClientConnection conn)
      {
         _clients.TryRemove(conn.Id, out _);
         var h = ClientDisconnected;
         if (h != null)
         {
            h(conn);
         }
      }

      internal void OnClientMessage(ClientConnection conn, string msg)
      {
         var h = MessageReceived;
         if (h != null)
         {
            h(conn, msg);
         }
      }

      internal void OnUnmatchedResponse(ClientConnection c, string cid, string type, string expectedAction, TimeSpan? age)
      {
         var h = UnmatchedResponse;
         if (h != null)
         {
            h(c, new UnmatchedResponseInfo { CorrelationId = cid, Type = type, ExpectedAction = expectedAction, Age = age, Side = "Server" });
         }
      }

      /// <summary>
      /// 代表一條與客戶端的連線（含雙向 RPC 與 pending 管理）。
      /// </summary>
      public sealed class ClientConnection : IDisposable
      {
         #region Fields

         private readonly PipeServer _owner;
         private readonly ConcurrentDictionary<string, PendingEntry> _pending = new ConcurrentDictionary<string, PendingEntry>();
         private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);
         private readonly NamedPipeServerStream _stream;

         #endregion

         #region Constructors

         internal ClientConnection(int id, NamedPipeServerStream stream, PipeServer owner)
         {
            Id = id;
            _stream = stream;
            _owner = owner;
         }

         #endregion

         #region Properties

         /// <summary>此連線的識別碼（遞增）。</summary>
         public int Id { get; private set; }

         /// <summary>是否仍連線。</summary>
         public bool IsConnected
         {
            get { return _stream.IsConnected; }
         }

         #endregion

         #region Public Methods

         /// <summary>取得此連線目前等待回覆的請求清單（快照）。</summary>
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

         /// <summary>傳送原始文字訊息給此客戶端。</summary>
         public async Task SendAsync(string message, CancellationToken ct = default(CancellationToken))
         {
            await _sendLock.WaitAsync(ct).ConfigureAwait(false);
            try
            {
               await Framing.WriteAsync(_stream, message ?? string.Empty, ct).ConfigureAwait(false);
            }
            finally
            {
               _sendLock.Release();
            }
         }

         /// <summary>向此客戶端發出 RPC 呼叫並等待回覆。</summary>
         public async Task<string> CallAsync(string action, string payload, int timeoutMs = 10000, CancellationToken ct = default(CancellationToken))
         {
            var cid = Guid.NewGuid().ToString("N");
            var tcs = new TaskCompletionSource<string>();
            _pending.TryAdd(cid, new PendingEntry { Tcs = tcs, Action = action ?? string.Empty, CreatedUtc = DateTime.UtcNow });
            var json = JsonUtil.Serialize(new Envelope { Type = action, CorrelationId = cid, Payload = payload ?? string.Empty, IsResponse = false });
            try
            {
               await SendAsync(EnvelopeOptions.AddPrefix(json), ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
               if (_pending.TryRemove(cid, out var removed))
               {
                  removed.Tcs.TrySetException(new InvalidOperationException("Send failed (" + removed.Action + ")", ex));
                  _owner.OnUnmatchedResponse(this, cid, removed.Action, removed.Action, DateTime.UtcNow - removed.CreatedUtc);
               }

               throw;
            }

            using (var timeoutCts = new CancellationTokenSource(timeoutMs))
            using (timeoutCts.Token.Register(() =>
                   {
                      if (_pending.TryRemove(cid, out var removed))
                      {
                         removed.Tcs.TrySetException(new TimeoutException("CallAsync timeout: " + removed.Action + ", " + removed.CreatedUtc));
                         _owner.OnUnmatchedResponse(this, cid, removed.Action, removed.Action, DateTime.UtcNow - removed.CreatedUtc);
                      }
                   }))
            {
               return await tcs.Task.ConfigureAwait(false);
            }
         }

         #endregion

         /// <summary>釋放底層串流。</summary>
         public void Dispose()
         {
            _stream.Dispose();
            try
            {
               _sendLock.Dispose();
            }
            catch
            {
            }
         }

         internal async Task RunAsync(CancellationToken ct)
         {
            try
            {
               while (!ct.IsCancellationRequested && _stream.IsConnected)
               {
                  string raw = await Framing.ReadAsync(_stream, ct).ConfigureAwait(false);
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
                        if (_pending.TryRemove(env.CorrelationId, out var waiter))
                        {
                           if (!string.IsNullOrEmpty(waiter.Action) && !string.Equals(waiter.Action, env.Type, StringComparison.Ordinal))
                           {
                              waiter.Tcs.TrySetException(new InvalidOperationException("Response type mismatch. cid=" + env.CorrelationId + ", expected=" +
                                                                                       waiter.Action + ", got=" + env.Type));
                              _owner.OnUnmatchedResponse(this, env.CorrelationId, env.Type, waiter.Action, DateTime.UtcNow - waiter.CreatedUtc);
                              continue;
                           }

                           waiter.Tcs.TrySetResult(env.Payload ?? string.Empty);
                           continue;
                        }

                        _owner.OnUnmatchedResponse(this, env.CorrelationId, env.Type, null, null);
                        continue;
                     }

                     if (!env.IsResponse && !string.IsNullOrEmpty(env.Type))
                     {
                        if (_owner._handlers.TryGetValue(env.Type, out var handler))
                        {
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
                                 var f = _owner.Faulted;
                                 if (f != null)
                                 {
                                    f(ex);
                                 }
                              }
                           });
                           continue;
                        }
                     }
                  }

                  _owner.OnClientMessage(this, raw);
               }
            }
            catch (EndOfStreamException)
            {
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
               var f = _owner.Faulted;
               f?.Invoke(ex);
            }
            finally
            {
               _owner.OnClientDisconnected(this);
               _stream.Dispose();
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
}