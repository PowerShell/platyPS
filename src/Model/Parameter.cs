// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.PowerShell.PlatyPS.Model
{
    /// <summary>
    /// Class to represent the properties of a parameter in PowerShell help.
    /// </summary>
    internal class Parameter : IEquatable<Parameter>
    {
        internal string? Description { get; set;}
        internal string Name { get; set;}
        internal List<string>? ParameterValue { get; set;}
        internal string Type { get; set;}

        internal string? DefaultValue { get; set;}

        internal bool Required { get; set; }

        private List<string>? RequiredTrueParameterSets { get; set; }
        private List<string>? RequiredFalseParameterSets { get; set; }

        // @TODO: find out what this is for??
        internal bool VariableLength { get; set;} = true;

        internal bool Globbing { get; set;}

        internal PipelineInputInfo PipelineInput { get; set;}

        internal string Position { get; set;}

        internal string? Aliases { get; set;}

        internal List<string> ParameterSets { get; private set;}

        internal bool DontShow { get; set;}

        internal List<string>? AcceptedValues { get; private set; }

        internal string? HelpMessage { get; set; }

        public Parameter(string name,
                         string type,
                         string position)
        {
            Name = name;
            Type = type;
            Position = position;
            ParameterSets = new();
            PipelineInput = new PipelineInputInfo(false);
        }

        internal void AddRequiredParameterSetsRange(bool required, IEnumerable<string> parameterSetNames)
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

        internal void AddRequiredParameterSets(bool required, string parameterSetName)
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

        internal void AddAcceptedValue(string value)
        {
            AcceptedValues ??= new List<string>();
            AcceptedValues.Add(value);
        }

        internal void AddAcceptedValueRange(IEnumerable<string> values)
        {
            AcceptedValues ??= new List<string>();
            AcceptedValues.AddRange(values);
        }

        internal void AddParameterSet(string parameterSetName)
        {
            if (string.Equals(parameterSetName, "__AllParameterSets", StringComparison.OrdinalIgnoreCase))
            {
                ParameterSets.Add(Constants.ParameterSetsAll);
            }
            else
            {
                ParameterSets.Add(parameterSetName);
            }
        }

        internal void AddParameterSetsRange(IEnumerable<string> values)
        {
            ParameterSets.AddRange(values);
        }

        internal string ToParameterString(string fmt)
        {
            if (Constants.CommonParametersNames.Contains(Name))
            {
                return string.Empty;
            }

            if (AcceptedValues?.Count <= 0)
            {
                return string.Format(Constants.mdParameterYamlBlock,
                    Name,
                    Description?.Trim(Environment.NewLine.ToCharArray()),
                    Type,
                    string.Join(Constants.DelimiterComma, ParameterSets),
                    Aliases,
                    RequiredString(),
                    Position,
                    DefaultValue,
                    PipelineInput.ToString(),
                    Globbing ? Constants.TrueString : Constants.FalseString,
                    DontShow ? Constants.TrueString : Constants.FalseString
                    );
            }
            else
            {
                return string.Format(fmt, // Constants.mdParameterYamlBlockWithAcceptedValues,
                    Name,
                    Description?.Trim(Environment.NewLine.ToCharArray()),
                    Type,
                    ParameterSets?.Count > 0 ? string.Join(Constants.DelimiterComma, ParameterSets) : Constants.ParameterSetsAll,
                    Aliases,
                    AcceptedValues?.Count > 0 ? string.Join(Constants.DelimiterComma, AcceptedValues) : string.Empty,
                    RequiredString(),
                    Position,
                    DefaultValue,
                    PipelineInput.ToString(),
                    Globbing ? Constants.TrueString : Constants.FalseString,
                    DontShow ? Constants.TrueString : Constants.FalseString
                    );
            }
        }

        private string RequiredString()
        {
            if (RequiredTrueParameterSets?.Count == ParameterSets?.Count)
            {
                return Constants.TrueString + "(All)";
            }
            else if (RequiredFalseParameterSets?.Count == ParameterSets?.Count)
            {
                return Constants.FalseString + "(All)";
            }
            else if (RequiredTrueParameterSets?.Count > 0 || RequiredFalseParameterSets?.Count > 0)
            {
                return string.Format(Constants.RequiredParameterSetsTemplate,
                    RequiredTrueParameterSets?.Count > 0 ? string.Join(Constants.DelimiterComma, RequiredTrueParameterSets) : Constants.NoneString,
                    RequiredFalseParameterSets?.Count > 0 ? string.Join(Constants.DelimiterComma, RequiredFalseParameterSets) : Constants.NoneString);
            }
            else
            {
                return Required ? Constants.TrueString : Constants.FalseString;
            }
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
                string.Compare(Position, other.Position, StringComparison.CurrentCulture) == 0 &&
                string.Compare(Description, other.Description, StringComparison.CurrentCulture) == 0 &&
                string.Compare(Aliases, other.Aliases, StringComparison.CurrentCulture) == 0 &&
                string.Compare(DefaultValue, other.DefaultValue, StringComparison.CurrentCulture) == 0 &&
                string.Compare(HelpMessage, other.HelpMessage, StringComparison.CurrentCulture) == 0 &&
                Required == other.Required &&
                VariableLength == other.VariableLength &&
                Globbing == other.Globbing &&
                DontShow == other.DontShow &&
                PipelineInput == other.PipelineInput &&
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
            return (Name, Type, Position, Description, Aliases, DefaultValue, HelpMessage, Required, VariableLength, Globbing, DontShow, PipelineInput, ParameterSets, AcceptedValues).GetHashCode();
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

    internal class PipelineInputInfo : IEquatable<PipelineInputInfo>
    {
        internal bool ByPropertyName { get; set; }
        internal bool ByValue { get; set; }

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
