using System;
using System.Collections.Generic;
using System.IO;

namespace CPEI_MFG.Config.FTU
{
    public class FtuConfig
    {

        public string DirPath { get; set; } = string.Empty;
        internal string Name => string.IsNullOrWhiteSpace(DirPath) ? string.Empty : Path.GetFileName(DirPath);
        public string CustomConfigFileName { get; set; } = string.Empty;
        public string FTUParam { get; set; } = string.Empty;
        public List<FtuDataConfig> FtuDataConfigs { get; set; } = new List<FtuDataConfig>() { new FtuDataConfig { } };
        public List<CheckLogConfig> CheckLogConfigs { get; set; } = new List<CheckLogConfig> { new CheckLogConfig { } };
        internal string ConfigPath => string.IsNullOrWhiteSpace(DirPath) ? string.Empty : Path.Combine(DirPath, "data", "custom_config_files", CustomConfigFileName);
        internal string FTUWindowsTitle => string.IsNullOrWhiteSpace(Name) ? string.Empty : $"Factory Test Utility Version {Name}";
        internal string LogDir => string.IsNullOrWhiteSpace(DirPath) ? string.Empty : Path.Combine(DirPath, "logs");
    }
}
