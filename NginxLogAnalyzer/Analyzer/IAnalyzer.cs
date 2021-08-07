using System.Collections.Generic;

namespace NginxLogAnalyzer.Analyzer
{
    internal interface IAnalyzer
    {
        bool CanExecute(IEnumerable<char> switches);

        void Execute(IEnumerable<RemoteAddress> addresses);

        string Name { get; }
    }
}
