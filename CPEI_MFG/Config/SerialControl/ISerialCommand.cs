using System;

namespace CPEI_MFG.Config.SerialControl
{
    public interface ISerialCommand
    {
        string Command { get; set; }
        string Target { get; set; }
        long TimeOutMs { get; set; }
        long ResponseTimeMs { get; set; }
        string Errorcode { get; set; }
    }
}
