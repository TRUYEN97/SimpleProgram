using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppUtil.Communicate.Implement.Serial;
using AppUtil.Timer;
using CPEI_MFG.Config;
using CPEI_MFG.Config.CCD;
using CPEI_MFG.Model;

namespace CPEI_MFG.Services.CCD
{
    public class CcdChecker : BaseService<CcdCommandModel>, IDisposable
    {
        private MySerialPort serialPort;


        public CcdChecker(TestModel testModel)
        {
            TestModel = testModel;
        }

        public override bool Init()
        {
            try
            {
                var appSettingModel = ConfigLoader.ProgramConfig;
                var config = appSettingModel.CcdConfig;
                models.Clear();
                foreach (var item in config.Commands)
                {
                    models.Add(item);
                }
                IsEnable = config.IsEnable;
                if (!IsEnable)
                {
                    return true;
                }
                if (string.IsNullOrWhiteSpace(config.Com))
                {
                    MessageBox.Show($"The CCD serial name is not config!");
                    return false;
                }
                CommandInfo = $"{appSettingModel.Model}-{appSettingModel.Station}";
                serialPort?.Dispose();
                serialPort = new MySerialPort(config.Com, config.Baudrate);
                return serialPort.Connect();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string MainWindowTitle => TestModel?.MainWindowTitle;
        public bool IsAcceptable { get; private set; }
        public TestModel TestModel { get; }

        public string CommandInfo { get; private set; }
        public override bool IsEnable { get; protected set; }

        protected override void ResetModel(CcdCommandModel model)
        {
            IsAcceptable = true;
        }

        public override bool TryGetErrorcode(out string errorcode)
        {
            if (base.TryGetErrorcode(out errorcode))
            {
                return true;
            }
            if (!IsAcceptable)
            {
                errorcode = "CHEAT";
                return true;
            }
            return false;
        }

        public override void Reset()
        {
            IsAcceptable = true;
            base.Reset();
        }

        private static bool IsShowConfirmWindow(CcdCommandModel model)
        {
            return WindowControl.GetMainWindow(null, model.WindowTitle) != IntPtr.Zero;
        }

        public event Action<string> WriteLog;

        public void Dispose()
        {
            serialPort?.Dispose();
        }

        protected override void CheckModel(CcdCommandModel model, string line)
        {
            if (string.IsNullOrEmpty(model?.Keywork) || !line.Contains(model.Keywork))
            {
                return;
            }
            Task.Run(() =>
            {
                try
                {
                    while (!IsShowConfirmWindow(model))
                    {
                        Thread.Sleep(500);
                    }
                    bool rs = SendCommandAndWaitTarget(model);
                    if (IsShowConfirmWindow(model))
                    {
                        if (rs)
                        {
                            WindowControl.RaiseWindowProcess(model.WindowTitle);
                            Thread.Sleep(50);
                            SendKeys.SendWait("y");
                        }
                        else
                        {
                            WindowControl.RaiseWindowProcess(model.WindowTitle);
                            Thread.Sleep(50);
                            SendKeys.SendWait("n");
                        }
                    }
                    else
                    {
                        IsAcceptable = false;
                    }
                }
                finally
                {
                    if (!string.IsNullOrEmpty(MainWindowTitle))
                    {
                        WindowControl.RaiseWindowProcess(MainWindowTitle);
                    }
                }
            });
        }

        private bool SendCommandAndWaitTarget(CcdCommandModel model)
        {
            string command = $"{CommandInfo}-{TestModel.ScanMAC}-{model.Command}";
            string line;
            Stopwatch stopwatch = new Stopwatch(model.TimeOut);
            Stopwatch waitResponceTimer = new Stopwatch(model.ResponseTime);
            bool hasResponce = false;
            do
            {
                while (serialPort.TryGetLine(out line, 100)) { Thread.Sleep(10); }
                if (!serialPort.WriteLine(command))
                {
                    WriteLog?.Invoke($"TE -> CCD: {command} failed!");
                    return false;
                }
                WriteLog?.Invoke($"TE -> CCD: {command}");
                waitResponceTimer.Reset();
                hasResponce = false;
                while (IsShowConfirmWindow(model) && serialPort.IsConnect && stopwatch.IsOntime && waitResponceTimer.IsOntime)
                {
                    if (serialPort.TryGetLine(out line, 100))
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            hasResponce = true;
                            WriteLog?.Invoke($"CCD -> TE: {line}");
                            if (line.Equals(model.Target, StringComparison.OrdinalIgnoreCase))
                            {
                                return true;
                            }
                            break;
                        }
                    }
                }
                if (hasResponce)
                {
                    Thread.Sleep(5000);
                }
            } while (IsShowConfirmWindow(model) && serialPort.IsConnect && stopwatch.IsOntime);
            return false;
        }
    }
}
