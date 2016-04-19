using Markdown.MAML.Renderer;
using Markdown.MAML.Model.MAML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Markdown.MAML.Test.Renderer
{
    public class MarkdownV2RendererTests
    {
        [Fact]
        public void RendererProduceMarkdownV2Output()
        {
            var renderer = new MarkdownV2Renderer();
            MamlCommand command = new MamlCommand()
            {
                Name = "Get-Foo",
                Synopsis = "This is the synopsis",
                Description = "This is a long description."
            };

            command.Parameters.Add(new MamlParameter()
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
            }
            );
            command.Inputs.Add(new MamlInputOutput()
            {
                TypeName = "String",
                Description = "Input Description goes here!"

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

            string markdown = renderer.MamlModelToString(new[] { command });
            Assert.Equal(@"# Get-Foo
## SYNOPSIS

This is the synopsis
## SYNTAX

## DESCRIPTION

This is a long description.
## EXAMPLES

## PARAMETERS

### Name

Parameter Description.

## INPUTS

## String
Input Description goes here!

## OUTPUTS

## String
Input Description goes here!

## RELATED LINKS

[PowerShell made by Microsoft Hackathon](www.microsoft.com)

", markdown);
        }
    }
}
