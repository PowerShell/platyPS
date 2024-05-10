// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Xml.Serialization;

namespace Microsoft.PowerShell.PlatyPS.MAML
{
    /// <summary>
    ///     MAML-related constants.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// In MAML if a cmdlet does not have common parameters, this is included as a parameter(?).
        /// </summary>
        public const string NoCommonParameter = "NoCommonParameter";

        /// <summary>
        ///     MAML-related XML namespace constants.
        /// </summary>
        public static class XmlNamespace
        {
            /// <summary>
            ///     The default namespace URI for the root ("helpItems") element.
            /// </summary>
            public const string Root = "http://msh";

            /// <summary>
            ///     The "maml" XML namespace URI.
            /// </summary>
            public const string MAML = "http://schemas.microsoft.com/maml/2004/10";

            /// <summary>
            ///     The "command" XML namespace URI.
            /// </summary>
            public const string Command = "http://schemas.microsoft.com/maml/dev/command/2004/10";

            /// <summary>
            ///     The "dev" XML namespace URI.
            /// </summary>
            public const string Dev = "http://schemas.microsoft.com/maml/dev/2004/10";

            /// <summary>
            ///     Create a <see cref="XmlSerializerNamespaces"/> with standard prefixes for well-known namespaces.
            /// </summary>
            /// <returns>
            ///     The configured <see cref="XmlSerializerNamespaces"/>.
            /// </returns>
            public static XmlSerializerNamespaces GetStandardPrefixes()
            {
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add("maml", MAML);
                namespaces.Add("command", Command);
                namespaces.Add("dev", Dev);

                return namespaces;
            }
        }
    }
}
