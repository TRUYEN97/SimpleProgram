using System;
using System.Text;
using CPEI_MFG.Common;

namespace CPEI_MFG.Services.ErrorCode
{
    internal class ErrorCodeAnalysis
    {
        private const string FIND_CODE_REGEX = @" Fail Items:\s*(\d+)\s*";
        private const string FIND_FUNCTION_NAME_REGEX = @"\s{2}(\[.+\]\s.*?)\s+\d+\s{7}\d+\/\d+";
        private const string FIND_FUNCTION_NAME_REGEX1 = @"\s{2}(.*?)\s+\d+\s{7}\d+\/\d+";
        private const string KEYWORD = "===Test report================";

        private readonly ErrorCodeModel _errorCodeModel;
        private readonly SpecialErrorCode _specialErrorCode;
        public int MaxLength { get; set; }

        public ErrorCodeAnalysis(ErrorCodeModel errorCodeModel, SpecialErrorCode specialErrorCode)
        {
            _errorCodeModel = errorCodeModel;
            _specialErrorCode = specialErrorCode;
            MaxLength = Int16.MaxValue;
        }

        public bool TryGetErrorCode(string logText, out string funcName, out string errorcode)
        {
            funcName = string.Empty;
            errorcode = string.Empty;

            if (string.IsNullOrWhiteSpace(logText))
                return false;
            string failCode = GetCode(logText);
            funcName = GetFunctionName(logText, failCode);
            if (!_errorCodeModel.TryGet(funcName, out string baseErrorcode))
            {
                baseErrorcode = string.Empty;
            }
            if (_specialErrorCode.IsSpecial(funcName, logText, baseErrorcode, out errorcode) && !string.IsNullOrWhiteSpace(errorcode))
            {
                return true;
            }
            errorcode = baseErrorcode;
            return !string.IsNullOrWhiteSpace(errorcode);
        }

        public string CreateNewErrorcode(string functionName)
        {
            StringBuilder newErrorBuilder = new StringBuilder();
            if (string.IsNullOrEmpty(functionName))
            {
                for (int i = 0; i < MaxLength; i++)
                {
                    newErrorBuilder.Append('F');
                }
            }
            else
            {
                foreach (var c in functionName)
                {
                    if (newErrorBuilder.Length >= MaxLength)
                    {
                        break;
                    }
                    if ((c >= 0 && c <= 9) || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                    {
                        newErrorBuilder.Append(c);
                    }
                }
            }
            return newErrorBuilder.ToString().ToUpper();
        }

        public static string GetCode(string logText)
        {
            return Util.FindGroup(logText, FIND_CODE_REGEX);
        }

        public static string GetFunctionName(string logText, string faildCode)
        {
            if (faildCode == null)
            {
                return null;
            }
            var name = Util.FindGroup(logText, $"\\s{faildCode}{FIND_FUNCTION_NAME_REGEX}");
            return name ?? Util.FindGroup(logText, $"\\s{faildCode}{FIND_FUNCTION_NAME_REGEX1}");
        }

        public static string GetTestResultStr(string log)
        {
            if (string.IsNullOrEmpty(log))
            {
                return null;
            }
            int index = log.LastIndexOf(KEYWORD);
            if (index == -1)
            {
                return null;
            }
            return log.Substring(index + KEYWORD.Length);
        }
    }
}
