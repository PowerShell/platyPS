using Markdown.MAML.Model.Markdown;

namespace Markdown.MAML.Model.MAML
{
    public class MamlParameter
    {
        public SourceExtent Extent { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public bool Required { get; set; }

        public string Description { get; set; }

        public bool VariableLength { get; set; }

        public bool Globbing { get; set; }

        public string PipelineInput { get; set; }

        public string Position { get; set; }

        public string[] Aliases { get; set; }

        public bool ValueRequired { get; set; }

        public bool ValueVariableLength { get; set; }

        public string AttributesText { get; set; }

        public MamlParameter()
        {
            VariableLength = false;
            Globbing = false;
            PipelineInput = "false";
            Aliases = new string[] {};
        }
    }
}