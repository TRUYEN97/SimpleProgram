using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEI_MFG.Config.TestCondition
{
    public class TestConditionConfig
    {
        public bool IsEnableContinueFail { get; set; } = false;
        public bool IsEnableOldMacFail { get; set; } = false;
        public int MaxContinueFailSpec { get; set; } = 3;
    }
}
