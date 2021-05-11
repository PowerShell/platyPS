// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text;

namespace Microsoft.PowerShell.PlatyPS.MarkdownWriter
{
    /// <summary>
    /// Settings for the markdown writer.
    /// </summary>
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
