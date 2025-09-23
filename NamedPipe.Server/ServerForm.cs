using GRT.SDK.Framework.Common;
using GRT.SDK.Framework.FlowChart;
using GRT.SDK.Framework.Logger;
using NamedPipe.Library;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static NamedPipe.Library.LaserCommand;

namespace NamedPipe.Server
{
   public partial class ServerForm : Form
   {
      #region Fields

      private bool _flowRunning;
      private ServerSingleClientHelper _helper;
      private bool _isSent;
      private CancellationTokenSource _monitorCts;
      private Task _task;
      private Task<string> _taskStatus;

      #endregion

      #region Constructors

      public ServerForm()
      {
         InitializeComponent();
         InitializeServer();
         Logger.Start($"{Application.StartupPath}");
      }

      #endregion

      #region Protected Methods

      protected override void OnFormClosing(FormClosingEventArgs e)
      {
         base.OnFormClosing(e);
         try
         {
            _helper?.Dispose();
         }
         catch
         {
         }
      }

      #endregion

      #region Private Methods

      private void InitializeServer()
      {
         EnvelopeOptions.UsePrefix = true;
         _helper = new ServerSingleClientHelper("demo.pipe");
         _helper.OnlineChanged += online => lblStatus.Text = online ? "已連線" : "未連線";
         _helper.RawReceived += msg => txtLog.AppendText($"[Server Received] {msg}\r\n");
         _helper.Faulted += ex => txtLog.AppendText($"[Server error] {ex}");
         _helper.UnmatchedResponse += info =>
         {
            txtLog.AppendText(
               $"[Unmatched] cid={info.CorrelationId} got={info.Type} expected={info.ExpectedAction} age={(int)(info.Age?.TotalMilliseconds ?? -1)}ms\r\n");
         };
         _helper.Start();
      }

      private static void WaitCloseCommand(PipeServer server)
      {
         string line;
         while ((line = Console.ReadLine()) != null && line != "q")
         {
            server.BroadcastAsync(line).Wait();
         }
      }

      private async void btnSendRawMessage_Click(object sender, EventArgs e)
      {
         if (!await _helper.TrySendRaw(txtRawMessage.Text))
         {
            MessageBox.Show("尚未連線或送出失敗");
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
         _task = _helper.TrySendRaw(text);
         txtLog.SafeInvoke(x => x.AppendText($"[Flow Send Message]\r\n"));
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

      private Go flowChart6_Run()
      {
         _taskStatus = _helper.CallClientAsync("GetStatus", string.Empty, timeoutMs: 5000);
         txtLog.SafeInvoke(x => x.AppendText($"[Flow Send GetStatus]\r\n"));

         return Go.Next;
      }

      private Go flowChart7_Run()
      {
         if (_taskStatus.IsCompleted)
         {
            if (_taskStatus.IsFaulted)
            {
               txtLog.SafeInvoke(x => x.AppendText($"[GetStatus] 呼叫失敗：{_taskStatus.Exception?.GetBaseException().Message}\r\n"));
            }
            else if (_taskStatus.IsCanceled)
            {
               txtLog.SafeInvoke(x => x.AppendText($"[GetStatus] 呼叫逾時\r\n"));
            }
            else
            {
               txtLog.SafeInvoke(x => x.AppendText($"[GetStatus] {_taskStatus.Result}\r\n"));
            }

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

      private async void btnGetStatus_Click(object sender, EventArgs e)
      {
         try
         {
            txtLog.AppendText($"[Send GetStatus]\r\n");
            var resp = await _helper.CallClientAsync("GetStatus", string.Empty, timeoutMs: 3000);
            txtLog.AppendText($"[GetStatus] {resp}\r\n");
         }
         catch (Exception ex)
         {
            txtLog.AppendText($"[Send GetStatus Fail] {ex.Message}\r\n");
         }
         //await SendCommandAsync(Command., txtOffset.Text);
      }

      private async Task SendCommandAsync(Command cmd, string parameter)
      {
         var commandName = cmd.ToString();
         try
         {
            await SendRpModeCommand(commandName, parameter);
         }
         catch (Exception ex)
         {
            txtLog.AppendText($"[Send {commandName} Fail] {ex.Message}\r\n");
         }
      }

      private async void btnSetBarcodeContent_Click(object sender, EventArgs e)
      {
         await SendCommandAsync(Command.SetBarcodeContent, txtBarcodeContent.Text);
      }

      private async Task SendRpModeCommand(string command, string parameter = "")
      {
         txtLog.SafeInvoke(x => x.AppendText($"[Send {command}]\r\n"));
         var resp = await _helper.CallClientAsync(command, parameter, timeoutMs: 5000);
         txtLog.SafeInvoke(x => x.AppendText($"[Receive] {resp}\r\n"));
      }

      private async void btnSetTextContent_Click(object sender, EventArgs e)
      {
         await SendCommandAsync(Command.SetTextContent, txtTextContent.Text);
      }

      private async void btnLoadFile_Click(object sender, EventArgs e)
      {
         await SendCommandAsync(Command.LoadFile, txtLoadFilePath.Text);
      }

      private async void btnSetLaserOffset_Click(object sender, EventArgs e)
      {
         await SendCommandAsync(Command.SetLaserOffset, txtOffset.Text);
      }

      private void btnStartMonitoring_Click(object sender, EventArgs e)
      {
         if (_monitorCts != null)
         {
            _monitorCts.Cancel();
            _monitorCts.Dispose();
         }

         _monitorCts = new CancellationTokenSource();
         var token = _monitorCts.Token;
         txtLog.AppendText("[Start Monitoring Loop]\r\n");
         btnStartMonitoring.Enabled = false;
         btnStopMonitoring.Enabled = true;
         _ = Task.Run(async () =>
         {
            while (!token.IsCancellationRequested)
            {
               await SendCommandAsync(Command.StartMonitoring, string.Empty);
               await Task.Delay(500, token);
            }
         }, token);
      }

      private void btnStopMonitoring_Click(object sender, EventArgs e)
      {
         if (_monitorCts != null)
         {
            _monitorCts.Cancel();
            _monitorCts.Dispose();
            _monitorCts = null;
            txtLog.AppendText("[Stop Monitoring Loop]\r\n");
         }

         btnStopMonitoring.Enabled = false;
         btnStartMonitoring.Enabled = true;
      }

      #endregion
   }
}