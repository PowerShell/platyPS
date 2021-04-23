using System;
using System.Collections;
using System.Globalization;
using System.Management.Automation.Runspaces;

namespace Microsoft.PowerShell.PlatyPS
{
    internal class TransformSettings
    {
        internal string? FwLink { get; set; }
        internal string? HelpVersion { get; set; }
        internal CultureInfo Locale { get; set; }
        internal Hashtable? Metadata { get; set; }
        internal Guid? ModuleGuid { get; set; }
        internal string? ModuleName { get; set; }
        internal string? OnlineVersionUrl { get; set; }
        internal bool? CreateModulePage { get; set; }
        internal bool? DoubleDashList { get; set; }
        internal bool? AlphabeticParamsOrder { get; set; }
        internal bool? UseFullTypeName { get; set; }
        internal PSSession? Session { get; set; }
        internal bool? ExcludeDontShow { get; set; }

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
