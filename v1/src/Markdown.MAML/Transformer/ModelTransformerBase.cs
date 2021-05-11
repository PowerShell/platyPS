using Markdown.MAML.Model.MAML;
using Markdown.MAML.Model.Markdown;
using Markdown.MAML.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Markdown.MAML.Transformer
{
    public abstract class ModelTransformerBase : IModelTransformer
    {
        private DocumentNode _root;
        private IEnumerator<MarkdownNode> _rootEnumerator;
        private Action<string> _infoCallback;
        private Action<string> _warningCallback;

        internal const int COMMAND_NAME_HEADING_LEVEL = 1;
        internal const int COMMAND_ENTRIES_HEADING_LEVEL = 2;
        internal const int PARAMETER_NAME_HEADING_LEVEL = 3;
        internal const int INPUT_OUTPUT_TYPENAME_HEADING_LEVEL = 3;
        internal const int EXAMPLE_HEADING_LEVEL = 3;
        internal const int PARAMETERSET_NAME_HEADING_LEVEL = 3;

        public ModelTransformerBase(Action<string> infoCallback, Action<string> warningCallback)
        {
            _infoCallback = infoCallback;
            _warningCallback = warningCallback;
        }

        public IEnumerable<MamlCommand> NodeModelToMamlModel(DocumentNode node)
        {
            _root = node;
            if (_root.Children == null)
            {
                // HACK:
                _rootEnumerator = (new LinkedList<MarkdownNode>()).GetEnumerator();
            }
            else
            {
                _rootEnumerator = _root.Children.GetEnumerator();
            }

            List<MamlCommand> commands = new List<MamlCommand>();
            MarkdownNode markdownNode;
            while ((markdownNode = GetNextNode()) != null)
            {
                if (markdownNode is HeadingNode)
                {
                    var headingNode = markdownNode as HeadingNode;
                    switch (headingNode.HeadingLevel)
                    {
                        case COMMAND_NAME_HEADING_LEVEL:
                            {
                                MamlCommand command = new MamlCommand()
                                {
                                    Name = headingNode.Text,
                                    Extent = headingNode.SourceExtent,
                                    // we have explicit entry for common parameters in markdown
                                    SupportCommonParameters = false
                                };

                                if (_infoCallback != null)
                                {
                                    _infoCallback.Invoke("Start processing command " + command.Name);
                                }

                                // fill up command 
                                while (SectionDispatch(command)) { }

                                commands.Add(command);
                                break;
                            }
                        default: throw new HelpSchemaException(headingNode.SourceExtent, "Booo, I don't know what is the heading level " + headingNode.HeadingLevel);
                    }
                }
            }
            return commands;
        }

        private MarkdownNode _ungotNode { get; set; }

        protected MarkdownNode GetCurrentNode()
        {
            if (_ungotNode != null)
            {
                var node = _ungotNode;
                return node;
            }

            return _rootEnumerator.Current;
        }

        protected MarkdownNode GetNextNode()
        {
            if (_ungotNode != null)
            {
                _ungotNode = null;
                return _rootEnumerator.Current;
            }

            if (_rootEnumerator.MoveNext())
            {
                return _rootEnumerator.Current;
            }

            return null;
        }

        protected void UngetNode(MarkdownNode node)
        {
            if (_ungotNode != null)
            {
                throw new ArgumentException("Cannot ungot token, already ungot one");
            }

            _ungotNode = node;
        }

        protected string SimpleTextSectionRule()
        {
            // grammar:
            // Simple paragraph Text
            return GetTextFromParagraphNode(ParagraphNodeRule());
        }
        
        protected void InputsRule(MamlCommand commmand)
        {
            MamlInputOutput input;
            while ((input = InputOutputRule()) != null)
            {
                commmand.Inputs.Add(input);
            }
        }

        protected void OutputsRule(MamlCommand commmand)
        {
            MamlInputOutput output;
            while ((output = InputOutputRule()) != null)
            {
                commmand.Outputs.Add(output);
            }
        }

        protected void ExamplesRule(MamlCommand commmand)
        {
            MamlExample example;
            while ((example = ExampleRule()) != null)
            {
                commmand.Examples.Add(example);
            }
        }

        protected MamlExample ExampleRule()
        {
            // grammar:
            // #### ExampleTitle
            // Introduction
            // ```
            // code
            // ```
            // Remarks
            var node = GetNextNode();
            try
            {
                var headingNode = GetHeadingWithExpectedLevel(node, EXAMPLE_HEADING_LEVEL);

                if (headingNode == null)
                {
                    return null;
                }

                MamlExample example = new MamlExample()
                {
                    Title = headingNode.Text
                };
                example.Introduction = GetTextFromParagraphNode(ParagraphNodeRule());
                example.FormatOption = headingNode.FormatOption;
                CodeBlockNode codeBlockNode;
                List<MamlCodeBlock> codeBlocks = new List<MamlCodeBlock>();

                while ((codeBlockNode = CodeBlockRule()) != null)
                {
                    codeBlocks.Add(new MamlCodeBlock(
                        codeBlockNode.Text,
                        codeBlockNode.LanguageMoniker
                    ));
                }

                example.Code = codeBlocks.ToArray();

                example.Remarks = GetTextFromParagraphNode(ParagraphNodeRule());

                return example;
            }
            catch (HelpSchemaException headingException)
            {
                Report("Schema exception. This can occur when there are multiple code blocks in one example. " + headingException.Message);

                throw headingException;
            }
            
        }

        protected void RelatedLinksRule(MamlCommand commmand)
        {
            var paragraphNode = ParagraphNodeRule();
            if (paragraphNode == null)
            {
                return;
            }

            foreach (var paragraphSpan in paragraphNode.Spans)
            {
                if (paragraphSpan.ParserMode == ParserMode.FormattingPreserve)
                {
                    commmand.Links.Add(new MamlLink(isSimplifiedTextLink: true)
                    {
                        LinkName = paragraphSpan.Text,
                    });
                }
                else
                {
                    var linkSpan = paragraphSpan as HyperlinkSpan;
                    if (linkSpan != null)
                    {
                        commmand.Links.Add(new MamlLink()
                        {
                            LinkName = linkSpan.Text,
                            LinkUri = linkSpan.Uri
                        });
                    }
                    else
                    {
                        throw new HelpSchemaException(paragraphSpan.SourceExtent, "Expect hyperlink, but got " + paragraphSpan.Text);
                    }
                }
            }
        }

        protected MamlInputOutput InputOutputRule()
        {
            // grammar:
            // #### TypeName
            // Description
            var node = GetNextNode();
            var headingNode = GetHeadingWithExpectedLevel(node, INPUT_OUTPUT_TYPENAME_HEADING_LEVEL);
            if (headingNode == null)
            {
                return null;
            }

            MamlInputOutput typeEntity = new MamlInputOutput()
            {
                TypeName = headingNode.Text,
                Description = SimpleTextSectionRule(),
                FormatOption = headingNode.FormatOption
            };

            return typeEntity;
        }

        protected SourceExtent GetExtent(MarkdownNode node)
        {
            TextNode textNode = node as TextNode;
            if (textNode != null)
            {
                return textNode.SourceExtent;
            }
            ParagraphNode paragraphNode = node as ParagraphNode;
            if (paragraphNode != null && paragraphNode.Spans.Any())
            {
                return paragraphNode.Spans.First().SourceExtent;
            }

            return new SourceExtent("", 0, 0, 0, 0, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="level"></param>
        /// <returns>
        /// return headingNode if expected heading level encounterd.
        /// null, if higher level encountered.
        /// throw exception, if unexpected node encountered.
        /// </returns>
        protected HeadingNode GetHeadingWithExpectedLevel(MarkdownNode node, int level)
        {
            if (node == null)
            {
                return null;
            }

            // check for appropriate header
            if (node.NodeType != MarkdownNodeType.Heading)
            {
                throw new HelpSchemaException(GetExtent(node), "Expect Heading");
            }

            var headingNode = node as HeadingNode;
            if (headingNode.HeadingLevel < level)
            {
                UngetNode(node);
                return null;
            }

            if (headingNode.HeadingLevel != level)
            {
                throw new HelpSchemaException(headingNode.SourceExtent, "Expect Heading level " + level);
            }
            return headingNode;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// return paragraphNode if encounterd.
        /// null, if any header level encountered.
        /// throw exception, if other unexpected node encountered.
        /// </returns>
        protected ParagraphNode ParagraphNodeRule()
        {
            var node = GetNextNode();
            if (node == null)
            {
                return null;
            }

            switch (node.NodeType)
            {
                case MarkdownNodeType.Paragraph:
                    break;
                case MarkdownNodeType.CodeBlock:
                case MarkdownNodeType.Heading:
                    UngetNode(node);
                    return null;
                default:
                    throw new HelpSchemaException(GetExtent(node), "Expect Paragraph");
            }

            return node as ParagraphNode;
        }

        protected string ParagraphOrCodeBlockNodeRule(string excludeLanguageMoniker)
        {
            var res = new List<string>();
            MarkdownNode node;

            while ((node = GetNextNode()) != null)
            {
                bool breakFlag = false;
                switch (node.NodeType)
                {
                    case MarkdownNodeType.Paragraph:
                        {
                            res.Add(GetTextFromParagraphNode(node as ParagraphNode));
                            break;
                        }
                    case MarkdownNodeType.CodeBlock:
                        {
                            var codeblock = node as CodeBlockNode;
                            if (!String.Equals(excludeLanguageMoniker, codeblock.LanguageMoniker, StringComparison.OrdinalIgnoreCase))
                            {
                                res.Add(codeblock.Text);
                            }
                            else
                            {
                                UngetNode(node);
                                breakFlag = true;
                            }

                            break;
                        }
                    case MarkdownNodeType.Heading:
                        {
                            UngetNode(node);
                            breakFlag = true;
                            break;
                        }
                    default:
                        {
                            throw new HelpSchemaException(GetExtent(node), "Expect Paragraph or CodeBlock");
                        }
                }

                if (breakFlag)
                {
                    break;
                }                
            }

            return string.Join("\r\n\r\n", res);
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// return paragraphNode if encounterd.
        /// null, if any header level encountered.
        /// throw exception, if other unexpected node encountered.
        /// </returns>
        protected CodeBlockNode CodeBlockRule()
        {
            var node = GetNextNode();
            if (node == null)
            {
                return null;
            }

            switch (node.NodeType)
            {
                case MarkdownNodeType.CodeBlock:
                    break;
                case MarkdownNodeType.Heading:
                case MarkdownNodeType.Paragraph:
                    UngetNode(node);
                    return null;
                default:
                    throw new HelpSchemaException(GetExtent(node), "Expect CodeBlock");
            }

            return node as CodeBlockNode;
        }

        private string GetTextFromParagraphSpans(IEnumerable<ParagraphSpan> spans)
        {
            // in preserve formatting there is only one span all the time
            if (spans.Count() == 1)
            {
                var textSpan = spans.First() as TextSpan;
                if (textSpan.ParserMode == ParserMode.FormattingPreserve)
                {
                    return textSpan.Text;
                }
            }

            StringBuilder sb = new StringBuilder();
            bool first = true;
            bool previousSpanIsSpecial = false;
            foreach (var paragraphSpan in spans)
            {
                // TODO: make it handle hyperlinks, codesnippets, italic, bold etc more wisely
                HyperlinkSpan hyperlink = paragraphSpan as HyperlinkSpan;
                TextSpan text = paragraphSpan as TextSpan;
                bool spanIsSpecial = hyperlink != null || (text != null && text.Style != TextSpanStyle.Normal);
                if (!first && spanIsSpecial)
                {
                    sb.Append(" ");
                }
                else if (previousSpanIsSpecial)
                {
                    sb.Append(" ");
                }
                
                sb.Append(paragraphSpan.Text);
                previousSpanIsSpecial = spanIsSpecial;
                if (hyperlink != null)
                {
                    if (!string.IsNullOrWhiteSpace(hyperlink.Uri))
                    {
                        sb.AppendFormat(" ({0})", hyperlink.Uri);
                        previousSpanIsSpecial = false;
                    }
                }

                first = false;
                
            }
            return sb.ToString();
        }

        protected string GetTextFromParagraphNode(ParagraphNode node)
        {
            if (node == null)
            {
                return "";
            }
            return GetTextFromParagraphSpans(node.Spans);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns>true if Section was found</returns>
        protected abstract bool SectionDispatch(MamlCommand command);

        protected void Report(string warning)
        {
            if (_warningCallback != null)
            {
                _warningCallback.Invoke("Error encountered: " + warning);
            }
        }
    }
}
