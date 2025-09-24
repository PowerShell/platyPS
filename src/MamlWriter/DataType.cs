// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.PowerShell.PlatyPS.MAML
{
    /// <summary>
    ///     Represents a "dev:type" element in a Powershell MAML help document.
    /// </summary>
    [XmlRoot("dataType", Namespace = Constants.XmlNamespace.Dev)]
    public class DataType
    {
        /// <summary>
        ///     The data-type name.
        /// </summary>
        [XmlElement("name", Namespace = Constants.XmlNamespace.MAML, Order = 0)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        ///     The data-type URI (unused, for the most part).
        /// </summary>
        // [XmlElement("uri", Namespace = Constants.XmlNamespace.MAML, Order = 1)]
        [XmlIgnore]
        public string Uri { get; set; } = string.Empty;

        /// <summary>
        ///     The data-type's detailed description (one or more paragraphs; "maml:description/maml:para").
        /// </summary>
        // [XmlArray("description", Namespace = Constants.XmlNamespace.MAML, Order = 2)]
        // [XmlArrayItem("para", Namespace = Constants.XmlNamespace.MAML)]
        [XmlIgnore]
        public List<string> Description { get; set; } = new List<string>();

        public bool Equals(DataType other)
        {
            if (other is null)
                return false;

            return (
                string.Compare(Name, other.Name) == 0 &&
                string.Compare(Uri, other.Uri) == 0 &&
                Description.SequenceEqual(other.Description)
            );
        }

        public override bool Equals(object other)
        {
            if (other is DataType dataType2)
            {
                return Equals(dataType2);
            }

            return false;
        }

        public override int GetHashCode()
        {
            // Valid?
            return Name.GetHashCode() ^ Uri.GetHashCode();
        }

        public static bool operator == (DataType dataType1, DataType dataType2)
        {
            if (dataType1 is not null && dataType2 is not null)
            {
                return dataType1.Equals(dataType2);
            }

            return false;
        }

        public static bool operator != (DataType dataType1, DataType dataType2)
        {
            if (dataType1 is not null && dataType2 is not null)
            {
                return ! dataType1.Equals(dataType2);
            }

            return false;
        }

    }

}
