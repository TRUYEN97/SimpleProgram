using System.Collections.Generic;

namespace CPEI_MFG.Service.Condition
{
    public static class CheckConditionFactory
    {
        private static readonly Dictionary<int, CheckTestFailed> checkConditions = new Dictionary<int, CheckTestFailed>();
        private static readonly Dictionary<int, GoldenVerify> goldenVerifys = new Dictionary<int, GoldenVerify>();
        private static readonly Dictionary<int, UnitCounter> initCounterRegistrys = new Dictionary<int, UnitCounter>();
        public static CheckTestFailed GetCheckTestFailedInstanceOf(int index)
        {
            if (index < 0)
            {
                index = 0;
            }
            if (checkConditions.TryGetValue(index, out var checkCondition))
            {
                return checkCondition;
            }
            checkCondition = new CheckTestFailed(index);
            checkConditions.Add(index, checkCondition);
            return checkCondition;
        }
        public static GoldenVerify GetGoldenVerifyInstanceOf(int index)
        {
            if (index < 0)
            {
                index = 0;
            }
            if (goldenVerifys.TryGetValue(index, out var goldenVerify))
            {
                return goldenVerify;
            }
            goldenVerify = new GoldenVerify(index);
            goldenVerifys.Add(index, goldenVerify);
            return goldenVerify;
        }

        public static UnitCounter GetUnitCounterInstanceOf(int index)
        {
            if (index < 0)
            {
                index = 0;
            }
            if (initCounterRegistrys.TryGetValue(index, out var ins))
            {
                return ins;
            }
            ins = new UnitCounter(index);
            initCounterRegistrys.Add(index, ins);
            return ins;
        }
    }
}
