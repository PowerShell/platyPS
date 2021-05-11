// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
            MarkdownWriterSettings settings = new(Encoding.UTF8, "SomePath");
            ModulePageWriter writer = new(settings);

            Action action = () => writer.Write(new Collection<CommandHelp>());
            ArgumentException argumentException = Assert.Throws<ArgumentOutOfRangeException>(action);
        }

        [Fact]
        public void Construct_NullPath()
        {
            MarkdownWriterSettings settings = new(Encoding.UTF8, null);
            Assert.Throws<ArgumentNullException>(() => new ModulePageWriter(settings));
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
