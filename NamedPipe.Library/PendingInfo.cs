using System;

namespace NamedPipe.Library
{
   /// <summary>
   /// 對外公開的等待清單快照項目。
   /// </summary>
   public sealed class PendingInfo
   {
      #region Properties

      public string CorrelationId { get; set; }
      public string Action { get; set; }
      public DateTime CreatedUtc { get; set; }
      public TimeSpan Age { get; set; }

      #endregion
   }
}