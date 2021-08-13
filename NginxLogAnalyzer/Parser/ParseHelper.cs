using System;
using System.Text;

namespace NginxLogAnalyzer.Parser
{
    internal static class ParseHelper
    {
        public static char GetNextChar(string str, ref int index)
        {
            index++;

            if (index > str.Length - 1)
                throw new IndexOutOfRangeException($"Can not read char at {index}! String is to short!");

            return str[index];
        }

        public static char PeekNextChar(string str, int index)
        {
            index++;

            if (index > str.Length - 1)
                return '\0';

            return str[index];
        }

        internal static int ParseMonth(string value, ref int offset)
        {
            string str = ParseUntill(value, ref offset, (c, sb) => !char.IsLetter(c));
            str = str.ToUpper();

            if (str == "JAN")
                return 1;

            if (str == "FEB")
                return 2;

            if (str == "MAR")
                return 3;

            if (str == "APR")
                return 4;

            if (str == "MAY")
                return 5;

            if (str == "JUN")
                return 6;

            if (str == "JUL")
                return 7;

            if (str == "AUG")
                return 8;

            if (str == "SEP" || str == "SEPT")
                return 9;

            if (str == "OCT")
                return 10;

            if (str == "NOV")
                return 11;

            if (str == "DEC")
                return 12;

            throw new ParseException($"Can not convert '{str}' to a month!", value, offset);
        }

        public static string ParseUntill(string str, ref int offset, Func<char, StringBuilder, bool> check)
        {
            StringBuilder sb = new StringBuilder(16);
            while(true)
            {
                char c = PeekNextChar(str, offset);
                if (check(c, sb) || c == '\0')
                    break;

                offset++;

                sb.Append(c);
            }

            return sb.ToString();
        }

        public static void ParseExactChar(string str, ref int offset, char compare)
        {
            char c = GetNextChar(str, ref offset);

            if (c != compare)
                throw new ParseException($"Expected char '{compare}' but found '{c}'", str, offset);
        }

        public static int ParseNumber(string str, ref int index)
        {
            bool isNegative = PeekNextChar(str, index) == '-';
            if (isNegative)
                index++;

            int ret = 0;
            bool ran = false;
            while (true)
            {
                char c = PeekNextChar(str, index);
                if (!char.IsDigit(c))
                {
                    if (!ran)
                        throw new ParseException($"Can not convert '{c}' to a Number!", str, index);

                    break;
                }

                index++;

                int val = c - '0';

                ret = ret * 10 + val;

                ran = true;
            }            

            if (isNegative)
                ret = ret * -1;

            return ret;
        }
    }
}
