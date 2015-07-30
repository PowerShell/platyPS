
namespace Markdown.MAML.Model.Markdown
{
    public abstract class ParagraphSpan
    {
        public string Text { get; private set; }

        public SourceExtent SourceExtent { get; private set; }

        public ParagraphSpan(string spanText, SourceExtent sourceExtent)
        {
            this.Text = spanText;
            this.SourceExtent = sourceExtent;
        }
    }
}
