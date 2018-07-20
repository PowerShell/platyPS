using System.Collections.Generic;
using Markdown.MAML.Model.Markdown;

namespace Markdown.MAML.Model.MAML
{
    public class MamlCommand
    {
        public SourceExtent Extent { get; set; }

        public string Name { get; set; }

        public SectionBody Synopsis { get; set; }

        public SectionBody Description { get; set; }

        public List<MamlInput> Inputs 
        {
            get { return _inputs; }
        }

        public List<MamlOutput> Outputs
        {
            get { return _outputs; }
        }

        public List<MamlParameter> Parameters 
        {
            get { return _parameters; } 
        }

        public SectionBody Notes { get; set; }

        public bool IsWorkflow { get; set; }

        public bool SupportCommonParameters { get; set; }

        public string ModuleName { get; set; }

        public List<MamlExample> Examples
        {
            get { return _examples; }
        } 

        public List<MamlLink> Links
        {
            get { return _links; }
        }

        public List<MamlSyntax> Syntax
        {
            get { return _syntax; }
        }

        private List<MamlParameter> _parameters = new List<MamlParameter>();

        private List<MamlOutput> _outputs = new List<MamlOutput>();

        private List<MamlInput> _inputs = new List<MamlInput>();

        private List<MamlExample> _examples = new List<MamlExample>();

        private List<MamlLink> _links = new List<MamlLink>();

        private List<MamlSyntax> _syntax = new List<MamlSyntax>();

        public MamlCommand()
        {
            // this is the default most often then not
            this.SupportCommonParameters = true;
        }
    }
}
