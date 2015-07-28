using System;
using System.Collections.Generic;

namespace Markdown.MAML.Model
{
    public class ParagraphNode : MarkdownNode
    {
        public override MarkdownNodeType NodeType
        {
            get { return MarkdownNodeType.Paragraph; }
        }

        public IEnumerable<ParagraphSpan> Spans
        {
            get;
            private set;
        }

        public ParagraphNode(IEnumerable<ParagraphSpan> childSpans)
        {
            this.Spans = childSpans;
        }
    }
}
