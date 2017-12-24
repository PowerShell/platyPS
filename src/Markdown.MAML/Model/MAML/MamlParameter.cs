using System.Collections.Generic;
using Markdown.MAML.Model.Markdown;
using System;
using System.Linq;

namespace Markdown.MAML.Model.MAML
{
    public class MamlParameter
    {
        public SourceExtent Extent { get; set; }

        /// <summary>
        /// Additional options that determine how the section will be formated when rendering markdown.
        /// </summary>
        public SectionFormatOption FormatOption { get; set; }

        public string Type { get; set; }

        public string FullType { get; set; }

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

        public string[] Applicable { get; set; }

        public bool ValueRequired { get; set; }

        public bool ValueVariableLength { get; set; }

        /// <summary>
        /// This string is used only in schema version 1.0.0 internal processing
        /// </summary>
        internal string AttributesMetadata { get; set; }

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
            Position = "Named";
            Aliases = new string[] {};
        }

        public MamlParameter Clone()
        {
            return (MamlParameter)this.MemberwiseClone();
        }

        public bool IsMetadataEqual(MamlParameter other)
        {
            return StringComparer.OrdinalIgnoreCase.Equals(this.Name, other.Name) &&
                this.Required == other.Required &&
                StringComparer.OrdinalIgnoreCase.Equals(this.Position, other.Position) &&
                StringComparer.OrdinalIgnoreCase.Equals(this.PipelineInput, other.PipelineInput) &&
                this.Globbing == other.Globbing;
        }

        public bool IsSwitchParameter()
        {
            return StringComparer.OrdinalIgnoreCase.Equals(this.Type, "SwitchParameter") ||
                StringComparer.OrdinalIgnoreCase.Equals(this.Type, "switch");
        }

        public bool IsNamed()
        {
            return string.IsNullOrWhiteSpace(this.Position) ||
                StringComparer.OrdinalIgnoreCase.Equals(this.Position, "Named");
        }

        public bool IsApplicable(string[] applicableTag)
        {
            if (applicableTag != null && this.Applicable != null)
            {
                // applicable if intersect is not empty
                return applicableTag.Intersect(this.Applicable, StringComparer.OrdinalIgnoreCase).Any();
            }

            // if one is null then it's applicable
            return true;
        }
    }
}