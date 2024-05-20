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

    public class ModuleFileInfo : IEquatable<ModuleFileInfo>
    {
        public OrderedDictionary Metadata { get; set; }
        public string Title { get; set; }
        [YamlIgnore]
        public string Module { get; set; }
        [YamlIgnore]
        public Guid? ModuleGuid { get; set; }
        public string Description { get; set; }
        [YamlIgnore]
        public CultureInfo Locale { get; set; }
        public string OptionalElement { get; set; }
        public List<ModuleCommandInfo> Commands { get; set; }
        [YamlIgnore]
        public Diagnostics Diagnostics { get; set; }

        public ModuleFileInfo()
        {
            Metadata = new();
            Title = string.Empty;
            Module = string.Empty;
            Description = string.Empty;
            OptionalElement = string.Empty;
            Diagnostics = new();
            Commands = new();
            Locale = CultureInfo.GetCultureInfo("en-US");
        }

        public ModuleFileInfo(string title, string moduleName, CultureInfo? locale)
        {
            Metadata = MetadataUtils.GetModuleFileBaseMetadata(title, moduleName, locale);
            Title = title;
            Module = moduleName;
            Description = string.Empty;
            OptionalElement = string.Empty;
            Diagnostics = new();
            Commands = new();
            Locale = locale ?? CultureInfo.GetCultureInfo("en-US");
        }

        public ModuleFileInfo(PSModuleInfo moduleInfo, CultureInfo? locale)
        {
            Metadata = MetadataUtils.GetModuleFileBaseMetadata(moduleInfo, locale);
            Title = $"{moduleInfo.Name} Module";
            Module = $"{moduleInfo.Name}";
            Description = moduleInfo.Description;
            OptionalElement = string.Empty;
            Diagnostics = new();
            Commands = new();
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
                Commands.SequenceEqual(other.Commands)
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
            moduleFileInfo.Title = GetModuleFileTitleFromMarkdown(markdownContent);
            if (MetadataUtils.TryGetGuidFromMetadata(moduleFileInfo.Metadata, "Module Guid", out Guid guid))
            {
                moduleFileInfo.ModuleGuid = guid;
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

            moduleFileInfo.Description = GetModuleFileDescriptionFromMarkdown(markdownContent);
            var optionalDescription = GetModuleFileOptionalDescriptionFromMarkdown(markdownContent);
            if (optionalDescription is not null)
            {
                moduleFileInfo.OptionalElement = optionalDescription;
            }

            foreach(var moduleCommandInfo in GetModuleFileCommandsFromMarkdown(markdownContent))
            {
                moduleFileInfo.Commands.Add(moduleCommandInfo);
            }

            return moduleFileInfo;
        }

        internal static string GetModuleFileTitleFromMarkdown(ParsedMarkdownContent md)
        {
            var index = md.FindHeader(1, "");
            if (index == -1)
            {
                return string.Empty;
            }

            HeadingBlock titleBlock;
            try
            {
                titleBlock = (HeadingBlock)md.Ast[index];
            }
            catch
            {
                return string.Empty;
            }

            var titleString = titleBlock.Inline?.FirstChild?.ToString().Trim(); 
            return titleString ?? string.Empty;
        }
        internal static string GetModuleFileDescriptionFromMarkdown(ParsedMarkdownContent md)
        {
            var index = md.FindHeader(2, "Description");
            if (index == -1)
            {
                return string.Empty;
            }
            index++;

            int nextHeaderLevel = 2;
            var nextHeader = md.FindHeader(nextHeaderLevel, "");
            if (nextHeader == -1)
            {
                nextHeaderLevel = 3;
            }

            string descriptionString = MarkdownConverter.GetLinesTillNextHeader(md, nextHeaderLevel, index);
            return descriptionString.Trim();
        }
        internal static string GetModuleFileOptionalDescriptionFromMarkdown(ParsedMarkdownContent md)
        {
            return string.Empty;
        }

        /// <summary>
        /// Read the module file looking for 
        /// "### [name](markdownfile.md)
        /// Description
        /// </summary>
        internal static List<ModuleCommandInfo> GetModuleFileCommandsFromMarkdown(ParsedMarkdownContent md)
        {
            List<ModuleCommandInfo> list = new();
            md.CurrentIndex = md.FindHeader(3, "");
            int index = md.CurrentIndex;
            while (!md.IsEnd())
            {
                var moduleCommandInfoLink = md.Take() as HeadingBlock;
                var moduleCommandInfoDescription = md.Take() as ParagraphBlock;
                if (moduleCommandInfoLink is not null && moduleCommandInfoDescription is not null)
                {
                    var mfci = new ModuleCommandInfo();
                    mfci.Name = ((moduleCommandInfoLink?.Inline?.FirstChild as LinkInline)?.FirstChild as LiteralInline)?.ToString() ?? string.Empty;
                    mfci.Link = (moduleCommandInfoLink?.Inline?.FirstChild as LinkInline)?.Url ?? string.Empty;
                    mfci.Description = md.MarkdownLines[moduleCommandInfoDescription.Line] ?? string.Empty;
                    list.Add(mfci);
                }
                // index = md.FindHeader(3, "");
            }

            return list;
        }
    }
}