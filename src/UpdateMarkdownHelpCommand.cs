// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.PowerShell.PlatyPS.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
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
            CommandHelp commandHelp;

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
                string commandName = title.Inline.FirstChild.ToString();
                commandHelp = new CommandHelp(commandName, yamlHeader.ModuleName, cultureInfo: null);
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

            /* No needed as we get infor from parameter details
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

            #region Get Examples

            int examplesHeaderIndex = GetNextHeaderIndex(mdAst, expectedHeaderLevel: 2, expectedHeaderTitle: "EXAMPLES", startIndex: descriptionHeaderIndex + 1);

            commandHelp.

            #endregion Get Examples



            return commandHelp;
        }

        private static string GetParameterSetName(HeadingBlock parameterSetBlock)
        {
            StringBuilder? sb = null;

            try
            {
                sb = Constants.StringBuilderPool.Get();

                sb.Append(parameterSetBlock.Inline.FirstChild);

                var item = parameterSetBlock.Inline.FirstChild.NextSibling;

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
                        if (string.Equals(headerItem.Inline.FirstChild.ToString(), expectedHeaderTitle))
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
                        var item = pb.Inline.FirstChild;

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
