using System;
using System.IO;
using System.Collections.Generic;
using NginxLogAnalyzer.Analyzer;
using NginxLogAnalyzer.Sources;
using NginxLogAnalyzer.Filter;

namespace NginxLogAnalyzer
{
    static class MainClass
    {
        public static void Main(string[] args)
        {
            List<ILogSource> sources = Setup.GetLogSources(args);
            List<AccessEntryFilterBase> accessEntryFilters = Setup.GetAccesEntryFilers(args);

            WriteHeader("Source");
            Dictionary<string, ILogSource> sourceParamAndSource = ParseLogSources(args, sources);
            if (sourceParamAndSource.Count == 0 && AddDefaultSource(sourceParamAndSource))
                return;

            WriteHeader("Reading");
            List<RemoteAddress> addresses = AccessLogParser.ReadSources(sourceParamAndSource, accessEntryFilters);
            if (addresses == null)
                return;

            List<char> switches = ParseSwitches(args);
            List<IAnalyzer> analyzers = Setup.GetAnalyzers(args);

            bool executeAll = switches.Count == 0 || switches.HasSwitch('A');
            foreach (IAnalyzer analyzer in analyzers)
            {
                if (!executeAll && !analyzer.CanExecute(switches))
                    continue;

                WriteHeader(analyzer.Name);

                try
                {
                    analyzer.Execute(addresses);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            WriteHeader("Done");
        }

        private static bool AddDefaultSource(Dictionary<string, ILogSource> sourceParamAndSource)
        {
            Console.WriteLine("No sources found!");

            string dir = Setup.GetDefaultLogDir();
            if (dir == null || !Directory.Exists(dir))
            {
                Console.WriteLine("OS default can not be used!");

                return false;
            }

            Console.WriteLine("Using " + dir);
            sourceParamAndSource.Add(dir, new LogDirectorySource());

            return true;
        }

        private static Dictionary<string, ILogSource> ParseLogSources(string[] args, IEnumerable<ILogSource> sources)
        {
            Dictionary<string, ILogSource> ret = new Dictionary<string, ILogSource>();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("-"))
                    continue;

                bool foundSource = false;
                foreach (ILogSource item in sources)
                {
                    if (item.SourceMatches(args[i]))
                    {
                        ret.TryAdd(args[i], item);

                        Console.WriteLine($"Using {item.GetType().Name} as source handler for {args[i]}");

                        foundSource = true;

                        break;
                    }
                }

                if (!foundSource)
                    Console.WriteLine("Found no valid source handler for " + args[i]);
            }

            return ret;
        }

        private static List<char> ParseSwitches(string[] args)
        {
            List<char> ret = new List<char>();

            for (int i = 0; i < args.Length; i++)
            {
                if (!args[i].StartsWith("-") || args[i].StartsWith("--"))
                    continue;

                for (int j = 1; j < args[i].Length; j++)
                    ret.Add(args[i][j]);
            }

            return ret;
        }               

        private static void WriteHeader(string name)
        {
            Console.WriteLine();

            for (int i = 0; i < Console.WindowWidth-1; i++)
                Console.Write('=');

            Console.WriteLine('=');
            Console.WriteLine(name);
            Console.WriteLine();
        }
    }
}
