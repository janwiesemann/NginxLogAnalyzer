using System;
using System.Text;

namespace NginxLogAnalyzer
{
    static class Extensions
    {
        public static void AppendMaxLength(this StringBuilder sb, int length, string str)
        {
            if (str == null)
                return;

            if (str.Length > length)
                str = str.Substring(0, length - 3) + "...";

            sb.Append(str);
        }

        public static void AppendURI(this StringBuilder sb, string url) => sb.AppendMaxLength(64, url);
    }
}
