using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdown.MAML.Model.Markdown;

namespace Markdown.MAML.Renderer
{
    public class TextRenderer
    {
        private StringBuilder _stringBuilder = new StringBuilder();

        private const string aboutIndentation = "    ";

        private int _maxLineWidth { get; set; }

        public TextRenderer() : this(80) { }

        public TextRenderer(int maxLineWidth)
        {
            _maxLineWidth = maxLineWidth;
        }
        
        public string AboutMarkdownToString(DocumentNode document)
        {
            //ensure that all node types in the about topic are handeled.
            List<MarkdownNodeType> AcceptableNodeTypes = new List<MarkdownNodeType>();
            AcceptableNodeTypes.Add(MarkdownNodeType.Heading);
            AcceptableNodeTypes.Add(MarkdownNodeType.Paragraph);
            AcceptableNodeTypes.Add(MarkdownNodeType.CodeBlock);
            if (document.Children.Any(c => (!AcceptableNodeTypes.Contains(c.NodeType))))
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
                }
            }

            return RenderCleaner.FullNormalization(_stringBuilder.ToString());
        }

        private void AddAboutCodeBlock(CodeBlockNode codeblockNode)
        {
            string[] lines = codeblockNode.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            foreach (string line in lines)
            {
                _stringBuilder.Append(aboutIndentation);
                _stringBuilder.AppendLine(line);
            }

            _stringBuilder.AppendLine();
        }

        private void AddAboutHeading(HeadingNode headingNode, DocumentNode document)
        {
            if (document.Children.ElementAt(0) as HeadingNode == headingNode)
            {
                _stringBuilder.AppendLine("TOPIC");
            }
            else if (headingNode.HeadingLevel == 1)
            {
                _stringBuilder.AppendLine(headingNode.Text.ToUpper());
            }
            else if (headingNode.HeadingLevel == 2 && document.Children.ElementAt(1) == headingNode)
            {
                _stringBuilder.AppendFormat("{0}{1}{2}{3}", 
                                                aboutIndentation, 
                                                headingNode.Text.ToLower(),
                                                Environment.NewLine, Environment.NewLine);
            }
            else if (headingNode.HeadingLevel == 2)
            {
                _stringBuilder.AppendLine(headingNode.Text);
            }
            else
            {
                _stringBuilder.AppendFormat("{0}{1}{2}", 
                                                aboutIndentation, 
                                                headingNode.Text.ToUpper(),Environment.NewLine);
            }
        }

        private void AddAboutParagraph(ParagraphNode paragraphNode)
        {
            foreach (string lineContent in paragraphNode.Spans.Select(span => span.Text))
            {
                //handles all paragraph lines over 80 characters long and not headers
                if (lineContent.Length > _maxLineWidth - 4)
                {
                    WrapAndAppendLines(lineContent, _stringBuilder);
                    _stringBuilder.AppendLine();
                }
                else if (StringComparer.OrdinalIgnoreCase.Equals(lineContent,"\n"))
                {
                    _stringBuilder.AppendLine();
                }
                else
                {
                    _stringBuilder.AppendFormat("{0}{1}{2}", 
                                                    aboutIndentation, 
                                                    lineContent,Environment.NewLine);
                }
            }
            _stringBuilder.AppendLine();
        }

        public void WrapAndAppendLines(string text, StringBuilder sb)
        {
            string SingleSpace = " ";

            string[] words = text.Split(' ');
            text = "";

            foreach (string word in words)
            {
                if (word.Contains("\r\n"))
                {
                    string[] breakLine = word.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string part in breakLine)
                    {
                        if (part == breakLine.Last())
                        {
                            text += part + SingleSpace;
                        }
                        else
                        {
                            text += part;
                            sb.AppendFormat("{0}{1}{2}",aboutIndentation,text,Environment.NewLine);
                            text = "";
                        }
                    }
                }
                else if (text.Length + word.Length > (this._maxLineWidth - 4))
                {
                    if (StringComparer.OrdinalIgnoreCase.Equals(text.Substring(text.Length - 1),SingleSpace))
                    {
                        text = text.Substring(0, text.Length - 1);
                    }
                    sb.AppendFormat("{0}{1}{2}", aboutIndentation, text, Environment.NewLine);

                    text = word + SingleSpace;
                }
                else
                {
                    text += word + SingleSpace;
                }

            }

            if (text.Length > 0 && !StringComparer.OrdinalIgnoreCase.Equals(text,SingleSpace))
            {
                if (StringComparer.OrdinalIgnoreCase.Equals(text.Substring(text.Length - 1),SingleSpace))
                {
                    text = text.Substring(0, text.Length - 1);
                }

                sb.AppendFormat("{0}{1}",aboutIndentation, text);
            }
        }
    }
}
