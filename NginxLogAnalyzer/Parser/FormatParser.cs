using System;
using System.Collections.Generic;
using System.Text;
using NginxLogAnalyzer.Settings;

namespace NginxLogAnalyzer.Parser
{
    internal static class FormatParser
    {
        private static string ReadVariableName(string str, ref int index)
        {
            if (ParseHelper.PeekNextChar(str, index) == '$')
                index++;

            StringBuilder sb = new StringBuilder(16);
            while(true)
            {
                char c = ParseHelper.PeekNextChar(str, index);
                if (c == '\0' || char.IsWhiteSpace(c) || (!char.IsLetter(c) && c != '_'))
                    break;

                index++;

                sb.Append(c);
            }

            return sb.ToString();
        }

        private static ITextBlock GetVariableBlock(string str, ref int index, IEnumerable<IVariable> vars)
        {
            string name = ReadVariableName(str, ref index);
            if (string.IsNullOrEmpty(name))
                throw new ParseException("Variable does not have a Name!", str, index);

            foreach (IVariable item in vars)
            {
                if (item.Name.Length == name.Length && item.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) == 0)
                    return item;
            }

            throw new ParseException($"Variable ${name} was not found!", str, index);
        }

        private static ExactTextBlock ReadTextBlock(string str, ref int index)
        {
            StringBuilder sb = new StringBuilder();
            while(true)
            {
                char c = ParseHelper.PeekNextChar(str, index);
                if (c == '\0' || c == '$')
                    break;

                index++;

                sb.Append(c);
            }

            if (sb.Length == 0)
                return null;

            return new ExactTextBlock(sb.ToString());
        }

        public static List<ITextBlock> ParseFormat(string format)
        {
            List<IVariable> vars = Setup.GetFormatVariables();

            List<ITextBlock> blocks = new List<ITextBlock>();

            int index = -1;
            while(true)
            {
                char c = ParseHelper.PeekNextChar(format, index);
                if(c == '$')
                {
                    ITextBlock varBlock = GetVariableBlock(format, ref index, vars);
                    blocks.Add(varBlock);

                    continue;
                }

                ITextBlock block = ReadTextBlock(format, ref index);
                if (block == null)
                    break;

                blocks.Add(block);
            }

            if (blocks.Count == 0)
            {
                Console.WriteLine("No log format specified!");

                return null;
            }

            return blocks;
        }
    }
}
