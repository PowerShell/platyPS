// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Text;

namespace Microsoft.PowerShell.PlatyPS.Model
{
    /// <summary>
    /// Class to represent the properties of an example in PowerShell help.
    /// </summary>
    internal class Example
    {
        internal string Title { get; set; }
        internal string Code { get; set; }
        internal string Remarks { get; set; }

        public Example(string title, string code, string remarks)
        {
            Title = title;
            Code = code;
            Remarks = remarks;
        }

        internal string ToExampleItemString(int serialNumber)
        {
            StringBuilder sb = Constants.StringBuilderPool.Get();

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

            var exampleText = sb.ToString();
            Constants.StringBuilderPool.Return(sb);
            return exampleText;
        }
    }
}
