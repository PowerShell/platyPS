// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Markdig.Extensions.CustomContainers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.PowerShell.PlatyPS.MarkdownWriter;
using Microsoft.PowerShell.PlatyPS.Model;
using System;
using System.Collections.Generic;
using System.Collections;
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.PowerShell.PlatyPS
{
    public class MarkdownConverter
    {

        /// <summary>
        /// Create a CommandHelp object from a markdown file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>A CommandHelp Object, but declared as object as the CommandHelp object is internal.</returns>
		public static object GetCommandHelpFromMarkdownFile(string path)
		{
            var md = ParsedMarkdownContent.ParseFile(path);
			return GetCommandHelpFromMarkdown(md);
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
        /// Create a CommandHelp markdown file from an existing markdown file.
        /// </summary>
        /// <param name="path">The path to the source markdown.</param>
        /// <param name="destinationPath">The path to the target markdown file.</param>
        public static void ExportMarkdown(string path, string destinationPath)
        {
            CommandHelp commandHelp = (CommandHelp)GetCommandHelpFromMarkdownFile(path);
            var encoding = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            var settings = new CommandHelpWriterSettings(encoding, destinationPath);
            var cmdWrt = new CommandHelpMarkdownWriter(settings);
            cmdWrt.Write(commandHelp, null);
        }

        internal static string GetCommandNameFromMarkdown(ParsedMarkdownContent mc)
        {
            var idx = mc.FindHeader(1, string.Empty);
            if(mc.Ast[idx] is HeadingBlock title)
            {
                return title?.Inline?.FirstChild?.ToString() ?? string.Empty;
            }
            return string.Empty;
        }

        internal static CommandHelp GetCommandHelpFromMarkdown(ParsedMarkdownContent markdownContent)
        {
            /*
            GetMetadataFromMarkdown
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

            OrderedDictionary? metadata = GetMetadata(markdownContent.Ast);
			string moduleName = metadata?["Module Name"] as string ?? string.Empty;
            var commandName = GetCommandNameFromMarkdown(markdownContent);
            if (commandName == string.Empty)
            {
                throw new InvalidDataException("commandName not found. markdown structure not according to schema.");
            }
            commandHelp = new CommandHelp(commandName, moduleName, cultureInfo: null);

            commandHelp.Metadata = metadata;

            commandHelp.HasCmdletBinding = GetCmdletBindingState(markdownContent);
            commandHelp.HasWorkflowCommonParameters = GetWorkflowCommonParameterState(markdownContent);

            commandHelp.Synopsis = GetSynopsisFromMarkdown(markdownContent);

            commandHelp.Syntax?.AddRange(GetSyntaxFromMarkdown(markdownContent));

            commandHelp.Aliases?.AddRange(GetAliasesFromMarkdown(markdownContent));

            commandHelp.Description = GetDescriptionFromMarkdown(markdownContent);

            commandHelp.Examples?.AddRange(GetExamplesFromMarkdown(markdownContent));

            commandHelp.Parameters.AddRange(GetParametersFromMarkdown(markdownContent));

            var commandInputs = GetInputsFromMarkdown(markdownContent);
            if (commandInputs is not null)
            {
                commandHelp.Inputs?.Add(commandInputs);
            }

            var commandOutputs = GetOutputsFromMarkdown(markdownContent);
            if (commandOutputs is not null)
            {
                commandHelp.Outputs?.Add(commandOutputs);
            }

            commandHelp.Notes = GetNotesFromMarkdown(markdownContent);

            commandHelp.RelatedLinks?.AddRange(GetRelatedLinksFromMarkdown(markdownContent));

            return commandHelp;

        }

        internal static OrderedDictionary? GetMetadata(MarkdownDocument ast)
        {
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
            }

            return null;
        }

        // convert the string we read to an ordered dictionary
        internal static OrderedDictionary ConvertTextToOrderedDictionary(string text)
        {
            OrderedDictionary od = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
			char[] fieldSeparator = { ':' };
            foreach(string s in text.Split(Constants.LineSplitter))
            {
                string[] kv = s.Split(fieldSeparator, 2, StringSplitOptions.None);
                if (kv.Length == 2)
                {
                    od.Add(kv[0].Trim(), kv[1].Trim());
                }
            }

            return od;
        }

        /// <summary>
        /// Retrieve the related link information from the markdown
        /// </summary>
        /// <param name="markdownContent">The parsed markdown data.</param>
        /// <returns>List<Links></returns>
        internal static List<Links> GetRelatedLinksFromMarkdown(ParsedMarkdownContent markdownContent)
        {
            markdownContent.Reset();
            var start = markdownContent.FindHeader(2, "RELATED LINKS");
            if (start == -1 || markdownContent.Ast.Count <= start + 1)
            {
                return new List<Links>();
            }

            markdownContent.Seek(start);
            var links = GetLinks(markdownContent);
            return links;
        }

        internal static string GetNotesFromMarkdown(ParsedMarkdownContent markdownContent)
        {
            markdownContent.Reset();
            var start = markdownContent.FindHeader(2, "NOTES");
            if (start == -1)
            {
                return string.Empty;
            }
            markdownContent.Seek(start);
            var end = markdownContent.FindHeader(2, "RELATED LINKS");
            // Notes may be blank, which means the NOTES header is followed by RELATED LINKS header
            if (end - start == 1)
            {
                return string.Empty;
            }

            markdownContent.Take();
            return markdownContent.GetStringFromAst(end);
        }

        internal static InputOutput? GetInputsFromMarkdown(ParsedMarkdownContent markdownContent)
        {
            markdownContent.Reset();
            var start = markdownContent.FindHeader(2, "INPUTS");
            if (start != -1)
            {
                markdownContent.Seek(start);
                var inputOutput = GetInputOutput(markdownContent);
                return inputOutput;
            }
            return null;
        }

        internal static InputOutput? GetOutputsFromMarkdown(ParsedMarkdownContent markdownContent)
        {
            markdownContent.Reset();
            var start = markdownContent.FindHeader(2, "OUTPUTS");
            if (start != -1)
            {
                markdownContent.Seek(start);
                var inputOutput = GetInputOutput(markdownContent);
                return inputOutput;
            }
            return null;
        }

        internal static Collection<Parameter> GetParametersFromMarkdown(ParsedMarkdownContent markdownContent)
        {
            markdownContent.Reset();
            var start = markdownContent.FindHeader(2, "PARAMETERS");
            var parameters = GetParameters(markdownContent, start + 1);
            return parameters;
        }

        internal static Collection<Example> GetExamplesFromMarkdown(ParsedMarkdownContent markdownContent)
        {

            markdownContent.Reset();
            var start = markdownContent.FindHeader(2, "EXAMPLES");
            var examples = GetExamples(markdownContent, start + 1);
            return examples;
        }

        internal static string GetDescriptionFromMarkdown(ParsedMarkdownContent markdownContent)
        {
            markdownContent.Reset();
            var start = markdownContent.FindHeader(2, "DESCRIPTION");
            markdownContent.Seek(start);
            markdownContent.Take();
            var end = markdownContent.FindHeader(2, string.Empty);
            return markdownContent.GetStringFromAst(end);
        }

        internal static List<SyntaxItem> GetSyntaxFromMarkdown(ParsedMarkdownContent markdownContent)
        {
            var start = markdownContent.FindHeader(2, "SYNTAX");
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
                    var syntaxLine = rawSyntax.Replace("[<CommonParameters>]", string.Empty).Trim();
                    syntax.Add(new SyntaxItem(syntaxLine.Trim(), "Default", true));
                }
                return syntax;
            }

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
                    bool isDefault = parameterSetName.EndsWith("(default)", StringComparison.OrdinalIgnoreCase);
                    if (isDefault)
                    {
                        parameterSetName = parameterSetName.Replace("(Default)", string.Empty).Trim();
                    }

                    string syntaxLine = string.Empty;
                    if (markdownContent.GetCurrent() is FencedCodeBlock fcb)
                    {
                        var rawSyntax = fcb.Lines.ToString();
                        // we don't need to keep CommonParameters here, as we will add it back as part of the writing process.
                        syntaxLine = rawSyntax.Replace("[<CommonParameters>]", string.Empty).Trim();
                        // no take - the next findheader should move us forward
                        // markdownContent.Take();
                    }

                    syntax.Add(new SyntaxItem(syntaxLine.Trim(), parameterSetName.Trim(), isDefault));
                }
                // Else Unget????

            }

            return syntax;
        }

        internal static List<string> GetAliasesFromMarkdown(ParsedMarkdownContent markdownContent)
        {
            var start = markdownContent.FindHeader(2, "ALIASES");
            if (start == -1)
            {
                return new List<string>();
            }
            return GetAliases(markdownContent.Ast, start + 1);
        }

        internal static string GetSynopsisFromMarkdown(ParsedMarkdownContent markdownContent)
        {
            var start = markdownContent.FindHeader(2, "SYNOPSIS");
            markdownContent.Seek(start);
            markdownContent.Take();
            if (start == -1)
            {
                return string.Empty;
            }
            var end = markdownContent.FindHeader(2, "SYNTAX");
            return markdownContent.GetStringFromAst(end);
        }

        internal static List<string> GetAliases(MarkdownDocument md, int startIndex)
        {
            List<string> aliases = new List<string>();

            while (md[startIndex] is not HeadingBlock)
            {
                if (md[startIndex] is ParagraphBlock pb)
                {
                    var item = pb?.Inline?.FirstChild;

                    while (item != null)
                    {
                        if (item is LiteralInline line)
                        {
                            aliases.Add(line.ToString());
                        }

                        item = item.NextSibling;
                    }
                }

                startIndex++;
            }

            return aliases;
        }

        internal static List<Links> GetLinks(ParsedMarkdownContent md)
        {
            List<Links> links = new List<Links>();

            if (md.GetCurrent() is HeadingBlock)
            {
                md.Take();
            }

            while (md.GetCurrent() is ParagraphBlock pb)
            {
                var item = pb?.Inline?.FirstChild;

                while (item != null)
                {
                    if (item is LinkInline link)
                    {
                        if (link is not null && link.Url is not null && link.FirstChild is not null)
                        {
                            links.Add(new Links(link.Url, link.FirstChild.ToString()));
                        }
                    }

                    item = item.NextSibling;
                }

                md.Take();
                if (md.IsEnd())
                {
                    break;
                }
            }

            return links;
        }

        internal static InputOutput GetInputOutput(ParsedMarkdownContent md)
        {
            InputOutput ioList = new();
            if (md.GetCurrent() is HeadingBlock)
            {
                md.Take();
            }

            // find the next major header
            var nextSectionIndex = md.FindHeader(2, string.Empty);
            
            // If the next header is found in the next index, there is no data to process
            if (md.CurrentIndex + 1 == nextSectionIndex)
            {
                return ioList;
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

                    ioList.AddInputOutputItem(inputType.Trim(), description.Trim());
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

                return sb.ToString();
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

                if (exampleItemIndex > endExampleIndex)
                {
                    break;
                }

                if (md[exampleItemIndex] is HeadingBlock exampleTitleBlock)
                {
                    if (exampleTitleBlock?.Inline?.FirstChild?.ToString() is string example)
                    {
                        // example title with a number and a colon
                        var exampleRegex1 = new System.Text.RegularExpressions.Regex(@"^Example\s+\d+[ :-]+\s");
                        // no actual example title
                        var exampleRegex2 = new System.Text.RegularExpressions.Regex(@"^Example\s+\d+[ :-]$");
                        if (exampleRegex1.IsMatch(example))
                        {
                            exampleTitle = exampleRegex1.Replace(example, string.Empty).Trim();
                        }
                        else if (exampleRegex1.IsMatch(example))
                        {
                            exampleTitle = exampleRegex2.Replace(example, string.Empty).Trim();
                        }
                        else
                        {
                            exampleTitle = example.Trim();
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
                    sb.AppendLine(md.MarkdownLines[i]);
                }
                return sb.ToString().Trim();
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
                    sb.AppendLine(md.MarkdownLines[descriptionLine]);
                }
                return sb.ToString().Trim();
            }
            finally
            {
                if (sb is not null)
                {
                    Constants.StringBuilderPool.Return(sb);
                }
            }
        }

        internal static Collection<Parameter> GetParameters(ParsedMarkdownContent markdownContent, int startIndex)
        {
            var parameters = new Collection<Parameter>();
            var md = markdownContent.Ast;

            int nextHeaderLevel2 = GetNextHeaderIndex(md, expectedHeaderLevel: 2, startIndex: startIndex);

            int currentIndex = startIndex;

            while(currentIndex < nextHeaderLevel2)
            {
                string parameterName = string.Empty;
                string typeAsString = string.Empty;
                string parameterSetsAsString = string.Empty;
                string aliasesAsString = string.Empty;
                string acceptedValuesAsString = string.Empty;
                string requiredAsString = string.Empty;
                string positionAsString = string.Empty;
                string defaultValuesAsString = string.Empty;
                string acceptedPipelineAsString = string.Empty;
                string acceptWildCardAsString = string.Empty;

                var parameterItemIndex = GetNextHeaderIndex(md, expectedHeaderLevel: 3, startIndex: currentIndex);

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
                    currentIndex = parameterItemIndex + 1;
                    continue;
                }

                // Ignore WorkflowCommonParameters, it's a property on the command and we have boilerplate for it.
                if (string.Equals(parameterName, "WorkflowCommonParameters", StringComparison.OrdinalIgnoreCase))
                {
                    currentIndex = parameterItemIndex + 1;
                    continue;
                }

                var yamlBlockIndex = GetNextCodeBlock(md, parameterItemIndex, "yaml");
                // string description = GetParameterDescription(markdownContent, parameterItemIndex);
                string description = GetParameterDescription(markdownContent, parameterItemIndex + 1, yamlBlockIndex);

                var paramYamlBlock = GetParameterYamlBlock(md, parameterItemIndex + 1, language: "yaml");

                if (paramYamlBlock != null)
                {
                    var yamlBlock = paramYamlBlock.Lines.ToString();

                    // yaml does not allow '*' to start a value, so we need to change the order here
                    yamlBlock = yamlBlock.Replace(" * (all)","\"* (all)\"");

                    StringReader stringReader = new StringReader(yamlBlock);
                    var deserializer = new YamlDotNet.Serialization.DeserializerBuilder().Build();
                    var yamlObject = deserializer.Deserialize(stringReader);

                    if (yamlObject is Dictionary<object, object> metadataHeader)
                    {
                        object type;
                        object parameterSets;
                        object aliases;
                        object acceptedValues;
                        object required;
                        object position;
                        object defaultValue;
                        object acceptPipeline;
                        object acceptWildcard;


                        if (metadataHeader.TryGetValue("Type", out type) && type is string typeStr)
                        {
                            typeAsString = typeStr.Trim();
                        }

                        if (metadataHeader.TryGetValue("Parameter Sets", out parameterSets) && parameterSets is string parameterSetStr)
                        {
                            parameterSetsAsString = parameterSetStr.Trim();
                        }

                        if (metadataHeader.TryGetValue("Aliases", out aliases) && aliases is string aliasesStr)
                        {
                            aliasesAsString = aliasesStr.Trim();
                        }

                        if (metadataHeader.TryGetValue("Accepted values", out acceptedValues) && acceptedValues is string acceptedValuesStr)
                        {
                            acceptedValuesAsString = acceptedValuesStr.Trim();
                        }

                        if (metadataHeader.TryGetValue("Required", out required) && required is string requiredBoolStr)
                        {
                            requiredAsString = requiredBoolStr.Trim();
                        }

                        if (metadataHeader.TryGetValue("Position", out position) && position is string positionStr)
                        {
                            positionAsString = positionStr.Trim();
                        }

                        if (metadataHeader.TryGetValue("Default value", out defaultValue) && defaultValue is string defaultValueStr)
                        {
                            defaultValuesAsString = EscapeYamlValue(defaultValueStr.Trim());
                        }

                        if (metadataHeader.TryGetValue("Accept pipeline input", out acceptPipeline) && acceptPipeline is string acceptPipelineStr)
                        {
                            acceptedPipelineAsString = acceptPipelineStr.Trim();
                        }

                        if (metadataHeader.TryGetValue("Accept wildcard characters", out acceptWildcard) && acceptWildcard is string acceptWildcardStr)
                        {
                            acceptWildCardAsString = acceptWildcardStr.Trim();
                        }
                    }
                }


                Parameter parameter = new Parameter(parameterName, typeAsString, positionAsString);

                var isAllParameterSets = string.Equals(parameterSetsAsString, Constants.ParameterSetsAll, StringComparison.OrdinalIgnoreCase);
                parameter.AddParameterSet(isAllParameterSets ? Constants.ParameterSetsAll : parameterSetsAsString);
                parameter.Aliases = string.IsNullOrEmpty(aliasesAsString) ? null : aliasesAsString;
                parameter.AddAcceptedValueRange(acceptedValuesAsString.Split(Constants.Comma).Select(x => x.Trim()).ToArray());
                parameter.Required = string.Equals(requiredAsString, Constants.TrueString, StringComparison.OrdinalIgnoreCase);
                parameter.DefaultValue = string.IsNullOrEmpty(defaultValuesAsString) ? null : defaultValuesAsString;
                parameter.Position = positionAsString;
                parameter.PipelineInput = new PipelineInputInfo(string.Equals(acceptedPipelineAsString, Constants.TrueString, StringComparison.OrdinalIgnoreCase));
                parameter.Globbing = string.Equals(acceptWildCardAsString, Constants.TrueString, StringComparison.OrdinalIgnoreCase);
                parameter.Description = description;

                parameters.Add(parameter);

                currentIndex = parameterItemIndex + 1;
            }

            return parameters;
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
                int endLine = md.Ast[nextHeaderIndex].Line - 1;
                // don't capture the empty lines at the end
                while (md.MarkdownLines[endLine].Trim().Length == 0)
                {
                    endLine--;
                }

                for (int i = startLine; i <= endLine; i++)
                {
                    sb.AppendLine(md.MarkdownLines[i]);
                }

                return sb.ToString();
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

                return sb.ToString();
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
                }

                if (elementIndex < currentElement)
                {
                    foundIssues.Add($"FAIL: {element.Name} is out of order.");
                    result = false;
                }
                else
                {
                    foundIssues.Add($"PASS: {element.Name} is in order.");
                }

                currentElement = elementIndex;
            }

            Issues = foundIssues;
            return result;
        }

        internal static bool GetWorkflowCommonParameterState(ParsedMarkdownContent markdownContent)
        {
            return (markdownContent.FindHeader(3, "WorkflowCommonParameters") > -1);
        }

        internal static bool GetCmdletBindingState(ParsedMarkdownContent markdownContent)
        {
            return (markdownContent.FindHeader(3, "CommonParameters") > -1);
        }
    }

    public class ParsedMarkdownContent
    {
        public List<string> MarkdownLines { get; set; }
        public MarkdownDocument Ast { get; set; }
        public int CurrentIndex { get; set; }
        public List<string> Errors { get; set; }
        public List<string> Warnings { get; set; }
        public ParsedMarkdownContent(string fileContent)
        {
            MarkdownLines = new List<string>(fileContent.Replace("\r", "").Split(Constants.LineSplitter));
            Ast = Markdig.Markdown.Parse(fileContent);
            CurrentIndex = 0;
            Errors = new List<string>();
            Warnings = new List<string>();
        }

        public ParsedMarkdownContent(string[] lines)
        {
            MarkdownLines = new List<string>(lines);
            Ast = Markdig.Markdown.Parse(string.Join("\n", lines));
            CurrentIndex = 0;
            Errors = new List<string>();
            Warnings = new List<string>();
        }

        public static ParsedMarkdownContent ParseFile(string path)
        {
            return new ParsedMarkdownContent(File.ReadAllLines(path));
        }

        public void AddError(string error)
        {
            Errors.Add(error);
        }

        public void AddWarning(string warning)
        {
            Warnings.Add(warning);
        }

        public void UnGet()
        {
            if (CurrentIndex > 0)
            {
                CurrentIndex--;
            }
        }

        public void Reset()
        {
            CurrentIndex = 0;
        }

        public int FindHeader(int level, string title)
        {
            for(int i = CurrentIndex+1; i < Ast.Count; i++)
            {
                if (Ast[i] is HeadingBlock headerItem && headerItem.Level == level)
                {
                    if (title == string.Empty)
                    {
                        return i;
                    }
                    else if (string.Equals(headerItem?.Inline?.FirstChild?.ToString(), title, StringComparison.OrdinalIgnoreCase))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        public bool IsEnd()
        {
            return CurrentIndex >= Ast.Count;
        }

        public object? Peek()
        {
            if (IsEnd())
            {
                return null;
            }

            return Ast[CurrentIndex+1];
        }

        public object GetCurrent()
        {
            return Ast[CurrentIndex];
        }

        public void Seek(int index)
        {
            CurrentIndex = index;
        }

        public object? Take()
        {
            if (IsEnd())
            {
                return null;
            }

            return Ast[CurrentIndex++];
        }

        public bool IsEmptyHeader(int Level)
        {
            var currentHeader = Ast[CurrentIndex] as HeadingBlock;
            var nextHeader = Ast[CurrentIndex + 1] as HeadingBlock;
            if (currentHeader is not null && nextHeader is not null)
            {
                if (currentHeader.Level == Level && nextHeader.Level == Level)
                {
                    return true;
                }
            }

            return false;
        }

        public string GetStringFromAst(int endIndex)
        {
            if (endIndex <= CurrentIndex)
            {
                return string.Empty;
            }

            StringBuilder? sb = null;
            try
            {
                sb = Constants.StringBuilderPool.Get();
                int startLine = Ast[CurrentIndex].Line;
                int endLine = Ast[endIndex].Line;
                if (Ast[endIndex] is HeadingBlock)
                {
                    endLine--;
                }

                for(int i = startLine; i < endLine; i++)
                {
                    sb.AppendLine(MarkdownLines[i]);
                }
                return sb.ToString().Trim();
            }
            finally
            {
                if (sb is not null)
                {
                    Constants.StringBuilderPool.Return(sb);
                }
            }
        }

        public string GetStringFromFile(int lineCount)
        {
            StringBuilder? sb = null;
            try
            {
                sb = Constants.StringBuilderPool.Get();
                int startLine = Ast[CurrentIndex].Line;
                for(int i = startLine; i < startLine + lineCount; i++)
                {
                    sb.AppendLine(MarkdownLines[i]);
                }

                return sb.ToString();
            }
            finally
            {
                if (sb is not null)
                {
                    Constants.StringBuilderPool.Return(sb);
                }
            }
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
