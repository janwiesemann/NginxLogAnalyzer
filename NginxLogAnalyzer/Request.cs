using System;
using System.Text;

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
    }
}
