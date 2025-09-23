using System;
using System.Threading.Tasks;

namespace NamedPipe.Library
{
   /// <summary>
   /// 代表一個待回覆的請求等待項，用於 CorrelationId 配對。
   /// </summary>
   internal sealed class PendingEntry
   {
      #region Fields

      public string Action;
      public DateTime CreatedUtc;
      public TaskCompletionSource<string> Tcs;

      #endregion
   }
}