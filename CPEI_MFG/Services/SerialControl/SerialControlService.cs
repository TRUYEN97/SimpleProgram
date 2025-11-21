using CPEI_MFG.Config;
using CPEI_MFG.Config.SerialControl;
using System;
using System.Linq;
using System.Windows.Forms;

namespace CPEI_MFG.Services.SerialControl
{
    public class SerialControlService : BaseSerialControl<SerialControlCommand>
    {
        private SerialControlConfig _config;
        public override bool IsEnable { get; protected set; }

        public override bool Init()
        {
            try
            {
                var appSettingModel = ConfigLoader.ProgramConfig;
                _config = appSettingModel.SerialControl;
                if (_config == null)
                {
                    MessageBox.Show($"{Name} not config!");
                    return false;
                }
                models.Clear();
                foreach (var item in _config.Commands)
                {
                    models.Add(item);
                }
                IsEnable = _config.IsEnable;
                Name = _config.Name;
                if (!IsEnable)
                {
                    return true;
                }
                if (string.IsNullOrWhiteSpace(_config.Com))
                {
                    MessageBox.Show($"The {Name} serial name is not config!");
                    return false;
                }
                return ConnectToSerial(_config.Com, _config.Baudrate);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool StartTest()
        {
            if (!IsEnable)
            {
                return true;
            }
            if (IsConnect && _config?.StartTestCommands?.Count > 0)
            {
                return RunCommand(_config.StartTestCommands.Select(i => i as ISerialCommand).ToList());
            }
            return false;
        }

        public bool EndTest()
        {
            if (!IsEnable)
            {
                return true;
            }
            if (IsConnect && _config?.EndTestCommands?.Count > 0)
            {
                return RunCommand(_config.EndTestCommands.Select(i => i as ISerialCommand).ToList());
            }
            return false;
        }

        protected override bool IsCorrectValue(string value, string target)
        {
            if (string.IsNullOrEmpty(target))
            {
                return true;
            }
            return value?.Equals(target, StringComparison.OrdinalIgnoreCase) == true;
        }

        protected override void AttackCommand(SerialControlCommand model, string line)
        {
            RunCommand(model.Commands.Select(i => i as ISerialCommand).ToList());
        }

        protected override string CreateCommnand(ISerialCommand model)
        {
            return model.Command;
        }

        protected override bool IsCanWait(ISerialCommand model)
        {
            return true;
        }
    }
}
