
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

        public TextSpan(string spanText, TextSpanStyle spanStyle = TextSpanStyle.Normal)
            : base(spanText.Trim())
        {
            this.Style = spanStyle;
        }
    }
}
