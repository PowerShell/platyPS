using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Markdown.MAML.Model.Markdown;
using Markdown.MAML.Resources;

namespace Markdown.MAML.Renderer
{
    public class TextRenderer
    {
        private StringBuilder _stringBuilder = new StringBuilder();

        private const string AboutIndentation = "    ";
        
        private const string NewLine = "\r\n";

        private int _maxLineWidth { get; set; }

        public TextRenderer() : this(80) { }

        public TextRenderer(int maxLineWidth)
        {
            _maxLineWidth = maxLineWidth;
        }
        
        public string AboutMarkdownToString(DocumentNode document)
        {
            //ensure that all node types in the about topic are handeled.
            var acceptableNodeTypes = new List<MarkdownNodeType>
            {
                MarkdownNodeType.Heading,
                MarkdownNodeType.Paragraph,
                MarkdownNodeType.CodeBlock
            };
            if (document.Children.Any(c => (!acceptableNodeTypes.Contains(c.NodeType))))
            {
                throw new NotSupportedException("About Topics can only have heading, parapgrah or code block nodes in their Markdown Model.");
            }

            //processes all nodes in order
            foreach (var currentNode in document.Children)
            {
                switch (currentNode.NodeType)
                {
                    case MarkdownNodeType.Paragraph:
                        ParagraphNode paragraphNode = currentNode as ParagraphNode;
                        AddAboutParagraph(paragraphNode);
                        break;
                    case MarkdownNodeType.Heading:
                        HeadingNode headingNode = currentNode as HeadingNode;
                        AddAboutHeading(headingNode, document);
                        break;
                    case MarkdownNodeType.CodeBlock:
                        CodeBlockNode codeblockNode = currentNode as CodeBlockNode;
                        AddAboutCodeBlock(codeblockNode);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return RenderCleaner.FullNormalization(_stringBuilder.ToString());
        }

        private void AddAboutCodeBlock(CodeBlockNode codeblockNode)
        {
            var lines = codeblockNode.Text.Split(new[] {"\r\n"}, StringSplitOptions.None);

            foreach (var line in lines)
            {
                _stringBuilder.Append(AboutIndentation);
                _stringBuilder.AppendLine(line);
            }

            _stringBuilder.AppendLine();
        }

        private void AddAboutHeading(HeadingNode headingNode, DocumentNode document)
        {
            const int level1Heading = 1;
            const int level2Heading = 2;

            if (document.Children.ElementAt(0) as HeadingNode == headingNode)
            {
                _stringBuilder.AppendLine(MarkdownStrings.AboutTopicFirstHeader.ToUpper());
            }
            else if (headingNode.HeadingLevel == level1Heading)
            {
                _stringBuilder.AppendLine(headingNode.Text.ToUpper());
            }
            else if (headingNode.HeadingLevel == level2Heading && document.Children.ElementAt(1) == headingNode)
            {
                _stringBuilder.AppendFormat("{0}{1}{2}{3}", AboutIndentation, headingNode.Text.ToLower(), NewLine, NewLine);
            }
            else if (headingNode.HeadingLevel == level2Heading)
            {
                _stringBuilder.AppendLine(headingNode.Text);
            }
            else
            {
                _stringBuilder.AppendFormat("{0}{1}{2}", AboutIndentation, headingNode.Text.ToUpper(), NewLine);
            }
        }

        private void AddAboutParagraph(ParagraphNode paragraphNode)
        {
            foreach (var lineContent in paragraphNode.Spans.Select(span => span.Text))
            {
                //handles all paragraph lines over 80 characters long and not headers
                if (lineContent.Length > _maxLineWidth - 4)
                {
                    WrapAndAppendLines(lineContent, _stringBuilder);
                    _stringBuilder.AppendLine();
                }
                else if (StringComparer.OrdinalIgnoreCase.Equals(lineContent, "\n"))
                {
                    _stringBuilder.AppendLine();
                }
                else
                {
                    _stringBuilder.AppendFormat("{0}{1}{2}", AboutIndentation, lineContent, NewLine);
                }
            }
            _stringBuilder.AppendLine();
        }

        public void WrapAndAppendLines(string text, StringBuilder sb)
        {
            const string singleSpace = " ";

            var words = text.Split(' ');
            text = "";

            foreach (var word in words)
            {
                if (word.Contains("\r\n"))
                {
                    var breakLine = word.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var part in breakLine)
                    {
                        if (part == breakLine.Last())
                        {
                            text += part + singleSpace;
                        }
                        else
                        {
                            text += part;
                            sb.AppendFormat("{0}{1}{2}", AboutIndentation, text, NewLine);
                            text = "";
                        }
                    }
                }
                else if (text.Length + word.Length > (_maxLineWidth - 4))
                {
                    if (StringComparer.OrdinalIgnoreCase.Equals(text.Substring(text.Length - 1), singleSpace))
                    {
                        text = text.Substring(0, text.Length - 1);
                    }
                    sb.AppendFormat("{0}{1}{2}", AboutIndentation, text, NewLine);

                    text = word + singleSpace;
                }
                else
                {
                    text += word + singleSpace;
                }
            }

            if (text.Length <= 0 || StringComparer.OrdinalIgnoreCase.Equals(text, singleSpace))
            {
                return;
            }
            if (StringComparer.OrdinalIgnoreCase.Equals(text.Substring(text.Length - 1), singleSpace))
            {
                text = text.Substring(0, text.Length - 1);
            }

            sb.AppendFormat("{0}{1}", AboutIndentation, text);
        }
    }
}
