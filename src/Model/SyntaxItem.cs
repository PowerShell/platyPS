using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Microsoft.PowerShell.PlatyPS.Model
{
    internal class SyntaxItem
    {
        internal string ParameterSetName { get; }

        // Sort parameters by position
        internal SortedList<int, Parameter> postitionalParameters;

        // Sort parameters by if they are Required by name
        internal SortedList<string, Parameter> requiredParameters;

        // Sort parameters by name
        internal SortedList<string, Parameter> alphabeticOrderParameters;

        internal bool IsDefaultParameterSet { get; }

        internal SyntaxItem(string parameterSetName, bool isDefaultParameterSet)
        {
            ParameterSetName = parameterSetName;
            IsDefaultParameterSet = isDefaultParameterSet;

            postitionalParameters = new SortedList<int, Parameter>();
            requiredParameters = new SortedList<string, Parameter>();
            alphabeticOrderParameters = new SortedList<string, Parameter>();
        }

        internal void AddParameter(Parameter parameter)
        {
            string name = parameter.Name;

            // First see if the parameter is positional
            if (!string.IsNullOrEmpty(parameter.Position))
            {
                int position = -1;

                if (int.TryParse(parameter.Position, out position))
                {
                    postitionalParameters.Add(position, parameter);
                }
                else
                {
                    if (!string.Equals(parameter.Position, "Named", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidCastException($"Invalid value '{parameter.Position}' provided for position for parameter '{name}'");
                    }
                }
            }
            //Next see if the parameter is required
            else if (parameter.Required)
            {
                requiredParameters.Add(name, parameter);
            }
            //Lastly add the parameter to alphabetic sorted list
            else
            {
                alphabeticOrderParameters.Add(name, parameter);
            }
        }

        internal string ToSyntaxString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(IsDefaultParameterSet ? Constants.ParameterSetHeaderDefaultTemplate : Constants.ParameterSetHeaderTemplate, ParameterSetName);
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(Constants.CodeBlock);

            // look for all the positional parameters
            foreach (KeyValuePair<int, Parameter> kv in postitionalParameters)
            {
                Parameter param = kv.Value;

                // positional parameters can be required, so chose the template accordingly
                sb.AppendFormat(param.Required? Constants.RequiredParamTemplate : Constants.OptionalParamTemplate, param.Name, param.Type.Name);
                sb.Append(Constants.SingleSpace);
            }

            // look for all the required parameters
            foreach(KeyValuePair<string, Parameter> kv in requiredParameters)
            {
                Parameter param = kv.Value;
                sb.AppendFormat(Constants.RequiredParamTemplate, param.Name, param.Type.Name);
                sb.Append(Constants.SingleSpace);
            }

            // look for all the remaining parameters
            foreach (KeyValuePair<string, Parameter> kv in alphabeticOrderParameters)
            {
                Parameter param = kv.Value;
                sb.AppendFormat(Constants.OptionalParamTemplate, param.Name, param.Type.Name);
                sb.Append(Constants.SingleSpace);
            }

            // trim the last single space
            sb.Remove(sb.Length - 1, 1);

            // finish syntax
            sb.Append(Environment.NewLine);

            // close code block
            sb.AppendLine(Constants.CodeBlock);

            // remove the last single space
            return sb.ToString();
        }
    }
}