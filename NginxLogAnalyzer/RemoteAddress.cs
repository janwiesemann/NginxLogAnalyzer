using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NginxLogAnalyzer
{
    [DebuggerDisplay("{Address} => {AccessEntrys.Count}")]
    class RemoteAddress
    {
        public RemoteAddress(string address)
        {
            Address = address;
            AccessEntrys = new List<AccessEntry>();
        }

        public string Address { get; }
        public List<AccessEntry> AccessEntrys { get; }
    }
}
