using Markdown.MAML.Parser;

namespace Markdown.MAML.Model.Markdown
{
    public abstract class ParagraphSpan
    {
        public string Text { get; private set; }

        public SourceExtent SourceExtent { get; private set; }

        public ParserMode ParserMode { get; private set; }

        public ParagraphSpan(string spanText, SourceExtent sourceExtent, ParserMode parserMode)
        {
            this.Text = spanText;
            this.SourceExtent = sourceExtent;
            this.ParserMode = parserMode;
        }
    }
}
