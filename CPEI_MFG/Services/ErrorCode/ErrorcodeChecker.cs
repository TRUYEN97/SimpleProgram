using System;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using CPEI_MFG.Config;
using CPEI_MFG.Config.Errorcode;

namespace CPEI_MFG.Services.ErrorCode
{
    public class ErrorcodeChecker : BaseInApprox<ErrorcodeModelConfig>
    {
        private readonly StringBuilder stringBuilder;
        private readonly ErrorCodeMapper errorCodeMapper;
        public ErrorcodeChecker()
        {
            stringBuilder = new StringBuilder();
            errorCodeMapper = ErrorCodeMapper.Instance;
        }

        public override bool IsEnable { get; protected set; }
        public override bool Init()
        {
            try
            {
                var programConfig = ConfigLoader.ProgramConfig;
                var config = programConfig.ErrorCodeConfig;
                if (config == null)
                {
                    MessageBox.Show("ErrorCodeConfig not config!");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(programConfig.Model))
                {
                    MessageBox.Show("Model not config!");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(programConfig.Station))
                {
                    MessageBox.Show("Station not config!");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(config.LocalFilePath))
                {
                    MessageBox.Show("ErrorCode LocalFilePath not config!");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(config.LocalNewFilePath))
                {
                    MessageBox.Show("ErrorCode LocalFilePath not config!");
                    return false;
                }
                errorCodeMapper.Config = new ErrorCodeMapperConfig
                {
                    SftpConfig = config.SftpConfig,
                    Product = programConfig.Model,
                    Station = programConfig.Station,
                    ErrorCodeMaxLength = config.MaxLength,
                    LocalFilePath = config.LocalFilePath,
                    LocalNewFilePath = config.LocalNewFilePath,
                };
                errorCodeMapper.LoadErrorcodeFromFile();
                errorCodeMapper.LoadErrorcodeFromServer();
                models.Clear();
                foreach (var item in config.Configs)
                {
                    models.Add(item);
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public override void Check(string line)
        {
            stringBuilder.AppendLine(line);
            base.Check(line);
        }



        public override bool TryGetErrorcode(out string errorcode)
        {
            string logResult = errorCodeMapper.GetTestResultStr(stringBuilder.ToString());
            return base.TryGetErrorcode(out errorcode) || errorCodeMapper.TryGetErrorcode(logResult, out errorcode);
        }

        public override void Reset()
        {
            stringBuilder.Clear();
            base.Reset();
        }
        protected override void OnTriggerStart(ErrorcodeModelConfig model, string line)
        {
            errorCodes.Add(model.ErrorCode);
        }
    }
}
