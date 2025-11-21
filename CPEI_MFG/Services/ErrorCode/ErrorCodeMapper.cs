using System;
using System.IO;
using System.Text;
using CPEI_MFG.Common;
using CPEI_MFG.Communicate;
using CPEI_MFG.Config;
using CPEI_MFG.Service;

namespace CPEI_MFG.Services.ErrorCode
{
    public class ErrorCodeMapper
    {
        private readonly static Lazy<ErrorCodeMapper> instance = new Lazy<ErrorCodeMapper>(() => new ErrorCodeMapper());
        private readonly ErrorCodeModel model;
        private readonly SpecialErrorCode specialError;
        private readonly ErrorCodeAnalysis errorCodeAnalysis;
        private ErrorCodeMapperConfig _config;
        private ErrorCodeMapper()
        {
            _config = new ErrorCodeMapperConfig();
            model = new ErrorCodeModel() { MaxLength = _config.ErrorCodeMaxLength };
            specialError = new SpecialErrorCode() { MaxLength = _config.ErrorCodeMaxLength };
            errorCodeAnalysis = new ErrorCodeAnalysis(model, specialError) { MaxLength = _config.ErrorCodeMaxLength };
        }
        public static ErrorCodeMapper Instance => instance.Value;
        public ErrorCodeMapperConfig Config { get => _config; set => _config = value; }
        public SftpConfig SftpConfig { get => _config.SftpConfig; set => _config.SftpConfig = value; }
        public string RemoteDir { get => _config.RemoteDir; set => _config.RemoteDir = value; }
        public string Product { get => _config.Product; set => _config.Product = value; }
        public string Station { get => _config.Station; set => _config.Station = value; }
        public SpecialErrorCode SpecialErrorCode => specialError;
        public string RemoteNewFilePath => Path.Combine(RemoteDir, Product, Station, "newErrorCodes.csv");
        public string RemoteFilePath => Path.Combine(RemoteDir, "ErrorCodes.csv");

        public bool LoadErrorcodeFromServer()
        {
            if (!Util.Ping(_config.SftpConfig.Host, 10000))
            {
                return false;
            }
            using (var sftp = new MySftp(_config.SftpConfig))
            {
                if (!sftp.IsConnected)
                {
                    return false;
                }
                if (!sftp.TryReadAllLines(RemoteFilePath, out string[] lines))
                {
                    return false;
                }
                if (AddErrorCode(lines))
                {
                    CopyToLocal();
                    return true;
                }
                return false;
            }
        }

        public bool LoadErrorcodeFromFile()
        {
            bool rs = LoadFromLocal(_config.LocalFilePath);
            rs &= LoadFromLocal(_config.LocalNewFilePath);
            return rs;
        }

        private void CopyToLocal()
        {
            string filePath = _config.LocalFilePath;
            if (!File.Exists(filePath))
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var errorCode in model.Errorcodes)
                {
                    string line = $"{errorCode.Key};{errorCode.Value}".ToUpper();
                    stringBuilder.AppendLine(line);
                }
                FileUtil.WriteAllText(filePath, stringBuilder.ToString());
            }
        }

        private bool LoadFromLocal(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return false;
            }
            string[] lines = FileUtil.ReadAllLines(filePath);
            return AddErrorCode(lines);
        }
        public string GetTestResultStr(string logText)
        {
            return ErrorCodeAnalysis.GetTestResultStr(logText);
        }

        public bool TryGetErrorcode(string logText, out string functionName, out string errorcode)
        {
            try
            {
                functionName = errorcode = string.Empty;
                if (string.IsNullOrWhiteSpace(logText) || logText.Contains("] TEST SUMMARY: SKIP"))
                {
                    functionName = "N/A";
                    errorcode = "SKIP";
                    return true;
                }
                if (logText.Contains("] TEST SUMMARY: PASS"))
                {
                    functionName = "";
                    errorcode = "";
                    return false;
                }
                if (errorCodeAnalysis.TryGetErrorCode(logText, out functionName, out errorcode) && !string.IsNullOrWhiteSpace(errorcode))
                {
                    return true;
                }
                errorcode = errorCodeAnalysis.CreateNewErrorcode(functionName);
                UpdateNewErrorCode(functionName, errorcode);
                return !string.IsNullOrWhiteSpace(errorcode);
            }
            catch (Exception)
            {
                functionName = null;
                errorcode = null;
                return false;
            }
        }

        public bool TryGetErrorcode(string logText, out string errorcode)
        {
            return TryGetErrorcode(logText, out _, out errorcode);
        }

        private void UpdateNewErrorCode(string functionName, string newErrorcode)
        {
            model.Set(functionName, newErrorcode);
            string line = $"{functionName};{newErrorcode}".ToUpper();
            UpdateToLocalFile(line);
            UpdateToSftpFile(line);
        }

        private void UpdateToSftpFile(string line)
        {
            using (var sftp = new MySftp(Instance._config.SftpConfig))
            {
                if (sftp.Connect())
                {
                    sftp.AppendLine(RemoteNewFilePath, line);
                }
            }
        }

        private void UpdateToLocalFile(string line)
        {
            string filePath = _config.LocalNewFilePath;
            FileUtil.WriteAllText(filePath, $"{line}\r\n", true);
        }

        private bool AddErrorCode(string[] lines)
        {
            if (lines == null) return false;
            foreach (var line in lines)
            {
                string[] elems = line.Split(new char[] { ';' });
                if (elems.Length >= 2)
                {
                    string funcName = elems[0];
                    string errorcode = elems[1];
                    model.Set(funcName, errorcode);
                }
            }
            return true;
        }
    }
}
