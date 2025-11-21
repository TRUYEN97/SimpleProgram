namespace CPEI_MFG.View
{
    partial class DhcpControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDhcpLog = new System.Windows.Forms.RichTextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label26 = new System.Windows.Forms.Label();
            this.txtStaticAddr = new System.Windows.Forms.TextBox();
            this.btDeleteStaticAdd = new System.Windows.Forms.Button();
            this.btAddStaticAdd = new System.Windows.Forms.Button();
            this.label23 = new System.Windows.Forms.Label();
            this.listStaticIps = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtDhcpStatus = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.txtLease = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.txtSubnetMask = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.txtEndIp = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtStartIp = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtServerIp = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackColor = System.Drawing.Color.Wheat;
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.txtDhcpLog);
            this.panel3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.panel3.Location = new System.Drawing.Point(3, 201);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(670, 194);
            this.panel3.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Log";
            // 
            // txtDhcpLog
            // 
            this.txtDhcpLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDhcpLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDhcpLog.Location = new System.Drawing.Point(6, 21);
            this.txtDhcpLog.Name = "txtDhcpLog";
            this.txtDhcpLog.ReadOnly = true;
            this.txtDhcpLog.Size = new System.Drawing.Size(652, 161);
            this.txtDhcpLog.TabIndex = 0;
            this.txtDhcpLog.Text = "";
            this.txtDhcpLog.WordWrap = false;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Wheat;
            this.panel2.Controls.Add(this.label26);
            this.panel2.Controls.Add(this.txtStaticAddr);
            this.panel2.Controls.Add(this.btDeleteStaticAdd);
            this.panel2.Controls.Add(this.btAddStaticAdd);
            this.panel2.Controls.Add(this.label23);
            this.panel2.Controls.Add(this.listStaticIps);
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(298, 192);
            this.panel2.TabIndex = 9;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(6, 146);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(40, 13);
            this.label26.TabIndex = 15;
            this.label26.Text = "Mac;Ip";
            this.label26.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // txtStaticAddr
            // 
            this.txtStaticAddr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStaticAddr.Location = new System.Drawing.Point(6, 162);
            this.txtStaticAddr.Name = "txtStaticAddr";
            this.txtStaticAddr.Size = new System.Drawing.Size(180, 20);
            this.txtStaticAddr.TabIndex = 14;
            // 
            // btDeleteStaticAdd
            // 
            this.btDeleteStaticAdd.Location = new System.Drawing.Point(243, 161);
            this.btDeleteStaticAdd.Name = "btDeleteStaticAdd";
            this.btDeleteStaticAdd.Size = new System.Drawing.Size(49, 23);
            this.btDeleteStaticAdd.TabIndex = 3;
            this.btDeleteStaticAdd.Text = "Delete";
            this.btDeleteStaticAdd.UseVisualStyleBackColor = true;
            this.btDeleteStaticAdd.Click += new System.EventHandler(this.btDeleteStaticAdd_Click);
            // 
            // btAddStaticAdd
            // 
            this.btAddStaticAdd.Location = new System.Drawing.Point(189, 161);
            this.btAddStaticAdd.Name = "btAddStaticAdd";
            this.btAddStaticAdd.Size = new System.Drawing.Size(48, 23);
            this.btAddStaticAdd.TabIndex = 2;
            this.btAddStaticAdd.Text = "Add";
            this.btAddStaticAdd.UseVisualStyleBackColor = true;
            this.btAddStaticAdd.Click += new System.EventHandler(this.btAddStaticAdd_Click);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(6, 10);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(51, 13);
            this.label23.TabIndex = 1;
            this.label23.Text = "Static Ips";
            // 
            // listStaticIps
            // 
            this.listStaticIps.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listStaticIps.FormattingEnabled = true;
            this.listStaticIps.ItemHeight = 20;
            this.listStaticIps.Location = new System.Drawing.Point(6, 29);
            this.listStaticIps.Name = "listStaticIps";
            this.listStaticIps.Size = new System.Drawing.Size(286, 104);
            this.listStaticIps.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.Wheat;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.txtDhcpStatus);
            this.panel1.Controls.Add(this.label24);
            this.panel1.Controls.Add(this.txtLease);
            this.panel1.Controls.Add(this.label22);
            this.panel1.Controls.Add(this.txtSubnetMask);
            this.panel1.Controls.Add(this.label20);
            this.panel1.Controls.Add(this.txtEndIp);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.txtStartIp);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.txtServerIp);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(307, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(366, 192);
            this.panel1.TabIndex = 8;
            // 
            // txtDhcpStatus
            // 
            this.txtDhcpStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDhcpStatus.Location = new System.Drawing.Point(79, 20);
            this.txtDhcpStatus.Name = "txtDhcpStatus";
            this.txtDhcpStatus.ReadOnly = true;
            this.txtDhcpStatus.Size = new System.Drawing.Size(274, 20);
            this.txtDhcpStatus.TabIndex = 13;
            // 
            // label24
            // 
            this.label24.Location = new System.Drawing.Point(3, 22);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(70, 18);
            this.label24.TabIndex = 12;
            this.label24.Text = "Status";
            this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtLease
            // 
            this.txtLease.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLease.Location = new System.Drawing.Point(79, 150);
            this.txtLease.Name = "txtLease";
            this.txtLease.ReadOnly = true;
            this.txtLease.Size = new System.Drawing.Size(274, 20);
            this.txtLease.TabIndex = 11;
            // 
            // label22
            // 
            this.label22.Location = new System.Drawing.Point(4, 152);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(69, 18);
            this.label22.TabIndex = 10;
            this.label22.Text = "Lease (S)";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtSubnetMask
            // 
            this.txtSubnetMask.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSubnetMask.Location = new System.Drawing.Point(79, 124);
            this.txtSubnetMask.Name = "txtSubnetMask";
            this.txtSubnetMask.ReadOnly = true;
            this.txtSubnetMask.Size = new System.Drawing.Size(274, 20);
            this.txtSubnetMask.TabIndex = 9;
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(4, 126);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(69, 18);
            this.label20.TabIndex = 8;
            this.label20.Text = "Subnet mask";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtEndIp
            // 
            this.txtEndIp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEndIp.Location = new System.Drawing.Point(79, 98);
            this.txtEndIp.Name = "txtEndIp";
            this.txtEndIp.ReadOnly = true;
            this.txtEndIp.Size = new System.Drawing.Size(274, 20);
            this.txtEndIp.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(4, 100);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 18);
            this.label5.TabIndex = 6;
            this.label5.Text = "End IP";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtStartIp
            // 
            this.txtStartIp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStartIp.Location = new System.Drawing.Point(79, 72);
            this.txtStartIp.Name = "txtStartIp";
            this.txtStartIp.ReadOnly = true;
            this.txtStartIp.Size = new System.Drawing.Size(274, 20);
            this.txtStartIp.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(4, 74);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 18);
            this.label4.TabIndex = 4;
            this.label4.Text = "Start IP";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtServerIp
            // 
            this.txtServerIp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServerIp.Location = new System.Drawing.Point(79, 46);
            this.txtServerIp.Name = "txtServerIp";
            this.txtServerIp.ReadOnly = true;
            this.txtServerIp.Size = new System.Drawing.Size(274, 20);
            this.txtServerIp.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(4, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 18);
            this.label3.TabIndex = 0;
            this.label3.Text = "Server IP";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DhcpControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "DhcpControl";
            this.Size = new System.Drawing.Size(676, 399);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox txtDhcpLog;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox txtStaticAddr;
        private System.Windows.Forms.Button btDeleteStaticAdd;
        private System.Windows.Forms.Button btAddStaticAdd;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.ListBox listStaticIps;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtDhcpStatus;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox txtLease;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox txtSubnetMask;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txtEndIp;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtStartIp;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtServerIp;
        private System.Windows.Forms.Label label3;
    }
}
