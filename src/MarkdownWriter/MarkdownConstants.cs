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
        internal const string MarkdownMetadataHeader = "---";
        internal const string SchemaVersionMarkdown = "schema: 2.0.0";
        internal const string mdSynopsisHeader = "## SYNOPSIS";
        internal const string mdSyntaxHeader = "## SYNTAX";
        internal const string mdDescriptionHeader = "## DESCRIPTION";
        internal const string mdAliasHeader = "## ALIASES";
        internal const string AliasMessage = "This cmdlet has the following aliases,\n  {{Insert list of aliases}}";
        internal const string mdExamplesHeader = "## EXAMPLES";
        internal const string mdParametersHeader = "## PARAMETERS";
        internal const string mdCommonParametersHeader = "### CommonParameters";
        internal const string mdWorkflowCommonParametersHeader = "### WorkflowCommonParameters";
        internal const string mdInputsHeader = "## INPUTS";
        internal const string mdOutputsHeader = "## OUTPUTS";
        internal const string mdNotesHeader = "## NOTES";
        internal const string mdRelatedLinksHeader = "## RELATED LINKS";
        internal const string mdRelatedLinksFmt = "[{0}]({1})";

        internal const string mdExampleItemHeaderTemplate = "### Example {0}: {1}";

        internal const string mdDefaultParameterSetHeaderTemplate = "### {0} (Default)";
        internal const string mdParameterSetHeaderTemplate = "### {0}";
        internal const string CodeBlock = "```";

        internal const string GenericParameterBackTick = "`";
        internal const string GenericParameterTypeNameStart = "<";
        internal const string GenericParameterTypeNameEnd = ">";

        internal const string mdNotesItemHeaderTemplate = "### {0}";

        internal const string mdModulePageModuleNameHeaderTemplate = "# {0} Module";
        internal const string mdModulePageDescriptionHeader = "## Description";
        internal const string mdModulePageCmdletHeaderTemplate = "## {0} Cmdlets";
        internal const string mdModulePageCmdletLinkTemplate = "### [{0}]({1})";

        // THIS IS MARKDOWN
        internal const string mdParameterYamlBlockWithAcceptedValues = @"### -{0}

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

        internal const string mdParameterYamlBlock = @"### -{0}

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
