// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    public partial class MarkdownConverter
    {
        public static ModuleFileInfo GetModuleFileInfoFromMarkdownFile(string path)
        {
            var md = ParsedMarkdownContent.ParseFile(path);
            var moduleFileInfo = GetModuleFileInfoFromMarkdown(md);
            moduleFileInfo.Diagnostics.FileName = path;
            return moduleFileInfo;
        }

        internal static ModuleFileInfo GetModuleFileInfoFromMarkdown(ParsedMarkdownContent markdownContent)
        {
            /*
            GetMetadata
            GetTitleFromMarkdown
            GetDescription
            GetModuleInfo - optional
            GetModuleCommandInfo
            */

            ModuleFileInfo moduleFileInfo = new();
            OrderedDictionary? metadata = GetMetadata(markdownContent.Ast);
            if (metadata is null)
            {
                throw new InvalidDataException("null metadata");
            }

            moduleFileInfo.Metadata = MetadataUtils.FixUpModuleFileMetadata(metadata);
            moduleFileInfo.Title = GetModuleFileTitleFromMarkdown(markdownContent, out List<DiagnosticMessage> titleDiagnostics);
            titleDiagnostics.ForEach(m => moduleFileInfo.Diagnostics.TryAddDiagnostic(m));

            if (MetadataUtils.TryGetGuidFromMetadata(moduleFileInfo.Metadata, "Module Guid", out Guid guid))
            {
                moduleFileInfo.Diagnostics.TryAddDiagnostic(new DiagnosticMessage(DiagnosticMessageSource.Metadata, $"found guid {guid}", DiagnosticSeverity.Information, "GetMetadata", -1));
                moduleFileInfo.ModuleGuid = guid;
            }
            else
            {
                moduleFileInfo.Diagnostics.TryAddDiagnostic(new DiagnosticMessage(DiagnosticMessageSource.Metadata, $"no module guid found.", DiagnosticSeverity.Warning, "GetMetadata", -1));
            }

            if (MetadataUtils.TryGetStringFromMetadata(moduleFileInfo.Metadata, "Module Name", out string name))
            {
                moduleFileInfo.Diagnostics.TryAddDiagnostic(new DiagnosticMessage(DiagnosticMessageSource.Metadata, $"found module name {name}", DiagnosticSeverity.Information, "GetMetadata", -1));
                moduleFileInfo.Module = name;
            }
            else
            {
                moduleFileInfo.Diagnostics.TryAddDiagnostic(new DiagnosticMessage(DiagnosticMessageSource.Metadata, $"no module name found.", DiagnosticSeverity.Warning, "GetMetadata", -1));
            }

            if (MetadataUtils.TryGetStringFromMetadata(moduleFileInfo.Metadata, "Locale", out string locale))
            {
                try
                {
                    moduleFileInfo.Locale = CultureInfo.GetCultureInfo(locale);
                    moduleFileInfo.Diagnostics.TryAddDiagnostic(new DiagnosticMessage(DiagnosticMessageSource.Metadata, $"locale set to {locale}", DiagnosticSeverity.Information, "GetMetadata", -1));
                }
                catch
                {
                    moduleFileInfo.Diagnostics.TryAddDiagnostic(new DiagnosticMessage(DiagnosticMessageSource.Metadata, $"could not set locale to {locale}, using CurrentCulture", DiagnosticSeverity.Warning, "GetMetadata", -1));
                    moduleFileInfo.Locale = CultureInfo.CurrentCulture;
                }
            }

            moduleFileInfo.Description = GetModuleFileDescriptionFromMarkdown(markdownContent, out List<DiagnosticMessage> descriptionDiagnostics);
            descriptionDiagnostics.ForEach(m => moduleFileInfo.Diagnostics.TryAddDiagnostic(m));

            List<DiagnosticMessage> commandGroupDiagnostics;
            var commandGroups = GetCommandGroupsFromMarkdown(markdownContent, out commandGroupDiagnostics);
            if (commandGroups.Count > 0)
            {
                moduleFileInfo.CommandGroups.AddRange(commandGroups);
            }

            commandGroupDiagnostics.ForEach(d => moduleFileInfo.Diagnostics.TryAddDiagnostic(d));
            return moduleFileInfo;
        }

        internal static List<ModuleCommandGroup> GetCommandGroupsFromMarkdown(ParsedMarkdownContent markdownContent, out List<DiagnosticMessage> diagnostics)
        {
            List<ModuleCommandGroup> commandGroups = new();
            diagnostics = new();
            while(!markdownContent.IsEnd())
            {
                var groupStartIndex = markdownContent.FindHeader(2, "");
                if (groupStartIndex == -1)
                {
                    return commandGroups;
                }

                markdownContent.CurrentIndex = groupStartIndex;
                var groupHeader = (HeadingBlock)markdownContent.Ast[groupStartIndex];
                string groupName;
                if (groupHeader?.Inline?.FirstChild is null)
                {
                    groupName = "unknown group";
                }
                else
                {
                    groupName = groupHeader.Inline.FirstChild.ToString();
                }

                var ModuleCommandGroup = new ModuleCommandGroup(groupName);
                List <DiagnosticMessage> moduleCommandDiagnostics;
                List <ModuleCommandInfo> groupModuleCommands = GetModuleFileCommandsFromMarkdown(markdownContent, out moduleCommandDiagnostics);
                if (moduleCommandDiagnostics.Count > 0)
                {
                    diagnostics.AddRange(moduleCommandDiagnostics);
                }

                ModuleCommandGroup.Commands.AddRange(groupModuleCommands);
                commandGroups.Add(ModuleCommandGroup);
            }

            return commandGroups;
        }

        internal static string GetModuleFileTitleFromMarkdown(ParsedMarkdownContent md, out List<DiagnosticMessage> diagnostics)
        {
            diagnostics = new();
            var index = md.FindHeader(1, "");
            if (index == -1)
            {
                diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.ModuleFileTitle, "No title header found", DiagnosticSeverity.Error, "GetModuleFileTitle", -1));
                return string.Empty;
            }

            HeadingBlock titleBlock;
            try
            {
                titleBlock = (HeadingBlock)md.Ast[index];
            }
            catch
            {
                diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.ModuleFileTitle, "No title block found", DiagnosticSeverity.Error, "GetModuleFileTitle", -1));
                return string.Empty;
            }

            var titleString = titleBlock.Inline?.FirstChild?.ToString().Trim(); 
            if (titleString is null)
            {
                diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.ModuleFileTitle, "Null title", DiagnosticSeverity.Error, "GetModuleFileTitle", -1));
            }
            else
            {
                diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.ModuleFileTitle, $"Title {titleString} found", DiagnosticSeverity.Information, "GetModuleFileTitle", titleBlock.Line));
            }

            return titleString ?? string.Empty;
        }

        internal static string GetModuleFileDescriptionFromMarkdown(ParsedMarkdownContent md, out List<DiagnosticMessage> diagnostics)
        {
            diagnostics = new();
            var index = md.FindHeader(2, "Description");
            if (index == -1)
            {
                diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.ModuleFileDescription, "No module description found", DiagnosticSeverity.Warning, "GetModuleFileDescription", -1));
                return string.Empty;
            }

            index++;
            md.CurrentIndex = index;

            int nextHeaderLevel = 2;
            var nextHeader = md.FindHeader(nextHeaderLevel, "");
            if (nextHeader == -1)
            {
                nextHeaderLevel = 3;
            }

            string descriptionString = MarkdownConverter.GetLinesTillNextHeader(md, nextHeaderLevel, index);
            diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.ModuleFileDescription, "Module description found", DiagnosticSeverity.Information, "GetModuleFileDescription", md.Ast[index].Line));
            return descriptionString.Trim();
        }

        /// <summary>
        /// Read the module file looking for 
        /// "### [name](markdownfile.md)
        /// Description
        /// </summary>
        internal static List<ModuleCommandInfo> GetModuleFileCommandsFromMarkdown(ParsedMarkdownContent md, out List<DiagnosticMessage> diagnostics)
        {
            diagnostics = new();
            List<ModuleCommandInfo> list = new();
            var commandListStart = md.FindHeader(3, "");
            if (commandListStart == -1)
            {
                return list;
            }

            md.CurrentIndex = commandListStart;
            var nextGroupIndex = md.FindHeader(2, "");
            if (nextGroupIndex == -1)
            {
                nextGroupIndex = md.Ast.Count;
            }

            while (md.CurrentIndex >= 0 && md.CurrentIndex < nextGroupIndex)
            {
                if (md.GetCurrent() is HeadingBlock moduleCommandInfoLink && moduleCommandInfoLink.Level == 3)
                {
                    md.Take();

                    // The description may be missing, so peek before taking it.
                    ParagraphBlock? moduleCommandInfoDescription = null;
                    if (md.GetCurrent() is ParagraphBlock)
                    {
                        moduleCommandInfoDescription = md.Take() as ParagraphBlock;
                    }

                    var mfci = new ModuleCommandInfo();
                    mfci.Name = ((moduleCommandInfoLink?.Inline?.FirstChild as LinkInline)?.FirstChild as LiteralInline)?.ToString() ?? string.Empty;
                    mfci.Link = (moduleCommandInfoLink?.Inline?.FirstChild as LinkInline)?.Url ?? string.Empty;
                    mfci.Description = moduleCommandInfoDescription is null ? string.Empty : md.MarkdownLines[moduleCommandInfoDescription.Line];
                    list.Add(mfci);
                    diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.ModuleFileCommand, $"command {mfci.Name} found", DiagnosticSeverity.Information, "GetModuleFileCommandsFromMarkdown", moduleCommandInfoLink is null ? -1 : moduleCommandInfoLink.Line));
                }
                else // Not sure what we got but we're going to ignore it
                {
                    var ignoredAst = md.Take() as MarkdownObject;
                    if (ignoredAst is not null)
                    {
                        diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.ModuleFileCommand, $"Ignore index {md.CurrentIndex}", DiagnosticSeverity.Warning, "GetFileCommandsFromMarkdown", ignoredAst.Line ));
                    }
                }
            }

            // Back up as we're now at the next header
            md.UnGet();
            return list;
        }
    }
}
