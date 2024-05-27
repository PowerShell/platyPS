using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using YamlDotNet;
using YamlDotNet.Serialization;
using Microsoft.PowerShell.PlatyPS.MarkdownWriter;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    /// <summary>
    /// This is needed to manage some of the Required yaml
    /// which includes property set information with the state of required.
    /// </summary>
    public class RequiredWithParameterSetName
    {
        public string ParameterSetName = string.Empty;
        public bool Required;

        public RequiredWithParameterSetName(string name, bool required)
        {
            ParameterSetName = name;
            Required = required;
        }
    }

    public class ParameterMetadataV1
    {
		public string Type { get; set; } = string.Empty;
		[YamlMember(Alias = "Parameter Sets")]

		public string ParameterSets { get; set; } = string.Empty;

		public string Aliases { get; set; } = string.Empty;

		public string Required { get; set; } = string.Empty;

		[YamlMember(Alias = "Accepted values")]
        public string AcceptedValues { get; set; } = string.Empty;

		public string Position { get; set; } = string.Empty;

		[YamlMember(Alias = "Default value")]
		public string DefaultValue { get; set; } = string.Empty;

        [YamlMember(Alias = "Accept pipeline input")]
        public string AcceptPipelineInput { get; set; } = string.Empty;

		[YamlMember(Alias = "Accept wildcard characters")]
		public bool AcceptWildcardCharacters { get; set; }

        public string[] GetParameterSetList()
        {
            List<string> l = new List<string>();
			if (string.IsNullOrEmpty(ParameterSets))
			{
				return new string[]{ "(All)" };
			}

            foreach(var p in ParameterSets.Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries))
            {
                l.Add(p.Trim());
            }
            return l.ToArray();
        }

        public bool TryGetRequiredAsBool(out bool value)
        {
            if (bool.TryParse(Required, out var result))
            {
                value = result;
                return true;
            }

            value = false;
            return false;
        }

        public static Regex RequiredTruePattern = new Regex(@"True \((.[^\)]+)\)", RegexOptions.IgnoreCase);
        public List<string> GetRequiredParameterSets()
        {
            List<string> l = new();

            var trueMatch = RequiredTruePattern.Match(Required ?? string.Empty);
            if (trueMatch.Success)
            {
                string foundParameterSets = trueMatch.Groups[1].Value.Trim();
                if (! string.IsNullOrEmpty(foundParameterSets))
                {
                    foreach(var parameterSet in foundParameterSets.Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries))
                    {
                        var pSetName = parameterSet.Trim();
                        if (! string.IsNullOrEmpty(pSetName))
                        {
                            l.Add(pSetName);
                        }
                    }
                }
            }
            else
            {
                l.Add(Constants.ParameterSetsAll);
            }

            return l;
        }

        // Get the list of accepted values.
        // note we might read an string of spaces, so we need to trim
        // and then return non-empty strings.
        public List<string> GetAcceptedValues()
        {
            List<string> l = new();
            if (AcceptedValues is null)
            {
                return l;
            }

            foreach(string v in AcceptedValues.Split(Constants.Comma))
            {
                var trimmedValue = v.Trim();
                if (! string.IsNullOrEmpty(trimmedValue))
                {
                    l.Add(trimmedValue);
                }
            }

            return l;
        }

        public bool ParameterSetIncludes(string name)
        {
            foreach(var p in ParameterSets.Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries))
            {
                if (string.Compare(p.Trim(), name, true) == 0)
                {
                    return true;
                }
            }
            return false;
        }

		public string[] GetAliases()
		{
			if (string.IsNullOrEmpty(Aliases))
			{
				return new string[] { };
			}

            List<string> l = new List<string>();
            foreach(var p in Aliases.Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries))
            {
                l.Add(p.Trim());
            }
            return l.ToArray();
		}

        // We have a number of things to check because there's no consistency in the
        // documentation repositories.
        private Regex trueNameRegex = new Regex(@"^true \(.*ByProperty", RegexOptions.IgnoreCase);
        private Regex trueValueRegex = new Regex(@"^true \(.*ByValue", RegexOptions.IgnoreCase);
        private Regex valueRegex = new Regex(@"^ByValue \((?<value>\w+)\), ByName \((?<name>\w+)\)", RegexOptions.IgnoreCase);

		public bool GetByValue(string byValueAndProperty)
		{
            // This might be one of the following:
            // ByValue (False), ByName (False)
            // ByValue (False), ByName (True)
            // ByValue (System.Object[]), ByName (System.Object[])
            // ByValue (True), ByName (False)
            // False
            // True (ByPropertyName, ByValue)
            // True (ByPropertyName)
            // True (ByValue)

            if (string.IsNullOrEmpty(byValueAndProperty))
            {
                return false;
            }
    
            var stringValue = byValueAndProperty.Trim();

			if (string.Compare(stringValue, "true", true) == 0)
			{
				return true;
			}

			if (string.Compare(stringValue, "false", true) == 0)
			{
				return false;
			}

            Match mInfo;
            mInfo = trueValueRegex.Match(stringValue);
            if (mInfo.Success)
            {
                return true;
            }

            mInfo = valueRegex.Match(stringValue);
            if (mInfo.Success)
            {
                if(string.Compare(mInfo.Groups["value"].Value, "true", true) == 0)
                {
                    return true;
                }
            }

			return false;
		}

		public bool GetByProperty(string byValueAndProperty)
		{
            if (string.IsNullOrEmpty(byValueAndProperty))
            {
                return false;
            }
    
			var stringValue = byValueAndProperty.Trim();
            // If it just says "True"
			if (string.Compare(stringValue, "true", true) == 0)
			{
				return true;
			}

			if (string.Compare(stringValue, "false", true) == 0)
			{
				return false;
			}

            Match mInfo;
            mInfo = trueNameRegex.Match(stringValue);
            if (mInfo.Success)
            {
                return true;
            }

            mInfo = valueRegex.Match(stringValue);
            if (mInfo.Success)
            {
                if(string.Compare(mInfo.Groups["name"].Value, "true", true) == 0)
                {
                    return true;
                }
            }

			return false;
		}

        public static bool TryConvertToV1(string yaml, out ParameterMetadataV1 v1)
        {
            try
            {
                var result = new DeserializerBuilder().Build().Deserialize<ParameterMetadataV1>(yaml);
                v1 = result;
                return true;
            }
            catch
            {
                ; // do nothing we couldn't parse the yaml, and we'll return false.
            }

            v1 = new ParameterMetadataV1();
            return false;
        }

        public bool TryConvertMetadataToV2(out ParameterMetadataV2 v2)
        {
            var result = new ParameterMetadataV2();
			result.Type = Type;
			result.DefaultValue = DefaultValue;
			result.SupportsWildcards = AcceptWildcardCharacters;
			result.Aliases.AddRange(GetAliases());
			result.DontShow = false;
            result.AcceptedValues = GetAcceptedValues();
			foreach(var pSetName in GetParameterSetList())
			{
				var pSetV2 = new ParameterSetV2(pSetName, Position);
				pSetV2.ValueByPipeline = GetByValue(AcceptPipelineInput);
				pSetV2.ValueByPipelineByPropertyName = GetByProperty(AcceptPipelineInput);
                if (TryGetRequiredAsBool(out var required))
                {
                    pSetV2.IsRequired = required;
                }
                else
                {
                    if(GetRequiredParameterSets().Any(p => string.Compare(p, pSetName) == 0))
                    {
                        pSetV2.IsRequired = true;
                    }
                }
				result.ParameterSets.Add(pSetV2);
			}

            v2 = result;
            return true;
        }

        public string ToYamlString()
        {
			StringBuilder sb = new();
			sb.AppendLine("```yaml");
			sb.Append(new SerializerBuilder().Build().Serialize(this));
			sb.AppendLine("```");
			return sb.ToString();
        }

        /// <summary>
        /// The last ditch effort for converting a dictionary to a metadata object
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static ParameterMetadataV1 ConvertDictionaryToParameterMetadataV1(Dictionary<string, string>dict)
        {
            ParameterMetadataV1 metadata = new ();
            return metadata; 
        }
    }
}
