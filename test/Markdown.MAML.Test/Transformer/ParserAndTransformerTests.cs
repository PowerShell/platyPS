using System.Collections.Generic;
using System.Linq;
using Markdown.MAML.Model.MAML;
using Markdown.MAML.Parser;
using Markdown.MAML.Renderer;
using Markdown.MAML.Transformer;
using Xunit;

namespace Markdown.MAML.Test.Transformer
{
    public class ParserAndTransformerTests
    {
        [Fact]
        public void TransformSimpleCommand()
        {
            var parser = new MarkdownParser();
            var doc = parser.ParseString(@"
## Get-Foo
### Synopsis
This is Synopsis
");
            MamlCommand mamlCommand = (new ModelTransformer()).NodeModelToMamlModel(doc).First();
            Assert.Equal(mamlCommand.Name, "Get-Foo");
            Assert.Equal(mamlCommand.Synopsis, "This is Synopsis");
        }

        [Fact]
        public void TransformMultilineDescription()
        {
            var parser = new MarkdownParser();
            var doc = parser.ParseString(@"
## Get-Foo
### Synopsis
This is Synopsis, but it doesn't matter in this test

### DESCRIPTION
Hello,

I'm a multiline description.

And this is my last line.
");
            string maml = new MamlRenderer().
                MamlModelToString((new ModelTransformer()).
                NodeModelToMamlModel(doc));

            MamlCommand mamlCommand = (new ModelTransformer()).NodeModelToMamlModel(doc).First();
            string[] description = mamlCommand.Description.Split('\n').Select(x => x.Trim()).ToArray();
            Assert.Equal(5, description.Length);
            Assert.Equal("Hello,", description[0]);
            Assert.Equal("", description[1]);
            Assert.Equal("I'm a multiline description.", description[2]);
            Assert.Equal("", description[3]);
            Assert.Equal("And this is my last line.", description[4]);
        }
    }
}
