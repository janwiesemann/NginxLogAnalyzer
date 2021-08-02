using System;
using System.Text;

namespace NginxLogAnalyzer
{
    static class ConsoleEx
    {
        public static void WriteURI(string uri)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendURI(uri);

            Console.Write(sb.ToString());
        }
    }
}
