using System;
using System.Collections.Generic;
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

        public static List<TValue> GetValuesAsList<TKey, TValue>(this Dictionary<TKey, TValue> dic)
        {
            List<TValue> ret = new List<TValue>();
            foreach (KeyValuePair<TKey, TValue> item in dic)
                ret.Add(item.Value);

            return ret;
        }

        public static bool HasSwitch(this IEnumerable<char> switches, char check)
        {
            foreach (char item in switches)
            {
                if (item == check)
                    return true;
            }

            return false;
        }
    }
}
