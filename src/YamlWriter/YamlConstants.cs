// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.PowerShell.PlatyPS.Model
{
    // YAML CONSTANTS
    internal static partial class Constants
    {
        internal const string YmlHeader = "---";
        // This should probably be "schema: {0}"
        internal const string SchemaVersionYml = "schema: 2.0.0";

        internal const string Example1 = "Example 1";
        internal const string ModuleNameHeaderTemplate = "Module Name: {0}";
        internal const string DownladHelpLinkTitle = "Download Help Link: ";
        internal const string HelpVersionTitle = "Help Version: ";
        internal const string LocaleTemplate = "Locale: {0}";
        internal const string ModuleGuidHeaderTemplate = "Module Guid: {0}";
    }
}
