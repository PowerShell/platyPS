// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.PowerShell.PlatyPS.MAML
{
    /// <summary>
    ///     Represents a "command" element in a Powershell MAML help document.
    /// </summary>
    // [XmlRoot("command", Namespace = Constants.XmlNamespace.Command)]
    public class Command
    {
         [XmlAttribute("xmlns:maml")]
         public const string MamlNamespace = "http://schemas.microsoft.com/maml/2004/10";

         [XmlAttribute("xmlns:command")]
         public const string CommandNamespace = "http://schemas.microsoft.com/maml/dev/command/2004/10";

         [XmlAttribute("xmlns:dev")]
         public const string DevNamespace = "http://schemas.microsoft.com/maml/dev/2004/10";

         [XmlAttribute("xmlns:MSHelp")]
         public const string MSHelpNamespace = "http://msdn.microsoft.com/mshelp";

        /// <summary>
        ///     The command details.
        /// </summary>
        [XmlElement("details", Namespace = Constants.XmlNamespace.Command, Order = 0)]
        public CommandDetails Details { get; set; } = new CommandDetails();

        /// <summary>
        ///     The command's detailed description (one or more paragraphs; "maml:description/maml:para").
        /// </summary>
        [XmlArray("description", Namespace = Constants.XmlNamespace.MAML, Order = 1)]
        [XmlArrayItem("para", Namespace = Constants.XmlNamespace.MAML)]
        public List<string> Description { get; set; } = new List<string>();

        /// <summary>
        ///     The command's syntax variants.
        /// </summary>
        [XmlArray("syntax", Namespace = Constants.XmlNamespace.Command, Order = 2)]
        [XmlArrayItem("syntaxItem", Namespace = Constants.XmlNamespace.Command)]
        public List<SyntaxItem> Syntax { get; set; } = new List<SyntaxItem>();

        /// <summary>
        ///     The command's parameters.
        /// </summary>
        [XmlArray("parameters", Namespace = Constants.XmlNamespace.Command, Order = 3)]
        [XmlArrayItem("parameter", Namespace = Constants.XmlNamespace.Command)]
        public List<Parameter> Parameters { get; set; } = new List<Parameter>();

        /// <summary>
        ///     The types of inputs accepted by the command.
        /// </summary>
        [XmlArray("inputTypes", Namespace = Constants.XmlNamespace.Command, Order = 4)]
        [XmlArrayItem("inputType", Namespace = Constants.XmlNamespace.Command)]
        public List<CommandValue> InputTypes { get; set; } = new List<CommandValue>();

        /// <summary>
        ///     The types of outputs produced by the command.
        /// </summary>
        [XmlArray("returnValues", Namespace = Constants.XmlNamespace.Command, Order = 5)]
        [XmlArrayItem("returnValue", Namespace = Constants.XmlNamespace.Command)]
        public List<CommandValue> ReturnValues { get; set; } = new List<CommandValue>();

        /// <summary>
        ///     ????
        /// </summary>
        [XmlArray("alertSet", Namespace = Constants.XmlNamespace.MAML, Order = 6)]
        [XmlArrayItem("alert", Namespace = Constants.XmlNamespace.MAML)]
        public List<AlertItem> AlertSet { get; set; } = new List<AlertItem>(); 

        /// <summary>
        ///     Usage examples for the command.
        /// </summary>
        [XmlArray("examples", Namespace = Constants.XmlNamespace.Command, Order = 7)]
        [XmlArrayItem("example", Namespace = Constants.XmlNamespace.Command)]
        public List<CommandExample> Examples { get; set; } = new List<CommandExample>();

        /// <summary>
        ///    Related links for the command.
        /// </summary>
        [XmlArray("relatedLinks", Namespace = Constants.XmlNamespace.Command, Order = 8)]
        [XmlArrayItem("navigationLink", Namespace = Constants.XmlNamespace.MAML)]
        public List<NavigationLink> RelatedLinks { get; set; } = new List<NavigationLink>();

    }
}
