using System;

namespace Markdown.MAML.Model
{
    public class HyperlinkSpan : ParagraphSpan
    {
        public Uri Uri { get; private set; }

        public HyperlinkSpan(string spanText, string uriText)
            : base(spanText)
        {
            this.Uri = new Uri(uriText, UriKind.Absolute);
        }
    }
}
