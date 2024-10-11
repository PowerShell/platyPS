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

        /// <summary>
        /// Create a copy of a syntax item.
        /// </summary>
        /// <param name="syntaxItem">The syntax item to copy.</param>
        public SyntaxItem(SyntaxItem syntaxItem)
        {
            CommandName = syntaxItem.CommandName;
            ParameterSetName = syntaxItem.ParameterSetName;
            IsDefaultParameterSet = syntaxItem.IsDefaultParameterSet;
            SyntaxParameters = new List<SyntaxParameter>(syntaxItem.SyntaxParameters);
            HasCmdletBinding = syntaxItem.HasCmdletBinding;
            Parameters = new List<Parameter>(syntaxItem.Parameters);

            _positionalParameters = new SortedList<int, Parameter>(syntaxItem._positionalParameters);
            _requiredParameters = new SortedList<string, Parameter>(syntaxItem._requiredParameters);
            _alphabeticOrderParameters = new SortedList<string, Parameter>(syntaxItem._alphabeticOrderParameters);
            _parameterNames = new List<string>(syntaxItem._parameterNames);
        }

        public void AddParameter(Parameter parameter)
        {
            string name = parameter.Name;

            if (Constants.CommonParametersNames.Contains(name))
            {
                HasCmdletBinding = true;
                return;
            }

            _parameterNames.Add(name);
            _alphabeticOrderParameters.Add(name, parameter);
        }

        /// <summary>
        /// This process takes our syntax parameters and sorts them like get-command does
        /// </summary>
        public void SortParameters()
        {
            List<SyntaxParameter> sortedList = new();
            List<SyntaxParameter> positionList = new();
            List<SyntaxParameter> mandatoryList = new();
            List<SyntaxParameter> namedList = new();
            foreach(var parameter in SyntaxParameters)
            {
                if (string.Compare(parameter.Position, "named") != 0)
                {
                    positionList.Add(parameter);
                }
                else if (parameter.IsMandatory)
                {
                    mandatoryList.Add(parameter);
                }
                else
                {
                    namedList.Add(parameter);
                }
            }

            if (positionList.Count > 0)
            {
                sortedList.AddRange(positionList.OrderBy(p => int.TryParse(p.Position, out var pos) ? pos : int.MaxValue));
            }

            if (mandatoryList.Count > 0)
            {
                sortedList.AddRange(mandatoryList);
            }

            if (namedList.Count > 0)
            {
                sortedList.AddRange(namedList);
            }

            SyntaxParameters = sortedList;
        }

        public void AddParameter(SyntaxParameter parameter)
        {
            string name = parameter.ParameterName;

            if (Constants.CommonParametersNames.Contains(name))
            {
                HasCmdletBinding = true;
                return;
            }

            _parameterNames.Add(name);
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
            SortParameters();
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

        public override string ToString()
        {
            SortParameters();
            StringBuilder sb = Constants.StringBuilderPool.Get(); 
            try
            {
                sb.Append(CommandName);
                foreach(var parameter in SyntaxParameters)
                {
                    var paramAndType = parameter.ToString();
                    sb.Append($" {paramAndType}");
                }

                if (HasCmdletBinding)
                {
                    sb.Append($" {Constants.SyntaxCommonParameters}");
                }

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

            return (
                string.Compare(CommandName, other.CommandName, StringComparison.CurrentCulture) == 0 &&
                string.Compare(ParameterSetName, other.ParameterSetName, StringComparison.CurrentCulture) == 0 &&
                IsDefaultParameterSet == other.IsDefaultParameterSet &&
                SyntaxParameters.Count == other.SyntaxParameters.Count &&
                SyntaxParameters.SequenceEqual<SyntaxParameter>(other.SyntaxParameters)
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
