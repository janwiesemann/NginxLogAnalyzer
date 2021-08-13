using System;
using System.Text;
using NginxLogAnalyzer.Parser;

namespace NginxLogAnalyzer
{
    class Request
    {
        public string Methode { get; set; }
        public string URI { get; set; }
        public string Version { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendMaxLength(10, Methode);
            sb.Append(' ');
            sb.AppendURI(URI);
            sb.Append(' ');
            sb.AppendMaxLength(10, Version);

            return sb.ToString();
        }

        public static Request ParseRequest(string line)
        {
            Request ret = new Request();
            int offset = -1;

            ret.Methode = ParseHelper.ParseUntill(line, ref offset, (c, sb) => char.IsWhiteSpace(c));
            offset++;
            ret.URI = ParseHelper.ParseUntill(line, ref offset, (c, sb) => char.IsWhiteSpace(c));
            offset++;
            ret.Version = ParseHelper.ParseUntill(line, ref offset, (c, sb) => c == '\0');

            return ret;
        }
    }
}
