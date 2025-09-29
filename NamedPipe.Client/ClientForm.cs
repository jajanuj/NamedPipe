using GRT.SDK.Framework.Logger;
using NamedPipe.Library;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static NamedPipe.Library.LaserCommand;

namespace NamedPipe.Client
{
   public partial class ClientForm : Form
   {
      #region Fields

      private readonly BlockingCollection<string> _logQ = new BlockingCollection<string>();
      private readonly AsyncManualResetEvent _ready = new AsyncManualResetEvent();

      private PipeClient _client;
      private CancellationTokenSource _logCts;
      private Task _uiAppenderTask;

      #endregion

      #region Constructors

      public ClientForm()
      {
         InitializeComponent();
         InitializeClient();
         Logger.Start($"{Application.StartupPath}");
      }

      #endregion

      #region Protected Methods

      protected override void OnFormClosing(FormClosingEventArgs e)
      {
         base.OnFormClosing(e);
         try
         {
            _logQ.CompleteAdding();
            _logCts?.Cancel();
            if (_uiAppenderTask is Task t)
            {
               t.Wait(500); // Wait with assignment
            }
         }
         catch
         {
         }
         finally
         {
            try
            {
               _client?.Dispose();
            }
            catch
            {
            }
         }
      }

      #endregion

      #region Private Methods

      private async void InitializeClient()
      {
         EnvelopeOptions.UsePrefix = true;
         _client = new PipeClient("demo.pipe");

         _client.MessageReceived += msg => { _logQ.Add($"[Received] {msg}\r\n"); };
         _client.Faulted += ex => _logQ.Add($"[ERR] {ex}");

         // 註冊可被 Server 呼叫的動作
         _client.RegisterHandler("GetStatus", ResponseStatus);
         _client.RegisterHandler(nameof(Command.SetBarcodeContent), s =>
         {
            _logQ.Add($"[SetBarcodeContent] {s}\r\n");
            return Task.FromResult("SetBarcodeContent OK");
         });

         _client.RegisterHandler(nameof(Command.SetTextContent), s =>
         {
            _logQ.Add($"[{Command.SetTextContent}] {s}\r\n");
            return Task.FromResult($"{Command.SetTextContent} OK");
         });

         _client.RegisterHandler(nameof(Command.LoadFile), s =>
         {
            _logQ.Add($"[{Command.LoadFile}] {s}\r\n");
            return Task.FromResult($"{Command.LoadFile} OK");
         });

         _client.RegisterHandler(nameof(Command.SetLaserOffset), s =>
         {
            _logQ.Add($"[{Command.SetLaserOffset}] {s}\r\n");
            return Task.FromResult($"{Command.SetLaserOffset} OK");
         });

         _client.RegisterHandler(nameof(Command.StartMonitoring), s =>
         {
            //_logQ.Add($"[{Command.StartMonitoring}] {s}\r\n");
            return Task.FromResult($"1,0,1");
         });

         // 註冊 EventStatusChanged 事件處理器
         _client.RegisterEventHandler("EventStatusChanged", HandleEventStatusChanged);

         await _client.ConnectAsync();
         _logQ.Add($"Connected.\r\n");

         _logCts = new CancellationTokenSource();
         _uiAppenderTask = Task.Run(() =>
         {
            try
            {
               foreach (var line in _logQ.GetConsumingEnumerable(_logCts.Token))
               {
                  BeginInvoke((Action)(() => txtLog.AppendText(line + "")));
               }
            }
            catch (OperationCanceledException)
            {
            }
         }, _logCts.Token);
      }

      private void MarkReady() => _ready.Set();

      private void MarkNotReady() => _ready.Reset();

      private async Task<string> ResponseStatus(string result)
      {
         await _ready.WaitAsync();
         MarkNotReady();
         var msg = $"OK- {Environment.MachineName}";
         _logQ.Add($"[Send] {msg}\r\n");
         return msg;
      }

      private async Task HandleEventStatusChanged(string payload, EventContext eventContext)
      {
         _logQ.Add($"[EventStatusChanged] 收到事件命令，開始監控狀態變化...\r\n");
         
         // 模擬狀態變化並多次回應
         var random = new Random();
         
         for (int i = 1; i <= 5; i++)
         {
            // 等待一段時間模擬狀態變化
            await Task.Delay(2000); // 2秒間隔
            
            var status = new
            {
               Timestamp = DateTime.Now.ToString("HH:mm:ss"),
               StatusId = i,
               MachineName = Environment.MachineName,
               ProcessId = System.Diagnostics.Process.GetCurrentProcess().Id,
               RandomValue = random.Next(100, 999)
            };
            
            var response = $"Status_{i}: {status.Timestamp} - {status.MachineName} (PID:{status.ProcessId}) Value:{status.RandomValue}";
            
            _logQ.Add($"[EventStatusChanged] 發送回覆 #{i}: {response}\r\n");
            
            // 向服務器發送狀態回覆
            await eventContext.SendResponseAsync(response);
         }
         
         _logQ.Add($"[EventStatusChanged] 狀態監控完成，共發送 5 次回覆\r\n");
      }

      private void btnResponseStatus_Click(object sender, EventArgs e)
      {
         MarkReady();
      }

      private async void btnSendRawMessage_Click(object sender, EventArgs e)
      {
         await _client.SendAsync(txtRawMessage.Text);
      }

      #endregion
   }
}