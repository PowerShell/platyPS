// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using YamlDotNet.Serialization;

namespace Microsoft.PowerShell.PlatyPS.Model
{
    /// <summary>
    /// Class to represent the properties of a parameter in PowerShell help.
    /// </summary>
    public class Parameter : IEquatable<Parameter>
    {
        public string Name { get; set;}

        public string Type { get; set;}

        public string Description { get; set;}

        [YamlIgnore]
        public List<string> ParameterValue { get; set;}

        public string DefaultValue { get; set;}

        private List<string>? RequiredTrueParameterSets { get; set; }
        private List<string>? RequiredFalseParameterSets { get; set; }

        [YamlIgnore]
        public bool VariableLength { get; set;} = true;

        public bool SupportsWildcards { get; set;}

        public bool IsDynamic { get; set ; }

        public string Aliases { get; set;}

        public bool DontShow { get; set;}

        public List<string> AcceptedValues { get; private set; }

        public List<ParameterSet> ParameterSets { get; set; }

        public string HelpMessage { get; set; }

        public Parameter()
        {
            Name = string.Empty;
            Type = string.Empty;
            Description = string.Empty;
            ParameterSets = new();
            ParameterValue = new();
            Aliases = string.Empty;
            AcceptedValues = new();
            DefaultValue = string.Empty;
            HelpMessage = string.Empty;
        }

        public Parameter(string name, string type)
        {
            Name = name;
            Type = type;
            ParameterSets = new();
            ParameterValue = new();
            Aliases = string.Empty;
            AcceptedValues = new();
            DefaultValue = string.Empty;
            Description = string.Empty;
            HelpMessage = string.Empty;
        }

        /// <summary>
        /// Copy a parameter.
        /// </summary>
        /// <param name="parameter">The parameter to copy.</param>
        public Parameter (Parameter parameter)
        {
            Name = parameter.Name;
            Type = parameter.Type;
            ParameterSets = new List<ParameterSet>(parameter.ParameterSets);
            ParameterValue = new List<string>(parameter.ParameterValue);
            Aliases = parameter.Aliases;
            AcceptedValues = new List<string>(parameter.AcceptedValues);
            DefaultValue = parameter.DefaultValue;
            Description = parameter.Description;
            HelpMessage = parameter.HelpMessage;
            SupportsWildcards = parameter.SupportsWildcards;
            IsDynamic = parameter.IsDynamic;
            DontShow = parameter.DontShow;
            VariableLength = parameter.VariableLength;
        }

        public void AddRequiredParameterSetsRange(bool required, IEnumerable<string> parameterSetNames)
        {
            if (required)
            {
                RequiredTrueParameterSets ??= new List<string>();
                RequiredTrueParameterSets.AddRange(parameterSetNames);
            }
            else
            {
                RequiredFalseParameterSets ??= new List<string>();
                RequiredFalseParameterSets.AddRange(parameterSetNames);
            }
        }

        public void AddRequiredParameterSets(bool required, string parameterSetName)
        {
            string updatedName = parameterSetName;

            if (string.Equals(parameterSetName, "__AllParameterSets", StringComparison.OrdinalIgnoreCase))
            {
                updatedName = Constants.ParameterSetsAllForRequired;
            }

            if (required)
            {
                RequiredTrueParameterSets ??= new List<string>();
                RequiredTrueParameterSets.Add(updatedName);
            }
            else
            {
                RequiredFalseParameterSets ??= new List<string>();
                RequiredFalseParameterSets.Add(updatedName);
            }
        }

        public void AddAcceptedValue(string value)
        {
            AcceptedValues ??= new List<string>();
            AcceptedValues.Add(value);
        }

        public void AddAcceptedValueRange(IEnumerable<string> values)
        {
            AcceptedValues ??= new List<string>();
            AcceptedValues.AddRange(values);
        }

        public Parameter(string name, string description, ParameterMetadataV2 metadata)
        {
            Name = name;
            Type = metadata.Type;
            Description = description;
            VariableLength = metadata.VariableLength;
            DefaultValue = metadata.DefaultValue;
            SupportsWildcards = metadata.SupportsWildcards;
            Aliases = string.Join(",", metadata.Aliases);
            DontShow = metadata.DontShow;
            AcceptedValues = metadata.AcceptedValues;
            HelpMessage = metadata.HelpMessage;
            ParameterValue = metadata.ParameterValue;
            VariableLength = metadata.VariableLength;
            DefaultValue = metadata.DefaultValue;

            ParameterSets = new();
            foreach(var parameterSet in metadata.ParameterSets)
            {
                ParameterSets.Add(
                    new ParameterSet()
                    {
                        Name = parameterSet.Name,
                        Position = parameterSet.Position,
                        IsRequired = parameterSet.IsRequired,
                        ValueFromPipeline = parameterSet.ValueFromPipeline,
                        ValueFromPipelineByPropertyName = parameterSet.ValueFromPipelineByPropertyName,
                        ValueFromRemainingArguments = parameterSet.ValueFromRemainingArguments
                    }
                );
            }
        }

        public static Parameter ConvertV1ParameterToV2(string name, string description, ParameterMetadataV1 V1metadata)
        {
            if (V1metadata.TryConvertMetadataToV2(out var metadata))
            {
                return new Parameter(name, description, metadata);
            }
            else
            {
                throw new InvalidOperationException("Could not convert to v2");
            }
        }

        public ParameterMetadataV2 GetMetadata()
        {
            var metadata = new ParameterMetadataV2();
            metadata.Type = Type;
            metadata.VariableLength = VariableLength;
            metadata.SupportsWildcards = SupportsWildcards;

            if (AcceptedValues is not null && AcceptedValues.Count > 0)
            {
                metadata.AcceptedValues = AcceptedValues;
            }

            if (DefaultValue is not null)
            {
                metadata.DefaultValue = DefaultValue;
            }

            if (! string.IsNullOrEmpty(Aliases))
            {
                var aliases = Aliases?.Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries);
                if (aliases is not null)
                    {
                    foreach(var alias in aliases)
                    {
                        metadata.Aliases.Add(alias.Trim());
                    }
                }
            }

            if (ParameterValue is not null && ParameterValue.Count > 0)
            {
                metadata.ParameterValue = ParameterValue;
            }

            foreach(var paramSet in ParameterSets)
            {
                var pSet = new ParameterSetV2();
                pSet.Name = string.Compare(paramSet.Name, "__AllParameterSets", true) == 0 ? "(All)" : paramSet.Name;
                pSet.Position = paramSet.Position;
                pSet.IsRequired = paramSet.IsRequired;
                pSet.ValueFromPipeline = paramSet.ValueFromPipeline;
                pSet.ValueFromPipelineByPropertyName = paramSet.ValueFromPipelineByPropertyName;
                pSet.ValueFromRemainingArguments = paramSet.ValueFromRemainingArguments;
                metadata.ParameterSets.Add(pSet);
            }
            return metadata;
        }

        /// <summary>
        /// This is a specialized equals check which does not include
        /// the description in the check.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>A boolean representing whether the parameters are the same (without the description).</returns>
        public bool EqualsNoDescription(Parameter other)
        {
            if (other is null)
            {
                return false;
            }

            return (
                string.Compare(Name, other.Name, StringComparison.CurrentCulture) == 0 &&
                string.Compare(Type, other.Type, StringComparison.CurrentCulture) == 0 &&
                string.Compare(Aliases, other.Aliases, StringComparison.CurrentCulture) == 0 &&
                string.Compare(DefaultValue, other.DefaultValue, StringComparison.CurrentCulture) == 0 &&
                string.Compare(HelpMessage, other.HelpMessage, StringComparison.CurrentCulture) == 0 &&
                SupportsWildcards == other.SupportsWildcards &&
                DontShow == other.DontShow &&
                IsDynamic == other.IsDynamic &&
                ParameterSets.SequenceEqual(other.ParameterSets) &&
                (AcceptedValues is null && other.AcceptedValues is null || AcceptedValues.SequenceEqual(other.AcceptedValues))
            );
        }

        public bool Equals(Parameter other)
        {
            if (other is null)
            {
                return false;
            }

            return (
                string.Compare(Name, other.Name, StringComparison.CurrentCulture) == 0 &&
                string.Compare(Type, other.Type, StringComparison.CurrentCulture) == 0 &&
                string.Compare(Description, other.Description, StringComparison.CurrentCulture) == 0 &&
                string.Compare(Aliases, other.Aliases, StringComparison.CurrentCulture) == 0 &&
                string.Compare(DefaultValue, other.DefaultValue, StringComparison.CurrentCulture) == 0 &&
                string.Compare(HelpMessage, other.HelpMessage, StringComparison.CurrentCulture) == 0 &&
                SupportsWildcards == other.SupportsWildcards &&
                IsDynamic == other.IsDynamic &&
                DontShow == other.DontShow &&
                ParameterSets.SequenceEqual(other.ParameterSets) &&
                (AcceptedValues is null && other.AcceptedValues is null || AcceptedValues.SequenceEqual(other.AcceptedValues))
            );
        }

        public override bool Equals(object other)
        {
            if (other is null)
            {
                return false;
            }

            if (other is Parameter parameter2)
            {
                return Equals(parameter2);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (Name, Type, Description, Aliases, DefaultValue, HelpMessage, SupportsWildcards, DontShow, ParameterSets, AcceptedValues).GetHashCode();
        }

        public static bool operator ==(Parameter parameter1, Parameter parameter2)
        {
            if (parameter1 is not null && parameter2 is not null)
            {
                return parameter1.Equals(parameter2);
            }

            return false;
        }

        public static bool operator !=(Parameter parameter1, Parameter parameter2)
        {
            if (parameter1 is not null && parameter2 is not null)
            {
                return ! parameter1.Equals(parameter2);
            }

            return false;
        }
    }

    public class PipelineInputInfo : IEquatable<PipelineInputInfo>
    {
        public bool ByPropertyName { get; set; }
        public bool ByValue { get; set; }

        public PipelineInputInfo(bool byPropertyName, bool byValue)
        {
            ByPropertyName = byPropertyName;
            ByValue = byValue;
        }

        public PipelineInputInfo(bool byBoth)
        {
            ByPropertyName = byBoth;
            ByValue = byBoth;
        }

        override public string ToString()
        {
            return string.Format("ByName ({0}), ByValue ({1})", ByPropertyName, ByValue);
        }

        public override int GetHashCode()
        {
            return ByPropertyName.GetHashCode() ^ ByValue.GetHashCode();
        }

        public bool Equals(PipelineInputInfo other)
        {
            if (other is null)
            {
                return false;
            }

            return (ByPropertyName == other.ByPropertyName && ByValue == other.ByValue);
        }

        public override bool Equals(object other)
        {
            if (other is null)
            {
                return false;
            }

            if (other is PipelineInputInfo info2)
            {
                return Equals(info2);
            }

            return false;
        }

        public static bool operator == (PipelineInputInfo info1, PipelineInputInfo info2)
        {
            if (info1 is not null && info2 is not null)
            {
                return info1.Equals(info2);
            }

            return false;
        }

        public static bool operator !=(PipelineInputInfo info1, PipelineInputInfo info2)
        {
            if (info1 is not null && info2 is not null)
            {
                return ! info1.Equals(info2);
            }

            return false;
        }
    }
}

