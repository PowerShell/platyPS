using System;
using System.Text;

namespace Microsoft.PowerShell.PlatyPS.Model
{
    internal class Example
    {
        internal string Title { get; set;}
        internal string Code { get; set;}
        internal string Remarks { get; set;}

        internal string ToExampleItemString(int serialNumber)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(Constants.ExampleItemHeaderTemplate, serialNumber, Title);
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(Constants.CodeBlock);
            sb.AppendLine(Code);
            sb.AppendLine(Constants.CodeBlock);
            sb.AppendLine();
            sb.AppendLine(Remarks);

            return sb.ToString();
        }
    }
}