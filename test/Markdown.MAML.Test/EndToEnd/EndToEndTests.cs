using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using Markdown.MAML.Renderer;
using Xunit;

namespace Markdown.MAML.Test.EndToEnd
{
    public class EndToEndTests
    {
        [Fact]
        public void ProduceNameAndSynopsis()
        {
            string maml = MamlRenderer.MarkdownStringToMamlString(@"
## Get-Foo
### Synopsis
This is Synopsis
");
            string[] name = GetXmlContent(maml, "/helpItems/command:command/command:details/command:name");
            Assert.Equal(1, name.Length);
            Assert.Equal("Get-Foo", name[0]);

            string[] synopsis = GetXmlContent(maml, "/helpItems/command:command/command:details/maml:description/maml:para");
            Assert.Equal(1, synopsis.Length);
            Assert.Equal("This is Synopsis", synopsis[0]);
        }

        [Fact]
        public void ProduceMultilineDescription()
        {
            string maml = MamlRenderer.MarkdownStringToMamlString(@"
## Get-Foo
### Synopsis
This is Synopsis, but it doesn't matter in this test

### DESCRIPTION
Hello,

I'm a multiline description.

And this is my last line.
");

            string[] description = GetXmlContent(maml, "/helpItems/command:command/maml:description/maml:para");
            Assert.Equal(5, description.Length);
        }

        public string[] GetXmlContent(string xml, string xpath)
        {
            List<string> result = new List<string>(); 
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            var nav = xmlDoc.CreateNavigator();


            XmlNamespaceManager xmlns = new XmlNamespaceManager(nav.NameTable);
            xmlns.AddNamespace("command", "http://schemas.microsoft.com/maml/dev/command/2004/10");
            xmlns.AddNamespace("maml", "http://schemas.microsoft.com/maml/2004/10");
            xmlns.AddNamespace("dev", "http://schemas.microsoft.com/maml/dev/2004/10");
            xmlns.AddNamespace("MSHelp", "http://msdn.microsoft.com/mshelp");

            XPathNodeIterator iterator = nav.Select(xpath, xmlns);
            foreach (var i in iterator)
            {
                result.Add(i.ToString().Trim());
            }

            return result.ToArray();
        }
    }
}
