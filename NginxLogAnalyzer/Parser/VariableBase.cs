using System;
using System.Text;

namespace NginxLogAnalyzer.Parser
{
    internal abstract class VariableBase : IVariable
    {
        public VariableBase(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public abstract bool IsStartOfBlock(char c);

        public void ReadValue(string line, ref int offset, AccessEntry entry, ITextBlock nextBlock)
        {
            StringBuilder sb = new StringBuilder(32);
            while(true)
            {
                char c = ParseHelper.PeekNextChar(line, offset);
                if (c == '\0' || (nextBlock != null && nextBlock.IsStartOfBlock(c)))
                    break;

                offset++;

                sb.Append(c);
            }

            SetValue(sb.ToString(), entry);
        }

        protected abstract void SetValue(string value, AccessEntry entry);
    }

    internal abstract class VariableBase<T> : VariableBase
    {
        private readonly Action<T, AccessEntry> setAction;

        public VariableBase(string name, Action<T, AccessEntry> setAction) : base(name)
        {
            this.setAction = setAction;
        }

        protected override void SetValue(string value, AccessEntry entry)
        {
            T t = Parse(value);

            setAction(t, entry);
        }

        protected abstract T Parse(string value);
    }
}
