using System;
using System.Linq;
using System.Collections.Generic;
using Markdown.MAML.Model;

namespace Markdown.MAML.Transformer
{
    public class ModelTransformer
    {
        private DocumentNode _root;
        private IEnumerator<MarkdownNode> _rootEnumerator;

        private const int COMMAND_NAME_HEADING_LEVEL = 2;
        private const int COMMAND_ENTRIES_HEADING_LEVEL = 3;

        private MarkdownNode GetCurrentNode()
        {
            return _rootEnumerator.Current;
        }

        private MarkdownNode GetNextNode()
        {
            if (_rootEnumerator.MoveNext())
            {
                return _rootEnumerator.Current;
            }

            return null;
        }

        private string GetSynopsis()
        {
            var node = GetNextNode();

            // check for appropriate header
            if (node.NodeType != MarkdownNodeType.Heading)
            {
                throw new HelpSchemaException("Expect ###SYNOPSIS");
            }

            var headingNode = node as HeadingNode;
            if (headingNode.HeadingLevel != COMMAND_ENTRIES_HEADING_LEVEL)
            {
                throw new HelpSchemaException("Expect ###SYNOPSIS");
            }

            if (StringComparer.OrdinalIgnoreCase.Compare(headingNode.Text, "SYNOPSIS") != 0)
            {
                throw new HelpSchemaException("Expect ###SYNOPSIS");
            }

            node = GetNextNode();
            if (node.NodeType != MarkdownNodeType.Paragraph)
            {
                throw new HelpSchemaException("Expect SYNOPSIS text");
            }

            return (node as ParagraphNode).Spans.First().Text;
        }

        private string GetDescription()
        {
            var node = GetNextNode();

            if (node == null)
            {
                return null;
            }

            // check for appropriate header
            if (node.NodeType != MarkdownNodeType.Heading)
            {
                throw new HelpSchemaException("Expect ###DESCRIPTION");
            }

            var headingNode = node as HeadingNode;
            if (headingNode.HeadingLevel != COMMAND_ENTRIES_HEADING_LEVEL)
            {
                throw new HelpSchemaException("Expect ###SYNOPSIS");
            }

            if (StringComparer.OrdinalIgnoreCase.Compare(headingNode.Text, "SYNOPSIS") != 0)
            {
                throw new HelpSchemaException("Expect ###SYNOPSIS");
            }

            node = GetNextNode();
            if (node.NodeType != MarkdownNodeType.Paragraph)
            {
                throw new HelpSchemaException("Expect SYNOPSIS text");
            }

            return ((node as ParagraphNode).Spans.First().Text);
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
                                    Synopsis = GetSynopsis(),
                                    Description = GetDescription()
                                };
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
