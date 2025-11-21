using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using CPEI_MFG.Common;

namespace CPEI_MFG.Service.DHCP
{
    internal class StaticIpManagement
    {
        private readonly ConcurrentDictionary<string, IPAddress> _staticMacIps;
        private readonly ConcurrentDictionary<uint, string> _staticIpMacs;

        public IReadOnlyDictionary<string, IPAddress> StaticMacIps => _staticMacIps;

        public StaticIpManagement()
        {
            _staticMacIps = new ConcurrentDictionary<string, IPAddress>();
            _staticIpMacs = new ConcurrentDictionary<uint, string>();
        }

        public event Func<uint, bool> IsCanAddIpEvent;
        public event Action<uint> OnRemoveIpEvent;
        public bool AddStaticIp(string mac, IPAddress ip)
        {
            if (string.IsNullOrWhiteSpace(mac) || ip == null) return false;

            mac = Util.NormalizeMac(mac);
            if (mac == null) return false;

            uint u = Util.ToUint(ip);
            if (_staticIpMacs.TryGetValue(u, out var oldMac))
            {
                RemoveStaticMac(oldMac);
            }
            if (_staticMacIps.TryGetValue(mac, out var oldIp))
            {
                RemoveStaticIp(oldIp);
            }
            if (IsCanAddIpEvent == null || IsCanAddIpEvent.Invoke(u))
            {
                _staticIpMacs[u] = mac;
                _staticMacIps[mac] = ip;
                return true;
            }
            return false;
        }

        public void RemoveStaticMac(string mac)
        {
            if (string.IsNullOrWhiteSpace(mac)) return;

            mac = Util.NormalizeMac(mac);
            if (mac == null) return;

            if (_staticMacIps.TryRemove(mac, out var ip))
            {
                uint u = Util.ToUint(ip);
                if (_staticIpMacs.TryRemove(u, out _))
                {
                    OnRemoveIpEvent?.Invoke(u);
                }
            }
        }

        public void RemoveStaticIp(IPAddress ip)
        {
            if (ip == null) return;
            uint u = Util.ToUint(ip);
            if (_staticIpMacs.TryRemove(u, out var mac))
            {
                OnRemoveIpEvent?.Invoke(u);
                _staticMacIps.TryRemove(mac, out _);
            }
        }
        public IPAddress GetIpByMac(string mac)
        {
            if (string.IsNullOrWhiteSpace(mac)) return null;
            mac = Util.NormalizeMac(mac);
            if (mac == null) return null;
            return _staticMacIps.TryGetValue(mac, out var ip) ? ip : null;
        }
        public string GetMacByIp(IPAddress ip)
        {
            if (ip == null) return null;
            uint u = Util.ToUint(ip);
            return _staticIpMacs.TryGetValue(u, out var mac) ? mac : null;
        }
        public bool TryGetIpByMac(string mac, out IPAddress ip)
        {
            ip = IPAddress.None;
            if (string.IsNullOrWhiteSpace(mac)) return false;
            mac = Util.NormalizeMac(mac);
            if (mac == null) return false;
            return _staticMacIps.TryGetValue(mac, out ip);
        }

        public bool TryGetMacByIp(IPAddress ip, out string mac)
        {
            mac = null;
            if (ip == null) return false;
            uint u = Util.ToUint(ip);
            return _staticIpMacs.TryGetValue(u, out mac);
        }

        public bool ContainsIp(IPAddress ip)
        {
            uint u = Util.ToUint(ip);
            return _staticIpMacs.ContainsKey(u);
        }

        public bool ContainsIp(uint u)
        {
            return _staticIpMacs.ContainsKey(u);
        }
    }
}
