using System.Collections.Generic;
using NginxLogAnalyzer.Settings;

namespace NginxLogAnalyzer.Analyzer
{
    internal interface IAnalyzer
    {
        bool CanExecute(IEnumerable<char> switches);

        void Execute(IEnumerable<RemoteAddress> addresses, IEnumerable<ISetting> settings);

        string Name { get; }
    }
}
