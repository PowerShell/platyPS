// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.PowerShell.PlatyPS.Model
{
    /// <summary>
    /// Class to represent the properties of a input or and output type in PowerShell help.
    /// </summary>
    internal class InputOutput
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
    }
}
