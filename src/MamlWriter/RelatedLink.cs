using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.PowerShell.PlatyPS.MAML
{
    /// <summary>
    ///     Represents a "command:syntaxItem" element in a Powershell MAML help document.
    ///      <maml:navigationLink>
    ///          <maml:linkText>Set-Content</maml:linkText>
    ///          <maml:uri></maml:uri>
    ///      </maml:navigationLink>
    /// </summary>
    public class NavigationLink : IEquatable<NavigationLink>
    {
        /// <summary>
        ///     The command name for this syntax.
        /// </summary>
        [XmlElement("linkText", Namespace = Constants.XmlNamespace.MAML, Order = 0)]
        public string LinkText { get; set; } = string.Empty;

        /// <summary>
        ///     The command parameters associated with this syntax.
        /// </summary>
        [XmlElement("uri", Namespace = Constants.XmlNamespace.MAML, Order = 1)]
        public string Uri { get; set; } = string.Empty;

        public bool Equals(NavigationLink other)
        {
            if (other is null)
                return false;

            return (string.Compare(LinkText, other.LinkText) == 0 && string.Compare(Uri, other.Uri) == 0);
        }

        public override bool Equals(object other)
        {
            if (other is NavigationLink link2)
            {
                return Equals(link2);
            }

            return false;
        }

        public override int GetHashCode()
        {
            // Valid?
            return LinkText.GetHashCode() ^ Uri.GetHashCode();
        }

        public static bool operator == (NavigationLink link1, NavigationLink link2)
        {
            if (link1 is not null && link2 is not null)
            {
                return link1.Equals(link2);
            }

            return false;
        }

        public static bool operator != (NavigationLink link1, NavigationLink link2)
        {
            if (link1 is not null && link2 is not null)
            {
                return ! link1.Equals(link2);
            }

            return false;
        }

    }



}
