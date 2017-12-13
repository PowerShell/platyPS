using Markdown.MAML.Model.MAML;
using Markdown.MAML.Transformer;
using Markdown.MAML.Renderer;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Markdown.MAML.Model.Markdown;

namespace Markdown.MAML.Test.Transformer
{
    public class MamlMultiModelMergerTests
    {
        [Fact]
        public void Merge3SimpleModels()
        {
            var merger = new MamlMultiModelMerger(null, false, "! ");
            var input = new Dictionary<string, MamlCommand>();
            input["First"] = GetModel1();
            input["Second"] = GetModel2();
            input["Third"] = GetModel3();

             var result = merger.Merge(input);

            Assert.Equal(result.Synopsis.Text, @"! First, Second

This is the synopsis

! Third

This is the synopsis 3

");

            Assert.Equal(result.Description.Text, "This is a long description.\r\nWith two paragraphs.");

            Assert.Equal(result.Notes.Text, @"! First

This is a multiline note.
Second line.
First Command

! Second

This is a multiline note.
Second line.
Second Command

! Third

This is a multiline note.
Second line.
Third Command

");

            // Links
            Assert.Equal(2, result.Links.Count);
            Assert.Equal("[foo]()\r\n\r\n", result.Links.ElementAt(0).LinkName);
            Assert.Equal("[bar]()\r\n\r\n", result.Links.ElementAt(1).LinkName);

            // Examples
            Assert.Equal(2, result.Examples.Count);
            Assert.Equal("Example 1 (Second)", result.Examples.ElementAt(0).Title);
            Assert.Equal("Example 1 (Third)", result.Examples.ElementAt(1).Title);

            // Inputs
            Assert.Equal(2, result.Inputs.Count);
            Assert.Equal("String", result.Inputs.ElementAt(0).TypeName);
            Assert.Equal("Foo", result.Inputs.ElementAt(0).Description);
            Assert.Equal("String", result.Inputs.ElementAt(1).TypeName);
            Assert.Equal("Foo 2", result.Inputs.ElementAt(1).Description);

            // Output
            Assert.Equal(1, result.Outputs.Count);
            Assert.Equal("String", result.Outputs.ElementAt(0).TypeName);
            Assert.Equal(null, result.Outputs.ElementAt(0).Description);

            // Syntax
            Assert.Equal(2, result.Syntax.Count);
            Assert.Equal("ByName", result.Syntax.ElementAt(0).ParameterSetName);
            Assert.Equal(2, result.Syntax.ElementAt(0).Parameters.Count);
            Assert.Equal("Name", result.Syntax.ElementAt(0).Parameters[0].Name);
            Assert.Equal("Remove", result.Syntax.ElementAt(0).Parameters[1].Name);

            Assert.Equal("BySomethingElse", result.Syntax.ElementAt(1).ParameterSetName);

            // Parameters
            Assert.Equal(2, result.Parameters.Count);

            Assert.Equal("Name", result.Parameters[0].Name);
            Assert.Equal(new string[] { "First", "Second", "Third" }, result.Parameters[0].Applicable);

            Assert.Equal("Remove", result.Parameters[1].Name);
            Assert.Equal(new string[] { "Third" }, result.Parameters[1].Applicable);
        }

        private MamlCommand GetModel1()
        {
            MamlCommand command = new MamlCommand()
            {
                Name = "Get-Foo",
                Synopsis = new SectionBody("This is the synopsis"),
                Description = new SectionBody("This is a long description.\r\nWith two paragraphs."),
                Notes = new SectionBody("This is a multiline note.\r\nSecond line.\r\nFirst Command")
            };

            command.Links.Add(new MamlLink(true)
            {
                LinkName = "[foo]()\r\n",
            });

            command.Inputs.Add(new MamlInputOutput()
            {
                TypeName = "String",
                Description = "Foo"

            }
            );

            command.Outputs.Add(new MamlInputOutput()
            {
                TypeName = "String",
                Description = null
            }
            );

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

            command.Parameters.Add(parameterName);

            var syntax1 = new MamlSyntax()
            {
                ParameterSetName = "ByName",
            };
            syntax1.Parameters.Add(parameterName);
            command.Syntax.Add(syntax1);

            return command;
        }

        private MamlCommand GetModel2()
        {
            MamlCommand command = new MamlCommand()
            {
                Name = "Get-Foo",
                Synopsis = new SectionBody("This is the synopsis"),
                Description = new SectionBody("This is a long description.\r\nWith two paragraphs."),
                Notes = new SectionBody("This is a multiline note.\r\nSecond line.\r\nSecond Command")
            };

            command.Links.Add(new MamlLink(true)
            {
                LinkName = "[foo]()\r\n[bar]()",
            });

            command.Examples.Add(new MamlExample()
            {
                Title = "Example 1",
                Code = new[] { new MamlCodeBlock("PS:> Get-Help -YouNeedIt") },
                Remarks = "This does stuff!"
            }
            );

            command.Inputs.Add(new MamlInputOutput()
            {
                TypeName = "String",
                Description = "Foo"

            }
            );

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

            command.Parameters.Add(parameterName);

            var syntax1 = new MamlSyntax()
            {
                ParameterSetName = "ByName",
            };
            syntax1.Parameters.Add(parameterName);
            command.Syntax.Add(syntax1);

            return command;
        }

        private MamlCommand GetModel3()
        {
            MamlCommand command = new MamlCommand()
            {
                Name = "Get-Foo",
                Synopsis = new SectionBody("This is the synopsis 3"),
                Description = new SectionBody("This is a long description.\r\nWith two paragraphs."),
                Notes = new SectionBody("This is a multiline note.\r\nSecond line.\r\nThird Command")
            };

            command.Links.Add(new MamlLink(true)
            {
                LinkName = "[bar]()",
            });

            command.Examples.Add(new MamlExample()
            {
                Title = "Example 1",
                Code = new[] { new MamlCodeBlock("PS:> Get-Help -YouNeedIt") },
                Remarks = "This does stuff too!"
            }
            );

            command.Inputs.Add(new MamlInputOutput()
            {
                TypeName = "String",
                Description = "Foo 2"

            }
            );

            command.Outputs.Add(new MamlInputOutput()
            {
                TypeName = "String",
                Description = null
            }
            );

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

            var parameterRemoved = new MamlParameter()
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

            command.Parameters.Add(parameterName);
            command.Parameters.Add(parameterRemoved);

            var syntax1 = new MamlSyntax()
            {
                ParameterSetName = "ByName",
            };
            syntax1.Parameters.Add(parameterName);
            syntax1.Parameters.Add(parameterRemoved);
            command.Syntax.Add(syntax1);

            var syntax2 = new MamlSyntax()
            {
                ParameterSetName = "BySomethingElse",
                IsDefault = true
            };
            syntax2.Parameters.Add(parameterName);
            command.Syntax.Add(syntax2);

            return command;
        }
    }

    public class MamlMultiModelMergerSyntaxTests
    {
        [Fact]
        public void MergeDefaultSyntaxAndCreateMarkdown()
        {
            // First merge two models with default syntax names 

            var merger = new MamlMultiModelMerger(null, false, "! ");
            var input = new Dictionary<string, MamlCommand>();
            input["First"] = GetModel1();
            input["Second"] = GetModel2();

            var result = merger.Merge(input);

            // Syntax
            Assert.Equal(1, result.Syntax.Count);
            Assert.Equal(null, result.Syntax.ElementAt(0).ParameterSetName);
            Assert.Equal(true, result.Syntax.ElementAt(0).IsDefault);

            // Parameters
            Assert.Equal(2, result.Parameters.Count);

            Assert.Equal("Name1", result.Parameters[0].Name);
            Assert.Equal(new string[] { "First" }, result.Parameters[0].Applicable);

            Assert.Equal("Name2", result.Parameters[1].Name);
            Assert.Equal(new string[] { "Second" }, result.Parameters[1].Applicable);

            // next render it as markdown and make sure that we don't crash

            var renderer = new MarkdownV2Renderer(MAML.Parser.ParserMode.FormattingPreserve);
            string markdown = renderer.MamlModelToString(result, true);

            int yamlLinesCount = markdown.Split('\n').Select(s => s.Trim()).Where(s => s.Equals("```yaml")).Count();
            // verify that ```yaml are all on a separate line
            Assert.Equal(2, yamlLinesCount);
        }

        private MamlCommand GetModel1()
        {
            MamlCommand command = new MamlCommand()
            {
                Name = "Get-Foo",
                Synopsis = new SectionBody("This is the synopsis"),
                Description = new SectionBody("This is a long description.\r\nWith two paragraphs."),
                Notes = new SectionBody("This is a multiline note.\r\nSecond line.\r\nFirst Command")
            };

            var parameterName = new MamlParameter()
            {
                Type = "String",
                Name = "Name1",
                Required = true,
                Description = "Parameter Description.\r\n",
                VariableLength = true,
                Globbing = true,
                PipelineInput = "True (ByValue)",
                Position = "1",
                DefaultValue = "trololo",
                Aliases = new string[] { "GF", "Foos", "Do" },
            };

            command.Parameters.Add(parameterName);

            var syntax1 = new MamlSyntax() { IsDefault = true };
            syntax1.Parameters.Add(parameterName);
            command.Syntax.Add(syntax1);

            return command;
        }

        private MamlCommand GetModel2()
        {
            MamlCommand command = new MamlCommand()
            {
                Name = "Get-Foo",
                Synopsis = new SectionBody("This is the synopsis"),
                Description = new SectionBody("This is a long description.\r\nWith two paragraphs."),
                Notes = new SectionBody("This is a multiline note.\r\nSecond line.\r\nSecond Command")
            };

            var parameterName = new MamlParameter()
            {
                Type = "String",
                Name = "Name2",
                Required = true,
                Description = "Parameter Description.\r\n",
                VariableLength = true,
                Globbing = true,
                PipelineInput = "True (ByValue)",
                Position = "1",
                DefaultValue = "trololo",
                Aliases = new string[] { "GF", "Foos", "Do" },
            };

            command.Parameters.Add(parameterName);

            var syntax1 = new MamlSyntax() { IsDefault = true };
            syntax1.Parameters.Add(parameterName);
            command.Syntax.Add(syntax1);

            return command;
        }
    }
}
