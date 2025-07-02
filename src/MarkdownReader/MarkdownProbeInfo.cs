// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using Microsoft.PowerShell.PlatyPS.Model;

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
        UnknownSchema   = 1 << 0x11,
        V1Schema        = 1 << 0x12,
        V2Schema        = 1 << 0x13,
    }

    /// <summary>
    /// A class to represent the type of Markdown file we are parsing.
    /// </summary>
    ///
    public class MarkdownProbeInfo
    {
        /// <summary>
        /// The path to the markdown file
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// The title of the markdown file.
        /// This might be null if the file cannot be identified
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// The type of the markdown file.
        /// </summary>
        public MarkdownFileType FileType { get; set; }

        /// <summary>
        /// The parsed markdown content.
        /// </summary>
        public ParsedMarkdownContent MarkdownContent { get; set; }

        /// <summary>
        /// The metadata of the file if it can be determined.
        /// </summary>
        public OrderedDictionary? MetaData { get; set; }

        /// <summary>
        /// The diagnostic messages created during the probe.
        /// </summary>
        public List<DiagnosticMessage> DiagnosticMessages { get; set; }

        /// <summary>
        /// The constructor of the probe information.
        /// </summary>
        /// <param name="filePath"></param>
        public MarkdownProbeInfo(string filePath)
        {
            FilePath = filePath;
            DiagnosticMessages = new();
            MarkdownContent = ParsedMarkdownContent.ParseFile(filePath);
            MetaData = MarkdownConverter.GetMetadata(MarkdownContent);
            DetermineContentType();
        }

        /// <summary>
        /// A helper method to easily determine if this is a module file.
        /// </summary>
        /// <returns>boolean true if the file has been determined to be a module file, false if not.</returns>
        public bool IsModuleFile()
        {
            return (FileType & MarkdownFileType.ModuleFile) == MarkdownFileType.ModuleFile;
        }

        /// <summary>
        /// A helper method to easily determine if this is a command help file.
        /// </summary>
        /// <returns>boolean true if the file has been determined to be a command help file, false if not.</returns>
        public bool IsCommandHelp()
        {
            return (FileType & MarkdownFileType.CommandHelp) == MarkdownFileType.CommandHelp;
        }

        public bool IsAboutTopic()
        {
            return (FileType & MarkdownFileType.AboutTopic) == MarkdownFileType.AboutTopic;
        }

        public override string ToString()
        {
            return $"{Title}";
        }

        /// <summary>
        /// Determine the content type by looking at various properties.
        /// This method inspects the metadata, if it exists, as well as the markdown content.
        /// </summary>
        private void DetermineContentType()
        {
            FileType = MarkdownFileType.Unknown;
            if (MetaData is not null && MetaData.Contains("title"))
            {
                Title = MetaData["title"].ToString();
                DiagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Identify, "Found metadata and title", DiagnosticSeverity.Information, "DetermineContentType", -1));
            }
            else if (MetaData is null)
            {
                Title = "unknown";
                DiagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Identify, "metadata is null", DiagnosticSeverity.Information, "DetermineContentType", -1));
                SearchElements();
                return;
            }
            else
            {
                Title = "unknown";
                DiagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Identify, "title not found", DiagnosticSeverity.Information, "DetermineContentType", -1));
            }

            if (MetaData is not null)
            {
                if (MetaData.Contains("document type"))
                {
                    var documentType = MetaData["document type"];
                    if (documentType is not null && string.Compare(documentType.ToString(), "cmdlet", true) == 0)
                    {
                        DiagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Identify, "document type found: cmdlet", DiagnosticSeverity.Information, "DetermineContentType", -1));
                        FileType = (MarkdownFileType.CommandHelp|MarkdownFileType.V2Schema);
                    }
                    else if (documentType is not null && string.Compare(documentType.ToString(), "module", true) == 0)
                    {
                        FileType = (MarkdownFileType.ModuleFile|MarkdownFileType.V2Schema);
                        DiagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Identify, "document type found: modulefile", DiagnosticSeverity.Information, "DetermineContentType", -1));
                    }
                }
                else // hunt for more information
                {
                    DiagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Identify, "document type not found, searching elements", DiagnosticSeverity.Information, "DetermineContentType", -1));
                    SearchElements();
                }
            }
        }

        private void SearchElements()
        {
            var synopsisIndex = MarkdownContent.FindHeader(2, "SYNOPSIS") != -1;
            var syntaxIndex = MarkdownContent.FindHeader(2, "SYNTAX") != -1;
            var exampleIndex = MarkdownContent.FindHeader(2, "EXAMPLES") != -1;
            var hasModuleGuid = MetaData?.Contains("Module Guid") ?? false;

            if (synopsisIndex && syntaxIndex && exampleIndex)
            {
                DiagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Identify, "synopsis/syntax/examples all found", DiagnosticSeverity.Information, "SearchElements", -1));
                FileType = MarkdownFileType.CommandHelp;
            }
            else if (hasModuleGuid)
            {
                DiagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Identify, "module guid found", DiagnosticSeverity.Information, "SearchElements", -1));
                FileType = MarkdownFileType.ModuleFile;
            }
            else if (Title?.StartsWith("about", true, CultureInfo.InvariantCulture) ?? false)
            {
                DiagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Identify, "title starts with 'about'", DiagnosticSeverity.Information, "SearchElements", -1));
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
                DiagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Identify, message, DiagnosticSeverity.Information, "SearchElements", -1));
            }

            if(MetaData?.Contains("PlatyPS schema version") ?? false)
            {
                DiagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Identify, "PlatyPS schema version found - marking as v2", DiagnosticSeverity.Information, "SearchElements", -1));
                FileType |= MarkdownFileType.V2Schema;
            }
            else if (FileType == MarkdownFileType.AboutTopic)
            {
                DiagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Identify, "no schema for about topics", DiagnosticSeverity.Information, "SearchElements", -1));
            }
            else if (FileType != MarkdownFileType.Unknown)
            {
                DiagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Identify, "PlatyPS schema version not found - marking as v1", DiagnosticSeverity.Information, "SearchElements", -1));
                FileType |= MarkdownFileType.V1Schema;
            }
            else if (FileType == MarkdownFileType.Unknown)
            {
                DiagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Identify, "Could not determine schema version", DiagnosticSeverity.Warning, "SearchElements", -1));
                FileType |= MarkdownFileType.UnknownSchema;
            }
        }
    }
}