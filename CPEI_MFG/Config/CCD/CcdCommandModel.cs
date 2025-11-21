using System;

namespace CPEI_MFG.Config.CCD
{
    public class CcdCommandModel
    {
        private long waitTime;
        private long responseTime;

        public CcdCommandModel()
        {
            Keywork = string.Empty;
            WindowTitle = "Work fine?";
            Command = "LCMR";
            Target = "LCMROK";
            waitTime = 20;
            ResponseTime = 5;
        }
        public string Keywork { get; set; }
        public string WindowTitle { get; set; }
        public string Command { get; set; }
        public string Target { get; set; }
        public long TimeOut { get => waitTime; set => waitTime = value < 1 ? 1 : value; }

        public long ResponseTime { get => responseTime; set => responseTime = value < 1 ? 1 : value; }

        public override string ToString()
        {
            return $"Key[{Keywork}] windowTitle[{WindowTitle}] Command[{Command}] Target[{Target}], WaitTime[{ResponseTime/TimeOut}]";
        }
    }
}
