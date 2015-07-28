using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown.MAML.Model
{
    public abstract class TextNode : MarkdownNode
    {
        public string Text { get; private set; }

        public TextNode(string textContents)
        {
            this.Text = textContents;
        }
    }
}
