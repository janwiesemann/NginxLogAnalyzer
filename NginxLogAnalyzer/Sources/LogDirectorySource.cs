using System;
using System.IO;

namespace NginxLogAnalyzer.Sources
{
    internal class LogDirectorySource : ILogSource
    {
        private readonly LogFileSource fileSource = new LogFileSource();

        public string GetSafeValuestring(string str) => str;

        public void ReadFile(string str, Action<Stream> parseSteamCallback)
        {
            foreach (string item in Directory.GetFiles(str))
            {
                string name = Path.GetFileName(item);
                if (name.IndexOf("access", StringComparison.OrdinalIgnoreCase) != 0)
                    continue;

                fileSource.ReadFile(item, parseSteamCallback);
            }
        }

        public bool SourceMatches(string str)
        {
            return Directory.Exists(str);
        }
    }
}
