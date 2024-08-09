// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// using Markdig.Extensions.CustomContainers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Microsoft.PowerShell.PlatyPS.Model;
using YamlDotNet.Core;

namespace Microsoft.PowerShell.PlatyPS
{
    [Flags]
    public enum MarkdownFileType
    {
        Unknown         = 1 << 0x01,
        CommandHelp     = 1 << 0x02,
        ModuleFile      = 1 << 0x03,
        ConceptualTopic = 1 << 0x04,
        AboutTopic      = 1 << 0x05,
        UnknownSchema   = 1 << 0x10,
        V1Schema        = 1 << 0x20,
        V2Schema        = 1 << 0x30,
    }

    /// <summary>
    /// A class to represent the type of Markdown file we are parsing.
    /// </summary>
    ///
    public class MarkdownProbeInfo
    {
        public string FilePath { get; set; }
        public string? Title { get; set; }

        public MarkdownFileType FileType { get; set; }

        public ParsedMarkdownContent MarkdownContent { get; set; }

        public OrderedDictionary? MetaData { get; set; }

        public List<DiagnosticMessage> DiagnosticMessages { get; set; }

        public MarkdownProbeInfo(string filePath)
        {
            FilePath = filePath;
            DiagnosticMessages = new();
            MarkdownContent = ParsedMarkdownContent.ParseFile(filePath);
            MetaData = MarkdownConverter.GetMetadata(MarkdownContent.Ast);
            GetContentType();
        }

        private void GetContentType()
        {
            FileType = MarkdownFileType.Unknown;
            if (MetaData is not null && MetaData.Contains("title"))
            {
                Title = MetaData["title"].ToString();
                DiagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Identify, "Found metadata and title", DiagnosticSeverity.Information, "GetContentType", -1));
            }
            else if (MetaData is null)
            {
                Title = "unknown";
                DiagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Identify, "metadata is null", DiagnosticSeverity.Information, "GetContentType", -1));
            }
            else
            {
                Title = "unknown";
                DiagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Identify, "title not found", DiagnosticSeverity.Information, "GetContentType", -1));
            }

            if (MetaData is not null)
            {
                if (MetaData.Contains("document type"))
                {
                    var documentType = MetaData["document type"];
                    if (documentType is not null && string.Compare(documentType.ToString(), "cmdlet", true) == 0)
                    {
                        DiagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Identify, "document type found: cmdlet", DiagnosticSeverity.Information, "GetContentType", -1));
                        FileType = (MarkdownFileType.CommandHelp|MarkdownFileType.V2Schema);
                    }
                    else if (documentType is not null && string.Compare(documentType.ToString(), "module", true) == 0)
                    {
                        FileType = (MarkdownFileType.ModuleFile|MarkdownFileType.V2Schema);
                        DiagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Identify, "document type found: modulefile", DiagnosticSeverity.Information, "GetContentType", -1));
                    }
                }
                else // hunt for more information
                {
                    DiagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Identify, "document type not found, searching elements", DiagnosticSeverity.Information, "GetContentType", -1));
                    var synopsisIndex = MarkdownContent.FindHeader(2, "SYNOPSIS") != -1;
                    var syntaxIndex = MarkdownContent.FindHeader(2, "SYNTAX") != -1;
                    var exampleIndex = MarkdownContent.FindHeader(2, "EXAMPLES") != -1;
                    var hasModuleGuid = MetaData.Contains("Module Guid");
                    if (synopsisIndex && syntaxIndex && exampleIndex)
                    {
                        DiagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Identify, "synopsis/syntax/examples all found", DiagnosticSeverity.Information, "GetContentType", -1));
                        FileType = MarkdownFileType.CommandHelp;
                    }
                    else if (hasModuleGuid)
                    {
                        DiagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Identify, "module guid found", DiagnosticSeverity.Information, "GetContentType", -1));
                        FileType = MarkdownFileType.ModuleFile;
                    }
                    else if (Title.StartsWith("about", true, CultureInfo.InvariantCulture))
                    {
                        DiagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Identify, "title starts with 'about'", DiagnosticSeverity.Information, "GetContentType", -1));
                        FileType = MarkdownFileType.AboutTopic;
                    }
                    else
                    {
                        List<string> missingElements = new();
                        if (! synopsisIndex)
                        {
                            missingElements.Add("SYNOPSIS");
                        }

                        if (! syntaxIndex)
                        {
                            missingElements.Add("SYNTAX");
                        }

                        if (! exampleIndex)
                        {
                            missingElements.Add("EXAMPLES");
                        }

                        if (! hasModuleGuid)
                        {
                            missingElements.Add("ModuleGuid");
                        }

                        string message = string.Format("Cannot determine file type: missing element(s): {0}", string.Join(", ", missingElements));
                        DiagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Identify, message, DiagnosticSeverity.Information, "GetContentType", -1));
                    }

                    if(MetaData.Contains("PlatyPS schema version"))
                    {
                        DiagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Identify, "PlatyPS schema version found - marking as v2", DiagnosticSeverity.Information, "GetContentType", -1));
                        FileType |= MarkdownFileType.V2Schema;
                    }
                    else if (FileType == MarkdownFileType.AboutTopic)
                    {
                        DiagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Identify, "no schema for about topics", DiagnosticSeverity.Information, "GetContentType", -1));
                    }
                    else if (FileType != MarkdownFileType.Unknown)
                    {
                        DiagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Identify, "PlatyPS schema version not found - marking as v1", DiagnosticSeverity.Information, "GetContentType", -1));
                        FileType |= MarkdownFileType.V1Schema;
                    }
                    else if (FileType == MarkdownFileType.Unknown)
                    {
                        DiagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Identify, "Could not determine schema version", DiagnosticSeverity.Warning, "GetContentType", -1));
                        FileType |= MarkdownFileType.UnknownSchema;
                    }
                }
            }
        }
    }
}