using CPEI_MFG.Common;
using CPEI_MFG.Config;
using CPEI_MFG.Config.SerialControl;
using CPEI_MFG.Config.SerialControl.CCD;
using CPEI_MFG.Model;
using System;
using System.Threading;
using System.Windows.Forms;

namespace CPEI_MFG.Services.SerialControl.CCD
{
    public class CcdChecker : BaseSerialControl<CcdCommandModel>
    {

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
                if (config == null)
                {
                    MessageBox.Show($"{Name} not config!");
                    return false;
                }
                models.Clear();
                foreach (var item in config.Commands)
                {
                    models.Add(item);
                }
                IsEnable = config.IsEnable;
                Name = "CCD";
                if (!IsEnable)
                {
                    return true;
                }
                if (string.IsNullOrWhiteSpace(config.Com))
                {
                    MessageBox.Show($"The {Name} serial name is not config!");
                    return false;
                }
                CommandInfo = $"{appSettingModel.Model}-{appSettingModel.Station}";
                return ConnectToSerial(config.Com, config.Baudrate);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public string MainWindowTitle => TestModel?.MainWindowTitle;
        public bool IsAcceptable { get; private set; }
        public TestModel TestModel { get; }
        public string CommandInfo { get; private set; }
        public override bool IsEnable { get; protected set; }

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

        protected override void AttackCommand(CcdCommandModel model, string line)
        {
            try
            {
                while (!IsShowConfirmWindow(model))
                {
                    Thread.Sleep(500);
                }
                bool rs = RunCommand(model);
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
        }

        protected override bool IsCorrectValue(string value, string target)
        {
            return value?.Equals(target, StringComparison.OrdinalIgnoreCase) == true;
        }

        protected override string CreateCommnand(ISerialCommand model)
        {
            return $"{CommandInfo}-{TestModel.ScanMAC}-{model.Command}";
        }

        protected override bool IsCanWait(ISerialCommand model)
        {
            return IsShowConfirmWindow(model as CcdCommandModel);
        }
    }
}
