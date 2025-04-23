using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using System.Text;
using YamlDotNet.Core;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    public class ParameterSetV2
    {
        public string Name { get; set;}
        public string Position { get; set; }
        public bool IsRequired { get; set; }
        public bool ValueFromPipeline { get; set; }
        public bool ValueFromPipelineByPropertyName { get; set; }
        public bool ValueFromRemainingArguments { get; set; }

        public ParameterSetV2()
        {
            Name = string.Empty;
            Position = string.Empty;
            IsRequired = false;
            ValueFromPipeline = false;
            ValueFromPipelineByPropertyName = false;
            ValueFromRemainingArguments = false;
        }

		public ParameterSetV2(string name, string position)
		{
			Name = name;
            Position = position;
            IsRequired = false;
            ValueFromPipeline = false;
            ValueFromPipelineByPropertyName = false;
            ValueFromRemainingArguments = false;
		}
    }

    public class ParameterMetadataV2
    {
        public string Type { get; set;}
        public string DefaultValue { get; set;}
        [YamlIgnore]
        public bool VariableLength { get; set;}
        public bool SupportsWildcards { get; set;}

        [YamlIgnore]
        public List<string> ParameterValue { get; set;}
        public List<string> Aliases { get; set;}
        public List<ParameterSetV2> ParameterSets { get; set; }
        public bool DontShow { get; set;}
        public List<string> AcceptedValues { get; set; }
        public string HelpMessage { get; set; }
        internal string? DeserializationErrorMessage { get; set; }

        public ParameterMetadataV2()
        {
            ParameterValue = new List<string>();
            Type = string.Empty;
            DefaultValue = string.Empty;
            VariableLength = true;
            SupportsWildcards = false;
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
                var result = new DeserializerBuilder().IgnoreUnmatchedProperties().Build().Deserialize<ParameterMetadataV2>(yaml);
                v2 = result;
                return true;
            }
            catch (SemanticErrorException deserializationFailure)
            {
                v2 = new ParameterMetadataV2();
                v2.DeserializationErrorMessage = GetDeserializationErrorMessage(deserializationFailure, yaml);
            }
            catch (Exception failure)
            {
                v2 = new ParameterMetadataV2();
                v2.DeserializationErrorMessage = failure.Message;
            }

            return false;
        }

        private static string GetDeserializationErrorMessage(SemanticErrorException deserializationFailure, string yaml)
        {
            var sb = Model.Constants.StringBuilderPool.Get();
            try
            {
                var errorStart = deserializationFailure.Start.Index;
                var errorEnd = deserializationFailure.End.Index;
                var length = errorEnd - errorStart;
                if (20 + length < yaml.Length)
                {
                    sb.AppendLine(deserializationFailure.Message);
                    sb.AppendLine("Error in the following YAML segment:");
                    sb.Append(new string(' ', 3));
                    sb.Append(yaml.Substring(errorStart - 20, length + 20));
                    sb.AppendLine(new string(' ', 3));
                    sb.Append(" ".PadLeft(23));
                    sb.Append(new string('^', length));
                    return sb.ToString();
                }
            }
            catch (Exception) // take no action if there's an error, we will return the generic error message below.
            {
                ;
            }
            finally
            {
                Model.Constants.StringBuilderPool.Return(sb);
            }

            return deserializationFailure.Message;
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
