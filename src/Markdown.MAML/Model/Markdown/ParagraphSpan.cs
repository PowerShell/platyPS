
using System.Text.RegularExpressions;

namespace Markdown.MAML.Model.Markdown
{
    public abstract class ParagraphSpan
    {
        public string Text { get; private set; }

        public SourceExtent SourceExtent { get; private set; }

        public static string UnwindMarkdownCharsEscaping(string spanText)
        {
            // this is reverse for this PS code:
            // ((($text -replace '\\','\\\\') -replace '([<>])','\$1') -replace '\\([\[\]\(\)])', '\\$1')
            spanText = spanText
                .Replace("\r\n([^\r])", "$1")

                .Replace(@"\[", @"[")
                .Replace(@"\]", @"]")
                .Replace(@"\(", @"(")
                .Replace(@"\)", @")")

                .Replace(@"\<", "<")
                .Replace(@"\>", ">")

                .Replace(@"\\", @"\");

            return Regex.Replace(spanText, "([^\n])\r\n", "$1 ").Replace(" \r\n", "\r\n");
        }

        public ParagraphSpan(string spanText, SourceExtent sourceExtent)
        {
            this.Text = spanText;
            this.SourceExtent = sourceExtent;
        }
    }
}
