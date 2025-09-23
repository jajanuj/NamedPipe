using System;
using System.Threading;
using System.Threading.Tasks;

namespace NamedPipe.Library
{
   /// <summary>
   /// 可 await 的 ManualResetEvent（以 TaskCompletionSource 實作）。
   /// </summary>
   public sealed class AsyncManualResetEvent
   {
      #region Fields

      private volatile TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();

      #endregion

      #region Public Methods

      /// <summary>等待事件被 Set。</summary>
      public Task WaitAsync(CancellationToken ct = default(CancellationToken))
      {
         if (!ct.CanBeCanceled)
         {
            return _tcs.Task;
         }

         var cancelTcs = new TaskCompletionSource<bool>();
         var reg = ct.Register(() => cancelTcs.TrySetCanceled());
         return Task.WhenAny(_tcs.Task, cancelTcs.Task).ContinueWith(t =>
         {
            reg.Dispose();
            if (t.Result == cancelTcs.Task)
            {
               throw new OperationCanceledException(ct);
            }

            return _tcs.Task;
         }).Unwrap();
      }

      /// <summary>將事件設為有訊號，喚醒所有等待者。</summary>
      public void Set()
      {
         _tcs.TrySetResult(true);
      }

      /// <summary>將事件重置為無訊號。</summary>
      public void Reset()
      {
         if (_tcs.Task.IsCompleted)
         {
            _tcs = new TaskCompletionSource<bool>();
         }
      }

      #endregion
   }
}