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
        internal Type Type { get; set;}

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

        internal void AddRequiredParameterSets(bool required, IEnumerable<string> parameterSets)
        {
            if (required)
            {
                RequiredTrueParameterSets ??= new List<string>();
                RequiredTrueParameterSets.AddRange(parameterSets);
            }
            else
            {
                RequiredFalseParameterSets ??= new List<string>();
                RequiredFalseParameterSets.AddRange(parameterSets);
            }
        }

        internal void AddAcceptedValues(IEnumerable<string> values)
        {
            AcceptedValues ??= new List<string>();
            AcceptedValues.AddRange(values);
        }

        internal void AddParameterSets(IEnumerable<string> values)
        {
            ParameterSets ??= new List<string>();
            ParameterSets.AddRange(values);
        }

        internal string ToParameterString()
        {
            if (AcceptedValues?.Count <= 0)
            {
                return string.Format(Constants.ParameterYmlBlock,
                    Name,
                    Description,
                    Type.Name,
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
                    Description,
                    Type.Name,
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
