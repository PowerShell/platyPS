using System.Collections.Generic;

namespace Markdown.MAML.Model
{
    public abstract class MarkdownNode
    {
        public abstract MarkdownNodeType NodeType { get; }
    }
}
