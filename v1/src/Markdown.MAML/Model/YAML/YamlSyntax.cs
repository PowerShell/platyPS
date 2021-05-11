using System.Collections.Generic;

namespace Markdown.MAML.Model.YAML
{
    public class YamlSyntax
    {
        public string ParameterValueGroup { get; set; }
        public bool IsDefault { get; set; }
        public List<string> Parameters { get; set; }
    }
}