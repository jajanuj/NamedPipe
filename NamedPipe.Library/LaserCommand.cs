using System;

namespace NamedPipe.Library
{
   /// <summary>
   /// 程式命令定義
   /// </summary>
   public static class LaserCommand
   {
      #region Enums

      /// <summary>
      /// 命令列舉。
      /// </summary>
      public enum Command
      {
         /// <summary>
         /// 設定條碼物件的內容。
         /// 格式: SetBarcodeContent,物件名稱,物件內容
         /// 參數: 物件名稱(string), 物件內容(string)
         /// </summary>
         SetBarcodeContent,

         /// <summary>
         /// 設定文字物件的內容。
         /// 格式: SetTextContent,物件名稱,物件內容
         /// 參數: 物件名稱(string), 物件內容(string)
         /// </summary>
         SetTextContent,

         /// <summary>
         /// 載入雷射標記圖檔。
         /// 格式: LoadFile,雷射圖檔路徑
         /// 參數: 雷射圖檔路徑(string)
         /// </summary>
         LoadFile,

         /// <summary>
         /// 設定雷射物件的位移和角度補償值。
         /// 格式: SetLaserOffset,物件名稱,補償值X,補償值Y,補償值角度
         /// 參數: 物件名稱(string), 補償值X(double), 補償值Y(double), 補償值角度(double)
         /// </summary>
         SetLaserOffset,

         /// <summary>
         /// 啟用或停用指定的雷射物件。
         /// 格式: SetLaserable,物件名稱,是否啟用
         /// 參數: 物件名稱(string), 是否啟用(bool)
         /// </summary>
         SetLaserable,

         /// <summary>
         /// 開始執行雷射標記任務。
         /// 格式: StartMarking
         /// </summary>
         StartMarking,

         /// <summary>
         /// 停止正在進行的雷射標記任務。
         /// 格式: StopMarking
         /// </summary>
         StopMarking,

         /// <summary>
         /// 開始監控雷射機台的狀態。
         /// 格式: StartMonitoring
         /// </summary>
         StartMonitoring,

         /// <summary>
         /// 停止監控雷射機台的狀態。
         /// 格式: StopMonitoring
         /// </summary>
         StopMonitoring
      }

      #endregion

      #region Public Methods

      /// <summary>
      /// 取得命令名稱字串（等同於 enum 名稱）。
      /// </summary>
      public static string ToActionString(Command command) => command.ToString();

      /// <summary>
      /// 由命令名稱字串解析為 enum，忽略大小寫。
      /// </summary>
      public static bool TryParse(string text, out Command command)
      {
         return Enum.TryParse(text, ignoreCase: true, out command);
      }

      #endregion
   }
}