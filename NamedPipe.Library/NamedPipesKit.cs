// NamedPipesKit.cs — .NET Framework 4.7 相容的單檔輕量函式庫（含 Request/Response + correlationId）
// 設計：UTF-8 文字訊息，4-byte little-endian Int32 長度前置分幀
// 功能：多客戶端伺服器、事件式訊息接收、廣播、非同步 API、可取消/關閉
// 新增：Envelope（Type/CorrelationId/Payload/IsResponse）、Client.CallAsync、Server.RegisterHandler
// 相容性：不使用 C# 8 可空註記、移除 IAsyncDisposable/ValueTask、避免 .NET Fx 4.7 無法使用的 API

using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NamedPipe.Library
{
   // ---------------- Framing：長度前置分幀 ----------------
   internal static class Framing
   {
      #region Constant

      private const int MaxFrame = 64 * 1024 * 1024; // 64MB 上限

      #endregion

      #region Public Methods

      public static async Task WriteAsync(Stream stream, string message, CancellationToken ct)
      {
         if (stream == null)
         {
            throw new ArgumentNullException(nameof(stream));
         }

         if (message == null)
         {
            message = string.Empty;
         }

         byte[] payload = Encoding.UTF8.GetBytes(message);
         byte[] lenBytes = BitConverter.GetBytes(payload.Length);
         await stream.WriteAsync(lenBytes, 0, lenBytes.Length, ct).ConfigureAwait(false);
         await stream.WriteAsync(payload, 0, payload.Length, ct).ConfigureAwait(false);
         await stream.FlushAsync(ct).ConfigureAwait(false);
      }

      public static async Task<string> ReadAsync(Stream stream, CancellationToken ct)
      {
         if (stream == null)
         {
            throw new ArgumentNullException(nameof(stream));
         }

         byte[] lenBuf = new byte[4];
         await FillBufferAsync(stream, lenBuf, 4, ct).ConfigureAwait(false);
         int len = BitConverter.ToInt32(lenBuf, 0);
         if (len < 0 || len > MaxFrame)
         {
            throw new IOException("Invalid frame length: " + len);
         }

         byte[] payload = new byte[len];
         await FillBufferAsync(stream, payload, len, ct).ConfigureAwait(false);
         return Encoding.UTF8.GetString(payload, 0, len);
      }

      #endregion

      #region Private Methods

      private static async Task FillBufferAsync(Stream s, byte[] buf, int len, CancellationToken ct)
      {
         int read = 0;
         while (read < len)
         {
            int n = await s.ReadAsync(buf, read, len - read, ct).ConfigureAwait(false);
            if (n == 0)
            {
               throw new EndOfStreamException("Pipe closed by peer");
            }

            read += n;
         }
      }

      #endregion
   }

   // ---------------- Envelope 與 JSON 工具 ----------------
   [DataContract]
   internal sealed class Envelope
   {
      #region Fields

      [DataMember(Order = 1)] public string CorrelationId; // GUID 字串；回覆需帶回同一值
      [DataMember(Order = 3)] public bool IsResponse;      // true=回覆；false=請求
      [DataMember(Order = 2)] public string Payload;       // 實際資料（字串或序列化後字串）
      [DataMember(Order = 0)] public string Type;          // 動作/路由，如 "Echo"、"Add"

      #endregion
   }

   internal static class JsonUtil
   {
      #region Public Methods

      public static string Serialize<T>(T obj)
      {
         var s = new DataContractJsonSerializer(typeof(T));
         using (var ms = new MemoryStream())
         {
            s.WriteObject(ms, obj);
            return Encoding.UTF8.GetString(ms.ToArray());
         }
      }

      public static T Deserialize<T>(string json)
      {
         var s = new DataContractJsonSerializer(typeof(T));
         using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
         {
            return (T)s.ReadObject(ms);
         }
      }

      #endregion
   }

   // ---------------- PipeClient ----------------
   public sealed class PipeClient : IDisposable
   {
      #region Fields

      // Request/Response 等待表
      private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _pending
         = new ConcurrentDictionary<string, TaskCompletionSource<string>>();

      private readonly string _pipeName;
      private readonly string _serverName;
      private NamedPipeClientStream _client;
      private CancellationTokenSource _cts;
      private Task _pumpTask;

      #endregion

      #region Constructors

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

      public event Action<Exception> Faulted;

      public event Action<string> MessageReceived; // 原始訊息事件（非 Envelope 或無匹配 pending 時）

      #endregion

      #region Properties

      public bool IsConnected
      {
         get { return _client != null && _client.IsConnected; }
      }

      #endregion

      #region Public Methods

      public async Task ConnectAsync(int timeoutMs = 30000, CancellationToken ct = default(CancellationToken))
      {
         _client = new NamedPipeClientStream(_serverName, _pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
         // .NET Fx 4.7：以同步 Connect 包裝成 Task，支援取消與逾時
         await Task.Run(() => _client.Connect(timeoutMs), ct).ConfigureAwait(false);
         _cts = new CancellationTokenSource();
         _pumpTask = Task.Run(() => PumpAsync(_cts.Token), ct);
      }

      public async Task SendAsync(string message, CancellationToken ct = default(CancellationToken))
      {
         if (_client == null || !_client.IsConnected)
         {
            throw new InvalidOperationException("Not connected");
         }

         await Framing.WriteAsync(_client, message, ct).ConfigureAwait(false);
      }

      // ---- 新增：可等待回覆的呼叫 ----
      public async Task<string> CallAsync(string action, string payload, int timeoutMs = 10000, CancellationToken ct = default(CancellationToken))
      {
         if (_client == null || !_client.IsConnected)
         {
            throw new InvalidOperationException("Not connected");
         }

         var cid = Guid.NewGuid().ToString("N");
         var tcs = new TaskCompletionSource<string>();
         _pending.TryAdd(cid, tcs);

         var env = new Envelope { Type = action, CorrelationId = cid, Payload = payload ?? string.Empty, IsResponse = false };
         var msg = JsonUtil.Serialize(env);
         try
         {
            await SendAsync(msg, ct).ConfigureAwait(false);
         }
         catch (Exception ex)
         {
            _pending.TryRemove(cid, out var removed);
            if (removed != null)
            {
               removed.TrySetException(ex);
            }

            throw;
         }

         using (var timeoutCts = new CancellationTokenSource(timeoutMs))
         using (timeoutCts.Token.Register(() =>
                {
                   if (_pending.TryRemove(cid, out var removed))
                   {
                      removed.TrySetException(new TimeoutException("CallAsync timeout"));
                   }
                }))
         {
            // 等待對應回覆（Pump 收到後會 TrySetResult）
            return await tcs.Task.ConfigureAwait(false);
         }
      }

      #endregion

      #region Private Methods

      private async Task PumpAsync(CancellationToken ct)
      {
         try
         {
            while (!ct.IsCancellationRequested && _client != null && _client.IsConnected)
            {
               string raw = await Framing.ReadAsync(_client, ct).ConfigureAwait(false);

               // 優先嘗試作為 Envelope 處理
               Envelope env = null;
               try
               {
                  env = JsonUtil.Deserialize<Envelope>(raw);
               }
               catch
               {
               }

               if (env != null && env.IsResponse && !string.IsNullOrEmpty(env.CorrelationId))
               {
                  if (_pending.TryRemove(env.CorrelationId, out var tcs))
                  {
                     tcs.TrySetResult(env.Payload ?? string.Empty);
                     continue;
                  }
               }

               var h = MessageReceived;
               h?.Invoke(raw);
            }
         }
         catch (Exception ex)
         {
            var f = Faulted;
            f?.Invoke(ex);
         }
      }

      #endregion

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
         }
      }
   }

   // ---------------- PipeServer ----------------
   public sealed class PipeServer : IDisposable
   {
      #region Fields

      private readonly ConcurrentDictionary<int, ClientConnection> _clients = new ConcurrentDictionary<int, ClientConnection>();

      // Action Handler 表
      private readonly ConcurrentDictionary<string, Func<string, Task<string>>> _handlers = new ConcurrentDictionary<string, Func<string, Task<string>>>();
      private readonly int _maxInstances;
      private readonly string _pipeName;
      private Task _acceptLoop;
      private CancellationTokenSource _cts;
      private int _nextId = 0;

      #endregion

      #region Constructors

      public PipeServer(string pipeName, int maxInstances = 5)
      {
         if (string.IsNullOrEmpty(pipeName))
         {
            throw new ArgumentNullException(nameof(pipeName));
         }

         _pipeName = pipeName;
         _maxInstances = Math.Max(1, maxInstances);
      }

      #endregion

      #region Delegates, Events

      public event Action<ClientConnection> ClientConnected;
      public event Action<ClientConnection> ClientDisconnected;
      public event Action<Exception> Faulted;
      public event Action<ClientConnection, string> MessageReceived; // 原始訊息事件（非 Envelope 或無 handler 時）

      #endregion

      #region Public Methods

      public void Start()
      {
         if (_cts != null)
         {
            throw new InvalidOperationException("Server already started");
         }

         _cts = new CancellationTokenSource();
         _acceptLoop = Task.Run(() => AcceptLoopAsync(_cts.Token));
      }

      public void RegisterHandler(string action, Func<string, Task<string>> handler)
      {
         if (string.IsNullOrEmpty(action))
         {
            throw new ArgumentNullException(nameof(action));
         }

         _handlers[action] = handler ?? throw new ArgumentNullException(nameof(handler));
      }

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
               var server = new NamedPipeServerStream(_pipeName, PipeDirection.InOut, _maxInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
               await WaitForConnectionAsync(server, ct).ConfigureAwait(false);

               int id = Interlocked.Increment(ref _nextId);
               var conn = new ClientConnection(id, server, this);
               if (!_clients.TryAdd(id, conn))
               {
                  server.Dispose();
                  continue;
               }

               var cc = ClientConnected;
               cc?.Invoke(conn);

               _ = Task.Run(() => conn.RunAsync(ct), ct);
            }
         }
         catch (OperationCanceledException)
         {
         }
         catch (Exception ex)
         {
            var f = Faulted;
            f?.Invoke(ex);
         }
      }

      // 以 APM 包裝 WaitForConnection 支援取消
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

      public void Dispose()
      {
         try
         {
            _cts?.Cancel();

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

            _cts?.Dispose();
         }
      }

      internal void OnClientDisconnected(ClientConnection conn)
      {
         _clients.TryRemove(conn.Id, out var @_);
         var h = ClientDisconnected;
         h?.Invoke(conn);
      }

      internal void OnClientMessage(ClientConnection conn, string msg)
      {
         var h = MessageReceived;
         h?.Invoke(conn, msg);
      }

      public sealed class ClientConnection : IDisposable
      {
         #region Fields

         private readonly PipeServer _owner;
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

         public int Id { get; private set; }

         public bool IsConnected
         {
            get { return _stream.IsConnected; }
         }

         #endregion

         #region Public Methods

         public Task SendAsync(string message, CancellationToken ct = default(CancellationToken))
         {
            return Framing.WriteAsync(_stream, message, ct);
         }

         #endregion

         public void Dispose()
         {
            _stream.Dispose();
         }

         internal async Task RunAsync(CancellationToken ct)
         {
            try
            {
               while (!ct.IsCancellationRequested && _stream.IsConnected)
               {
                  string raw = await Framing.ReadAsync(_stream, ct).ConfigureAwait(false);

                  // 嘗試 Envelope 請求 -> 執行 handler -> 回覆
                  Envelope env = null;
                  try
                  {
                     env = JsonUtil.Deserialize<Envelope>(raw);
                  }
                  catch
                  {
                  }

                  if (env != null && !env.IsResponse && !string.IsNullOrEmpty(env.Type))
                  {
                     if (_owner._handlers.TryGetValue(env.Type, out var handler))
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
                        await SendAsync(JsonUtil.Serialize(resp), ct).ConfigureAwait(false);
                        continue; // 已處理
                     }
                  }

                  _owner.OnClientMessage(this, raw); // 回退到原事件
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
            }
         }
      }
   }
}

// ------------------------------
// Usage 範例（.NET Framework 4.7 主控台）
// ------------------------------
/*
using System;
using System.Threading.Tasks;
using NamedPipesKit;

class ServerDemo
{
    static void Main()
    {
        var server = new PipeServer("demo.pipe", maxInstances: 10);

        // 事件：原始訊息（非信封）
        server.MessageReceived += (c, msg) => Console.WriteLine("[raw <= #" + c.Id + "] " + msg);

        // 註冊 Request/Response handlers
        server.RegisterHandler("Echo", s => Task.FromResult("ACK:" + s));
        server.RegisterHandler("Add", s =>
        {
            var parts = (s ?? "0,0").Split(',');
            int a = int.Parse(parts[0]); int b = int.Parse(parts[1]);
            return Task.FromResult((a + b).ToString());
        });

        server.ClientConnected += c => Console.WriteLine("Client #" + c.Id + " connected");
        server.ClientDisconnected += c => Console.WriteLine("Client #" + c.Id + " disconnected");
        server.Faulted += ex => Console.WriteLine("Server error: " + ex);
        server.Start();

        Console.WriteLine("Server started. Enter to broadcast raw message, 'q' to quit.");
        string line;
        while ((line = Console.ReadLine()) != null && line != "q")
            server.BroadcastAsync(line).Wait();
    }
}

class ClientDemo
{
    static void Main()
    {
        var client = new PipeClient("demo.pipe");
        client.MessageReceived += msg => Console.WriteLine("[raw <=] " + msg);
        client.Faulted += ex => Console.WriteLine("Client error: " + ex);
        client.ConnectAsync().Wait();

        // 事件式傳訊
        client.SendAsync("hello raw").Wait();

        // Request/Response 呼叫
        var echo = client.CallAsync("Echo", "ping", 3000).Result; // => "ACK:ping"
        Console.WriteLine("Echo resp: " + echo);

        var sum = client.CallAsync("Add", "3,5", 3000).Result; // => "8"
        Console.WriteLine("Add resp: " + sum);

        Console.WriteLine("Type to send raw; 'q' to quit.");
        string line;
        while ((line = Console.ReadLine()) != null && line != "q")
            client.SendAsync(line).Wait();
    }
}
*/