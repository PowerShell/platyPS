namespace Markdown.MAML.Model.Markdown
{
    public class CodeBlockNode : TextNode
    {
        public override MarkdownNodeType NodeType
        {
            get { return MarkdownNodeType.CodeBlock; }
        }

        public CodeBlockNode(string codeBlockContents, SourceExtent sourceExtent)
            : base(codeBlockContents.Trim(), sourceExtent)
        {
        }
    }
}
