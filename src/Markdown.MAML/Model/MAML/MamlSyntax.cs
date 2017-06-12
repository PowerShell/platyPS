using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown.MAML.Model.MAML
{
    public class MamlSyntax : IEquatable<MamlSyntax>
    {
        public MamlSyntax()
        {
            // default for parameter set names is __AllParameterSets
            //commented out for future consideration.
            //ParameterSetName = "__AllParameterSets";
        }
        
        public string ParameterSetName { get; set; }

        public bool IsDefault { get; set; }

        public List<MamlParameter> Parameters { get { return _parameters; } }

        private List<MamlParameter> _parameters = new List<MamlParameter>();

        bool IEquatable<MamlSyntax>.Equals(MamlSyntax other)
        {
            if (!StringComparer.OrdinalIgnoreCase.Equals(other.ParameterSetName, this.ParameterSetName))
            {
                return false;
            }

            // This is not 100% accurate, we just compare parameter names here
            var names1 = this.Parameters.Select(p => p.Name).ToList();
            var names2 = other.Parameters.Select(p => p.Name).ToList();

            if (names1.Count != names2.Count)
            {
                return false;
            }

            if (names1.Except(names2).Any())
            {
                return false;
            }

            return true;
        }
    }
}
