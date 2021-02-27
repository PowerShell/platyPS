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
        internal const string OptionalParamTemplate = "[[-{0} <{1}>]";
        internal const string SingleSpace = " ";
        internal const string ParameterSetHeaderDefaultTemplate = "### {0} (Default)";
        internal const string ParameterSetHeaderTemplate = "### {0}";
        internal const string CodeBlock = "```";

        internal const string TrueString = "True";
        internal const string FalseString = "False";
        internal const string NoneString = "None";
        internal const string NamedString = "Named";
        internal const string ParameterSetsAll = "(All)";
        internal const string DelimiterComma = ", ";

        internal const string RequiredParameterSetsTemplate = "True ({0}) False ({1})";
        internal const string NotesItemHeaderTemplate = "### {0}";

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
