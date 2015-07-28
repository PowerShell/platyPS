using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown.MAML.Renderer
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
