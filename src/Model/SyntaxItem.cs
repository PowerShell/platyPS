// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Microsoft.PowerShell.PlatyPS.Model
{
    /// <summary>
    /// Class to represent the properties of a syntax item in PowerShell help.
    /// </summary>
    internal class SyntaxItem : IEquatable<SyntaxItem>
    {
        internal string CommandName { get; }
        internal string ParameterSetName { get; }

        private List<string> _parameterNames = new();

        internal ReadOnlyCollection<string> ParameterNames {
            get => new ReadOnlyCollection<string>(_parameterNames);
        }

        // Sort parameters by position
        private SortedList<int, Parameter> _positionalParameters;

        // Sort parameters by if they are Required by name
        private SortedList<string, Parameter> _requiredParameters;

        // Sort parameters by name
        private SortedList<string, Parameter> _alphabeticOrderParameters;

        internal bool IsDefaultParameterSet { get; }

        internal SyntaxItem(string commandName, string parameterSetName, bool isDefaultParameterSet)
        {
            ParameterSetName = parameterSetName;
            IsDefaultParameterSet = isDefaultParameterSet;
            CommandName = commandName;

            _positionalParameters = new SortedList<int, Parameter>();
            _requiredParameters = new SortedList<string, Parameter>();
            _alphabeticOrderParameters = new SortedList<string, Parameter>();
        }

        internal void AddParameter(Parameter parameter)
        {
            string name = parameter.Name;

            if (Constants.CommonParametersNames.Contains(name))
            {
                return;
            }

            _parameterNames.Add(name);

            // First see if the parameter is positional

            int position = int.MinValue;

            if (int.TryParse(parameter.Position, out position))
            {
                _positionalParameters.Add(position, parameter);
                return;
            }

            // The position should be 'Named' if not a number
            if (!string.Equals(parameter.Position, "Named", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidCastException($"Invalid value '{parameter.Position}' provided for position for parameter '{name}'");
            }

            if (parameter.Required)
            {
                _requiredParameters.Add(name, parameter);
                return;
            }

            _alphabeticOrderParameters.Add(name, parameter);
        }

        private string GetFormattedSyntaxParameter(string paramName, string paramTypeName, bool isPositional, bool isRequired)
        {
            bool isSwitchParam = string.Equals(paramTypeName, "SwitchParameter", StringComparison.OrdinalIgnoreCase);
            string paramType = isSwitchParam ? string.Empty : paramTypeName;

            bool requiredPositionalSwitch = isRequired && isPositional && isSwitchParam;
            bool requiredPositional = isRequired && isPositional;
            bool requiredSwitch = isRequired && isSwitchParam;
            bool optionalSwitch = !isRequired && isSwitchParam;

            if (requiredPositionalSwitch)
            {
                return string.Format(Constants.RequiredSwitchParamTemplate, paramName, paramType);
            }
            else if (requiredPositional)
            {
                return string.Format(Constants.RequiredPositionalParamTemplate, paramName, paramType);
            }
            else if (requiredSwitch)
            {
                return string.Format(Constants.RequiredSwitchParamTemplate, paramName, paramType);
            }
            else if (isRequired)
            {
                return string.Format(Constants.RequiredParamTemplate, paramName, paramType);
            }
            else if (optionalSwitch)
            {
                return string.Format(Constants.OptionalSwitchParamTemplate, paramName, paramType);
            }
            else if (isPositional)
            {
                return string.Format(Constants.OptionalPositionalParamTemplate, paramName, paramType);
            }
            else
            {
                return string.Format(Constants.OptionalParamTemplate, paramName, paramType);
            }
        }

        internal string ToSyntaxString(string fmt)
        {
            StringBuilder sb = Constants.StringBuilderPool.Get();

            try
            {
                // sb.AppendFormat(IsDefaultParameterSet ? Constants.ParameterSetHeaderDefaultTemplate : Constants.ParameterSetHeaderTemplate, ParameterSetName);
                sb.AppendFormat(fmt, ParameterSetName);
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine(Constants.CodeBlock);

                sb.Append(CommandName);
                sb.Append(Constants.SingleSpace);

                // look for all the positional parameters
                foreach (KeyValuePair<int, Parameter> kv in _positionalParameters)
                {
                    Parameter param = kv.Value;

                    // positional parameters can be required, so chose the template accordingly
                    sb.Append(GetFormattedSyntaxParameter(param.Name, param.Type, isPositional: true, isRequired: param.Required));
                    sb.Append(Constants.SingleSpace);
                }

                // look for all the required parameters
                foreach (KeyValuePair<string, Parameter> kv in _requiredParameters)
                {
                    Parameter param = kv.Value;
                    sb.Append(GetFormattedSyntaxParameter(param.Name, param.Type, isPositional: false, isRequired: true));
                    sb.Append(Constants.SingleSpace);
                }

                // look for all the remaining parameters
                foreach (KeyValuePair<string, Parameter> kv in _alphabeticOrderParameters)
                {
                    Parameter param = kv.Value;
                    sb.Append(GetFormattedSyntaxParameter(param.Name, param.Type, isPositional: false, isRequired: false));
                    sb.Append(Constants.SingleSpace);
                }

                sb.Append(Constants.SyntaxCommonParameters);
                sb.AppendLine();

                // close code block
                sb.AppendLine(Constants.CodeBlock);

                return sb.ToString();
            }
            finally
            {
                Constants.StringBuilderPool.Return(sb);
            }
        }

        public bool Equals(SyntaxItem other)
        {
            if (other is null)
            {
                return false;
            }
            // TODO: compare syntax string
            return (
                string.Compare(CommandName, other.CommandName, StringComparison.CurrentCulture) == 0 &&
                string.Compare(ParameterSetName, other.ParameterSetName, StringComparison.CurrentCulture) == 0 &&
                IsDefaultParameterSet == other.IsDefaultParameterSet
                );
        }

        public override bool Equals(object other)
        {
            if (other is null)
            {
                return false;
            }
            if (other is SyntaxItem syntaxItem2)
            {
                return Equals(syntaxItem2);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (CommandName, ParameterSetName, IsDefaultParameterSet).GetHashCode();
        }

        public static bool operator ==(SyntaxItem syntaxItem1, SyntaxItem syntaxItem2)
        {
            if (syntaxItem1 is not null && syntaxItem2 is not null)
            {
                return syntaxItem1.Equals(syntaxItem2);
            }
            return false;
        }   

        public static bool operator !=(SyntaxItem syntaxItem1, SyntaxItem syntaxItem2)
        {
            if (syntaxItem1 is not null && syntaxItem2 is not null)
            {
                return ! syntaxItem1.Equals(syntaxItem2);
            }
            return false;
        }   

    }
}
