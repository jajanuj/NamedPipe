using GRT.SDK.Framework.FlowChart;
using NamedPipe.Library;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NamedPipe.Server
{
   public partial class ServerForm : Form
   {
      #region Fields

      private bool _flowRunning;
      private bool _isSent;

      private PipeServer.ClientConnection _onlyClient;

      private bool _serverClose;
      private Task _task;

      #endregion

      #region Constructors

      public ServerForm()
      {
         InitializeComponent();
         InitializeServer();
      }

      #endregion

      #region Private Methods

      private void InitializeServer()
      {
         var server = new PipeServer("demo.pipe", maxInstances: 10);

         // 事件：原始訊息（非信封）
         server.MessageReceived += (c, msg) => Console.WriteLine("[raw <= #" + c.Id + "] " + msg);

         // 註冊 Request/Response handlers
         server.RegisterHandler("Echo", s => Task.FromResult("ACK:" + s));
         server.RegisterHandler("Add", s =>
         {
            var parts = (s ?? "0,0").Split(',');
            int a = int.Parse(parts[0]);
            int b = int.Parse(parts[1]);
            return Task.FromResult((a + b).ToString());
         });

         server.ClientConnected += c =>
         {
            _onlyClient = c;
            Console.WriteLine("Client #" + c.Id + " connected");
         };
         server.ClientDisconnected += c => Console.WriteLine("Client #" + c.Id + " disconnected");
         server.Faulted += ex => Console.WriteLine("Server error: " + ex);
         server.Start();

         Console.WriteLine("Server started. Enter to broadcast raw message, 'q' to quit.");
         Task.Run(() => WaitCloseCommand(server));
      }

      private static void WaitCloseCommand(PipeServer server)
      {
         string line;
         while ((line = Console.ReadLine()) != null && line != "q")
         {
            server.BroadcastAsync(line).Wait();
         }
      }

      private void btnServerClose_Click(object sender, EventArgs e)
      {
         _serverClose = false;
      }

      private async void btnSendRawMessage_Click(object sender, EventArgs e)
      {
         if (_onlyClient == null || !_onlyClient.IsConnected)
         {
            MessageBox.Show("尚未連線。");
            return;
         }

         try
         {
            var text = txtRawMessage.Text;
            await _onlyClient.SendAsync(text);
            //txtLog.AppendText($"[=>] {text}\r\n");
            Console.WriteLine($"[S=>C] {text}\r\n");
         }
         catch (Exception ex)
         {
            MessageBox.Show("送出失敗：" + ex.Message);
         }
      }

      private Go flowChart1_Run()
      {
         return Go.Next;
      }

      private Go flowChart2_Run()
      {
         if (_isSent == false)
         {
            return Go.Idle;
         }

         var text = "Flow2 Message";
         _task = _onlyClient.SendAsync(text);
         Console.WriteLine($"[S=>C] {text}\r\n");
         _isSent = false;

         return Go.Next;
      }

      private Go flowChart3_Run()
      {
         if (_task.IsCompleted)
         {
            return Go.Next;
         }

         return Go.Idle;
      }

      private Go flowChart4_Run() => Go.Next;

      private Go flowChart5_Run() => Go.Next;

      private void btnFlowStart_Click(object sender, EventArgs e)
      {
         flowChart1.TaskReset();
         if (_flowRunning)
         {
            return;
         }

         _flowRunning = true;
         Task.Run(() =>
         {
            while (_flowRunning)
            {
               flowChart1.MainRun();
               Thread.Sleep(10);
            }
         });
      }

      private void btnFlowStop_Click(object sender, EventArgs e)
      {
         _flowRunning = false;
      }

      private void btnFlowChart2Send_Click(object sender, EventArgs e)
      {
         _isSent = true;
      }

      #endregion
   }
}