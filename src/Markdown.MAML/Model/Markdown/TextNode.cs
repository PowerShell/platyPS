namespace Markdown.MAML.Model.Markdown
{
    public abstract class TextNode : MarkdownNode
    {
        public string Text { get; private set; }

        public SourceExtent SourceExtent { get; private set; }

        public TextNode(string textContents, SourceExtent sourceExtent)
        {
            this.Text = textContents;
            this.SourceExtent = sourceExtent;
        }
    }
}
