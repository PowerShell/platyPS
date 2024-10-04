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
        internal const string YamlHeader = "---";
        // This should probably be "schema: {0}"
        internal const string SchemaVersionYaml = "PlatyPS schema version: 2024-05-01";

        internal const string YamlExtension = "yml";
        internal const string Example1 = "Example 1";
        internal const string ModuleNameHeaderTemplate = "Module Name: {0}";
        internal const string DownloadHelpLinkTitle = "HelpInfoUri: ";
        internal const string HelpVersionTitle = "Help Version: ";
        internal const string LocaleTemplate = "Locale: {0}";
        internal const string ModuleGuidHeaderTemplate = "Module Guid: {0}";
        internal const string SynopsisYamlHeader = "synopsis:";
        internal const string SyntaxYamlHeader = "syntaxes:";
        internal const string DescriptionYamlHeader = "description:";
        internal const string ExamplesYamlHeader = "examples:";
        internal const string ParametersYamlHeader = "parameters:";
        internal const string CommonParametersYamlHeader = "- name: CommonParameters";
        internal const string OutputsYamlHeader = "outputs:";
        internal const string InputsYamlHeader = "inputs:";
        internal const string NotesYamlHeader = "notes:";
        internal const string RelatedLinksYamlHeader = "links:";
        internal const string SyntaxYamlTemplate = "- >-";
        internal const string DefaultSyntaxYamlTemplate = "- >-";
        internal const string RelatedLinksYamlFmt = "placeholder";
        internal const string yamlExampleItemHeaderTemplate = "placeholder";
        internal const string yamlNotesItemHeaderTemplate = "placeholder";
        internal const string yamlModulePageModuleNameHeaderTemplate = "placeholder";
        internal const string yamlModulePageDescriptionHeader = "placeholder";
        internal const string yamlModulePageCmdletLinkTemplate = "placeholder";
        internal const string yamlAliasHeader = "aliases:";
        internal const string yamlParameterYamlBlockWithAcceptedValues = "placeholder -{0}";
    }
}


