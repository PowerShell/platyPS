// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.PowerShell.PlatyPS.MarkdownWriter;
using Microsoft.PowerShell.PlatyPS.Model;
using System.Collections;

namespace Microsoft.PowerShell.PlatyPS
{
    /// <summary>
    /// A class to represent a parse error.
    /// This might occur at any time, but especially when invalid yaml is included in the markdown.
    /// </summary>
    public class ParseError
    {
        /// <summary>
        /// The context of the error.
        /// This might be a parameter name or some other context.
        /// </summary>
        public string Context { get; set; }
        /// <summary>
        /// The message of the error.
        /// If an exception is caught, this will be the exception message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The line number which generated the error.
        /// If unknown, this will be -1.
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// An error that may occur during parsing.
        /// </summary>
        /// <param name="context">The context of error.</param>
        /// <param name="message">The message which describes the error.</param>
        public ParseError(string context, string message, int line = -1)
        {
            Context = context;
            Message = message;
            Line = line;
        }
    }

    public partial class MarkdownConverter
    {
        /// <summary>
        /// Create a CommandHelp object from a markdown file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>A CommandHelp Object, but declared as object as the CommandHelp object is internal.</returns>
		public static CommandHelp GetCommandHelpFromMarkdownFile(string path)
		{
            var md = ParsedMarkdownContent.ParseFile(path);
			var commandHelp = GetCommandHelpFromMarkdown(md);
            commandHelp.Diagnostics.FileName = path;
            return commandHelp;
		}

        /// <summary>
        /// Validate a markdown file against the schema.
        /// </summary>
        /// <param name="path">The path of the markdown file.</param>
        /// <param name="Issues">The list of findings found during validation. This will include passes and failures.</param>
        /// <returns>boolean</returns>
        public static bool ValidateMarkdownFile(string path, out List<string>Issues)
        {
            var md = ParsedMarkdownContent.ParseFile(path);
            return ValidateMarkdown(md.Ast, out Issues);
        }

        /// <summary>
        /// Captures whether there were errors found during the parsing.
        /// </summary>
        public static bool HadErrors { get; private set; } = false;

        /// <summary>
        /// The collection of parsing errors.
        /// </summary>
        public static List<ParseError> ParseErrors { get; private set; } = new List<ParseError>();

        /// <summary>
        /// Add an error to the collection.
        /// </summary>
        /// <param name="context">Information about the context where the error occurred.</param>
        /// <param name="message">A message to further elaborate on the error.</param>
        /// <param name="line">The line which caused the parsing error.</param>
        public static void AddParseError(string context, string message, int line = -1)
        {
            ParseErrors.Add(new ParseError(context, message, line));
            HadErrors = true;
        }

        /// <summary>
        /// Clear the error collection.
        /// </summary>
        public static void ClearParseErrors()
        {
            ParseErrors.Clear();
            HadErrors = false;
        }

        /// <summary>
        /// Create a CommandHelp markdown file from an existing markdown file.
        /// </summary>
        /// <param name="path">The path to the source markdown.</param>
        /// <param name="destinationPath">The path to the target markdown file.</param>
        public static void ExportMarkdown(string path, string destinationPath)
        {
            CommandHelp commandHelp = (CommandHelp)GetCommandHelpFromMarkdownFile(path);
            var encoding = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            var settings = new WriterSettings(encoding, destinationPath);
            var cmdWrt = new CommandHelpMarkdownWriter(settings);
            cmdWrt.Write(commandHelp, null);
        }

        // This should be a single token
        internal static string GetCommandNameFromMarkdown(ParsedMarkdownContent mc)
        {
            var idx = mc.FindHeader(1, string.Empty);
            if(mc.Ast[idx] is HeadingBlock title)
            {
                var commandLineString = title?.Inline?.FirstChild?.ToString().Trim();
                if (commandLineString is not null)
                {
                    if (commandLineString.IndexOf(' ') == -1)
                    {
                        return commandLineString;
                    }
                    else
                    {
                        return commandLineString.Substring(0, commandLineString.IndexOf(' '));
                    }
                }
            }
            return string.Empty;
        }

        // There are some fields which are required, we will check those here
        // and create diagnostic messages for them
        internal static void ValidateMetadata(OrderedDictionary metadata, out List<DiagnosticMessage> diagnostics)
        {
            diagnostics = new List<DiagnosticMessage>();
            string[]requiredKeys = new string[] {
                "external help file",
                "Locale",
                "Module Name",
                "ms.date",
                "HelpUri",
                "PlatyPS schema version",
                "title"
            };

            foreach(var key in requiredKeys)
            {
                if (metadata.Contains(key))
                {
                    // we don't have line information here.
                    diagnostics.Add(
                        new DiagnosticMessage(DiagnosticMessageSource.Metadata, "Metadata", DiagnosticSeverity.Information, $"found '{key}'", 1)
                    );
                }
                else
                {
                    diagnostics.Add(
                        new DiagnosticMessage(DiagnosticMessageSource.Metadata, "Metadata", DiagnosticSeverity.Error, $"not found '{key}'", 1)
                    );
                }
            }
        }

        internal static CommandHelp GetCommandHelpFromMarkdown(ParsedMarkdownContent markdownContent)
        {
            /*
            GetMetadata
            GetTitleFromMarkdown
            GetSynopsisFromMarkdown // Not parsed
            GetSyntaxFromMarkdown // Must be parsed
            GetAliasesFromMarkdown // Not parsed, may not be present
            GetDescriptionFromMarkdown // Not parsed
            GetExamplesFromMarkdown // Must be parsed
            GetParametersFromMarkdown // Must be parsed
            GetInputsFromMarkdown // level 3 markdown header is an input type
            GetOutputsFromMarkdown // level 3 markdown header is an output type
            GetNotesFromMarkdown // Not parsed, may not be present
            GetRelatedLinksFromMarkdown
            */

            CommandHelp commandHelp;

            OrderedDictionary? metadata = GetMetadata(markdownContent);
            if (metadata is null)
            {
                throw new InvalidDataException($"No metadata found in {markdownContent.FilePath}");
            }
            else
            {
                metadata = MetadataUtils.FixUpCommandHelpMetadata(metadata);
            }

            var commandName = GetCommandNameFromMarkdown(markdownContent);
            if (commandName == string.Empty)
            {
                throw new InvalidDataException("commandName not found. markdown structure not according to schema.");
            }

			string moduleName = metadata["Module Name"] as string ?? string.Empty;

            commandHelp = new CommandHelp(commandName, moduleName, cultureInfo: null);
            ValidateMetadata(metadata, out var metadataDiagnostics);
            if (metadataDiagnostics is not null)
            {
                metadataDiagnostics.ForEach(d => commandHelp.Diagnostics.TryAddDiagnostic(d));
            }

            commandHelp.Metadata = metadata;
            commandHelp.HasCmdletBinding = GetCmdletBindingState(markdownContent, out var cmdletBindingDiagnostics);
            if (cmdletBindingDiagnostics is not null)
            {
                commandHelp.Diagnostics.Messages.AddRange(cmdletBindingDiagnostics);
            }

            commandHelp.HasWorkflowCommonParameters = GetWorkflowCommonParameterState(markdownContent, out var workflowDiagnostics);
            if (workflowDiagnostics is not null)
            {
                commandHelp.Diagnostics.Messages.AddRange(workflowDiagnostics);
            }

            List<DiagnosticMessage> synopsisDiagnostics = new();
            commandHelp.Synopsis = GetSynopsisFromMarkdown(markdownContent, out synopsisDiagnostics);
            if (synopsisDiagnostics is not null && synopsisDiagnostics.Count > 0)
            {
                synopsisDiagnostics.ForEach(d => commandHelp.Diagnostics.TryAddDiagnostic(d));
            }

            List<DiagnosticMessage> syntaxDiagnostics = new();
            commandHelp.AddSyntaxItemRange(GetSyntaxFromMarkdown(markdownContent, out syntaxDiagnostics));
            if (syntaxDiagnostics is not null && syntaxDiagnostics.Count > 0)
            {
                syntaxDiagnostics.ForEach(d => commandHelp.Diagnostics.TryAddDiagnostic(d));
            }

            List<DiagnosticMessage> aliasesDiagnostics = new();
            bool aliasHeaderFound = false;
            commandHelp.Aliases = GetAliasesFromMarkdown(markdownContent, out aliasesDiagnostics, out aliasHeaderFound);
            commandHelp.AliasHeaderFound = aliasHeaderFound;
            if (aliasesDiagnostics is not null && aliasesDiagnostics.Count > 0)
            {
                aliasesDiagnostics.ForEach(d => commandHelp.Diagnostics.TryAddDiagnostic(d));
            }

            List<DiagnosticMessage> descriptionDiagnostics = new();
            commandHelp.Description = GetDescriptionFromMarkdown(markdownContent, out descriptionDiagnostics).Trim();
            if (descriptionDiagnostics is not null && descriptionDiagnostics.Count > 0)
            {
                descriptionDiagnostics.ForEach(d => commandHelp.Diagnostics.TryAddDiagnostic(d));
            }

            List<DiagnosticMessage> examplesDiagnostics = new();
            commandHelp.Examples?.AddRange(GetExamplesFromMarkdown(markdownContent, out examplesDiagnostics));
            if (examplesDiagnostics is not null && examplesDiagnostics.Count > 0)
            {
                examplesDiagnostics.ForEach(d => commandHelp.Diagnostics.TryAddDiagnostic(d));
            }

            List<DiagnosticMessage> parameterDiagnostics = new();
            foreach(var parameter in GetParametersFromMarkdown(markdownContent, out parameterDiagnostics))
            {
                commandHelp.AddParameter(parameter);
            }
            commandHelp.Parameters.Sort((x,y) => x.Name.CompareTo(y.Name));

            if (parameterDiagnostics is not null && parameterDiagnostics.Count > 0)
            {
                parameterDiagnostics.ForEach(d => commandHelp.Diagnostics.TryAddDiagnostic(d));
            }

            List<DiagnosticMessage> inputsDiagnostics = new();
            var commandInputs = GetInputsFromMarkdown(markdownContent, out inputsDiagnostics);
            if (commandInputs is not null && commandInputs.Count > 0)
            {
                commandHelp.Inputs?.AddRange(commandInputs);
            }

            if (inputsDiagnostics is not null && inputsDiagnostics.Count > 0)
            {
                inputsDiagnostics.ForEach(d => commandHelp.Diagnostics.TryAddDiagnostic(d));
            }

            List<DiagnosticMessage> outputsDiagnostics = new();
            var commandOutputs = GetOutputsFromMarkdown(markdownContent, out outputsDiagnostics);
            if (commandOutputs is not null && commandOutputs.Count > 0)
            {
                commandHelp.Outputs?.AddRange(commandOutputs);
            }

            if (outputsDiagnostics is not null && outputsDiagnostics.Count > 0)
            {
                outputsDiagnostics.ForEach(d => commandHelp.Diagnostics.TryAddDiagnostic(d));
            }

            List<DiagnosticMessage> notesDiagnostics = new();
            commandHelp.Notes = GetNotesFromMarkdown(markdownContent, out notesDiagnostics);
            if (notesDiagnostics is not null && notesDiagnostics.Count > 0)
            {
                notesDiagnostics.ForEach(d => commandHelp.Diagnostics.TryAddDiagnostic(d));
            }

            List<DiagnosticMessage> linksDiagnostics = new();
            commandHelp.RelatedLinks?.AddRange(GetRelatedLinksFromMarkdown(markdownContent, out linksDiagnostics));
            if (linksDiagnostics is not null && linksDiagnostics.Count > 0)
            {
                linksDiagnostics.ForEach(d => commandHelp.Diagnostics.TryAddDiagnostic(d));
            }

            return commandHelp;

        }

        internal static OrderedDictionary? GetMetadata(ParsedMarkdownContent md)
        {
            var ast = md.Ast;
            // The metadata must be the first block in the markdown
            if (ast.Count < 2)
            {
                return null;
            }

            if (ast[0] is Markdig.Syntax.ThematicBreakBlock)
            {
                if (ast[1] is Markdig.Syntax.HeadingBlock metadata)
                {
                    if (metadata.Inline?.FirstChild is Markdig.Syntax.Inlines.LiteralInline metadataText)
                    {
                        return ConvertTextToOrderedDictionary(metadataText.Content.Text);
                    }
                }
                else if (ast[2] is Markdig.Syntax.ThematicBreakBlock && ast[1] is ParagraphBlock paragraph)
                {
                    if (paragraph.Inline?.FirstChild is LiteralInline metadataAsParagraph)
                    {
                        return ConvertTextToOrderedDictionary(metadataAsParagraph.Content.Text);
                    }
                }
                else if (ast[2] is Markdig.Syntax.ListBlock && ast[1] is ParagraphBlock paragraphWithList)
                {
                    if (paragraphWithList.Inline?.FirstChild is LiteralInline metadataAsParagraph)
                    {
                        var metadataDictionary = ConvertTextToOrderedDictionary(metadataAsParagraph.Content.Text);

                        StringBuilder? sb = null;

                        try
                        {
                            sb = Constants.StringBuilderPool.Get();
                            int startIndex = ast[2].Line - 1; // -1 because we are 0 based
                            int endIndex = ast[3].Line - 1; // -1 because we are 0 based
                            for (int i = startIndex; i < endIndex; i++)
                            {
                                sb.AppendLine(md.MarkdownLines[i].TrimEnd());
                            }
                            string blockContent = sb.ToString().Replace("\r", "").Trim();

                            foreach(DictionaryEntry kv in ConvertTextToOrderedDictionary(blockContent))
                            {
                                metadataDictionary[kv.Key] = kv.Value;
                            }
                        }
                        finally
                        {
                            if (sb is not null)
                            {
                                Constants.StringBuilderPool.Return(sb);
                            }
                        }

                        return metadataDictionary;
                    }
                }
            }

            return null;
        }

        // convert the string we read to an ordered dictionary
        internal static OrderedDictionary ConvertTextToOrderedDictionary(string text)
        {
            // Try to get the metadata via the yaml deserializer
            if (YamlUtils.TryGetOrderedDictionaryFromText(text, out OrderedDictionary md))
            {
                return md;
            }


            OrderedDictionary od = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
			char[] fieldSeparator = { ':' };
            foreach(string s in text.Split(Constants.LineSplitter))
            {
                string[] kv = s.Split(fieldSeparator, 2, StringSplitOptions.None);
                if (kv.Length == 2)
                {
                    var value = kv[1].Trim();
                    if (value == "''") // If we have an empty space, don't save empty quotes
                    {
                        value = string.Empty;
                    }

                    od[kv[0].Trim()] = value;
                }
            }

            return od;
        }

        /// <summary>
        /// Retrieve the related link information from the markdown
        /// </summary>
        /// <param name="markdownContent">The parsed markdown data.</param>
        /// <returns>List<Links></returns>
        internal static List<Links> GetRelatedLinksFromMarkdown(ParsedMarkdownContent markdownContent, out List<DiagnosticMessage> diagnostics)
        {
            diagnostics = new List<DiagnosticMessage>();
            markdownContent.Reset();
            var start = markdownContent.FindHeader(2, "RELATED LINKS");
            if (start == -1 || markdownContent.Ast.Count <= start + 1)
            {
                var dm0 = new DiagnosticMessage(DiagnosticMessageSource.Links, "no links found", DiagnosticSeverity.Warning, "GetRelatedLinks", markdownContent.GetTextLine(start));
                diagnostics.Add(dm0);
                return new List<Links>();
            }

            var dm1 = new DiagnosticMessage(DiagnosticMessageSource.Links, "Links found", DiagnosticSeverity.Information, "GetRelatedLinks", markdownContent.GetTextLine(start));
            diagnostics.Add(dm1);
            markdownContent.Seek(start);
            var links = GetLinks(markdownContent, out var linkDiagnostics);
            diagnostics.AddRange(linkDiagnostics);
            return links;
        }

        internal static string GetNotesFromMarkdown(ParsedMarkdownContent markdownContent, out List<DiagnosticMessage> diagnostics)
        {
            diagnostics = new List<DiagnosticMessage>();
            markdownContent.Reset();
            var start = markdownContent.FindHeader(2, "NOTES");
            if (start == -1)
            {
                var dm0 = new DiagnosticMessage(DiagnosticMessageSource.Notes, "Notes header not found", DiagnosticSeverity.Error, "GetNotes", markdownContent.GetTextLine(start));
                diagnostics.Add(dm0);
                return string.Empty;
            }

            markdownContent.Seek(start);
            var end = markdownContent.FindHeader(2, "RELATED LINKS");
            // Notes may be blank, which means the NOTES header is followed by RELATED LINKS header
            if (end - start == 1)
            {
                var dm1 = new DiagnosticMessage(DiagnosticMessageSource.Notes, "Notes content not found", DiagnosticSeverity.Warning, "GetNotes", markdownContent.GetTextLine(start));
                diagnostics.Add(dm1);
                return string.Empty;
            }

            markdownContent.Take();
            var noteContent = markdownContent.GetStringFromAst(end);
            var dm2 = new DiagnosticMessage(DiagnosticMessageSource.Notes, "Notes content found", DiagnosticSeverity.Information, $"Notes length = {noteContent.Length}", markdownContent.GetTextLine(start));
            diagnostics.Add(dm2);
            return noteContent;
        }

        internal static List<InputOutput> GetInputsFromMarkdown(ParsedMarkdownContent markdownContent, out List<DiagnosticMessage> diagnostics)
        {
            diagnostics = new List<DiagnosticMessage>();
            List<InputOutput> input = new();
            markdownContent.Reset();
            var start = markdownContent.FindHeader(2, "INPUTS");
            if (start == -1)
            {
                diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.Inputs, "GetInput", DiagnosticSeverity.Error, "INPUTS header not found", start));
            }
            else
            {
                markdownContent.Seek(start);
                input.AddRange(GetInputOutput(markdownContent));
                diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.Inputs, "GetInput", DiagnosticSeverity.Information, $"{input.Count} items found", start));
            }

            return input;
        }

        internal static List<InputOutput> GetOutputsFromMarkdown(ParsedMarkdownContent markdownContent, out List<DiagnosticMessage> diagnostics)
        {
            List<InputOutput> output = new();
            diagnostics = new List<DiagnosticMessage>();
            markdownContent.Reset();
            var start = markdownContent.FindHeader(2, "OUTPUTS");
            if (start == -1)
            {
                diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.Inputs, "GetOutput", DiagnosticSeverity.Error, "OUTPUTS header not found", start));
            }
            else
            {
                markdownContent.Seek(start);
                output.AddRange(GetInputOutput(markdownContent));
                diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.Outputs, "GetOutput", DiagnosticSeverity.Information, $"{output.Count} items found", start));
            }

            return output;
        }

        internal static Collection<Parameter> GetParametersFromMarkdown(ParsedMarkdownContent markdownContent, out List<DiagnosticMessage> diagnostics)
        {
            diagnostics = new List<DiagnosticMessage>();
            markdownContent.Reset();
            var start = markdownContent.FindHeader(2, "PARAMETERS");
            if (start == -1)
            {
                diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.Parameter, "GetParameters", DiagnosticSeverity.Error, "PARAMETER header not found", start));
                return new Collection<Parameter>();
            }

            var parameters = GetParameters(markdownContent, start + 1, out List<DiagnosticMessage> parameterDiagnostics);
            var dm = new DiagnosticMessage(DiagnosticMessageSource.Parameter, "Parameters", DiagnosticSeverity.Information, $"{parameters.Count} parameters found", start);
            diagnostics.Add(dm);
            diagnostics.AddRange(parameterDiagnostics);
            return parameters;
        }

        internal static Collection<Example> GetExamplesFromMarkdown(ParsedMarkdownContent markdownContent, out List<DiagnosticMessage> diagnostics)
        {
            diagnostics = new List<DiagnosticMessage>();
            markdownContent.Reset();
            var start = markdownContent.FindHeader(2, "EXAMPLES");
            if (start == -1)
            {
                diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.Example, "GetExamples", DiagnosticSeverity.Error, "EXAMPLES header not found", start));
                return new Collection<Example>();
            }

            var examples = GetExamples(markdownContent, start + 1);
            var dm = new DiagnosticMessage(DiagnosticMessageSource.Example, "EXAMPLES header found", DiagnosticSeverity.Information, $"{examples.Count} examples found", markdownContent.GetTextLine(start));
            diagnostics.Add(dm);
            return examples;
        }

        internal static string GetDescriptionFromMarkdown(ParsedMarkdownContent markdownContent, out List<DiagnosticMessage> diagnostics)
        {
            diagnostics = new List<DiagnosticMessage>();
            markdownContent.Reset();
            var start = markdownContent.FindHeader(2, "DESCRIPTION");
            if (start == -1)
            {
                diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.Description, "DESCRIPTION header not found", DiagnosticSeverity.Error, "DESCRIPTION", -1));
                return string.Empty;
            }

            diagnostics.Add(new DiagnosticMessage( DiagnosticMessageSource.Description, "DESCRIPTION header found", DiagnosticSeverity.Information, "DESCRIPTION", markdownContent.GetTextLine(start)));
            markdownContent.Seek(start);
            markdownContent.Take();
            var end = markdownContent.FindHeader(2, "EXAMPLES");
            return markdownContent.GetStringFromAst(end).Trim();
        }

        internal static List<SyntaxItem> GetSyntaxFromMarkdown(ParsedMarkdownContent markdownContent, out List<DiagnosticMessage> diagnostics)
        {
            diagnostics = new List<DiagnosticMessage>();
            var start = markdownContent.FindHeader(2, "SYNTAX");
            if (start == -1)
            {
                diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.Syntax, "SYNTAX header not found", DiagnosticSeverity.Error, "missing syntax", -1));
                return new List<SyntaxItem>();
            }

            markdownContent.Seek(start);
            var end   = markdownContent.FindHeader(2, string.Empty);
            var syntax = new List<SyntaxItem>();

            // Some markdown does not have a SYNTAX with a parameter set, so we should synthesize one
            // We will also mark this as having cmdlet binding.
            var subHeaderOffset = markdownContent.FindHeader(3, string.Empty);
            if (subHeaderOffset > end)
            {
                if (markdownContent.Peek() is FencedCodeBlock fcb)
                {
                    var rawSyntax = fcb.Lines.ToString();
                    var si = CreateSyntaxFromText(rawSyntax, "Default", true);
                    syntax.Add(si);
                }

                diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.Syntax, "SYNTAX header found", DiagnosticSeverity.Information, "SYNTAX", markdownContent.GetTextLine(start)));
                return syntax;
            }

            // Now read the rest of the remaining syntax blocks
            while(markdownContent.CurrentIndex < end)
            {
                var individualSyntaxStart = markdownContent.FindHeader(3, string.Empty);
                if (individualSyntaxStart == -1 || individualSyntaxStart > end)
                {
                    break;
                }

                markdownContent.Seek(individualSyntaxStart);
                if (markdownContent.Take() is HeadingBlock syntaxBlock)
                {
                    string parameterSetName = GetParameterSetName(syntaxBlock);
                    bool isDefault = parameterSetName.EndsWith(Constants.DefaultString, StringComparison.OrdinalIgnoreCase);
                    if (isDefault)
                    {
                        parameterSetName = parameterSetName.Replace(Constants.DefaultString, string.Empty).Trim();
                    }

                    if (markdownContent.GetCurrent() is FencedCodeBlock fcb)
                    {
                        var rawSyntax = fcb.Lines.ToString();
                        var si = CreateSyntaxFromText(rawSyntax, parameterSetName, isDefault);
                        diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.Syntax, "Syntax found", DiagnosticSeverity.Information, si.ToStringWithWrap(), fcb.Line));
                        syntax.Add(si);
                    }
                }
            }

            return syntax;
        }

        private static SyntaxItem CreateSyntaxFromText(string text, string pSetName, bool isDefault)
        {
            var spaceIndex = text.IndexOf(" ");
            var commandName = spaceIndex < 0 ? text : text.Substring(0, spaceIndex);
            var si = new SyntaxItem(commandName, pSetName, isDefault);
            si.SyntaxParameters = GetSyntaxParameters(text);
            si.HasCmdletBinding = text.IndexOf("CommonParameters", StringComparison.CurrentCultureIgnoreCase) != -1 ? true : false;
            return si;
        }

        internal static List<SyntaxParameter> GetSyntaxParameters(string rawSyntax)
        {
            var parameters = new List<SyntaxParameter>();
            int position = 0;
            string[] elements = rawSyntax.Split(new char[]{' ','\n','\r'}, StringSplitOptions.RemoveEmptyEntries);

            if (elements.Length <= 1)
            {
                return parameters;
            }

            var paramTrimChars = new char[] { '[', ']', '-'};
            var valueTrimChars = new char[] {'<', '>'};
            for (int i = 1; i < elements.Length; i++)
            {
                string parameter = elements[i];
                var parameterName = parameter.Trim(paramTrimChars);
                var commonParameterPattern = new Regex("CommonParameters", RegexOptions.IgnoreCase);
                if (commonParameterPattern.Match(parameter).Success)
                {
                    continue;
                }

                // This designates a type.
                if (i+1 < elements.Length && elements[i+1].StartsWith("<"))
                {
                    i++;
                    var pType = elements[i];
                    string pTypeElement = pType;
                    string pTypeName = pType;
                    if (pType[pType.Length - 1] == ']')
                    {
                        pType = pType.Remove(pType.Length - 1);
                    }

                    // Remove the initial '<' and a single trailing '>'
                    // This is because it might look like this: <Nullable<Int32>>
                    if (pType[0] == '<' && pType[pType.Length - 1] == '>')
                    {
                        pTypeName = pType.Remove(pType.Length - 1).Remove(0, 1);
                    }

                    if (parameter.StartsWith("[[") && parameter.EndsWith("]")) // [[-parm] <type>] optional, positional parameter
                    {
                        parameters.Add(
                            new SyntaxParameter {
                                ParameterName = parameterName,
                                ParameterType = pTypeName,
                                Position = position.ToString(),
                                IsMandatory = false,
                                IsPositional = true,
                                IsSwitchParameter = false
                            }
                        );
                        position++;
                    }
                    else if(parameter.StartsWith("[") && parameter.EndsWith("]")) // [-parm] <string[]> - mandatory positional
                    {
                        parameters.Add(
                            new SyntaxParameter {
                                ParameterName = parameterName,
                                ParameterType = pTypeName,
                                Position = position.ToString(),
                                IsMandatory = true,
                                IsPositional = true,
                                IsSwitchParameter = false
                            }
                        );
                        position++;
                    }
                    else if (parameter.StartsWith("[") && pTypeElement.EndsWith("]")) // [-par <string[]>] optional parameter and argument
                    {
                        parameters.Add(
                            new SyntaxParameter {
                                ParameterName = parameterName,
                                ParameterType = pTypeName,
                                Position = "named",
                                IsMandatory = false,
                                IsPositional = false,
                                IsSwitchParameter = false
                            }
                        );
                    }
                    else if (pType.StartsWith("<") && pType.EndsWith(">")) // -par <string[]> mandatory, non-positional
                    {
                        parameters.Add(
                            new SyntaxParameter {
                                ParameterName = parameterName,
                                ParameterType = pTypeName,
                                Position = "named",
                                IsMandatory = true,
                                IsPositional = false,
                                IsSwitchParameter = false
                            }
                        );
                    }
                }
                else // [-parm] or -parm - switch parameter
                {
                    if (parameter.StartsWith("-")) // Mandatory Switch -switch
                    {
                        parameters.Add(
                            new SyntaxParameter {
                                ParameterName = parameterName,
                                ParameterType = "SwitchParameter",
                                Position = "named",
                                IsMandatory = true,
                                IsPositional = false,
                                IsSwitchParameter = true
                            }
                        );
                    }
                    else if (parameter.StartsWith("[") && parameter.EndsWith("]")) // [-switch] optional
                    {
                        parameters.Add(
                            new SyntaxParameter {
                                ParameterName = parameterName,
                                ParameterType = "SwitchParameter",
                                Position = "named",
                                IsMandatory = false,
                                IsPositional = false,
                                IsSwitchParameter = true
                            }
                        );
                    }
                }
            }

            return parameters;
        }

        internal static string GetAliasesFromMarkdown(ParsedMarkdownContent markdownContent, out List<DiagnosticMessage> diagnostics, out bool aliasHeaderFound)
        {
            diagnostics = new List<DiagnosticMessage>();
            aliasHeaderFound = false;
            var start = markdownContent.FindHeader(2, "ALIASES");
            if (start == -1)
            {
                diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.Alias, "ALIASES header not found", DiagnosticSeverity.Warning, string.Empty, -1));
                return string.Empty;
            }

            markdownContent.Take();
            diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.Alias, "ALIASES header found", DiagnosticSeverity.Information, $"alias header is AST {start}", markdownContent.GetTextLine(start)));
            var end = markdownContent.FindHeader(2, string.Empty);
            // If there is no content between the ALIASES header and the next header, report that as no aliases found.
            if (start + 1 == end)
            {
                aliasHeaderFound = true;
                diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.Alias, "No ALIASES found", DiagnosticSeverity.Information, "Alias header AST {start} - next header AST {end}", markdownContent.GetTextLine(start)));
                return string.Empty;
            }

            var aliasList = GetAliases(markdownContent, start + 1);
            int totalAliasLength = aliasList.Length;
            diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.Alias, "Alias string length", DiagnosticSeverity.Information, $"alias string length: {totalAliasLength}", markdownContent.GetTextLine(start+1)));

            aliasHeaderFound = true;
            return aliasList;
        }

        internal static string GetSynopsisFromMarkdown(ParsedMarkdownContent markdownContent, out List<DiagnosticMessage> diagnostics)
        {
            string synopsisString = string.Empty;
            diagnostics = new List<DiagnosticMessage>();
            var start = markdownContent.FindHeader(2, "SYNOPSIS");
            if (start == -1)
            {
                diagnostics.Add(
                    new DiagnosticMessage(DiagnosticMessageSource.Synopsis, "SYNOPSIS not found", DiagnosticSeverity.Error, "missing synopsis", -1)
                );
                return string.Empty;
            }
            else
            {
                markdownContent.Seek(start);
                markdownContent.Take();
                var end = markdownContent.FindHeader(2, "SYNTAX");
                diagnostics.Add(
                    new DiagnosticMessage(DiagnosticMessageSource.Synopsis, "SYNOPSIS found", DiagnosticSeverity.Information, synopsisString, markdownContent.GetTextLine(start))
                );

                synopsisString = markdownContent.GetStringFromAst(end);
                // if the synopsis string is empty, report that as an error.
                if (synopsisString == string.Empty)
                {
                    diagnostics.Add(
                        new DiagnosticMessage(DiagnosticMessageSource.Synopsis, "SYNOPSIS found", DiagnosticSeverity.Error, "Synopsis text is empty", markdownContent.GetTextLine(start))
                    );
                }
            }

            return synopsisString;
        }

        // Avoid turning boilerplate into an alias
        internal static string GetAliases(ParsedMarkdownContent md, int startIndex)
        {
            var lines = GetLinesTillNextHeader(md, 2, startIndex);
            // don't include the boilerplate
            if (lines.IndexOf(Constants.AliasMessage2) == -1)
            {
                return lines;
            }

            return string.Empty;
        }

        private static bool ContainsAliasBoilerPlate(string aliasString)
        {
            if (string.Compare(aliasString, Constants.AliasMessage1) == 0)
            {
                return true;
            }

            if (string.Compare(aliasString, Constants.AliasMessage2) == 0)
            {
                return true;
            }

            return false;
        }

        internal static List<Links> GetLinks(ParsedMarkdownContent md, out List<DiagnosticMessage>diagnostics)
        {
            List<Links> links = new List<Links>();
            diagnostics = new List<DiagnosticMessage>();
            int start = -1;

            if (md.GetCurrent() is HeadingBlock)
            {
                start = md.CurrentIndex;
                md.Take();
            }

            // This is the v1 style of Links which were paragraphs
            // [text1](uri1)
            //
            // [text2](uri2)
            //
            // [text3](uri3)
            while (md.GetCurrent() is ParagraphBlock pb)
            {
                var item = pb?.Inline?.FirstChild;

                while (item != null)
                {
                    if (item is LinkInline link)
                    {
                        if (link is not null && link.Url is not null)
                        {
                            // While not common, the linkText may be null
                            // but that does not invalidate the related link.
                            string linkText = string.Empty;
                            if (link.FirstChild is not null)
                            {
                                linkText = link.FirstChild.ToString();
                            }
                            links.Add(new Links(link.Url, linkText));
                        }
                    }

                    item = item.NextSibling;
                }

                md.Take();
            }

            // This is link v2 where links are presented as a list
            // - [text1](uri1)
            // - [text2](uri2)
            // - [text3](uri3)
            if (md.GetCurrent() is ListBlock lb)
            {
                diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.Links, "Found related links as unordered list.", DiagnosticSeverity.Information, "", md.GetTextLine(start)));
                foreach(var inlineLink in lb.Descendants<LinkInline>())
                {
                    string url = inlineLink.Url is null ?  string.Empty : inlineLink.Url;
                    string linkText = inlineLink.FirstChild is null ? string.Empty : inlineLink.FirstChild.ToString();
                    diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.Links, $"adding link [{linkText}]({url})", DiagnosticSeverity.Information, "", md.GetTextLine(start)));
                    links.Add(new Links(url, linkText));
                }
            }

            diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.Links, links.Count > 0 ? "Links found" : "No links found", DiagnosticSeverity.Information, $"{links.Count} links found", md.GetTextLine(start)));
            return links;
        }

        internal static List<InputOutput> GetInputOutput(ParsedMarkdownContent md)
        {
            List<InputOutput> ioList = new();

            // find the next major header
            var nextSectionIndex = md.FindHeader(2, string.Empty);

            // If the next header is found in the next index, there is no data to process
            if (md.CurrentIndex + 1 == nextSectionIndex)
            {
                return ioList;
            }

            if (md.GetCurrent() is HeadingBlock header)
            {
                if (header.Level == 2)
                {
                    md.Take();
                }
            }

            string description;
            while(md.CurrentIndex < nextSectionIndex)
            {
                if (md.GetCurrent() is HeadingBlock inputOutputHeader)
                {
                    string inputType = inputOutputHeader?.Inline?.FirstChild?.ToString() ?? string.Empty;
                    md.Take();
                    if (md.GetCurrent() is ParagraphBlock pBlock)
                    {
                        description = GetLinesTillNextHeader(md, -1, md.CurrentIndex).Trim();
                        while(md.GetCurrent() is ParagraphBlock pBlock2)
                        {
                            md.Take();
                        }
                    }
                    else
                    {
                        description = string.Empty;
                    }

                    ioList.Add(new InputOutput(inputType.Trim(), description.Trim()));
                }
                else
                {
                    md.Take();
                }
            }

            return ioList;
        }

        internal static string GetParameterSetName(HeadingBlock parameterSetBlock)
        {
            if (parameterSetBlock is null)
            {
                return string.Empty;
            }

            StringBuilder? sb = null;

            try
            {
                sb = Constants.StringBuilderPool.Get();

                sb.Append(parameterSetBlock?.Inline?.FirstChild);

                var item = parameterSetBlock?.Inline?.FirstChild?.NextSibling;

                while (item != null)
                {
                    sb.Append(item.ToString());
                    item = item.NextSibling;
                }

                return sb.ToString().Replace("\r", "").TrimEnd();
            }
            finally
            {
                if (sb is not null)
                {
                    Constants.StringBuilderPool.Return(sb);
                }
            }
        }

        internal static int GetNextHeaderIndex(MarkdownDocument md, int expectedHeaderLevel, string? expectedHeaderTitle = null, int startIndex = 0)
        {
            for (int i = startIndex; i < md.Count; i++)
            {
                var item = md[i];

                if (item is HeadingBlock headerItem && headerItem.Level == expectedHeaderLevel)
                {
                    if (expectedHeaderTitle is not null)
                    {
                        if (string.Equals(headerItem?.Inline?.FirstChild?.ToString(), expectedHeaderTitle))
                        {
                            return i;
                        }
                    }
                    else
                    {
                        // sometime we do not know the header title, like for command name.
                        return i;
                    }
                }
                else if (item is HeadingBlock && expectedHeaderLevel == -1)
                {
                    // We just want the next header, no matter what level it is.
                    return i;
                }

            }

            return -1;
        }

        internal static Collection<Example> GetExamples(ParsedMarkdownContent mdc, int startIndex)
        {
            var md = mdc.Ast;
            var examples = new Collection<Example>();
            int endExampleIndex = GetNextHeaderIndex(md, expectedHeaderLevel: 2, expectedHeaderTitle: "PARAMETERS", startIndex: startIndex);
            int currentIndex = startIndex;
            while (currentIndex < endExampleIndex)
            {
                string exampleTitle = string.Empty;
                string exampleDescription = string.Empty;

                var exampleItemIndex = GetNextHeaderIndex(md, expectedHeaderLevel: 3, startIndex: currentIndex);

                if (exampleItemIndex > endExampleIndex || exampleItemIndex == -1)
                {
                    break;
                }

                if (md[exampleItemIndex] is HeadingBlock exampleTitleBlock)
                {
                    if (exampleTitleBlock is not null)
                    {
                        // First try to get the title from the markdown text
                        // This is because the markdown text may have embedded (e.g. **emphasis**)
                        // which requires more parsing, and won't improve the output.
                        try
                        {
                            // The title is expected to be in the format "### Title"
                            exampleTitle = mdc.MarkdownLines[exampleTitleBlock.Line].Substring(4).Trim();
                        }
                        catch
                        {
                            // Fallback to the AST
                            if (exampleTitleBlock?.Inline?.FirstChild?.ToString() is string example)
                            {
                                exampleTitle = example.Trim();
                            }
                            else
                            {
                                exampleTitle = string.Empty;
                            }
                        }
                    }

                }

                exampleDescription = GetLinesTillNextHeader(mdc, expectedLevel: -1, startIndex: exampleItemIndex + 1).Trim();
                examples.Add(new Example(exampleTitle, exampleDescription));
                currentIndex = exampleItemIndex + 1;
            }

            return examples;
        }

        internal static string GetParameterDescription(ParsedMarkdownContent md, int startIndex, int endIndex)
        {
            StringBuilder? sb = null;
            try
            {
                sb = Constants.StringBuilderPool.Get();
                var startLine = md.Ast[startIndex].Line;
                var endLine = md.Ast[endIndex].Line - 1;
                for(int i = startLine; i < endLine; i++)
                {
                    sb.AppendLine(md.MarkdownLines[i].TrimEnd());
                }
                return sb.ToString().Replace("\r","").Trim();
            }
            finally
            {
                if (sb is not null)
                {
                    Constants.StringBuilderPool.Return(sb);
                }
            }
        }

        internal static string GetParameterDescription(ParsedMarkdownContent md, int startIndex)
        {
            StringBuilder? sb = null;

            try
            {
                sb = Constants.StringBuilderPool.Get();
                var descriptionLine = md.Ast[startIndex + 1].Line;
                int paramYamlBlockIndex = GetParameterYamlBlockIndex(md.Ast, startIndex + 1);
                int paramYamlBlockLine = md.Ast[paramYamlBlockIndex].Line - 1;
                if (paramYamlBlockIndex == -1)
                {
                    return string.Empty;
                }

                for(; descriptionLine < paramYamlBlockLine; descriptionLine++)
                {
                    sb.AppendLine(md.MarkdownLines[descriptionLine].TrimEnd());
                }
                return sb.ToString().Replace("\r","").Trim();
            }
            finally
            {
                if (sb is not null)
                {
                    Constants.StringBuilderPool.Return(sb);
                }
            }
        }

        internal static Collection<Parameter> GetParameters(ParsedMarkdownContent markdownContent, int startIndex, out List<DiagnosticMessage> diagnostics)
        {
            diagnostics = new List<DiagnosticMessage>();
            var parameters = new Collection<Parameter>();
            var md = markdownContent.Ast;

            int nextHeaderLevel2 = GetNextHeaderIndex(md, expectedHeaderLevel: 2, startIndex: startIndex);

            int currentIndex = startIndex;

            // Read until we get to our next level 2 header (the end of the parameters)
            while(currentIndex < nextHeaderLevel2)
            {
                var parameterItemIndex = GetNextHeaderIndex(md, expectedHeaderLevel: 3, startIndex: currentIndex);
                string parameterName = string.Empty;

                if (parameterItemIndex > nextHeaderLevel2 || parameterItemIndex == -1)
                {
                    break;
                }

                if (md[parameterItemIndex] is HeadingBlock parameterTitle)
                {
                    if (parameterTitle?.Inline?.FirstChild?.ToString()?.TrimStart('-') is string param)
                    {
                        parameterName = param;
                    }
                }

                // Ignore CommonParameters, it's a property on the command and we have boilerplate for it.
                if (string.Equals(parameterName, "CommonParameters", StringComparison.OrdinalIgnoreCase))
                {
                    diagnostics.Add(
                        new DiagnosticMessage(DiagnosticMessageSource.Parameter, $"{parameterName} found", DiagnosticSeverity.Information, "GetParameters", md[currentIndex].Line + 1)
                        );
                    currentIndex = parameterItemIndex + 1;
                    continue;
                }

                // Ignore WorkflowCommonParameters, it's a property on the command and we have boilerplate for it.
                if (string.Equals(parameterName, "WorkflowCommonParameters", StringComparison.OrdinalIgnoreCase))
                {
                    currentIndex = parameterItemIndex + 1;
                    diagnostics.Add(
                        new DiagnosticMessage(DiagnosticMessageSource.Parameter, $"{parameterName} found", DiagnosticSeverity.Information, "GetParameters", md[currentIndex].Line + 1)
                        );
                    continue;
                }

                // Get the next yaml block as it has all the parameter metadata
                var yamlBlockIndex = GetNextCodeBlock(md, parameterItemIndex, "yaml");
                string description = GetParameterDescription(markdownContent, parameterItemIndex + 1, yamlBlockIndex).Trim();
                var paramYamlBlock = GetParameterYamlBlock(md, parameterItemIndex + 1, language: "yaml");

                if (paramYamlBlock != null)
                {
                    var yamlBlock = paramYamlBlock.Lines.ToString();

                    // yaml does not allow '*' to start a value, so we need to change this to be (all).
                    // yamlBlock = yamlBlock.Replace("* (all)","\"(all)\"");

                    if (ParameterMetadataV1.TryConvertToV1(yamlBlock, out var v1))
                    {
                        diagnostics.Add(
                            new DiagnosticMessage(DiagnosticMessageSource.Parameter, $"{parameterName} found", DiagnosticSeverity.Information, "Version 1 metadata found", md[parameterItemIndex].Line + 1)
                        );
                        parameters.Add(Parameter.ConvertV1ParameterToV2(parameterName, description, v1));
                    }
                    else if (ParameterMetadataV2.TryConvertToV2(yamlBlock, out var v2))
                    {
                        diagnostics.Add(
                            new DiagnosticMessage(DiagnosticMessageSource.Parameter, $"{parameterName} found", DiagnosticSeverity.Information, "Version 2 metadata found", md[parameterItemIndex].Line + 1)
                        );
                        parameters.Add(new Parameter(parameterName, description, v2));
                    }
                    else if (YamlUtils.TryConvertYamlToDictionary(yamlBlock, out var yamlDict))
                    {
                        diagnostics.Add(
                            new DiagnosticMessage(DiagnosticMessageSource.Parameter, "LastChance! Yaml may be invalid.", DiagnosticSeverity.Warning, yamlBlock, md[parameterItemIndex].Line + 1)
                        );

                        // We need to get the error from the V2 conversion, so do that first.
                        if(! ParameterMetadataV2.TryConvertToV2(yamlBlock, out var v2again) && v2again.DeserializationErrorMessage != null)
                        {
                            diagnostics.Add(
                                new DiagnosticMessage(DiagnosticMessageSource.Parameter, "YAML Parse Failure", DiagnosticSeverity.Error, v2again.DeserializationErrorMessage, md[parameterItemIndex].Line + 1)

                            );
                        }

                        // Last ditch effort - try a dictionary
                        AddParseError(parameterName, "YAML may have illegal elements, trying last chance", paramYamlBlock.Line);
                        if (YamlUtils.TryLastChance(yamlBlock, out var lastChance))
                        {
                            diagnostics.Add(
                                new DiagnosticMessage(DiagnosticMessageSource.Parameter, $"{parameterName} found", DiagnosticSeverity.Information, "found in LastChance parse", md[parameterItemIndex].Line + 1)
                            );
                                parameters.Add(new Parameter(parameterName, description, lastChance));
                            }
                        }
                        else
                        {
                            AddParseError(parameterName, "YAML was not v1 or v2 shape", md[parameterItemIndex].Line);
                        diagnostics.Add(
                            new DiagnosticMessage(DiagnosticMessageSource.Parameter, $"{parameterName} found", DiagnosticSeverity.Error, "YAML Parse Failure", md[parameterItemIndex].Line + 1)
                        );
                    }
                }

                currentIndex = parameterItemIndex + 1;
            }

            return parameters;
        }

        private static Parameter GetParameterFromV2ParameterMetadata(string name, ParameterMetadataV2 v2)
        {
            var parameter = new Parameter(name, v2.Type);

            return parameter;
        }

        private static Regex newPipelineInfoFormat = new Regex(@"ByName \((?<n>False|True)\), ByValue \((?<v>False|True)\)");
        private static PipelineInputInfo GetPipelineInputInfoFromString(string acceptedPipelineAsString)
        {
            if (string.IsNullOrWhiteSpace(acceptedPipelineAsString))
            {
                return new PipelineInputInfo(false);
            }

            if  (newPipelineInfoFormat.Match(acceptedPipelineAsString).Success)
            {
                var match = newPipelineInfoFormat.Match(acceptedPipelineAsString);
                bool byValue = string.Equals(match.Groups["v"].Value, "True", StringComparison.OrdinalIgnoreCase);
                bool byPropertyName = string.Equals(match.Groups["n"].Value, "True", StringComparison.OrdinalIgnoreCase);
                return new PipelineInputInfo(byValue, byPropertyName);
            }

            if (acceptedPipelineAsString.IndexOf("true", StringComparison.OrdinalIgnoreCase) != -1)
            {
                bool byValue = acceptedPipelineAsString.IndexOf("ByValue", StringComparison.OrdinalIgnoreCase) > 0;
                bool byPropertyName = acceptedPipelineAsString.IndexOf("ByPropertyName", StringComparison.OrdinalIgnoreCase) > 0;
                return new PipelineInputInfo(byValue, byPropertyName);
            }

            return new PipelineInputInfo(false);
        }

        private static Dictionary<string, object> parseYamlBlock(object? parsedYamlObject)
        {
            Dictionary<string, object> metadataHeader = new Dictionary<string, object>();
            if (parsedYamlObject is null)
            {
                return metadataHeader;
            }

            var yamlObject = parsedYamlObject as Dictionary<object, object>;
            if (yamlObject is null)
            {
                return metadataHeader;
            }

            if (yamlObject.TryGetValue("Type", out object type) && type is string typeStr)
            {
                metadataHeader["Type"] = typeStr.Trim();
            }
            else
            {
                metadataHeader["Type"] = string.Empty;
            }

            if (yamlObject.TryGetValue("Parameter Sets", out object parameterSets) && parameterSets is string parameterSetStr)
            {
                metadataHeader["Parameter Sets"] = parameterSetStr.Trim();
            }
            else
            {
                metadataHeader["Parameter Sets"] = string.Empty;
            }

            if (yamlObject.TryGetValue("Aliases", out object aliases) && aliases is string aliasesStr)
            {
                metadataHeader["Aliases"] = aliasesStr.Trim();
            }
            else
            {
                metadataHeader["Aliases"] = string.Empty;
            }

            if (yamlObject.TryGetValue("Accepted values", out object acceptedValues) && acceptedValues is string acceptedValuesStr)
            {
                metadataHeader["Accepted values"] = acceptedValuesStr.Trim();
            }
            else
            {
                metadataHeader["Accepted values"] = string.Empty;
            }

            if (yamlObject.TryGetValue("Required", out object required) && required is string requiredBoolStr)
            {
                metadataHeader["Required"] = requiredBoolStr.Trim();
            }
            else
            {
                metadataHeader["Required"] = string.Empty;
            }

            if (yamlObject.TryGetValue("Position", out object position) && position is string positionStr)
            {
                metadataHeader["Position"] = positionStr.Trim();
            }
            else
            {
                metadataHeader["Position"] = string.Empty;
            }

            if (yamlObject.TryGetValue("Default value", out object defaultValue) && defaultValue is string defaultValueStr)
            {
                metadataHeader["Default value"] = EscapeYamlValue(defaultValueStr.Trim());
            }
            else
            {
                metadataHeader["Default value"] = string.Empty;
            }

            if (yamlObject.TryGetValue("Accept pipeline input", out object acceptPipeline) && acceptPipeline is string acceptPipelineStr)
            {
                metadataHeader["Accept pipeline input"] = acceptPipelineStr.Trim();
            }
            else
            {
                metadataHeader["Accept pipeline input"] = string.Empty;
            }

            if (yamlObject.TryGetValue("Accept wildcard characters", out object acceptWildcard) && acceptWildcard is string acceptWildcardStr)
            {
                metadataHeader["Accept wildcard characters"] = acceptWildcardStr.Trim();
            }

            return metadataHeader;
        }

        internal static int GetNextCodeBlock(MarkdownDocument md, int startIndex, string language = "yaml")
        {
            for (int i = startIndex; i < md.Count; i++)
            {
                if (md[i] is FencedCodeBlock fcb && fcb.Info is not null && fcb.Info.Equals(language, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        // We need to ensure that when we write the yaml, we escape any values that need to be escaped.
        internal static string EscapeYamlValue(string value)
        {
            // It has an embedded ": ", so we need to quote it
            if (value.Contains(": ") || value.StartsWith(">>") || value.StartsWith("*") || value.Contains("]:"))
            {
                return $"\"{value}\"";
            }

            if (value.StartsWith("[") && ! value.EndsWith("]"))
            {
                return $"\"{value}\"";
            }

            return value;
        }

        // Look for the next header and return how many ast elements we need to skip to get to it.
        internal static int PeekHeader(MarkdownDocument md, int currentIndex, int level, string title)
        {
            int i = currentIndex;
            while (i < md.Count)
            {
                if (md[i] is HeadingBlock headerItem && headerItem.Level == level)
                {
                    if (string.Equals(headerItem?.Inline?.FirstChild?.ToString(), title, StringComparison.OrdinalIgnoreCase))
                    {
                        return i - currentIndex;
                    }
                }

                i++;
            }

            // there are no headers found
            return -1;
        }

        internal static Collection<ParagraphBlock> GetParagraphsTillNextHeaderBlock(MarkdownDocument md, int startIndex)
        {
            Collection<ParagraphBlock> paragraphs = new Collection<ParagraphBlock>();

            while (md[startIndex] is not HeadingBlock)
            {
                if (md[startIndex] is ParagraphBlock pb)
                {
                    paragraphs.Add(pb);
                }

                startIndex++;
            }

            return paragraphs;
        }

        internal static int GetParameterYamlBlockIndex(MarkdownDocument md, int startIndex)
        {
            for(int i = startIndex; i < md.Count; i++)
            {
                if (md[i] is FencedCodeBlock fencedCodeBlock)
                {
                    if (fencedCodeBlock.Info is not null && fencedCodeBlock.Info.StartsWith("yaml", StringComparison.OrdinalIgnoreCase))
                    {
                        return i;
                    }
                }
                else if (md[i] is HeadingBlock) // We've hit a header, so there is no yaml block
                {
                    return -1;
                }

                startIndex++;
            }

            return -1;
        }

        internal static FencedCodeBlock? GetParameterYamlBlock(MarkdownDocument md, int startIndex, string language = "yaml")
        {
            while (md[startIndex] is not HeadingBlock)
            {
                if (md[startIndex] is FencedCodeBlock fencedCodeBlock)
                {
                    if (fencedCodeBlock.Info is not null && fencedCodeBlock.Info.StartsWith(language, StringComparison.OrdinalIgnoreCase))
                    {
                        return fencedCodeBlock;
                    }
                }

                startIndex++;
            }

            return null;
        }

        internal static string GetLinesTillNextHeader(ParsedMarkdownContent md, int expectedLevel, int startIndex)
        {
            StringBuilder? sb = null;

            try
            {
                sb = Constants.StringBuilderPool.Get();
                var nextHeaderIndex = GetNextHeaderIndex(md.Ast, expectedHeaderLevel: expectedLevel, startIndex: startIndex+1);
                int startLine = md.Ast[startIndex].Line;
                int endLine = md.MarkdownLines.Count - 1; // by default, read to the end of the lines.
                if (nextHeaderIndex != -1)
                {
                    endLine = md.Ast[nextHeaderIndex].Line - 1;
                }
                // don't capture the empty lines at the end
                while (md.MarkdownLines[endLine].Trim().Length == 0)
                {
                    endLine--;
                }

                for (int i = startLine; i <= endLine; i++)
                {
                    sb.AppendLine(md.MarkdownLines[i].TrimEnd());
                }

                return sb.ToString().Replace("\r", "");
            }
            finally
            {
                if (sb is not null)
                {
                    Constants.StringBuilderPool.Return(sb);
                }
            }
        }

        internal static string GetParagraphsTillNextHeader(MarkdownDocument md, int startIndex)
        {
            StringBuilder? sb = null;

            try
            {
                sb = Constants.StringBuilderPool.Get();

                while (md[startIndex] is not HeadingBlock)
                {
                    if (md[startIndex] is ParagraphBlock pb)
                    {
                        var item = pb?.Inline?.FirstChild;

                        while (item != null)
                        {
                            if (item is LiteralInline line)
                            {
                                sb.Append(line.ToString());
                            }
                            else if (item is CodeInline cbLine)
                            {
                                sb.Append($"{cbLine.Delimiter}{cbLine.Content}{cbLine.Delimiter}");
                            }
                            else if (item is LineBreakInline lbLine)
                            {
                                sb.Append(" ");
                            }

                            item = item.NextSibling;
                        }
                    }

                    sb.AppendLine();

                    startIndex++;
                }

                return sb.ToString().Replace("\r", "");
            }
            finally
            {
                if (sb is not null)
                {
                    Constants.StringBuilderPool.Return(sb);
                }
            }

        }

        internal static string RemoveLineEndings(string line)
        {
            return line.TrimEnd('\r', '\n');
        }

        internal static bool ValidateMarkdown(MarkdownDocument md, out List<string>Issues)
        {
            List<string> foundIssues = new();
            bool result = true;

            List<MarkdownElement> markdownElements = new List<MarkdownElement>();
            markdownElements.Add(new MarkdownElement("SYNOPSIS", 2));
            markdownElements.Add(new MarkdownElement("SYNTAX", 2));
            markdownElements.Add(new MarkdownElement("DESCRIPTION", 2));
            markdownElements.Add(new MarkdownElement("EXAMPLES", 2));
            markdownElements.Add(new MarkdownElement("PARAMETERS", 2));
            markdownElements.Add(new MarkdownElement("INPUTS", 2));
            markdownElements.Add(new MarkdownElement("OUTPUTS", 2));
            markdownElements.Add(new MarkdownElement("NOTES", 2));
            markdownElements.Add(new MarkdownElement("RELATED LINKS", 2));

            if (md[0] is not Markdig.Syntax.ThematicBreakBlock)
            {
                foundIssues.Add("FAIL: First element is not a thematic break");
                result = false;
            }
            else
            {
                foundIssues.Add("PASS: First element is a thematic break");
            }

            int currentElement = 0;
            foreach(var element in markdownElements)
            {
                var elementIndex = GetNextHeaderIndex(md, expectedHeaderLevel: element.Level, expectedHeaderTitle: element.Name, startIndex: currentElement + 1);
                if (elementIndex == -1)
                {
                    foundIssues.Add($"FAIL: {element.Name} not found.");
                    result = false;
                }
                else
                {
                    foundIssues.Add($"PASS: {element.Name} found.");
                    if (elementIndex < currentElement)
                    {
                        foundIssues.Add($"FAIL: {element.Name} is out of order.");
                        result = false;
                    }
                    else
                    {
                        foundIssues.Add($"PASS: {element.Name} is in order.");
                    }
                }

                currentElement = elementIndex;
            }

            Issues = foundIssues;
            return result;
        }

        internal static bool GetWorkflowCommonParameterState(ParsedMarkdownContent markdownContent, out List<DiagnosticMessage>diagnostics)
        {
            diagnostics = new List<DiagnosticMessage>();
            var index = markdownContent.FindHeader(3, "WorkflowCommonParameters");
            var dm = new DiagnosticMessage(DiagnosticMessageSource.General, "Workflow parameters not present", DiagnosticSeverity.Information, "GetWorkflowCommonParameterState", markdownContent.GetTextLine(index));
            diagnostics.Add(dm);
            return index > -1;
        }

        internal static bool GetCmdletBindingState(ParsedMarkdownContent markdownContent, out List<DiagnosticMessage> diagnostics)
        {
            diagnostics = new List<DiagnosticMessage>();
            var index = markdownContent.FindHeader(3, "CommonParameters");
            var dm = new DiagnosticMessage(DiagnosticMessageSource.General, index != -1 ? "CmdletBinding is present" : "CmdletBinding is not present", DiagnosticSeverity.Information, "GetCmdletBindingState", markdownContent.GetTextLine(index));
            diagnostics.Add(dm);
            return index > -1;
        }
    }

    public class MarkdownElement
    {
        // The public name of the element
        public string Name { get; set; }

        // The level of the element
        public int Level { get; set; }

        public MarkdownElement(string name, int level)
        {
            Name = name;
            Level = level;
        }
    }
}
