namespace Markdown.MAML.Model.Markdown
{
    public class CodeBlockNode : TextNode
    {
        public string LanguageMoniker { get; private set; }

        public override MarkdownNodeType NodeType
        {
            get { return MarkdownNodeType.CodeBlock; }
        }

        public CodeBlockNode(string languageMoniker, string codeBlockContents, SourceExtent sourceExtent)
            : base(codeBlockContents.Trim(), sourceExtent)
        {
            if (!string.IsNullOrEmpty(languageMoniker))
            {
                this.LanguageMoniker = languageMoniker;
            }
        }
    }
}
