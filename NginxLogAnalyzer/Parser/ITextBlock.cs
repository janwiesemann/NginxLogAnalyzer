namespace NginxLogAnalyzer.Parser
{
    internal interface ITextBlock
    {
        void ReadValue(string line, ref int offset, AccessEntry entry, ITextBlock nextBlock);

        bool IsStartOfBlock(char c);
    }
}
