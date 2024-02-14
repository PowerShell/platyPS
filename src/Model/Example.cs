// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Text;

namespace Microsoft.PowerShell.PlatyPS.Model
{

    /// <summary>
    /// Class to represent the properties of an example in PowerShell help.
    /// </summary>
    internal class Example : IEquatable<Example>
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
                // sb.AppendFormat(Constants.ExampleItemHeaderTemplate, serialNumber, Title);
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

        public bool Equals(Example other)
        {
            if (other is null)
            {
                return false;
            }

            return (string.Compare(Title, other.Title, StringComparison.CurrentCulture) == 0 && string.Compare(Remarks, other.Remarks, StringComparison.CurrentCulture) == 0);
        }

        public override bool Equals(object other)
        {
            if (other is null)
            {
                return false;
            }

            if (other is Example example2)
            {
                return Equals(example2);
            }

            return false;
        }

        public override int GetHashCode() => (Title, Remarks).GetHashCode();

        public static bool operator == (Example example1, Example example2)
        {
            if (example1 is not null && example2 is not null)
            {
                return example1.Equals(example2);
            }

            return false;
        }

        public static bool operator != (Example example1, Example example2)
        {
            if (example1 is not null && example2 is not null)
            {
                return ! example1.Equals(example2);
            }

            return false;
        }

    }
    
}
