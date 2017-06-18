using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown.MAML.Model.MAML
{
    /// <summary>
    /// This class represents the related links properties for MAML
    /// </summary>
    public class MamlLink : IEquatable<MamlLink>
    {
        /// <summary>
        /// This is a workaround for PreserveFormatting
        /// So we can store a block of texts that suppose to represent links
        /// without parsing them
        /// </summary>
        public bool IsSimplifiedTextLink  { get; private set; }

        public string LinkName { get; set; }
        public string LinkUri { get; set; }

        public MamlLink() : this(false) { }

        public MamlLink(bool isSimplifiedTextLink)
        {
            this.IsSimplifiedTextLink = isSimplifiedTextLink;
        }

        bool IEquatable<MamlLink>.Equals(MamlLink other)
        {
            if (!StringComparer.OrdinalIgnoreCase.Equals(other.LinkName, this.LinkName))
            {
                return false;
            }

            if (!StringComparer.OrdinalIgnoreCase.Equals(other.LinkUri, this.LinkUri))
            {
                return false;
            }

            return true;
        }
    }
}
