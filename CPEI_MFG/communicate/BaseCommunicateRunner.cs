using System;
using System.Threading;
using System.Threading.Tasks;

namespace CPEI_MFG.Communicate
{
    public abstract class BaseCommunicateRunner : BaseCommunicate
    {
        protected BaseCommunicateRunner() { }
        protected CancellationTokenSource Cts;
        private Task currentTask;

        public event Action<string> DataReceived;

        public bool IsRunning => currentTask != null && !currentTask.IsCompleted;

        public void Start()
        {
            if (IsRunning) { return; }
            if (Cts?.IsCancellationRequested == true)
            {
                Cts.Cancel();
            }
            Cts = new CancellationTokenSource();
            currentTask = Task.Run(async () =>
            {
                string line;
                while (!Cts.IsCancellationRequested && (line = await ReadLineAsync()) != null)
                {
                    DataReceived?.Invoke(line.Trim());
                }
            }, Cts.Token);

        }
    }
}
