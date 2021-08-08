using System;
using System.Collections.Generic;
using System.Linq;
using NginxLogAnalyzer.Settings;

namespace NginxLogAnalyzer.Analyzer
{
    internal class MostRequestedPagesAnalyzer : IAnalyzer
    {
        public string Name => "Most requested pages";

        public bool CanExecute(IEnumerable<char> switches)
        {
            return switches.HasSwitch('p');
        }

        public void Execute(IEnumerable<RemoteAddress> addresses, IEnumerable<ISetting> settings)
        {
            settings.GetCountSettings(out int addressCount, out int entryCount);

            Dictionary<string, int> counts = new Dictionary<string, int>();
            int totalCount = 0;
            foreach (var address in addresses)
            {
                foreach (var item in address.AccessEntrys)
                {
                    string uri = item.Request?.URI ?? string.Empty;
                    if (counts.TryGetValue(uri, out int count))
                        counts[uri] = count + 1;
                    else
                        counts.Add(uri, 1);

                    totalCount++;
                }
            }

            if (totalCount == 0)
                return;

            foreach (var item in counts.OrderByDescending(i => i.Value))
            {
                int percentage = (int)Math.Round((double)item.Value / totalCount * 100);

                Console.Write(item.Value.ToString().PadRight(4) + " (" + percentage.ToString().PadLeft(2, '0') + "%) => ");
                ConsoleEx.WriteURI(item.Key);
                Console.WriteLine();

                if (!Count.Continue(ref addressCount))
                    break;
            }
        }
    }
}
