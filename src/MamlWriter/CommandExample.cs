// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.PowerShell.PlatyPS.MAML
{
    /// <summary>
    ///     Represents a "command:example" element in a Powershell MAML help document.
    /// </summary>
    public class CommandExample
    {
        /// <summary>
        ///     The command details.
        /// </summary>
        [XmlElement("title", Namespace = Constants.XmlNamespace.MAML, Order = 0)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        ///     An introduction to the example (one or more paragraphs; "maml:introduction/maml:para").
        /// </summary>
        [XmlArray("introduction", Namespace = Constants.XmlNamespace.MAML, Order = 1)]
        [XmlArrayItem("para", Namespace = Constants.XmlNamespace.MAML)]
        public List<string> Description { get; set; } = new List<string>();

        /// <summary>
        ///     The example code.
        /// </summary>
        [XmlElement("code", Namespace = Constants.XmlNamespace.Dev, Order = 2)]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        ///     An introduction to the example (one or more paragraphs; "dev:remarks/maml:para").
        /// </summary>
        [XmlArray("remarks", Namespace = Constants.XmlNamespace.Dev, Order = 3)]
        [XmlArrayItem("para", Namespace = Constants.XmlNamespace.MAML)]
        public List<string> Remarks { get; set; } = new List<string>();
    }
}
