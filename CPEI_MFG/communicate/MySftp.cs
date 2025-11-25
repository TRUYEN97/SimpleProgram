using CPEI_MFG.Config;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;

namespace CPEI_MFG.Communicate
{
    public class MySftp
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _user;
        private readonly string _password;

        public MySftp(string host, int port, string user, string password)
        {
            _host = host;
            _port = port;
            _user = user;
            _password = password;
        }

        public MySftp(SftpConfig sftpConfig) : this(sftpConfig.Host, sftpConfig.Port, sftpConfig.User, sftpConfig.Password) { }
        private T UseClient<T>(Func<SftpClient, T> action)
        {
            using (var client = new SftpClient(_host, _port, _user, _password))
            {
                client.KeepAliveInterval = TimeSpan.FromSeconds(10);
                client.Connect();

                T result = action(client);

                client.Disconnect();
                return result;
            }
        }

        private void UseClient(Action<SftpClient> action)
        {
            using (var client = new SftpClient(_host, _port, _user, _password))
            {
                client.KeepAliveInterval = TimeSpan.FromSeconds(10);
                client.Connect();

                action(client);

                client.Disconnect();
            }
        }

        // ------------------------
        // ---- FILE OPERATIONS ---
        // ------------------------

        public bool Exists(string remotePath)
        {
            return UseClient(client => client.Exists(remotePath));
        }

        public string ReadAllText(string remotePath)
        {
            return UseClient(client =>
            {
                if (!client.Exists(remotePath))
                    return null;

                return client.ReadAllText(remotePath);
            });
        }

        public bool TryReadAllLines(string remoteFilePath, out string[] lines)
        {
            lines = ReadAllLines(remoteFilePath);
            return lines?.Length > 0;
        }

        public string[] ReadAllLines(string remotePath)
        {
            return UseClient(client =>
            {
                if (!client.Exists(remotePath))
                    return null;

                return client.ReadAllLines(remotePath);
            });
        }

        public bool WriteAllText(string remotePath, string content)
        {
            return UseClient(client =>
            {
                CreateDirectoryInternal(client, Path.GetDirectoryName(remotePath));
                client.WriteAllText(remotePath, content);
                return true;
            });
        }

        public bool AppendAllText(string remotePath, string content)
        {
            return UseClient(client =>
            {
                CreateDirectoryInternal(client, Path.GetDirectoryName(remotePath));
                client.AppendAllText(remotePath, content);
                return true;
            });
        }

        public bool AppendLine(string remotePath, string content)
        {
            return AppendAllText(remotePath, content + "\r\n");
        }

        public bool DeleteFile(string remotePath)
        {
            return UseClient(client =>
            {
                if (client.Exists(remotePath))
                    client.DeleteFile(remotePath);

                return true;
            });
        }

        // -------------------------
        // ---- UPLOAD/DOWNLOAD ----
        // -------------------------

        public bool UploadFile(string remotePath, string localPath)
        {
            return UseClient(client =>
            {
                if (!File.Exists(localPath))
                    return false;

                CreateDirectoryInternal(client, Path.GetDirectoryName(remotePath));

                using (var fs = new FileStream(localPath, FileMode.Open))
                {
                    client.UploadFile(fs, remotePath, true);
                }

                return true;
            });
        }

        public bool DownloadFile(string remotePath, string localPath)
        {
            return UseClient(client =>
            {
                if (!client.Exists(remotePath))
                    return false;

                Directory.CreateDirectory(Path.GetDirectoryName(localPath));

                using (var fs = new FileStream(localPath, FileMode.Create))
                {
                    client.DownloadFile(remotePath, fs);
                }

                return true;
            });
        }

        // -------------------------
        // ---- DIRECTORY  ---------
        // -------------------------

        private void CreateDirectoryInternal(SftpClient client, string remotePath)
        {
            if (string.IsNullOrWhiteSpace(remotePath))
                return;

            var parts = remotePath.Replace('\\', '/').Split('/');

            string current = string.Empty;

            foreach (var part in parts)
            {
                if (string.IsNullOrWhiteSpace(part))
                    continue;

                current += "/" + part;

                if (!client.Exists(current))
                    client.CreateDirectory(current);
            }
        }

        public bool CreateDirectory(string remotePath)
        {
            return UseClient(client =>
            {
                CreateDirectoryInternal(client, remotePath);
                return true;
            });
        }

        public List<string> ListDirectory(string remotePath)
        {
            return UseClient(client =>
            {
                var list = new List<string>();

                foreach (var file in client.ListDirectory(remotePath))
                {
                    if (file.Name != "." && file.Name != "..")
                        list.Add(file.FullName);
                }

                return list;
            });
        }

        public bool DeleteFolder(string remotePath)
        {
            return UseClient(client =>
            {
                if (!client.Exists(remotePath))
                    return false;

                foreach (var file in client.ListDirectory(remotePath))
                {
                    if (file.Name == "." || file.Name == "..")
                        continue;

                    if (file.IsDirectory)
                        DeleteFolder(file.FullName);
                    else
                        client.DeleteFile(file.FullName);
                }

                client.DeleteDirectory(remotePath);
                return true;
            });
        }
    }
}
