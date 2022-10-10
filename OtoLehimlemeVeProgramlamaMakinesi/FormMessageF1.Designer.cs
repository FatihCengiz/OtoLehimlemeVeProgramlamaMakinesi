
namespace OtoLehimlemeVeProgramlamaMakinesi
{
    partial class FormMessageF1
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMessageF1));
			this.picMessage = new System.Windows.Forms.PictureBox();
			this.btnContinue = new System.Windows.Forms.Button();
			this.btnAgain = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.picMessage)).BeginInit();
			this.SuspendLayout();
			// 
			// picMessage
			// 
			this.picMessage.Image = ((System.Drawing.Image)(resources.GetObject("picMessage.Image")));
			this.picMessage.Location = new System.Drawing.Point(116, 13);
			this.picMessage.Margin = new System.Windows.Forms.Padding(0);
			this.picMessage.Name = "picMessage";
			this.picMessage.Size = new System.Drawing.Size(168, 171);
			this.picMessage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.picMessage.TabIndex = 16;
			this.picMessage.TabStop = false;
			// 
			// btnContinue
			// 
			this.btnContinue.BackColor = System.Drawing.Color.Red;
			this.btnContinue.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.btnContinue.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.btnContinue.Location = new System.Drawing.Point(588, 135);
			this.btnContinue.Name = "btnContinue";
			this.btnContinue.Size = new System.Drawing.Size(233, 61);
			this.btnContinue.TabIndex = 17;
			this.btnContinue.Text = "Sonraki PCB ye geç !";
			this.btnContinue.UseVisualStyleBackColor = false;
			this.btnContinue.Click += new System.EventHandler(this.btnContinue_Click);
			// 
			// btnAgain
			// 
			this.btnAgain.BackColor = System.Drawing.Color.Green;
			this.btnAgain.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.btnAgain.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.btnAgain.Location = new System.Drawing.Point(328, 135);
			this.btnAgain.Name = "btnAgain";
			this.btnAgain.Size = new System.Drawing.Size(219, 61);
			this.btnAgain.TabIndex = 18;
			this.btnAgain.Text = "Tekrar Dene";
			this.btnAgain.UseVisualStyleBackColor = false;
			this.btnAgain.Click += new System.EventHandler(this.btnAgain_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Font = new System.Drawing.Font("Century Gothic", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.label1.Location = new System.Drawing.Point(322, 40);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(499, 34);
			this.label1.TabIndex = 19;
			this.label1.Text = "FİKSTÜR-1 PROGRAM YÜKLENEMEDİ !";
			// 
			// FormMessageF1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.ClientSize = new System.Drawing.Size(962, 208);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnAgain);
			this.Controls.Add(this.btnContinue);
			this.Controls.Add(this.picMessage);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "FormMessageF1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "FormMessage";
			this.Load += new System.EventHandler(this.FormMessageF1_Load);
			((System.ComponentModel.ISupportInitialize)(this.picMessage)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picMessage;
        private System.Windows.Forms.Button btnContinue;
        private System.Windows.Forms.Button btnAgain;
        private System.Windows.Forms.Label label1;
    }
}