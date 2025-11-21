using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using CPEI_MFG.Timer;

namespace CPEI_MFG.Common
{
    public static class FileUtil
    {
        public static void WriteAllText(string filePath, string text, bool append = false)
        {
            CreateDirectory(filePath);
            if (append)
            {
                File.AppendAllText(filePath, text);
            }
            else
            {
                File.WriteAllText(filePath, text);
            }
        }
        public static void WriteAllText(string filePath, ISerializable text, bool append = false)
        {
            CreateDirectory(filePath);
            if (append)
            {
                File.AppendAllText(filePath, text?.ToString());
            }
            else
            {
                File.WriteAllText(filePath, text?.ToString());
            }
        }
        public static void CopyFile(string fileSource, string fileTaget)
        {
            CreateDirectory(fileTaget);
            File.Copy(fileSource, fileTaget);
        }

        private static void CreateDirectory(string fileTaget)
        {
            string dir = Path.GetDirectoryName(fileTaget);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public static void TailLogFile(string path, Func<string, bool> IsOutReadAction, Stopwatch stopwatch = null, bool isJustNewLine = false, CancellationToken token = default)
        {
            using (FileStream fs = new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(fs))
            {
                if (isJustNewLine)
                {
                    fs.Seek(0, SeekOrigin.End);
                }
                stopwatch?.Reset();
                while (IsOutReadAction != null && (stopwatch == null || stopwatch.IsOntime == true) && (token == default || !token.IsCancellationRequested))
                {
                    string line = reader.ReadLine();
                    if (line != null)
                    {
                        if (IsOutReadAction.Invoke(line))
                        {
                            break;
                        }
                    }
                    else
                    {
                        Thread.Sleep(200);
                    }
                }
            }
        }

        public static string ReadAllText(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return null;
            }
            return File.ReadAllText(filePath);
        }

        public static string[] ReadAllLines(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return null;
            }
            return File.ReadAllLines(filePath);
        }

        public static byte[] ReadAllByte(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return null;
            }
            return File.ReadAllBytes(filePath);
        }
    }
}
