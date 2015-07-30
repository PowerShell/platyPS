using System;

namespace Markdown.MAML.Model.Markdown
{
    public class HyperlinkSpan : ParagraphSpan
    {
        public string Uri { get; private set; }

        public HyperlinkSpan(string spanText, string uriText, SourceExtent sourceExtent)
            : base(spanText, sourceExtent)
        {
            this.Uri = uriText;
        }
    }
}
