using System;

namespace Microsoft.PowerShell.PlatyPS.Model
{
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
            return $"[{LinkText}] ({Uri})";
        }
    }
}