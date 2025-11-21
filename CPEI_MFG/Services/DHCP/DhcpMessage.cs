using System;
using System.Linq;
using System.Net;

namespace CPEI_MFG.Service.DHCP
{
    public class DhcpMessage
    {
        public byte Op;
        public byte Htype;
        public byte Hlen;
        public byte Hops;
        public uint Xid;
        public ushort Secs;
        public ushort Flags;
        public IPAddress Ciaddr;
        public IPAddress Yiaddr;
        public IPAddress Siaddr;
        public IPAddress Giaddr;
        public string MacAddress;
        public byte MessageType;

        public static DhcpMessage Parse(byte[] buf)
        {
            if (buf.Length < 240) return null;
            var msg = new DhcpMessage
            {
                Op = buf[0],
                Htype = buf[1],
                Hlen = buf[2],
                Xid = BitConverter.ToUInt32(buf.Skip(4).Take(4).Reverse().ToArray(), 0),
                Ciaddr = new IPAddress(buf.Skip(12).Take(4).ToArray()),
                Yiaddr = new IPAddress(buf.Skip(16).Take(4).ToArray()),
                Siaddr = new IPAddress(buf.Skip(20).Take(4).ToArray()),
                Giaddr = new IPAddress(buf.Skip(24).Take(4).ToArray())
            };

            var mac = buf.Skip(28).Take(msg.Hlen).ToArray();
            msg.MacAddress = BitConverter.ToString(mac);

            // Magic cookie at 236–239
            int optIndex = 240;
            while (optIndex < buf.Length)
            {
                byte code = buf[optIndex++];
                if (code == 255) break;
                byte len = buf[optIndex++];
                if (code == 53) // DHCP Message Type
                    msg.MessageType = buf[optIndex];
                optIndex += len;
            }
            return msg;
        }

        public byte[] CreateReply(IPAddress serverIp, IPAddress yiaddr, IPAddress mask, IPAddress router, IPAddress dns,
                                  byte msgType, int leaseSeconds)
        {
            byte[] buf = new byte[300];
            buf[0] = 2; // BOOTREPLY
            buf[1] = Htype;
            buf[2] = Hlen;
            Array.Copy(BitConverter.GetBytes(Xid).Reverse().ToArray(), 0, buf, 4, 4);
            Array.Copy(yiaddr.GetAddressBytes(), 0, buf, 16, 4);
            Array.Copy(serverIp.GetAddressBytes(), 0, buf, 20, 4);
            var macBytes = MacAddress.Split('-').Select(b => Convert.ToByte(b, 16)).ToArray();
            Array.Copy(macBytes, 0, buf, 28, macBytes.Length);

            // Magic cookie
            buf[236] = 99; buf[237] = 130; buf[238] = 83; buf[239] = 99;

            int i = 240;
            buf[i++] = 53; buf[i++] = 1; buf[i++] = msgType;         // MsgType
            buf[i++] = 1; buf[i++] = 4; Array.Copy(mask.GetAddressBytes(), 0, buf, i, 4); i += 4; // Subnet mask
            buf[i++] = 3; buf[i++] = 4; Array.Copy(router.GetAddressBytes(), 0, buf, i, 4); i += 4; // Router
            buf[i++] = 6; buf[i++] = 4; Array.Copy(dns.GetAddressBytes(), 0, buf, i, 4); i += 4; // DNS
            buf[i++] = 51; buf[i++] = 4; Array.Copy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(leaseSeconds)), 0, buf, i, 4); i += 4; // Lease
            buf[i++] = 54; buf[i++] = 4; Array.Copy(serverIp.GetAddressBytes(), 0, buf, i, 4); i += 4; // Server ID
            buf[i++] = 255; // END

            return buf.Take(i).ToArray();
        }
    }
}
