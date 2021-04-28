using System;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.PowerShell.PlatyPS.MarkdownWriter;
using Microsoft.PowerShell.PlatyPS.Model;
using Xunit;

namespace Microsoft.PowerShell.PlatyPS.Tests
{
    public class ModulePageWriterTests
    {
        [Fact]
        public void Write_EmptyHelpItems()
        {
            ModulePageWriter writer = new("SomePath", Encoding.UTF8);

            Action action = () => writer.Write(new Collection<CommandHelp>());
            ArgumentException argumentException = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Not enough command help items", argumentException.Message);
        }

        [Fact]
        public void Construct_NullPath()
        {
            Assert.Throws<ArgumentNullException>(() => new ModulePageWriter(modulePagePath: null, Encoding.UTF8));
        }

        [Fact]
        public void Write_OneHelpItem()
        {

        }

        [Fact]
        public void Write_TwoHelpItems()
        {

        }
    }
}
