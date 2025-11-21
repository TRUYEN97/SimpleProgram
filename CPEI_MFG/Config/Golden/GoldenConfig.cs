using System;
using System.Collections.Generic;

namespace CPEI_MFG.Config.Golden
{
    public class GoldenConfig
    {
        public bool IsGoodGdEnable { get; set; } = false;
        public List<string> GoodGoldens { get; set; } = new List<string>();
        public bool IsBadGdEnable { get; set; } = false;
        public List<string> BadGoldens { get; set; } = new List<string>();
        public double Time { get; set; } = 12;
    }
}
