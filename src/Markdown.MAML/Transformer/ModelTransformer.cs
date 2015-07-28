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

        private string DescriptionRule()
        {
            var node = GetNextNode();
            // TODO: handle hyperlinks, code, etc
            if (node.NodeType != MarkdownNodeType.Paragraph)
            {
                throw new HelpSchemaException("Expect DESCRIPTION text");
            }

            return ((node as ParagraphNode).Spans.First().Text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns>true if Section was found</returns>
        private bool SectionDispatch(MamlCommand command)
        {
            var node = GetNextNode();
            if (node == null)
            {
                return false;
            }

            // check for appropriate header
            if (node.NodeType != MarkdownNodeType.Heading)
            {
                throw new HelpSchemaException("Expect Heading");
            }

            var headingNode = node as HeadingNode;
            if (headingNode.HeadingLevel == COMMAND_NAME_HEADING_LEVEL)
            {
                UngetNode(node);
                return false;
            }

            if (headingNode.HeadingLevel != COMMAND_ENTRIES_HEADING_LEVEL)
            {
                throw new HelpSchemaException("Expect Heading level " + COMMAND_ENTRIES_HEADING_LEVEL);
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
