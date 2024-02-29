// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.PowerShell.PlatyPS.MAML
{
    /// <summary>
    ///     Represents the "details" element under a "command" element in a Powershell MAML help document.
    /// </summary>
    [XmlRoot("details", Namespace = Constants.XmlNamespace.Command)]
    public class CommandDetails
    {
        /// <summary>
        ///     The Cmdlet name.
        /// </summary>
        [XmlElement("name", Namespace = Constants.XmlNamespace.Command, Order = 0)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        ///     The command synopsis (one or more paragraphs; "maml:description/maml:para").
        /// </summary>
        [XmlArray("description", Namespace = Constants.XmlNamespace.MAML, Order = 1)]
        [XmlArrayItem("para", Namespace = Constants.XmlNamespace.MAML)]
        public List<string> Synopsis { get; set; } = new List<string>();

        /// <summary>
        ///     The verb component of the Cmdlet's name.
        /// </summary>
        [XmlElement("verb", Namespace = Constants.XmlNamespace.Command, Order = 2)]
        public string Verb { get; set; } = string.Empty;

        /// <summary>
        ///     The noun component of the Cmdlet's name.
        /// </summary>
        [XmlElement("noun", Namespace = Constants.XmlNamespace.Command, Order = 3)]
        public string Noun { get; set; } = string.Empty;
    }
}
