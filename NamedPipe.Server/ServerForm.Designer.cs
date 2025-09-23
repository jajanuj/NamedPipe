namespace NamedPipe.Server
{
   partial class ServerForm
   {
      /// <summary>
      /// 設計工具所需的變數。
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// 清除任何使用中的資源。
      /// </summary>
      /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form 設計工具產生的程式碼

      /// <summary>
      /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
      /// 這個方法的內容。
      /// </summary>
      private void InitializeComponent()
      {
         this.btnSendRawMessage = new System.Windows.Forms.Button();
         this.txtRawMessage = new System.Windows.Forms.TextBox();
         this.btnFlowStart = new System.Windows.Forms.Button();
         this.flowChart1 = new GRT.SDK.Framework.FlowChart.FlowChart();
         this.flowChart2 = new GRT.SDK.Framework.FlowChart.FlowChart();
         this.flowChart3 = new GRT.SDK.Framework.FlowChart.FlowChart();
         this.flowChart6 = new GRT.SDK.Framework.FlowChart.FlowChart();
         this.flowChart7 = new GRT.SDK.Framework.FlowChart.FlowChart();
         this.flowChart4 = new GRT.SDK.Framework.FlowChart.FlowChart();
         this.flowChart5 = new GRT.SDK.Framework.FlowChart.FlowChart();
         this.btnFlowStop = new System.Windows.Forms.Button();
         this.btnFlowChart2Send = new System.Windows.Forms.Button();
         this.txtLog = new System.Windows.Forms.TextBox();
         this.lblStatus = new System.Windows.Forms.Label();
         this.btnGetStatus = new System.Windows.Forms.Button();
         this.btnSetBarcodeContent = new System.Windows.Forms.Button();
         this.btnSetTextContent = new System.Windows.Forms.Button();
         this.btnLoadFile = new System.Windows.Forms.Button();
         this.btnSetLaserOffset = new System.Windows.Forms.Button();
         this.txtBarcodeContent = new System.Windows.Forms.TextBox();
         this.txtTextContent = new System.Windows.Forms.TextBox();
         this.txtLoadFilePath = new System.Windows.Forms.TextBox();
         this.txtOffset = new System.Windows.Forms.TextBox();
         this.btnStartMonitoring = new System.Windows.Forms.Button();
         this.btnStopMonitoring = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // btnSendRawMessage
         // 
         this.btnSendRawMessage.Location = new System.Drawing.Point(17, 44);
         this.btnSendRawMessage.Name = "btnSendRawMessage";
         this.btnSendRawMessage.Size = new System.Drawing.Size(181, 31);
         this.btnSendRawMessage.TabIndex = 1;
         this.btnSendRawMessage.Text = "Send Raw Message";
         this.btnSendRawMessage.UseVisualStyleBackColor = true;
         this.btnSendRawMessage.Click += new System.EventHandler(this.btnSendRawMessage_Click);
         // 
         // txtRawMessage
         // 
         this.txtRawMessage.Location = new System.Drawing.Point(220, 44);
         this.txtRawMessage.Name = "txtRawMessage";
         this.txtRawMessage.Size = new System.Drawing.Size(158, 25);
         this.txtRawMessage.TabIndex = 2;
         this.txtRawMessage.Text = "Hello Client";
         // 
         // btnFlowStart
         // 
         this.btnFlowStart.Location = new System.Drawing.Point(380, 86);
         this.btnFlowStart.Name = "btnFlowStart";
         this.btnFlowStart.Size = new System.Drawing.Size(103, 31);
         this.btnFlowStart.TabIndex = 14;
         this.btnFlowStart.Text = "Flow Start";
         this.btnFlowStart.UseVisualStyleBackColor = true;
         this.btnFlowStart.Click += new System.EventHandler(this.btnFlowStart_Click);
         // 
         // flowChart1
         // 
         this.flowChart1.BackColor = System.Drawing.Color.LightSalmon;
         this.flowChart1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.flowChart1.Caption = "Init Flow";
         this.flowChart1.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Bold);
         this.flowChart1.Location = new System.Drawing.Point(385, 163);
         this.flowChart1.Margin = new System.Windows.Forms.Padding(2);
         this.flowChart1.Name = "flowChart1";
         this.flowChart1.Next = this.flowChart2;
         this.flowChart1.Size = new System.Drawing.Size(167, 32);
         this.flowChart1.TabIndex = 15;
         this.flowChart1.Run += new GRT.SDK.Framework.FlowChart.FlowChart.ReturnType(this.flowChart1_Run);
         // 
         // flowChart2
         // 
         this.flowChart2.BackColor = System.Drawing.Color.LightSalmon;
         this.flowChart2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.flowChart2.Caption = "Send To Client";
         this.flowChart2.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Bold);
         this.flowChart2.Location = new System.Drawing.Point(385, 220);
         this.flowChart2.Margin = new System.Windows.Forms.Padding(2);
         this.flowChart2.Name = "flowChart2";
         this.flowChart2.Next = this.flowChart3;
         this.flowChart2.Size = new System.Drawing.Size(167, 32);
         this.flowChart2.TabIndex = 16;
         this.flowChart2.Run += new GRT.SDK.Framework.FlowChart.FlowChart.ReturnType(this.flowChart2_Run);
         // 
         // flowChart3
         // 
         this.flowChart3.BackColor = System.Drawing.Color.LightSalmon;
         this.flowChart3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.flowChart3.Caption = "Wait Finish";
         this.flowChart3.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Bold);
         this.flowChart3.Location = new System.Drawing.Point(385, 277);
         this.flowChart3.Margin = new System.Windows.Forms.Padding(2);
         this.flowChart3.Name = "flowChart3";
         this.flowChart3.Next = this.flowChart6;
         this.flowChart3.Size = new System.Drawing.Size(167, 32);
         this.flowChart3.TabIndex = 17;
         this.flowChart3.Run += new GRT.SDK.Framework.FlowChart.FlowChart.ReturnType(this.flowChart3_Run);
         // 
         // flowChart6
         // 
         this.flowChart6.BackColor = System.Drawing.Color.LightSalmon;
         this.flowChart6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.flowChart6.Caption = "Get Client Status";
         this.flowChart6.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Bold);
         this.flowChart6.Location = new System.Drawing.Point(385, 329);
         this.flowChart6.Margin = new System.Windows.Forms.Padding(2);
         this.flowChart6.Name = "flowChart6";
         this.flowChart6.Next = this.flowChart7;
         this.flowChart6.Size = new System.Drawing.Size(167, 32);
         this.flowChart6.TabIndex = 25;
         this.flowChart6.Run += new GRT.SDK.Framework.FlowChart.FlowChart.ReturnType(this.flowChart6_Run);
         // 
         // flowChart7
         // 
         this.flowChart7.BackColor = System.Drawing.Color.LightSalmon;
         this.flowChart7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.flowChart7.Caption = "Wait Result";
         this.flowChart7.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Bold);
         this.flowChart7.Location = new System.Drawing.Point(385, 378);
         this.flowChart7.Margin = new System.Windows.Forms.Padding(2);
         this.flowChart7.Name = "flowChart7";
         this.flowChart7.Next = this.flowChart4;
         this.flowChart7.Size = new System.Drawing.Size(167, 32);
         this.flowChart7.TabIndex = 26;
         this.flowChart7.Run += new GRT.SDK.Framework.FlowChart.FlowChart.ReturnType(this.flowChart7_Run);
         // 
         // flowChart4
         // 
         this.flowChart4.BackColor = System.Drawing.Color.LightSalmon;
         this.flowChart4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.flowChart4.Caption = "-";
         this.flowChart4.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Bold);
         this.flowChart4.Location = new System.Drawing.Point(582, 378);
         this.flowChart4.Margin = new System.Windows.Forms.Padding(2);
         this.flowChart4.Name = "flowChart4";
         this.flowChart4.Next = this.flowChart5;
         this.flowChart4.Size = new System.Drawing.Size(40, 32);
         this.flowChart4.TabIndex = 18;
         this.flowChart4.Run += new GRT.SDK.Framework.FlowChart.FlowChart.ReturnType(this.flowChart4_Run);
         // 
         // flowChart5
         // 
         this.flowChart5.BackColor = System.Drawing.Color.LightSalmon;
         this.flowChart5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.flowChart5.Caption = "-";
         this.flowChart5.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Bold);
         this.flowChart5.Location = new System.Drawing.Point(582, 220);
         this.flowChart5.Margin = new System.Windows.Forms.Padding(2);
         this.flowChart5.Name = "flowChart5";
         this.flowChart5.Next = this.flowChart2;
         this.flowChart5.Size = new System.Drawing.Size(40, 32);
         this.flowChart5.TabIndex = 19;
         this.flowChart5.Run += new GRT.SDK.Framework.FlowChart.FlowChart.ReturnType(this.flowChart5_Run);
         // 
         // btnFlowStop
         // 
         this.btnFlowStop.Location = new System.Drawing.Point(519, 86);
         this.btnFlowStop.Name = "btnFlowStop";
         this.btnFlowStop.Size = new System.Drawing.Size(103, 31);
         this.btnFlowStop.TabIndex = 20;
         this.btnFlowStop.Text = "Flow Stop";
         this.btnFlowStop.UseVisualStyleBackColor = true;
         this.btnFlowStop.Click += new System.EventHandler(this.btnFlowStop_Click);
         // 
         // btnFlowChart2Send
         // 
         this.btnFlowChart2Send.Location = new System.Drawing.Point(380, 123);
         this.btnFlowChart2Send.Name = "btnFlowChart2Send";
         this.btnFlowChart2Send.Size = new System.Drawing.Size(134, 31);
         this.btnFlowChart2Send.TabIndex = 21;
         this.btnFlowChart2Send.Text = "Flow Chart2 Send";
         this.btnFlowChart2Send.UseVisualStyleBackColor = true;
         this.btnFlowChart2Send.Click += new System.EventHandler(this.btnFlowChart2Send_Click);
         // 
         // txtLog
         // 
         this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.txtLog.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.txtLog.Location = new System.Drawing.Point(0, 428);
         this.txtLog.Multiline = true;
         this.txtLog.Name = "txtLog";
         this.txtLog.Size = new System.Drawing.Size(649, 301);
         this.txtLog.TabIndex = 22;
         // 
         // lblStatus
         // 
         this.lblStatus.AutoSize = true;
         this.lblStatus.Location = new System.Drawing.Point(256, 15);
         this.lblStatus.Name = "lblStatus";
         this.lblStatus.Size = new System.Drawing.Size(48, 17);
         this.lblStatus.TabIndex = 23;
         this.lblStatus.Text = "Offline";
         // 
         // btnGetStatus
         // 
         this.btnGetStatus.BackColor = System.Drawing.Color.LawnGreen;
         this.btnGetStatus.Location = new System.Drawing.Point(17, 86);
         this.btnGetStatus.Name = "btnGetStatus";
         this.btnGetStatus.Size = new System.Drawing.Size(181, 31);
         this.btnGetStatus.TabIndex = 24;
         this.btnGetStatus.Text = "Get Stauts";
         this.btnGetStatus.UseVisualStyleBackColor = false;
         this.btnGetStatus.Click += new System.EventHandler(this.btnGetStatus_Click);
         // 
         // btnSetBarcodeContent
         // 
         this.btnSetBarcodeContent.Location = new System.Drawing.Point(12, 177);
         this.btnSetBarcodeContent.Name = "btnSetBarcodeContent";
         this.btnSetBarcodeContent.Size = new System.Drawing.Size(181, 31);
         this.btnSetBarcodeContent.TabIndex = 27;
         this.btnSetBarcodeContent.Text = "Set Barcode Content";
         this.btnSetBarcodeContent.UseVisualStyleBackColor = true;
         this.btnSetBarcodeContent.Click += new System.EventHandler(this.btnSetBarcodeContent_Click);
         // 
         // btnSetTextContent
         // 
         this.btnSetTextContent.Location = new System.Drawing.Point(12, 214);
         this.btnSetTextContent.Name = "btnSetTextContent";
         this.btnSetTextContent.Size = new System.Drawing.Size(181, 31);
         this.btnSetTextContent.TabIndex = 28;
         this.btnSetTextContent.Text = "Set Text Content";
         this.btnSetTextContent.UseVisualStyleBackColor = true;
         this.btnSetTextContent.Click += new System.EventHandler(this.btnSetTextContent_Click);
         // 
         // btnLoadFile
         // 
         this.btnLoadFile.Location = new System.Drawing.Point(12, 251);
         this.btnLoadFile.Name = "btnLoadFile";
         this.btnLoadFile.Size = new System.Drawing.Size(181, 31);
         this.btnLoadFile.TabIndex = 29;
         this.btnLoadFile.Text = "Load File";
         this.btnLoadFile.UseVisualStyleBackColor = true;
         this.btnLoadFile.Click += new System.EventHandler(this.btnLoadFile_Click);
         // 
         // btnSetLaserOffset
         // 
         this.btnSetLaserOffset.Location = new System.Drawing.Point(12, 288);
         this.btnSetLaserOffset.Name = "btnSetLaserOffset";
         this.btnSetLaserOffset.Size = new System.Drawing.Size(181, 31);
         this.btnSetLaserOffset.TabIndex = 30;
         this.btnSetLaserOffset.Text = "Set Laser Offset";
         this.btnSetLaserOffset.UseVisualStyleBackColor = true;
         this.btnSetLaserOffset.Click += new System.EventHandler(this.btnSetLaserOffset_Click);
         // 
         // txtBarcodeContent
         // 
         this.txtBarcodeContent.Location = new System.Drawing.Point(199, 181);
         this.txtBarcodeContent.Name = "txtBarcodeContent";
         this.txtBarcodeContent.Size = new System.Drawing.Size(132, 25);
         this.txtBarcodeContent.TabIndex = 31;
         this.txtBarcodeContent.Text = "barcode content";
         // 
         // txtTextContent
         // 
         this.txtTextContent.Location = new System.Drawing.Point(199, 218);
         this.txtTextContent.Name = "txtTextContent";
         this.txtTextContent.Size = new System.Drawing.Size(132, 25);
         this.txtTextContent.TabIndex = 32;
         this.txtTextContent.Text = "text content";
         // 
         // txtLoadFilePath
         // 
         this.txtLoadFilePath.Location = new System.Drawing.Point(199, 255);
         this.txtLoadFilePath.Name = "txtLoadFilePath";
         this.txtLoadFilePath.Size = new System.Drawing.Size(132, 25);
         this.txtLoadFilePath.TabIndex = 33;
         this.txtLoadFilePath.Text = "D:\\Laser.VLM";
         // 
         // txtOffset
         // 
         this.txtOffset.Location = new System.Drawing.Point(199, 292);
         this.txtOffset.Name = "txtOffset";
         this.txtOffset.Size = new System.Drawing.Size(132, 25);
         this.txtOffset.TabIndex = 34;
         this.txtOffset.Text = "0.1,0.2,0.3";
         // 
         // btnStartMonitoring
         // 
         this.btnStartMonitoring.Location = new System.Drawing.Point(12, 378);
         this.btnStartMonitoring.Name = "btnStartMonitoring";
         this.btnStartMonitoring.Size = new System.Drawing.Size(181, 31);
         this.btnStartMonitoring.TabIndex = 35;
         this.btnStartMonitoring.Text = "Start Moniting";
         this.btnStartMonitoring.UseVisualStyleBackColor = true;
         this.btnStartMonitoring.Click += new System.EventHandler(this.btnStartMonitoring_Click);
         // 
         // btnStopMonitoring
         // 
         this.btnStopMonitoring.Location = new System.Drawing.Point(199, 378);
         this.btnStopMonitoring.Name = "btnStopMonitoring";
         this.btnStopMonitoring.Size = new System.Drawing.Size(181, 31);
         this.btnStopMonitoring.TabIndex = 36;
         this.btnStopMonitoring.Text = "Stop Moniting";
         this.btnStopMonitoring.UseVisualStyleBackColor = true;
         this.btnStopMonitoring.Click += new System.EventHandler(this.btnStopMonitoring_Click);
         // 
         // ServerForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(649, 729);
         this.Controls.Add(this.btnStopMonitoring);
         this.Controls.Add(this.btnStartMonitoring);
         this.Controls.Add(this.txtOffset);
         this.Controls.Add(this.txtLoadFilePath);
         this.Controls.Add(this.txtTextContent);
         this.Controls.Add(this.txtBarcodeContent);
         this.Controls.Add(this.btnSetLaserOffset);
         this.Controls.Add(this.btnLoadFile);
         this.Controls.Add(this.btnSetTextContent);
         this.Controls.Add(this.btnSetBarcodeContent);
         this.Controls.Add(this.flowChart7);
         this.Controls.Add(this.flowChart6);
         this.Controls.Add(this.btnGetStatus);
         this.Controls.Add(this.lblStatus);
         this.Controls.Add(this.txtLog);
         this.Controls.Add(this.btnFlowChart2Send);
         this.Controls.Add(this.btnFlowStop);
         this.Controls.Add(this.flowChart5);
         this.Controls.Add(this.flowChart4);
         this.Controls.Add(this.flowChart3);
         this.Controls.Add(this.flowChart2);
         this.Controls.Add(this.flowChart1);
         this.Controls.Add(this.btnFlowStart);
         this.Controls.Add(this.txtRawMessage);
         this.Controls.Add(this.btnSendRawMessage);
         this.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
         this.Location = new System.Drawing.Point(100, 100);
         this.Name = "ServerForm";
         this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
         this.Text = "Server";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion
      private System.Windows.Forms.Button btnSendRawMessage;
      private System.Windows.Forms.TextBox txtRawMessage;
      private System.Windows.Forms.Button btnFlowStart;
      private GRT.SDK.Framework.FlowChart.FlowChart flowChart1;
      private GRT.SDK.Framework.FlowChart.FlowChart flowChart2;
      private GRT.SDK.Framework.FlowChart.FlowChart flowChart3;
      private GRT.SDK.Framework.FlowChart.FlowChart flowChart4;
      private GRT.SDK.Framework.FlowChart.FlowChart flowChart5;
      private System.Windows.Forms.Button btnFlowStop;
      private System.Windows.Forms.Button btnFlowChart2Send;
      private System.Windows.Forms.TextBox txtLog;
      private System.Windows.Forms.Label lblStatus;
      private System.Windows.Forms.Button btnGetStatus;
      private GRT.SDK.Framework.FlowChart.FlowChart flowChart6;
      private GRT.SDK.Framework.FlowChart.FlowChart flowChart7;
      private System.Windows.Forms.Button btnSetBarcodeContent;
      private System.Windows.Forms.Button btnSetTextContent;
      private System.Windows.Forms.Button btnLoadFile;
      private System.Windows.Forms.Button btnSetLaserOffset;
      private System.Windows.Forms.TextBox txtBarcodeContent;
      private System.Windows.Forms.TextBox txtTextContent;
      private System.Windows.Forms.TextBox txtLoadFilePath;
      private System.Windows.Forms.TextBox txtOffset;
      private System.Windows.Forms.Button btnStartMonitoring;
      private System.Windows.Forms.Button btnStopMonitoring;
   }
}

