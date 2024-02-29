using System;
using System.Xml.Serialization;

namespace Microsoft.PowerShell.PlatyPS.MAML
{
    /// <summary>
    ///     Supported Powershell pipeline input types for Cmdlet parameters.
    /// </summary>
    [Flags]
    public enum PipelineInputType
    {
        /// <summary>
        ///     Parameter does not take its value from the pipeline.
        /// </summary>
        [XmlEnum("false")]
        None = 0,

        /// <summary>
        ///     Parameter can take its value from the pipeline.
        /// </summary>
        [XmlEnum("true (ByValue)")]
        ByValue = 1,

        /// <summary>
        ///     Parameter can take its value from a property of the same name on objects in the pipeline.
        /// </summary>
        [XmlEnum("true (ByPropertyName)")]
        ByPropertyName = 2
    }
}
