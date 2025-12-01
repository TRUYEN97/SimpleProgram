using System;
using System.Windows.Forms;

namespace CPEI_MFG.View
{
    public partial class InputForm : Form
    {
        public InputForm(string title = "Input", bool isPassword = false)
        {
            InitializeComponent();
            Text = title;
            if (isPassword)
            {
                txtInput.PasswordChar = '*';
            }
        }
        public static bool GetInput(string title, string message, out string input)
        {
            return new InputForm(title).ShowInput(message, out input);
        }
        public static bool GetPassword(string title, string message, out string input)
        {
            return new InputForm(title, true).ShowInput(message, out input);
        }

        public bool ShowInput(string message, out string input)
        {
            lbMess.Text = message;
            input = null;
            if (ShowDialog() == DialogResult.OK)
            {
                input = txtInput.Text;
                return true;
            }
            return false;
        }

        private void btOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DialogResult = DialogResult.OK;
            }
        }
    }
}
