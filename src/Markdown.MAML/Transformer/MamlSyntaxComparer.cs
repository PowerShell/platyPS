using System;
using System.Collections.Generic;
using Markdown.MAML.Model.MAML;

namespace Markdown.MAML.Transformer
{
    internal class MamlSyntaxComparer : IComparer<MamlSyntax>
    {
        public int Compare(MamlSyntax x, MamlSyntax y)
        {
            if (x.ParameterSetName == y.ParameterSetName)
            {
                return 0;
            }
            return -1;
        }
    }
}