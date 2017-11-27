using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown.MAML.Model.Markdown
{
    public enum SectionFormatOption : byte
    {
        None = 0,

        // A line break should be added after the section header
        LineBreakAfterHeader = 1
    }
}
