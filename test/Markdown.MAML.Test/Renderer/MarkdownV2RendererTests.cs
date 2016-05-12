using Markdown.MAML.Renderer;
using Markdown.MAML.Model.MAML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Collections;

namespace Markdown.MAML.Test.Renderer
{
    public class MarkdownV2RendererTests
    {
        [Fact]
        public void RendererUsesCorrectEscaping()
        {
            Assert.Equal(@"\\\<", MarkdownV2Renderer.GetEscapedMarkdownText(@"\<"));
            Assert.Equal(@"\\\\\<", MarkdownV2Renderer.GetEscapedMarkdownText(@"\\<"));
            Assert.Equal(@"\\\\\\\<", MarkdownV2Renderer.GetEscapedMarkdownText(@"\\\<"));
            Assert.Equal(@"\", MarkdownV2Renderer.GetEscapedMarkdownText(@"\"));
            Assert.Equal(@"\\\\", MarkdownV2Renderer.GetEscapedMarkdownText(@"\\"));
            Assert.Equal(@"\\\(", MarkdownV2Renderer.GetEscapedMarkdownText(@"\("));
            Assert.Equal(@"\(", MarkdownV2Renderer.GetEscapedMarkdownText(@"("));
            Assert.Equal(@"\)", MarkdownV2Renderer.GetEscapedMarkdownText(@")"));
            Assert.Equal(@"\[", MarkdownV2Renderer.GetEscapedMarkdownText(@"["));
            Assert.Equal(@"\]", MarkdownV2Renderer.GetEscapedMarkdownText(@"]"));
        }

        [Fact]
        public void RendererProduceMarkdownV2Output()
        {
            var renderer = new MarkdownV2Renderer();
            MamlCommand command = new MamlCommand()
            {
                Name = "Get-Foo",
                Synopsis = "This is the synopsis",
                Description = "This is a long description.\r\nWith two paragraphs."
            };

            var parameterName = new MamlParameter()
            {
                Type = "String",
                Name = "Name",
                Required = true,
                Description = "Parameter Description.",
                VariableLength = true,
                Globbing = true,
                PipelineInput = "True (ByValue)",
                Position = "1",
                Aliases = new string[] { "GF", "Foos", "Do" },
            };

            command.Parameters.Add(parameterName);

            var syntax1 = new MamlSyntax()
            {
                ParameterSetName = "ByName"
            };
            syntax1.Parameters.Add(parameterName);
            command.Syntax.Add(syntax1);

            command.Inputs.Add(new MamlInputOutput()
            {
                TypeName = "String",
                Description = "Input <Description> goes here!"

            }
            );
            command.Outputs.Add(new MamlInputOutput()
            {
                TypeName = "String",
                Description = "Output Description goes here!"
            }
            );
            command.Examples.Add(new MamlExample()
            {
                Title = "Example 1",
                Code = "PS:> Get-Help -YouNeedIt",
                Remarks = "This does stuff!"
            }
            );
            command.Links.Add(new MamlLink()
            {
                LinkName = "PowerShell made by Microsoft Hackathon",
                LinkUri = "www.microsoft.com"

            }
            );

            var metadata = new Hashtable();
            metadata["foo"] = "bar";
            string markdown = renderer.MamlModelToString(command, metadata);
            Assert.Equal(@"---
schema: 2.0.0
foo: bar
---

# Get-Foo
## SYNOPSIS
This is the synopsis

## SYNTAX

```
Get-Foo [-Name] <String> [<CommonParameters>]
```

## DESCRIPTION
This is a long description.

With two paragraphs.

## EXAMPLES

### Example 1
```
PS:> Get-Help -YouNeedIt
```

This does stuff!

## PARAMETERS

### Name
Parameter Description.

```yaml
Type: String
Aliases: GF, Foos, Do

Required: True
Position: 1
Default value: 
Accept pipeline input: True (ByValue)
Accept wildcard characters: True
```

## INPUTS

### String
Input \<Description\> goes here!

## OUTPUTS

### String
Output Description goes here!

## RELATED LINKS

[PowerShell made by Microsoft Hackathon](www.microsoft.com)

", markdown);
        }
    }
}
