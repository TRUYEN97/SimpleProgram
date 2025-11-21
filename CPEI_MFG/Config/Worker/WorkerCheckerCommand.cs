using System;

namespace CPEI_MFG.Config.Worker
{
    public class WorkerCheckerCommand : AbsApproxCommand
    {

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public QuestionType Type { get; set; } = QuestionType.AutoOk;
        public int DelaySeconds { get; set; }
    }
}
