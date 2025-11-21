using CPEI_MFG.Common;
using CPEI_MFG.Communicate.Implement.AppProcess;
using CPEI_MFG.Config;
using CPEI_MFG.Config.Dhcp;
using CPEI_MFG.Config.FTU;
using CPEI_MFG.Model;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace CPEI_MFG.Services.FTU
{
    public class FtuService : BaseService<CheckLogConfig>
    {
        public FtuService(TestModel testModel)
        {
            TestModel = testModel;
        }
        public TestModel TestModel { get; }
        public override bool IsEnable { get; protected set; }
        public override bool Init()
        {
            try
            {
                var ftuConfig = ProgramConfig.FtuConfig;
                if (ftuConfig == null)
                {
                    MessageBox.Show("ftuConfig not config!");
                    return false;
                }
                ///////////////////////
                if (!Directory.Exists(ftuConfig.DirPath))
                {
                    MessageBox.Show($"FTP dir: '{ftuConfig.DirPath}' find not found!");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(ftuConfig.Name))
                {
                    MessageBox.Show($"The FTP name that name get from FTU dir('{ftuConfig.Name}') is null or empty!");
                    return false;
                }
                if (!File.Exists(ftuConfig.ConfigPath))
                {
                    MessageBox.Show($"FTP config path: '{ftuConfig.ConfigPath}' find not found!");
                    return false;
                }
                var verConfig = ProgramConfig.VersionConfig;
                if (string.IsNullOrWhiteSpace(verConfig.FWVer))
                {
                    MessageBox.Show($"FWVersion == Empty!");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(verConfig.FTUVer) || verConfig.FTUVer != FTUVersionFromName(ftuConfig.Name))
                {
                    MessageBox.Show($"Version does not match FTU name\r\nFTUVersion: [{verConfig.FTUVer}] \r\nFTU name: [{ftuConfig.Name}]");
                    return false;
                }
                return CheckFtuConfig();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool CheckFtuConfig()
        {
            bool rs = true;
            var ftuConfig = ProgramConfig.FtuConfig;
            IniFile iniFile = new IniFile(ftuConfig.ConfigPath);
            if (ftuConfig.FtuDataConfigs?.Count > 0)
            {
                foreach (var item in ftuConfig.FtuDataConfigs)
                {
                    if (string.IsNullOrEmpty(item.TargetValue) || string.IsNullOrEmpty(item.Sector) || string.IsNullOrEmpty(item.Key))
                    {
                        continue;
                    }
                    string ftuCfValue = iniFile.ReadString(item.Sector, item.Key, null);
                    if (ftuCfValue == null)
                    {
                        MessageBox.Show($"not found Sector[{item.Sector}]-Key[{item.Key}] in the FTU config");
                        rs = false;
                    }
                    if (ftuCfValue != item.TargetValue)
                    {
                        MessageBox.Show($"{item.ErrorMessage}\r\nFtu config value: {ftuCfValue} != {item.TargetValue}");
                        rs = false;
                    }
                }
            }
            return rs;
        }

        private string FTUVersionFromName(string fullName)
        {
            fullName = fullName.Replace(".", "").Trim();
            string best = Util.FindGroup(fullName, @"_(\d{2,}_\d{2,})_?");
            return Regex.Replace(best, @"\D", "");
        }

        protected override void CheckModel(CheckLogConfig model, string line)
        {
            if (!model.IsPass)
            {
                if (line.Contains(model.Keywork))
                {
                    model.IsPass = true;
                }
            }
        }

        public override bool TryGetErrorcode(out string errorcode)
        {
            errorcode = null;
            foreach (var item in models)
            {
                if (!item.IsPass)
                {
                    errorcode = item.ErrorCode;
                    return true;
                }
            }
            return false;
        }

        protected override void ResetModel(CheckLogConfig model)
        {
            foreach (var item in models)
            {
                item.IsPass = false;
            }
        }

        public void StartTest()
        {
            string appPath = Path.GetFullPath(Path.Combine(ProgramConfig.FtuConfig.DirPath, ProgramConfig.FtuConfig.Name));
            using (MyProcess cmd = new MyProcess(appPath, ProgramConfig.FtuConfig.FTUParam)) { }
            IntPtr ftuPtr;
            while ((ftuPtr = WindowControl.GetMainWindow(null, ProgramConfig.FtuConfig.FTUWindowsTitle)) == IntPtr.Zero)
            {
                Thread.Sleep(500);
            }
            WindowControl.RaiseWindowProcess(ftuPtr);
            WindowControl.SetWindowMaximize(ftuPtr);
            Thread.Sleep(1000);
            SendKeys.SendWait(TestModel.Input);
            Thread.Sleep(500);
            SendKeys.SendWait("{F2}");
            WindowControl.RaiseWindowProcess(TestModel.MainWindowTitle);
        }

        public ProgramConfig ProgramConfig => ConfigLoader.ProgramConfig;
    }
}
