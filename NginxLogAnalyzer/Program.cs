using System;
using System.IO;
using System.Collections.Generic;
using NginxLogAnalyzer.Analyzer;
using NginxLogAnalyzer.Sources;
using NginxLogAnalyzer.Filters;
using NginxLogAnalyzer.Settings;
using NginxLogAnalyzer.Parser;

namespace NginxLogAnalyzer
{
    static class MainClass
    {
        public static void Main(string[] args)
        {
            WriteHeader("Source");
            List<ILogSource> sources = Setup.GetLogSources(args);
            Dictionary<string, ILogSource> sourceParamAndSource = ParseLogSources(args, sources);
            if (sourceParamAndSource.Count == 0 && AddDefaultSource(sourceParamAndSource))
                return;

            WriteHeader("Filter");
            List<IFilter> accessEntryFilters = Setup.GetAccesEntryFilers(args);
            accessEntryFilters.ParseValues(args);

            WriteHeader("Settings");
            List<ISetting> settings = Setup.GetSettings(args);
            settings.ParseValues(args);

            WriteHeader("Switches");
            List<char> switches = ParseSwitches(args);
            if (switches.Count == 0)
                Console.WriteLine("-A");

            WriteHeader("Format");
            List<ITextBlock> formatBlocks = ParseFormat(settings);
            if (formatBlocks == null)
                return;

            WriteHeader("Reading");
            List<RemoteAddress> addresses = LogParser.ReadSources(sourceParamAndSource, accessEntryFilters, formatBlocks, settings);
            if (addresses == null)
                return;

            List<IAnalyzer> analyzers = Setup.GetAnalyzers(args);

            bool executeAll = switches.Count == 0 || switches.HasSwitch('A');
            foreach (IAnalyzer analyzer in analyzers)
            {
                if (!executeAll && !analyzer.CanExecute(switches))
                    continue;

                WriteHeader(analyzer.Name);

                try
                {
                    analyzer.Execute(addresses, settings);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            WriteHeader("Done");
        }

        private static List<ITextBlock> ParseFormat(List<ISetting> settings)
        {
            if (!settings.TryGetValue("format", out string format))
                throw new InvalidOperationException();

            Console.WriteLine(format);

            return FormatParser.ParseFormat(format);                    
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

                        Console.WriteLine($"Using {item.GetType().Name} as source handler for {item.GetSafeValuestring(args[i])}");

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
                {
                    char switchChar = args[i][j];

                    if (!ret.Contains(switchChar))
                    {
                        Console.WriteLine($"-{switchChar}");

                        ret.Add(switchChar);
                    }
                }
            }

            return ret;
        }               

        private static void WriteHeader(string name)
        {
            Console.WriteLine();

            for (int i = 0; i < Math.Max(Console.WindowWidth - 1, 16); i++)
                Console.Write('=');

            Console.WriteLine('=');
            Console.WriteLine(name);
            Console.WriteLine();
        }
    }
}
