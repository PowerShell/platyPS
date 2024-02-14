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
    internal class InputOutput : IEquatable<InputOutput>
    {
        // tuple<typename, description>
        internal List<(string, string)> _inputOutputItems;

        public InputOutput()
        {
            _inputOutputItems = new List<(string, string)>();
        }

        internal void AddInputOutputItem(string typeName, string description)
        {
            _inputOutputItems.Add((typeName, description));
        }

        internal string ToInputOutputString(string fmt)
        {
            StringBuilder sb = Constants.StringBuilderPool.Get();

            if (_inputOutputItems.Count == 0)
            {
                return string.Empty;
            }

            try
            {
                foreach (var item in _inputOutputItems)
                {
                    // sb.AppendFormat(Constants.NotesItemHeaderTemplate, item.Item1);
                    sb.AppendFormat(fmt, item.Item1);
                    sb.AppendLine();
                    sb.AppendLine();
                    if (!string.IsNullOrEmpty(item.Item2))
                    {
                        sb.AppendLine(item.Item2);
                        sb.AppendLine();
                    }
                }

                // Remove the last new line
                sb.Remove(sb.Length - 1, 1);

                return sb.ToString();
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

            return _inputOutputItems.SequenceEqual(other._inputOutputItems);
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
            return _inputOutputItems.GetHashCode();
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
