using System.Collections.Generic;
using Markdown.MAML.Model.Markdown;
using System;

namespace Markdown.MAML.Model.MAML
{
    public class MamlParameter
    {
        public SourceExtent Extent { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public bool Required { get; set; }

        public string Description { get; set; }

        public string DefaultValue { get; set; }

        public bool VariableLength { get; set; }

        /// <summary>
        /// Corresponds to "Accept wildcard characters"
        /// </summary>
        public bool Globbing { get; set; }

        public string PipelineInput { get; set; }

        public string Position { get; set; }

        public string[] Aliases { get; set; }

        public bool ValueRequired { get; set; }

        public bool ValueVariableLength { get; set; }

        public string AttributesText { get; set; }

        public List<string> ParameterValueGroup
        {
            get { return _parameterValueGroup; }
        }

        private readonly List<string> _parameterValueGroup = new List<string>();
 

        public MamlParameter()
        {
            VariableLength = true;
            ValueVariableLength = false;
            Globbing = false;
            PipelineInput = "false";
            Aliases = new string[] {};
        }

        public bool IsMetadataEqual(MamlParameter other)
        {
            return StringComparer.OrdinalIgnoreCase.Equals(this.Name, other.Name) &&
                this.Required == other.Required &&
                StringComparer.OrdinalIgnoreCase.Equals(this.Position, other.Position) &&
                StringComparer.OrdinalIgnoreCase.Equals(this.PipelineInput, other.PipelineInput) &&
                this.Globbing == other.Globbing;
        }
    }
}