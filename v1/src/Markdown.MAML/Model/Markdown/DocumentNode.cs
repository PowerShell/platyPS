using System.Collections.Generic;

namespace Markdown.MAML.Model.Markdown
{
    public class DocumentNode : MarkdownNode
    {
        private List<MarkdownNode> childNodes = new List<MarkdownNode>();

        public override MarkdownNodeType NodeType
        {
            get { return MarkdownNodeType.Document; }
        }

        public IEnumerable<MarkdownNode> Children { get; protected set; }

        public DocumentNode()
        {
            this.Children = this.childNodes;
        }

        public void AddChildNode(MarkdownNode childNode)
        {
            this.childNodes.Add(childNode);
        }
    }
}
