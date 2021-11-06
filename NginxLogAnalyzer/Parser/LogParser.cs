using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NginxLogAnalyzer.Filters;
using NginxLogAnalyzer.Settings;
using NginxLogAnalyzer.Sources;

namespace NginxLogAnalyzer.Parser
{
    internal static class LogParser
    {
        public static List<RemoteAddress> ReadSources(Dictionary<string, ILogSource> sourceParamAndSource, List<IFilter> accessEntryFilters, List<ITextBlock> format, List<ISetting> settings)
        {
            int totalRequestCount = 0;
            int totalRequestCountWithMatch = 0;

            Dictionary<string, RemoteAddress> ret = new Dictionary<string, RemoteAddress>();
            foreach (KeyValuePair<string, ILogSource> item in sourceParamAndSource)
            {
                DateTime start = DateTime.Now;
                Console.Write($"Reading source {item.Value.GetType().Name} ({item.Value.GetSafeValuestring(item.Key)})...");

                try
                {
                    item.Value.ReadFile(item.Key, stream => ParseStream(stream, ret, accessEntryFilters, ref totalRequestCount, ref totalRequestCountWithMatch, format), settings);

                    Console.WriteLine($" finished in {Math.Round((DateTime.Now - start).TotalMilliseconds, 1)}ms");
                }
                catch (Exception ex)
                {
                    Console.Write("Error: ");
                    Console.WriteLine(ex);
                }

            }

            Console.WriteLine();
            Console.WriteLine($"Found {ret.Count} unique addresses with a total of {totalRequestCountWithMatch} (unfiltered {totalRequestCount}) requests.");

            return ret.GetValuesAsList();
        }

        private static bool AccessEntryMatchesFilters(AccessEntry entry, IEnumerable<IFilter> accessEntryFilters)
        {
            Dictionary<FilterGroups, bool> resuls = new Dictionary<FilterGroups, bool>();
            foreach (IFilter item in accessEntryFilters)
            {
                if (!item.HasValue)
                    continue;

                bool hasKey = resuls.TryGetValue(item.Group, out bool res);
                if (hasKey && res) //One filter of this group already matched. Further checks are not needet
                    continue;

                res = item.Matches(entry);
                if (hasKey)
                    resuls[item.Group] = res;
                else
                    resuls.Add(item.Group, res);
            }

            //checks if one filter is false
            foreach (var item in resuls)
            {
                if (!item.Value)
                    return false;
            }

            return true;
        }

        private static void ParseStream(Stream stream, Dictionary<string, RemoteAddress> addresses, IEnumerable<IFilter> accessEntryFilters, ref int totalRequestCount, ref int totalRequestCountWithMatch, List<ITextBlock> format)
        {
            StreamReader reader = new StreamReader(stream, true);

            int lineNumber = 0;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;

                try
                {
                    AccessEntry entry = ParseLine(line, format);

                    totalRequestCount++;

                    if (!AccessEntryMatchesFilters(entry, accessEntryFilters))
                        continue;

                    totalRequestCountWithMatch++;

                    if (!addresses.TryGetValue(entry.RemoteAddr, out RemoteAddress addr))
                    {
                        addr = new RemoteAddress(entry.RemoteAddr);
                        addresses.Add(entry.RemoteAddr, addr);
                    }

                    addr.AccessEntrys.Add(entry);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error at line {lineNumber}: {ex.Message}");
                }
            }
        }

        private static AccessEntry ParseLine(string line, List<ITextBlock> format)
        {
            AccessEntry ret = new AccessEntry();

            int index = -1;
            for (int i = 0; i < format.Count; i++)
            {
                ITextBlock current = format[i];
                ITextBlock next;
                if (i + 1 < format.Count)
                    next = format[i + 1];
                else
                    next = null;

                current.ReadValue(line, ref index, ret, next);
            }

            return ret;
        }
    }
}
