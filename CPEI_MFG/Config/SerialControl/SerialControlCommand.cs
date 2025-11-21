using System;
using System.Collections.Generic;

namespace CPEI_MFG.Config.SerialControl
{
    public class SerialControlCommand : AbsApproxCommand
    {
        public SerialControlCommand() {
            Commands = new List<SerialCommand> { new SerialCommand() };
        }
        public List<SerialCommand> Commands { get; set; }
        internal override bool IsEnable => base.IsEnable && Commands?.Count > 0;
    }
}
