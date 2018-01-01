using Markdown.MAML.Model.Markdown;
using System;

namespace Markdown.MAML.Model.MAML
{
    /// <summary>
    /// This class represent Input and Output properties for MAML.
    /// </summary>
    public class MamlInputOutput : IEquatable<MamlInputOutput>
    {
        public string TypeName { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Additional options that determine how the section will be formated when rendering markdown.
        /// </summary>
        public SectionFormatOption FormatOption { get; set; }

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
