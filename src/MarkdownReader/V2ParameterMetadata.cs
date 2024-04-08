using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using System.Text;

namespace Microsoft.PowerShell.PlatyPS
{
    public class ParameterSetV2
    {
        public string Name { get; set;}
        public string Position { get; set; }
        public bool IsRequired { get; set; }
        public bool ValueByPipeline { get; set; }
        public bool ValueByPipelineByPropertyName { get; set; }
        public bool ValueFromRemainingArguments { get; set; }

        public ParameterSetV2()
        {
            Name = string.Empty;
            Position = string.Empty;
            IsRequired = false;
            ValueByPipeline = false;
            ValueByPipelineByPropertyName = false;
            ValueFromRemainingArguments = false;
        }

		public ParameterSetV2(string name, string position)
		{
			Name = name;
            Position = position;
            IsRequired = false;
            ValueByPipeline = false;
            ValueByPipelineByPropertyName = false;
            ValueFromRemainingArguments = false;
		}
    }

    public class ParameterMetadataV2
    {
        public string Type { get; set;}
        public string DefaultValue { get; set;}
        public bool VariableLength { get; set;}
        public bool Globbing { get; set;}
        public List<string> ParameterValue { get; set;}
        public List<string> Aliases { get; set;}
        public List<ParameterSetV2> ParameterSets { get; set; }
        public bool DontShow { get; set;}
        public List<string> AcceptedValues { get; set; }
        public string HelpMessage { get; set; }

        public ParameterMetadataV2()
        {
            ParameterValue = new List<string>();
            Type = string.Empty;
            DefaultValue = string.Empty;
            VariableLength = true;
            Globbing = false;
            Aliases = new List<string>();
            ParameterSets = new List<ParameterSetV2>();
            DontShow = false;
            AcceptedValues = new List<string>();
            HelpMessage = string.Empty;
        }

        public static ParameterMetadataV2 ConvertFromV1(ParameterMetadataV1 v1p)
        {
            return new ParameterMetadataV2();
        }

        public static bool TryConvertToV2(string yaml, out ParameterMetadataV2 v2)
        {
            try
            {
                var result = new DeserializerBuilder().Build().Deserialize<ParameterMetadataV2>(yaml);
                v2 = result;
                return true;
            }
            catch
            {
                ; // do nothing we couldn't parse the yaml, and we'll return false.
            }

            v2 = new ParameterMetadataV2();
            return false;
        }

		public string ToYamlString()
		{
			StringBuilder sb = new();
			sb.AppendLine("```yaml");
			sb.Append(new SerializerBuilder().Build().Serialize(this));
			sb.AppendLine("```");
			return sb.ToString();
		}
    }
}
