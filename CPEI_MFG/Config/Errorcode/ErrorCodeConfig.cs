using CPEI_MFG.Service;
using System;
using System.Collections.Generic;

namespace CPEI_MFG.Config.Errorcode
{
    public class ErrorCodeConfig
    {
        public SftpConfig SftpConfig {  get; set; } = new SftpConfig();
        public int MaxLength { get; set; } = 6;
        public string LocalFilePath { get; set; } = "../Errorcodes.csv";
        public string LocalNewFilePath { get; set; } = "../NewErrorcodes.csv";
        public List<ErrorcodeModelConfig> Configs { get; set; } = new List<ErrorcodeModelConfig>() { new ErrorcodeModelConfig { } };
    }
}
