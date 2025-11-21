using System;
using System.Collections.Generic;

namespace CPEI_MFG.Config.CCD
{
    public class CcdConfig
    {
        public bool IsEnable { get; set; } = false;
        public string Com { get; set; } = "COM10";
        public int Baudrate { get; set; } = 9600;
        public List<CcdCommandModel> Commands { get; set; } = new List<CcdCommandModel>() { new CcdCommandModel { } };
    }
}
