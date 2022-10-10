
namespace OtoLehimlemeVeProgramlamaMakinesi
{
    partial class ModelForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModelForm));
			this.label1 = new System.Windows.Forms.Label();
			this.cmbBxModelF1 = new System.Windows.Forms.ComboBox();
			this.lblMessage = new System.Windows.Forms.Label();
			this.btnDataSendF1 = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Century Gothic", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.label1.Location = new System.Drawing.Point(75, 86);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(236, 23);
			this.label1.TabIndex = 11;
			this.label1.Text = "Fikstür-1 İçin Model Seç :";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// cmbBxModelF1
			// 
			this.cmbBxModelF1.Font = new System.Drawing.Font("Century Gothic", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.cmbBxModelF1.FormattingEnabled = true;
			this.cmbBxModelF1.Location = new System.Drawing.Point(343, 83);
			this.cmbBxModelF1.Name = "cmbBxModelF1";
			this.cmbBxModelF1.Size = new System.Drawing.Size(334, 30);
			this.cmbBxModelF1.TabIndex = 12;
			// 
			// lblMessage
			// 
			this.lblMessage.AutoSize = true;
			this.lblMessage.Font = new System.Drawing.Font("Century Gothic", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.lblMessage.ForeColor = System.Drawing.Color.Red;
			this.lblMessage.Location = new System.Drawing.Point(412, 19);
			this.lblMessage.Name = "lblMessage";
			this.lblMessage.Size = new System.Drawing.Size(170, 28);
			this.lblMessage.TabIndex = 13;
			this.lblMessage.Text = "Model Seçimi";
			this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btnDataSendF1
			// 
			this.btnDataSendF1.BackColor = System.Drawing.Color.Goldenrod;
			this.btnDataSendF1.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.btnDataSendF1.ForeColor = System.Drawing.Color.Black;
			this.btnDataSendF1.Location = new System.Drawing.Point(775, 61);
			this.btnDataSendF1.Name = "btnDataSendF1";
			this.btnDataSendF1.Size = new System.Drawing.Size(258, 73);
			this.btnDataSendF1.TabIndex = 14;
			this.btnDataSendF1.Text = "Verileri Gönder";
			this.btnDataSendF1.UseVisualStyleBackColor = false;
			this.btnDataSendF1.Click += new System.EventHandler(this.btnDataSend_Click);
			// 
			// btnDelete
			// 
			this.btnDelete.BackColor = System.Drawing.Color.Red;
			this.btnDelete.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.btnDelete.ForeColor = System.Drawing.Color.Black;
			this.btnDelete.Location = new System.Drawing.Point(775, 149);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(258, 73);
			this.btnDelete.TabIndex = 15;
			this.btnDelete.Text = "Model Sil";
			this.btnDelete.UseVisualStyleBackColor = false;
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// ModelForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Gainsboro;
			this.ClientSize = new System.Drawing.Size(1106, 234);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cmbBxModelF1);
			this.Controls.Add(this.lblMessage);
			this.Controls.Add(this.btnDataSendF1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "ModelForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Model Form1";
			this.Load += new System.EventHandler(this.ModelForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbBxModelF1;
        private System.Windows.Forms.Button btnDataSendF1;
		public System.Windows.Forms.Label lblMessage;
		private System.Windows.Forms.Button btnDelete;
	}
}