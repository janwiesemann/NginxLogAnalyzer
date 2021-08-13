using System;
namespace NginxLogAnalyzer.Parser
{
    internal class IntVariable : VariableBase<int>
    {
        public IntVariable(string name, Action<int, AccessEntry> setAction) : base(name, setAction)
        { }

        public override bool IsStartOfBlock(char c)
        {
            return char.IsDigit(c);
        }

        protected override int Parse(string value) => int.Parse(value);
    }
}
