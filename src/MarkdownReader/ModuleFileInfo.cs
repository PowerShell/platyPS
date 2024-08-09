// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using Microsoft.PowerShell.PlatyPS.Model;
using YamlDotNet.Serialization;
using System.Management.Automation;

namespace Microsoft.PowerShell.PlatyPS
{
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

        public ModuleFileInfo(ModuleFileInfo mfi)
        {
            Metadata = new();
            if (mfi.Metadata is not null)
            {
                foreach (var key in mfi.Metadata.Keys)
                {
                    Metadata[key] = mfi.Metadata[key];
                }
            }

            Title = mfi.Title;
            Module = mfi.Module;
            Locale = mfi.Locale;
            Description = mfi.Description;
            Diagnostics = new();
            Diagnostics.TryAddDiagnostic(new DiagnosticMessage(DiagnosticMessageSource.ModuleFileTitle, "Copied MFI", DiagnosticSeverity.Information, "ModuleFileInfo:Constructor", -1));
            CommandGroups = new();
            foreach(var cg in mfi.CommandGroups)
            {
                CommandGroups.Add(new ModuleCommandGroup(cg));
            }
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
}
