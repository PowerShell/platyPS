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
        private string _reportStream;
        
        [Fact]
        public void RendererProduceMarkdownV2Output()
        {
            var merger = new MamlModelMerger(WriteMessage);
            var originalCommand = GetOriginal();
            var metadataCommand = GetRegenerated();

            var result = merger.Merge(metadataCommand, originalCommand);

            Assert.Equal(2, result.Parameters.Count);
            Assert.Equal(2, originalCommand.Parameters.Count);
            Assert.Equal("Name", result.Parameters[0].Name);
            Assert.Equal("NewParam", result.Parameters[1].Name);
            Assert.Contains("Get-Foo: parameter Remove is not longer present.", _reportStream);
            Assert.Contains("Get-Foo: parameter Name - description has been updated:\r\n<Old from MAML\r\n    Old Description\r\n>\r\n\r\n[New from Markdown\r\n    Parameter Description.\r\n]", _reportStream);

            Assert.Equal(originalCommand.Parameters[0].Description, result.Parameters[0].Description);

            Assert.Equal(originalCommand.Links.Count, result.Links.Count);
            Assert.Equal(originalCommand.Links[0].LinkName, result.Links[0].LinkName);
            Assert.Equal(originalCommand.Links[0].LinkUri, result.Links[0].LinkUri);
        }

        private void WriteMessage(string message)
        {
            _reportStream += message;
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
            var removedParameterName = new MamlParameter()
            {
                Type = "int",
                Name = "Remove",
                Required = true,
                Description = "Parameter Description 2.",
                VariableLength = true,
                Globbing = true,
                PipelineInput = "True (ByValue)",
                Position = "2",
                DefaultValue = "dodododo",
                Aliases = new string[] { "Pa1", "RemovedParam", "Gone" },
            };

            originalCommand.Parameters.Add(parameterName);
            originalCommand.Parameters.Add(removedParameterName);

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
                Name = "Get-Foo",
                Description = "This is a long description.\r\nWith two paragraphs.",
                Synopsis = "This is a old synopsis.",
                Notes = "These are old notes"
            };

            var parameterName1 = new MamlParameter()
            {
                Type = "String[]", // DIFF!!
                Name = "Name",
                Description = "Old Description",
                Required = true,
                VariableLength = true,
                Globbing = false, // DIFF!!
                PipelineInput = "True (ByValue)",
                Position = "Named",  // DIFF!!
            };

            var parameterNew = new MamlParameter()
            {
                Type = "String",
                Name = "NewParam",
                Description = "Old Param Description" 
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
