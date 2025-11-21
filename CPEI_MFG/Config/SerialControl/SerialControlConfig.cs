using System;
using System.Collections.Generic;

namespace CPEI_MFG.Config.SerialControl
{
    public class SerialControlConfig
    {
        public bool IsEnable { get; set; } = false;
        public string Com { get; set; } = "COM3";
        public int Baudrate { get; set; } = 9600;
        public List<SerialCommand> StartTestCommands { get; set; } = new List<SerialCommand> { new SerialCommand() };
        public List<SerialCommand> EndTestCommands { get; set; } = new List<SerialCommand> { new SerialCommand() };
        public List<SerialControlCommand> Commands { get; set; } = new List<SerialControlCommand>() { new SerialControlCommand { } };
        public string Name { get; set; } = "Fixture";
    }
}
