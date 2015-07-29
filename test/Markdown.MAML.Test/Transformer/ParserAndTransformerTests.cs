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

    }
}
