using System;
using System.Linq;
using System.Collections.Generic;
using Markdown.MAML.Model;
using Markdown.MAML.Model.Markdown;
using Markdown.MAML.Model.MAML;

namespace Markdown.MAML.Transformer
{
    public class ModelTransformer
    {
        private DocumentNode _root;
        private IEnumerator<MarkdownNode> _rootEnumerator;

        private const int COMMAND_NAME_HEADING_LEVEL = 2;
        private const int COMMAND_ENTRIES_HEADING_LEVEL = 3;
        private const int PARAMETER_NAME_HEADING_LEVEL = 4;

        private MarkdownNode _ungotNode { get; set; }

        private MarkdownNode GetCurrentNode()
        {
            if (_ungotNode != null)
            {
                var node = _ungotNode;
                return node;
            }

            return _rootEnumerator.Current;
        }

        private MarkdownNode GetNextNode()
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

        private void UngetNode(MarkdownNode node)
        {
            if (_ungotNode != null)
            {
                throw new ArgumentException("Cannot ungot token, already ungot one");
            }

            _ungotNode = node;
        }

        private string SynopsisRule()
        {
            var node = GetNextNode();
            if (node.NodeType != MarkdownNodeType.Paragraph)
            {
                throw new HelpSchemaException("Expect SYNOPSIS text");
            }

            return (node as ParagraphNode).Spans.First().Text;
        }

        private void ParametersRule(MamlCommand commmand)
        {
            while (ParameterRule(commmand))
            {
            }
        }

        private string DescriptionRule()
        {
            var node = GetNextNode();
            if (node.NodeType != MarkdownNodeType.Paragraph)
            {
                throw new HelpSchemaException("Expect DESCRIPTION text");
            }

            return (GetTextFromParagraphNode(node as ParagraphNode));
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
        private HeadingNode GetHeadingWithExpectedLevel(MarkdownNode node, int level)
        {
            if (node == null)
            {
                return null;
            }

            // check for appropriate header
            if (node.NodeType != MarkdownNodeType.Heading)
            {
                throw new HelpSchemaException("Expect Heading");
            }

            var headingNode = node as HeadingNode;
            if (headingNode.HeadingLevel < level)
            {
                UngetNode(node);
                return null;
            }

            if (headingNode.HeadingLevel != level)
            {
                throw new HelpSchemaException("Expect Heading level " + level);
            }
            return headingNode;
        }

        private string GetTextFromParagraphNode(ParagraphNode node)
        {
            // TODO: make it handle hyperlinks, codesnippets, etc 
            return node.Spans.First().Text;
        }

        private void FillParameterDetailsFromAttribute(MamlParameter parameter, CodeBlockNode node)
        {
            if (node == null)
            {
                return;
            }
            // TODO: add some parsing here
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns>true if Parameter was found</returns>
        private bool ParameterRule(MamlCommand command)
        {
            // grammar:
            // #### Name `[Parameter(...)]`
            // Description
            var node = GetNextNode();
            var headingNode = GetHeadingWithExpectedLevel(node, PARAMETER_NAME_HEADING_LEVEL);
            if (headingNode == null)
            {
                return false;
            }

            MamlParameter parameter = new MamlParameter()
            {
                Name = headingNode.Text
            };

            node = GetNextNode();

            ParagraphNode descriptionNode = null;
            CodeBlockNode attributesNode = null;

            switch (node.NodeType)
            {
                case MarkdownNodeType.Unknown:
                    break;
                case MarkdownNodeType.Document:
                    break;
                case MarkdownNodeType.Paragraph:
                    descriptionNode = node as ParagraphNode;
                    break;
                case MarkdownNodeType.Heading:
                    break;
                case MarkdownNodeType.CodeBlock:
                    attributesNode = node as CodeBlockNode;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (descriptionNode == null)
            {
                node = GetNextNode();
                if (node.NodeType == MarkdownNodeType.Heading)
                {
                    UngetNode(node);
                }
                else
                {
                    if (node.NodeType != MarkdownNodeType.Paragraph)
                    {
                        throw new HelpSchemaException("Expect parameter " + parameter.Name + " description");
                    }
                    descriptionNode = node as ParagraphNode;
                }
            }

            parameter.Description = descriptionNode != null ? GetTextFromParagraphNode(descriptionNode) : "";
            FillParameterDetailsFromAttribute(parameter, attributesNode);
            command.Parameters.Add(parameter);
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns>true if Section was found</returns>
        private bool SectionDispatch(MamlCommand command)
        {
            var node = GetNextNode();
            var headingNode = GetHeadingWithExpectedLevel(node, COMMAND_ENTRIES_HEADING_LEVEL);
            if (headingNode == null)
            {
                return false;
            }

            switch (headingNode.Text.ToUpper())
            {
                case "DESCRIPTION":
                    {
                        command.Description = DescriptionRule();
                        break;
                    }
                case "SYNOPSIS":
                    {
                        command.Synopsis = SynopsisRule();
                        break;
                    }
                case "PARAMETERS":
                    {
                        ParametersRule(command);
                        break;
                    }
                default:
                    {
                        throw new HelpSchemaException("Unexpected header name " + headingNode.Text);
                    }
            }
            return true;
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
                                };
                                // fill up command 
                                while (SectionDispatch(command)) { }

                                commands.Add(command);
                                break;
                            }
                        default: throw new HelpSchemaException("Booo, I don't know what is the heading level " + headingNode.HeadingLevel);
                    }
                }
            }
            return commands;
        }
    }
}
