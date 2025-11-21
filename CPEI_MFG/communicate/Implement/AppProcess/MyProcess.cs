using System;
using System.Diagnostics;
using System.IO;

namespace CPEI_MFG.Communicate.Implement.AppProcess
{
    public class MyProcess : BaseProcess
    {
        public MyProcess(string fileName, string param) : base(fileName, Path.GetDirectoryName(fileName), param, null)
        {

        }
        public MyProcess(string fileName, Action<BaseProcess, string> outputDataReceived) : base(fileName, Path.GetDirectoryName(fileName), null, outputDataReceived)
        {

        }
        public MyProcess(string fileName, string param, Action<BaseProcess, string> outputDataReceived) : base(fileName, Path.GetDirectoryName(fileName), param, outputDataReceived)
        {

        }
    }
}
