namespace NginxLogAnalyzer.Parser
{
    internal interface IVariable : ITextBlock
    {
        public string Name { get; }
    }
}
