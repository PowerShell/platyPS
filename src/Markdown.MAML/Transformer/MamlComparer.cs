using System;
using System.Collections.Generic;
using Markdown.MAML.Model.MAML;

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
            return obj.Name.ToUpper().GetHashCode();
        }
    }
}