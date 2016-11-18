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
            Assert.Equal(mamlCommand.Name, "Get-Foo");
            Assert.Equal(mamlCommand.Synopsis, "This is Synopsis");
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
            Assert.Equal(mamlCommand.Name, "Get-Foo");
            Assert.Equal(mamlCommand.Synopsis, "Here is a hyperlink (http://non-existing-uri).");
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
            Assert.Equal(mamlCommand.Name, "Get-Foo");
            Assert.Equal(mamlCommand.Synopsis, "This is Synopsis");
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
            Assert.Equal(mamlCommand.Name, "Add-Member");
            Assert.Equal(mamlCommand.Synopsis, "Adds custom properties and methods to an instance of a Windows PowerShell object.");
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
            string[] description = mamlCommand.Description.Split('\n').Select(x => x.Trim()).ToArray();
            Assert.Equal(3, description.Length);
            Assert.Equal("Hello,", description[0]);
            Assert.Equal("I'm a multiline description.", description[1]);
            Assert.Equal("And this is my last line.", description[2]);
            Assert.Equal(false, mamlCommand.SupportCommonParameters);
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
            Assert.Equal(mamlCommand.Count(), 2);
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
            Assert.Equal(mamlCommand.Count(), 1);
            var param = mamlCommand[0].Parameters.ToArray();
            Assert.Equal(param.Count(), 1);
            Assert.Equal(param[0].Name, "Bar");
            Assert.Equal(param[0].Description, "This is bar parameter");
            Assert.Equal(param[0].DefaultValue, "Fooo");
            Assert.Equal(param[0].PipelineInput, "false");
            Assert.Equal(param[0].Globbing, true);
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
            Assert.Equal(mamlCommand.Count(), 1);
            var param = mamlCommand[0].Parameters.ToArray();
            Assert.Equal(param.Count(), 1);
            Assert.Equal(param[0].Name, "Bar");
            Assert.Equal(param[0].Description, "This is bar parameter\r\n\r\n// With a codesnippet\r\n\r\nAnd something else");
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
            Assert.Equal(mamlCommand.Count(), 1);
            var inputs = mamlCommand[0].Inputs.ToArray();
            var outputs = mamlCommand[0].Outputs.ToArray();
            Assert.Equal(inputs.Count(), 1);
            Assert.Equal(outputs.Count(), 1);
            Assert.Equal(inputs[0].TypeName, "System.String");
            Assert.Equal(outputs[0].TypeName, "Microsoft.PowerShell.Commands.ComputerChangeInfo");
            Assert.Equal(inputs[0].Description, "You can pipe computer names and new names to the Add-ComputerCmdlet.");
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
```PowerShell
# PS More code here
```
Remarks

");
            var mamlCommand = NodeModelToMamlModelV2(doc).ToArray();
            Assert.Equal(mamlCommand.Count(), 1);
            var examples = mamlCommand[0].Examples.ToArray();
            Assert.Equal(examples.Count(), 2);
            Assert.Equal(examples[0].Title, "--EXAMPLE1--");
            Assert.Equal(examples[0].Code, "# PS code here");
            Assert.Equal(examples[0].Remarks, "Remarks");
            Assert.Equal(examples[0].Introduction, "Introduction");
            Assert.Equal(examples[1].Code, "# PS code here\r\n\r\n# PS More code here");
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
            Assert.Equal(mamlCommand.Count(), 1);
            var links = mamlCommand[0].Links.ToArray();
            Assert.Equal(links.Count(), 8);
            Assert.Equal(links[0].LinkName, "Online Version:");
            Assert.Equal(links[0].LinkUri, "http://go.microsoft.com/fwlink/p/?linkid=289795");
            Assert.Equal(links[1].LinkName, "Checkpoint-Computer");
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
            Assert.Equal(mamlCommand.Count(), 1);
            Assert.Equal(mamlCommand[0].Synopsis, "Runs the Set-WSManQuickConfig cmdlet");
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
            Assert.Equal(mamlCommand.Count(), 1);
            Assert.Equal(mamlCommand[0].Synopsis, "Runs the Set-WSManQuickConfig cmdlet");
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
            Assert.Equal(mamlCommand.Count(), 1);
            Assert.Equal(mamlCommand[0].Synopsis, "Runs the Set-WSManQuickConfig cmdlet");
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
            Assert.Equal(mamlCommand.Name, "Get-Foo");

            Assert.Equal(3, mamlCommand.Parameters.Count);

            var fooParam = mamlCommand.Parameters[0];
            Assert.Equal(fooParamName, fooParam.Name);
            Assert.Equal("string", fooParam.Type);
            Assert.True(fooParam.Required);
            Assert.Equal(0, fooParam.Aliases.Length);

            var barParam = mamlCommand.Parameters[1];
            Assert.Equal(barParamName, barParam.Name);
            Assert.Equal("double", barParam.Type);
            Assert.True(barParam.Required);
            Assert.Equal(1, barParam.Aliases.Length);
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
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters \(http://go.microsoft.com/fwlink/?LinkID=113216\).

### NoTypeParam

NoTypeParam description.
";
            var doc = ParseString(docFormatString);

            MamlCommand mamlCommand = NodeModelToMamlModelV2(doc).First();
            Assert.Equal(mamlCommand.Name, "Get-Foo");

            Assert.Equal(3, mamlCommand.Parameters.Count);

            var nonExistinTypeParam = mamlCommand.Parameters[0];
            Assert.Equal("NonExistingTypeParam", nonExistinTypeParam.Name);
            Assert.Equal("NonExistingType", nonExistinTypeParam.Type);
            Assert.Equal("This is NonExistingTypeParam description.", nonExistinTypeParam.Description);

            var noDescriptionParam = mamlCommand.Parameters[1];
            Assert.Equal("NoDescriptionParam", noDescriptionParam.Name);
            Assert.Equal("string", noDescriptionParam.Type);
            Assert.Equal("", noDescriptionParam.Description);
            Assert.Equal(true, noDescriptionParam.ValueRequired);

            var noTypeParam = mamlCommand.Parameters[2];
            Assert.Equal("NoTypeParam", noTypeParam.Name);
            Assert.Equal(null, noTypeParam.Type);
            Assert.Equal("NoTypeParam description.", noTypeParam.Description);

            Assert.Equal(true, mamlCommand.SupportCommonParameters);
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
            Assert.Equal(mamlCommand.Name, "Get-Foo");

            Assert.Equal(2, mamlCommand.Parameters.Count);

            var informationVariable = mamlCommand.Parameters[0];
            Assert.Equal("informationVariable", informationVariable.Name);
            Assert.Equal(null, informationVariable.Type);
            Assert.Equal(true, informationVariable.ValueRequired);

            var force = mamlCommand.Parameters[1];
            Assert.Equal("force", force.Name);
            Assert.Equal("SwitchParameter", force.Type);
            Assert.Equal(false, force.ValueRequired);
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
            Assert.Equal(mamlCommand.Name, "Get-Foo");

            Assert.Equal(1, mamlCommand.Syntax.Count);
            Assert.Equal(1, mamlCommand.Syntax[0].Parameters.Count);

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
            Assert.Equal(mamlCommand.Name, "Get-Foo");

            Assert.Equal(2, mamlCommand.Syntax.Count);
            var syntax1 = mamlCommand.Syntax[0];
            var syntax2 = mamlCommand.Syntax[1];

            Assert.Equal(syntax1.Parameters.Count, 1);
            Assert.Equal(syntax2.Parameters.Count, 1);

            Assert.Equal(syntax1.Parameters[0].Name, "TypeName");
            Assert.Equal(syntax2.Parameters[0].Name, "TypeName");

            Assert.Equal(syntax1.Parameters[0].Type, "string");
            Assert.Equal(syntax2.Parameters[0].Type, "string");

            Assert.Equal(syntax1.Parameters[0].Required, true);
            Assert.Equal(syntax2.Parameters[0].Required, false);
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
            Assert.Equal(mamlCommand.Name, "Get-Foo");

            Assert.Equal(2, mamlCommand.Syntax.Count);
            var syntax1 = mamlCommand.Syntax[0];
            var syntax2 = mamlCommand.Syntax[1];

            Assert.Equal(syntax1.Parameters.Count, 1);
            Assert.Equal(syntax2.Parameters.Count, 1);

            Assert.Equal(syntax2.Parameters[0].Name, "FirstSetParam");
            Assert.Equal(syntax1.Parameters[0].Name, "SecondSetParam");
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
            Assert.Equal(mamlCommand.Name, "Get-Foo");

            Assert.Equal(1, mamlCommand.Syntax.Count);
            var syntax = mamlCommand.Syntax[0];

            Assert.Equal(syntax.Parameters.Count, 3);

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
            Assert.Equal(mamlCommand.Name, "Get-Foo");

            Assert.Equal(1, mamlCommand.Parameters.Count);
            var parameter = mamlCommand.Parameters[0];

            Assert.Equal(parameter.DefaultValue, "PowerShell");
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
            Assert.Equal(mamlCommand.Name, "Get-Foo");

            Assert.Equal(1, mamlCommand.Parameters.Count);
            var parameter = mamlCommand.Parameters[0];

            Assert.True(parameter.Globbing);
            Assert.Equal(parameter.Type, "string");
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
            Assert.Equal(mamlCommand.Name, "Clear-History");

            Assert.Equal(2, mamlCommand.Syntax.Count);
            var syntax1 = mamlCommand.Syntax[0];
            var syntax2 = mamlCommand.Syntax[1];

            Assert.Equal(syntax1.Parameters.Count, 5);
            Assert.Equal(syntax2.Parameters.Count, 5);

            Assert.Equal(syntax1.Parameters[0].Name, "Id");
            Assert.Equal(syntax2.Parameters[0].Name, "Count");
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

            Assert.Equal(description, mamlCommand.Description);
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

        private IEnumerable<MamlCommand> NodeModelToMamlModelV2(DocumentNode doc)
        {
            return (new ModelTransformerVersion2()).NodeModelToMamlModel(doc);
        }
    }
}
