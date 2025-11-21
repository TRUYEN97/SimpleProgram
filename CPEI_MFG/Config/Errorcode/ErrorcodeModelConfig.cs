using System;
namespace CPEI_MFG.Config.Errorcode
{
    public class ErrorcodeModelConfig: AbsApproxCommand
    {
        public string ErrorCode { get; set; } = string.Empty;
        internal override bool IsEnable => base.IsEnable && !string.IsNullOrWhiteSpace(Keywork) && !string.IsNullOrWhiteSpace(ErrorCode);
    }
}
