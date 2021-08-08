using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using NginxLogAnalyzer.Filters;
using NginxLogAnalyzer.Sources;

namespace NginxLogAnalyzer
{
    internal static class AccessLogParser
    {
        private static string ReadWhile(string line, ref int offset, Func<char, StringBuilder, bool> check)
        {
            StringBuilder sb = new StringBuilder(line.Length - offset);
            while (line.Length > offset)
            {
                char c = line[offset];

                offset++;

                if (!check(c, sb))
                    break;

                sb.Append(c);
            }

            return sb.ToString();
        }

        private static AccessEntry ParseAccessLine(string line)
        {
            int offset = 0;

            return new AccessEntry
            {
                RemoteAddress = ParseBlock(line, ref offset),
                ServerName = ParseBlock(line, ref offset),
                RemoteUser = ParseBlock(line, ref offset),
                DateTime = ParseDateTime(line, ref offset),
                Request = ParseRequest(line, ref offset)
            };
        }

        private static string ParseBlock(string line, ref int offset) => ReadWhile(line, ref offset, (c, sb) => c != ' ' && c != '\t');

        private static int ParseNumber(string line, ref int offset)
        {
            string str = ReadWhile(line, ref offset, (c, sb) => char.IsNumber(c));

            return int.Parse(str);
        }

        private static int ParseMonth(string line, ref int offset)
        {
            string str = ReadWhile(line, ref offset, (c, sb) => c != '/');
            str = str.ToUpper();

            if (str == "JAN")
                return 1;

            if (str == "FEB")
                return 2;

            if (str == "MAR")
                return 3;

            if (str == "APR")
                return 4;

            if (str == "MAY")
                return 5;

            if (str == "JUN")
                return 6;

            if (str == "JUL")
                return 7;

            if (str == "AUG")
                return 8;

            if (str == "SEP" || str == "SEPT")
                return 9;

            if (str == "OCT")
                return 10;

            if (str == "NOV")
                return 11;

            if (str == "DEC")
                return 12;

            return 1;
        }

        private static DateTime ParseDateTime(string line, ref int offset)
        {
            ReadWhile(line, ref offset, (c, sb) => c != '[');

            int day = ParseNumber(line, ref offset);
            int month = ParseMonth(line, ref offset);
            int year = ParseNumber(line, ref offset);
            int hour = ParseNumber(line, ref offset);
            int minute = ParseNumber(line, ref offset);
            int second = ParseNumber(line, ref offset);

            ReadWhile(line, ref offset, (c, sb) => c != ']');

            return new DateTime(year, month, day, hour, minute, second);
        }

        private static Request ParseRequest(string line, ref int offset)
        {
            ReadWhile(line, ref offset, (c, sb) => c != '"');

            string requestStr = ReadWhile(line, ref offset, (c, sb) => c != '"');

            int offsetRequest = 0;
            return new Request
            {
                Methode = ParseBlock(requestStr, ref offsetRequest),
                URI = ParseBlock(requestStr, ref offsetRequest),
                Version = ParseBlock(requestStr, ref offsetRequest)
            };
        }

        private static bool AccessEntryMatchesFilters(AccessEntry entry, IEnumerable<IFilter> accessEntryFilters)
        {
            foreach (IFilter item in accessEntryFilters)
            {
                if (!item.HasValue)
                    continue;

                if (!item.Matches(entry))
                    return false;
            }

            return true;
        }

        private static void ParseStream(Stream stream, Dictionary<string, RemoteAddress> addresses, IEnumerable<IFilter> accessEntryFilters)
        {
            StreamReader reader = new StreamReader(stream, true);

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                AccessEntry entry = ParseAccessLine(line);

                if (!AccessEntryMatchesFilters(entry, accessEntryFilters))
                    continue;

                if (!addresses.TryGetValue(entry.RemoteAddress, out RemoteAddress addr))
                {
                    addr = new RemoteAddress(entry.RemoteAddress);
                    addresses.Add(entry.RemoteAddress, addr);
                }

                addr.AccessEntrys.Add(entry);
            }
        }

        public static List<RemoteAddress> ReadSources(Dictionary<string, ILogSource> sourceParamAndSource, List<IFilter> accessEntryFilters)
        {
            Dictionary<string, RemoteAddress> ret = new Dictionary<string, RemoteAddress>();
            foreach (KeyValuePair<string, ILogSource> item in sourceParamAndSource)
            {
                DateTime start = DateTime.Now;
                Console.Write($"Reading source {item.Value.GetType().Name} ({item.Key})...");

                try
                {
                    item.Value.ReadFile(item.Key, stream => ParseStream(stream, ret, accessEntryFilters));

                    Console.WriteLine($" finished in {Math.Round((DateTime.Now - start).TotalMilliseconds, 1)}ms");
                }
                catch(Exception ex)
                {
                    Console.Write("Error: ");
                    Console.WriteLine(ex);
                }
            }

            return ret.GetValuesAsList();
        }
    }
}
