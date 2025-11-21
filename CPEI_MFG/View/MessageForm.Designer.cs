namespace CPEI_MFG
{
    partial class MessageForm
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
            this.lbMessage = new System.Windows.Forms.Label();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.btYes = new System.Windows.Forms.Button();
            this.btNo = new System.Windows.Forms.Button();
            this.btOk = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // lbMessage
            // 
            this.lbMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbMessage.AutoSize = true;
            this.lbMessage.Font = new System.Drawing.Font("STZhongsong", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMessage.Location = new System.Drawing.Point(12, 24);
            this.lbMessage.Name = "lbMessage";
            this.lbMessage.Size = new System.Drawing.Size(171, 55);
            this.lbMessage.TabIndex = 1;
            this.lbMessage.Text = "label1";
            // 
            // pictureBox
            // 
            this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox.BackColor = System.Drawing.Color.Cornsilk;
            this.pictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox.Location = new System.Drawing.Point(12, 82);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(878, 444);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 2;
            this.pictureBox.TabStop = false;
            // 
            // btYes
            // 
            this.btYes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btYes.BackColor = System.Drawing.Color.Cyan;
            this.btYes.Font = new System.Drawing.Font("Microsoft YaHei", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btYes.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btYes.Location = new System.Drawing.Point(22, 545);
            this.btYes.Margin = new System.Windows.Forms.Padding(4);
            this.btYes.Name = "btYes";
            this.btYes.Size = new System.Drawing.Size(176, 48);
            this.btYes.TabIndex = 0;
            this.btYes.Text = "YES";
            this.btYes.UseVisualStyleBackColor = false;
            this.btYes.Click += new System.EventHandler(this.btYes_Click);
            // 
            // btNo
            // 
            this.btNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btNo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btNo.Font = new System.Drawing.Font("Microsoft YaHei", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btNo.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btNo.Location = new System.Drawing.Point(693, 545);
            this.btNo.Margin = new System.Windows.Forms.Padding(4);
            this.btNo.Name = "btNo";
            this.btNo.Size = new System.Drawing.Size(185, 48);
            this.btNo.TabIndex = 6;
            this.btNo.Text = "NO";
            this.btNo.UseVisualStyleBackColor = false;
            this.btNo.Click += new System.EventHandler(this.btNo_Click);
            // 
            // btOk
            // 
            this.btOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btOk.BackColor = System.Drawing.Color.Cyan;
            this.btOk.Font = new System.Drawing.Font("Microsoft YaHei", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btOk.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btOk.Location = new System.Drawing.Point(362, 545);
            this.btOk.Margin = new System.Windows.Forms.Padding(4);
            this.btOk.Name = "btOk";
            this.btOk.Size = new System.Drawing.Size(184, 48);
            this.btOk.TabIndex = 2;
            this.btOk.Text = "OK";
            this.btOk.UseVisualStyleBackColor = false;
            this.btOk.Click += new System.EventHandler(this.btOk_Click);
            // 
            // MessageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(902, 606);
            this.ControlBox = false;
            this.Controls.Add(this.btYes);
            this.Controls.Add(this.btNo);
            this.Controls.Add(this.btOk);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.lbMessage);
            this.Font = new System.Drawing.Font("SimSun", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MessageForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lbMessage;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Button btYes;
        private System.Windows.Forms.Button btNo;
        private System.Windows.Forms.Button btOk;
    }
}