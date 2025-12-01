using CPEI_MFG.Common;

namespace CPEI_MFG.Service.Condition
{
    public class UnitCounter
    {
        private readonly RegistryUtil testCountRegistry;
        public UnitCounter(int index)
        {
            testCountRegistry = new RegistryUtil($"Software\\CPEI_MFG\\Unit{index}\\Counter");
            MaxRJ45Count = 4500;
        }
        public bool IsEnable { get; set; }
        public int MaxRJ45Count { get; set; }

        public bool IsRj45OutOfUseSpec => RJ45Count >= MaxRJ45Count;

        public int RJ45Count
        {
            get
            {
                return testCountRegistry.GetValue("RJ45", 0);
            }
            set
            {
                testCountRegistry.SaveIntValue("RJ45", value);
            }
        }

        public int PassCount
        {
            get
            {
                return testCountRegistry.GetValue("PassCount", 0);
            }
            set
            {
                testCountRegistry.SaveIntValue("PassCount", value);
            }
        }

        public int FailCount
        {
            get
            {
                return testCountRegistry.GetValue("FailCount", 0);
            }
            set
            {
                testCountRegistry.SaveIntValue("FailCount", value);
            }
        }
    }
}
