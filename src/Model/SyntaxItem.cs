// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Microsoft.PowerShell.PlatyPS.Model
{
    /// <summary>
    /// Class to represent the properties of a syntax item in PowerShell help.
    /// </summary>
    public class SyntaxItem : IEquatable<SyntaxItem>
    {
        public string CommandName { get; }
        public string ParameterSetName { get; }
        public bool HasCmdletBinding { get; set; }

        public List<SyntaxParameter> SyntaxParameters = new();

        public List<Parameter> Parameters = new();

        private List<string> _parameterNames = new();

        public ReadOnlyCollection<string> ParameterNames {
            get => new ReadOnlyCollection<string>(_parameterNames);
        }

        public ReadOnlyCollection<int> PositionalParameterKeys {
            get => new ReadOnlyCollection<int>(_positionalParameters.Keys);
        }

        // Sort parameters by position
        private SortedList<int, Parameter> _positionalParameters;

        // Sort parameters by if they are Required by name
        private SortedList<string, Parameter> _requiredParameters;

        // Sort parameters by name
        private SortedList<string, Parameter> _alphabeticOrderParameters;

        public bool IsDefaultParameterSet { get; }

        public SyntaxItem(string commandName, string parameterSetName, bool isDefaultParameterSet)
        {
            CommandName = commandName;
            ParameterSetName = parameterSetName;
            IsDefaultParameterSet = isDefaultParameterSet;

            _positionalParameters = new SortedList<int, Parameter>();
            _requiredParameters = new SortedList<string, Parameter>();
            _alphabeticOrderParameters = new SortedList<string, Parameter>();
        }

        public void AddParameter(Parameter parameter)
        {
            string name = parameter.Name;

            if (Constants.CommonParametersNames.Contains(name))
            {
                HasCmdletBinding = true;
                return;
            }

            AddParameterToSyntaxParameter(parameter);
            _parameterNames.Add(name);

            // First see if the parameter is positional

            int position = int.MinValue;

            if (int.TryParse(parameter.Position, out position))
            {
                // This can throw because the position is already in the list.
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

        public void AddParameterToSyntaxParameter(Parameter parameter)
        {
            bool isPositional = false;
            if (int.TryParse(parameter.Position, out int _))
            {
                isPositional = true;
            }

            bool isSwitchParameter = string.Compare(parameter.Type, "SwitchParameter", true) != -1;
            if (! SyntaxParameters.Any(p => string.Compare(p.ParameterName, parameter.Name, true) == 0))
            {
                this.SyntaxParameters.Add(
                    new SyntaxParameter {
                        ParameterName = parameter.Name,
                        ParameterType = parameter.Type,
                        Position = parameter.Position,
                        IsMandatory = parameter.Required,
                        IsPositional = isPositional,
                        IsSwitchParameter = isSwitchParameter
                    }
                );
            }
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

        public IEnumerable<Parameter> GetParametersInOrder()
        {
            foreach (KeyValuePair<int, Parameter> kv in _positionalParameters)
            {
                yield return kv.Value;
            }

            foreach (KeyValuePair<string, Parameter> kv in _requiredParameters)
            {
                yield return kv.Value;
            }

            foreach (KeyValuePair<string, Parameter> kv in _alphabeticOrderParameters)
            {
                yield return kv.Value;
            }
        }

        /// <summary>
        /// This emits the command and parameters as if they were returned by Get-Command -syntax
        /// </summary>
        /// <returns></returns>
        public string ToStringWithWrap()
        {
            StringBuilder sb = Constants.StringBuilderPool.Get();
            StringBuilder currentLine = Constants.StringBuilderPool.Get();
            currentLine.Append(CommandName);

            foreach(var parameter in SyntaxParameters)
            {
                var paramAndType = parameter.ToString();
                if (currentLine.Length + paramAndType.Length + 1 > 100)
                {
                    sb.Append(currentLine.ToString());
                    sb.AppendLine();
                    currentLine.Clear();
                }

                currentLine.Append($" {paramAndType}");
            }

            sb.Append(currentLine.ToString());

            if (HasCmdletBinding)
            {
                if (currentLine.Length + 21 > 100)
                {
                    sb.AppendLine();
                }
                sb.Append(" [<CommonParameters>]");
            }

            try
            {
                return sb.ToString();
            }
            finally
            {
                Constants.StringBuilderPool.Return(sb);
            }

        }

        public string ToSyntaxString(string fmt)
        {
            StringBuilder sb = Constants.StringBuilderPool.Get();

            try
            {
                sb.AppendFormat(fmt, ParameterSetName);
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine(Constants.CodeBlock);
                sb.AppendLine(ToStringWithWrap());
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
