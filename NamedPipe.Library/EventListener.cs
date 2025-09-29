using System;
using System.Collections.Concurrent;

namespace NamedPipe.Library
{
   /// <summary>
   /// 事件監聽器，用於接收客戶端對事件命令的多次回覆。
   /// </summary>
   public sealed class EventListener : IDisposable
   {
      #region Fields

      private readonly PipeServer _server;
      private readonly PipeServer.ClientConnection _connection;
      private readonly string _correlationId;
      private readonly string _eventAction;
      private bool _disposed = false;

      #endregion

      #region Constructors

      internal EventListener(PipeServer server, PipeServer.ClientConnection connection, string correlationId, string eventAction)
      {
         _server = server;
         _connection = connection;
         _correlationId = correlationId;
         _eventAction = eventAction;
         
         // Register this listener with the server to receive responses
         _server.RegisterEventListener(this);
      }

      #endregion

      #region Properties

      /// <summary>關聯 ID。</summary>
      public string CorrelationId => _correlationId;

      /// <summary>事件動作名稱。</summary>
      public string EventAction => _eventAction;

      /// <summary>關聯的客戶端連線。</summary>
      public PipeServer.ClientConnection Connection => _connection;

      #endregion

      #region Events

      /// <summary>收到客戶端回覆時觸發。</summary>
      public event Action<string> ResponseReceived;

      /// <summary>發生錯誤時觸發。</summary>
      public event Action<Exception> ErrorOccurred;

      #endregion

      #region Internal Methods

      internal void OnResponse(string payload)
      {
         if (!_disposed)
         {
            ResponseReceived?.Invoke(payload);
         }
      }

      internal void OnError(Exception exception)
      {
         if (!_disposed)
         {
            ErrorOccurred?.Invoke(exception);
         }
      }

      #endregion

      #region IDisposable

      public void Dispose()
      {
         if (!_disposed)
         {
            _disposed = true;
            _server.UnregisterEventListener(this);
         }
      }

      #endregion
   }
}