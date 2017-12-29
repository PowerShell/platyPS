using Markdown.MAML.Model.MAML;
using System.Collections.Generic;

namespace Markdown.MAML.Transformer
{
    internal class MamlSyntaxNameComparer : IComparer<MamlSyntax>
    {
        public int Compare(MamlSyntax x, MamlSyntax y)
        {
            return string.Compare(x.ParameterSetName.ToUpper(), y.ParameterSetName.ToUpper());
        }
    }

    internal class MamlParameterNameComparer : IComparer<MamlParameter>
    {
        public int Compare(MamlParameter x, MamlParameter y)
        {
            return string.Compare(x.Name.ToUpper(), y.Name.ToUpper());
        }
    }

    internal class MamlParameterAttributeComparer : IComparer<MamlParameter>
    {
        public int Compare(MamlParameter x, MamlParameter y)
        {
            var match = x.Name == y.Name;

            if (
                (x.Required != y.Required) ||
                (x.Aliases != y.Aliases) ||
                (x.DefaultValue != y.DefaultValue) ||
                (x.Position != y.Position) ||
                (x.Type != y.Type) ||
                (x.PipelineInput != y.PipelineInput) ||
                (x.AttributesMetadata != y.AttributesMetadata)
                )
            {
                match = false;
            }
            
            if (match)
            {
                return 0;
            }

            return -1;
        }
    }

    internal class MamlParameterEqualityComparer : IEqualityComparer<MamlParameter>
    {
        public bool Equals(MamlParameter x, MamlParameter y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode(MamlParameter obj)
        {
            if (obj == null || obj.Name == null)
            {
                return 0;
            }
            return obj.Name.ToUpper().GetHashCode();
        }
    }

    internal class MamlParameterSetEqualityComparer : IEqualityComparer<MamlSyntax>
    {
        public bool Equals(MamlSyntax x, MamlSyntax y)
        {
            return x.ParameterSetName == y.ParameterSetName;
        }

        public int GetHashCode(MamlSyntax obj)
        {
            if (obj == null || obj.ParameterSetName == null)
            {
                return 0;
            } 
            return obj.ParameterSetName.ToUpper().GetHashCode();
        }
    }
}