using System;
using System.Collections.Generic;
using System.Linq;
using NginxLogAnalyzer.Settings;

namespace NginxLogAnalyzer.Analyzer
{
    internal class UserAgentAnalyzer : IAnalyzer
    {
        public string Name => "UserAgentAnalyzer";

        public bool CanExecute(IEnumerable<char> switches)
        {
            return switches.Contains('u');
        }

        class Entry
        {
            public Entry(string agent)
            {
                UserAgent = agent;
                AddressCount = new Dictionary<string, int>();
            }

            public int Count { get; private set; }
            public string UserAgent { get; }
            public Dictionary<string, int> AddressCount { get; }

            public void Add(RemoteAddress entry)
            {
                Count++;

                if (AddressCount.TryGetValue(entry.Address, out int count))
                    AddressCount[entry.Address] = count + 1;
                else
                    AddressCount.Add(entry.Address, 1);
            }
        }

        private IEnumerable<Entry> GetGroups(IEnumerable<RemoteAddress> addresses)
        {
            Dictionary<string, Entry> groups = new Dictionary<string, Entry>();
            foreach (RemoteAddress address in addresses)
            {
                foreach (AccessEntry item in address.AccessEntrys)
                {
                    if (!groups.TryGetValue(item.HttpUserAgent, out Entry entry))
                    {
                        entry = new Entry(item.HttpUserAgent);
                        groups.Add(item.HttpUserAgent, entry);
                    }

                    entry.Add(address);
                }
            }

            return groups.Select(i => i.Value).OrderByDescending(i => i.Count);
        }

        public void Execute(IEnumerable<RemoteAddress> addresses, IEnumerable<ISetting> settings)
        {
            IEnumerable<Entry> groups = GetGroups(addresses);

            settings.GetCountSettings(out int addressCount, out int entryCount);

            foreach (Entry item in groups)
            {
                Console.WriteLine(item.Count.ToString().PadRight(4) + ": " + item.UserAgent);

                int ec = entryCount;
                foreach (KeyValuePair<string, int> counts in item.AddressCount.OrderByDescending(i => i.Value))
                {
                    Console.WriteLine("\t" + counts.Value.ToString().PadRight(3) + " => " + counts.Key);

                    if (!Count.Continue(ref ec))
                        break;
                }

                if (!Count.Continue(ref addressCount))
                    break;
            }
        }
    }
}
