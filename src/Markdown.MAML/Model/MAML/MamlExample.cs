﻿using Markdown.MAML.Model.Markdown;
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
        public string Code { get; set; }
        public string Remarks { get; set; }
        public string Introduction { get; set; }
        public SectionFormatOption FormatOption { get; set; }
    }
}
