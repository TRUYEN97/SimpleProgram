using CPEI_MFG.Communicate.Implement.Serial;
using CPEI_MFG.Timer;
using CPEI_MFG.Config;
using CPEI_MFG.Config.SerialControl;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace CPEI_MFG.Services.SerialControl
{
    public abstract class BaseSerialControl<T> : BaseInApprox<T>, IDisposable where T : AbsApproxCommand
    {
        protected MySerialPort serialPort;
        public string Name { get; protected set; }
        public event Action<string> WriteLog;
        public bool IsConnect => serialPort?.IsConnect == true;

        protected bool ConnectToSerial(string serialName, int baudrate)
        {
            serialPort?.Dispose();
            serialPort = new MySerialPort(serialName, baudrate);
            if (!serialPort.Connect() || !serialPort.WriteLine(""))
            {
                MessageBox.Show($"Connect to serial({serialName}) failed!");
                return false;
            }
            return true;
        }

        protected bool RunCommand(List<ISerialCommand> models)
        {
            if (models == null)
            {
                WriteLog?.Invoke($"TE -> {Name}: RunCommand(List<ISerialCommand> models): models == null!!");
                return false;
            }
            foreach (var model in models)
            {
                if (!string.IsNullOrWhiteSpace(model?.Command) && !RunCommand(model))
                {
                    errorCodes.Add(model.Errorcode);
                    return false;
                }
            }
            return true;
        }
        protected override void OnTriggerStart(T model, string line)
        {
            if (IsEnable && IsConnect && model?.IsEnable == true)
            {
                AttackCommand(model, line);
            }
        }
        protected abstract void AttackCommand(T model, string line);
        protected bool RunCommand(ISerialCommand model)
        {
            if (model == null)
            {
                WriteLog?.Invoke($"TE -> {Name}: RunCommand(ISerialCommand model): model == null!!");
                return false;
            }
            string line;
            Stopwatch stopwatch = new Stopwatch(model.TimeOutMs > 0 ? model.TimeOutMs : 0);
            Stopwatch waitResponceTimer = new Stopwatch(model.ResponseTimeMs > 0 ? model.ResponseTimeMs : 0);
            do
            {
                string command = CreateCommnand(model);
                if (command == null)
                {
                    WriteLog?.Invoke($"TE -> {Name}: command == null!!");
                    return false;
                }
                while (serialPort.TryGetLine(out line, 100)) { Thread.Sleep(10); }
                if (!serialPort.WriteLine(command))
                {
                    WriteLog?.Invoke($"TE -> {Name}: {command} failed!");
                    return false;
                }
                WriteLog?.Invoke($"TE -> {Name}: {command}");
                waitResponceTimer.Reset();
                while (IsCanWait(model) && IsEnable && IsConnect && stopwatch.IsOntime && waitResponceTimer.IsOntime)
                {
                    if (serialPort.TryGetLine(out line, 100))
                    {
                        WriteLog?.Invoke($"{Name} -> TE: {line}");
                        if (IsCorrectValue(line, model.Target))
                        {
                            return true;
                        }
                        break;
                    }
                }
            } while (IsCanWait(model) && IsEnable && IsConnect && stopwatch.IsOntime);
            return false;
        }

        protected abstract string CreateCommnand(ISerialCommand model);
        protected abstract bool IsCanWait(ISerialCommand model);
        protected abstract bool IsCorrectValue(string value, string target);
        public void Dispose()
        {
            serialPort?.Dispose();
        }
    }
}
