using System;
using System.Collections.Generic;
using System.Linq;
using Markdown.MAML.Model.MAML;
using System.Xml.Linq;

namespace Markdown.MAML.Renderer
{
    /// <summary>
    /// This class contain logic to render maml from the Model.
    /// </summary>
    public class MamlRenderer
    {
        private static XNamespace mshNS = XNamespace.Get("http://msh");
        private static XNamespace mamlNS = XNamespace.Get("http://schemas.microsoft.com/maml/2004/10");
        private static XNamespace commandNS = XNamespace.Get("http://schemas.microsoft.com/maml/dev/command/2004/10");
        private static XNamespace devNS = XNamespace.Get("http://schemas.microsoft.com/maml/dev/2004/10");
        private static XNamespace msHelpNS = XNamespace.Get("http://msdn.microsoft.com/mshelp");

        public string MamlModelToString(IEnumerable<MamlCommand> mamlCommands)
        {
            return MamlModelToString(mamlCommands, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mamlCommands"></param>
        /// <param name="skipPreambula">Don't include header and footer for xml doc</param>
        /// <returns></returns>
        public string MamlModelToString(IEnumerable<MamlCommand> mamlCommands, bool skipPreambula)
        {
            var document = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement(mshNS + "helpItems", new XAttribute(mshNS + "schema","maml"),
                    mamlCommands.Select(mc => CreateCommandElement(mc))));

            return document.ToString();
        }

        private static XElement CreateCommandElement(MamlCommand command)
        {
            var commandParts = command.Name.Split('-');
            var verb = commandParts[0];
            var noun = commandParts[1];

            return new XElement(commandNS + "command",
                    new XAttribute(XNamespace.Xmlns + "maml", mamlNS),
                    new XAttribute(XNamespace.Xmlns + "command", commandNS),
                    new XAttribute(XNamespace.Xmlns + "dev", devNS),
                    new XAttribute(XNamespace.Xmlns + "MSHelp", msHelpNS),

                    new XElement(commandNS + "details",
                        new XElement(commandNS + "name", command.Name),
                        new XElement(commandNS + "verb", verb),
                        new XElement(commandNS + "noun", noun),
                        new XElement(mamlNS + "description", GenerateParagraphs(command.Synopsis))),
                    new XElement(mamlNS + "description", GenerateParagraphs(command.Description)),
                    new XElement(commandNS + "syntax", command.Syntax.Select(syn => CreateSyntaxItem(syn, command))),
                    new XElement(commandNS + "parameters", command.Parameters.Select(param => CreateParameter(param, false))),
                    new XElement(commandNS + "inputTypes", command.Inputs.Select(input => CreateInput(input))),
                    new XElement(commandNS + "returnValues", command.Outputs.Select(output => CreateOutput(output))),
                    new XElement(mamlNS + "alertSet", 
                        new XElement(mamlNS + "alert", command.Notes)),
                    new XElement(commandNS + "examples", command.Examples.Select(example => CreateExample(example))),
                    new XElement(commandNS + "relatedLinks", command.Links.Select(link => CreateLink(link))));
        }

        private static IEnumerable<XElement> GenerateParagraphs(string text)
        {
            if (text != null)
            {
                return text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)
                    .Select(para => new XElement(mamlNS + "para", para));
            }

            return Enumerable.Empty<XElement>();
        }

        private static XElement CreateSyntaxItem(MamlSyntax syntaxItem, MamlCommand command)
        {
            return new XElement(commandNS + "syntaxItem", 
                    new XElement(mamlNS + "name", command.Name),
                    syntaxItem.Parameters.Select(param => CreateParameter(param, true)));
        }

        private static XElement CreateParameter(MamlParameter param, bool syntax)
        {
            string mamlType = ConvertPSTypeToMamlType(param);
            bool isSwitchParameter = mamlType == "SwitchParameter";

            return new XElement(commandNS + "parameter",
                    new XAttribute(commandNS + "required", param.Required),
                    new XAttribute(commandNS + "variableLength", param.VariableLength),
                    new XAttribute(commandNS + "globbing", param.Globbing),
                    new XAttribute(commandNS + "pipelineInput", param.PipelineInput),
                    new XAttribute(commandNS + "position", param.Position.ToLower()),
                    new XAttribute(commandNS + "aliases", param.Aliases.Any() ? string.Join(", ", param.Aliases) : "none"),

                    new XElement(mamlNS + "name", param.Name),
                    new XElement(mamlNS + "Description", GenerateParagraphs(param.Description)),
                    syntax && param.ParameterValueGroup.Any() 
                        ? new XElement(commandNS + "parameterValueGroup", param.ParameterValueGroup.Select(pvg => CreateParameterValueGroup(pvg))) 
                        : null,
                    !syntax || !isSwitchParameter 
                        ? new XElement(commandNS + "parameterValue",
                            new XAttribute(commandNS + "required", param.ValueRequired),
                            new XAttribute(commandNS + "variableLength", param.ValueVariableLength))
                        : null,
                    new XElement(devNS + "type", 
                        new XElement(mamlNS + "name", mamlType),
                        new XElement(mamlNS + "uri")),
                    new XElement(devNS + "defaultValue",
                        isSwitchParameter && (param.DefaultValue ?? string.Empty) == "None"
                            ? "False"
                            : param.DefaultValue ?? "None"));
        }

        private static XElement CreateParameterValueGroup(string pvg)
        {
            return new XElement(commandNS + "parameterValue",
                    new XAttribute(commandNS + "required", "false"), new XAttribute(commandNS + "variableLength", "false"),
                    pvg);
        }

        private static XElement CreateInput(MamlInputOutput input)
        {
            return new XElement(commandNS + "inputType",
                    new XElement(devNS + "type",
                        new XElement(mamlNS + "name", input.TypeName)),
                    new XElement(mamlNS + "description", GenerateParagraphs(input.Description)));
        }

        private static XElement CreateOutput(MamlInputOutput output)
        {
            return new XElement(commandNS + "returnValue",
                    new XElement(devNS + "type",
                        new XElement(mamlNS + "name", output.TypeName)),
                    new XElement(mamlNS + "description", GenerateParagraphs(output.Description)));
        }

        private static XElement CreateExample(MamlExample example)
        {
            return new XElement(commandNS + "example",
                    new XElement(mamlNS + "title", example.Title),
                    new XElement(devNS + "code", example.Code),
                    new XElement(devNS + "remarks", example.Remarks));
        }

        private static XElement CreateLink(MamlLink link)
        {
            if(link.IsSimplifiedTextLink)
            {
                return null;
            }

            string uriValue = string.Empty;
            Uri uri;
            if(Uri.TryCreate(link.LinkUri, UriKind.Absolute, out uri))
            {
                uriValue = link.LinkUri;
            }

            return new XElement(mamlNS + "navigationLink",
                    new XElement(mamlNS + "linkText", link.LinkName),
                    new XElement(mamlNS + "uri", uriValue));
        }

        private static string ConvertPSTypeToMamlType(MamlParameter parameter)
        {
            if (parameter.Type == null)
            {
                return "";
            }

            if (parameter.IsSwitchParameter())
            {
                return "SwitchParameter";
            }

            return parameter.Type;
        }
    }
}
