using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace Microsoft.PowerShell.PlatyPS.Model
{
    /// <summary>
    /// Class to represent the properties of a input or and output type in PowerShell help.
    /// </summary>
    internal class InputOutput
    {
        // tuple<typename, description>
        private List<Tuple<string, string>> inputOutputItems;

        public InputOutput()
        {
            inputOutputItems = new List<Tuple<string, string>>();
        }

        internal void AddInputOutputItem(string typeName, string description)
        {
            inputOutputItems.Add(new Tuple<string, string>(typeName, description));
        }

        internal string ToInputOutputString()
        {
            StringBuilder sb = new();

            foreach(var item in inputOutputItems)
            {
                sb.AppendFormat(Constants.NotesItemHeaderTemplate, item.Item1);
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine(item.Item2);
                sb.AppendLine();
            }

            // Remove the last new line
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }
    }
}