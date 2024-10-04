// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.PowerShell.PlatyPS.MAML
{
    /// <summary>
    ///     Represents a "command:syntaxItem" element in a Powershell MAML help document.
    /// </summary>
    public class AlertItem
    {
        /// <summary>
        ///     The command name for this syntax.
        /// </summary>
        [XmlElement("para", Namespace = Constants.XmlNamespace.MAML, Order = 0)]
        public List<string> Remark { get; set; }

        public AlertItem()
        {
            Remark = new();
        }

    }
}
