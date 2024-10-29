// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.PowerShell.PlatyPS.MAML
{
    /// <summary>
    ///     Represents a "parameter" element in a Powershell MAML help document.
    /// </summary>
    [XmlRoot("parameter", Namespace = Constants.XmlNamespace.Command)]
    public class Parameter
    {
        /// <summary>
        ///     The parameter name ("maml:name").
        /// </summary>
        [XmlElement("name", Namespace = Constants.XmlNamespace.MAML, Order = 0)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        ///     The parameter's detailed description (one or more paragraphs; "maml:description/maml:para").
        /// </summary>
        [XmlArray("description", Namespace = Constants.XmlNamespace.MAML, Order = 1)]
        [XmlArrayItem("para", Namespace = Constants.XmlNamespace.MAML)]
        public List<string> Description { get; set; } = new List<string>();

        /// <summary>
        ///     The parameter value information ("command:parameterValue").
        /// </summary>
        [XmlElement("parameterValue", Namespace = Constants.XmlNamespace.Command, Order = 2)]
        public ParameterValue? Value { get; set; }

        /// <summary>
        ///     The parameter value type information ("dev:type").
        /// </summary>
        [XmlElement("type", Namespace = Constants.XmlNamespace.Dev, Order = 3)]
        public DataType Type { get; set; } = new DataType();

        /// <summary>
        ///     Is the parameter mandatory?
        /// </summary>
        [XmlAttribute("required")]
        public bool IsMandatory { get; set; }

        [XmlAttribute("variableLength")]
        public bool VariableLength { get; set; }

        /// <summary>
        ///     Does the parameter support globbing (wildcards)?
        /// </summary>
        [XmlAttribute("globbing")]
        public bool SupportsGlobbing { get; set; }

        /// <summary>
        ///     Can the parameter accept its value from the pipeline?
        /// </summary>
        /// <remarks>
        ///     Either "true (ByValue)", "true (ByPropertyName)", or "false".
        /// </remarks>
        [XmlAttribute("pipelineInput")]
        public string PipelineInputString
        {
            set { throw new System.NotImplementedException(); }
            get
            {
                if (SupportsPipelineInput == PipelineInputType.ByValue && SupportsPipelineInput == PipelineInputType.ByPropertyName)
                {
                    return "true (ByValue, ByPropertyName)";
                }
                else if (SupportsPipelineInput == PipelineInputType.ByValue)
                {
                    return "true (ByValue)";
                }
                else if (SupportsPipelineInput == PipelineInputType.ByPropertyName)
                {
                    return "true (ByPropertyName)";
                }
                else
                {
                    return "false";
                }
            }
        }

        [XmlIgnore]
        public PipelineInputType SupportsPipelineInput { get; set; }

        /// <summary>
        ///     The parameter's position.
        /// </summary>
        /// <remarks>
        ///     Either "named", or the 0-based position of the parameter.
        /// </remarks>
        [XmlAttribute("position")]
        public string Position { get; set; } = "named";

        /// <summary>
        ///     The parameter's aliases.
        /// </summary>
        [XmlAttribute("aliases")]
        public string Aliases { get; set; } = "none";
    }
}
