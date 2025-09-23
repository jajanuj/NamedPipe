using System;

namespace NamedPipe.Library
{
   /// <summary>
   /// Envelope 訊息辨識策略。建議使用固定前綴避免 raw 訊息被誤判為 JSON。
   /// </summary>
   public static class EnvelopeOptions
   {
      #region Constant

      /// <summary>是否將所有 Envelope JSON 字串加上固定前綴，接收端僅解析具此前綴的訊息。</summary>
      public static bool UsePrefix = true;

      /// <summary>Envelope 前綴字串（預設 "@"）。</summary>
      public static string Prefix = "@";

      /// <summary>若未使用前綴，是否啟用簡易嗅探以降低誤判機率。</summary>
      public static bool UseSniff = true;

      #endregion

      internal static string AddPrefix(string s)
      {
         if (UsePrefix && s != null && !s.StartsWith(Prefix))
         {
            return Prefix + s;
         }

         return s;
      }

      internal static bool LooksLikeEnvelope(string s)
      {
         if (string.IsNullOrEmpty(s))
         {
            return false;
         }

         if (UsePrefix)
         {
            return s.StartsWith(Prefix);
         }

         if (s.Length < 2 || s[0] != '{' || s[s.Length - 1] != '}')
         {
            return false;
         }

         return s.IndexOf("\"Type\"", StringComparison.OrdinalIgnoreCase) >= 0
                && s.IndexOf("\"CorrelationId\"", StringComparison.OrdinalIgnoreCase) >= 0
                && s.IndexOf("\"IsResponse\"", StringComparison.OrdinalIgnoreCase) >= 0;
      }

      internal static string StripPrefix(string s)
      {
         if (UsePrefix && s != null && s.StartsWith(Prefix))
         {
            return s.Substring(Prefix.Length);
         }

         return s;
      }
   }
}