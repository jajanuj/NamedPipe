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
         this.btnServerClose = new System.Windows.Forms.Button();
         this.btnSendRawMessage = new System.Windows.Forms.Button();
         this.txtRawMessage = new System.Windows.Forms.TextBox();
         this.btnFlowStart = new System.Windows.Forms.Button();
         this.flowChart1 = new GRT.SDK.Framework.FlowChart.FlowChart();
         this.flowChart2 = new GRT.SDK.Framework.FlowChart.FlowChart();
         this.flowChart3 = new GRT.SDK.Framework.FlowChart.FlowChart();
         this.flowChart4 = new GRT.SDK.Framework.FlowChart.FlowChart();
         this.flowChart5 = new GRT.SDK.Framework.FlowChart.FlowChart();
         this.btnFlowStop = new System.Windows.Forms.Button();
         this.btnFlowChart2Send = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // btnServerClose
         // 
         this.btnServerClose.Location = new System.Drawing.Point(81, 58);
         this.btnServerClose.Name = "btnServerClose";
         this.btnServerClose.Size = new System.Drawing.Size(157, 29);
         this.btnServerClose.TabIndex = 0;
         this.btnServerClose.Text = "Server Close";
         this.btnServerClose.UseVisualStyleBackColor = true;
         this.btnServerClose.Click += new System.EventHandler(this.btnServerClose_Click);
         // 
         // btnSendRawMessage
         // 
         this.btnSendRawMessage.Location = new System.Drawing.Point(81, 108);
         this.btnSendRawMessage.Name = "btnSendRawMessage";
         this.btnSendRawMessage.Size = new System.Drawing.Size(157, 29);
         this.btnSendRawMessage.TabIndex = 1;
         this.btnSendRawMessage.Text = "Send Raw Message";
         this.btnSendRawMessage.UseVisualStyleBackColor = true;
         this.btnSendRawMessage.Click += new System.EventHandler(this.btnSendRawMessage_Click);
         // 
         // txtRawMessage
         // 
         this.txtRawMessage.Location = new System.Drawing.Point(259, 108);
         this.txtRawMessage.Name = "txtRawMessage";
         this.txtRawMessage.Size = new System.Drawing.Size(158, 25);
         this.txtRawMessage.TabIndex = 2;
         // 
         // btnFlowStart
         // 
         this.btnFlowStart.Location = new System.Drawing.Point(81, 157);
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
         this.flowChart1.Location = new System.Drawing.Point(220, 176);
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
         this.flowChart2.Location = new System.Drawing.Point(220, 233);
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
         this.flowChart3.Location = new System.Drawing.Point(220, 290);
         this.flowChart3.Margin = new System.Windows.Forms.Padding(2);
         this.flowChart3.Name = "flowChart3";
         this.flowChart3.Next = this.flowChart4;
         this.flowChart3.Size = new System.Drawing.Size(167, 32);
         this.flowChart3.TabIndex = 17;
         this.flowChart3.Run += new GRT.SDK.Framework.FlowChart.FlowChart.ReturnType(this.flowChart3_Run);
         // 
         // flowChart4
         // 
         this.flowChart4.BackColor = System.Drawing.Color.LightSalmon;
         this.flowChart4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.flowChart4.Caption = "-";
         this.flowChart4.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Bold);
         this.flowChart4.Location = new System.Drawing.Point(417, 290);
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
         this.flowChart5.Location = new System.Drawing.Point(417, 233);
         this.flowChart5.Margin = new System.Windows.Forms.Padding(2);
         this.flowChart5.Name = "flowChart5";
         this.flowChart5.Next = this.flowChart2;
         this.flowChart5.Size = new System.Drawing.Size(40, 32);
         this.flowChart5.TabIndex = 19;
         this.flowChart5.Run += new GRT.SDK.Framework.FlowChart.FlowChart.ReturnType(this.flowChart5_Run);
         // 
         // btnFlowStop
         // 
         this.btnFlowStop.Location = new System.Drawing.Point(81, 194);
         this.btnFlowStop.Name = "btnFlowStop";
         this.btnFlowStop.Size = new System.Drawing.Size(103, 31);
         this.btnFlowStop.TabIndex = 20;
         this.btnFlowStop.Text = "Flow Stop";
         this.btnFlowStop.UseVisualStyleBackColor = true;
         this.btnFlowStop.Click += new System.EventHandler(this.btnFlowStop_Click);
         // 
         // btnFlowChart2Send
         // 
         this.btnFlowChart2Send.Location = new System.Drawing.Point(50, 234);
         this.btnFlowChart2Send.Name = "btnFlowChart2Send";
         this.btnFlowChart2Send.Size = new System.Drawing.Size(134, 31);
         this.btnFlowChart2Send.TabIndex = 21;
         this.btnFlowChart2Send.Text = "Flow Chart2 Send";
         this.btnFlowChart2Send.UseVisualStyleBackColor = true;
         this.btnFlowChart2Send.Click += new System.EventHandler(this.btnFlowChart2Send_Click);
         // 
         // ServerForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(519, 492);
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
         this.Controls.Add(this.btnServerClose);
         this.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
         this.Name = "ServerForm";
         this.Text = "Server";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Button btnServerClose;
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
   }
}

