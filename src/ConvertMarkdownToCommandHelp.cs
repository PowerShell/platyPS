using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Net;
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Markdig.Extensions.CustomContainers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.PowerShell.PlatyPS.Model;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Management.Automation.Runspaces;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.PowerShell.PlatyPS
{
    public class MarkdownConverter
    {
        // convert the string we read to a dictionary
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
            

        internal static OrderedDictionary? GetMetadata(MarkdownDocument ast)
        {
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

		public static object GetCommandHelpFromMarkdownFile(string path)
		{
			return GetCommandHelpFromMarkdown(File.ReadAllText(path));
		}

        internal static CommandHelp GetCommandHelpFromMarkdown(string fileContent)
        {
            CommandHelp commandHelp;
            var parsedMarkdownFile = new ParsedMarkdownContent(fileContent);

            if (parsedMarkdownFile.Ast is null)
            {
                throw new ParseException("File content cannot be parsed as markdown");
            }

            if (! ValidateMarkdown(parsedMarkdownFile.Ast))
            {
                throw new ParseException("File content does not validate as command help markdown");
            }

            // Metadata
            OrderedDictionary? metadata = GetMetadata(parsedMarkdownFile.Ast);
			string moduleName;
			if (metadata is not null) {
				moduleName = (string)metadata["module Name"] ?? string.Empty;
			}
			else
			{
				moduleName = string.Empty;
			}

            #region Get Command Name

            int titleHeaderIndex = GetNextHeaderIndex(parsedMarkdownFile.Ast, 1);

            if (parsedMarkdownFile.Ast[titleHeaderIndex] is HeadingBlock title)
            {
                string? commandName = title?.Inline?.FirstChild?.ToString();

                if (commandName is not null)
                {
                    commandHelp = new CommandHelp(commandName, moduleName, cultureInfo: null);
                }
                else
                {
                    throw new InvalidDataException("commandName not found. markdown structure not according to schema.");
                }
            }
            else
            {
                throw new InvalidDataException("markdown structure not according to schema.");
            }

            commandHelp.Metadata = metadata;

            #endregion

            #region Get Synopsis

            int synopsisHeaderIndex = GetNextHeaderIndex(parsedMarkdownFile.Ast, expectedHeaderLevel: 2, expectedHeaderTitle: "SYNOPSIS", startIndex: titleHeaderIndex + 1);
            commandHelp.Synopsis = GetParagraphsTillNextHeader(parsedMarkdownFile.Ast, synopsisHeaderIndex + 1);

            #endregion Get Synopsis

            #region Get Syntax
            int syntaxHeaderIndex = GetNextHeaderIndex(parsedMarkdownFile.Ast, expectedHeaderLevel: 2, expectedHeaderTitle: "SYNTAX", startIndex: synopsisHeaderIndex + 1);

            int parameterSetIndex = GetNextHeaderIndex(parsedMarkdownFile.Ast, expectedHeaderLevel: 3, startIndex: synopsisHeaderIndex + 1);
            int endParametersIndex = GetNextHeaderIndex(parsedMarkdownFile.Ast, expectedHeaderLevel: 2, expectedHeaderTitle: "DESCRIPTION", startIndex: parameterSetIndex + 1);
            while(parameterSetIndex < endParametersIndex)
            {
                if (parsedMarkdownFile.Ast[parameterSetIndex] is HeadingBlock parameterSet)
                {
                    string parameterSetName = GetParameterSetName(parameterSet);
                    bool isDefault = parameterSetName.EndsWith("(default)", StringComparison.OrdinalIgnoreCase);
                    string syntaxLine = string.Empty;

                    if (parsedMarkdownFile.Ast[parameterSetIndex + 1] is FencedCodeBlock fcb)
                    {
                        syntaxLine = fcb.Lines.ToString();
                    }

                    commandHelp.Syntax.Add(new SyntaxItem(parameterSetName, syntaxLine, isDefault));
                }

                parameterSetIndex = GetNextHeaderIndex(parsedMarkdownFile.Ast, expectedHeaderLevel: 3, startIndex: parameterSetIndex + 1);
            }
            #endregion Get Syntax

            #region Get Description

            int descriptionHeaderIndex = GetNextHeaderIndex(parsedMarkdownFile.Ast, expectedHeaderLevel: 2, expectedHeaderTitle: "DESCRIPTION", startIndex: synopsisHeaderIndex + 1);
            // commandHelp.Description = GetParagraphsTillNextHeader(parsedMarkdownFile.Ast, descriptionHeaderIndex + 1);
            commandHelp.Description = GetLinesTillNextHeader(parsedMarkdownFile, 2, descriptionHeaderIndex + 1);

            #endregion Get Description

			#region Get Examples

            int exampleHeaderIndex = GetNextHeaderIndex(parsedMarkdownFile.Ast, expectedHeaderLevel: 2, expectedHeaderTitle: "EXAMPLES");
            var examples = GetExamples(parsedMarkdownFile.Ast, exampleHeaderIndex + 1);
            commandHelp.Examples?.AddRange(examples);

			#endregion

            #region Get Parameters

            int paramHeaderIndex = GetNextHeaderIndex(parsedMarkdownFile.Ast, expectedHeaderLevel: 2, expectedHeaderTitle: "PARAMETERS");
            var parameters = GetParameters(parsedMarkdownFile, paramHeaderIndex + 1);
            commandHelp.Parameters.AddRange(parameters);

            #endregion

			#region Get Aliases
            int aliasHeaderIndex = GetNextHeaderIndex(parsedMarkdownFile.Ast, expectedHeaderLevel: 2, expectedHeaderTitle: "ALIASES");
            if (aliasHeaderIndex != -1)
            {
                var aliases = GetAliases(parsedMarkdownFile.Ast, aliasHeaderIndex + 1);
                commandHelp.Aliases?.AddRange(aliases);
            }
			#endregion

			#region Get Inputs
            int inputHeaderIndex = GetNextHeaderIndex(parsedMarkdownFile.Ast, expectedHeaderLevel: 2, expectedHeaderTitle: "INPUTS");
            if (inputHeaderIndex != -1)
            {
                var inputs = GetInputOutput(parsedMarkdownFile, inputHeaderIndex + 1);
                commandHelp.Inputs?.Add(inputs);
            }
			#endregion

			#region Get Outputs
            int outputHeaderIndex = GetNextHeaderIndex(parsedMarkdownFile.Ast, expectedHeaderLevel: 2, expectedHeaderTitle: "OUTPUTS");
            if (outputHeaderIndex != -1)
            {
                var outputs = GetInputOutput(parsedMarkdownFile, outputHeaderIndex + 1);
                commandHelp.Outputs?.Add(outputs);
            }
			#endregion

			#region Get Notes
            int noteHeaderIndex = GetNextHeaderIndex(parsedMarkdownFile.Ast, expectedHeaderLevel: 2, expectedHeaderTitle: "NOTES");
            if (noteHeaderIndex != -1)
            {
                var notes = GetLinesTillNextHeader(parsedMarkdownFile, 2, noteHeaderIndex + 1);
                commandHelp.Notes = notes;
            }
			#endregion

			#region Get RelatedLinks
            int linkHeaderIndex = GetNextHeaderIndex(parsedMarkdownFile.Ast, expectedHeaderLevel: 2, expectedHeaderTitle: "RELATED LINKS");
            if (linkHeaderIndex != -1)
            {
                var links = GetLinks(parsedMarkdownFile.Ast, linkHeaderIndex + 1);
                commandHelp.RelatedLinks?.AddRange(links);
            }
			#endregion

            return commandHelp;
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

        internal static List<Links> GetLinks(MarkdownDocument md, int startIndex)
        {
            List<Links> links = new List<Links>();

            while (md[startIndex] is not HeadingBlock)
            {
                if (md[startIndex] is ParagraphBlock pb)
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
                }

                startIndex++;
                if (startIndex >= md.Count)
                {
                    break;
                }
            }

            return links;
        }

        internal static InputOutput GetInputOutput(ParsedMarkdownContent md, int startIndex)
        {
            InputOutput ioList = new();

            var nextSectionIndex = GetNextHeaderIndex(md.Ast, expectedHeaderLevel: 2, startIndex: startIndex);
            var currentIndex = startIndex;
            
            for ( ;currentIndex < nextSectionIndex; currentIndex++)
            {
                string inputType = string.Empty;
                if (md.Ast[currentIndex] is HeadingBlock inputOutputHeader)
                {
                    inputType = inputOutputHeader?.Inline?.FirstChild?.ToString() ?? string.Empty;    
                    currentIndex++;
                    string description = GetLinesTillNextHeader(md, -1, currentIndex);
                    ioList.AddInputOutputItem(inputType, description);
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

        internal static Collection<Example> GetExamples(MarkdownDocument md, int startIndex)
        {
            var examples = new Collection<Example>();
            int endExampleIndex = GetNextHeaderIndex(md, expectedHeaderLevel: 2, expectedHeaderTitle: "PARAMETERS", startIndex: startIndex);
            int currentIndex = startIndex;
            while (currentIndex < endExampleIndex)
            {
                string exampleTitle = string.Empty;
                string exampleCode = string.Empty;
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
                        exampleTitle = example;
                    }
                }

                var exampleCodeBlock = GetParameterYamlBlock(md, exampleItemIndex + 1);

                if (exampleCodeBlock != null)
                {
                    exampleCode = exampleCodeBlock.Lines.ToString();
                }

                exampleDescription = GetParagraphsTillNextHeader(md, exampleItemIndex + 1);

                examples.Add(new Example(exampleTitle, exampleCode, exampleDescription));

                currentIndex = exampleItemIndex + 1;
            }
            return examples;
        }

        internal static string GetParameterDescription(ParsedMarkdownContent md, int startIndex)
        {
            StringBuilder? sb = null;

            try
            {
                sb = Constants.StringBuilderPool.Get();
                var descriptionIndex = md.Ast[startIndex + 1].Line;
                int paramYamlBlockIndex = GetParameterYamlBlockIndex(md.Ast, startIndex + 1);
                if (paramYamlBlockIndex == -1)
                {
                    return string.Empty;
                }
                startIndex++;
                for(; descriptionIndex < paramYamlBlockIndex; descriptionIndex++)
                {
                    sb.AppendLine(md.MarkdownLines[descriptionIndex]);
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

                if (parameterItemIndex > nextHeaderLevel2)
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

                if (string.Equals(parameterName, "CommonParameters", StringComparison.OrdinalIgnoreCase))
                {
                    currentIndex = parameterItemIndex + 1;
                    continue;
                }

                string description = GetParameterDescription(markdownContent, parameterItemIndex);

                var paramYamlBlock = GetParameterYamlBlock(md, parameterItemIndex + 1);

                if (paramYamlBlock != null)
                {
                    var yamlBlock = paramYamlBlock.Lines.ToString();

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
                            defaultValuesAsString = defaultValueStr.Trim();
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
                parameter.PipelineInput = string.Equals(acceptedPipelineAsString, Constants.TrueString, StringComparison.OrdinalIgnoreCase);
                parameter.Globbing = string.Equals(acceptWildCardAsString, Constants.TrueString, StringComparison.OrdinalIgnoreCase);

                parameters.Add(parameter);

                currentIndex = parameterItemIndex + 1;
            }

            return parameters;
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

        internal static FencedCodeBlock? GetParameterYamlBlock(MarkdownDocument md, int startIndex)
        {
            while (md[startIndex] is not HeadingBlock)
            {
                if (md[startIndex] is FencedCodeBlock fencedCodeBlock)
                {
                    if (fencedCodeBlock.Info is not null && fencedCodeBlock.Info.StartsWith("yaml", StringComparison.OrdinalIgnoreCase))
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

        internal static bool ValidateMarkdown(MarkdownDocument md)
        {
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
                return false;
            }

            int currentElement = 0;
            foreach(var element in markdownElements)
            {
                var elementIndex = GetNextHeaderIndex(md, expectedHeaderLevel: element.Level, expectedHeaderTitle: element.Name, startIndex: currentElement + 1);
                if (elementIndex == -1)
                {
                    return false;
                }
                if (elementIndex < currentElement)
                {
                    return false;
                }
                currentElement = elementIndex;
            }
            return true;
        }

    }

    public class ParsedMarkdownContent
    {
        public List<string> MarkdownLines { get; set; }
        public MarkdownDocument Ast { get; set; }
        public ParsedMarkdownContent(string fileContent)
        {
            MarkdownLines = new List<string>(fileContent.Replace("\r", "").Split(Constants.LineSplitter));
            Ast = Markdig.Markdown.Parse(fileContent);
        }
    }

    public class MarkdownElement
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public MarkdownElement(string name, int level)
        {
            Name = name;
            Level = level;
        }
    
    }
}
