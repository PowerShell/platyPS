
namespace Markdown.MAML.Model
{
    public class TextSpan : ParagraphSpan
    {
        public bool IsBold { get; private set; }

        public bool IsItalic { get; private set; }

        public TextSpan(string spanText, bool isBold = false, bool isItalic = false)
            : base(spanText)
        {
            this.IsBold = isBold;
            this.IsItalic = isItalic;
        }
    }
}
