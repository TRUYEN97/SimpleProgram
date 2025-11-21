using CPEI_MFG.Common;
using CPEI_MFG.Config;
using CPEI_MFG.Config.Worker;
using CPEI_MFG.Model;
using System;
using System.Threading;
using System.Windows.Forms;

namespace CPEI_MFG.Services.Worker
{
    public class WorkerChecker : BaseInApprox<WorkerCheckerCommand>
    {
        private readonly TestModel testModel;
        public string WindowTitle => testModel.MainWindowTitle;
        public ProgramConfig ProgramConfig => ConfigLoader.ProgramConfig;

        public WorkerChecker(TestModel testModel)
        {
            this.testModel = testModel;
        }

        public event Action<string> DebugLogEnvent;
        public override bool IsEnable { get; protected set; }
        public override bool Init()
        {
            try
            {
                models.Clear();
                var config = ProgramConfig.WorkerChecker;
                IsEnable = config.IsEnable;
                if (config.Commands?.Count > 0)
                {
                    foreach (var command in config.Commands)
                    {
                        if (command?.IsEnable == true)
                        {
                            models.Add(command);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        protected override void OnTriggerStart(WorkerCheckerCommand model, string line)
        {
            while (!IsShowConfirmWindow(model.Title))
            {
                Thread.Sleep(500);
            }
            switch (model.Type)
            {
                case QuestionType.Nothing:
                    ShowMessage(model);
                    break;
                case QuestionType.Instruction:
                    IntructionAction(model);
                    break;
                case QuestionType.YesNo:
                    YesNoConfirmAction(model);
                    break;
                case QuestionType.AutoOk:
                    ClickOkAction(model);
                    break;
                case QuestionType.AutoYes:
                    ClickYesAction(model);
                    break;
            }
            if (!string.IsNullOrWhiteSpace(WindowTitle))
            {
                WindowControl.RaiseWindowProcess(WindowTitle);
            }
        }
        private static void DelayS(int time)
        {
            if (time > 0)
            {
                Thread.Sleep(time * 1000);
            }
        }

        private static void ClickYesAction(WorkerCheckerCommand model)
        {
            WindowControl.RaiseWindowProcess(model.Title);
            Thread.Sleep(200);
            DelayS(model.DelaySeconds);
            SendKeys.SendWait("y");
        }
        private static void ClickNoAction(WorkerCheckerCommand model)
        {
            WindowControl.RaiseWindowProcess(model.Title);
            Thread.Sleep(200);
            DelayS(model.DelaySeconds);
            SendKeys.SendWait("n");
        }
        private static void ClickOkAction(WorkerCheckerCommand model)
        {
            WindowControl.RaiseWindowProcess(model.Title);
            WindowControl.SetWindowToTop(model.Title);
            Thread.Sleep(200);
            DelayS(model.DelaySeconds);
            WindowControl.keybd_event((byte)Keys.Tab, 0, 0, 0);
            Thread.Sleep(20);
            WindowControl.keybd_event((byte)Keys.F2, 0, 2, 0);
            Thread.Sleep(20);
            WindowControl.keybd_event((byte)Keys.Space, 0, 0, 0);
            Thread.Sleep(20);
            WindowControl.keybd_event((byte)Keys.Space, 0, 2, 0);
        }

        private static void IntructionAction(WorkerCheckerCommand model)
        {
            ShowMessage(model);
            ClickOkAction(model);
        }

        private static void YesNoConfirmAction(WorkerCheckerCommand model)
        {
            if (ShowMessage(model))
            {
                ClickYesAction(model);
            }
            else
            {
                ClickNoAction(model);
            }
        }

        private static bool ShowMessage(WorkerCheckerCommand item)
        {
            MessageForm form = new MessageForm();
            bool rs = form.ShowMessage(item.Message, item.ImagePath, item.Type == QuestionType.YesNo);
            return rs;
        }

        private static bool IsShowConfirmWindow(string windowName)
        {
            return WindowControl.GetMainWindow(null, windowName) != IntPtr.Zero;
        }
    }
}
