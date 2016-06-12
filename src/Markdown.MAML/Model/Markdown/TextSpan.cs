using Markdown.MAML.Parser;

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

        public TextSpan(string spanText, SourceExtent sourceExtent, ParserMode parserMode, TextSpanStyle spanStyle = TextSpanStyle.Normal)
            : base(
                  parserMode == ParserMode.Full 
                  ? MarkdownParser.UnwindMarkdownCharsEscaping(spanText.Trim()) 
                  : spanText, 
                  sourceExtent,
                  parserMode)
        {
            this.Style = spanStyle;
        }
    }
}
