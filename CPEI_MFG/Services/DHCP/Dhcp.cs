using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows.Forms;
using CPEI_MFG.Service;
using CPEI_MFG.Service.DHCP;
using CPEI_MFG.Config;
using CPEI_MFG.View;
using CPEI_MFG.Common;

namespace CPEI_MFG.Services.DHCP
{
    public class Dhcp
    {
        private DhcpServer _dhcpServer;
        public Dhcp()
        {
            DhcpControl = new DhcpControl();
            WriteInfoLog += DhcpControl.AddDhcpLog;
        }
        public void AddStaticIp(string mac, IPAddress ip)
        {
            _dhcpServer?.AddStaticIp(mac, ip);
        }
        public DhcpControl DhcpControl { get; }
        public bool IsRunning => _dhcpServer?.IsRunning == true;
        public event Action<string> WriteInfoLog;
        public void AddStaticIp(string cf)
        {
            if (string.IsNullOrWhiteSpace(cf))
            {
                return;
            }
            string[] elems = cf.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (elems.Length < 2)
            {
                return;
            }
            DhcpControl?.AddDhcpLog(cf);
            _dhcpServer?.AddStaticIp(elems[0]?.Trim(), IPAddress.Parse(elems[1]?.Trim()));
        }
        public bool Init()
        {
            try
            {
                var dhcpConfig = ConfigLoader.ProgramConfig.DhcpConfig;
                if (dhcpConfig == null)
                {
                    return false;
                }
                var serverIp = IPAddress.Parse(dhcpConfig.ServerIp?.Trim());
                _dhcpServer = new DhcpServer(serverIp,
                    IPAddress.Parse(dhcpConfig.StartIp?.Trim()),
                    IPAddress.Parse(dhcpConfig.EndIp?.Trim()),
                    IPAddress.Parse(dhcpConfig.SubnetMask?.Trim()),
                    serverIp, IPAddress.Parse("8.8.8.8"),
                    dhcpConfig.LeaseSeconds);
                _dhcpServer.DebugLogEvent += WriteInfoLog;
                if (dhcpConfig.IsEnable)
                {
                    try
                    {
                        if (dhcpConfig.StaticIps?.Count > 0)
                        {
                            foreach (var item in dhcpConfig.StaticIps)
                            {
                                var ip = item.Ip?.Trim();
                                var mac = item.Mac?.Trim();
                                if (!string.IsNullOrWhiteSpace(ip) && !string.IsNullOrWhiteSpace(mac))
                                {
                                    _dhcpServer?.AddStaticIp(mac, IPAddress.Parse(ip));
                                }
                            }
                        }
                        _dhcpServer.Start();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return false;
                    }
                }
                string[] lines = FileUtil.ReadAllLines("C:/DHCP/StaticMacIp.txt");
                if (lines != null)
                {
                    foreach (var line in lines)
                    {
                        AddStaticIp(line);
                    }
                }
                SaveStaticIp();
                DhcpControl.SetConfig(dhcpConfig);
                DhcpControl.RefreshStaticIp();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public void SaveStaticIp()
        {
            if (StaticMacIps == null) return;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in StaticMacIps)
            {
                string macIpStr = $"{item.Key} ; {item.Value}";
                stringBuilder.AppendLine(macIpStr);
            }
            FileUtil.WriteAllText("C:/DHCP/StaticMacIp.txt", stringBuilder.ToString());
        }
        public IReadOnlyDictionary<string, IPAddress> StaticMacIps => _dhcpServer?.StaticMacIps;

        public void Stop()
        {
            _dhcpServer?.Stop();
        }

        internal void Delete(string cf)
        {
            if (string.IsNullOrWhiteSpace(cf))
            {
                return;
            }
            string[] elems = cf.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (elems.Length < 2)
            {
                return;
            }
            DhcpControl?.AddDhcpLog(cf);
            _dhcpServer.RemoveStaticMac(elems[0]);
        }
    }
}
