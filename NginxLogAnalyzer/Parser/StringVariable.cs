using System;

namespace NginxLogAnalyzer.Parser
{
    internal class StringVariable : VariableBase<string>
    {
        public StringVariable(string name, Action<string, AccessEntry> setAction) : base(name, setAction)
        { }

        public override bool IsStartOfBlock(char c)
        {
            return char.IsLetterOrDigit(c) || c == '_';
        }

        protected override string Parse(string value) => value;
    }
}
