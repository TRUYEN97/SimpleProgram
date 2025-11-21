using System;

namespace CPEI_MFG.Config.SerialControl
{
    public class SerialCommand : ISerialCommand
    {
        public string Command { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public long ResponseTimeMs { get; set; }
        public long TimeOutMs { get; set; }
        public string Errorcode { get; set; } = string.Empty;
    }
}
