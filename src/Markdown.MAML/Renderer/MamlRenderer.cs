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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagName">the name of tag</param>
        /// <param name="tag">content of tag, i.e. include parameters</param>
        private void PushTag(string tagName, string tag)
        {
            _stringBuilder.AppendFormat("<{0} {1}>", tagName, tag);
            _tagStack.Push(tagName);
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

                // NAME, VERB, NOUN
                PushTag("command:details");
                _stringBuilder.AppendFormat("<command:name>{0}</command:name>{1}", command.Name, Environment.NewLine);
                try
                {
                    string Verb, Noun;
                    string [] Split;
                    Split = command.Name.Split('-');
                    Verb = Split[0];
                    Noun = Split[1];
                    _stringBuilder.AppendFormat("<command:verb>{0}</command:verb>{2}<command:noun>{1}</command:noun>{2}", Verb, Noun, Environment.NewLine);
                }
                //UPDATE THIS
                catch
                {
                    Console.WriteLine("Whoops.....");
                }
                // SYNOPSIS
                AddSynopsis(command);
                PopTag("command:details");

                // DESCRIPTION
                AddDescription(command);

                // PARAMETERS
                PushTag("command:syntax");
                PushTag("command:syntaxItem");
                PushTag("maml:Name");
                _stringBuilder.AppendLine(command.Name);
                PopTag(1);
                PushTag("command:parameters");
                foreach(MamlParameter Parameter in command.Parameters)
                {
                    string Attributes;
                    
                    PushTag("command:parameter");
                    
                    Attributes = "required=\"" + Parameter.Required.ToString() + "\" " +
                                                "variableLength=\"" + Parameter.VariableLength.ToString() + "\" " +
                                                "globbing=\"" + Parameter.Globbing.ToString() + "\" " +
                                                "pipelineInput=\"" + Parameter.PipelineInput.ToString() + "\" " +
                                                "position=\"" + Parameter.Position + "\" " + 
                                                "Aliases=\"";
                    int AliasCount = 0;
                    foreach(string Alias in Parameter.Aliases)
                    {
                        Attributes += Alias + ", ";
                        AliasCount++;
                    }
                    if(AliasCount > 0)
                    {
                        Attributes = Attributes.Substring(Attributes.Length - 2);
                    }
                    Attributes += "\"";

                    PushTag("command:parameter", Attributes);
                    PopTag(1);

                    PushTag("maml:Name");
                    _stringBuilder.AppendLine(Parameter.Name);
                    PopTag(1);

                    PushTag("maml:Description");
                    AddParas(Parameter.Description);
                    PopTag(1);

                    Attributes = "required=\"" + Parameter.ValueRequired.ToString() + "\" " +
                                               "variableLength=\"" + Parameter.ValueVariableLength.ToString();
                    Attributes += "\"";

                    PushTag("command:parameterValue", Attributes);
                    _stringBuilder.AppendLine(Parameter.Type);
                    PopTag(2);
                }
                PopTag(3);

                //INPUTS
                PushTag("command:InputTypes");
                foreach(MamlInputOutput Input in command.Inputs)
                {
                    PushTag("command:InputType");
                    
                    PushTag("dev:Type");
                    _stringBuilder.AppendLine(Input.TypeName);
                    PopTag(1);

                    PushTag("maml:Description");
                    AddParas(Input.Description);
                    PopTag(1);

                    PopTag(1);
                }
                PopTag(1);

                //OUTPUTS
                PushTag("command:returnValues");
                foreach (MamlInputOutput Output in command.Outputs)
                {
                    PushTag("command:returnValue");

                    PushTag("dev:Type");
                    _stringBuilder.AppendLine(Output.TypeName);
                    PopTag(1);

                    PushTag("maml:Description");
                    AddParas(Output.Description);
                    PopTag(1);

                    PopTag(1);
                }
                PopTag(1);

                //NOTES
                PushTag("maml:alertSet");
                PushTag("maml:alert");
                AddParas(command.Notes);
                PopTag(2);

                //EXAMPLES
                PushTag("command:examples");
                foreach (MamlExample Example in command.Examples)
                {
                    PushTag("command:examples");

                    PushTag("maml:Title");
                    _stringBuilder.AppendLine(Example.Title);
                    PopTag(1);

                    PushTag("dev:code");
                    _stringBuilder.AppendLine(Example.Code);
                    PopTag(1);

                    PushTag("dev:remarks");
                    AddParas(Example.Remarks);
                    PopTag(1);

                    PopTag(1);
                }
                PopTag(1);

                //RELATED LINKS
                PushTag("command:RelatedLinks");
                foreach (MamlLinks Link in command.Links)
                {
                    PushTag("maml:NavigationLink");

                    PushTag("maml:LinkText");
                    _stringBuilder.AppendLine(Link.LinkName);
                    PopTag(1);

                    PushTag("maml:URI");
                    _stringBuilder.AppendLine(Link.LinkUri);
                    PopTag(1);

                    PopTag(1);
                }
                PopTag(1);

                break;
            }
            _stringBuilder.AppendLine("</command:command>");
        }

        private void AddSynopsis(MamlCommand command)
        {
            PushTag("maml:description");
            AddParas(command.Synopsis);
            PopTag(1);
        }

        private void AddDescription(MamlCommand command)
        {
            PushTag("maml:description");
            AddParas(command.Description);
            PopTag(1);
        }

        private void AddParas(string Body)
        {
            if (Body != null)
            {
                string[] paragraphs = Body.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                foreach (string para in paragraphs)
                {
                    PushTag("maml:para");
                    _stringBuilder.AppendLine(para);
                    PopTag(1);
                }
            }
        }
    }
}
