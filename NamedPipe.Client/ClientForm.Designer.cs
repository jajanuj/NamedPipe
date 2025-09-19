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
         this.btnEventMessage = new System.Windows.Forms.Button();
         this.btnRequestResponse = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // btnEventMessage
         // 
         this.btnEventMessage.Location = new System.Drawing.Point(81, 64);
         this.btnEventMessage.Name = "btnEventMessage";
         this.btnEventMessage.Size = new System.Drawing.Size(135, 32);
         this.btnEventMessage.TabIndex = 0;
         this.btnEventMessage.Text = "Event Message";
         this.btnEventMessage.UseVisualStyleBackColor = true;
         this.btnEventMessage.Click += new System.EventHandler(this.btnEventMessage_Click);
         // 
         // btnRequestResponse
         // 
         this.btnRequestResponse.Location = new System.Drawing.Point(81, 129);
         this.btnRequestResponse.Name = "btnRequestResponse";
         this.btnRequestResponse.Size = new System.Drawing.Size(135, 32);
         this.btnRequestResponse.TabIndex = 1;
         this.btnRequestResponse.Text = "Request / Response";
         this.btnRequestResponse.UseVisualStyleBackColor = true;
         this.btnRequestResponse.Click += new System.EventHandler(this.btnRequestResponse_Click);
         // 
         // ClientForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BackColor = System.Drawing.Color.AntiqueWhite;
         this.ClientSize = new System.Drawing.Size(535, 320);
         this.Controls.Add(this.btnRequestResponse);
         this.Controls.Add(this.btnEventMessage);
         this.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
         this.Name = "ClientForm";
         this.Text = "Client";
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.Button btnEventMessage;
      private System.Windows.Forms.Button btnRequestResponse;
   }
}

