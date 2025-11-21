using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEI_MFG.Config
{
    public class LoggerConfig
    {
        public string Host { get; set; } = "192.168.240.52";
        public int Port { get; set; } = 4422;
        public string User { get; set; } = "user";
        public string Password { get; set; } = "ubnt";
    }
}
