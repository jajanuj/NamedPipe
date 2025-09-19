using System;
using System.Windows.Forms;

namespace NamedPipe.Server
{
   internal static class Program
   {
      #region Private Methods

      /// <summary>
      /// 應用程式的主要進入點。
      /// </summary>
      [STAThread]
      static void Main()
      {
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);
         Application.Run(new ServerForm());
      }

      #endregion
   }
}