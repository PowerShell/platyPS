using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown.MAML.Model
{
    public class DocumentNode : MarkdownNode
    {
        private List<MarkdownNode> childNodes = new List<MarkdownNode>();

        public override MarkdownNodeType NodeType
        {
            get { return MarkdownNodeType.Document; }
        }

        public DocumentNode()
        {
            this.Children = this.childNodes;
        }

        internal void AddChildNode(MarkdownNode childNode)
        {
            this.childNodes.Add(childNode);
        }
    }
}
