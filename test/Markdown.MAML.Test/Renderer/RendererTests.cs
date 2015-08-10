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
                PipelineInput = true,
                Position = "1",
                Aliases = new string []{"GF","Foos","Do"},
                ValueRequired = false,
                ValueVariableLength = false
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
    }
   
}
