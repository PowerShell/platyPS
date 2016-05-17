using Markdown.MAML.Model.MAML;
using Markdown.MAML.Transformer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Markdown.MAML.Test.Transformer
{
    public class MamlModelMergerTests
    {

        [Fact]
        public void RendererProduceMarkdownV2Output()
        {
            var merger = new MamlModelMerger();
            var originalCommand = GetOriginal();
            var metadataCommand = GetRegenerated();

            var result = merger.Merge(metadataCommand, originalCommand);

            Assert.Equal(2, result.Parameters.Count);
            Assert.Equal("Name", result.Parameters[0].Name);
            Assert.Equal("NewParam", result.Parameters[1].Name);

            Assert.Equal(originalCommand.Parameters[0].Description, result.Parameters[0].Description);

            Assert.Equal(originalCommand.Links.Count, result.Links.Count);
            Assert.Equal(originalCommand.Links[0].LinkName, result.Links[0].LinkName);
            Assert.Equal(originalCommand.Links[0].LinkUri, result.Links[0].LinkUri);
        }

        private MamlCommand GetOriginal()
        {
            MamlCommand originalCommand = new MamlCommand()
            {
                Name = "Get-Foo",
                Synopsis = "This is the synopsis",
                Description = "This is a long description.\r\nWith two paragraphs.",
                Notes = "This is a multiline note.\r\nSecond line."
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
                DefaultValue = "trololo",
                Aliases = new string[] { "GF", "Foos", "Do" },
            };

            originalCommand.Parameters.Add(parameterName);

            var syntax1 = new MamlSyntax()
            {
                ParameterSetName = "ByName"
            };
            syntax1.Parameters.Add(parameterName);
            originalCommand.Syntax.Add(syntax1);

            originalCommand.Inputs.Add(new MamlInputOutput()
            {
                TypeName = "String",
                Description = "Input <Description> goes here!"

            }
            );
            originalCommand.Outputs.Add(new MamlInputOutput()
            {
                TypeName = "String",
                Description = "Output Description goes here!"
            }
            );
            originalCommand.Examples.Add(new MamlExample()
            {
                Title = "Example 1",
                Code = "PS:> Get-Help -YouNeedIt",
                Remarks = "This does stuff!"
            }
            );
            originalCommand.Links.Add(new MamlLink()
            {
                LinkName = "PowerShell made by Microsoft Hackathon",
                LinkUri = "www.microsoft.com"

            });

            originalCommand.Links.Add(new MamlLink()
            {
                LinkName = "", // if name is empty, it would be populated with uri
                LinkUri = "http://foo.com"
            });

            return originalCommand;
        }

        private MamlCommand GetRegenerated()
        {
            MamlCommand metadataCommand = new MamlCommand()
            {
                Name = "Get-Foo"
            };

            var parameterName1 = new MamlParameter()
            {
                Type = "String[]", // DIFF!!
                Name = "Name",
                Required = true,
                VariableLength = true,
                Globbing = false, // DIFF!!
                PipelineInput = "True (ByValue)",
                Position = "Named",  // DIFF!!
            };

            var parameterNew = new MamlParameter()
            {
                Type = "String",
                Name = "NewParam"
            };

            metadataCommand.Parameters.Add(parameterName1);
            metadataCommand.Parameters.Add(parameterNew);

            var syntax2 = new MamlSyntax()
            {
                ParameterSetName = "AnotherName"
            };

            syntax2.Parameters.Add(parameterName1);
            syntax2.Parameters.Add(parameterNew);

            metadataCommand.Syntax.Add(syntax2);
            
            return metadataCommand;
        }
    }
}
