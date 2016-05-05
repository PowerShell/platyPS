using Markdown.MAML.Model.MAML;
using Markdown.MAML.Model.Markdown;
using Markdown.MAML.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace Markdown.MAML.Transformer
{
    public class ModelTransformerVersion2 : ModelTransformerBase
    {
        List<Tuple<string, Dictionary<string, MamlParameter>>> _parameterName2ParameterSetMap = 
            new List<Tuple<string, Dictionary<string, MamlParameter>>>();

        private static readonly string ALL_PARAM_SETS = "*";

        private static readonly string[] LINE_BREAKS = new [] { "\r\n", "\n" };
        private static readonly char[] YAML_SEPARATORS = new [] { ':' };

        public ModelTransformerVersion2() : this(null) { }

        /// <summary>
        /// </summary>
        /// <param name="infoCallback">Report string information to some channel</param>
        public ModelTransformerVersion2(Action<string> infoCallback) : base(infoCallback) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns>true if Section was found</returns>
        override protected bool SectionDispatch(MamlCommand command)
        {
            var node = GetNextNode();
            var headingNode = GetHeadingWithExpectedLevel(node, COMMAND_ENTRIES_HEADING_LEVEL);
            if (headingNode == null)
            {
                return false;
            }

            // TODO: When we are going to implement Localization story, we would need to replace
            // this strings by MarkdownStrings values.
            switch (headingNode.Text.ToUpper())
            {
                case "DESCRIPTION":
                    {
                        command.Description = SimpleTextSectionRule();
                        break;
                    }
                case "SYNOPSIS":
                    {
                        command.Synopsis = SimpleTextSectionRule();
                        break;
                    }
                case "SYNTAX":
                    {
                        SyntaxRule(command);
                        break;
                    }
                case "EXAMPLES":
                    {
                        ExamplesRule(command);
                        break;
                    }
                case "PARAMETERS":
                    {
                        ParametersRule(command);
                        break;
                    }
                case "INPUTS":
                    {
                        InputsRule(command);
                        break;
                    }
                case "OUTPUTS":
                    {
                        OutputsRule(command);
                        break;
                    }
                case "NOTES":
                    {
                        command.Notes = SimpleTextSectionRule();
                        break;
                    }
                case "RELATED LINKS":
                    {
                        RelatedLinksRule(command);
                        break;
                    }
                default:
                    {
                        throw new HelpSchemaException(headingNode.SourceExtent, "Unexpected header name " + headingNode.Text);
                    }
            }
            return true;
        }

        protected void SyntaxRule(MamlCommand commmand)
        {
            MamlSyntax syntax;
            while ((syntax = SyntaxEntryRule()) != null)
            {
                // We ignore Syntax section from markdown in the transformation
                // We get all nessesary info from parameters section
            }
        }

        protected MamlSyntax SyntaxEntryRule()
        {
            // grammar:
            // ### ParameterSetName 
            // ```
            // code
            // ```
            var node = GetNextNode();
            var headingNode = GetHeadingWithExpectedLevel(node, PARAMETERSET_NAME_HEADING_LEVEL);
            if (headingNode == null)
            {
                return null;
            }

            MamlSyntax syntax = new MamlSyntax()
            {
                ParameterSetName = headingNode.Text
            };

            var codeBlock = CodeBlockRule();
            // we don't use the output of it
            // TODO: we should capture syntax and verify that it's complient.
            return syntax;
        }

        protected void ParametersRule(MamlCommand command)
        {
            _parameterName2ParameterSetMap.Clear();
            while (ParameterRule(command))
            {
            }

            GatherSyntax(command);
        }

        private void FillUpSyntax(MamlSyntax syntax, string name)
        {
            foreach (var pair in _parameterName2ParameterSetMap)
            {
                MamlParameter param = null;
                if (pair.Item2.ContainsKey(name))
                {
                    param = pair.Item2[name];
                }
                else
                {
                    if (pair.Item2.Count == 1 && pair.Item2.First().Key == ALL_PARAM_SETS)
                    {
                        param = pair.Item2.First().Value;
                    }
                }
                if (param != null)
                {
                    syntax.Parameters.Add(param);
                }
            }
        }

        private void GatherSyntax(MamlCommand command)
        {
            // Inefficient alogrithm, but it's fine, because all collections are pretty small.
            var parameterSetNames = GetParameterSetNames();

            if (parameterSetNames.Count == 0)
            {
                // special case: there are no parameters and hence there is only one parameter set
                MamlSyntax syntax = new MamlSyntax();
                command.Syntax.Add(syntax);
            }

            foreach (var setName in parameterSetNames)
            {
                MamlSyntax syntax = new MamlSyntax();
                if (setName == ALL_PARAM_SETS)
                {
                    if (parameterSetNames.Count == 1)
                    {
                        // special case: there is only one parameter set and it's the deafult one
                        // we don't specify the name in this case.
                    }
                    else
                    {
                        continue;
                    }                    
                }
                else
                {
                    syntax.ParameterSetName = setName;
                }

                FillUpSyntax(syntax, setName);
                command.Syntax.Add(syntax);
            }
        }

        private List<string> GetParameterSetNames()
        {
            var parameterSetNames = new List<string>();
            foreach (var pair in _parameterName2ParameterSetMap)
            {
                foreach (var pair2 in pair.Item2)
                {
                    var paramSetName = pair2.Key;
                    
                    bool found = false;
                    foreach (var candidate in parameterSetNames)
                    {
                        if (StringComparer.OrdinalIgnoreCase.Equals(candidate, paramSetName))
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        parameterSetNames.Add(paramSetName);
                    }
                }
            }

            return parameterSetNames;
        }

        private bool IsKnownKey(string key)
        {
            return StringComparer.OrdinalIgnoreCase.Equals(key, MarkdownStrings.Type) ||
                StringComparer.OrdinalIgnoreCase.Equals(key, MarkdownStrings.Parameter_Sets) ||
                StringComparer.OrdinalIgnoreCase.Equals(key, MarkdownStrings.Aliases) ||
                StringComparer.OrdinalIgnoreCase.Equals(key, MarkdownStrings.Accepted_values) ||
                StringComparer.OrdinalIgnoreCase.Equals(key, MarkdownStrings.Required) ||
                StringComparer.OrdinalIgnoreCase.Equals(key, MarkdownStrings.Position) ||
                StringComparer.OrdinalIgnoreCase.Equals(key, MarkdownStrings.Default_value) ||
                StringComparer.OrdinalIgnoreCase.Equals(key, MarkdownStrings.Accept_pipeline_input) ||
                StringComparer.OrdinalIgnoreCase.Equals(key, MarkdownStrings.Accept_wildcard_characters);
        }

        /// <summary>
        /// we only parse simple key-value pairs here
        /// </summary>
        /// <param name="yamlSnippet"></param>
        /// <returns></returns>
        private Dictionary<string, string> ParseYamlKeyValuePairs(CodeBlockNode yamlSnippet)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string lineIterator in yamlSnippet.Text.Split(LINE_BREAKS, StringSplitOptions.None))
            {
                var line = lineIterator.Trim();
                if (string.IsNullOrEmpty(line.Trim()))
                {
                    continue;
                }

                string[] parts = line.Split(YAML_SEPARATORS, 2);
                if (parts.Length != 2)
                {
                    throw new HelpSchemaException(yamlSnippet.SourceExtent, "Invalid yaml: expected simple key-value pairs");
                }

                var key = parts[0].Trim();
                if (!IsKnownKey(key))
                {
                    throw new HelpSchemaException(yamlSnippet.SourceExtent, "Invalid yaml: unknown key " + key);
                }

                result[parts[0].Trim()] = parts[1].Trim();
            }

            return result;
        }

        private string[] SplitByCommaAndTrim(string input)
        {
            return input.Split(',').Select(x => x.Trim()).ToArray();
        }

        private void FillUpParameterFromKeyValuePairs(Dictionary<string, string> pairs, MamlParameter parameter)
        {
            string value;
            parameter.Type = pairs.TryGetValue(MarkdownStrings.Type, out value) ? value : "object";
            parameter.Aliases = pairs.TryGetValue(MarkdownStrings.Aliases, out value) ? SplitByCommaAndTrim(value) : new string [0];
            parameter.ParameterValueGroup.AddRange(pairs.TryGetValue(MarkdownStrings.Accepted_values, out value) ? SplitByCommaAndTrim(value) : new string[0]);
            parameter.Required = pairs.TryGetValue(MarkdownStrings.Required, out value) ? StringComparer.OrdinalIgnoreCase.Equals("true", value) : false;
            parameter.Position = pairs.TryGetValue(MarkdownStrings.Position, out value) ? value : "named";
            parameter.DefaultValue = pairs.TryGetValue(MarkdownStrings.Default_value, out value) ? value : null;
            parameter.PipelineInput = pairs.TryGetValue(MarkdownStrings.Accept_pipeline_input, out value) ? value : "false";
            parameter.Globbing = pairs.TryGetValue(MarkdownStrings.Accept_wildcard_characters, out value) ? StringComparer.OrdinalIgnoreCase.Equals("true", value) : false;
        }

        private bool ParameterRule(MamlCommand commmand)
        {
            // grammar:
            // #### Name
            // Description              -  optional
            //
            // ```yaml                  -  one entry for every unique parameter metadata set
            // ...
            // ```

            var node = GetNextNode();
            var headingNode = GetHeadingWithExpectedLevel(node, PARAMETER_NAME_HEADING_LEVEL);
            if (headingNode == null)
            {
                return false;
            }

            var name = headingNode.Text;

            MamlParameter parameter = new MamlParameter()
            {
                Name = name,
                Extent = headingNode.SourceExtent
            };

            parameter.Description = GetTextFromParagraphNode(ParagraphNodeRule());

            // we are filling up two piences here: Syntax and Parameters
            // we are adding this parameter object to the parameters and later modifying it
            // in the rare case, when there are multiply yaml snippets,
            // the first one should be present in the resulted maml in the Parameters section
            // (all of them would be present in Syntax entry)
            commmand.Parameters.Add(parameter);

            var parameterSetMap = new Dictionary<string, MamlParameter>(StringComparer.OrdinalIgnoreCase);
            _parameterName2ParameterSetMap.Add(Tuple.Create(name, parameterSetMap));

            CodeBlockNode codeBlock;
            while ((codeBlock = CodeBlockRule()) != null)
            {
                var yaml = ParseYamlKeyValuePairs(codeBlock);
                FillUpParameterFromKeyValuePairs(yaml, parameter);
                if (yaml.ContainsKey(MarkdownStrings.Parameter_Sets))
                {
                    foreach (string parameterSetName in SplitByCommaAndTrim(yaml[MarkdownStrings.Parameter_Sets]))
                    {
                        if (string.IsNullOrEmpty(parameterSetName))
                        {
                            continue;
                        }

                        parameterSetMap[parameterSetName] = parameter;
                    }
                }
                else
                {
                    parameterSetMap[ALL_PARAM_SETS] = parameter;
                }

                // in the rare case, when there are multiply yaml snippets
                parameter = parameter.Clone();
            }

            return true;
        }
    }
}
