namespace CPEI_MFG.Config.TestCondition
{
    public class TestConditionConfig
    {
        public bool IsCheckMacOldEnable { get; set; } = true;
        public ContinueFailConfig ContinueFailConfig { get; set; } = new ContinueFailConfig();
        public CounterConfig CounterConfig { get; set; } = new CounterConfig();
    }
}
