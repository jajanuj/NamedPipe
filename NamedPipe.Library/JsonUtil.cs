using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace NamedPipe.Library
{
   /// <summary>
   /// DataContractJsonSerializer 輔助方法（.NET Fx 4.7 可用）。
   /// </summary>
   internal static class JsonUtil
   {
      #region Public Methods

      public static string Serialize<T>(T obj)
      {
         var s = new DataContractJsonSerializer(typeof(T));
         using (var ms = new MemoryStream())
         {
            s.WriteObject(ms, obj);
            return Encoding.UTF8.GetString(ms.ToArray());
         }
      }

      public static T Deserialize<T>(string json)
      {
         var s = new DataContractJsonSerializer(typeof(T));
         using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
         {
            return (T)s.ReadObject(ms);
         }
      }

      #endregion
   }
}