using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.PowerShell.PlatyPS.Model
{
    internal static class Constants
    {
        internal const string YmlHeader = "---";
        internal const string SchemaVersionYml = "schema: 2.0.0";
        internal const string SynopsisMdHeader = "## SYNOPSIS";
        internal const string SyntaxMdHeader = "## SYNTAX";
        internal const string DescriptionMdHeader = "## DESCRIPTION";
        internal const string ExamplesMdHeader = "## EXAMPLES";
        internal const string ParametersMdHeader = "## PARAMETERS";
        internal const string CommonParameters = @"
### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).
";
        internal const string InputsMdHeader = "## INPUTS";
        internal const string OutputsMdHeader = "## OUTPUTS";
        internal const string NotesMdHeader = "## NOTES";
        internal const string RelatedLinksMdHeader = "## RELATED LINKS";

        internal const string ExampleItemHeaderTemplate = "### Example {0}: {1}";

        internal const string RequiredParamTemplate = "-{0} <{1}>";
        internal const string OptionalParamTemplate = "[-{0} <{1}>]";
        internal const string RequiredPositionalParamTemplate = "[-{0}] <{1}>";
        internal const string OptionalPositionalParamTemplate = "[[-{0} <{1}>]]";
        internal const string OptionalSwitchParamTemplate = "[-{0}]";
        internal const string RequiredSwitchParamTemplate = "-{0}";


        internal const char   SingleSpace = ' ';
        internal const string ParameterSetHeaderDefaultTemplate = "### {0} (Default)";
        internal const string ParameterSetHeaderTemplate = "### {0}";
        internal const string CodeBlock = "```";

        internal const string TrueString = "True";
        internal const string FalseString = "False";
        internal const string NoneString = "None";
        internal const string NamedString = "Named";
        internal const string ParameterSetsAll = "(All)";
        internal const string ParameterSetsAllForRequired = "All";
        internal const string DelimiterComma = ", ";
        internal const string SystemObjectTypename = "System.Object";
        internal const string SyntaxCommonParameters = "[<CommonParameters>]";

        internal const string RequiredParameterSetsTemplate = "True ({0}) False ({1})";
        internal const string NotesItemHeaderTemplate = "### {0}";

        internal const string FillInDescription = "{{ Fill in the Description }}";
        internal const string FillInSynopsis = "{{ Fill in the Synopsis }}";
        internal const string FillInNotes= "{{ Fill in the Notes }}";

        internal const string FillInExampleTitle = "Example 1";
        internal const string FillInExampleCode = @"PS C:\> {{ Add example code here }}";
        internal const string FillInExampleDescription = "{{ Add example description here }}";
        internal const string FillInParameterDescriptionTemplate = "{{ Fill {0} Description }}";
        internal const string FillInReleatedLinks = "{{ Fill in the related links here }}";

        internal const string MamlCommandCommandTag = "command:command";
        internal const string MamlCommandNameTag = "command:name";
        internal const string MamlDescriptionTag = "maml:description";
        internal const string MamlParaTag = "maml:para";
        internal const string MamlSyntaxTag = "command:syntax";
        internal const string MamlSyntaxItemTag = "command:syntaxItem";
        internal const string MamlNameTag = "maml:name";
        internal const string MamlCommandParameterTag = "command:parameter";
        internal const string MamlCommandParameterValueTag = "command:parameterValue";
        internal const string MamlDevTypeTag = "dev:type";
        internal const string MamlDevDefaultValueTag = "dev:defaultValue";
        internal const string MamlCommandParameterValueGroupTag = "command:parameterValueGroup";
        internal const string UnnamedParameterSetTemplate = "UNNAMED_PARAMETER_SET_{0}";
        internal const string MamlCommandParametersTag = "command:parameters";
        internal const string MamlCommandInputTypesTag = "command:inputTypes";
        internal const string MamlCommandInputTypeTag = "command:inputType";
        internal const string MamlCommandReturnValuesTag = "command:returnValues";
        internal const string MamlCommandReturnValueTag = "command:returnValue";
        internal const string MamlAlertSetTag = "maml:alertSet";
        internal const string MamlAlertTag = "maml:alert";
        internal const string MamlCommandExamplesTag = "command:examples";
        internal const string MamlCommandExampleTag = "command:example";
        internal const string MamlTitleTag = "maml:title";
        internal const string MamlDevCodeTag = "dev:code";
        internal const string MamlDevRemarksTag = "dev:remarks";
        internal const string MamlCommandRelatedLinksTag = "command:relatedLinks";
        internal const string MamlNavigationLinkTag = "maml:navigationLink";
        internal const string MamlLinkTextTag = "maml:linkText";
        internal const string MamlUriTag = "maml:uri";
        internal const string Example1 = "Example 1";
        internal const string MamlFileExtensionSuffix = "-help.xml";
        internal const string ModuleNameHeaderTemplate = "Module Name: {0}";
        internal const string DownladHelpLinkTitle = "Download Help Link: ";
        internal const string FillDownloadHelpLink = "{{ Update Download Link}}";
        internal const string HelpVersionTitle = "Help Version: ";
        internal const string FillHelpVersion = "{{ Please enter version of help manually (X.X.X.X) format }}";
        internal const string LocaleTemplate = "Locale: {0}";
        internal const string ModulePageModuleNameHeaderTemplate = "# {0} Module";
        internal const string ModulePageDescriptionHeader = "## Description";
        internal const string ModulePageCmdletHeaderTemplate = "## {0} Cmdlets";
        internal const string ModulePageCmdletLinkTemplate = "### [{0}]({1})";
        internal const string ModuleGuidHeaderTemplate = "Module Guid: {0}";
        internal const string FillInGuid = "XXXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX";
        internal const string LocaleEnUs = "en-US";

        internal static HashSet<string> CommonParametersNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Debug",
            "ErrorAction",
            "ErrorVariable",
            "InformationAction",
            "InformationVariable",
            "OutBuffer",
            "OutVariable",
            "PipelineVariable",
            "Verbose",
            "WarningAction",
            "WarningVariable"
        };


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
