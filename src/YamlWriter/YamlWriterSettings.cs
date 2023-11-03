// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text;

namespace Microsoft.PowerShell.PlatyPS.YamlWriter
{
    /// <summary>
    /// Settings for the yaml writer.
    /// </summary>
    internal class YamlWriterSettings
    {
        internal Encoding Encoding { get; set; }
        internal string DestinationPath { get; set; }

        public YamlWriterSettings(Encoding encoding, string destinationPath)
        {
            Encoding = encoding;
            DestinationPath = destinationPath;
        }
    }
}
