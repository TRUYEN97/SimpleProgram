using System.Collections.Generic;
using CPEI_MFG.Config;

namespace CPEI_MFG.Config.Worker
{
    public class WorkerCheckerConfig
    {
      public bool IsEnable {  get; set; } = false;
        public List<WorkerCheckerCommand> Commands { get; set; } = new List<WorkerCheckerCommand>() { new WorkerCheckerCommand { } };
    }
}