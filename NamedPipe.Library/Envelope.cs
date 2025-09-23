using System.Runtime.Serialization;

namespace NamedPipe.Library
{
   /// <summary>
   /// RPC 信封：包含動作名稱、關聯 ID、負載、以及是否為回覆。
   /// </summary>
   [DataContract]
   internal sealed class Envelope
   {
      #region Fields

      [DataMember(Order = 1)] public string CorrelationId; // GUID 字串；回覆需帶回同一值
      [DataMember(Order = 3)] public bool IsResponse;      // true=回覆；false=請求
      [DataMember(Order = 2)] public string Payload;       // 字串負載（也可放 JSON 字串）
      [DataMember(Order = 0)] public string Type;          // 動作/路由，如 "Echo"、"GetStatus"

      #endregion
   }
}