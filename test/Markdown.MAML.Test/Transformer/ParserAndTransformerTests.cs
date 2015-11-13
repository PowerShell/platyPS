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
## Get-Foo
### Synopsis
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
##Add-Member

###SYNOPSIS
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
## Get-Foo
### Synopsis
This is Synopsis, but it doesn't matter in this test

### DESCRIPTION
Hello,

I'm a multiline description.

And this is my last line.
");
            MamlCommand mamlCommand = (new ModelTransformer()).NodeModelToMamlModel(doc).First();
            string[] description = mamlCommand.Description.Split('\n').Select(x => x.Trim()).ToArray();
            Assert.Equal(5, description.Length);
            Assert.Equal("Hello,", description[0]);
            Assert.Equal("", description[1]);
            Assert.Equal("I'm a multiline description.", description[2]);
            Assert.Equal("", description[3]);
            Assert.Equal("And this is my last line.", description[4]);
        }

        [Fact]
        public void RecogniceTwoCommandsWithDifferentOrdersOfEntries()
        {
            var parser = new MarkdownParser();
            var doc = parser.ParseString(@"
## Get-Foo
### Synopsis
This is Synopsis, but it doesn't matter in this test
### DESCRIPTION
This is description

## Get-Bar
### DESCRIPTION
This is description
### Synopsis
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
## Get-Foo
### Parameters
#### Bar
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
## Get-Foo
### INPUTS
#### System.String
You can pipe computer names and new names to the Add-ComputerCmdlet.

### OUTPUTS
#### Microsoft.PowerShell.Commands.ComputerChangeInfo

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
## Get-Foo
### EXAMPLES
#### --EXAMPLE1--
```
# PS code here
```
Remarks

#### --EXAMPLE2--
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
## Get-Foo
###RELATED LINKS
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
## Get-Foo
### PARAMETERS

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
            Assert.Equal("FooParam", mamlCommand.Syntax[0].Parameters[0].Name);
            Assert.Equal("BazParam", mamlCommand.Syntax[0].Parameters[1].Name);
            Assert.Equal("BarParam", mamlCommand.Syntax[1].Parameters[0].Name);
            Assert.Equal("BazParam", mamlCommand.Syntax[1].Parameters[1].Name);
        }

        [Fact]
        public void ProducesParameterAndSyntaxEntriesForNonExistingTypes()
        {
            var parser = new MarkdownParser();

            const string fooParamName = "FooParam";
            const string docFormatString = @"
## Get-Foo
### PARAMETERS

#### FooParam [NonExistingType]

```powershell
```

";
            var doc = parser.ParseString(docFormatString);

            MamlCommand mamlCommand = (new ModelTransformer()).NodeModelToMamlModel(doc).First();
            Assert.Equal(mamlCommand.Name, "Get-Foo");

            Assert.Equal(1, mamlCommand.Parameters.Count);

            var fooParam = mamlCommand.Parameters[0];
            Assert.Equal(fooParamName, fooParam.Name);
            Assert.Equal("NonExistingType", fooParam.Type);

            Assert.Equal(1, mamlCommand.Syntax.Count);
            Assert.Equal("FooParam", mamlCommand.Syntax[0].Parameters[0].Name);
            Assert.Equal("NonExistingType", mamlCommand.Syntax[0].Parameters[0].Type);
        }

        private static string GetParameterText(string paramName, string paramType, string paramAttributes)
        {
            const string paramFormatString = @"
#### {0} [{1}]

```powershell
{2}
```

This is the documentation for {0}

";
            return string.Format(paramFormatString, paramName, paramType, paramAttributes);
        }
    }
}
