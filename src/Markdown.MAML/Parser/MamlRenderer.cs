using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdown.MAML.Model;

namespace Markdown.MAML.Parser
{
    /// <summary>
    /// This class contain logic to render maml from the Model.
    /// </summary>
    public class MamlRenderer
    {
        private enum State
        {
            None,
            Command,
            
        }

        private StringBuilder _stringBuilder = new StringBuilder();
        private DocumentNode _root;
        private State _state;
        private Stack<string> _tagStack;
        private IEnumerator<MarkdownNode> _rootEnumerator;

        public const string XML_PREAMBULA = @"<?xml version=""1.0"" encoding=""utf-8""?>
<helpItems schema=""maml"">
";
        public const string COMMAND_PREAMBULA = @"<command:command xmlns:maml=""http://schemas.microsoft.com/maml/2004/10"" xmlns:command=""http://schemas.microsoft.com/maml/dev/command/2004/10"" xmlns:dev=""http://schemas.microsoft.com/maml/dev/2004/10"" xmlns:MSHelp=""http://msdn.microsoft.com/mshelp"">";

        private const int COMMAND_NAME_HEADING_LEVEL = 2;
        private const int COMMAND_ENTRIES_HEADING_LEVEL = 3;

        public MamlRenderer(DocumentNode root)
        {
            _root = root;
            _tagStack = new Stack<string>();
            if (_root.Children == null)
            {
                // HACK:
                _rootEnumerator = (new LinkedList<MarkdownNode>()).GetEnumerator();
            }
            else
            {
                _rootEnumerator = _root.Children.GetEnumerator();
            }
        }

        private void PushTag(string tag)
        {
            _stringBuilder.AppendFormat("<{0}>\n", tag);
            _tagStack.Push(tag);
        }

        private void PopTag(string tag)
        {
            string poped = _tagStack.Pop();
            if (poped != tag)
            {
                throw new FormatException("Expecting pop " + tag + ", but got " + poped);
            }
            _stringBuilder.AppendFormat("</{0}>\n", tag);
        }

        private void PopAllTags()
        {
            PopTag(_tagStack.Count);
        }

        private void PopTag(int count)
        {
            for (int i = 0; i < count; i++) 
            { 
                _stringBuilder.AppendFormat("</{0}>\n", _tagStack.Pop());
            }
        }

        public string ToMamlString()
        {
            _stringBuilder.Clear();
            _state = State.None;
            _stringBuilder.AppendLine(XML_PREAMBULA);
             
            AddCommands();

            _stringBuilder.AppendLine("</helpItems>");
            return _stringBuilder.ToString();
        }

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

        private void AddCommands()
        {
            MarkdownNode markdownNode;
            _stringBuilder.AppendLine(COMMAND_PREAMBULA);
            while ((markdownNode = GetNextNode()) != null)
            {
                if (markdownNode is HeadingNode)
                {
                    var headingNode = markdownNode as HeadingNode;
                    switch (headingNode.HeadingLevel)
                    {
                        case COMMAND_NAME_HEADING_LEVEL :
                        {
                            PopAllTags();
                            _state = State.Command;

                            PushTag("command:details");
                            _stringBuilder.AppendFormat("<command:name>{0}</command:name>", headingNode.Text);
                            AddDescription();
                            PopTag("command:details");
                            
                            break;
                        }
                        default: throw new HelpSchemaException("Booo, I don't know what is the heading level " + headingNode.HeadingLevel);
                    }
                }
            }
            //PopAllTags();

            _stringBuilder.AppendLine("</command:command>");
        }

        private void AddDescription()
        {
            var node = GetNextNode();

            // check for appropriate header
            if (node.NodeType != MarkdownNodeType.Heading)
            {
                throw new HelpSchemaException("Expect ###DESCRIPTION");
            }

            var headingNode = node as HeadingNode;
            if (headingNode.HeadingLevel != COMMAND_ENTRIES_HEADING_LEVEL)
            {
                throw new HelpSchemaException("Expect ###DESCRIPTION");
            }

            if (StringComparer.OrdinalIgnoreCase.Compare(headingNode.Text, "DESCRIPTION") != 0)
            {
                throw new HelpSchemaException("Expect ###DESCRIPTION");
            }

            node = GetNextNode();
            if (node.NodeType != MarkdownNodeType.Paragraph)
            {
                throw new HelpSchemaException("Expect DESCRIPTION text");
            }

            PushTag("maml:description");
            PushTag("maml:para");
            _stringBuilder.AppendLine((node as ParagraphNode).Text);
            PopTag(2);
        }
    }
}
