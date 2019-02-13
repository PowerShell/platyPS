using Markdown.MAML.Model.MAML;
using Markdown.MAML.Parser;
using Markdown.MAML.Resources;
using Markdown.MAML.Transformer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Markdown.MAML.Model.Markdown;

namespace Markdown.MAML.Renderer
{
    /// <summary>
    /// Renders MamlModel as markdown with schema v 2.0.0
    /// </summary>
    public class MarkdownV2Renderer
    {
        private StringBuilder _stringBuilder = new StringBuilder();

        private ParserMode _mode;

        public int MaxSyntaxWidth { get; private set; }

        private const string NewLine = "\r\n";

        /// <summary>
        /// 110 is a good width value, because it doesn't cause github to add horizontal scroll bar
        /// </summary>
        public const int DEFAULT_SYNTAX_WIDTH = 110;

        public MarkdownV2Renderer(ParserMode mode) : this(mode, DEFAULT_SYNTAX_WIDTH) { }

        public MarkdownV2Renderer(ParserMode mode, int maxSyntaxWidth)
        {
            this.MaxSyntaxWidth = maxSyntaxWidth;
            this._mode = mode;
        }

        public string MamlModelToString(MamlCommand mamlCommand, bool skipYamlHeader)
        {
            return MamlModelToString(mamlCommand, null, skipYamlHeader);
        }

        public string MamlModelToString(MamlCommand mamlCommand, Hashtable yamlHeader)
        {
            return MamlModelToString(mamlCommand, yamlHeader, false);
        }

        private string MamlModelToString(MamlCommand mamlCommand, Hashtable yamlHeader, bool skipYamlHeader)
        {
            // clear, so we can re-use this instance
            _stringBuilder.Clear();
            if (!skipYamlHeader)
            {
                if (yamlHeader == null)
                {
                    yamlHeader = new Hashtable();
                }

                // put version there
                yamlHeader["schema"] = "2.0.0";
                AddYamlHeader(yamlHeader);
            }

            AddCommand(mamlCommand);

            // at the end, just normalize all ends
            return RenderCleaner.NormalizeLineBreaks(
                RenderCleaner.NormalizeWhitespaces(
                    RenderCleaner.NormalizeQuotesAndDashes(
                        _stringBuilder.ToString())));
        }
        
        private void AddYamlHeader(Hashtable yamlHeader)
        {
            _stringBuilder.AppendFormat("---{0}", NewLine);
            
            // Use a sorted dictionary to force the metadata into alphabetical order by key for consistency.
            var sortedHeader = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (DictionaryEntry pair in yamlHeader)
            {
                sortedHeader[pair.Key.ToString()] = pair.Value == null ? "" : pair.Value.ToString();
            }
            
            foreach (var pair in sortedHeader)
            {
                AppendYamlKeyValue(pair.Key, pair.Value);
            }

            _stringBuilder.AppendFormat("---{0}{0}", NewLine);
        }

        private void AddCommand(MamlCommand command)
        {
            AddHeader(ModelTransformerBase.COMMAND_NAME_HEADING_LEVEL, command.Name);
            AddEntryHeaderWithText(MarkdownStrings.SYNOPSIS, command.Synopsis);
            AddSyntax(command);
            AddEntryHeaderWithText(MarkdownStrings.DESCRIPTION, command.Description);
            AddExamples(command);
            AddParameters(command);
            AddInputs(command);
            AddOutputs(command);
            AddEntryHeaderWithText(MarkdownStrings.NOTES, command.Notes);
            AddLinks(command);
        }

        private void AddLinks(MamlCommand command)
        {
            var extraNewLine = command.Links != null && command.Links.Count > 0;

            AddHeader(ModelTransformerBase.COMMAND_ENTRIES_HEADING_LEVEL, MarkdownStrings.RELATED_LINKS, extraNewLine);
            foreach (var link in command.Links)
            {
                if (link.IsSimplifiedTextLink)
                {
                    _stringBuilder.AppendFormat("{0}", link.LinkName);
                }
                else
                {
                    var name = link.LinkName;
                    if (string.IsNullOrEmpty(name))
                    {
                        // we need a valid name to produce a valid markdown
                        name = link.LinkUri;
                    }

                    _stringBuilder.AppendFormat("[{0}]({1}){2}{2}", name, link.LinkUri, NewLine);
                }
            }
        }

        private void AddInputOutput(MamlInputOutput io)
        {
            if (string.IsNullOrEmpty(io.TypeName) && string.IsNullOrEmpty(io.Description))
            {
                // in this case ignore
                return;
            }

            var extraNewLine = ShouldBreak(io.FormatOption);
            AddHeader(ModelTransformerBase.INPUT_OUTPUT_TYPENAME_HEADING_LEVEL, io.TypeName, extraNewLine);
            AddParagraphs(io.Description);
        }

        private void AddOutputs(MamlCommand command)
        {
            AddHeader(ModelTransformerBase.COMMAND_ENTRIES_HEADING_LEVEL, MarkdownStrings.OUTPUTS);
            foreach (var io in command.Outputs)
            {
                AddInputOutput(io);
            }
        }

        private void AddInputs(MamlCommand command)
        {
            AddHeader(ModelTransformerBase.COMMAND_ENTRIES_HEADING_LEVEL, MarkdownStrings.INPUTS);
            foreach (var io in command.Inputs)
            {
                AddInputOutput(io);
            }
        }

        private void AddParameters(MamlCommand command)
        {
            AddHeader(ModelTransformerBase.COMMAND_ENTRIES_HEADING_LEVEL, MarkdownStrings.PARAMETERS);
            foreach (var param in command.Parameters)
            {
                AddParameter(param, command);
            }

            if (command.IsWorkflow)
            {
                AddWorkflowParameters();
            }

            // Workflows always support CommonParameters
            if (command.SupportCommonParameters || command.IsWorkflow)
            {
                AddCommonParameters();
            }
        }

        private void AddCommonParameters()
        {
            AddHeader(ModelTransformerBase.PARAMETERSET_NAME_HEADING_LEVEL, MarkdownStrings.CommonParametersToken, extraNewLine: false);
            AddParagraphs(MarkdownStrings.CommonParametersText, noNewLines: false, skipAutoWrap: true);
        }

        private void AddWorkflowParameters()
        {
            AddHeader(ModelTransformerBase.PARAMETERSET_NAME_HEADING_LEVEL, MarkdownStrings.WorkflowParametersToken, extraNewLine: false);
            AddParagraphs(MarkdownStrings.WorkflowParametersText, noNewLines: false, skipAutoWrap: true);
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
                        if (string.IsNullOrEmpty(syntax.ParameterSetName))
                        {
                            // Note (vors) : I guess that means it's applicable to all parameter sets,
                            // but it's hard to tell anymore...
                            result[ModelTransformerVersion2.ALL_PARAM_SETS_MONIKER] = param;
                        }
                        else
                        {
                            result[syntax.ParameterSetName] = param;
                        }
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

        private string JoinWithComma(IEnumerable<string> args)
        {
            if (args == null)
            {
                return "";
            }

            return string.Join(", ", args);
        }

        private bool ShouldBreak(SectionFormatOption formatOption)
        {
            return formatOption.HasFlag(SectionFormatOption.LineBreakAfterHeader);
        }

        private void AddParameter(MamlParameter parameter, MamlCommand command)
        {
            var extraNewLine = ShouldBreak(parameter.FormatOption);

            AddHeader(ModelTransformerBase.PARAMETERSET_NAME_HEADING_LEVEL, '-' + parameter.Name, extraNewLine: extraNewLine);

            AddParagraphs(parameter.Description);
            
            var sets = SimplifyParamSets(GetParamSetDictionary(parameter.Name, command.Syntax));
            foreach (var set in sets)
            {
                _stringBuilder.AppendFormat("```yaml{0}", NewLine);

                AppendYamlKeyValue(MarkdownStrings.Type, parameter.Type);

                string parameterSetsString;
                if (command.Syntax.Count == 1 || set.Item1.Count == command.Syntax.Count)
                {
                    // ignore, if there is just one parameter set
                    // or this parameter belongs to All parameter sets, use (All)
                    parameterSetsString = ModelTransformerVersion2.ALL_PARAM_SETS_MONIKER;
                }
                else
                {
                    parameterSetsString = JoinWithComma(set.Item1);
                }

                AppendYamlKeyValue(MarkdownStrings.Parameter_Sets, parameterSetsString);

                AppendYamlKeyValue(MarkdownStrings.Aliases, JoinWithComma(parameter.Aliases));
                if (parameter.ParameterValueGroup.Count > 0)
                {
                    AppendYamlKeyValue(MarkdownStrings.Accepted_values, JoinWithComma(parameter.ParameterValueGroup));
                }

                if (parameter.Applicable != null)
                {
                    AppendYamlKeyValue(MarkdownStrings.Applicable, JoinWithComma(parameter.Applicable));
                }

                _stringBuilder.AppendLine();

                AppendYamlKeyValue(MarkdownStrings.Required, set.Item2.Required.ToString());
                AppendYamlKeyValue(MarkdownStrings.Position, set.Item2.IsNamed() ? "Named" : set.Item2.Position);
                AppendYamlKeyValue(MarkdownStrings.Default_value, string.IsNullOrWhiteSpace(parameter.DefaultValue) ? "None" : parameter.DefaultValue);
                AppendYamlKeyValue(MarkdownStrings.Accept_pipeline_input, parameter.PipelineInput);
                AppendYamlKeyValue(MarkdownStrings.Accept_wildcard_characters, parameter.Globbing.ToString());

                _stringBuilder.AppendFormat("```{0}{0}", NewLine);
            }
        }

        private void AppendYamlKeyValue(string key, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                _stringBuilder.AppendFormat("{0}:{1}", key, NewLine);

                return;
            }

            _stringBuilder.AppendFormat("{0}: {1}{2}", key, value, NewLine);
        }

        private void AddExamples(MamlCommand command)
        {
            AddHeader(ModelTransformerBase.COMMAND_ENTRIES_HEADING_LEVEL, MarkdownStrings.EXAMPLES);
            foreach (var example in command.Examples)
            {
                var extraNewLine = ShouldBreak(example.FormatOption);

                AddHeader(ModelTransformerBase.EXAMPLE_HEADING_LEVEL, GetExampleTitle(example.Title), extraNewLine: extraNewLine);

                if (!string.IsNullOrEmpty(example.Introduction))
                {
                    AddParagraphs(example.Introduction);
                }

                if (example.Code != null)
                {
                    for (var i = 0; i < example.Code.Length; i++)
                    {
                        AddCodeSnippet(example.Code[i].Text, example.Code[i].LanguageMoniker);
                    }
                }

                if (!string.IsNullOrEmpty(example.Remarks))
                {
                    AddParagraphs(example.Remarks);
                }
            }
        }

        private static string GetExampleTitle(string title)
        {
            var match = Regex.Match(title, @"^(-| ){0,}(?<title>([^\f\n\r\t\v\x85\p{Z}-][^\f\n\r\t\v\x85]+[^\f\n\r\t\v\x85\p{Z}-]))(-| ){0,}$");
            
            if (match.Success)
            {
                return match.Groups["title"].Value;
            }

            return title;
        }

        public static string GetSyntaxString(MamlCommand command, MamlSyntax syntax)
        {
            return GetSyntaxString(command, syntax, DEFAULT_SYNTAX_WIDTH);
        }

        public static string GetSyntaxString(MamlCommand command, MamlSyntax syntax, int maxSyntaxWidth)
        {
            // TODO: we may want to add ParameterValueGroup info here,
            // but it's fine for now

            var sb = new StringBuilder();
            sb.Append(command.Name);

            var paramStrings = new List<string>();
                        
            // first we create list of param string we want to add
            foreach (var param in syntax.Parameters)
            {
                string paramStr;
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
                paramStrings.Add(paramStr);   
            }

            if (command.IsWorkflow)
            {
                paramStrings.Add("[<" + MarkdownStrings.WorkflowParametersToken + ">]");
            }

            if (command.SupportCommonParameters)
            {
                paramStrings.Add("[<" + MarkdownStrings.CommonParametersToken + ">]");
            }

            // then we format them properly with repsect to max width for window.
            int widthBeforeLastBreak = 0;
            foreach (string paramStr in paramStrings) { 

                if (sb.Length - widthBeforeLastBreak + paramStr.Length > maxSyntaxWidth)
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
            AddHeader(ModelTransformerBase.COMMAND_ENTRIES_HEADING_LEVEL, MarkdownStrings.SYNTAX);
            foreach (var syntax in command.Syntax)
            {
                if (command.Syntax.Count > 1)
                {
                    AddHeader(ModelTransformerBase.PARAMETERSET_NAME_HEADING_LEVEL, string.Format("{0}{1}",syntax.ParameterSetName,syntax.IsDefault ? MarkdownStrings.DefaultParameterSetModifier : null), extraNewLine: false);
                }

                AddCodeSnippet(GetSyntaxString(command, syntax));
            }
        }

        private void AddEntryHeaderWithText(string header, SectionBody body)
        {
            var extraNewLine = body == null || string.IsNullOrEmpty(body.Text) || ShouldBreak(body.FormatOption);

            // Add header
            AddHeader(ModelTransformerBase.COMMAND_ENTRIES_HEADING_LEVEL, header, extraNewLine: extraNewLine);

            // to correctly handle empty text case, we are adding new-line here
            if (body != null && !string.IsNullOrEmpty(body.Text))
            {
                AddParagraphs(body.Text);
            }
        }

        private void AddCodeSnippet(string code, string lang = "")
        {
            _stringBuilder.AppendFormat("```{1}{2}{0}{2}```{2}{2}", code, lang, NewLine);
        }

        private void AddHeader(int level, string header, bool extraNewLine = true)
        {
            for (int i = 0; i < level; i++)
            {
                _stringBuilder.Append('#');
            }
            _stringBuilder.Append(' ');
            _stringBuilder.AppendFormat("{0}{1}", header, NewLine);
            if (extraNewLine)
            {
                _stringBuilder.Append(NewLine);
            }
        }

        private string GetAutoWrappingForNonListLine(string line)
        {
            return Regex.Replace(line, @"([\.\!\?]) ( )*([^\r\n])", "$1$2\r\n$3");
        }

        private string GetAutoWrappingForMarkdown(string[] lines)
        {
            // this is an implementation of https://github.com/PowerShell/platyPS/issues/93
            
            // algorithm: identify chunks that represent lists
            // Every entry in a list should be preserved as is and 1 EOL between them
            // Every entry not in a list should be split with GetAutoWrappingForNonListLine
            // delimiters between lists and non-lists are 2 EOLs.

            var newLines = new List<string>();

            for (int i = 0; i < lines.Length; i++)
            {
                if (MarkdownParser.HasListPrefix(lines[i]))
                {
                    if (i > 0 && !MarkdownParser.HasListPrefix(lines[i - 1]))
                    {
                        // we are in a list and it just started
                        newLines.Add(NewLine + lines[i]);
                    }
                    else
                    {
                        newLines.Add(lines[i]);
                    }
                }
                else
                {
                    if (i > 0)
                    {
                        // we are just finished a list
                        newLines.Add(NewLine + GetAutoWrappingForNonListLine(lines[i]));
                    }
                    else
                    {
                        newLines.Add(GetAutoWrappingForNonListLine(lines[i]));
                    }
                }
            }

            return string.Join(NewLine, newLines);
        }

        private void AddParagraphs(string body, bool noNewLines = false, bool skipAutoWrap = false)
        {
            if (string.IsNullOrWhiteSpace(body))
            {
                return;
            }

            if (this._mode != ParserMode.FormattingPreserve && skipAutoWrap != true)
            {
                string[] paragraphs = body.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                body = GetAutoWrappingForMarkdown(paragraphs.Select(para => GetEscapedMarkdownText(para.Trim())).ToArray());
            }

            // The the body already ended in a line break don't add extra lines on to the end
            if (body.EndsWith("\r\n\r\n"))
            {
                noNewLines = true;
            }

            _stringBuilder.AppendFormat("{0}{1}{1}", body, noNewLines ? null : NewLine);
        }

        private static string BackSlashMatchEvaluater(Match match)
        {
            // '\<' -> '\\<'
            // '\\<' -> '\\<' - noop
            // '\\\<' -> '\\\\<'

            var g1 = match.Groups[1].Value;
            var g2 = match.Groups[2].Value[0];

            if (g1.Length % 2 == 0 && "<>()[]`".Contains(g2))
            {
                return @"\" + match.Value;
            }

            return match.Value;
        }

        // public just to make testing easier
        public static string GetEscapedMarkdownText(string text)
        {
            // this is kind of a crazy replacement to handle escaping properly.
            // we need to do the reverse operation in our markdown parser.

            // PS code: (((($text - replace '\\\\','\\\\') -replace '([<>])','\$1') -replace '\\([\[\]\(\)])', '\\$1')

            // examples: 
            // '\<' -> '\\\<'
            // '\' -> '\'
            // '\\' -> '\\\\'
            // '<' -> '\<'

            text = text
                .Replace(@"\\", @"\\\\");

            text = Regex.Replace(text, @"(\\*)\\(.)", new MatchEvaluator(BackSlashMatchEvaluater));

            return text
                .Replace(@"<", @"\<")
                .Replace(@">", @"\>")

                .Replace(@"[", @"\[")
                .Replace(@"]", @"\]")
                // per https://github.com/PowerShell/platyPS/issues/121 we don't perform escaping for () in markdown renderer, but we do in the parser
                //.Replace(@"(", @"\(")
                //.Replace(@")", @"\)")
                .Replace(@"`", @"\`");
        }
    }
}
