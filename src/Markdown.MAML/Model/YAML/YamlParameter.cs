using System.Collections.Generic;

namespace Markdown.MAML.Model.YAML
{
    public class YamlParameter
    {
        public string Name { get; set; }
        public bool AcceptWildcardCharacters { get; set; }
        public List<string> Aliases { get; set; }
        public string DefaultValue { get; set; }
        public string Description { get; set; }
        public List<string> ParameterValueGroup { get; set; }
        public string PipelineInput { get; set; }
        public string Position { get; set; }
        public string Type { get; set; }
    }
}