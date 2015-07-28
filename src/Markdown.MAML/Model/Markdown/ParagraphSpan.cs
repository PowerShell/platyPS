
namespace Markdown.MAML.Model
{
    public abstract class ParagraphSpan
    {
        public string Text { get; private set; }

        public ParagraphSpan(string spanText)
        {
            this.Text = spanText;
        }
    }
}
