// NamedPipesKit.cs — .NET Framework 4.7 相容的單檔輕量函式庫（含 Request/Response + correlationId）
// Version: 1.2.0  (2025-09-20)
// 設計：UTF-8 文字訊息，4-byte little-endian Int32 長度前置分幀
// 功能：多客戶端伺服器、事件式訊息接收、廣播、非同步 API、可取消/關閉
// 新增：Envelope（Type/CorrelationId/Payload/IsResponse）、Client/Server 雙向 RPC、WinForms 單一客戶端 Helper
// 相容性：不使用 C# 8 可空註記、移除 IAsyncDisposable/ValueTask、避免 .NET Fx 4.7 無法使用的 API

namespace NamedPipe.Library
{
}

// ------------------------------
// WinForms 範例 — Server（單一客戶端 + 雙向 RPC）
// ------------------------------
/*
using NamedPipe.Library;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

public partial class ServerForm : Form
{
    private ServerSingleClientHelper _helper;

    public ServerForm() { InitializeComponent(); }

    private void ServerForm_Load(object sender, EventArgs e)
    {
        EnvelopeOptions.UsePrefix = true; // 建議使用前綴
        _helper = new ServerSingleClientHelper("demo.pipe");
        _helper.OnlineChanged += online => lblStatus.Text = online ? "已連線" : "未連線";
        _helper.RawReceived  += msg => txtLog.AppendText($"[<=] {msg}
");
        _helper.Faulted      += ex  => txtLog.AppendText($"[ERR] {ex}
");
        _helper.Start();
    }

    private async void btnSendRaw_Click(object sender, EventArgs e)
    {
        if (!await _helper.TrySendRaw(txtToSend.Text))
            MessageBox.Show("尚未連線或送出失敗");
    }

    private async void btnGetStatus_Click(object sender, EventArgs e)
    {
        try
        {
            var resp = await _helper.CallClientAsync("GetStatus", string.Empty, timeoutMs: 3000);
            txtLog.AppendText($"[GetStatus] {resp}
");
        }
        catch (Exception ex)
        { MessageBox.Show("呼叫失敗：" + ex.Message); }
    }
    private void btnViewPending_Click(object sender, EventArgs e)
    {
        var items = _helper.GetClientPendingSnapshot();
        if (items.Length == 0)
        {
            txtLog.AppendText("[pending] (empty)
");
            return;
        }
        foreach (var it in items)
            txtLog.AppendText($"[pending] cid={it.CorrelationId}, action={it.Action}, age={(int)it.Age.TotalMilliseconds}ms
");
    }



    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        try { _helper?.Dispose(); } catch { }
    }
}
*/

// ------------------------------
// WinForms 範例 — Client（註冊 handler，回覆 Server 的呼叫）
// ------------------------------
/*
using NamedPipe.Library;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

public partial class ClientForm : Form
{
    private PipeClient _client;

    // 使用 Task + CancellationToken，集中把訊息寫回 UI
    private readonly BlockingCollection<string> _logQ = new BlockingCollection<string>();
    private CancellationTokenSource _logCts;
    private Task _uiAppenderTask;

    public ClientForm() { InitializeComponent(); }

    private async void ClientForm_Load(object sender, EventArgs e)
    {
        EnvelopeOptions.UsePrefix = true; // 與 server 一致
        _client = new PipeClient("demo.pipe");

        _client.MessageReceived += msg => _logQ.Add($"[raw <=] {msg}");
        _client.Faulted += ex => _logQ.Add($"[ERR] {ex}");

        // 註冊可被 Server 呼叫的動作
        _client.RegisterHandler("GetStatus", _ => Task.FromResult("OK-" + Environment.MachineName));

        await _client.ConnectAsync();
        _logQ.Add("Connected.");

        _logCts = new CancellationTokenSource();
        _uiAppenderTask = Task.Run(() =>
        {
            try
            {
                foreach (var line in _logQ.GetConsumingEnumerable(_logCts.Token))
                    BeginInvoke((Action)(() => txtLog.AppendText(line + "
")));
            }
            catch (OperationCanceledException) { }
        }, _logCts.Token);
    }

    private async void btnSendRaw_Click(object sender, EventArgs e)
    {
        await _client.SendAsync(txtToSend.Text);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        try
        {
            _logQ.CompleteAdding();
            _logCts?.Cancel();
            if (_uiAppenderTask is Task t) t.Wait(500); // Wait with assignment
        }
        catch { }
        finally
        {
            try { _client?.Dispose(); } catch { }
        }
    }
}
*/

// ------------------------------
// Changelog
// ------------------------------
// v1.2.0 (2025-09-20)
// - 新增雙向 RPC：Client.RegisterHandler 讓 server 呼叫 client；Server.ClientConnection.CallAsync 可對單一 client 呼叫並等待回覆；
//   ServerSingleClientHelper 新增 CallClientAsync（UI 友善）。
// - 使用 SemaphoreSlim 管控伺服器實例數，避免「所有的管道例項都在使用中」。
// v1.1.2 (2025-09-19)
// - 修正：達到 maxInstances 時不再超額建立伺服器實例（避免「所有的管道例項都在使用中」）。
// v1.1.1 (2025-09-19)
// - 新增 EnvelopeOptions（UsePrefix/Prefix/UseSniff）；新增 ServerSingleClientHelper。
// v1.1.0 (2025-09-19)
// - 命名空間改為 NamedPipe.Library；CallAsync await 送出；一般穩健性微調。
// v1.0 (2025-09-19)
// - 初版：多客戶端伺服器/客戶端；UTF-8 + 4-byte 分幀；事件；Request/Response；APM 取消。