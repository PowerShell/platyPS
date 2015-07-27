using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown.MAML.Model
{
    public class CodeBlockNode : TextNode
    {
        public override MarkdownNodeType NodeType
        {
            get { return MarkdownNodeType.CodeBlock; }
        }

        public CodeBlockNode(string codeBlockContents)
            : base(codeBlockContents)
        {
        }
    }
}
