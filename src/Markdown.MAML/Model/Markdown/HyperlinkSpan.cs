using System;

namespace Markdown.MAML.Model.Markdown
{
    public class HyperlinkSpan : ParagraphSpan
    {
        public string Uri { get; private set; }

        public HyperlinkSpan(string spanText, string uriText)
            : base(spanText)
        {
            this.Uri = uriText;
        }
    }
}
