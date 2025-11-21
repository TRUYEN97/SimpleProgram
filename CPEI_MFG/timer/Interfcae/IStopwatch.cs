using System;

namespace CPEI_MFG.Timer.Interfcae
{
    public interface IStopwatch
    {
        void Reset();
        void Start(long interval);
        bool IsOntime { get; }
        bool IsOutOfTime {  get; }
        long GetCurrentTime { get; }
        long Interval { get; }
    }
}
