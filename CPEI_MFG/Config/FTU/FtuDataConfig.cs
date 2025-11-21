using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEI_MFG.Config.FTU
{
    public class FtuDataConfig
    {
        public string Sector { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string TargetValue { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
