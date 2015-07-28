namespace Markdown.MAML.Model.MAML
{
    public class MamlParameter
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public bool VariableLength { get; set; }

        public bool Globbing { get; set; }

        public bool PipelineInput { get; set; }

        public string Position { get; set; }

        public string[] Aliases { get; set; }

        public MamlParameter()
        {
            VariableLength = false;
            Globbing = false;
            PipelineInput = false;
            Aliases = new string[] {};
        }

        
    }
}