using CPEI_MFG.Common;
using System;

namespace CPEI_MFG.Service.Condition
{
    public class CheckTestFailed
    {
        private const string ERROR_KEY = "ERROR_KEY";
        private const string COUNT_KEY = "COUNT_KEY";
        private const string MAC_KEY = "MAC_KEY";
        private readonly RegistryUtil registry;

        public CheckTestFailed(int index)
        {
            registry = new RegistryUtil($"Software\\CPEI_MFG\\Unit{index}\\ContinueFaile");
        }
        public bool EnableOldMac { get; set; }
        public bool EnableFailCheck { get; set; }
        public int Spec { get; set; }
        public bool IsFailedTimeOutOfSpec => EnableFailCheck && Count >= Spec;

        public bool IsOldMac(string mac)
        {
            if (!EnableOldMac)
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(OldMac) || string.IsNullOrWhiteSpace(mac))
            {
                return false;
            }
            if (OldMac.Equals(mac, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        public void SetFailed(string errorcode)
        {
            if (string.IsNullOrWhiteSpace(errorcode))
            {
                return;
            }
            if (ErrorCode != errorcode)
            {
                ErrorCode = errorcode;
                Count = 1;
            }
            else
            {
                Count++;
            }

        }
        public void SetPass()
        {
            ErrorCode = "";
            OldMac = "";
            Count = 0;
        }
        public string ErrorCode
        {
            get
            {
                return registry.GetValue<string>(ERROR_KEY, null);
            }
            private set
            {
                registry.SaveStringValue(ERROR_KEY, value ?? "");
            }
        }
        public string OldMac
        {
            get
            {
                return registry.GetValue<string>(MAC_KEY, null);
            }
            set
            {
                registry.SaveStringValue(MAC_KEY, value ?? "");
            }
        }

        public int Count
        {
            get
            {
                return registry.GetValue(COUNT_KEY, 0);
            }
            private set
            {
                registry.SaveIntValue(COUNT_KEY, value < 0 ? 0 : value);
            }
        }

    }
}
