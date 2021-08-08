using System;
using System.Collections.Generic;
using System.Linq;
using NginxLogAnalyzer.Settings;

namespace NginxLogAnalyzer.Analyzer
{
    internal class PrintAccessesAnalyzer : IAnalyzer
    {
        public string Name => "Accesses";

        public bool CanExecute(IEnumerable<char> switches)
        {
            return switches.HasSwitch('r');
        }

        public void Execute(IEnumerable<RemoteAddress> addresses, IEnumerable<ISetting> settings)
        {
            settings.GetCountSettings(out int addressCount, out int entryCount);

            List<KeyValuePair<AccessEntry, RemoteAddress>> allItems = new List<KeyValuePair<AccessEntry, RemoteAddress>>();
            foreach (RemoteAddress address in addresses)
            {
                foreach (AccessEntry entry in address.AccessEntrys)
                    allItems.Add(new KeyValuePair<AccessEntry, RemoteAddress>(entry, address));
            }

            foreach (KeyValuePair<AccessEntry, RemoteAddress> entry in allItems.OrderByDescending(i => i.Key.DateTime))
            {
                Console.WriteLine($"{entry.Value.Address.PadRight(4 * 3 + 3)}: {entry.Key.DateTime} => {entry.Key.Request?.ToString()}");

                if (!Count.Continue(ref addressCount))
                    break;
            }
        }
    }
}
