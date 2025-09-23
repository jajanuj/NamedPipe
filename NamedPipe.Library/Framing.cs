using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NamedPipe.Library
{
   /// <summary>
   /// 以長度前置（4-byte little-endian Int32）分幀的傳輸協定。
   /// </summary>
   internal static class Framing
   {
      #region Constant

      private const int MaxFrame = 64 * 1024 * 1024; // 64MB 上限

      #endregion

      #region Public Methods

      /// <summary>寫入一個 UTF-8 文字訊息（含長度欄位）。</summary>
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

      /// <summary>讀取一個 UTF-8 文字訊息（根據長度欄位）。</summary>
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
}