namespace Markdown.MAML.Model.Markdown
{
    public class HeadingNode : TextNode
    {
        public override MarkdownNodeType NodeType
        {
            get { return MarkdownNodeType.Heading; }
        }

        public int HeadingLevel { get; private set; }

        /// <summary>
        /// Format options that control markdown generation.
        /// </summary>
        public SectionFormatOption FormatOption { get; private set; }

        public HeadingNode(string headingText, int headingLevel, SourceExtent sourceExtent, SectionFormatOption formatOption) 
            : base(headingText, sourceExtent)
        {
            this.HeadingLevel = headingLevel;
            this.FormatOption = formatOption;
        }
    }
}
