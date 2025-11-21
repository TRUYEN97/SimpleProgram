using System;
using System.Windows.Forms;
using CPEI_MFG.Config.Dhcp;
using CPEI_MFG.Services.DHCP;

namespace CPEI_MFG.View
{
    public partial class DhcpControl : UserControl
    {
        public DhcpControl()
        {
            InitializeComponent();
        }

        public Dhcp Dhcp {  get; set; }

        public void SetConfig(DhcpConfig config)
        {
            if (config == null) return;
            txtDhcpStatus.Text = config.IsEnable.ToString();
            txtServerIp.Text = config.ServerIp;
            txtStartIp.Text = config.StartIp;
            txtEndIp.Text = config.EndIp;
            txtSubnetMask.Text = config.SubnetMask;
            txtLease.Text = config.LeaseSeconds.ToString();
        }
        public void AddDhcpLog(string line)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("ACK"))
                {
                    return;
                }
                if (txtDhcpLog.InvokeRequired)
                {
                    Invoke(new Action<string>(AddDhcpLog), line);
                }
                else
                {
                    txtDhcpLog.AppendText($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}  {line}\r\n");
                    txtDhcpLog.ScrollToCaret();
                }
            }
            catch (Exception)
            {
            }
        }
        public void RefreshStaticIp()
        {
            if (Dhcp == null)
            {
                return;
            }
            listStaticIps.Items.Clear();
            foreach (var item in Dhcp.StaticMacIps)
            {
                string macIpStr = $"{item.Key} ; {item.Value}";
                listStaticIps.Items.Add(macIpStr);
            }
            Dhcp.SaveStaticIp();
        }

        private void btDeleteStaticAdd_Click(object sender, EventArgs e)
        {
            var item = listStaticIps.SelectedItem;
            if (Dhcp == null || item == null)
            {
                return;
            }
            listStaticIps.Items.Remove(item);
            Dhcp.Delete(item.ToString());
            RefreshStaticIp();
        }

        private void btAddStaticAdd_Click(object sender, EventArgs e)
        {
            if (Dhcp == null)
            {
                return;
            }
            Dhcp.AddStaticIp(txtStaticAddr.Text.Trim());
            txtStaticAddr.Text = null;
            RefreshStaticIp();
        }
    }
}
