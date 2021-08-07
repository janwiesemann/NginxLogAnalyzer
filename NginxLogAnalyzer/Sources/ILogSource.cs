using System;
using System.IO;

namespace NginxLogAnalyzer.Sources
{
    internal interface ILogSource
    {
        bool SourceMatches(string str);

        void ReadFile(string str, Action<Stream> parseSteamCallback);
    }
}
