// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Xml.Serialization;

namespace Microsoft.PowerShell.PlatyPS.MAML
{
    /// <summary>
    ///     Represents a "command" element in a Powershell MAML help document.
    /// </summary>
    [XmlRoot("commandLine", Namespace = Constants.XmlNamespace.Command)]
    public class CommandLine
    {
        /// <summary>
        ///     The command text.
        /// </summary>
        [XmlElement("commandText", Namespace = Constants.XmlNamespace.Command)]
        public string CommandText { get; set; } = string.Empty;
    }
}
