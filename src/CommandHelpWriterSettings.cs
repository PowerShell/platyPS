// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text;

namespace Microsoft.PowerShell.PlatyPS
{
    /// <summary>
    /// Settings for the command help writers.
    /// </summary>
    internal class CommandHelpWriterSettings
    {
        internal Encoding Encoding { get; set; }
        internal string DestinationPath { get; set; }

        public CommandHelpWriterSettings(Encoding encoding, string destinationPath)
        {
            Encoding = encoding;
            DestinationPath = destinationPath;
        }
    }
}
