using System;
using System.Collections.Generic;
using System.IO;
using NginxLogAnalyzer.Settings;

namespace NginxLogAnalyzer.Sources
{
    internal interface ILogSource
    {
        bool SourceMatches(string str);

        void ReadFile(string str, Action<Stream> parseSteamCallback, List<ISetting> settings);

        /// <summary>
        /// Removes any confidential informations for thew input for printing in the console
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        string GetSafeValuestring(string str);
    }
}
