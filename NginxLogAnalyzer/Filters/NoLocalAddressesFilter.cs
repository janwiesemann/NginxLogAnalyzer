using System;

namespace NginxLogAnalyzer.Filters
{
    internal class NoLocalAddressesFilter : IFilter
    {
        public bool HasValue => true;

        public string ParameterName => "onlypublicaddresses";

        public object Value => "10.0.0.0/8, 127.0.0.0/8, 172.16.0.0/12, 192.168.0.0/16";

        private static readonly string[] startsWith = new string[] { "10.", "127.", "172.16.", "172.17.", "172.18.", "172.19.", "172.20.", "172.21.", "172.22.", "172.23.", "172.24.", "172.25.", "172.26.", "172.27.", "172.28.", "172.29.", "172.30.", "172.31.", "192.168." };
        
        public bool Matches(AccessEntry entry)
        {
            if (string.IsNullOrEmpty(entry.RemoteAddr))
                return true;

            for (int i = 0; i < startsWith.Length; i++)
            {
                if (entry.RemoteAddr.StartsWith(startsWith[i]))
                    return false;
            }

            return true;
        }

        public void ParseValue(string value)
        { }
    }
}
