using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown.MAML.Model.MAML
{
    /// <summary>
    /// This class represent Input and Output properties for MAML.
    /// </summary>
    public class MamlInputOutput : IEquatable<MamlInputOutput>
    {
        public string TypeName { get; set; }
        public string Description { get; set; }

        bool IEquatable<MamlInputOutput>.Equals(MamlInputOutput other)
        {
            if (!StringComparer.OrdinalIgnoreCase.Equals(other.TypeName, this.TypeName))
            {
                return false;
            }

            if (!StringComparer.OrdinalIgnoreCase.Equals(other.Description, this.Description))
            {
                return false;
            }

            return true;
        }
    }
}
