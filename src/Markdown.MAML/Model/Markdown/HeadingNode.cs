using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown.MAML.Model
{
    public class HeadingNode : TextNode
    {
        public override MarkdownNodeType NodeType
        {
            get { return MarkdownNodeType.Heading; }
        }

        public int HeadingLevel { get; private set; }

        public HeadingNode(string headingText, int headingLevel) 
            : base(headingText)
        {
            this.HeadingLevel = headingLevel;
        }
    }
}
