using System;
using System.Threading.Tasks;
using CPEI_MFG.Communicate;

namespace UiTest.Service.Communicate.Implement.Cmd
{
    public class CmdProcess : BaseProcess
    {
        public CmdProcess(bool keepSeason, string command, Action<BaseProcess, string> outputDataReceived) :
            base("cmd.exe", $"{(keepSeason ? "/K" : "/C")} {(string.IsNullOrWhiteSpace(command) ? "" : ($"{command}"))}",
                outputDataReceived)
        { }
        public CmdProcess(bool keepSeason, string command) : this(keepSeason, command, null) { }
        public CmdProcess(bool keepSeason) : this(keepSeason, "") { }
        public CmdProcess() : this(true) { }

        public override bool Write(string mess)
        {
            try
            {
                base.Write(mess);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override bool WriteLine(string mess)
        {
            try
            {
                base.WriteLine(mess);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<bool> WriteAsync(string mess)
        {
            try
            {
                await base.WriteAsync(mess);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<bool> WriteLineAsync(string mess)
        {
            try
            {
                await base.WriteAsync(mess);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
