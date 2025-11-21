using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CPEI_MFG.Config;

namespace CPEI_MFG
{
    public partial class MessageForm : Form
    {
        public MessageForm()
        {
            InitializeComponent();
        }
        public bool ShowMessage(string messgage, string picPath, bool isYesNoType)
        {
            TopMost = true;
            if (File.Exists(picPath))
            {
                try
                {
                    pictureBox.Image = Image.FromFile(picPath);
                }
                catch (Exception)
                {
                }
            }
            lbMessage.Text = messgage;
            btOk.Visible = !isYesNoType;
            btYes.Visible = isYesNoType;
            btNo.Visible = isYesNoType;
            return ShowDialog() == DialogResult.OK;
        }

        private void btYes_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btNo_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
        }
    }
}
