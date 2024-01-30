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
        internal string Remarks { get; set; }

        public Example(string title, string remarks)
        {
            Title = title;
            Remarks = remarks;
        }

        internal string ToExampleItemString(string fmt, int serialNumber)
        {
            StringBuilder sb = Constants.StringBuilderPool.Get();

            try
            {
                sb.AppendFormat(fmt, serialNumber, Title);
                sb.AppendLine();

                if (!string.IsNullOrEmpty(Remarks) && !string.Equals(Remarks, Environment.NewLine))
                {
                    sb.AppendLine();
                    sb.Append(Remarks);
                }

                return sb.ToString();
            }
            finally
            {
                Constants.StringBuilderPool.Return(sb);
            }
        }
    }
}
