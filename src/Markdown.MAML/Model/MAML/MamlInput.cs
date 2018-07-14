using Markdown.MAML.Model.Markdown;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Markdown.MAML.Model.MAML
{
    /// <summary>
    /// This class represent Input and Output properties for MAML.
    /// </summary>
    public class MamlInput : IEquatable<MamlInput>
    {
        public string TypeName { get; set; }
        public List<MamlParameter> Parameters { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Additional options that determine how the section will be formated when rendering markdown.
        /// </summary>
        public SectionFormatOption FormatOption { get; set; }

        bool IEquatable<MamlInput>.Equals(MamlInput other)
        {
            if (!StringComparer.OrdinalIgnoreCase.Equals(other.TypeName, this.TypeName))
            {
                return false;
            }

            if (!StringComparer.OrdinalIgnoreCase.Equals(other.Description, this.Description))
            {
                return false;
            }

            foreach (MamlParameter param in this.Parameters)
            {
                if (!other.Parameters.Contains(param))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
