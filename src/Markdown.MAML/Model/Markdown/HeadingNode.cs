namespace Markdown.MAML.Model.Markdown
{
    public class HeadingNode : TextNode
    {
        public override MarkdownNodeType NodeType
        {
            get { return MarkdownNodeType.Heading; }
        }

        public int HeadingLevel { get; private set; }

        public HeadingNode(string headingText, int headingLevel, SourceExtent sourceExtent) 
            : base(headingText, sourceExtent)
        {
            this.HeadingLevel = headingLevel;
        }
    }
}
