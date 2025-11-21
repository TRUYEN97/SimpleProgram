using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Net.NetworkInformation;
namespace CPEI_MFG.Service.DHCP
{
    public class DhcpServer
    {
        private readonly IPAddress _serverIp;
        private readonly IPAddress _subnetMask;
        private readonly IPAddress _router;
        private readonly IPAddress _dns;

        private UdpClient _udp;
        private readonly IpManagement ipManagement;
        private Thread _thread;
        public event Action<string> DebugLogEvent;
        public DhcpServer(IPAddress serverIp, IPAddress poolStart, IPAddress poolEnd,
                          IPAddress subnetMask, IPAddress router, IPAddress dns,
                          int defaultLeaseSeconds = 3600)
        {
            _serverIp = serverIp;
            _subnetMask = subnetMask;
            _router = router;
            _dns = dns;
            ipManagement = new IpManagement(poolStart, poolEnd, defaultLeaseSeconds);
        }
        public void AddBanMac(string mac)
        {
            ipManagement.AddBanMac(mac);
        }
        public void AddStaticIp(string mac, IPAddress iPAddress)
        {
            ipManagement.AddStaticIp(mac, iPAddress);
        }
        public void RemoveStaticMac(string mac)
        {
            ipManagement.RemoveStaticMac(mac);
        }
        public void RemoveStaticIp(string ip)
        {
            ipManagement.RemoveStaticIp(IPAddress.Parse(ip));
        }
        public bool IsRunning => _thread?.IsAlive == true;

        private volatile bool _stop = false;
        public IReadOnlyDictionary<string, IPAddress> StaticMacIps => ipManagement.StaticMacIps;
        public void Start()
        {
            if (IsRunning)
            {
                return;
            }
            if (!Enumerable.Any(Enumerable.SelectMany(NetworkInterface.GetAllNetworkInterfaces(), (NetworkInterface i) => i.GetIPProperties().UnicastAddresses), (UnicastIPAddressInformation ip) => object.Equals(ip.Address, _serverIp)))
            {
                string text = $"Địa chỉ IP({_serverIp}) không tồn tại trên máy.";
                this.DebugLogEvent?.Invoke(text);
                throw new Exception(text);
            }
            DebugLogEvent?.Invoke($"DHCP server start. Server IP({_serverIp})");
            _udp = new UdpClient(new IPEndPoint(_serverIp, 67));
            _thread = new Thread(() =>
            {
                try
                {
                    IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                    _stop = false;
                    while (!_stop)
                    {
                        try
                        {
                            if (_udp.Available == 0)
                            {
                                Thread.Sleep(10);
                                continue;
                            }
                            byte[] data = _udp.Receive(ref remote);
                            DhcpMessage msg = DhcpMessage.Parse(data);
                            if (msg == null) continue;

                            if (msg.MessageType == 1) // DISCOVER
                            {
                                HandleDiscover(msg);
                            }
                            else if (msg.MessageType == 3) // REQUEST
                            {
                                HandleRequest(msg);
                            }
                        }
                        catch (SocketException)
                        {
                            break;
                        }
                        catch (ObjectDisposedException)
                        {
                            break;
                        }
                    }
                }
                finally
                {
                    try
                    {
                        _udp?.Close();
                    }
                    catch (Exception)
                    {
                    }
                    DebugLogEvent?.Invoke("DHCP server stopped.");
                }
            });
            _thread.Start();
        }
        public IReadOnlyDictionary<string, IPAddress> ActiveLeases => ipManagement.ActiveLeases;
        public void Stop()
        {
            _stop = true;
            if (_udp != null)
            {
                try
                {
                    _udp?.Close();
                }
                catch (Exception)
                {
                }
                _udp = null;
            }
        }

        public bool TryGetIp(string mac, out IPAddress ip)
        {
            return ipManagement.TryGetIp(mac, out ip);
        }

        private void HandleDiscover(DhcpMessage discover)
        {
            var mac = discover.MacAddress;
            if (ipManagement.IsBanMac(mac))
            {
                DebugLogEvent?.Invoke($"WARNING, '{mac}' in the ban list!");
            }
            if (ipManagement.TryAllocateIp(mac, out IPAddress ip))
            {
                var offer = discover.CreateReply(
                _serverIp, ip, _subnetMask, _router, _dns,
                2, ipManagement.DefaultLeaseSeconds); // 2 = DHCPOFFER
                _udp.Send(offer, offer.Length, new IPEndPoint(IPAddress.Broadcast, 68));
                DebugLogEvent?.Invoke($"OFFER, '{mac}' -> '{ip}'");
            }
            else
            {
                DebugLogEvent?.Invoke($"OFFER, '{mac}' -> out of IP to allocate!");
            }
        }

        private void HandleRequest(DhcpMessage request)
        {
            var mac = request.MacAddress;
            if (ipManagement.IsBanMac(mac))
            {
                DebugLogEvent?.Invoke($"WARNING, '{mac}' in the ban list!");
            }
            if (ipManagement.TryAllocateIp(mac, out IPAddress ip))
            {
                var ack = request.CreateReply(
                    _serverIp, ip, _subnetMask, _router, _dns,
                    5, ipManagement.DefaultLeaseSeconds); // 5 = DHCPACK

                _udp.Send(ack, ack.Length, new IPEndPoint(IPAddress.Broadcast, 68));
                DebugLogEvent?.Invoke($"ACK,  '{mac}' -> '{ip}'");
            }
            else
            {
                DebugLogEvent?.Invoke($"ACK,  '{mac}' -> out of IP to allocate!");
            }
        }

    }
}
