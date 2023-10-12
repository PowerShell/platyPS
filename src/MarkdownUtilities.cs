using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.PowerShell.PlatyPS
{
    internal class MarkdownUtilities
    {
        internal static string GetMarkdownMetadataHeaderReader(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return string.Empty;
            }

            var mdAst = Markdig.Markdown.Parse(content);

            if (mdAst.Count < 2)
            {
                return string.Empty;
            }

            if (mdAst[0] is Markdig.Syntax.ThematicBreakBlock)
            {
                if (mdAst[1] is Markdig.Syntax.HeadingBlock metadata)
                {
                    if (metadata.Inline?.FirstChild is Markdig.Syntax.Inlines.LiteralInline metadataText)
                    {
                        return metadataText.Content.Text;
                    }
                }
            }

            return string.Empty;
        }
    }
}
