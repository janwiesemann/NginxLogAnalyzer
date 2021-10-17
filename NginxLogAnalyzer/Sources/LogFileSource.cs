using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using NginxLogAnalyzer.Settings;

namespace NginxLogAnalyzer.Sources
{
    internal class LogFileSource : ILogSource
    {
        private static Stream OpenFile(string path)
        {
            string extension = Path.GetExtension(path).ToUpper();

            Stream ret = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            if (extension == ".GZ")
                return new GZipStream(ret, CompressionMode.Decompress);

            return ret;
        }

        public string GetSafeValuestring(string str) => str;

        public void ReadFile(string str, Action<Stream> parseSteamCallback, List<ISetting> settings)
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
