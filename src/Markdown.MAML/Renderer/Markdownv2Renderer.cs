using Markdown.MAML.Model.MAML;
using Markdown.MAML.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown.MAML.Renderer
{
    public class MarkdownV2Renderer
    {
        private StringBuilder _stringBuilder = new StringBuilder();

        public string MamlModelToString(IEnumerable<MamlCommand> mamlCommands)
        {
            _stringBuilder.Clear();
            foreach (var command in mamlCommands)
            {
                AddCommand(command);
            }

            return _stringBuilder.ToString();
        }

        private void AddCommand(MamlCommand command)
        {
            _stringBuilder.AppendFormat("# {0}{1}", command.Name, Environment.NewLine); // name
            _stringBuilder.AppendFormat("## {0}{2}{1}{2}{2}", MarkdownStrings.SYNOPSIS, command.Synopsis, Environment.NewLine);
            AddSyntax(command);
            _stringBuilder.AppendFormat("## {0}{2}{1}{2}{2}", MarkdownStrings.DESCRIPTION, command.Description, Environment.NewLine);
            AddExamples(command);
            AddParameters(command);
            AddInputs(command);
            AddOutputs(command);
            AddLinks(command);
        }

        private void AddLinks(MamlCommand command)
        {
            _stringBuilder.AppendFormat("## {0}{1}{1}", MarkdownStrings.RELATED_LINKS, Environment.NewLine);
            foreach (var link in command.Links)
            {
                _stringBuilder.AppendFormat("[{0}]({1}){2}{2}", link.LinkName, link.LinkUri, Environment.NewLine);
            }
        }

        private void AddInputOutput(MamlInputOutput io)
        {
            _stringBuilder.AppendFormat("### {0}{2}{1}{2}{2}", io.TypeName, io.Description, Environment.NewLine);
        }

        private void AddOutputs(MamlCommand command)
        {
            _stringBuilder.AppendFormat("## {0}{1}{1}", MarkdownStrings.OUTPUTS, Environment.NewLine);
            foreach (var io in command.Outputs)
            {
                AddInputOutput(io);
            }
        }

        private void AddInputs(MamlCommand command)
        {
            _stringBuilder.AppendFormat("## {0}{1}{1}", MarkdownStrings.INPUTS, Environment.NewLine);
            foreach (var io in command.Inputs)
            {
                AddInputOutput(io);
            }
        }

        private void AddParameters(MamlCommand command)
        {
            _stringBuilder.AppendFormat("## {0}{1}{1}", MarkdownStrings.PARAMETERS, Environment.NewLine);
            foreach (var param in command.Parameters)
            {
                AddParameter(param, command);
            }
        }

        private void AddParameter(MamlParameter parameter, MamlCommand command)
        {
            _stringBuilder.AppendFormat("### {0}{2}{1}{2}{2}", parameter.Name, parameter.Description, Environment.NewLine);
            //TODO: command.Syntax
            // to generate ```yaml
        }

        private void AddExamples(MamlCommand command)
        {
            _stringBuilder.AppendFormat("## {0}{1}{1}", MarkdownStrings.EXAMPLES, Environment.NewLine);
            foreach (var example in command.Examples)
            {
                _stringBuilder.AppendFormat("### {0}{1}", example.Title, Environment.NewLine);
                if (example.Introduction != null)
                {
                    _stringBuilder.AppendFormat("{0}{1}{1}", example.Introduction, Environment.NewLine);
                }

                if (example.Code != null)
                {
                    _stringBuilder.AppendFormat("```{1}{0}{1}```{1}{1}", example.Code, Environment.NewLine);
                }

                if (example.Remarks != null)
                {
                    _stringBuilder.AppendFormat("{0}{1}{1}", example.Remarks, Environment.NewLine);
                }
            }
        }

        private void AddSyntax(MamlCommand command)
        {
            _stringBuilder.AppendFormat("## {0}{1}{1}", MarkdownStrings.SYNTAX, Environment.NewLine);
            // TODO
        }
    }
}
