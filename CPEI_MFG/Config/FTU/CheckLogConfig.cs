using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEI_MFG.Config.FTU
{
    public class CheckLogConfig
    {
        public string Keywork { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;
        internal bool IsPass { get; set; } = false;
    }
}
