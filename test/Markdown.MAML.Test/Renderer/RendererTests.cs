using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using Markdown.MAML.Parser;
using Markdown.MAML.Renderer;
using Markdown.MAML.Transformer;
using Xunit;
using Markdown.MAML.Test.EndToEnd;
using Markdown.MAML.Model.MAML;

namespace Markdown.MAML.Test.Renderer
{
    public class RendererTests
    {
        [Fact]
        public void RendererProduceNameAndSynopsis()
        {
            MamlRenderer renderer = new MamlRenderer();
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
                Aliases = new string []{"GF","Foos","Do"},
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

            string maml = renderer.MamlModelToString(new [] {command});

            string[] name = EndToEndTests.GetXmlContent(maml, "/helpItems/command:command/command:details/command:name");
            Assert.Equal(1, name.Length);
            Assert.Equal("Get-Foo", name[0]);

            string[] synopsis = EndToEndTests.GetXmlContent(maml, "/helpItems/command:command/command:details/maml:description/maml:para");
            Assert.Equal(1, synopsis.Length);
            Assert.Equal("This is the synopsis", synopsis[0]);
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
                Position = "Named"
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

            string[] syntaxItemName = EndToEndTests.GetXmlContent(maml, "/helpItems/command:command/command:syntax/command:syntaxItem/maml:name");
            Assert.Equal(1, syntaxItemName.Length);
            Assert.Equal("Get-Foo", syntaxItemName[0]);

            string[] nameSyntax = EndToEndTests.GetXmlContent(maml, "/helpItems/command:command/command:syntax/command:syntaxItem/command:parameter/maml:name");
            Assert.Equal(2, nameSyntax.Length);
            Assert.Equal("Param1", nameSyntax[0]);
            Assert.Equal("Param2", nameSyntax[1]);

            string[] nameParam = EndToEndTests.GetXmlContent(maml, "/helpItems/command:command/command:parameters/command:parameter/maml:name");
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
                Synopsis = "<port&number>" // < and > should be properly escaped
            };
            
            string maml = renderer.MamlModelToString(new[] { command });

            string[] synopsis = EndToEndTests.GetXmlContent(maml, "/helpItems/command:command/command:details/maml:description/maml:para");
            Assert.Equal(1, synopsis.Length);
            Assert.Equal(synopsis[0], command.Synopsis);
        }

    }

}
