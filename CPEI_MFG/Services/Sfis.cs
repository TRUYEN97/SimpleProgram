using System;
using System.Threading;
using CPEI_MFG.Communicate.Implement.Serial;
using CPEI_MFG.Config;
using CPEI_MFG.Timer;
using System.Windows.Forms;
using CPEI_MFG.Model;

namespace CPEI_MFG.Services
{
    internal class Sfis : IDisposable
    {
        public Sfis(TestModel testModel)
        {
            TestModel = testModel;
        }
        public TestModel TestModel { get; private set; }
        public ProgramConfig ProgramCf { get; private set; }

        public event Action<string> WriteErrorLog;
        public event Action<string> WriteInfoLog;
        public readonly string SYANTE5 = $"{"SYANTE5",-20}END";
        private MySerialPort serialPort;

        public bool Init()
        {
            ProgramCf = ConfigLoader.ProgramConfig;
            var config = ProgramCf.SfisConfig;
            if (config == null)
            {
                MessageBox.Show($"SfisConfig not config!");
                return false;
            }
            if (TestModel == null)
            {
                MessageBox.Show($"TestModel == null!");
                return false;
            }
            serialPort?.Dispose();
            serialPort = new MySerialPort(config.Com, config.Baudrate);
            if (!serialPort.Connect())
            {
                MessageBox.Show($"Check SFIS({config.Com} - {config.Baudrate}) failed!");
                return false;
            }
            SendSYANTE5();
            Thread.Sleep(1000);
            SendSYANTE5();
            return true;
        }

        private enum SfisResponceStatus
        {
            SUCCESS, FAIL, SKIP
        }

        public enum SfisResult
        {
            PASS, FAIL, TIME_OUT
        }
        public string ComName => serialPort?.Name;
        public SfisResult CheckMacSfis(string mac)
        {
            if (string.IsNullOrEmpty(mac) || mac.Length != 12)
            {
                return SfisResult.FAIL;
            }
            string command = $"{mac,-25}{mac,-12}{ProgramCf.Station,-25}{TestModel.PcName,-25}END";
            return SendSfis(command, (s, line) =>
            {
                if (line.Contains("ERRO"))
                {
                    return SfisResponceStatus.FAIL;
                }
                else
                if (line.Length == 25 + 12 + 25 + 10 + 4 && line.Contains("PASS"))
                {
                    string szModel = line.Substring(37, 25).Trim();
                    if (szModel != ProgramCf.Model)
                    {
                        string errorMsg = $"SFC Model :  {szModel} , Setting Model : {ProgramCf.Model}";
                        WriteErrorLog?.Invoke(errorMsg);
                        return SfisResponceStatus.FAIL;
                    }
                    string szPSN = line.Substring(0, 25).Trim();
                    if (szPSN != TestModel.ScanMAC)
                    {
                        string errorMsg = $"SFC Mac : {szPSN}, Scan Mac : {TestModel.ScanMAC}";
                        TestModel.SfcPSN = szPSN;
                        WriteErrorLog?.Invoke(errorMsg);
                        return SfisResponceStatus.FAIL;
                    }
                    TestModel.DutMO = line.Substring(25 + 12 + 25, 10).Trim().ToUpper();
                    return SfisResponceStatus.SUCCESS;
                }
                return SfisResponceStatus.SKIP;
            }, 5);
        }

        private SfisResult SendSfis(string commnad, Func<MySerialPort, string, SfisResponceStatus> receivedAction, int timeOut = 1)
        {
            SfisResult rs = SfisResult.TIME_OUT;
            bool exit = false;
            serialPort.DataReceivedAction = (s, l) =>
            {
                if (receivedAction == null)
                {
                    return;
                }
                WriteErrorLog?.Invoke($"SFC-->TE: {l}");
                if (string.IsNullOrWhiteSpace(l) || l.Contains("SYANTE5"))
                {
                    return;
                }
                switch (receivedAction.Invoke(s, l))
                {
                    case SfisResponceStatus.SUCCESS:
                        rs = SfisResult.PASS;
                        exit = true;
                        break;
                    case SfisResponceStatus.FAIL:
                        rs = SfisResult.FAIL;
                        exit = true;
                        break;
                    case SfisResponceStatus.SKIP:
                        break;
                }
            };
            Stopwatch stopwatch = new Stopwatch((timeOut < 1 ? 1 : timeOut) * 1000);
            if (!SendSYANTE5())
            {
                return SfisResult.FAIL;
            }
            Thread.Sleep(1000);
            if (serialPort.WriteLine(commnad ?? string.Empty))
            {
                WriteInfoLog($"TE-->SFC: {commnad}");
                while (!exit && stopwatch.IsOntime && serialPort.IsConnect)
                {
                    Thread.Sleep(100);
                }
            }
            return rs;
        }

        public SfisResult SendTestResultToSFC(bool result, string errorCode)//PC-->SFIS For Test Finish
        {
            var verCf = ProgramCf.VersionConfig;
            string commnad = $"{TestModel.ScanMAC,-25}{TestModel.ScanMAC,-12}{verCf.BOMVer,-15}{verCf.FWVer,-25}{verCf.FCDVer,-15}{verCf.RegionVer ?? "",-26}{verCf.FTUVer,-30}{TestModel.PcName.PadRight(12, '-')}";
            if (!result)
            {
                if (string.IsNullOrWhiteSpace(errorCode))
                {
                    errorCode = "UNKNFF";
                }
                else if (errorCode.Length > 6)
                {
                    errorCode = errorCode.Substring(0, 6);
                }
                commnad = $"{commnad}{errorCode.PadRight(6, '-')}";
            }
            return SendSfis(commnad, (s, line) =>
            {
                if (line.Contains("ERRO"))
                {
                    return SfisResponceStatus.FAIL;
                }
                else
                if (line.Trim().EndsWith("PASS"))
                {
                    return SfisResponceStatus.SUCCESS;
                }
                return SfisResponceStatus.SKIP;
            }, 20);
        }
        public bool SendSYANTE5()
        {
            if (serialPort == null)
            {
                return false;
            }
            if (!serialPort.WriteLine(SYANTE5))
            {
                return false;
            }
            return true;
        }

        public void Dispose()
        {
            serialPort?.Dispose();
        }
    }
}
