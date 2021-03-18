using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Microsoft.PowerShell.PlatyPS.Model
{
    internal class SyntaxItem
    {
        internal string CommandName { get; }
        internal string ParameterSetName { get; }

        // Sort parameters by position
        internal SortedList<int, Parameter> postitionalParameters;

        // Sort parameters by if they are Required by name
        internal SortedList<string, Parameter> requiredParameters;

        // Sort parameters by name
        internal SortedList<string, Parameter> alphabeticOrderParameters;

        internal bool IsDefaultParameterSet { get; }

        internal SyntaxItem(string commandName, string parameterSetName, bool isDefaultParameterSet)
        {
            ParameterSetName = parameterSetName;
            IsDefaultParameterSet = isDefaultParameterSet;
            CommandName = commandName;

            postitionalParameters = new SortedList<int, Parameter>();
            requiredParameters = new SortedList<string, Parameter>();
            alphabeticOrderParameters = new SortedList<string, Parameter>();
        }

        internal void AddParameter(Parameter parameter)
        {
            string name = parameter.Name;

            if (Constants.CommonParametersNames.Contains(name))
            {
                return;
            }

            // First see if the parameter is positional

            int position = int.MinValue;

            if (int.TryParse(parameter.Position, out position))
            {
                postitionalParameters.Add(position, parameter);
                return;
            }

            // The position should be 'Named' if not a number
            if (!string.Equals(parameter.Position, "Named", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidCastException($"Invalid value '{parameter.Position}' provided for position for parameter '{name}'");
            }

            //Next see if the parameter is required
            if (parameter.Required)
            {
                requiredParameters.Add(name, parameter);
                return;
            }

            //Lastly add the parameter to alphabetic sorted list
            alphabeticOrderParameters.Add(name, parameter);
        }

        private string GetFormattedSyntaxParameter(string paramName, string paramTypeName, bool isPositional, bool isRequired)
        {
            bool isSwitchParam = string.Equals(paramTypeName, "SwitchParameter", StringComparison.OrdinalIgnoreCase);
            string paramType = isSwitchParam ? string.Empty : paramTypeName;

            if (isRequired && isPositional && isSwitchParam)
            {
                return string.Format(Constants.RequiredSwitchParamTemplate, paramName, paramType);
            }
            else if (isRequired && isPositional)
            {
                return string.Format(Constants.RequiredPositionalParamTemplate, paramName, paramType);
            }
            else if (isRequired && isSwitchParam)
            {
                return string.Format(Constants.RequiredSwitchParamTemplate, paramName, paramType);
            }
            else if (isRequired)
            {
                return string.Format(Constants.RequiredParamTemplate, paramName, paramType);
            }
            else if (!isRequired && isSwitchParam)
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

        internal string ToSyntaxString()
        {
            StringBuilder sb = new();

            sb.AppendFormat(IsDefaultParameterSet ? Constants.ParameterSetHeaderDefaultTemplate : Constants.ParameterSetHeaderTemplate, ParameterSetName);
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(Constants.CodeBlock);

            sb.Append(CommandName);
            sb.Append(Constants.SingleSpace);

            // look for all the positional parameters
            foreach (KeyValuePair<int, Parameter> kv in postitionalParameters)
            {
                Parameter param = kv.Value;

                // positional parameters can be required, so chose the template accordingly
                sb.Append(GetFormattedSyntaxParameter(param.Name, param.Type.Name, isPositional: true, isRequired: param.Required));
                sb.Append(Constants.SingleSpace);
            }

            // look for all the required parameters
            foreach(KeyValuePair<string, Parameter> kv in requiredParameters)
            {
                Parameter param = kv.Value;
                sb.Append(GetFormattedSyntaxParameter(param.Name, param.Type.Name, isPositional: false, isRequired: true));
                sb.Append(Constants.SingleSpace);
            }

            // look for all the remaining parameters
            foreach (KeyValuePair<string, Parameter> kv in alphabeticOrderParameters)
            {
                Parameter param = kv.Value;
                sb.Append(GetFormattedSyntaxParameter(param.Name, param.Type.Name, isPositional: false, isRequired: false));
                sb.Append(Constants.SingleSpace);
            }

            sb.Append(Constants.SyntaxCommonParameters);

            // finish syntax
            sb.Append(Environment.NewLine);

            // close code block
            sb.AppendLine(Constants.CodeBlock);

            // remove the last single space
            return sb.ToString();
        }
    }
}