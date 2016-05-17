using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Markdown.MAML.Model.Markdown;
using Markdown.MAML.Model.MAML;
using System.Management.Automation;

namespace Markdown.MAML.Transformer
{
    public abstract class ModelTransformerBase : IModelTransformer
    {
        private DocumentNode _root;
        private IEnumerator<MarkdownNode> _rootEnumerator;
        private Action<string> _infoCallback;

        internal const int COMMAND_NAME_HEADING_LEVEL = 1;
        internal const int COMMAND_ENTRIES_HEADING_LEVEL = 2;
        internal const int PARAMETER_NAME_HEADING_LEVEL = 3;
        internal const int INPUT_OUTPUT_TYPENAME_HEADING_LEVEL = 3;
        internal const int EXAMPLE_HEADING_LEVEL = 3;
        internal const int PARAMETERSET_NAME_HEADING_LEVEL = 3;

        public ModelTransformerBase() : this(null) {}

        /// <summary>
        /// </summary>
        /// <param name="infoCallback">Report string information to some channel</param>
        public ModelTransformerBase(Action<string> infoCallback)
        {
            _infoCallback = infoCallback;
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
            var codeBlock = CodeBlockRule();

            example.Code = codeBlock.Text;
            example.Remarks = GetTextFromParagraphNode(ParagraphNodeRule());
            
            return example;
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
                TypeName = headingNode.Text
            };

            typeEntity.Description = SimpleTextSectionRule();

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

            return new SourceExtent("", 0, 0, 0, 0);
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
                    UngetNode(node);
                    return null;
                default:
                    throw new HelpSchemaException(GetExtent(node), "Expect CodeBlock");
            }

            return node as CodeBlockNode;
        }

        private string GetTextFromParagraphSpans(IEnumerable<ParagraphSpan> spans)
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            bool previousIsHyperLink = false;
            foreach (var paragraphSpan in spans)
            {
                // TODO: make it handle hyperlinks, codesnippets, etc more wisely

                HyperlinkSpan hyperlink = paragraphSpan as HyperlinkSpan;
                if (!first && hyperlink != null)
                {
                    sb.Append(" ");
                }
                else if (previousIsHyperLink)
                {
                    sb.Append(" ");
                }
                
                sb.Append(paragraphSpan.Text);
                if (hyperlink != null)
                {
                    if (!string.IsNullOrWhiteSpace(hyperlink.Uri))
                    {
                        sb.AppendFormat(" ({0})", hyperlink.Uri);
                    }
                    else
                    {
                        previousIsHyperLink = paragraphSpan is HyperlinkSpan;
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
    }
}
