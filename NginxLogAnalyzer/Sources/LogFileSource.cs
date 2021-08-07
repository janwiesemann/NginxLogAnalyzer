using System;
using System.IO;
using System.IO.Compression;

namespace NginxLogAnalyzer.Sources
{
    public class LogFileSource : ILogSource
    {
        private static Stream OpenFile(string path)
        {
            string extension = Path.GetExtension(path).ToUpper();

            Stream ret = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            if (extension == ".GZ")
                return new GZipStream(ret, CompressionMode.Decompress);

            return ret;
        }

        public void ReadFile(string str, Action<Stream> parseSteamCallback)
        {
            using (Stream stream = OpenFile(str))
                parseSteamCallback(stream);
        }

        public bool SourceMatches(string str)
        {
            return !Directory.Exists(str) && File.Exists(str);
        }
    }
}
