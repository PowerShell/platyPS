using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.PowerShell.PlatyPS.Model
{
    internal class Parameter
    {
        internal string Description { get; set;}
        internal string Name { get; set;}
        internal List<string> ParameterValue { get; set;}
        internal string Type { get; set;}

        internal string DefaultValue { get; set;}

        internal bool Required { get; set; }

        private List<string> RequiredTrueParameterSets { get; set; }
        private List<string> RequiredFalseParameterSets { get; set; }

        // @TODO: find out what this is for??
        internal bool VariableLength { get; set;} = true;

        internal bool Globbing { get; set;}

        internal bool PipelineInput { get; set;}

        internal string Position { get; set;}

        internal string Aliases { get; set;}

        internal List<string> ParameterSets { get; private set;}

        internal bool DontShow { get; set;}

        internal List<string> AcceptedValues { get; private set; }

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
            ParameterSets ??= new List<string>();

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
            ParameterSets ??= new List<string>();
            ParameterSets.AddRange(values);
        }

        internal string ToParameterString()
        {
            if (Constants.CommonParametersNames.Contains(Name))
            {
                return null;
            }

            if (AcceptedValues?.Count <= 0)
            {
                return string.Format(Constants.ParameterYmlBlock,
                    Name,
                    Description?.Trim(Environment.NewLine.ToCharArray()),
                    Type,
                    string.Join(Constants.DelimiterComma, ParameterSets),
                    Aliases,
                    RequiredString(),
                    Position,
                    DefaultValue,
                    PipelineInput ? Constants.TrueString : Constants.FalseString,
                    Globbing ? Constants.TrueString : Constants.FalseString,
                    DontShow ? Constants.TrueString : Constants.FalseString
                    );
            }
            else
            {
                return string.Format(Constants.ParameterYmlBlockWithAcceptedValues,
                    Name,
                    Description?.Trim(Environment.NewLine.ToCharArray()),
                    Type,
                    ParameterSets?.Count > 0 ? string.Join(Constants.DelimiterComma, ParameterSets) : Constants.ParameterSetsAll,
                    Aliases,
                    AcceptedValues?.Count > 0 ? string.Join(Constants.DelimiterComma, AcceptedValues) : string.Empty,
                    RequiredString(),
                    Position,
                    DefaultValue,
                    PipelineInput ? Constants.TrueString : Constants.FalseString,
                    Globbing ? Constants.TrueString : Constants.FalseString,
                    DontShow ? Constants.TrueString : Constants.FalseString
                    );
            }
        }

        private string RequiredString()
        {
            return string.Format(Constants.RequiredParameterSetsTemplate,
                RequiredTrueParameterSets?.Count > 0 ? string.Join(Constants.DelimiterComma, RequiredTrueParameterSets) : Constants.NoneString,
                RequiredFalseParameterSets?.Count > 0 ? string.Join(Constants.DelimiterComma, RequiredFalseParameterSets) : Constants.NoneString);
        }
    }
}
