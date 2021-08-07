using System;
using System.Collections.Generic;
using System.Linq;

namespace NginxLogAnalyzer.Analyzer
{
    internal class MostRequestsByAddressAnalyzer : IAnalyzer
    {
        public string Name => "Most requests by address";

        public bool CanExecute(IEnumerable<char> switches)
        {
            return switches.HasSwitch('a');
        }
        private static List<List<AccessEntry>> GroupRequests(List<AccessEntry> entries)
        {
            Dictionary<string, List<AccessEntry>> groupes = new Dictionary<string, List<AccessEntry>>();
            foreach (var item in entries)
            {
                string uri = item.Request?.URI ?? "";

                if (!groupes.TryGetValue(uri, out List<AccessEntry> list))
                {
                    list = new List<AccessEntry>();
                    groupes.Add(uri, list);
                }

                list.Add(item);
            }

            return groupes.GetValuesAsList();
        }

        public void Execute(IEnumerable<RemoteAddress> addresses)
        {
            int i = 0;
            foreach (RemoteAddress address in addresses.OrderByDescending(e => e.AccessEntrys.Count))
            {
                Console.WriteLine(address.Address.PadRight(16) + " => " + address.AccessEntrys.Count);

                int j = 0;
                foreach (var item in GroupRequests(address.AccessEntrys).OrderByDescending(e => e.Count))
                {
                    Console.WriteLine("\t" + item.Count.ToString().PadRight(3) + " => " + item.First().Request);

                    j++;

                    if (j >= 5)
                        break;
                }

                i++;

                if (i >= 10)
                    break;
            }
        }
    }
}
