using System.Text;
using Microsoft.PowerShell.PlatyPS.MarkdownWriter;
using Xunit;

namespace Microsoft.PowerShell.PlatyPS.Tests
{
    public class MarkdownWriterSettingsTest
    {
        [Fact]
        public void MarkdownSettingsReadWrite()
        {
            var expectedPath = @"/somefolder/somepath";
            MarkdownWriterSettings markdownWriterSettings = new(Encoding.UTF8, expectedPath);
            Assert.Equal(Encoding.UTF8, markdownWriterSettings.Encoding);
            Assert.Equal(expectedPath, markdownWriterSettings.DestinationPath);
        }
    }
}
