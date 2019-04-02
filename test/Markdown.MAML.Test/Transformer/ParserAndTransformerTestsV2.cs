using System.Collections.Generic;
using System.Linq;
using Markdown.MAML.Model.MAML;
using Markdown.MAML.Parser;
using Markdown.MAML.Transformer;
using Xunit;
using Markdown.MAML.Model.Markdown;

namespace Markdown.MAML.Test.Transformer
{
    public class ParserAndTransformerTestsV2
    {
        [Fact]
        public void TransformSimpleCommand()
        {

            var doc = ParseString(@"
# Get-Foo
## Synopsis
This is Synopsis
");
            MamlCommand mamlCommand = NodeModelToMamlModelV2(doc).First();
            Assert.Equal("Get-Foo", mamlCommand.Name);
            Assert.Equal("This is Synopsis", mamlCommand.Synopsis.Text);
        }

        [Fact]
        public void TransformSynopsisWithHyperlink()
        {

            var doc = ParseString(@"
# Get-Foo
## Synopsis
Here is a [hyperlink](http://non-existing-uri).
");
            MamlCommand mamlCommand = NodeModelToMamlModelV2(doc).First();
            Assert.Equal("Get-Foo", mamlCommand.Name);
            Assert.Equal("Here is a hyperlink (http://non-existing-uri).", mamlCommand.Synopsis.Text);
        }

        [Fact]
        public void SkipYamlMetadataBlock()
        {

            var doc = ParseString(@"
---
foo: bar
baz: foo
schema: 2.0.0
---

# Get-Foo
## Synopsis
This is Synopsis
");
            MamlCommand mamlCommand = NodeModelToMamlModelV2(doc).First();
            Assert.Equal("Get-Foo", mamlCommand.Name);
            Assert.Equal("This is Synopsis", mamlCommand.Synopsis.Text);
        }

        [Fact]
        public void TransformCommandWithExtraLine()
        {
            var doc = ParseString(@"
#Add-Member

##SYNOPSIS
Adds custom properties and methods to an instance of a Windows PowerShell object.

");
            MamlCommand mamlCommand = NodeModelToMamlModelV2(doc).First();
            Assert.Equal("Add-Member", mamlCommand.Name);
            Assert.Equal("Adds custom properties and methods to an instance of a Windows PowerShell object.", mamlCommand.Synopsis.Text);
        }

        [Fact]
        public void TransformCommandWithHeaderLineBreak()
        {
            var doc = ParseString(@"
#Add-Member

##SYNOPSIS

Adds custom properties and methods to an instance of a Windows PowerShell object.");
            MamlCommand mamlCommand = NodeModelToMamlModelV2(doc).First();
            Assert.Equal("Add-Member", mamlCommand.Name);
            Assert.Equal("Adds custom properties and methods to an instance of a Windows PowerShell object.", mamlCommand.Synopsis.Text);
            Assert.Equal(SectionFormatOption.LineBreakAfterHeader, mamlCommand.Synopsis.FormatOption);
        }

        [Fact]
        public void TransformCommandWithParameterHeaderLineBreak()
        {
            var doc = ParseString(@"
# Add-Member

## PARAMETERS

### -Name

This is the name parameter.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```
");
            MamlCommand mamlCommand = NodeModelToMamlModelV2(doc).First();
            Assert.Equal("This is the name parameter.", mamlCommand.Parameters[0].Description);
            Assert.Equal(SectionFormatOption.LineBreakAfterHeader, mamlCommand.Parameters[0].FormatOption);
        }

        [Fact]
        public void TransformCommandWithExampleHeaderLineBreak()
        {
            var doc = ParseString(@"
# Add-Member

## EXAMPLES

### Example 1

This is an example.

```powershell
PS C:\> Get-PSDocumentHeader -Path '.\build\Default\Server1.md';
```

```
Output
```

### Example 2
This is an example.

```
PS C:\> Get-PSDocumentHeader -Path '.\build\Default\Server1.md';
```

```
Get-PSDocumentHeader -Path '.\build\Default\Server1.md';
```

```
Output
```
");
            MamlCommand mamlCommand = NodeModelToMamlModelV2(doc).First();

            // Check the number of examples
            Assert.Equal(2, mamlCommand.Examples.Count);

            // Confirm example fields and code block language is read
            Assert.Equal("This is an example.", mamlCommand.Examples[0].Introduction);
            Assert.Equal(SectionFormatOption.LineBreakAfterHeader, mamlCommand.Examples[0].FormatOption);
            Assert.Equal(2, mamlCommand.Examples[0].Code.Length);
            Assert.Equal("powershell", mamlCommand.Examples[0].Code[0].LanguageMoniker);
            Assert.Equal(string.Empty, mamlCommand.Examples[0].Code[1].LanguageMoniker);

            // Confirm example fields
            Assert.Equal(SectionFormatOption.None, mamlCommand.Examples[1].FormatOption);
        }

        [Fact]
        public void TransformMultilineDescription()
        {

            var doc = ParseString(@"
# Get-Foo
## Synopsis
This is Synopsis, but it doesn't matter in this test

## DESCRIPTION
Hello,

I'm a multiline description.

And this is my last line.
");
            MamlCommand mamlCommand = NodeModelToMamlModelV2(doc).First();
            string[] description = mamlCommand.Description.Text.Split('\n').Select(x => x.Trim()).ToArray();
            Assert.Equal(3, description.Length);
            Assert.Equal("Hello,", description[0]);
            Assert.Equal("I'm a multiline description.", description[1]);
            Assert.Equal("And this is my last line.", description[2]);
            Assert.False(mamlCommand.SupportCommonParameters);
        }

        [Fact]
        public void RecogniceTwoCommandsWithDifferentOrdersOfEntries()
        {

            var doc = ParseString(@"
# Get-Foo
## Synopsis
This is Synopsis, but it doesn't matter in this test
## DESCRIPTION
This is description

# Get-Bar
## DESCRIPTION
This is description
## Synopsis
This is Synopsis, but it doesn't matter in this test

");
            var mamlCommand = NodeModelToMamlModelV2(doc).ToArray();
            Assert.Equal(2, mamlCommand.Count());
            Assert.NotNull(mamlCommand[0].Description);
            Assert.NotNull(mamlCommand[0].Synopsis);
            Assert.NotNull(mamlCommand[1].Description);
            Assert.NotNull(mamlCommand[1].Synopsis);
        }

        [Fact]
        public void SingleParameter()
        {

            var doc = ParseString(@"
# Get-Foo
## Parameters
### Bar
This is bar parameter

```yaml
Required: true
Position: named
Default value: Fooo
Accept pipeline input: false
Accept wildcard characters: true
```

");
            var mamlCommand = NodeModelToMamlModelV2(doc).ToArray();
            Assert.Single(mamlCommand);
            var param = mamlCommand[0].Parameters.ToArray();
            Assert.Single(param);
            Assert.Equal("Bar", param[0].Name);
            Assert.Equal("This is bar parameter", param[0].Description);
            Assert.Equal("Fooo", param[0].DefaultValue);
            Assert.Equal("false", param[0].PipelineInput);
            Assert.True(param[0].Globbing);
        }

        [Fact]
        public void SingleYamlApplicableParameter()
        {

            var doc = ParseString(@"
# Get-Foo
## Parameters
### Bar1
This is bar parameter

```yaml
Required: true
Position: named
Default value: Fooo
Accept pipeline input: false
Accept wildcard characters: true
Applicable: fOo, bAr
```
### Bar2
This is bar parameter

```yaml
Required: true
Position: named
Default value: Fooo
Accept pipeline input: false
Accept wildcard characters: true
Applicable: baz
```

### Bar3
This is bar parameter

```yaml
Required: true
Position: named
Default value: Fooo
Accept pipeline input: false
Accept wildcard characters: true
```

### Bar4
This is bar parameter

```yaml
Required: true
Position: named
Default value: Fooo
Accept pipeline input: false
Accept wildcard characters: true
Applicable: tag
```

");
            var mamlCommand = NodeModelToMamlModelV2(doc, new[] { "foo", "tag" }).ToArray();
            Assert.Single(mamlCommand);
            var param = mamlCommand[0].Parameters.ToArray();
            Assert.Equal(3, param.Count());
            // Bar1 has Applicable "foo"
            Assert.Equal("Bar1", param[0].Name);
            // Bar2 should not match
            // Bar3 has no applicable
            Assert.Equal("Bar3", param[1].Name);
            // Bar4 has Applicable "tag"
            Assert.Equal("Bar4", param[2].Name);
        }

        // For more context see https://github.com/PowerShell/platyPS/issues/239
        [Fact]
        public void SingleParameterWithCodesnippet()
        {

            var doc = ParseString(@"
# Get-Foo
## Parameters
### Bar
This is bar parameter

```
// With a codesnippet
```

And something else

```yaml
Required: true
Position: named
Default value: Fooo
Accept pipeline input: false
Accept wildcard characters: true
```

");
            var mamlCommand = NodeModelToMamlModelV2(doc).ToArray();
            Assert.Single(mamlCommand);
            var param = mamlCommand[0].Parameters.ToArray();
            Assert.Single(param);
            Assert.Equal("Bar", param[0].Name);
            Assert.Equal("This is bar parameter\r\n\r\n// With a codesnippet\r\n\r\nAnd something else", param[0].Description);
        }

        [Fact]
        public void InputAndOutput()
        {

            var doc = ParseString(@"
# Get-Foo
## INPUTS
### System.String
You can pipe computer names and new names to the Add-ComputerCmdlet.

## OUTPUTS
### Microsoft.PowerShell.Commands.ComputerChangeInfo

");
            var mamlCommand = NodeModelToMamlModelV2(doc).ToArray();
            Assert.Single(mamlCommand);
            var inputs = mamlCommand[0].Inputs.ToArray();
            var outputs = mamlCommand[0].Outputs.ToArray();
            Assert.Single(inputs);
            Assert.Single(outputs);
            Assert.Equal("System.String", inputs[0].TypeName);
            Assert.Equal("Microsoft.PowerShell.Commands.ComputerChangeInfo", outputs[0].TypeName);
            Assert.Equal("You can pipe computer names and new names to the Add-ComputerCmdlet.", inputs[0].Description);
            Assert.Empty(outputs[0].Description);
        }

        [Fact]
        public void Produce2Examples()
        {

            var doc = ParseString(@"
# Get-Foo

## NOTES

## EXAMPLES
### --EXAMPLE1--
Introduction
```
# PS code here
```
Remarks

### --EXAMPLE2--
Introduction
```
# PS code here
```
```powershell
# PS More code here
```
Remarks

");
            var mamlCommand = NodeModelToMamlModelV2(doc).ToArray();
            Assert.Single(mamlCommand);
            var examples = mamlCommand[0].Examples.ToArray();
            Assert.Equal(2, examples.Count());
            Assert.Equal("--EXAMPLE1--", examples[0].Title);
            Assert.Equal("# PS code here", examples[0].Code[0].Text);
            Assert.Equal("Remarks", examples[0].Remarks);
            Assert.Equal("Introduction", examples[0].Introduction);
            Assert.Equal("# PS code here", examples[1].Code[0].Text);
            Assert.Equal("# PS More code here", examples[1].Code[1].Text);
            Assert.Equal("powershell", examples[1].Code[1].LanguageMoniker);
        }

        [Fact]
        public void ProduceRelatedLinks()
        {

            var doc = ParseString(@"
# Get-Foo
##RELATED LINKS

[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289795)

[Checkpoint-Computer]()

[Remove-Computer]()

[Restart-Computer]()

[Rename-Computer]()

[Restore-Computer]()

[Stop-Computer]()

[Test-Connection]()
");
            var mamlCommand = NodeModelToMamlModelV2(doc).ToArray();
            Assert.Single(mamlCommand);
            var links = mamlCommand[0].Links.ToArray();
            Assert.Equal(8, links.Count());
            Assert.Equal("Online Version:", links[0].LinkName);
            Assert.Equal("http://go.microsoft.com/fwlink/p/?linkid=289795", links[0].LinkUri);
            Assert.Equal("Checkpoint-Computer", links[1].LinkName);
            Assert.Empty(links[1].LinkUri);
        }

        [Fact]
        public void HandlesHyperLinksInsideText()
        {
            var doc = ParseString(@"
# Get-Foo
## SYNOPSIS
Runs the [Set-WSManQuickConfig]() cmdlet

");
            var mamlCommand = NodeModelToMamlModelV2(doc).ToArray();
            Assert.Single(mamlCommand);
            Assert.Equal("Runs the Set-WSManQuickConfig cmdlet", mamlCommand[0].Synopsis.Text);
        }

        [Fact]
        public void HandlesItalicInsideText()
        {
            var doc = ParseString(@"
# Get-Foo
## SYNOPSIS
Runs the *Set-WSManQuickConfig* cmdlet

");
            var mamlCommand = NodeModelToMamlModelV2(doc).ToArray();
            Assert.Single(mamlCommand);
            Assert.Equal("Runs the Set-WSManQuickConfig cmdlet", mamlCommand[0].Synopsis.Text);
        }

        [Fact]
        public void HandlesBoldInsideText()
        {
            var doc = ParseString(@"
# Get-Foo
## SYNOPSIS
Runs the **Set-WSManQuickConfig** cmdlet

");
            var mamlCommand = NodeModelToMamlModelV2(doc).ToArray();
            Assert.Single(mamlCommand);
            Assert.Equal("Runs the Set-WSManQuickConfig cmdlet", mamlCommand[0].Synopsis.Text);
        }

        [Fact]
        public void ProducesParameterAndSyntaxEntries()
        {


            const string fooParamName = "FooParam";
            const string fooAttributes = @"
Type: string

Required: true
Position: 0
Default value: None
Accept pipeline input: true (ByValue, ByPropertyName)
Accept wildcard characters: false
";

            const string barParamName = "BarParam";
            const string barAttributes = @"
Type: double
Parameter Sets: BarParamSet
Aliases: br

Required: true
Position: 0
Default value: None
Accept pipeline input: true (ByValue, ByPropertyName)
Accept wildcard characters: false
";

            const string bazParamName = "BazParam";
            const string bazAttributes = @"
Type: int
Parameter Sets: BazParamSet
Aliases: bz, z

Required: false
Position: named
Default value: None
Accept pipeline input: false
Accept wildcard characters: false
";

            const string docFormatString = @"
# Get-Foo
## PARAMETERS

{0}
{1}
{2}
";
            var doc =
                ParseString(
                    string.Format(
                        docFormatString,
                        GetParameterText(fooParamName, fooAttributes),
                        GetParameterText(barParamName, barAttributes),
                        GetParameterText(bazParamName, bazAttributes)));

            MamlCommand mamlCommand = NodeModelToMamlModelV2(doc).First();
            Assert.Equal("Get-Foo", mamlCommand.Name);

            Assert.Equal(3, mamlCommand.Parameters.Count);

            var fooParam = mamlCommand.Parameters[0];
            Assert.Equal(fooParamName, fooParam.Name);
            Assert.Equal("string", fooParam.Type);
            Assert.True(fooParam.Required);
            Assert.Empty(fooParam.Aliases);

            var barParam = mamlCommand.Parameters[1];
            Assert.Equal(barParamName, barParam.Name);
            Assert.Equal("double", barParam.Type);
            Assert.True(barParam.Required);
            Assert.Single(barParam.Aliases);
            Assert.Contains("br", barParam.Aliases);

            var bazParam = mamlCommand.Parameters[2];
            Assert.Equal(bazParamName, bazParam.Name);
            Assert.Equal("int", bazParam.Type);
            Assert.False(bazParam.Required);
            Assert.Equal(2, bazParam.Aliases.Length);
            Assert.Contains("bz", bazParam.Aliases);
            Assert.Contains("z", bazParam.Aliases);

            Assert.Equal(2, mamlCommand.Syntax.Count);
            Assert.Equal("FooParam", mamlCommand.Syntax[1].Parameters[0].Name);
            Assert.Equal("BazParam", mamlCommand.Syntax[1].Parameters[1].Name);
            Assert.Equal("FooParam", mamlCommand.Syntax[0].Parameters[0].Name);
            Assert.Equal("BarParam", mamlCommand.Syntax[0].Parameters[1].Name);
        }

        [Fact]
        public void ProducesParameterEntriesForCornerCases()
        {


            const string docFormatString = @"
# Get-Foo
## PARAMETERS

### NonExistingTypeParam
This is NonExistingTypeParam description.

```yaml
Type: NonExistingType
```

### NoDescriptionParam

```yaml
Type: string
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters]\(http://go.microsoft.com/fwlink/?LinkID=113216\).

### NoTypeParam

NoTypeParam description.
";
            var doc = ParseString(docFormatString);

            MamlCommand mamlCommand = NodeModelToMamlModelV2(doc).First();
            Assert.Equal("Get-Foo", mamlCommand.Name);

            Assert.Equal(3, mamlCommand.Parameters.Count);

            var nonExistinTypeParam = mamlCommand.Parameters[0];
            Assert.Equal("NonExistingTypeParam", nonExistinTypeParam.Name);
            Assert.Equal("NonExistingType", nonExistinTypeParam.Type);
            Assert.Equal("This is NonExistingTypeParam description.", nonExistinTypeParam.Description);

            var noDescriptionParam = mamlCommand.Parameters[1];
            Assert.Equal("NoDescriptionParam", noDescriptionParam.Name);
            Assert.Equal("string", noDescriptionParam.Type);
            Assert.Equal("", noDescriptionParam.Description);
            Assert.True(noDescriptionParam.ValueRequired);

            var noTypeParam = mamlCommand.Parameters[2];
            Assert.Equal("NoTypeParam", noTypeParam.Name);
            Assert.Null(noTypeParam.Type);
            Assert.Equal("NoTypeParam description.", noTypeParam.Description);

            Assert.True(mamlCommand.SupportCommonParameters);
        }

        [Fact]
        public void ProducesParameterForDefaultParameterName()
        {


            const string docFormatString = @"
# Get-Foo
## PARAMETERS

### informationVariable

### force

```yaml
Type: SwitchParameter
Required: false
```
";
            var doc = ParseString(docFormatString);

            MamlCommand mamlCommand = NodeModelToMamlModelV2(doc).First();
            Assert.Equal("Get-Foo", mamlCommand.Name);

            Assert.Equal(2, mamlCommand.Parameters.Count);

            var informationVariable = mamlCommand.Parameters[0];
            Assert.Equal("informationVariable", informationVariable.Name);
            Assert.Null(informationVariable.Type);
            Assert.True(informationVariable.ValueRequired);

            var force = mamlCommand.Parameters[1];
            Assert.Equal("force", force.Name);
            Assert.Equal("SwitchParameter", force.Type);
            Assert.False(force.ValueRequired);
        }

        [Fact]
        public void ProducesParameterValueGroup()
        {


            const string docFormatString = @"
# Get-Foo
## PARAMETERS

### foo
```yaml
Type: string
Accepted values: a, b, c
```
";
            var doc = ParseString(docFormatString);

            MamlCommand mamlCommand = NodeModelToMamlModelV2(doc).First();
            Assert.Equal("Get-Foo", mamlCommand.Name);

            Assert.Single(mamlCommand.Syntax);
            Assert.Single(mamlCommand.Syntax[0].Parameters);

            var fooParam = mamlCommand.Syntax[0].Parameters[0];
            Assert.Equal("foo", fooParam.Name);
            Assert.Equal("string", fooParam.Type);
            Assert.Equal(3, fooParam.ParameterValueGroup.Count);
            Assert.Equal("a", fooParam.ParameterValueGroup[0]);
            Assert.Equal("b", fooParam.ParameterValueGroup[1]);
            Assert.Equal("c", fooParam.ParameterValueGroup[2]);
        }

        [Fact]
        public void ProducesSyntaxForTwoSets()
        {


            const string docFormatString = @"
# Get-Foo
## PARAMETERS

### TypeName

```yaml
Type: string
Parameter sets: Set 1
Required: true
```

```yaml
Type: string
Parameter sets: Set 2
Required: false
```
";
            var doc = ParseString(docFormatString);

            MamlCommand mamlCommand = NodeModelToMamlModelV2(doc).First();
            Assert.Equal("Get-Foo", mamlCommand.Name);

            // Check Syntax
            Assert.Equal(2, mamlCommand.Syntax.Count);
            var syntax1 = mamlCommand.Syntax[0];
            var syntax2 = mamlCommand.Syntax[1];

            Assert.Single(syntax1.Parameters);
            Assert.Single(syntax2.Parameters);

            Assert.Equal("TypeName", syntax1.Parameters[0].Name);
            Assert.Equal("TypeName", syntax2.Parameters[0].Name);

            Assert.Equal("string", syntax1.Parameters[0].Type);
            Assert.Equal("string", syntax2.Parameters[0].Type);

            Assert.True(syntax1.Parameters[0].Required);
            Assert.False(syntax2.Parameters[0].Required);

            // Check Parameters
            Assert.Single(mamlCommand.Parameters);
            var parameter = mamlCommand.Parameters[0];
            Assert.Equal("TypeName", parameter.Name);
            // Required == true because first takes precedence
            Assert.True(parameter.Required);
        }

        [Fact]
        public void ApplicableAndSyntaxForTwoSetsInterraction()
        {
            // we are mixing together two yaml definitions and applicable tag

            const string docFormatString = @"
# Get-Foo
## PARAMETERS

### TypeName

```yaml
Type: string
Parameter sets: Set 1
Required: true
applicable: foo
```

```yaml
Type: string
Parameter sets: Set 2
Required: false
Applicable: bar
```
";
            var doc = ParseString(docFormatString);

            MamlCommand mamlCommand = NodeModelToMamlModelV2(doc, new[] { "bar" }).First();
            Assert.Equal("Get-Foo", mamlCommand.Name);

            Assert.Single(mamlCommand.Syntax);
            var syntax1 = mamlCommand.Syntax[0];

            Assert.Single(syntax1.Parameters);
            Assert.Equal("TypeName", syntax1.Parameters[0].Name);
            Assert.Equal("string", syntax1.Parameters[0].Type);
            Assert.False(syntax1.Parameters[0].Required);
        }

        [Fact]
        public void ProducesSyntaxInTheRightOrder()
        {


            const string docFormatString = @"
# Get-Foo
## PARAMETERS

### SecondSetParam

```yaml
Type: String
Parameter sets: Set 2
```

### FirstSetParam

```yaml
Type: String
Parameter sets: Set 1
```

";
            var doc = ParseString(docFormatString);

            MamlCommand mamlCommand = NodeModelToMamlModelV2(doc).First();
            Assert.Equal("Get-Foo", mamlCommand.Name);

            Assert.Equal(2, mamlCommand.Syntax.Count);
            var syntax1 = mamlCommand.Syntax[0];
            var syntax2 = mamlCommand.Syntax[1];

            Assert.Single(syntax1.Parameters);
            Assert.Single(syntax2.Parameters);

            Assert.Equal("FirstSetParam", syntax2.Parameters[0].Name);
            Assert.Equal("SecondSetParam", syntax1.Parameters[0].Name);
        }

        [Fact]
        public void ProducesParametersInTheRightOrderInSyntax()
        {


            const string docFormatString = @"
# Get-Foo
## PARAMETERS

### -Third

```yaml
Type: String
Position: 3
```

### -Named

```yaml
Type: String
Position: Named
```

### -First

```yaml
Type: String
Position: 1
```

";
            var doc = ParseString(docFormatString);

            MamlCommand mamlCommand = NodeModelToMamlModelV2(doc).First();
            Assert.Equal("Get-Foo", mamlCommand.Name);

            Assert.Single(mamlCommand.Syntax);
            var syntax = mamlCommand.Syntax[0];

            Assert.Equal(3, syntax.Parameters.Count);

            Assert.Equal("First", syntax.Parameters[0].Name);
            Assert.Equal("Third", syntax.Parameters[1].Name);
            Assert.Equal("Named", syntax.Parameters[2].Name);
        }

        [Fact]
        public void ProduceDefaultValues()
        {


            const string docFormatString = @"
# Get-Foo
## PARAMETERS

### Name

```yaml
Type: String
Parameter Sets: Set1
Aliases:

Required: False
Position: Named
Default value: PowerShell
Accept pipeline input: False
Accept wildcard characters: False
```

";
            var doc = ParseString(docFormatString);

            MamlCommand mamlCommand = NodeModelToMamlModelV2(doc).First();
            Assert.Equal("Get-Foo", mamlCommand.Name);

            Assert.Single(mamlCommand.Parameters);
            var parameter = mamlCommand.Parameters[0];

            Assert.Equal("PowerShell", parameter.DefaultValue);
        }

        [Fact]
        public void UsesEntryToMarkGlobbing()
        {


            const string docFormatString = @"
# Get-Foo
## PARAMETERS

### Name

```yaml
Type: string
Accept wildcard characters: true
```

";
            var doc = ParseString(docFormatString);

            MamlCommand mamlCommand = NodeModelToMamlModelV2(doc).First();
            Assert.Equal("Get-Foo", mamlCommand.Name);

            Assert.Single(mamlCommand.Parameters);
            var parameter = mamlCommand.Parameters[0];

            Assert.True(parameter.Globbing);
            Assert.Equal("string", parameter.Type);
        }

        [Fact]
        public void ParseClearHistorySyntaxInTheRightOrder()
        {
            const string docFormatString = @"

# Clear-History

## SYNOPSIS
Deletes entries from the command history.

## DESCRIPTION
The Clear-History cmdlet deletes commands from the command history, that is, the list of commands entered during the current session.
Without parameters, Clear-History deletes all commands from the session history, but you can use the parameters of Clear-History to delete selected commands.

## PARAMETERS

### Id
Deletes commands with the specified history IDs.
To find the history ID of a command, use Get-History.

```yaml
Type: Int32[]
Parameter Sets: IDParameter
Aliases:

Required: False
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: True
```

### Count
Clears the specified number of  history entries, beginning with the oldest entry in the history.
If you use the Count and Id parameters in the same command, the cmdlet clears the number of entries specified by the Count parameter, beginning with the entry specified by the Id parameter.  For example, if Count is 10 and Id is 30, Clear-History clears items 21 through 30 inclusive.
If you use the Count and CommandLine parameters in the same command, Clear-History clears the number of entries specified by the Count parameter, beginning with the entry specified by the CommandLine parameter.

```yaml
Type: Int32
Parameter Sets: IDParameter, CommandLineParameter
Aliases:

Required: False
Position: 2
Default value:
Accept pipeline input: False
Accept wildcard characters: True
```

### Newest
Deletes the newest entries in the history. By default, Clear-History deletes the oldest entries in the history.

```yaml
Type: SwitchParameter
Parameter Sets: IDParameter, CommandLineParameter
Aliases:

Required: False
Position: named
Default value: False
Accept pipeline input: False
Accept wildcard characters: True
```

### WhatIf
Shows what would happen if the cmdlet runs. The cmdlet is not run.Shows what would happen if the cmdlet runs. The cmdlet is not run.

```yaml
Type: SwitchParameter
Parameter Sets: IDParameter, CommandLineParameter
Aliases: wi

Required: False
Position: named
Default value: false
Accept pipeline input: False
Accept wildcard characters: True
```

### Confirm
Prompts you for confirmation before running the cmdlet.Prompts you for confirmation before running the cmdlet.

```yaml
Type: SwitchParameter
Parameter Sets: IDParameter, CommandLineParameter
Aliases: cf

Required: False
Position: named
Default value: false
Accept pipeline input: False
Accept wildcard characters: True
```

### CommandLine
Deletes commands with the specified text strings. If you enter more than one string, Clear-History deletes commands with any of the strings.

```yaml
Type: String[]
Parameter Sets: CommandLineParameter
Aliases:

Required: False
Position: named
Default value:
Accept pipeline input: False
Accept wildcard characters: True
```
";
            var doc = ParseString(docFormatString);

            MamlCommand mamlCommand = NodeModelToMamlModelV2(doc).First();
            Assert.Equal("Clear-History", mamlCommand.Name);

            Assert.Equal(2, mamlCommand.Syntax.Count);
            var syntax1 = mamlCommand.Syntax[0];
            var syntax2 = mamlCommand.Syntax[1];

            Assert.Equal(5, syntax1.Parameters.Count);
            Assert.Equal(5, syntax2.Parameters.Count);

            Assert.Equal("Id", syntax1.Parameters[0].Name);
            Assert.Equal("Count", syntax2.Parameters[0].Name);
        }

        [Fact]
        public void PreserveFormattingIfNeeded()
        {
            const string description = @"Hello

This description block test formatting preservance.
-- It need to
-- Be. Very. [Weiredly](formatted)



\< to keep > the purpose. \\( \\\( \(
   This is intentional.
* we

* dont't want. * Mess up with user's formatting.
";

            const string docFormatString = @"
# Get-Foo
## DESCRIPTION
" + description;
            var doc = ParseStringPreserveFormat(docFormatString);

            MamlCommand mamlCommand = NodeModelToMamlModelV2(doc).First();
            Assert.Equal("Get-Foo", mamlCommand.Name);

            Common.AssertMultilineEqual(description, mamlCommand.Description.Text);
        }

        private DocumentNode ParseString(string markdown)
        {
            var parser = new MarkdownParser();
            return parser.ParseString(new string[] { markdown });
        }

        private DocumentNode ParseStringPreserveFormat(string markdown)
        {
            var parser = new MarkdownParser();
            return parser.ParseString(new string[] { markdown }, ParserMode.FormattingPreserve, null);
        }

        private static string GetParameterText(string paramName, string paramAttributes)
        {
            const string paramFormatString = @"
### {0}

This is the documentation for {0}

```yaml
{1}
```

";
            return string.Format(paramFormatString, paramName, paramAttributes);
        }

        private IEnumerable<MamlCommand> NodeModelToMamlModelV2(DocumentNode doc, string[] applicableTag = null)
        {
            return (new ModelTransformerVersion2(null, null, applicableTag)).NodeModelToMamlModel(doc);
        }
    }
}
