using System;
using System.Threading;
using System.Threading.Tasks;

namespace NamedPipe.Library
{
   /// <summary>
   /// 針對「只會有一個固定 client 連線」的 WinForms/WPF 介面封裝：
   /// 提供 UI 安全事件、連線狀態、原始傳送、與對 client 發 RPC 呼叫。
   /// </summary>
   public sealed class ServerSingleClientHelper : IDisposable
   {
      #region Fields

      private readonly PipeServer _server;
      private readonly SynchronizationContext _ui;
      private PipeServer.ClientConnection _conn;

      #endregion

      #region Constructors

      public ServerSingleClientHelper(string pipeName, SynchronizationContext ui = null)
      {
         _ui = ui ?? SynchronizationContext.Current ?? new SynchronizationContext();
         _server = new PipeServer(pipeName, maxInstances: 1);
         _server.ClientConnected += c =>
         {
            _conn = c;
            _ui.Post(_ =>
            {
               var h = OnlineChanged;
               h?.Invoke(true);
            }, null);
         };
         _server.ClientDisconnected += c =>
         {
            if (_conn == c)
            {
               _conn = null;
            }

            _ui.Post(_ =>
            {
               var h = OnlineChanged;
               h?.Invoke(false);
            }, null);
         };
         _server.MessageReceived += (c, msg) => _ui.Post(_ =>
         {
            var h = RawReceived;
            h?.Invoke(msg);
         }, null);
         _server.Faulted += ex => _ui.Post(_ =>
         {
            var h = Faulted;
            h?.Invoke(ex);
         }, null);
         _server.UnmatchedResponse += (c, info) => _ui.Post(_ =>
         {
            var h = UnmatchedResponse;
            h?.Invoke(info);
         }, null);
      }

      #endregion

      #region Delegates, Events

      /// <summary>伺服器錯誤事件（UI 執行緒）。</summary>
      public event Action<Exception> Faulted;

      /// <summary>連線狀態改變（UI 執行緒）。</summary>
      public event Action<bool> OnlineChanged;

      /// <summary>收到 raw 訊息（UI 執行緒）。</summary>
      public event Action<string> RawReceived;

      /// <summary>未匹配回覆診斷事件（UI 執行緒）。</summary>
      public event Action<UnmatchedResponseInfo> UnmatchedResponse;

      #endregion

      #region Properties

      /// <summary>目前是否有 client 連線。</summary>
      public bool IsOnline
      {
         get { return _conn != null && _conn.IsConnected; }
      }

      #endregion

      #region Public Methods

      /// <summary>啟動伺服器。</summary>
      public void Start() => _server.Start();

      /// <summary>嘗試傳送 raw 訊息；若未連線則回傳 false。</summary>
      public async Task<bool> TrySendRaw(string text, CancellationToken ct = default(CancellationToken))
      {
         var c = _conn;
         if (c == null || !c.IsConnected)
         {
            return false;
         }

         try
         {
            await c.SendAsync(text ?? string.Empty, ct).ConfigureAwait(false);
            return true;
         }
         catch
         {
            return false;
         }
      }

      /// <summary>對唯一 client 發出 RPC 呼叫。</summary>
      public async Task<string> CallClientAsync(string action, string payload, int timeoutMs = 10000, CancellationToken ct = default(CancellationToken))
      {
         var c = _conn;
         if (c == null || !c.IsConnected)
         {
            throw new InvalidOperationException("未連線");
         }

         return await c.CallAsync(action, payload, timeoutMs, ct).ConfigureAwait(false);
      }

      /// <summary>對唯一 client 發送事件命令，返回可接收多次回覆的事件監聽器。</summary>
      public EventListener SendEventToClient(string eventAction, string payload)
      {
         var c = _conn;
         if (c == null || !c.IsConnected)
         {
            throw new InvalidOperationException("未連線");
         }

         return c.SendEventAsync(eventAction, payload);
      }

      /// <summary>取得唯一 client 的 pending 請求快照（未連線則回空陣列）。</summary>
      public PendingInfo[] GetClientPendingSnapshot()
      {
         var c = _conn;
         return (c != null && c.IsConnected) ? c.GetPendingSnapshot() : new PendingInfo[0];
      }

      #endregion

      /// <summary>釋放資源。</summary>
      public void Dispose()
      {
         _server.Dispose();
      }
   }
}