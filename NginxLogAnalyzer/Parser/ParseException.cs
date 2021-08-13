using System;

namespace NginxLogAnalyzer.Parser
{
    internal class ParseException : Exception
    {
        public ParseException(string message, string line, int index) : this(message, line, index, null)
        { }

        public ParseException(string message, string line, int index, Exception innerException) : base(message, innerException)
        {
            Line = line;
            Index = index;
        }

        public override string Message => $"Can not read '{(Line.Length > Index ? Line.Substring(Index) : Line)}' at offset {Index}: " + base.Message;

        public string Line { get; }

        public int Index { get; }
    }
}
