using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Markdown.MAML.Model.Markdown;
using Markdown.MAML.Model.MAML;
using Markdown.MAML.Parser;
using Markdown.MAML.Transformer;

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
            _stringBuilder.AppendFormat("<{0}>", tag);
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

        /// <summary>
        /// This is a helper method to do all 3 steps.
        /// </summary>
        /// <param name="markdown"></param>
        /// <returns></returns>
        public static string MarkdownStringToMamlString(string markdown)
        {
            var parser = new MarkdownParser();
            var transformer = new ModelTransformer();
            var renderer = new MamlRenderer();

            var markdownModel = parser.ParseString(markdown);
            var mamlModel = transformer.NodeModelToMamlModel(markdownModel);
            string maml = renderer.MamlModelToString(mamlModel);

            return maml;
        }

        public string MamlModelToString(IEnumerable<MamlCommand> mamlCommands)
        {
            return MamlModelToString(mamlCommands, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mamlCommands"></param>
        /// <param name="skipPreambula">Don't include header and footer for xml doc</param>
        /// <returns></returns>
        public string MamlModelToString(IEnumerable<MamlCommand> mamlCommands, bool skipPreambula)
        {
            _stringBuilder.Clear();
            if (!skipPreambula)
            {
                _stringBuilder.AppendLine(XML_PREAMBULA);  
            }

            AddCommands(mamlCommands);

            if (!skipPreambula)
            {
                _stringBuilder.AppendLine("</helpItems>");
            }

            return _stringBuilder.ToString();
        }

        private void AddCommands(IEnumerable<MamlCommand> mamlCommands)
        {
            foreach (var command in mamlCommands)
            {
                _stringBuilder.AppendLine(COMMAND_PREAMBULA);

                #region NAME, VERB, NOUN, + SYNOPSIS
                PushTag("command:details");
                _stringBuilder.AppendFormat("<command:name>{0}</command:name>{1}", XmlEscape(command.Name), Environment.NewLine);
                var splittedName = command.Name.Split('-');
                var verb = splittedName[0];
                var noun = command.Name.Substring(Math.Min(verb.Length + 1, command.Name.Length));
                _stringBuilder.AppendFormat("<command:verb>{0}</command:verb>{2}<command:noun>{1}</command:noun>{2}", XmlEscape(verb), XmlEscape(noun), Environment.NewLine);
                AddSynopsis(command);
                PopTag("command:details");
                #endregion // NAME, VERB, NOUN, + SYNOPSIS

                AddDescription(command);
                AddSyntax(command);
                AddParameters(command);
                AddInputs(command);
                AddOutputs(command);
                AddNotes(command);
                AddExamples(command);
                AddLinks(command);

                _stringBuilder.AppendLine("</command:command>");
            }
        }

        private void AddSyntax(MamlCommand command)
        {
            PushTag("command:syntax");
            foreach (MamlSyntax syntaxItem in command.Syntax)
            {
                PushTag("command:syntaxItem");
                _stringBuilder.AppendFormat("<maml:name>{0}</maml:name>{1}", XmlEscape(command.Name), Environment.NewLine);
                foreach (MamlParameter parameter in syntaxItem.Parameters)
                {
                    AddParameter(command, parameter, inSyntax: true);
                }
                PopTag("command:syntaxItem");
            }
            PopTag("command:syntax");
        }

        private void AddLinks(MamlCommand command)
        {
            PushTag("command:relatedLinks");
            foreach (MamlLink Link in command.Links)
            {
                PushTag("maml:navigationLink");

                PushTag("maml:linkText");
                _stringBuilder.Append(XmlEscape(Link.LinkName));
                PopTag(1);

                PushTag("maml:uri");
                _stringBuilder.Append(XmlEscape(Link.LinkUri));
                PopTag(1);

                PopTag(1);
            }
            PopTag(1);
        }

        private void AddExamples(MamlCommand command)
        {
            PushTag("command:examples");
            foreach (MamlExample example in command.Examples)
            {
                PushTag("command:example");

                PushTag("maml:title");
                _stringBuilder.Append(XmlEscape(example.Title));
                PopTag("maml:title");

                PushTag("dev:code");
                _stringBuilder.Append(XmlEscape(example.Code));
                PopTag("dev:code");

                PushTag("dev:remarks");
                AddParas(example.Remarks);
                PopTag("dev:remarks");

                PopTag("command:example");
            }
            PopTag("command:examples");
        }

        private void AddNotes(MamlCommand command)
        {
            PushTag("maml:alertSet");
            PushTag("maml:alert");
            AddParas(command.Notes);
            PopTag("maml:alert");
            PopTag("maml:alertSet");
        }

        private void AddOutputs(MamlCommand command)
        {
            PushTag("command:returnValues");
            foreach (var output in command.Outputs)
            {
                PushTag("command:returnValue");

                PushTag("dev:type");
                PushTag("maml:name");
                _stringBuilder.Append(XmlEscape(output.TypeName));
                PopTag("maml:name");
                PopTag("dev:type");

                PushTag("maml:description");
                AddParas(output.Description);
                PopTag("maml:description");

                PopTag("command:returnValue");
            }
            PopTag("command:returnValues");
        }

        private void AddInputs(MamlCommand command)
        {
            PushTag("command:inputTypes");
            foreach (var input in command.Inputs)
            {
                PushTag("command:inputType");

                PushTag("dev:type");
                PushTag("maml:name");
                _stringBuilder.Append(XmlEscape(input.TypeName));
                PopTag("maml:name");
                PopTag("dev:type");

                PushTag("maml:description");
                AddParas(input.Description);
                PopTag("maml:description");

                PopTag("command:inputType");
            }
            PopTag("command:inputTypes");
        }

        private void AddParameters(MamlCommand command)
        {
            PushTag("command:parameters");
            foreach (MamlParameter parameter in command.Parameters)
            {
                AddParameter(command, parameter, inSyntax:false);
            }
            PopTag("command:parameters");
        }

        /// <summary>
        /// This function is reverse of Convert-ParameterTypeTextToType from MamlToMarkdown.psm1
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        private string ConvertPSTypeToMamlType(string typeName)
        {
            if (typeName.ToLower().Equals("switch"))
            {
                return "SwitchParameter";
            }

            return typeName;
        }

        private void AddParameterValueGroup(List<string> parameterValueGroup)
        {
            PushTag("command:parameterValueGroup");
            foreach (string val in parameterValueGroup)
            {
                _stringBuilder.AppendLine("<command:parameterValue required=\"false\" variableLength=\"false\">" + XmlEscape(val) + "</command:parameterValue>");
            }
            PopTag("command:parameterValueGroup");   
        }

        private void AddParameter(MamlCommand command, MamlParameter parameter, bool inSyntax)
        {
            var attributes = "required=\"" + parameter.Required.ToString().ToLower() + "\" " +
                             "variableLength=\"" + parameter.VariableLength.ToString().ToLower() + "\" " +
                             "globbing=\"" + parameter.Globbing.ToString().ToLower() + "\" " +
                             "pipelineInput=\"" + parameter.PipelineInput + "\" " +
                             "position=\"" + parameter.Position.ToLower() + "\" " +
                             "aliases=\"";
            attributes += parameter.Aliases.Length > 0 ? string.Join(", ", parameter.Aliases) : "none";
            attributes += "\"";

            PushTag("command:parameter", attributes);

            PushTag("maml:name");
            _stringBuilder.Append(XmlEscape(parameter.Name));
            PopTag("maml:name");

            PushTag("maml:Description");
            AddParas(parameter.Description);
            PopTag("maml:Description");

            if (inSyntax && parameter.ParameterValueGroup.Count > 0)
            {
                AddParameterValueGroup(parameter.ParameterValueGroup);
            }

            attributes = "required=\"" + parameter.ValueRequired.ToString().ToLower() + "\" " +
                         "variableLength=\"" + parameter.ValueVariableLength.ToString().ToLower();
            attributes += "\"";

            string mamlType = ConvertPSTypeToMamlType(parameter.Type);

            // this is weired quirk inside <syntax>:
            // we don't add [switch] info to make it appear good.
            if (!inSyntax || mamlType != "SwitchParameter")
            {
                PushTag("command:parameterValue", attributes);
                _stringBuilder.Append(XmlEscape(mamlType));
                PopTag("command:parameterValue");
            }

            PushTag("dev:type");
            PushTag("maml:name");
            _stringBuilder.Append(XmlEscape(mamlType));
            PopTag("maml:name");
            _stringBuilder.Append("<maml:uri />");
            PopTag("dev:type");

            // Region defaultValue

            PushTag("dev:defaultValue");

            if (mamlType == "SwitchParameter" && parameter.DefaultValue == null)
            {
                _stringBuilder.Append("False");
            }
            else
            {
                if (parameter.DefaultValue != null)
                {
                    _stringBuilder.Append(XmlEscape(parameter.DefaultValue));
                }
                else
                {
                    // sometimes default is none, but empty makes more sense.
                    // _stringBuilder.Append("none");
                }
            }

            PopTag("dev:defaultValue");
            // closing tag
            PopTag("command:parameter");
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
                    _stringBuilder.AppendLine(XmlEscape(para));
                    PopTag(1);
                }
            }
        }

        private string XmlEscape(string s)
        {
            return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
        }
    }
}
