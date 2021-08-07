using System;
using System.IO;
using System.IO.Compression;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace NginxLogAnalyzer.Sources
{
    internal class LogSFTPSource : ILogSource
    {
        public void ReadFile(string str, Action<Stream> parseSteamCallback)
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

            using(SftpClient client = new SftpClient(uri.Host, username, password))
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
