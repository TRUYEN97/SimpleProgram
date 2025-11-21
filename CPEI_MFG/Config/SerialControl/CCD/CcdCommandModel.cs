using System;

namespace CPEI_MFG.Config.SerialControl.CCD
{
    public class CcdCommandModel: AbsApproxCommand, ISerialCommand
    {
        public CcdCommandModel()
        {
            Keywork = string.Empty;
            WindowTitle = "Work fine?";
            Command = "LCMR";
            Target = "LCMROK";
            Errorcode = string.Empty;
            ResponseTimeMs = 5000;
            TimeOutMs = 20000;
        }
        public string WindowTitle { get; set; }
        public string Command { get; set; }
        public string Target { get; set; }
        public long TimeOutMs { get; set; }
        public long ResponseTimeMs { get; set; }
        public string Errorcode { get; set; }
    }
}
