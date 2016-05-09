using Markdown.MAML.Model.MAML;
using Markdown.MAML.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown.MAML.Renderer
{
    /// <summary>
    /// Renders MamlModel as markdown with schema v 2.0.0
    /// </summary>
    public class MarkdownV2Renderer
    {
        private StringBuilder _stringBuilder = new StringBuilder();

        public int MaxSyntaxWidth { get; private set; }

        public MarkdownV2Renderer() : this(120) { }

        public MarkdownV2Renderer(int maxSyntaxWidth)
        {
            this.MaxSyntaxWidth = maxSyntaxWidth;
        }

        /// <summary>
        /// this helper API is handful for calling from PowerShell
        /// </summary>
        /// <param name="mamlCommands"></param>
        /// <returns></returns>
        public string MamlModelToString(MamlCommand mamlCommand)
        {
            return MamlModelToString(new[] { mamlCommand }, null);
        }

        /// <summary>
        /// Convert MamlCommands into markdown. Use yamlHeader for yaml header metadata.
        /// </summary>
        /// <param name="mamlCommands"></param>
        /// <param name="yamlHeader">can be null</param>
        /// <returns></returns>
        public string MamlModelToString(IEnumerable<MamlCommand> mamlCommands, Hashtable yamlHeader)
        {
            // clear, so we can re-use this instance
            _stringBuilder.Clear();
            if (yamlHeader == null)
            {
                yamlHeader = new Hashtable();
            }

            // put version there
            yamlHeader["schema"] = "2.0.0";
            AddYamlHeader(yamlHeader);
            foreach (var command in mamlCommands)
            {
                AddCommand(command);
            }

            return _stringBuilder.ToString();
        }

        private void AddYamlHeader(Hashtable yamlHeader)
        {
            _stringBuilder.AppendFormat("---{0}", Environment.NewLine);
            foreach (DictionaryEntry pair in yamlHeader)
            {
                _stringBuilder.AppendFormat("{0}: {1}{2}", pair.Key.ToString(), pair.Value.ToString() , Environment.NewLine);
            }

            _stringBuilder.AppendFormat("---{0}", Environment.NewLine);
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

        private Dictionary<string, MamlParameter> GetParamSetDictionary(string parameterName, List<MamlSyntax> syntaxes)
        {
            var result = new Dictionary<string, MamlParameter>();
            foreach (var syntax in syntaxes)
            {
                foreach (var param in syntax.Parameters)
                {
                    if (StringComparer.OrdinalIgnoreCase.Equals(parameterName, param.Name))
                    {
                        result[syntax.ParameterSetName] = param;
                        // there could be only one parameter in the param set with the same name
                        break;
                    }
                }
            }
            return result;
        }

        private List<Tuple<List<string>, MamlParameter>> SimplifyParamSets(Dictionary<string, MamlParameter> parameterMap)
        {
            var res = new List<Tuple<List<string>, MamlParameter>>();
            // using a O(n^2) algorithm, because it's simpler and n is very small.
            foreach (var pair in parameterMap)
            {
                var seekValue = pair.Value;
                var paramSetName = pair.Key;
                bool found = false;
                foreach (var tuple in res)
                {
                    if (tuple.Item2.IsMetadataEqual(seekValue))
                    {
                        tuple.Item1.Add(paramSetName);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    // create a new entry
                    var paramSets = new List<string>();
                    paramSets.Add(paramSetName);
                    res.Add(new Tuple<List<string>, MamlParameter>(paramSets, seekValue));
                }
            }

            return res;
        }

        private void AddParameter(MamlParameter parameter, MamlCommand command)
        {
            _stringBuilder.AppendFormat("### {0}{2}{1}{2}{2}", parameter.Name, parameter.Description, Environment.NewLine);
            
            var sets = SimplifyParamSets(GetParamSetDictionary(parameter.Name, command.Syntax));
            foreach (var set in sets)
            {
                _stringBuilder.AppendFormat("```yaml{0}", Environment.NewLine);

                _stringBuilder.AppendFormat("{0}: {1}{2}", MarkdownStrings.Type, parameter.Type, Environment.NewLine);
                _stringBuilder.AppendFormat("{0}: {1}{2}", MarkdownStrings.Parameter_Sets, string.Join(", ", set.Item1), Environment.NewLine);
                _stringBuilder.AppendFormat("{0}: {1}{2}{2}", MarkdownStrings.Aliases, string.Join(", ", parameter.Aliases), Environment.NewLine);

                _stringBuilder.AppendFormat("{0}: {1}{2}", MarkdownStrings.Required, set.Item2.Required, Environment.NewLine);
                _stringBuilder.AppendFormat("{0}: {1}{2}", MarkdownStrings.Position, set.Item2.Position, Environment.NewLine);
                _stringBuilder.AppendFormat("{0}: {1}{2}", MarkdownStrings.Default_value, set.Item2.DefaultValue, Environment.NewLine);
                _stringBuilder.AppendFormat("{0}: {1}{2}", MarkdownStrings.Accept_pipeline_input, set.Item2.PipelineInput, Environment.NewLine);
                _stringBuilder.AppendFormat("{0}: {1}{2}", MarkdownStrings.Accept_wildcard_characters, set.Item2.Globbing, Environment.NewLine);

                _stringBuilder.AppendFormat("```{0}{0}", Environment.NewLine);
            }
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

        private string GetSyntaxString(string commandName, MamlSyntax syntax)
        {
            var sb = new StringBuilder();
            sb.Append(commandName);
            int widthBeforeLastBreak = 0;
            for (int i = 0; i <= syntax.Parameters.Count; i++)
            {
                string paramStr;
                if (i < syntax.Parameters.Count)
                {
                    var param = syntax.Parameters[i];
                    if (param.IsSwitchParameter())
                    {
                        paramStr = string.Format("[-{0}]", param.Name);
                    }
                    else
                    {
                        paramStr = string.Format("-{0}", param.Name);
                        if (!param.IsNamed())
                        {
                            // for positional parameters, we can avoid specifying the name
                            paramStr = string.Format("[{0}]", paramStr);
                        }

                        paramStr = string.Format("{0} <{1}>", paramStr, param.Type);
                        if (!param.Required)
                        {
                            paramStr = string.Format("[{0}]", paramStr);
                        }
                    }
                }
                else
                {
                    paramStr = "[<CommonParameters>]";
                }

                if (sb.Length - widthBeforeLastBreak + paramStr.Length > this.MaxSyntaxWidth)
                {
                    sb.AppendLine();
                    widthBeforeLastBreak = sb.Length;
                }

                sb.AppendFormat(" {0}", paramStr);
            }

            return sb.ToString();
        }

        private void AddSyntax(MamlCommand command)
        {
            _stringBuilder.AppendFormat("## {0}{1}{1}", MarkdownStrings.SYNTAX, Environment.NewLine);
            foreach (var syntax in command.Syntax)
            {
                _stringBuilder.AppendFormat("### {0}{2}```{2}{1}{2}```{2}{2}", syntax.ParameterSetName, GetSyntaxString(command.Name, syntax), Environment.NewLine);
            }
        }
    }
}
