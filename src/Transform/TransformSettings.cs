// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Globalization;
using System.Management.Automation.Runspaces;

namespace Microsoft.PowerShell.PlatyPS
{
    public class TransformSettings
    {
        public string? FwLink { get; set; }
        public string? HelpVersion { get; set; }
        public CultureInfo Locale { get; set; }
        public Hashtable? Metadata { get; set; }
        public Guid? ModuleGuid { get; set; }
        public string? ModuleName { get; set; }
        public string? OnlineVersionUrl { get; set; }
        public bool? CreateModulePage { get; set; }
        public bool? DoubleDashList { get; set; }
        public bool? UseFullTypeName { get; set; }
        public PSSession? Session { get; set; }
        public bool? ExcludeDontShow { get; set; }

        public TransformSettings()
        {
            Locale = CultureInfo.CurrentUICulture;
        }

        public TransformSettings(CultureInfo cultureInfo)
        {
            Locale = cultureInfo;
        }
    }
}
