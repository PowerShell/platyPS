using System.Collections.Generic;

namespace Markdown.MAML.Model.MAML
{
    public class MamlCommand
    {
        public string Name { get; set; }
        public string Synopsis { get; set; }
        public string Description { get; set; }

        private List<MamlParameter> _parameters = new List<MamlParameter>();
        public List<MamlParameter> Parameters 
        {
            get { return _parameters; } 
        }

    }
}
