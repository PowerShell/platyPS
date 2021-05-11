namespace Markdown.MAML.Model.Markdown
{
    /// <summary>
    /// A section of text with formatting options.
    /// </summary>
    public sealed class SectionBody
    {
        public SectionBody(string text, SectionFormatOption formatOption)
        {
            Text = text;
            FormatOption = formatOption;
        }

        public SectionBody(string text)
            : this(text, SectionFormatOption.None)
        { }

        /// <summary>
        /// The text of the section body.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Additional options that determine how the section will be formated when rendering markdown.
        /// </summary>
        public SectionFormatOption FormatOption { get; private set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
