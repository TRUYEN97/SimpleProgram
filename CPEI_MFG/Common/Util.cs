using CPEI_MFG.Timer;
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using UiTest.Service.Communicate.Implement.Cmd;

namespace CPEI_MFG.Common
{
    public static class Util
    {
        public static string GetStringBetween(string sInput, string sFrom, string sTo)
        {
            int iFrom = sInput.IndexOf(sFrom) + sFrom.Length;
            int iTo = sInput.LastIndexOf(sTo);
            return sInput.Substring(iFrom, iTo - iFrom).Trim();
        }
        public static bool Ping(string ip, int timeOut)
        {
            using (CmdProcess cmd = new CmdProcess(false, $"arp -d {ip}"))
            {
                cmd.WaitForExit();
            }
            Ping devicesPing = new Ping();
            Stopwatch stopwatch = new Stopwatch(timeOut);
            while (stopwatch.IsOntime)
            {
                if (devicesPing.Send(ip).Status == IPStatus.Success)
                {
                    return true;
                }
            }
            return false;
        }
        public static string FindGroup(string str, string regex)
        {
            if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(regex))
            {
                Match match = Regex.Match(str, regex);
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
            }
            return null;
        }
        public static uint ToUint(IPAddress ip) { return BitConverter.ToUInt32(ip.GetAddressBytes().Reverse().ToArray(), 0); }
        public static IPAddress FromUint(uint u) { return new IPAddress(BitConverter.GetBytes(u).Reverse().ToArray()); }
        public static string NormalizeMac(string mac)
        {
            if (string.IsNullOrWhiteSpace(mac))
                return null;
            string cleaned = Regex.Replace(mac, @"[^0-9A-Fa-f]", "");
            if (cleaned.Length != 12)
                return null;
            StringBuilder macStr = new StringBuilder(cleaned.Substring(0, 2));
            for (int i = 2; i < 12; i += 2)
            {
                macStr.Append(":");
                macStr.Append(cleaned.Substring(i, 2));
            }
            return macStr.ToString().ToUpper();
        }
    }
}
