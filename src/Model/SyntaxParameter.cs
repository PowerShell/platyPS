// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Microsoft.PowerShell.PlatyPS.Model
{
    /// <summary>
    /// Class to represent the properties of a parameter used for a syntax item in PowerShell help.
    /// </summary>
    public class SyntaxParameter : IEquatable<SyntaxParameter>
    {
        public string ParameterName { get; set; } = string.Empty;
        public string ParameterType { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty; // this can look like an int or "named", or empty
        public bool IsMandatory { get; set; }
        public bool IsPositional { get; set; }
        public bool IsSwitchParameter { get; set; }

        public SyntaxParameter()
        {

        }

        public SyntaxParameter(string name)
        {
            ParameterName = name;
        }

        public SyntaxParameter(string name, string type, string position, bool isMandatory, bool isPositional, bool isSwitchParameter)
        {
            ParameterName = name;
            ParameterType = type;
            Position = position;
            IsMandatory = isMandatory;
            IsPositional = isPositional;
            IsSwitchParameter = isSwitchParameter;
        }
    
        /// <summary>
        /// We build the string that is identical to the output of Get-Command -Syntax
        /// This could look like:
        /// [[-Parameter] <string>] optional and positional
        /// [-Parameter] optional, type SwitchParameter
        /// -Parameter <string> mandatory, non-positional
        /// [-Parameter] <string> positional, mandatory
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            StringBuilder sb = Constants.StringBuilderPool.Get();
            if (! IsMandatory)
            {
                sb.Append("[");
            }

            // Positional parameters are surrounded by square brackets
            if (IsPositional)
            {
                sb.Append("[");
            }

            sb.Append($"-{ParameterName}");
            if (IsPositional || IsSwitchParameter)
            {
                sb.Append("]");
            }

            if (! IsSwitchParameter)
            {
                sb.Append($" <{ParameterType}>");
                if (! IsMandatory)
                {
                    sb.Append("]");
                }
            }
            // It's not possible to have an optional, positional switch parameter

            try
            {
                return sb.ToString();
            }
            finally
            {
                Constants.StringBuilderPool.Return(sb);
            }
        }

        public bool Equals(SyntaxParameter other)
        {
            if (other is null)
            {
                return false;
            }
            return other.IsMandatory == IsMandatory &&
                    other.IsPositional == IsPositional &&
                    other.IsSwitchParameter == IsSwitchParameter &&
                    string.Compare(other.ParameterName, ParameterName, true) == 0 &&
                    string.Compare(other.ParameterType, ParameterType, true) == 0 &&
                    string.Compare(other.Position, Position, true) == 0;
        }

        public override bool Equals(object other)
        {
            if (other is null)
            {
                return false;
            }
            if (other is SyntaxParameter syntaxParameter2)
            {
                return Equals(syntaxParameter2);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (ParameterName, ParameterType, Position).GetHashCode();
        }

        public static bool operator ==(SyntaxParameter syntaxParameter1, SyntaxParameter syntaxParameter2)
        {
            if (syntaxParameter1 is not null && syntaxParameter2 is not null)
            {
                return syntaxParameter1.Equals(syntaxParameter2);
            }
            return false;
        }

        public static bool operator !=(SyntaxParameter syntaxParameter1, SyntaxParameter syntaxParameter2)
        {
            if (syntaxParameter1 is not null && syntaxParameter2 is not null)
            {
                return ! syntaxParameter1.Equals(syntaxParameter2);
            }
            return false;
        }
    }
}