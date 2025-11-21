using System;
using System.Collections.Generic;

namespace CPEI_MFG.Config.Dhcp
{
    public class DhcpConfig
    {
        public bool IsEnable { get; set; } = false;
        public string ServerIp { get; set; } = "192.168.1.254";
        public string StartIp { get; set; } = "192.168.1.20";
        public string EndIp { get; set; } = "192.168.1.20";
        public string SubnetMask { get; set; } = "255.255.255.0";
        public int LeaseSeconds { get; set; } = 30;
        public List<StaticIpConfig> StaticIps { get; set; } = new List<StaticIpConfig>() { new StaticIpConfig { } };
    }
}
