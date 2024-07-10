// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Management.Automation.Language;

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

        private bool EqualityIncludeDescription = false;

        public void IncludeDescriptionForEqualityCheck()
        {
            EqualityIncludeDescription = true;
        }

        public void ExcludeDescriptionForEqualityCheck()
        {
            EqualityIncludeDescription = false;
        }

        public bool GetDescriptionCheckState()
        {
            return EqualityIncludeDescription;
        }

        /// <summary>
        /// Create a new inputoutput object from an existing one.
        /// </summary>
        /// <param name="inputOutput">the inputoutput to copy.</param>
        public InputOutput(InputOutput inputOutput)
        {
            Typename = inputOutput.Typename;
            Description = inputOutput.Description;
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

            var typenameIsSame = other.Typename == Typename;
            var areEqual = EqualityIncludeDescription ? other.Description == Description && typenameIsSame : typenameIsSame;
            return areEqual;
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
