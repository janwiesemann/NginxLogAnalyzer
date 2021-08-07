using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NginxLogAnalyzer.Analyzer;
using NginxLogAnalyzer.Filter;
using NginxLogAnalyzer.Sources;

namespace NginxLogAnalyzer
{
    internal static class Setup
    {
        public static List<AccessEntryFilterBase> GetAccesEntryFilers(string[] args)
        {
            return new List<AccessEntryFilterBase>
            {
                new AccessEntryFilterFieldMatchesValue("Address", e => e.RemoteAddress),
                new AccessEntryFilterAccessTime()
            };
        }

        public static List<ILogSource> GetLogSources(string[] args)
        {
            return new List<ILogSource>
            {
                new LogDirectorySource(),
                new LogFileSource()
            };
        }

        public static string GetDefaultLogDir()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "/usr/local/var/log/nginx/";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "/var/log/nginx/";

            return null;
        }

        internal static List<IAnalyzer> GetAnalyzers(string[] args)
        {
            return new List<IAnalyzer>
            {
                new MostRequestedPagesAnalyzer(),
                new MostRequestsByAddressAnalyzer()
            };
        }
    }
}
