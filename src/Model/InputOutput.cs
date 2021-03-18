using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.PowerShell.PlatyPS.Model
{
    internal class InputOutput
    {
        // tuple<typename, description>
        private List<Tuple<string, string>> inputOutputItems;

        internal void AddInputOutputItem(string typeName, string description)
        {
            inputOutputItems ??= new List<Tuple<string, string>>();
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