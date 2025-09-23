using System;

namespace NamedPipe.Library
{
   /// <summary>
   /// 診斷用途：未能匹配的回覆（遲到或動作不符）。
   /// </summary>
   public sealed class UnmatchedResponseInfo
   {
      #region Properties

      public string CorrelationId { get; set; }
      public string Type { get; set; }
      public string ExpectedAction { get; set; }
      public TimeSpan? Age { get; set; }
      public string Side { get; set; } // "Client" 或 "Server"

      #endregion
   }
}