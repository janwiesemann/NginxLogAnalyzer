using System;
using System.Collections.Generic;
using System.IO;
using NginxLogAnalyzer.Settings;

namespace NginxLogAnalyzer.Sources
{
    internal class LogDirectorySource : ILogSource
    {
        private readonly LogFileSource fileSource = new LogFileSource();

        public string GetSafeValuestring(string str) => str;

        public void ReadFile(string str, Action<Stream> parseSteamCallback, List<ISetting> settings)
        {
            foreach (string item in Directory.GetFiles(str))
            {
                string name = Path.GetFileName(item);
                if (name.IndexOf("access", StringComparison.OrdinalIgnoreCase) != 0)
                    continue;

                fileSource.ReadFile(item, parseSteamCallback, settings);
            }
        }

        public bool SourceMatches(string str)
        {
            return Directory.Exists(str);
        }
    }
}
