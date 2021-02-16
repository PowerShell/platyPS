namespace Markdown.MAML.Model.Markdown
{
    public class HardBreakSpan : ParagraphSpan
    {
        public HardBreakSpan(SourceExtent sourceExtent)
            : base("\n", sourceExtent, Parser.ParserMode.Full)
        {
        }
    }
}
