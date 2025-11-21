using System;
using System.Net;

namespace CPEI_MFG.Service.DHCP
{
    public class LeaseModel
    {
        public uint UIp {  get; set; }
        public bool IsExpire => ExpireAt == default || ExpireAt <= DateTime.Now;
        public IPAddress Ip { get; set; } = null;
        public DateTime ExpireAt { get; set; } = default;
    }
}
