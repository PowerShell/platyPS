// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.PowerShell.PlatyPS.MAML
{
    /// <summary>
    ///     Represents a "command:inputType" or "command:returnValue" element in a Powershell MAML help document.
    /// </summary>
    public class CommandValue
    {
        /// <summary>
        ///     The value data-type.
        /// </summary>
        [XmlElement("type", Namespace = Constants.XmlNamespace.Dev, Order = 0)]
        public DataType DataType { get; set; } = new DataType();

        /// <summary>
        ///     The value's detailed description (one or more paragraphs; "maml:description/maml:para").
        /// </summary>
        [XmlArray("description", Namespace = Constants.XmlNamespace.MAML, Order = 1)]
        [XmlArrayItem("para", Namespace = Constants.XmlNamespace.MAML)]
        public List<string> Description { get; set; } = new List<string>();
    }
}
