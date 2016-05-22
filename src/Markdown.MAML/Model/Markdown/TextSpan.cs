
using Markdown.MAML.Parser;
using static Markdown.MAML.Parser.MarkdownParser;

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
        public ParserMode ParserMode { get; private set; }

        public TextSpanStyle Style { get; private set; }

        public TextSpan(string spanText, SourceExtent sourceExtent, ParserMode parserMode, TextSpanStyle spanStyle = TextSpanStyle.Normal)
            : base(
                  parserMode == ParserMode.Full ? MarkdownParser.UnwindMarkdownCharsEscaping(spanText.Trim()) : spanText, 
                  sourceExtent)
        {
            this.Style = spanStyle;
            this.ParserMode = parserMode;
        }
    }
}
