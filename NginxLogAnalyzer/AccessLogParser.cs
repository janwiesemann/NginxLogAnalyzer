using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using NginxLogAnalyzer.Filter;
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

        private static DateTime ParseDateTime(string line, ref int offset)
        {
            ReadWhile(line, ref offset, (c, sb) => c != '[');

            string dateString = ReadWhile(line, ref offset, (c, sb) => c != ']');
            DateTime.TryParseExact(dateString, "dd/MMM/yyyy:hh:mm:ss K", null, DateTimeStyles.None, out DateTime res);

            return res;
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

        private static bool AccessEntryMatchesFilters(AccessEntry entry, IEnumerable<AccessEntryFilterBase> accessEntryFilters)
        {
            foreach (AccessEntryFilterBase item in accessEntryFilters)
            {
                if (!item.HasValue)
                    continue;

                if (!item.Matches(entry))
                    return false;
            }

            return true;
        }

        private static void ParseStream(Stream stream, Dictionary<string, RemoteAddress> addresses, IEnumerable<AccessEntryFilterBase> accessEntryFilters)
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

        public static List<RemoteAddress> ReadSources(Dictionary<string, ILogSource> sourceParamAndSource, List<AccessEntryFilterBase> accessEntryFilters)
        {
            Dictionary<string, RemoteAddress> ret = new Dictionary<string, RemoteAddress>();
            foreach (KeyValuePair<string, ILogSource> item in sourceParamAndSource)
                item.Value.ReadFile(item.Key, stream => ParseStream(stream, ret, accessEntryFilters));

            return ret.GetValuesAsList();
        }
    }
}
