// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.PowerShell.PlatyPS.MarkdownWriter;
using Microsoft.PowerShell.PlatyPS.Model;
using YamlDotNet;
using YamlDotNet.Serialization;
using System.Management.Automation;
using System.Security.Cryptography;
using System.Data.Common;

namespace Microsoft.PowerShell.PlatyPS
{
    public class ModuleCommandInfo : IEquatable<ModuleCommandInfo>
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public ModuleCommandInfo()
        {
            Name = string.Empty;
            Link = string.Empty;
            Description = string.Empty;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Link.GetHashCode() ^ Description.GetHashCode();
        }

        public bool Equals(ModuleCommandInfo other)
        {
            if (other is null)
            {
                return false;
            }

            return string.Compare(Name, other.Name) == 0 &&
                string.Compare(Link, other.Link) == 0 &&
                string.Compare(Description, other.Description) == 0;
        }

        public override bool Equals(object other)
        {
            if (other is null)
            {
                return false;
            }

            if (other is ModuleCommandInfo info2)
            {
                return Equals(info2);
            }

            return false;
        }

        public static bool operator == (ModuleCommandInfo info1, ModuleCommandInfo info2)
        {
            if (info1 is not null && info2 is not null)
            {
                return info1.Equals(info2);
            }

            return false;
        }

        public static bool operator !=(ModuleCommandInfo info1, ModuleCommandInfo info2)
        {
            if (info1 is not null && info2 is not null)
            {
                return ! info1.Equals(info2);
            }

            return false;
        }

    }

    public class ModuleCommandGroup : IEquatable<ModuleCommandGroup>
    {
        public string GroupTitle { get; set; }
        public List<ModuleCommandInfo> Commands { get; set; }

        public ModuleCommandGroup()
        {
            GroupTitle = string.Empty;
            Commands = new();
        }

        public ModuleCommandGroup(string title)
        {
            GroupTitle = title;
            Commands = new();
        }

       public override int GetHashCode()
        {
            return (GroupTitle, Commands).GetHashCode();
        }

        public bool Equals(ModuleCommandGroup other)
        {
            if (other is null)
            {
                return false;
            }

            return (
                string.Compare(GroupTitle, other.GroupTitle, StringComparison.CurrentCultureIgnoreCase) == 0 &&
                Commands.SequenceEqual(other.Commands)
                );
        }

        public override bool Equals(object other)
        {
            if (other is null)
            {
                return false;
            }

            if (other is ModuleCommandGroup group2)
            {
                return Equals(group2);
            }

            return false;
        }

        public static bool operator == (ModuleCommandGroup group1, ModuleCommandGroup group2)
        {
            if (group1 is not null && group2 is not null)
            {
                return group1.Equals(group2);
            }

            return false;
        }

        public static bool operator !=(ModuleCommandGroup group1, ModuleCommandGroup group2)
        {
            if (group1 is not null && group2 is not null)
            {
                return ! group1.Equals(group2);
            }

            return false;
        }
    }

    public class ModuleFileInfo : IEquatable<ModuleFileInfo>
    {
        public SortedDictionary<string, string> Metadata { get; set; }
        public string Title { get; set; }
        [YamlIgnore]
        public string Module { get; set; }
        [YamlIgnore]
        public Guid? ModuleGuid { get; set; }
        public string Description { get; set; }
        [YamlIgnore]
        public CultureInfo Locale { get; set; }
        public List<ModuleCommandGroup> CommandGroups { get; set; }
        [YamlIgnore]
        public Diagnostics Diagnostics { get; set; }

        public ModuleFileInfo()
        {
            Metadata = new();
            Title = string.Empty;
            Module = string.Empty;
            Description = string.Empty;
            Diagnostics = new();
            CommandGroups = new();
            Locale = CultureInfo.GetCultureInfo("en-US");
        }

        public ModuleFileInfo(string title, string moduleName, CultureInfo? locale)
        {
            Metadata = MetadataUtils.GetModuleFileBaseMetadata(title, moduleName, locale);
            Title = title;
            Module = moduleName;
            Description = string.Empty;
            Diagnostics = new();
            CommandGroups = new();
            Locale = locale ?? CultureInfo.GetCultureInfo("en-US");
        }

        public ModuleFileInfo(PSModuleInfo moduleInfo, CultureInfo? locale)
        {
            Metadata = MetadataUtils.GetModuleFileBaseMetadata(moduleInfo, locale);
            Title = $"{moduleInfo.Name} Module";
            Module = $"{moduleInfo.Name}";
            Description = moduleInfo.Description;
            Diagnostics = new();
            CommandGroups = new();
            Locale = locale ?? CultureInfo.GetCultureInfo("en-US");
        }

        public override int GetHashCode()
        {
            return (Metadata, Title, Description).GetHashCode();
        }

        public bool Equals(ModuleFileInfo other)
        {
            if (other is null)
            {
                return false;
            }

            return (
                string.Compare(Title, other.Title, StringComparison.CurrentCultureIgnoreCase) == 0 &&
                string.Compare(Module, other.Module, StringComparison.CurrentCultureIgnoreCase) == 0 &&
                string.Compare(Description, other.Description, StringComparison.CurrentCultureIgnoreCase) == 0 &&
                CommandGroups.SequenceEqual(other.CommandGroups)
                );
        }

        public override bool Equals(object other)
        {
            if (other is null)
            {
                return false;
            }

            if (other is ModuleFileInfo info2)
            {
                return Equals(info2);
            }

            return false;
        }

        public static bool operator == (ModuleFileInfo info1, ModuleFileInfo info2)
        {
            if (info1 is not null && info2 is not null)
            {
                return info1.Equals(info2);
            }

            return false;
        }

        public static bool operator !=(ModuleFileInfo info1, ModuleFileInfo info2)
        {
            if (info1 is not null && info2 is not null)
            {
                return ! info1.Equals(info2);
            }

            return false;
        }

    }

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
            GetMetadataFromMarkdown
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
                moduleFileInfo.Diagnostics.TryAddDiagnostic(new DiagnosticMessage());
                moduleFileInfo.ModuleGuid = guid;
            }
            else
            {

            }

            if (MetadataUtils.TryGetStringFromMetadata(moduleFileInfo.Metadata, "Module Name", out string name))
            {
                moduleFileInfo.Module = name;
            }

            if (MetadataUtils.TryGetStringFromMetadata(moduleFileInfo.Metadata, "Locale", out string locale))
            {
                try
                {
                    moduleFileInfo.Locale = CultureInfo.GetCultureInfo(locale);
                }
                catch
                {
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
            diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.ModuleFileDescription, "Module description found", DiagnosticSeverity.Warning, "GetModuleFileDescription", md.Ast[index].Line));
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
