
using System;
using CPEI_MFG.Timer.Interfcae;

namespace CPEI_MFG.Timer
{
    public class Stopwatch : IStopwatch
    {
        private long _interval;
        private long _startTime;
        public Stopwatch(long interval)
        {
            Start(interval);
        }

        public long Interval { get => _interval; set { _interval = value < 0 ? 0 : value; } }

        public long GetCurrentTime => DateTimeOffset.Now.ToUnixTimeMilliseconds() - _startTime;

        public bool IsOntime => GetCurrentTime < _interval;

        public bool IsOutOfTime => !IsOntime;

        public void Reset()
        {
            _startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        public void Start(long interval)
        {
            Interval = interval;
            Reset();
        }
    }
}
