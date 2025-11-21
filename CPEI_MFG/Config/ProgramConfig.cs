using System;
using System.IO;
using CPEI_MFG.Config.Dhcp;
using CPEI_MFG.Config.Errorcode;
using CPEI_MFG.Config.FTU;
using CPEI_MFG.Config.Golden;
using CPEI_MFG.Config.SerialControl;
using CPEI_MFG.Config.SerialControl.CCD;
using CPEI_MFG.Config.Sfis;
using CPEI_MFG.Config.TestCondition;
using CPEI_MFG.Config.Worker;

namespace CPEI_MFG.Config
{
    public class ProgramConfig
    {
        public string Model { get; set; } = string.Empty;
        public string Station { get; set; } = string.Empty;
        public bool IsDebugModeEnable { get; set; }
        public string LocalLog { get; set; } = "D:\\UBNT_Test_Logs";
        public string LocalLogGolden { get; set; } = "D:\\UBNT_Golden_Test_Logs";
        public string LocalLogNoSFC { get; set; } = "D:\\UBNT_Test_Logs_NoSFC";
        internal string ServerlogDir => string.IsNullOrWhiteSpace(LocalLog) ? string.Empty : Path.GetFileName(LocalLog);
        internal string ServerLogGolden => string.IsNullOrWhiteSpace(LocalLogGolden) ? string.Empty : Path.GetFileName(LocalLogGolden);
        internal string ServerLogNoSFC => string.IsNullOrWhiteSpace(LocalLogNoSFC) ? string.Empty : Path.GetFileName(LocalLogNoSFC);
        public int InputMaxLength { get; set; } = 19;
        public int MacIndex { get; set; } = 0;
        public string DUT_IP { get; set; } = "192.168.1.20";
        public LoggerConfig LoggerConfig { get; set; } = new LoggerConfig();
        public VersionConfig VersionConfig { get; set; } = new VersionConfig();
        public SfisConfig SfisConfig { get; set; } = new SfisConfig();
        public DhcpConfig DhcpConfig { get; set; } = new DhcpConfig();
        public CcdConfig CcdConfig { get; set; } = new CcdConfig();
        public FtuConfig FtuConfig { get; set; } = new FtuConfig();
        public GoldenConfig GoldenConfig { get; set; } = new GoldenConfig();
        public TestConditionConfig TestCondition { get; set; } = new TestConditionConfig();
        public ErrorCodeConfig ErrorCodeConfig { get; set; } = new ErrorCodeConfig();
        public WorkerCheckerConfig WorkerChecker { get; set; } = new WorkerCheckerConfig();
        public SerialControlConfig SerialControl { get; set; } = new SerialControlConfig();

    }
}
