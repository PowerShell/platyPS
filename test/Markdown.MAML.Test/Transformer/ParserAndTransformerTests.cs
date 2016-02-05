using System.Collections.Generic;
using System.Linq;
using Markdown.MAML.Model.MAML;
using Markdown.MAML.Parser;
using Markdown.MAML.Renderer;
using Markdown.MAML.Transformer;
using Xunit;

namespace Markdown.MAML.Test.Transformer
{
    public class ParserAndTransformerTests
    {
        [Fact]
        public void TransformSimpleCommand()
        {
            var parser = new MarkdownParser();
            var doc = parser.ParseString(@"
# Get-Foo
## Synopsis
This is Synopsis
");
            MamlCommand mamlCommand = (new ModelTransformer()).NodeModelToMamlModel(doc).First();
            Assert.Equal(mamlCommand.Name, "Get-Foo");
            Assert.Equal(mamlCommand.Synopsis, "This is Synopsis");
        }

        [Fact]
        public void TransformCommandWithExtraLine()
        {
            var parser = new MarkdownParser();
            var doc = parser.ParseString(@"
#Add-Member

##SYNOPSIS
Adds custom properties and methods to an instance of a Windows PowerShell object.

");
            MamlCommand mamlCommand = (new ModelTransformer()).NodeModelToMamlModel(doc).First();
            Assert.Equal(mamlCommand.Name, "Add-Member");
            Assert.Equal(mamlCommand.Synopsis, "Adds custom properties and methods to an instance of a Windows PowerShell object.");
        }

        [Fact]
        public void TransformMultilineDescription()
        {
            var parser = new MarkdownParser();
            var doc = parser.ParseString(@"
# Get-Foo
## Synopsis
This is Synopsis, but it doesn't matter in this test

## DESCRIPTION
Hello,

I'm a multiline description.

And this is my last line.
");
            MamlCommand mamlCommand = (new ModelTransformer()).NodeModelToMamlModel(doc).First();
            string[] description = mamlCommand.Description.Split('\n').Select(x => x.Trim()).ToArray();
            Assert.Equal(3, description.Length);
            Assert.Equal("Hello,", description[0]);
            Assert.Equal("I'm a multiline description.", description[1]);
            Assert.Equal("And this is my last line.", description[2]);
        }

        [Fact]
        public void RecogniceTwoCommandsWithDifferentOrdersOfEntries()
        {
            var parser = new MarkdownParser();
            var doc = parser.ParseString(@"
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
            var mamlCommand = (new ModelTransformer()).NodeModelToMamlModel(doc).ToArray();
            Assert.Equal(mamlCommand.Count(), 2);
            Assert.NotNull(mamlCommand[0].Description);
            Assert.NotNull(mamlCommand[0].Synopsis);
            Assert.NotNull(mamlCommand[1].Description);
            Assert.NotNull(mamlCommand[1].Synopsis);
        }

        [Fact]
        public void SingleParameter()
        {
            var parser = new MarkdownParser();
            var doc = parser.ParseString(@"
# Get-Foo
## Parameters
### Bar
This is bar parameter

");
            var mamlCommand = (new ModelTransformer()).NodeModelToMamlModel(doc).ToArray();
            Assert.Equal(mamlCommand.Count(), 1);
            var param = mamlCommand[0].Parameters.ToArray();
            Assert.Equal(param.Count(), 1);
            Assert.Equal(param[0].Name, "Bar");
            Assert.Equal(param[0].Description, "This is bar parameter");
        }

        [Fact]
        public void InputAndOutput()
        {
            var parser = new MarkdownParser();
            var doc = parser.ParseString(@"
# Get-Foo
## INPUTS
### System.String
You can pipe computer names and new names to the Add-ComputerCmdlet.

## OUTPUTS
### Microsoft.PowerShell.Commands.ComputerChangeInfo

");
            var mamlCommand = (new ModelTransformer()).NodeModelToMamlModel(doc).ToArray();
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
            var parser = new MarkdownParser();
            var doc = parser.ParseString(@"
# Get-Foo

## NOTES

## EXAMPLES
### --EXAMPLE1--
```
# PS code here
```
Remarks

### --EXAMPLE2--
```
# PS code here
```
Remarks

");
            var mamlCommand = (new ModelTransformer()).NodeModelToMamlModel(doc).ToArray();
            Assert.Equal(mamlCommand.Count(), 1);
            var examples = mamlCommand[0].Examples.ToArray();
            Assert.Equal(examples.Count(), 2);
            Assert.Equal(examples[0].Title, "--EXAMPLE1--");
            Assert.Equal(examples[0].Code, "# PS code here");
            Assert.Equal(examples[0].Remarks, "Remarks");
        }

        [Fact]
        public void ProduceRelatedLinks()
        {
            var parser = new MarkdownParser();
            var doc = parser.ParseString(@"
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
            var mamlCommand = (new ModelTransformer()).NodeModelToMamlModel(doc).ToArray();
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
            var parser = new MarkdownParser();
            var doc = parser.ParseString(@"
# Get-Foo
## SYNOPSIS

Runs the [Set-WSManQuickConfig]() cmdlet

");
            var mamlCommand = (new ModelTransformer()).NodeModelToMamlModel(doc).ToArray();
            Assert.Equal(mamlCommand.Count(), 1);
            Assert.Equal(mamlCommand[0].Synopsis, "Runs the Set-WSManQuickConfig cmdlet");
        }

        [Fact]
        public void ProducesParameterAndSyntaxEntries()
        {
            var parser = new MarkdownParser();

            const string fooParamName = "FooParam";
            const string fooAttributes = @"
[Parameter(
    Position = 0,
    ParameterSetName = ""FooParamSet"",
    Mandatory = $true,
    ValueFromPipeline = $true,
    ValueFromPipelineByPropertyName = $true)]
";

            const string barParamName = "BarParam";
            const string barAttributes = @"
[Parameter(
    Position = 0,
    ParameterSetName = ""BarParamSet"",
    Mandatory = $true,
    ValueFromPipeline = $true,
    ValueFromPipelineByPropertyName = $true)]
[Alias(""br"")]
";

            const string bazParamName = "BazParam";
            const string bazAttributes = @"
[Alias(""bz"")]
[Alias(""z"")]
";

            const string docFormatString = @"
# Get-Foo
## PARAMETERS

{0}
{1}
{2}
";
            var doc = 
                parser.ParseString(
                    string.Format(
                        docFormatString,
                        GetParameterText(fooParamName, "string", fooAttributes),
                        GetParameterText(barParamName, "double", barAttributes),
                        GetParameterText(bazParamName, "int", bazAttributes)));

            MamlCommand mamlCommand = (new ModelTransformer()).NodeModelToMamlModel(doc).First();
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
            Assert.Equal("BarParam", mamlCommand.Syntax[0].Parameters[0].Name);
            Assert.Equal("BazParam", mamlCommand.Syntax[0].Parameters[1].Name);
        }

        [Fact]
        public void ProducesParameterEntriesForCornerCases()
        {
            var parser = new MarkdownParser();

            const string docFormatString = @"
# Get-Foo
## PARAMETERS

### NonExistingTypeParam [NonExistingType]

```powershell
```

This is NonExistingTypeParam description.

### NoDescriptionParam [string]

### NoTypeParam

NoTypeParam description.
";
            var doc = parser.ParseString(docFormatString);

            MamlCommand mamlCommand = (new ModelTransformer()).NodeModelToMamlModel(doc).First();
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

            var noTypeParam = mamlCommand.Parameters[2];
            Assert.Equal("NoTypeParam", noTypeParam.Name);
            Assert.Equal(null, noTypeParam.Type);
            Assert.Equal("NoTypeParam description.", noTypeParam.Description);
        }

        [Fact]
        public void ProducesParameterForDefaultParameterName()
        {
            var parser = new MarkdownParser();

            const string docFormatString = @"
# Get-Foo
## PARAMETERS

### informationVariable

### force [switch]
```powershell
[Parameter(Mandatory=$false)]
```
";
            var doc = parser.ParseString(docFormatString);

            MamlCommand mamlCommand = (new ModelTransformer()).NodeModelToMamlModel(doc).First();
            Assert.Equal(mamlCommand.Name, "Get-Foo");

            Assert.Equal(2, mamlCommand.Parameters.Count);

            var fooParam = mamlCommand.Parameters[0];
            Assert.Equal("informationVariable", fooParam.Name);
            Assert.Equal(null, fooParam.Type);
        }

        [Fact]
        public void ProducesParameterValueGroup()
        {
            var parser = new MarkdownParser();

            const string docFormatString = @"
# Get-Foo
## PARAMETERS

### foo [string]
```powershell
[ValidateSet('a', 'b', 'c')]
```
";
            var doc = parser.ParseString(docFormatString);

            MamlCommand mamlCommand = (new ModelTransformer()).NodeModelToMamlModel(doc).First();
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
            var parser = new MarkdownParser();

            const string docFormatString = @"
# Get-Foo
## PARAMETERS

### TypeName [String]

```powershell
[Parameter(Mandatory = $true, ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
```
";
            var doc = parser.ParseString(docFormatString);

            MamlCommand mamlCommand = (new ModelTransformer()).NodeModelToMamlModel(doc).First();
            Assert.Equal(mamlCommand.Name, "Get-Foo");

            Assert.Equal(2, mamlCommand.Syntax.Count);
            var syntax1 = mamlCommand.Syntax[0];
            var syntax2 = mamlCommand.Syntax[1];

            Assert.Equal(syntax1.Parameters.Count, 1);
            Assert.Equal(syntax2.Parameters.Count, 1);

            Assert.Equal(syntax1.Parameters[0].Name, "TypeName");
            Assert.Equal(syntax2.Parameters[0].Name, "TypeName");

            Assert.Equal(syntax1.Parameters[0].Type, "String");
            Assert.Equal(syntax2.Parameters[0].Type, "String");

            Assert.Equal(syntax1.Parameters[0].Required, false);
            Assert.Equal(syntax2.Parameters[0].Required, true);
        }

        [Fact]
        public void ProducesSyntaxInTheRightOrder()
        {
            var parser = new MarkdownParser();

            const string docFormatString = @"
# Get-Foo
## PARAMETERS

### SecondSetParam [String]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
```

### FirstSetParam [String]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

";
            var doc = parser.ParseString(docFormatString);

            MamlCommand mamlCommand = (new ModelTransformer()).NodeModelToMamlModel(doc).First();
            Assert.Equal(mamlCommand.Name, "Get-Foo");

            Assert.Equal(2, mamlCommand.Syntax.Count);
            var syntax1 = mamlCommand.Syntax[0];
            var syntax2 = mamlCommand.Syntax[1];

            Assert.Equal(syntax1.Parameters.Count, 1);
            Assert.Equal(syntax2.Parameters.Count, 1);

            Assert.Equal(syntax1.Parameters[0].Name, "FirstSetParam");
            Assert.Equal(syntax2.Parameters[0].Name, "SecondSetParam");
        }

        [Fact]
        public void ProduceDefaultValues()
        {
            var parser = new MarkdownParser();

            const string docFormatString = @"
# Get-Foo
## PARAMETERS

### Name [String] = PowerShell

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

";
            var doc = parser.ParseString(docFormatString);

            MamlCommand mamlCommand = (new ModelTransformer()).NodeModelToMamlModel(doc).First();
            Assert.Equal(mamlCommand.Name, "Get-Foo");

            Assert.Equal(1, mamlCommand.Parameters.Count);
            var parameter = mamlCommand.Parameters[0];

            Assert.Equal(parameter.DefaultValue, "PowerShell");
        }

        [Fact]
        public void UsesSupportsWildCardsToMarkGlobbing()
        {
            var parser = new MarkdownParser();

            const string docFormatString = @"
# Get-Foo
## PARAMETERS

### Name [String]

```
[SupportsWildCards()]
```

";
            var doc = parser.ParseString(docFormatString);

            MamlCommand mamlCommand = (new ModelTransformer()).NodeModelToMamlModel(doc).First();
            Assert.Equal(mamlCommand.Name, "Get-Foo");

            Assert.Equal(1, mamlCommand.Parameters.Count);
            var parameter = mamlCommand.Parameters[0];

            Assert.True(parameter.Globbing);
            Assert.Equal(parameter.Type, "String");
        }

        [Fact]
        public void ParseClearHistorySyntaxInTheRightOrder()
        {
            var parser = new MarkdownParser();

            const string docFormatString = @"

# Clear-History

## SYNOPSIS
Deletes entries from the command history.

## DESCRIPTION
The Clear-History cmdlet deletes commands from the command history, that is, the list of commands entered during the current session.
Without parameters, Clear-History deletes all commands from the session history, but you can use the parameters of Clear-History to delete selected commands.

## PARAMETERS

### CommandLine [String[]]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[SupportsWildCards()]
```

Deletes commands with the specified text strings. If you enter more than one string, Clear-History deletes commands with any of the strings.


### Count [Int32]

```powershell
[Parameter(Position = 2)]
```

Clears the specified number of  history entries, beginning with the oldest entry in the history.
If you use the Count and Id parameters in the same command, the cmdlet clears the number of entries specified by the Count parameter, beginning with the entry specified by the Id parameter.
For example, if Count is 10 and Id is 30, Clear-History clears items 21 through 30 inclusive.
If you use the Count and CommandLine parameters in the same command, Clear-History clears the number of entries specified by the Count parameter, beginning with the entry specified by the CommandLine parameter.


### Id [Int32[]]

```powershell
[Parameter(
  Position = 1,
  ParameterSetName = 'Set 1')]
```

Deletes commands with the specified history IDs.
To find the history ID of a command, use Get-History.


### Newest [switch]

Deletes the newest entries in the history. By default, Clear-History deletes the oldest entries in the history.


### Confirm [switch]

Prompts you for confirmation before running the cmdlet.Prompts you for confirmation before running the cmdlet.


### WhatIf [switch]

Shows what would happen if the cmdlet runs. The cmdlet is not run.Shows what would happen if the cmdlet runs. The cmdlet is not run.


";
            var doc = parser.ParseString(docFormatString);

            MamlCommand mamlCommand = (new ModelTransformer()).NodeModelToMamlModel(doc).First();
            Assert.Equal(mamlCommand.Name, "Clear-History");

            Assert.Equal(2, mamlCommand.Syntax.Count);
            var syntax1 = mamlCommand.Syntax[0];
            var syntax2 = mamlCommand.Syntax[1];

            Assert.Equal(syntax1.Parameters.Count, 5);
            Assert.Equal(syntax2.Parameters.Count, 5);

            Assert.Equal(syntax1.Parameters[0].Name, "Id");
            Assert.Equal(syntax2.Parameters[1].Name, "CommandLine");
        }

        private static string GetParameterText(string paramName, string paramType, string paramAttributes)
        {
            const string paramFormatString = @"
### {0} [{1}]

```powershell
{2}
```

This is the documentation for {0}

";
            return string.Format(paramFormatString, paramName, paramType, paramAttributes);
        }
    }
}
