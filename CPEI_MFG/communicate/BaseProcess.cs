
using System;
using System.Diagnostics;
using System.Threading;
using CPEI_MFG.Communicate.InOutStream;

namespace CPEI_MFG.Communicate
{
    public abstract class BaseProcess : BaseInOutStream
    {
        private readonly Process _process;
        private readonly DynamicTextReader outPutReader;
        protected BaseProcess(string fileName, string command, Action<BaseProcess, string> outputDataReceived) : this(fileName, null, command, outputDataReceived) { }
        protected BaseProcess(string fileName, string workingDirectory, string command, Action<BaseProcess, string> dataReceivedAction)
        {
            DataReceivedAction = dataReceivedAction;
            _process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = fileName ?? "",
                    WorkingDirectory = workingDirectory,
                    Arguments = command ?? "",
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };
            _process.Start();
            outPutReader = new DynamicTextReader();
            _process.OutputDataReceived += (sender, args) =>
            {
                string line = args.Data?.Trim();
                if (line != null)
                {
                    outPutReader?.AddLine(line);
                    DataReceivedAction?.Invoke(this, line);
                }
                else
                {
                    outPutReader?.Complete();
                }
            };
            _process.ErrorDataReceived += (sender, args) =>
            {
                string line = args.Data?.Trim();
                if (line != null)
                {
                    outPutReader?.AddLine(args.Data);
                    DataReceivedAction?.Invoke(this, line);
                }
                else
                {
                    outPutReader?.Complete();
                }
            };
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
            OutPutReader = outPutReader;
            InputWriter = new StringWriter(_process.StandardInput);
        }

        public bool TryGetLine(out string line, int msTimeout, CancellationToken token)
        {
            return outPutReader.TryGetLine(out line, msTimeout, token);
        }
        public bool TryGetLine(out string line, int msTimeout)
        {
            return outPutReader.TryGetLine(out line, msTimeout);
        }
        public bool TryGetLine(out string line)
        {
            return outPutReader.TryGetLine(out line);
        }

        public Action<BaseProcess, string> DataReceivedAction { private get; set; }
        public Process Process => _process;
        public static Process GetProcessFromID(int id)
        {
            try
            {
                if (id < 0)
                {
                    return null;
                }
                return Process.GetProcessById(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Không thể kill process: " + ex.Message);
                return null;
            }
        }
        public void Kill()
        {
            if (!_process.HasExited)
            {
                _process.Kill();
            }
        }
        public int PID => !_process.HasExited ? _process.Id : -1;
        public void WaitForExit()
        {
            _process?.WaitForExit();
        }

        public void Complete()
        {
            if (OutPutReader is DynamicTextReader reader)
            {
                reader.Complete();
            }
        }

        public virtual bool HasExited()
        {
            try
            {
                return _process == null || _process.HasExited;
            }
            catch (Exception)
            {
                return true;
            }
        }

        protected override void Close()
        {
            _process?.CancelErrorRead();
            _process?.CancelOutputRead();
            _process?.Close();
            _process?.Dispose();
            Complete();
        }
    }
}
