using System;
namespace CPEI_MFG.Config
{
    public abstract class AbsApproxCommand
    {
        public string TextTrigget { get; set; } = string.Empty;
        public string Keywork { get; set; } = string.Empty;
        public int InApprox { get; set; } = 1;
        internal virtual bool IsEnable => !string.IsNullOrWhiteSpace(TextTrigget) && !string.IsNullOrWhiteSpace(Keywork);
        internal int Count { get; set; } = 0;
        internal bool Start { get; set; } = false;
    }
}
