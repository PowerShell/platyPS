using System;
using System.Collections.Generic;
using System.Text;
using Markdown.MAML.Model.Markdown;
using Markdown.MAML.Model.MAML;

namespace Markdown.MAML.Renderer
{
    /// <summary>
    /// This class contain logic to render maml from the Model.
    /// </summary>
    public class MamlRenderer
    {
        private StringBuilder _stringBuilder = new StringBuilder();
        private Stack<string> _tagStack = new Stack<string>();

        public const string XML_PREAMBULA = @"<?xml version=""1.0"" encoding=""utf-8""?>
<helpItems schema=""maml"">
";
        public const string COMMAND_PREAMBULA = @"<command:command xmlns:maml=""http://schemas.microsoft.com/maml/2004/10"" xmlns:command=""http://schemas.microsoft.com/maml/dev/command/2004/10"" xmlns:dev=""http://schemas.microsoft.com/maml/dev/2004/10"" xmlns:MSHelp=""http://msdn.microsoft.com/mshelp"">";

        private void PushTag(string tag)
        {
            _stringBuilder.AppendFormat("<{0}>{1}", tag, Environment.NewLine);
            _tagStack.Push(tag);
        }

        private void PopTag(string tag)
        {
            string poped = _tagStack.Pop();
            if (poped != tag)
            {
                throw new FormatException("Expecting pop " + tag + ", but got " + poped);
            }
            _stringBuilder.AppendFormat("</{0}>{1}", tag, Environment.NewLine);
        }

        private void PopAllTags()
        {
            PopTag(_tagStack.Count);
        }

        private void PopTag(int count)
        {
            for (int i = 0; i < count; i++) 
            {
                _stringBuilder.AppendFormat("</{0}>{1}", _tagStack.Pop(), Environment.NewLine);
            }
        }

        public string MamlModelToString(IEnumerable<MamlCommand> mamlCommands)
        {
            _stringBuilder.Clear();
            _stringBuilder.AppendLine(XML_PREAMBULA);

            AddCommands(mamlCommands);

            _stringBuilder.AppendLine("</helpItems>");
            return _stringBuilder.ToString();
        }

        private void AddCommands(IEnumerable<MamlCommand> mamlCommands)
        {
            _stringBuilder.AppendLine(COMMAND_PREAMBULA);
            foreach (var command in mamlCommands)
            {
                PopAllTags();

                // NAME
                PushTag("command:details");
                _stringBuilder.AppendFormat("<command:name>{0}</command:name>", command.Name);
                // SYNOPSIS
                AddSynopsis(command);
                PopTag("command:details");

                // DESCRIPTION
                AddDescription(command);
                
                break;
            }
            _stringBuilder.AppendLine("</command:command>");
        }

        private void AddSynopsis(MamlCommand command)
        {
            PushTag("maml:description");
            PushTag("maml:para");
            _stringBuilder.AppendLine(command.Synopsis);
            PopTag(2);
        }

        private void AddDescription(MamlCommand command)
        {
            PushTag("maml:description");
            PushTag("maml:para");
            _stringBuilder.AppendLine(command.Description);
            PopTag(2);
        }
    }
}
