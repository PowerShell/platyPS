namespace Markdown.MAML.Model.Markdown
{
    public abstract class TextNode : MarkdownNode
    {
        public string Text { get; private set; }

        public TextNode(string textContents)
        {
            this.Text = textContents;
        }
    }
}
