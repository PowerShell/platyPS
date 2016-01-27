
namespace Markdown.MAML.Model.Markdown
{
    public enum TextSpanStyle
    {
        Normal,
        Bold,
        Italic
    }

    public class TextSpan : ParagraphSpan
    {
        public TextSpanStyle Style { get; private set; }

        public TextSpan(string spanText, SourceExtent sourceExtent, TextSpanStyle spanStyle = TextSpanStyle.Normal)
            : base(UnwindMarkdownCharsEscaping(spanText.Trim()), sourceExtent)
        {
            this.Style = spanStyle;
        }
    }
}
