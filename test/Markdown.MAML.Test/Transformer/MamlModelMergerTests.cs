using Markdown.MAML.Model.MAML;
using Markdown.MAML.Model.Markdown;
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

            var result = merger.Merge(metadataCommand, originalCommand, updateInputOutput: false);

            Assert.Equal(3, result.Parameters.Count);
            Assert.Equal(3, originalCommand.Parameters.Count);
            Assert.Equal("Name", result.Parameters[0].Name);
            Assert.Equal("NewParam", result.Parameters[1].Name);
            Assert.Contains("Parameter Updated: Name", _reportStream);
            Assert.Contains("Parameter Set Added: AnotherName", _reportStream);
            Assert.Contains("Parameter Set Deleted: ByName", _reportStream);
            Assert.Contains("Old Set: SetOldName", _reportStream);
            Assert.Contains("New Set: SetNewName", _reportStream);
            Assert.Contains("Parameter Added: NewParam", _reportStream);
            Assert.Contains("Parameter Deleted: Remove", _reportStream);
            Assert.Contains("---- UPDATING Cmdlet : Get-Foo ----", _reportStream);
            Assert.Contains("---- COMPLETED UPDATING Cmdlet : Get-Foo ----\r\n\r\n", _reportStream);

            Assert.Equal(originalCommand.Synopsis.Text, result.Synopsis.Text);
            Assert.Equal(originalCommand.Description.Text, result.Description.Text);
            Assert.Equal(originalCommand.Notes.Text, result.Notes.Text);
            Assert.Equal(originalCommand.Parameters[0].Description, result.Parameters[0].Description);
            Assert.Equal(originalCommand.Parameters[0].FormatOption, result.Parameters[0].FormatOption);
            Assert.Equal(originalCommand.Parameters[2].Description, result.Parameters[2].Description);
            Assert.Equal(originalCommand.Parameters[2].FormatOption, result.Parameters[2].FormatOption);

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
                Synopsis = new SectionBody("This is the synopsis"),
                Description = new SectionBody("This is a long description.\r\nWith two paragraphs."),
                Notes = new SectionBody("This is a multiline note.\r\nSecond line.")
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
            var parameterPath = new MamlParameter()
            {
                Type = "String",
                Name = "Path",
                Required = true,
                Description = "Parameter path description.",
                FormatOption = SectionFormatOption.LineBreakAfterHeader,
                Globbing = true
            };

            originalCommand.Parameters.Add(parameterName);
            originalCommand.Parameters.Add(removedParameterName);
            originalCommand.Parameters.Add(parameterPath);

            var syntax1 = new MamlSyntax()
            {
                ParameterSetName = "ByName",
            };
            syntax1.Parameters.Add(parameterName);
            syntax1.Parameters.Add(removedParameterName);
            syntax1.Parameters.Add(parameterPath);
            originalCommand.Syntax.Add(syntax1);

            var syntax2 = new MamlSyntax()
            {
                ParameterSetName = "SetOldName",
                IsDefault = true
            };
            syntax2.Parameters.Add(parameterName);
            syntax2.Parameters.Add(parameterPath);
            originalCommand.Syntax.Add(syntax2);

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
                Code = new[] { new MamlCodeBlock("PS:> Get-Help -YouNeedIt") },
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
                Description = new SectionBody("This is a long description.\r\nWith two paragraphs."),
                Synopsis = new SectionBody("This is a old synopsis."), // DIFF!!
                Notes = new SectionBody("These are old notes") // DIFF!!
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
                Aliases = new string[]{"GF","Foo","Bar"}
            };

            var parameterNew = new MamlParameter()
            {
                Type = "String",
                Name = "NewParam",
                Description = "Old Param Description" 
            };

            var parameterPath = new MamlParameter()
            {
                Type = "String",
                Name = "Path",
                Required = true,
                Description = "Parameter path description.",
                Globbing = true
            };

            metadataCommand.Parameters.Add(parameterName1);
            metadataCommand.Parameters.Add(parameterNew);
            metadataCommand.Parameters.Add(parameterPath);

            var syntax3 = new MamlSyntax()
            {
                ParameterSetName = "AnotherName"
            };

            syntax3.Parameters.Add(parameterName1);
            syntax3.Parameters.Add(parameterNew);
            syntax3.Parameters.Add(parameterPath);
            metadataCommand.Syntax.Add(syntax3);

            var syntax4 = new MamlSyntax()
            {
                ParameterSetName = "SetNewName",
                IsDefault = true
            };

            syntax4.Parameters.Add(parameterName1);
            syntax4.Parameters.Add(parameterPath);
            metadataCommand.Syntax.Add(syntax4);

            return metadataCommand;
        }
    }
}
