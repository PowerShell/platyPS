using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown.MAML.Model.YAML
{
    public class YamlCommand
    {
        public List<YamlExample> Examples { get; set; }
        public List<YamlInputOutput> Inputs { get; set; }
        public List<YamlLink> Links { get; set; }
        public YamlModule Module { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public List<YamlParameter> OptionalParameters { get; set; }
        public List<YamlInputOutput> Outputs { get; set; }
        public List<YamlParameter> RequiredParameters { get; set; }
        public string Remarks { get; set; }
        public string Summary { get; set; }
        public List<YamlSyntax> Syntaxes { get; set; }
    }
}
