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
            ParameterSetName = "__AllParameterSets";
        }


        public string ParameterSetName { get; set; }

        public List<MamlParameter> Parameters { get { return _parameters; } }

        private List<MamlParameter> _parameters = new List<MamlParameter>();
    }
}
