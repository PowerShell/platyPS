using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown.MAML.Model.MAML
{
    public class MamlSyntax
    {
        public MamlSyntax()
        {
            // default for parameter set names is __AllParameterSets
            //commented out for future consideration.
            //ParameterSetName = "__AllParameterSets";
        }
        
        public string ParameterSetName { get; set; }

        public string RawSyntaxString { get; set; }

        public bool IsDefault { get; set; }

        public List<MamlParameter> Parameters { get { return _parameters; } }

        private List<MamlParameter> _parameters = new List<MamlParameter>();
    }
}
