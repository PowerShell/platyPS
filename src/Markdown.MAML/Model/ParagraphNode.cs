using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown.MAML.Model
{
    public class ParagraphNode : TextNode
    {
        public override MarkdownNodeType NodeType
        {
            get { return MarkdownNodeType.Paragraph; }
        }

        public ParagraphNode(string paragraphText)
            : base(paragraphText)
        {
        }
    }
}
