using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using NginxLogAnalyzer.Settings;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace NginxLogAnalyzer.Sources
{
    internal class LogSFTPSource : ILogSource
    {
        public string GetSafeValuestring(string str)
        {
            int i = str.IndexOf(':');
            int j = str.IndexOf('@', i);

            return str.Substring(0, i + 1) + "***" + str.Substring(j);
        }

        private static SftpClient GetNewClient(string host, int port, string username, string password, PrivateKeyFile[] keyFiles)
        {
            if (!string.IsNullOrEmpty(password))
                return new SftpClient(host, port, username, password);

            if (keyFiles == null || keyFiles.Length == 0)
                throw new NotSupportedException("Login without password or key is not supported!");

            return new SftpClient(host, port, username, keyFiles);
        }

        public void ReadFile(string str, Action<Stream> parseSteamCallback, List<ISetting> settings)
        {
            Uri uri = new Uri(str, UriKind.Absolute);
            string username = uri.UserInfo;
            if (username == null)
                throw new ArgumentException("sftp user info missing!");

            string password = null;
            int i = username.IndexOf(':');
            if(i > 0)
            {
                password = username.Substring(i + 1);
                username = username.Substring(0, i);
            }

            PrivateKeyFile[] keyFiles = settings.TryGetValues<PrivateKeyFile>("keyFile").ToArray();

            using(SftpClient client = GetNewClient(uri.Host, uri.IsDefaultPort ? 22 : uri.Port, username, password, keyFiles))
            {
                client.Connect();

                foreach (SftpFile item in client.ListDirectory(uri.AbsolutePath))
                {
                    if (!item.IsRegularFile)
                        continue;

                    if (item.Name.IndexOf("access") != 0)
                        continue;

                    using(MemoryStream stream = new MemoryStream((int)item.Length))
                    {
                        client.DownloadFile(item.FullName, stream);
                        stream.Position = 0;

                        Stream actualStream;
                        if (item.Name.IndexOf(".gz", StringComparison.OrdinalIgnoreCase) == item.Name.Length - 3)
                            actualStream = new GZipStream(stream, CompressionMode.Decompress);
                        else
                            actualStream = stream;

                        parseSteamCallback(actualStream);
                    }
                }
            }
        }

        public bool SourceMatches(string str)
        {
            if (!Uri.TryCreate(str, UriKind.Absolute, out Uri res))
                return false;

            return res.Scheme?.ToUpper() == "SFTP";
        }
    }
}
