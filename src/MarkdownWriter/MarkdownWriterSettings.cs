using Microsoft.PowerShell.PlatyPS.Model;
using System.Text;

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
