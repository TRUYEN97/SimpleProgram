using System;
using System.Collections.Generic;
using System.Net;
using CPEI_MFG.Common;

namespace CPEI_MFG.Service.DHCP
{
    public class IpManagement
    {
        private readonly IpPool ipPool;
        private readonly HashSet<string> _banMacs;
        private readonly object _lock = new object();

        public int DefaultLeaseSeconds => ipPool.DefaultLeaseSeconds;

        public IpManagement(IPAddress poolStart, IPAddress poolEnd, int defaultLeaseSeconds)
        {
            ipPool = new IpPool(poolStart, poolEnd, defaultLeaseSeconds);
            _banMacs = new HashSet<string>();
        }
        public void AddBanMac(string mac)
        {
            lock (_lock)
            {
                if (string.IsNullOrWhiteSpace(mac))
                {
                    return;
                }
                mac = Util.NormalizeMac(mac);
                if (mac == null)
                {
                    return;
                }
                _banMacs.Add(mac);
            }
        }
        public IReadOnlyDictionary<string, IPAddress> ActiveLeases => ipPool.ActiveLeases;
        public void RemoveBanMac(string mac)
        {
            lock (_lock)
            {
                if (string.IsNullOrWhiteSpace(mac))
                {
                    return;
                }
                mac = Util.NormalizeMac(mac);
                if (mac == null)
                {
                    return;
                }
                _banMacs.Remove(mac);
            }
        }
        public bool IsBanMac(string mac)
        {
            lock (_lock)
            {
                if (string.IsNullOrWhiteSpace(mac))
                {
                    return false;
                }
                mac = Util.NormalizeMac(mac);
                if (mac == null)
                {
                    return false;
                }
                return _banMacs.Contains(mac);
            }
        }

        public bool TryAllocateIp(string mac, out IPAddress ip)
        {
            lock (_lock)
            {
                ip = null;
                if (string.IsNullOrWhiteSpace(mac))
                {
                    return false;
                }
                mac = Util.NormalizeMac(mac);
                if (mac == null)
                {
                    return false;
                }
                if (_banMacs.Contains(mac))
                {
                    return false;
                }
                return ipPool.TryGetFreeIp(mac, out ip);
            }
        }
        public IReadOnlyDictionary<string, IPAddress> StaticMacIps => ipPool.StaticMacIps;

        public void AddStaticIp(string mac, IPAddress iPAddress)
        {
            ipPool.AddStaticIp(mac, iPAddress);
        }
        public void RemoveStaticMac(string mac)
        {
            ipPool.RemoveStaticMac(mac);
        }
        public void RemoveStaticIp(IPAddress iPAddress)
        {
            ipPool.RemoveStaticIp(iPAddress);
        }

        public bool TryGetIp(string mac, out IPAddress ip)
        {
            return ipPool.TryGetIp(mac,out ip);
        }
    }
}
