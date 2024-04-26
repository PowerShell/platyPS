// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Microsoft.PowerShell.PlatyPS.Model
{
    /// <summary>
    /// Class to represent the properties of a input or and output type in PowerShell help.
    /// </summary>
    public class InputOutput : IEquatable<InputOutput>
    {
        public string Typename { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public InputOutput(string typename, string description)
        {
            Typename = typename;
            Description = description;
        }

        internal string ToInputOutputString(string fmt)
        {
            StringBuilder sb = Constants.StringBuilderPool.Get();

            try
            {
                sb.AppendLine(string.Format(fmt, Typename));
                sb.AppendLine();
                if (!string.IsNullOrEmpty(Description))
                {
                    sb.AppendLine(Description);
                    sb.AppendLine();
                }

                return sb.ToString().Trim();
            }
            finally
            {
                Constants.StringBuilderPool.Return(sb);
            }
        }

        public bool Equals(InputOutput other)
        {
            if (other is null)
            {
                return false;
            }

            return (other.Typename == Typename && other.Description == Description);
        }

        public override bool Equals(object other)
        {
            if (other is null)
            {
                return false;
            }

            if (other is InputOutput inputOutput2)
            {
                return Equals(inputOutput2);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (Typename, Description).GetHashCode();
        }

        public static bool operator ==(InputOutput inputOutput1, InputOutput inputOutput2)
        {
            if (inputOutput1 is not null && inputOutput2 is not null)
            {
                return inputOutput1.Equals(inputOutput2);
            }

            return false;
        }

        public static bool operator !=(InputOutput inputOutput1, InputOutput inputOutput2)
        {
            if (inputOutput1 is not null && inputOutput2 is not null)
            {
                return ! inputOutput1.Equals(inputOutput2);
            }

            return false;
        }
    }
}
