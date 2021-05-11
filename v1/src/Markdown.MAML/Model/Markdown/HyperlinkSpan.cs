using Markdown.MAML.Parser;

namespace Markdown.MAML.Model.Markdown
{
    public class HyperlinkSpan : ParagraphSpan
    {
        public string Uri { get; private set; }

        public HyperlinkSpan(string spanText, string uriText, SourceExtent sourceExtent, ParserMode parserMode)
            : base(spanText, sourceExtent, parserMode)
        {
            this.Uri = uriText;
        }
    }
}
