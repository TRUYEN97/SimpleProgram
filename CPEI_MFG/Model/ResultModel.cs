
using System;

namespace CPEI_MFG.Model
{
    internal class ResultModel
    {
        public ResultModel(string logPath) { LogPath = logPath; }
        public bool IsPass { get; private set; }
        public string ErrorCode { get; private set; }
        public string LogPath { get; private set; }

        internal void SetFail(string errorCode)
        {
            ErrorCode = errorCode;
            IsPass = false;
        }

        internal void SetPass()
        {

            ErrorCode = string.Empty;
            IsPass = true;
        }
    }
}
