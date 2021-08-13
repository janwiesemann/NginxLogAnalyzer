using System;

namespace NginxLogAnalyzer.Parser
{
    internal class ExactTextBlock : ITextBlock
    {
        public ExactTextBlock(string str)
        {
            String = str;

        }

        public string String { get; }

        public bool IsStartOfBlock(char c)
        {
            if (String.Length == 0)
                return false;

            return String[0] == c;
        }

        public void ReadValue(string line, ref int offset, AccessEntry entry, ITextBlock nextBlock)
        {
            for (int i = 0; i < String.Length; i++)
            {
                char next = ParseHelper.GetNextChar(line, ref offset);

                if (next != String[i])
                    throw new ParseException($"Char '{next}' does not match '{String[i]}'", line, offset);
            }
        }

        public override string ToString()
        {
            return $"Exact: {String}";
        }
    }
}
