using System.Collections.Generic;

namespace Markdown.MAML.Transformer
{

    public class ParameterModel
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }
    }
    
    public class MamlCommand
    {
        public string Name { get; set; }
        public string Synopsis { get; set; }
        public string Description { get; set; }

        private List<ParameterModel> _parameters = new List<ParameterModel>();
        public List<ParameterModel> Parameters 
        {
            get { return _parameters; } 
        }

    }
}
