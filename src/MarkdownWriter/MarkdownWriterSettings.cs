using System.Text;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.PowerShell.PlatyPS.Tests")]
namespace Microsoft.PowerShell.PlatyPS.MarkdownWriter
{
    internal class MarkdownWriterSettings
    {
        internal Encoding Encoding { get; set; }
        internal string DestinationPath { get; set; }

        public MarkdownWriterSettings(Encoding encoding, string destinationPath)
        {
            Encoding = encoding;
            DestinationPath = destinationPath;
        }
    }
}
