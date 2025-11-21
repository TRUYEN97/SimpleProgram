

using System;
using CPEI_MFG.Communicate;

namespace UiTest.Service.Communicate.Implement.Cmd
{
    internal class PingProcess : BaseProcess
    {
        public PingProcess(string command) : this(command, null) { }
        public PingProcess(string command, Action<BaseProcess, string> outputDataReceived) : base("ping", command, outputDataReceived) { }

        public bool WaitForPing()
        {
            string line;
            while ((line = OutPutReader.ReadLine()) != null)
            {
                if (line.ToLower().Contains(" ttl="))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
