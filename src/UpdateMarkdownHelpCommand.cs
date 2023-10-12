// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Markdig.Extensions.CustomContainers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.PowerShell.PlatyPS.Model;
using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Cmdlet to generate the markdown help for commands, all commands in a module or from a MAML file.
    /// </summary>
    [Cmdlet(VerbsData.Update, "MarkdownHelp", HelpUri = "https://go.microsoft.com/fwlink/?LinkID=2096483")]
    [OutputType(typeof(FileInfo[]))]
    public sealed class UpdateMarkdownHelpCommand : PSCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        [SupportsWildcards()]
        public string[] Path { get; set; } = Array.Empty<string>();

        [Parameter()]
        public System.Text.Encoding Encoding { get; set; } = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        [Parameter()]
        public PSSession? Session { get; set; }

        [Parameter()]
        public SwitchParameter AlphabeticParamsOrder { get; set; } = true;

        [Parameter()]
        public SwitchParameter UseFullTypeName { get; set; }

        [Parameter()]
        public SwitchParameter UpdateInputOutput { get; set; }

        [Parameter()]
        public SwitchParameter Force { get; set; }

        [Parameter()]
        public SwitchParameter ExcludeDontShow { get; set; }

        #endregion

        private class YamlMetadataHeader
        {
            public string ExternalHelp { get; private set; }

            public string ModuleName { get; private set; }

            public string OnlineVersion { get; private set; }

            public string Schema { get; private set; }

            public YamlMetadataHeader(string externalHelp, string moduleName, string onlineVersion, string schema)
            {
                ExternalHelp = externalHelp;
                ModuleName = moduleName;
                OnlineVersion = onlineVersion;
                Schema = schema;
            }
        }

        protected override void EndProcessing()
        {
            TransformSettings settings = new TransformSettings
            {
                AlphabeticParamsOrder = AlphabeticParamsOrder,
                ExcludeDontShow = ExcludeDontShow
            };

            foreach (string filePath in Path)
            {
                Collection<PathInfo> resolvedPaths = this.SessionState.Path.GetResolvedPSPathFromPSPath(filePath);

                foreach (var resolvedPath in resolvedPaths)
                {
                    string commandName = System.IO.Path.GetFileNameWithoutExtension(resolvedPath.Path);

                    Collection<CommandHelp> currentCommand = new TransformCommand(settings).Transform(new string[] { commandName });

                    if (currentCommand is not null && currentCommand.Count != 1) {
                        throw new CommandNotFoundException($"Command not found: {commandName}");
                    }

                    string fileContent = File.ReadAllText(resolvedPath.Path);

                    CommandHelp mdHelp = GetCommandHelpFromMarkdown(fileContent);
                }
            }
        }

        private static YamlMetadataHeader GetYamlHeader(string fileContent)
        {
            string yamlHeader = MarkdownUtilities.GetMarkdownMetadataHeaderReader(fileContent);

            StringReader stringReader = new StringReader(yamlHeader);
            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder().Build();
            var yamlObject = deserializer.Deserialize(stringReader);

            if (yamlObject is Dictionary<object, object> metadataHeader)
            {
                object schemaVersionObj;
                object externalFileObj;
                object moduleVerObj;
                object onlineVerObj;

                if (metadataHeader.TryGetValue("schema", out schemaVersionObj) && schemaVersionObj is string schemaVersion)
                {
                    if (!string.Equals(schemaVersion, "2.0.0", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new ParseException("Schema is not 2.0.0");
                    }
                    else
                    {
                        string onlineVersion = string.Empty;
                        string moduleName = string.Empty;
                        string externalHelpFile = string.Empty;

                        if (metadataHeader.TryGetValue("external help file", out externalFileObj) && externalFileObj is string externalFile)
                        {
                            externalHelpFile = externalFile ?? string.Empty;
                        }

                        if (metadataHeader.TryGetValue("Module Name", out moduleVerObj) && moduleVerObj is string moduleNameStr)
                        {
                            if (string.IsNullOrEmpty(moduleNameStr))
                            {
                                throw new ParseException("Module name cannot be empty");
                            }
                            else
                            {
                                moduleName = moduleNameStr;
                            }
                        }

                        if (metadataHeader.TryGetValue("online version", out onlineVerObj) && onlineVerObj is string onlineVer)
                        {
                            if (string.IsNullOrEmpty(onlineVer))
                            {
                                throw new ParseException("Module name cannot be empty");
                            }
                            else
                            {
                                onlineVersion = onlineVer;
                            }
                        }

                        return new YamlMetadataHeader(externalHelpFile, moduleName, onlineVersion, schemaVersion);
                    }
                }
            }

            throw new InvalidDataException("Yaml header incorrect");
        }

        private static CommandHelp GetCommandHelpFromMarkdown(string fileContent)
        {
            CommandHelp commandHelp = new CommandHelp("x", "y", cultureInfo: null);

            YamlMetadataHeader yamlHeader = GetYamlHeader(fileContent);

            var mdAst = Markdig.Markdown.Parse(fileContent);

            if (mdAst is null)
            {
                throw new ParseException("File content cannot be parsed as markdown");
            }

            #region Get Command Name

            int titleHeaderIndex = GetNextHeaderIndex(mdAst, 1);

            if (mdAst[titleHeaderIndex] is HeadingBlock title)
            {
                string? commandName = title?.Inline?.FirstChild?.ToString();

                if (commandName is not null)
                {
                    commandHelp = new CommandHelp(commandName, yamlHeader.ModuleName, cultureInfo: null);
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

            #endregion

            #region Get Synopsis

            int synopsisHeaderIndex = GetNextHeaderIndex(mdAst, expectedHeaderLevel: 2, expectedHeaderTitle: "SYNOPSIS", startIndex: titleHeaderIndex + 1);
            commandHelp.Synopsis = GetParagraphsTillNextHeader(mdAst, synopsisHeaderIndex + 1);

            #endregion Get Synopsis

            /* Not needed as we get info from parameter details. Also not needed as it is auto generated.
            #region Get Syntax
            int syntaxHeaderIndex = GetNextHeaderIndex(mdAst, expectedHeaderLevel: 2, expectedHeaderTitle: "SYNTAX", startIndex: synopsisHeaderIndex + 1);

            int parameterSetIndex = GetNextHeaderIndex(mdAst, expectedHeaderLevel: 3, startIndex: synopsisHeaderIndex + 1);
            if (mdAst[parameterSetIndex] is HeadingBlock parameterSet)
            {
                string parameterSetName = GetParameterSetName(parameterSet);
                string syntaxLine = string.Empty;

                if (mdAst[parameterSetIndex + 1] is FencedCodeBlock fcb)
                {
                    syntaxLine = fcb.Lines.ToString();
                }

            }
            #endregion Get Syntax
            */

            #region Get Description

            int descriptionHeaderIndex = GetNextHeaderIndex(mdAst, expectedHeaderLevel: 2, expectedHeaderTitle: "DESCRIPTION", startIndex: synopsisHeaderIndex + 1);
            commandHelp.Description = GetParagraphsTillNextHeader(mdAst, descriptionHeaderIndex + 1);

            #endregion Get Description

            #region Get Parameters

            int paramHeaderIndex = GetNextHeaderIndex(mdAst, expectedHeaderLevel: 2, expectedHeaderTitle: "PARAMETERS");

            var parameters = GetParameters(mdAst, paramHeaderIndex + 1);

            #endregion





            return commandHelp;
        }

        private static string GetParameterSetName(HeadingBlock parameterSetBlock)
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


        private static int GetNextHeaderIndex(MarkdownDocument md, int expectedHeaderLevel, string? expectedHeaderTitle = null, int startIndex = 0)
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
            }

            return -1;
        }

        private static Collection<Parameter> GetParameters(MarkdownDocument md, int startIndex)
        {
            var parameters = new Collection<Parameter>();

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

        private static Collection<ParagraphBlock> GetParagraphsTillNextHeaderBlock(MarkdownDocument md, int startIndex)
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

        private static FencedCodeBlock? GetParameterYamlBlock(MarkdownDocument md, int startIndex)
        {
            while (md[startIndex] is not HeadingBlock)
            {
                if (md[startIndex] is FencedCodeBlock fencedCodeBlock)
                {
                    return fencedCodeBlock;
                }

                startIndex++;
            }

            return null;
        }

        private static string GetParagraphsTillNextHeader(MarkdownDocument md, int startIndex)
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

        private static string RemoveLineEndings(string line)
        {
            return line.TrimEnd('\r', '\n');
        }
    }
}
