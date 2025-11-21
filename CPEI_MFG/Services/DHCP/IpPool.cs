using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using CPEI_MFG.Common;

namespace CPEI_MFG.Service.DHCP
{
    internal class IpPool
    {
        private readonly ConcurrentDictionary<string, LeaseModel> _activeLeases;
        private readonly ConcurrentDictionary<uint, LeaseModel> _staticLeases;
        private readonly ConcurrentDictionary<uint, LeaseModel> _freeLeases;
        private readonly Dictionary<uint, LeaseModel> _poolLeases;
        private readonly StaticIpManagement _staticIpManagement;
        private readonly object _lock = new object();
        public IpPool(IPAddress poolStart, IPAddress poolEnd, int defaultLeaseSeconds)
        {
            _activeLeases = new ConcurrentDictionary<string, LeaseModel>();
            _staticLeases = new ConcurrentDictionary<uint, LeaseModel>();
            _freeLeases = new ConcurrentDictionary<uint, LeaseModel>();
            _poolLeases = new Dictionary<uint, LeaseModel>();
            DefaultLeaseSeconds = defaultLeaseSeconds;
            _staticIpManagement = new StaticIpManagement();
            var poolStartU = Util.ToUint(poolStart);
            var poolEndU = Util.ToUint(poolEnd);
            for (uint u = poolStartU; u <= poolEndU; u++)
            {
                var newLease = new LeaseModel { Ip = Util.FromUint(u), UIp = u, ExpireAt = DateTime.MinValue };
                if (_staticIpManagement.ContainsIp(u))
                {
                    _staticLeases[u] = newLease;
                }
                else
                {
                    _freeLeases[u] = newLease;
                }
                _poolLeases[u] = newLease;
            }
            _staticIpManagement.IsCanAddIpEvent += u =>
            {
                lock (_lock)
                {
                    if (!_staticLeases.ContainsKey(u) && _poolLeases.ContainsKey(u))
                    {
                        if (!_freeLeases.TryRemove(u, out var lease))
                        {
                            return false;
                        }
                        lease.ExpireAt = DateTime.MinValue;
                        _staticLeases[u] = lease;
                    }
                    return true;
                }
            };
            _staticIpManagement.OnRemoveIpEvent += u =>
            {
                lock (_lock)
                {
                    if (!_poolLeases.ContainsKey(u))
                    {
                        return;
                    }
                    if (_staticLeases.TryRemove(u, out var lease))
                    {
                        lease.ExpireAt = DateTime.MinValue;
                        _freeLeases[u] = lease;
                    }
                }
            };
        }
        public int DefaultLeaseSeconds { get; private set; }
        public IReadOnlyDictionary<string, IPAddress> ActiveLeases => _activeLeases.ToDictionary(i => i.Key, i => i.Value.Ip);
        public bool TryGetFreeIp(string mac, out IPAddress ip)
        {
            lock (_lock)
            {
                ip = null;
                if (_staticIpManagement.TryGetIpByMac(mac, out ip))
                {
                    return true;
                }
                if (_activeLeases.TryGetValue(mac, out var lease))
                {
                    if (!lease.IsExpire)
                    {
                        ip = _activeLeases[mac].Ip;
                        return true;
                    }
                    ReleaseIp(mac);
                }
                if (_freeLeases.Count > 0)
                {
                    var freeLease = _freeLeases.First().Value;
                    freeLease.ExpireAt = DateTime.Now.AddSeconds(DefaultLeaseSeconds);
                    _activeLeases[mac] = freeLease;
                    ip = freeLease.Ip;
                    return true;
                }
                return false;
            }
        }
        public bool ReleaseIp(string mac)
        {
            if (string.IsNullOrWhiteSpace(mac))
                return false;

            mac = Util.NormalizeMac(mac);
            if (mac == null) return false;
            if (_activeLeases.TryRemove(mac, out var lease))
            {
                lease.ExpireAt = DateTime.MinValue;
                if (_staticIpManagement.ContainsIp(lease.UIp))
                {
                    _staticLeases[lease.UIp] = lease;
                }
                else
                {
                    _freeLeases[lease.UIp] = lease;
                }
                return true;
            }
            return false;
        }
        public IReadOnlyDictionary<string, IPAddress> StaticMacIps => _staticIpManagement.StaticMacIps;

        public void AddStaticIp(string mac, IPAddress iPAddress)
        {
            _staticIpManagement.AddStaticIp(mac, iPAddress);
        }

        public void RemoveStaticMac(string mac)
        {
            _staticIpManagement.RemoveStaticMac(mac);
        }

        public void RemoveStaticIp(IPAddress iPAddress)
        {
            _staticIpManagement.RemoveStaticIp(iPAddress);
        }

        public bool TryGetIp(string mac, out IPAddress ip)
        {
            ip = null;
            if (_activeLeases.TryGetValue(mac, out var lease))
            {
                ip = lease.Ip;
                return true;
            }
            return false;
        }
    }
}
