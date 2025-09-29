using System;
using System.Threading;
using System.Threading.Tasks;

namespace NamedPipe.Library
{
   /// <summary>
   /// 提供給 Event 類型命令的上下文，允許客戶端發送多次回覆。
   /// </summary>
   public sealed class EventContext
   {
      #region Fields

      private readonly PipeClient _client;
      private readonly string _eventType;
      private readonly string _correlationId;

      #endregion

      #region Constructors

      internal EventContext(PipeClient client, string eventType, string correlationId)
      {
         _client = client;
         _eventType = eventType;
         _correlationId = correlationId;
      }

      #endregion

      #region Properties

      /// <summary>事件類型（命令名稱）。</summary>
      public string EventType => _eventType;

      /// <summary>關聯 ID，用於配對請求和回覆。</summary>
      public string CorrelationId => _correlationId;

      #endregion

      #region Public Methods

      /// <summary>向伺服器發送事件回覆。此方法可以被多次調用。</summary>
      /// <param name="payload">回覆內容</param>
      /// <param name="ct">取消權杖</param>
      public async Task SendResponseAsync(string payload, CancellationToken ct = default(CancellationToken))
      {
         var resp = new Envelope 
         { 
            Type = _eventType, 
            CorrelationId = _correlationId, 
            Payload = payload ?? string.Empty, 
            IsResponse = true 
         };
         
         await _client.SendAsync(EnvelopeOptions.AddPrefix(JsonUtil.Serialize(resp)), ct).ConfigureAwait(false);
      }

      #endregion
   }
}