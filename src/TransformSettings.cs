using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.PowerShell.PlatyPS
{
    internal class TransformSettings
    {
        internal string FwLink { get; set; }
        internal string HelpVersion { get; set; }
        internal CultureInfo Locale { get; set; }
        internal Hashtable Metadata { get; set; }
        internal Guid? ModuleGuid { get; set; }
        internal string ModuleName { get; set; }
        internal string OnlineVersionUrl { get; set; }
        internal bool CreateModulePage { get; set; }
        internal bool DoubleDashList { get; set; }
        internal bool AlphabeticParamsOrder { get; set; }
        internal bool UseFullTypeName { get; set; }
        internal PSSession Session { get; set; }
        internal bool ExcludeDontShow { get; set; }
    }
}
