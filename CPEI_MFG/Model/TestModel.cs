

using System;

namespace CPEI_MFG.Model
{
    public class TestModel
    {
        private int rJ45;
        private int failNum;
        private int passNum;
        public string SfcPSN { get; set; }
        public string ScanPSN { get; set; }
        public string ScanMAC { get; set; }
        public string DutMO { get; set; }
        public int CycleTime { get; set; }
        public string Input { get; set; }
        public string IpAddr { get; set; }
        public string AppVer { get; set; }
        public string MainWindowTitle { get; set; }
        public string PcName { get; set; }
    }
}
