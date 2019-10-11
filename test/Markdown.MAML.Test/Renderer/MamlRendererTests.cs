using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using Markdown.MAML.Parser;
using Markdown.MAML.Renderer;
using Markdown.MAML.Transformer;
using Xunit;
using Markdown.MAML.Test.EndToEnd;
using Markdown.MAML.Model.MAML;
using Markdown.MAML.Model.Markdown;

namespace Markdown.MAML.Test.Renderer
{
    public class MamlRendererTests
    {
        [Fact]
        public void RendererProduceNameAndSynopsis()
        {
            MamlRenderer renderer = new MamlRenderer();
            MamlCommand command = new MamlCommand()
            {
                Name = "Get-Foo",
                Synopsis = new SectionBody("This is the synopsis"),
                Description = new SectionBody("This is a long description.")
            };

            command.Parameters.Add(new MamlParameter()
            {
                Type = "String",
                Name = "Name",
                Required = true,
                Description = "This is the name parameter description.",
                VariableLength = true,
                Globbing = true,
                PipelineInput = "True (ByValue)",
                Position = "1",
                Aliases = new string []{"GF","Foos","Do"},
            }
            );
            command.Parameters.Add(new MamlParameter()
            {
                Type = "String",
                Name = "Path",
                Required = true,
                Description = "This is the path parameter description.",
                VariableLength = true,
                Globbing = true,
                PipelineInput = "True (ByValue)",
                Position = "2",
                Aliases = new string[] {  },
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
                Code = new[] { new MamlCodeBlock("PS:> Get-Help -YouNeedIt") },
                Remarks = "This does stuff!"
            });
            command.Examples.Add(new MamlExample()
            {
                Title = "Example 2",
                Code = new[] {
                    new MamlCodeBlock("PS:> Get-Help -YouNeedIt", "powershell"),
                    new MamlCodeBlock("Output")},
                Remarks = "This does stuff!"
            });
            command.Links.Add(new MamlLink()
            {
                    LinkName = "PowerShell made by Microsoft Hackathon",
                    LinkUri = "www.microsoft.com"
            }
            );

            string maml = renderer.MamlModelToString(new [] {command});

            string[] name = EndToEndTests.GetXmlContent(maml, "/msh:helpItems/command:command/command:details/command:name");
            Assert.Single(name);
            Assert.Equal("Get-Foo", name[0]);

            string[] synopsis = EndToEndTests.GetXmlContent(maml, "/msh:helpItems/command:command/command:details/maml:description/maml:para");
            Assert.Single(synopsis);
            Assert.Equal("This is the synopsis", synopsis[0]);

            string[] description = EndToEndTests.GetXmlContent(maml, "/msh:helpItems/command:command/maml:description/maml:para");
            Assert.Single(description);
            Assert.Equal("This is a long description.", description[0]);

            string[] parameter1 = EndToEndTests.GetXmlContent(maml, "/msh:helpItems/command:command/command:parameters/command:parameter[maml:name='Name']/maml:description/maml:para");
            Assert.Single(parameter1);
            Assert.Equal("This is the name parameter description.", parameter1[0]);

            string[] parameter2 = EndToEndTests.GetXmlContent(maml, "/msh:helpItems/command:command/command:parameters/command:parameter[maml:name='Path']/maml:description/maml:para");
            Assert.Single(parameter2);
            Assert.Equal("This is the path parameter description.", parameter2[0]);

            string[] example1 = EndToEndTests.GetXmlContent(maml, "/msh:helpItems/command:command/command:examples/command:example[contains(maml:title,'Example 1')]/dev:code");
            Assert.Single(example1);
            Assert.Equal("PS:> Get-Help -YouNeedIt", example1[0]);

            // Check that multiple code blocks in the same example merge together when rendering maml
            string[] example2 = EndToEndTests.GetXmlContent(maml, "/msh:helpItems/command:command/command:examples/command:example[contains(maml:title,'Example 2')]/dev:code");
            Assert.Single(example2);
            Common.AssertMultilineEqual("PS:> Get-Help -YouNeedIt\r\n\r\nOutput", example2[0]);
        }

        [Fact]
        public void RendererProduceSyntaxAndParameter()
        {
            MamlRenderer renderer = new MamlRenderer();
            MamlCommand command = new MamlCommand()
            {
                Name = "Get-Foo",
            };

            var param1 = new MamlParameter()
            {
                Type = "String",
                Name = "Param1",
                Position = ""
            };

            var param2 = new MamlParameter()
            {
                Type = "System.Int32",
                Name = "Param2",
                Position = "Named"
            };

            command.Parameters.Add(param1);
            command.Parameters.Add(param2);

            var syntax = new MamlSyntax();
            syntax.Parameters.Add(param1);
            syntax.Parameters.Add(param2);
            command.Syntax.Add(syntax);

            string maml = renderer.MamlModelToString(new[] { command });

            string[] syntaxItemName = EndToEndTests.GetXmlContent(maml, "/msh:helpItems/command:command/command:syntax/command:syntaxItem/maml:name");
            Assert.Single(syntaxItemName);
            Assert.Equal("Get-Foo", syntaxItemName[0]);

            string[] nameSyntax = EndToEndTests.GetXmlContent(maml, "/msh:helpItems/command:command/command:syntax/command:syntaxItem/command:parameter/maml:name");
            Assert.Equal(2, nameSyntax.Length);
            Assert.Equal("Param1", nameSyntax[0]);
            Assert.Equal("Param2", nameSyntax[1]);

            string[] nameParam = EndToEndTests.GetXmlContent(maml, "/msh:helpItems/command:command/command:parameters/command:parameter/maml:name");
            Assert.Equal(2, nameParam.Length);
            Assert.Equal("Param1", nameParam[0]);
            Assert.Equal("Param2", nameParam[1]);
        }

        [Fact]
        public void RendererProduceEscapeXmlSpecialChars()
        {
            MamlRenderer renderer = new MamlRenderer();
            MamlCommand command = new MamlCommand()
            {
                Name = "Get-Foo",
                Synopsis = new SectionBody("<port&number>") // < and > should be properly escaped
            };
            
            string maml = renderer.MamlModelToString(new[] { command });

            string[] synopsis = EndToEndTests.GetXmlContent(maml, "/msh:helpItems/command:command/command:details/maml:description/maml:para");
            Assert.Single(synopsis);
            Assert.Equal(command.Synopsis.Text, synopsis[0]);
        }

        [Fact]
        public void RendererProducePaddedExampleTitle()
        {
            MamlRenderer renderer = new MamlRenderer();
            MamlCommand command = new MamlCommand()
            {
                Name = "Get-Foo",
                Synopsis = new SectionBody("This is a description")
            };

            var example1 = new MamlExample()
            {
                Title = "Example 1",
                Code = new[] { new MamlCodeBlock("PS:> Get-Help -YouNeedIt") },
                Remarks = "This does stuff!"
            };

            var example10 = new MamlExample()
            {
                Title = "Example 10",
                Code = new[] { new MamlCodeBlock("PS:> Get-Help -YouNeedIt") },
                Remarks = "This does stuff!"
            };

            var exampleWithTitle = new MamlExample()
            {
                Title = "Example 11: With a title",
                Code = new[] { new MamlCodeBlock("PS:> Get-Help -YouNeedIt") },
                Remarks = "This does stuff!"
            };

            var exampleWithLongTitle = new MamlExample()
            {
                Title = "Example 12: ".PadRight(66, 'A'),
                Code = new[] { new MamlCodeBlock("PS:> Get-Help -YouNeedIt") },
                Remarks = "This does stuff!"
            };

            command.Examples.Add(example1);
            command.Examples.Add(example10);
            command.Examples.Add(exampleWithTitle);
            command.Examples.Add(exampleWithLongTitle);

            string maml = renderer.MamlModelToString(new[] { command });

            // Check that example header is padded by dashes (-) unless to long
            string[] example = EndToEndTests.GetXmlContent(maml, "/msh:helpItems/command:command/command:examples/command:example/maml:title");
            Assert.Equal(4, example.Length);
            Assert.Equal(63, example[0].Length);
            Assert.Equal(64, example[1].Length);
            Assert.Equal(66, example[3].Length);
            Assert.Matches($"^-+ {example1.Title} -+$", example[0]);
            Assert.Matches($"^-+ {example10.Title} -+$", example[1]);
            Assert.Matches($"^-+ {exampleWithTitle.Title} -+$", example[2]);
            Assert.Matches($"^{exampleWithLongTitle.Title}$", example[3]);
        }
    }

}
