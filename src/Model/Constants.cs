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
