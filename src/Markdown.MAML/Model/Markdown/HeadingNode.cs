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
        /// Additional options that determine how the section will be formated when rendering markdown. This options will be passed on to MAML models when they are generated.
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
