// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.PowerShell.PlatyPS.Model
{
    // MARKDOWN CONSTANTS
    internal static partial class Constants
    {
        internal const string SynopsisMdHeader = "## SYNOPSIS";
        internal const string SyntaxMdHeader = "## SYNTAX";
        internal const string DescriptionMdHeader = "## DESCRIPTION";
        internal const string ExamplesMdHeader = "## EXAMPLES";
        internal const string ParametersMdHeader = "## PARAMETERS";
        internal const string InputsMdHeader = "## INPUTS";
        internal const string OutputsMdHeader = "## OUTPUTS";
        internal const string NotesMdHeader = "## NOTES";
        internal const string RelatedLinksMdHeader = "## RELATED LINKS";

        internal const string ExampleItemHeaderTemplate = "### Example {0}: {1}";

        internal const string ParameterSetHeaderDefaultTemplate = "### {0} (Default)";
        internal const string ParameterSetHeaderTemplate = "### {0}";
        internal const string CodeBlock = "```";

        internal const string GenericParameterBackTick = "`";
        internal const string GenericParameterTypeNameStart = "<";
        internal const string GenericParameterTypeNameEnd = ">";

        internal const string NotesItemHeaderTemplate = "### {0}";

        internal const string ModulePageModuleNameHeaderTemplate = "# {0} Module";
        internal const string ModulePageDescriptionHeader = "## Description";
        internal const string ModulePageCmdletHeaderTemplate = "## {0} Cmdlets";
        internal const string ModulePageCmdletLinkTemplate = "### [{0}]({1})";

        // THIS IS MARKDOWN
        internal const string ParameterYmlBlockWithAcceptedValues = @"### -{0}

{1}

```yaml
Type: {2}
Parameter Sets: {3}
Aliases: {4}
Accepted values: {5}

Required: {6}
Position: {7}
Default value: {8}
Accept pipeline input: {9}
Accept wildcard characters: {10}
DontShow: {11}
```";

        internal const string ParameterYmlBlock = @"### -{0}

{1}

```yaml
Type: {2}
Parameter Sets: {3}
Aliases: {4}

Required: {5}
Position: {6}
Default value: {7}
Accept pipeline input: {8}
Accept wildcard characters: {9}
DontShow: {10}
```";
    }
}
