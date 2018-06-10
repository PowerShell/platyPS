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

            Common.AssertMultilineEqual(result.Synopsis.Text, @"! First, Second

This is the synopsis

! Third

This is the synopsis 3

");

            Assert.Equal("This is a long description.\r\nWith two paragraphs.", result.Description.Text);

            Common.AssertMultilineEqual(result.Notes.Text, @"! First

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
            Assert.Single(result.Outputs);
            Assert.Equal("String", result.Outputs.ElementAt(0).TypeName);
            Assert.Null(result.Outputs.ElementAt(0).Description);

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
            Assert.Single(result.Syntax);
            Assert.Null(result.Syntax.ElementAt(0).ParameterSetName);
            Assert.True(result.Syntax.ElementAt(0).IsDefault);

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

    public class MamlMultiModelMergerExampleTest
    {
        [Fact]
        public void Merge2ExamplesInOne()
        {
            // First merge two models with default syntax names

            var merger = new MamlMultiModelMerger(null, false, "! ");
            var input = new Dictionary<string, MamlCommand>();

            input["First"] = GetModel(
                title: "Example 1",
                code: "PS:> 1+1",
                remarks: "Hello"
            );
            input["Second"] = GetModel(
                title: "Example 1",
                code: "PS:> 1+1",
                remarks: "Hello"
            );

            var result = merger.Merge(input);

             // Examples
            Assert.Single(result.Examples);
            Assert.Equal("Example 1", result.Examples.ElementAt(0).Title);

            // next render it as markdown and make sure that we don't crash
            var renderer = new MarkdownV2Renderer(MAML.Parser.ParserMode.FormattingPreserve);
            string markdown = renderer.MamlModelToString(result, true);
        }

        [Fact]
        public void MergeExamplesOrder()
        {
            // First merge two models with default syntax names

            var merger = new MamlMultiModelMerger(null, false, "! ");
            var input = new Dictionary<string, MamlCommand>();

            input["First"] = GetModel(
                new []
                {
                    new MamlExample() { Title = "E1" },
                    new MamlExample() { Title = "E2" },
                }
            );
            input["Second"] = GetModel(
                new []
                {
                    new MamlExample() { Title = "E2" },
                    new MamlExample() { Title = "E1" },
                    new MamlExample() { Title = "E3" },
                }
            );

            var result = merger.Merge(input);

             // Examples
            Assert.Equal(3, result.Examples.Count);
            Assert.Equal("E1", result.Examples.ElementAt(0).Title);
            Assert.Equal("E2", result.Examples.ElementAt(1).Title);
            Assert.Equal("E3 (Second)", result.Examples.ElementAt(2).Title);

            // next render it as markdown and make sure that we don't crash
            var renderer = new MarkdownV2Renderer(MAML.Parser.ParserMode.FormattingPreserve);
            string markdown = renderer.MamlModelToString(result, true);
        }

        [Fact]
        public void Merge3ExamplesIn2()
        {
            // First merge two models with default syntax names

            var merger = new MamlMultiModelMerger(null, false, "! ");
            var input = new Dictionary<string, MamlCommand>();

            input["First"] = GetModel(
                title: "Example 1",
                code: "PS:> 1+1",
                remarks: "Hello"
            );
            input["Second"] = GetModel(
                title: "Example 1",
                code: "PS:> 1+1",
                remarks: "Hello"
            );

            input["Third"] = GetModel(
                title: "Example 1",
                code: "PS:> 1+1",
                remarks: "Hello world"
            );

            var result = merger.Merge(input);

             // Examples
            Assert.Equal(2, result.Examples.Count);
            Assert.Equal("Example 1 (First, Second)", result.Examples.ElementAt(0).Title);
            Assert.Equal("Hello", result.Examples.ElementAt(0).Remarks);
            Assert.Equal("Example 1 (Third)", result.Examples.ElementAt(1).Title);
            Assert.Equal("Hello world", result.Examples.ElementAt(1).Remarks);

            // next render it as markdown and make sure that we don't crash
            var renderer = new MarkdownV2Renderer(MAML.Parser.ParserMode.FormattingPreserve);
            string markdown = renderer.MamlModelToString(result, true);
        }

        private MamlCommand GetModel(string title, string code, string remarks)
        {
            MamlCommand command = new MamlCommand()
            {
                Name = "Get-Foo",
            };

            command.Examples.Add(new MamlExample()
            {
                Title = title,
                Code = new[] { new MamlCodeBlock(code) },
                Remarks = remarks,
            }
            );

            return command;
        }

        private MamlCommand GetModel(MamlExample[] examples)
        {
            MamlCommand command = new MamlCommand()
            {
                Name = "Get-Foo",
            };

            command.Examples.AddRange(examples);
            return command;
        }
    }
}
