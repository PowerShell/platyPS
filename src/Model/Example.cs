using System;
using System.Text;

namespace Microsoft.PowerShell.PlatyPS.Model
{
    internal class Example
    {
        internal string Title { get; set;}
        internal string Code { get; set;}
        internal string Remarks { get; set;}

        public Example(string title, string code, string remarks)
        {
            Title = title;
            Code = code;
            Remarks = remarks;
        }

        internal string ToExampleItemString(int serialNumber)
        {
            StringBuilder sb = new();

            sb.AppendFormat(Constants.ExampleItemHeaderTemplate, serialNumber, Title);
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(Constants.CodeBlock);
            sb.AppendLine(Code);
            sb.AppendLine(Constants.CodeBlock);

            if (!string.IsNullOrEmpty(Remarks) && !string.Equals(Remarks, Environment.NewLine))
            {
                sb.AppendLine();
                sb.Append(Remarks);
            }

            return sb.ToString();
        }
    }
}