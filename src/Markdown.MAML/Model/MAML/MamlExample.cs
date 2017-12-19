using Markdown.MAML.Model.Markdown;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown.MAML.Model.MAML
{
    public class MamlExample
    {
        public string Title { get; set; }
        public MamlCodeBlock[] Code { get; set; }
        public string Remarks { get; set; }
        public string Introduction { get; set; }

        /// <summary>
        /// Additional options that determine how the section will be formated when rendering markdown.
        /// </summary>
        public SectionFormatOption FormatOption { get; set; }
    }
}
