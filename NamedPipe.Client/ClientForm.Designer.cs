namespace NamedPipe.Client
{
   partial class ClientForm
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
         this.components = new System.ComponentModel.Container();
         this.txtLog = new System.Windows.Forms.TextBox();
         this.btnResponseStatus = new System.Windows.Forms.Button();
         this.btnSendRawMessage = new System.Windows.Forms.Button();
         this.txtRawMessage = new System.Windows.Forms.TextBox();
         this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
         this.SuspendLayout();
         // 
         // txtLog
         // 
         this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.txtLog.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.txtLog.Location = new System.Drawing.Point(0, 136);
         this.txtLog.Multiline = true;
         this.txtLog.Name = "txtLog";
         this.txtLog.Size = new System.Drawing.Size(535, 329);
         this.txtLog.TabIndex = 2;
         // 
         // btnResponseStatus
         // 
         this.btnResponseStatus.BackColor = System.Drawing.Color.LawnGreen;
         this.btnResponseStatus.Location = new System.Drawing.Point(12, 45);
         this.btnResponseStatus.Name = "btnResponseStatus";
         this.btnResponseStatus.Size = new System.Drawing.Size(181, 31);
         this.btnResponseStatus.TabIndex = 3;
         this.btnResponseStatus.Text = "Response Status";
         this.btnResponseStatus.UseVisualStyleBackColor = false;
         this.btnResponseStatus.Click += new System.EventHandler(this.btnResponseStatus_Click);
         // 
         // btnSendRawMessage
         // 
         this.btnSendRawMessage.Location = new System.Drawing.Point(12, 8);
         this.btnSendRawMessage.Name = "btnSendRawMessage";
         this.btnSendRawMessage.Size = new System.Drawing.Size(181, 31);
         this.btnSendRawMessage.TabIndex = 4;
         this.btnSendRawMessage.Text = "Send Raw Message";
         this.btnSendRawMessage.UseVisualStyleBackColor = true;
         this.btnSendRawMessage.Click += new System.EventHandler(this.btnSendRawMessage_Click);
         // 
         // txtRawMessage
         // 
         this.txtRawMessage.Location = new System.Drawing.Point(199, 12);
         this.txtRawMessage.Name = "txtRawMessage";
         this.txtRawMessage.Size = new System.Drawing.Size(140, 25);
         this.txtRawMessage.TabIndex = 5;
         this.txtRawMessage.Text = "Hello Server";
         // 
         // contextMenuStrip1
         // 
         this.contextMenuStrip1.Name = "contextMenuStrip1";
         this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
         // 
         // ClientForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BackColor = System.Drawing.Color.AntiqueWhite;
         this.ClientSize = new System.Drawing.Size(535, 465);
         this.Controls.Add(this.txtRawMessage);
         this.Controls.Add(this.btnSendRawMessage);
         this.Controls.Add(this.btnResponseStatus);
         this.Controls.Add(this.txtLog);
         this.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
         this.Location = new System.Drawing.Point(900, 300);
         this.Name = "ClientForm";
         this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
         this.Text = "Client";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion
      private System.Windows.Forms.TextBox txtLog;
      private System.Windows.Forms.Button btnResponseStatus;
      private System.Windows.Forms.Button btnSendRawMessage;
      private System.Windows.Forms.TextBox txtRawMessage;
      private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
   }
}

