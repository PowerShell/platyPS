using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown.MAML.Model
{
    public class HardBreakSpan : ParagraphSpan
    {
        public HardBreakSpan()
            : base("\n")
        {
        }
    }
}
