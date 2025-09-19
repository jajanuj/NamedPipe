using NamedPipe.Library;
using System;
using System.Windows.Forms;

namespace NamedPipe.Client
{
   public partial class ClientForm : Form
   {
      #region Fields

      private PipeClient _client;

      #endregion

      #region Constructors

      public ClientForm()
      {
         InitializeComponent();
         InitializeClient();
      }

      #endregion

      #region Private Methods

      private void InitializeClient()
      {
         _client = new PipeClient("demo.pipe");
         _client.MessageReceived += msg => Console.WriteLine("[Client Received] " + msg);
         _client.Faulted += ex => Console.WriteLine("Client error: " + ex);
         _client.ConnectAsync().Wait();

         Console.WriteLine("Type to send raw; 'q' to quit.");
         string line;
         while ((line = Console.ReadLine()) != null && line != "q")
         {
            _client.SendAsync(line).Wait();
         }
      }

      private void btnEventMessage_Click(object sender, EventArgs e)
      {
         // 事件式傳訊
         _client.SendAsync("hello raw").Wait();
      }

      private void btnRequestResponse_Click(object sender, EventArgs e)
      {
         // Request/Response 呼叫
         var echo = _client.CallAsync("Echo", "ping", 3000).Result; // => "ACK:ping"
         Console.WriteLine("Echo resp: " + echo);

         var sum = _client.CallAsync("Add", "3,5", 3000).Result; // => "8"
         Console.WriteLine("Add resp: " + sum);
      }

      #endregion
   }
}