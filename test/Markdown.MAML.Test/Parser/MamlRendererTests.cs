using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Markdown.MAML.Model;
using Markdown.MAML.Parser;
using Xunit;

namespace Markdown.MAML.Test.Parser
{
    public class MamlRendererTests
    {
        [Fact]
        public void EmptyMarkdownProduceEmptyMaml()
        {
            var renderer = new MamlRenderer(new DocumentNode());
            string maml = renderer.ToMamlString();
            Assert.Equal(maml, @"<command:command xmlns:maml=""http://schemas.microsoft.com/maml/2004/10"" xmlns:command=""http://schemas.microsoft.com/maml/dev/command/2004/10"" xmlns:dev=""http://schemas.microsoft.com/maml/dev/2004/10"" xmlns:MSHelp=""http://msdn.microsoft.com/mshelp"">");
        }
    }
}
