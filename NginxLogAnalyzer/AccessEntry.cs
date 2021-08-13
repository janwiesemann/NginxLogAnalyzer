using System;

namespace NginxLogAnalyzer
{
    class AccessEntry
    {
        public string RemoteAddress { get; set; }
        public string ServerName { get; set; }
        public string RemoteUser { get; set; }
        public DateTime DateTime { get; set; }
        public Request Request { get; set; }
        public int StatusCode { get; internal set; }
    }
}
