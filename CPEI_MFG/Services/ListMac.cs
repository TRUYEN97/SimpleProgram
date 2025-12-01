using CPEI_MFG.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CPEI_MFG.Services
{
    public sealed class ListMac
    {
        private readonly HashSet<string> listMac;
        private static readonly Lazy<ListMac> _insatance = new Lazy<ListMac>(() => new ListMac());
        private ListMac()
        {
            listMac = new HashSet<string>();
        }
        public static ListMac Instance => _insatance.Value;
        public static bool IsEnable => ConfigLoader.ProgramConfig?.CheckListMac?.IsEnable == true;

        public static bool Init()
        {
            var config = ConfigLoader.ProgramConfig.CheckListMac;
            if (config?.IsEnable == true)
            {
                if (string.IsNullOrWhiteSpace(config.FilePath)
                    || !File.Exists(config.FilePath))
                {
                    return false;
                }
                return Instance.InitMac(config.FilePath);
            }
            return true;
        }

        public bool InitMac(string listMacPath)
        {
            if (!File.Exists(listMacPath))
            {
                return false;
            }
            listMac.Clear();
            foreach (var line in File.ReadAllLines(listMacPath))
            {
                AddMac(line);
            }
            return true;
        }
        public static bool ContainsMac(string mac)
        {
            return Instance.Contains(mac);
        }
        public bool Contains(string mac)
        {
            return IsEnable && !string.IsNullOrWhiteSpace(mac) && listMac.Contains(mac);
        }
        public void AddMac(string line)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                string mac = GetMacFrom(line);
                listMac.Add(mac);
            }
        }

        private static string GetMacFrom(string line)
        {
            string mac = line.ToUpper().Trim().Split('/').Last();
            if (mac.Length > 12)
            {
                mac = mac.Substring(0, 12);
            }
            return mac;
        }
    }
}
