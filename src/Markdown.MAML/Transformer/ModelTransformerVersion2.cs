using Markdown.MAML.Model.MAML;
using Markdown.MAML.Model.Markdown;
using Markdown.MAML.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using Markdown.MAML.Parser;

namespace Markdown.MAML.Transformer
{
    public class ModelTransformerVersion2 : ModelTransformerBase
    {
        List<Tuple<string, Dictionary<string, MamlParameter>>> _parameterName2ParameterSetMap = 
            new List<Tuple<string, Dictionary<string, MamlParameter>>>();

        public static readonly string ALL_PARAM_SETS_MONIKER = "(All)";
        private String[] _applicableTag;
        public ModelTransformerVersion2() : this(null, null, null) { }

        /// <summary>
        /// </summary>
        /// <param name="infoCallback">Report string information to some callback</param>
        /// <param name="warningCallback">Report string warnings to some callback</param>
        /// <param name="applicableTag">tag to filter out applicable Parameters, null accept everything</param>
        public ModelTransformerVersion2(Action<string> infoCallback, Action<string> warningCallback, String[] applicableTag) : base(infoCallback, warningCallback)
        {
            _applicableTag = applicableTag;
        }

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
                        command.Description = new SectionBody(SimpleTextSectionRule(), headingNode.FormatOption);
                        break;
                    }
                case "SYNOPSIS":
                    {
                        command.Synopsis = new SectionBody(SimpleTextSectionRule(), headingNode.FormatOption);
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
                        command.Notes = new SectionBody(SimpleTextSectionRule(), headingNode.FormatOption);
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
                //this is the only way to retain information on which syntax is the default 
                // without adding new members to command object.
                //Though the cmdlet object, does have a member which contains the default syntax name only.
                if (syntax.IsDefault) { commmand.Syntax.Add(syntax); }
            }
        }

        protected MamlSyntax SyntaxEntryRule()
        {
            // grammar:
            // ### ParameterSetName 
            // ```
            // code
            // ```

            MamlSyntax syntax;

            var node = GetNextNode();
            if (node.NodeType == MarkdownNodeType.CodeBlock)
            {
                // if header is omitted
                syntax = new MamlSyntax()
                {
                    ParameterSetName = ALL_PARAM_SETS_MONIKER,
                    IsDefault = true
                };
            }
            else
            { 
                var headingNode = GetHeadingWithExpectedLevel(node, PARAMETERSET_NAME_HEADING_LEVEL);
                if (headingNode == null)
                {
                    return null;
                }

                bool isDefault = headingNode.Text.EndsWith(MarkdownStrings.DefaultParameterSetModifier);
                syntax = new MamlSyntax()
                {
                    ParameterSetName = isDefault ? headingNode.Text.Substring(0, headingNode.Text.Length - MarkdownStrings.DefaultParameterSetModifier.Length) : headingNode.Text,
                    IsDefault = isDefault
                };

                var codeBlock = CodeBlockRule();
            }
            // we don't use the output of it
            // TODO: we should capture syntax and verify that it's complient.
            return syntax;
        }

        protected void ParametersRule(MamlCommand command)
        {
            while (ParameterRule(command))
            {
            }

            GatherSyntax(command);
        }

        private void FillUpSyntax(MamlSyntax syntax, string name)
        {
            var parametersList = new List<MamlParameter>();
            
            foreach (var pair in _parameterName2ParameterSetMap)
            {
                MamlParameter param = null;
                if (pair.Item2.ContainsKey(name))
                {
                    param = pair.Item2[name];
                }
                else
                {
                    if (pair.Item2.Count == 1 && pair.Item2.First().Key == ALL_PARAM_SETS_MONIKER)
                    {
                        param = pair.Item2.First().Value;
                    }
                }
                if (param != null)
                {
                    parametersList.Add(param);
                }
            }

            // order parameters based on position
            // User OrderBy instead of Sort for stable sort
            syntax.Parameters.AddRange(parametersList.OrderBy(x => x.Position));
        }

        private void GatherSyntax(MamlCommand command)
        {
            var parameterSetNames = GetParameterSetNames();
            var defaultSetName = string.Empty;

            if(command.Syntax.Count == 1 && command.Syntax[0].IsDefault)
            {
                //checks for existing IsDefault paramset and remove it while saving the name
                defaultSetName = command.Syntax[0].ParameterSetName;
                command.Syntax.Remove(command.Syntax[0]);
            }
            
            if (parameterSetNames.Count == 0)
            {
                // special case: there are no parameters and hence there is only one parameter set
                MamlSyntax syntax = new MamlSyntax();
                command.Syntax.Add(syntax);
            }

            foreach (var setName in parameterSetNames)
            {
                MamlSyntax syntax = new MamlSyntax();
                if (setName == ALL_PARAM_SETS_MONIKER)
                {
                    if (parameterSetNames.Count == 1)
                    {
                        // special case: there is only one parameter set and it's the default one
                        // we don't specify the name in this case.
                    }
                    else
                    {
                        continue;
                    }                    
                }
                else
                {
                    syntax.ParameterSetName = StringComparer.OrdinalIgnoreCase.Equals(syntax.ParameterSetName, defaultSetName) 
                        ? string.Format("{0}{1}", setName, MarkdownStrings.DefaultParameterSetModifier) 
                        : setName;
                }

                FillUpSyntax(syntax, setName);
                command.Syntax.Add(syntax);
            }
        }

        private List<string> GetParameterSetNames()
        {
            // Inefficient alogrithm, but it's fine, because all collections are pretty small.
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
                StringComparer.OrdinalIgnoreCase.Equals(key, MarkdownStrings.Accept_wildcard_characters) ||
                StringComparer.OrdinalIgnoreCase.Equals(key, MarkdownStrings.Applicable);
        }

        /// <summary>
        /// we only parse simple key-value pairs here
        /// </summary>
        /// <param name="yamlSnippet"></param>
        /// <returns></returns>
        private Dictionary<string, string> ParseYamlKeyValuePairs(CodeBlockNode yamlSnippet)
        {
            Dictionary<string, string> result;
            try
            {
                result = MarkdownParser.ParseYamlKeyValuePairs(yamlSnippet.Text);
            }
            catch (ArgumentException)
            {
                throw new HelpSchemaException(yamlSnippet.SourceExtent, "Invalid yaml: expected simple key-value pairs");
            }

            foreach (var pair in result)
            {
                if (!IsKnownKey(pair.Key))
                {
                    throw new HelpSchemaException(yamlSnippet.SourceExtent, "Invalid yaml: unknown key " + pair.Key);
                }
            }

            return result;
        }

        private string[] SplitByCommaAndTrim(string input)
        {
            if (input == null)
            {
                return new string[0];
            }

            return input.Split(',').Select(x => x.Trim()).ToArray();
        }

        private void FillUpParameterFromKeyValuePairs(Dictionary<string, string> pairs, MamlParameter parameter)
        {
            // for all null keys, we should ignore the value in this context
            var newPairs = new Dictionary<string, string>(pairs.Comparer);

            foreach (var pair in pairs)
            {
                if (pair.Value != null)
                {
                    newPairs[pair.Key] = pair.Value;
                }
            }

            pairs = newPairs;

            string value;
            parameter.Type = pairs.TryGetValue(MarkdownStrings.Type, out value) ? value : null;
            parameter.Aliases = pairs.TryGetValue(MarkdownStrings.Aliases, out value) ? SplitByCommaAndTrim(value) : new string [0];
            parameter.ParameterValueGroup.AddRange(pairs.TryGetValue(MarkdownStrings.Accepted_values, out value) ? SplitByCommaAndTrim(value) : new string[0]);
            parameter.Required = pairs.TryGetValue(MarkdownStrings.Required, out value) ? StringComparer.OrdinalIgnoreCase.Equals("true", value) : false;
            parameter.Position = pairs.TryGetValue(MarkdownStrings.Position, out value) ? value : "named";
            parameter.DefaultValue = pairs.TryGetValue(MarkdownStrings.Default_value, out value) ? value : null;
            parameter.PipelineInput = pairs.TryGetValue(MarkdownStrings.Accept_pipeline_input, out value) ? value : "false";
            parameter.Globbing = pairs.TryGetValue(MarkdownStrings.Accept_wildcard_characters, out value) ? StringComparer.OrdinalIgnoreCase.Equals("true", value) : false;
            // having Applicable for the whole parameter is a little bit sloppy: ideally it should be per yaml entry.
            // but that will make the code super ugly and it's unlikely that these two features would need to be used together.
            parameter.Applicable = pairs.TryGetValue(MarkdownStrings.Applicable, out value) ? SplitByCommaAndTrim(value) : null;
        }

        private bool ParameterRule(MamlCommand commmand)
        {
            // grammar:
            // #### -Name
            // Description              -  optional, there also could be codesnippets in the description
            //                             but no yaml codesnippets
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
            if (name.Length > 0 && name[0] == '-')
            {
                name = name.Substring(1);
            }

            MamlParameter parameter = new MamlParameter()
            {
                Name = name,
                Extent = headingNode.SourceExtent
            };

            parameter.Description = ParagraphOrCodeBlockNodeRule("yaml");
            parameter.FormatOption = headingNode.FormatOption;

            if (StringComparer.OrdinalIgnoreCase.Equals(parameter.Name, MarkdownStrings.CommonParametersToken))
            {
                // ignore text body
                commmand.SupportCommonParameters = true;
                return true;
            }

            if (StringComparer.OrdinalIgnoreCase.Equals(parameter.Name, MarkdownStrings.WorkflowParametersToken))
            {
                // ignore text body
                commmand.IsWorkflow = true;
                return true;
            }

            // we are filling up two pieces here: Syntax and Parameters
            // we are adding this parameter object to the parameters and later modifying it
            // in the rare case, when there are multiply yaml snippets,
            // the first one should be present in the resulted maml in the Parameters section
            // (all of them would be present in Syntax entry)
            var parameterSetMap = new Dictionary<string, MamlParameter>(StringComparer.OrdinalIgnoreCase);

            CodeBlockNode codeBlock;

            // fill up couple other things, even if there are no codeBlocks
            // if there are, we will fill it up inside
            parameter.ValueRequired = true;

            // First parameter is what should be used in the Parameters section
            MamlParameter firstParameter = null;
            bool isAtLeastOneYaml = false;

            while ((codeBlock = CodeBlockRule()) != null)
            {
                isAtLeastOneYaml = true;
                var yaml = ParseYamlKeyValuePairs(codeBlock);
                FillUpParameterFromKeyValuePairs(yaml, parameter);

                parameter.ValueRequired = parameter.IsSwitchParameter() ? false : true;

                // handle applicable tag
                if (parameter.IsApplicable(this._applicableTag))
                {
                    if (firstParameter == null)
                    {
                        firstParameter = parameter;
                    }

                    // handle parameter sets
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
                        parameterSetMap[ALL_PARAM_SETS_MONIKER] = parameter;
                    }
                }

                // in the rare case, when there are multiply yaml snippets
                parameter = parameter.Clone();
            }

            if (!isAtLeastOneYaml)
            {
                // if no yaml are present it's a special case and we leave it as is
                firstParameter = parameter;
            }

            // capture these two piece of information
            if (firstParameter != null)
            {
                commmand.Parameters.Add(firstParameter);
                _parameterName2ParameterSetMap.Add(Tuple.Create(name, parameterSetMap));
            }

            return true;
        }
    }
}
