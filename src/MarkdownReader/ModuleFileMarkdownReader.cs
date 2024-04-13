// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
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

            return (Name == other.Name && Link == other.Link && Description == other.Link);
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
        public string Module { get; set; }
        public string Description { get; set; }
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

            moduleFileInfo.Metadata = metadata;
            moduleFileInfo.Title = GetModuleFileTitleFromMarkdown(markdownContent.Ast);
            moduleFileInfo.Description = GetModuleFileDescriptionFromMarkdown(markdownContent.Ast);
            var optionalDescription = GetModuleFileOptionalDescriptionFromMarkdown(markdownContent.Ast);
            if (optionalDescription is not null)
            {
                moduleFileInfo.OptionalElement = optionalDescription;
            }

            foreach(var moduleCommandInfo in GetModuleFileCommandsFromMarkdown(markdownContent.Ast))
            {
                moduleFileInfo.Commands.Add(moduleCommandInfo);
            }

            return moduleFileInfo;
        }

        internal static string GetModuleFileTitleFromMarkdown(MarkdownDocument ast)
        {
            return string.Empty;
        }
        internal static string GetModuleFileDescriptionFromMarkdown(MarkdownDocument ast)
        {
            return string.Empty;
        }
        internal static string GetModuleFileOptionalDescriptionFromMarkdown(MarkdownDocument ast)
        {
            return string.Empty;
        }
        internal static List<ModuleCommandInfo> GetModuleFileCommandsFromMarkdown(MarkdownDocument ast)
        {
            List<ModuleCommandInfo> list = new();
            return list;
        }
    }
}