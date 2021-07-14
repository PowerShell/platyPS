// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.PowerShell.PlatyPS.Model
{
    /// <summary>
    /// Class to represent the properties of a link in PowerShell help.
    /// </summary>
    internal class Links
    {
        public Links(string uri, string linkText)
        {
            Uri = uri;
            LinkText = linkText;
        }

        internal string Uri { get; set;}
        internal string LinkText { get; set;}

        internal string ToRelatedLinksString()
        {
            return $"[{LinkText}]({Uri})";
        }
    }
}
