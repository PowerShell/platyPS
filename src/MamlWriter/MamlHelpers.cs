// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;
using Microsoft.PowerShell.PlatyPS;
using Microsoft.PowerShell.PlatyPS.Model;
using System.Text;

namespace Microsoft.PowerShell.PlatyPS.MAML
{
    /// <summary>
    ///     Represents a "command:syntaxItem" element in a Powershell MAML help document.
    /// </summary>
    public class MamlConversionHelper
    {

        /// <summary>
        /// Write the help items to a file.
        /// </summary>
        public static FileInfo WriteToFile(HelpItems helpItems, string path, Encoding encoding)
        {
            var outputFile = new FileInfo(path);
            using(var writer = new StreamWriter(new FileStream(outputFile.FullName, FileMode.OpenOrCreate, FileAccess.ReadWrite), encoding))
            {
                helpItems.WriteTo(writer);
            }

            return new FileInfo(outputFile.FullName);
        }

        /// <summary>
        /// Convert a collection of platyPS CommandHelp objects into a MAML HelpItems object which can be serialized to XML.
        /// </summary>
        public static HelpItems ConvertCommandHelpToMamlHelpItems(List<CommandHelp> commandHelp)
        {
            var helpItems = new HelpItems();
            foreach(var command in commandHelp)
            {
                helpItems.Commands.Add(ConvertCommandHelpToMamlCommand(command));
            }
            return helpItems;
        }

        /// <summary>
        /// Convert the CommandHelp object into the serializable XML
        /// </summary>
        public static Command ConvertCommandHelpToMamlCommand(CommandHelp commandHelp)
        {
            var command = new Command();
            command.Details = ConvertCommandDetails(commandHelp);
            if (commandHelp.Description is not null)
            {
                foreach(string s in commandHelp.Description.Split(new string[] { "\n\n" }, StringSplitOptions.None))
                {
                    command.Description.Add(s.Replace("\n"," ").Trim());
                }
            }

            if (commandHelp.Syntax is not null)
            {
                foreach (var syntax in commandHelp.Syntax)
                {
                    command.Syntax.Add(ConvertSyntax(syntax));
                }
            }

            if (commandHelp.Examples is not null)
            {
                int exampleNumber = 0;
                foreach (var example in commandHelp.Examples)
                {
                    exampleNumber++;
                    command.Examples.Add(ConvertExample(example, exampleNumber));
                }
            }

            if (commandHelp.Parameters is not null)
            {
                foreach (var parameter in commandHelp.Parameters)
                {
                    command.Parameters.Add(ConvertParameter(parameter));
                }
            }

            if (commandHelp.Inputs is not null && commandHelp.Inputs.Count > 0)
            {
                command.InputTypes.AddRange(ConvertInputOutput(commandHelp.Inputs));
            }

            if (commandHelp.Outputs is not null && commandHelp.Outputs.Count > 0)
            {
                command.ReturnValues.AddRange(ConvertInputOutput(commandHelp.Outputs));
            }

            if (commandHelp.RelatedLinks is not null)
            {
                foreach (var link in commandHelp.RelatedLinks)
                {
                    command.RelatedLinks.Add(ConvertLink(link));
                }
            }

            return command;
        }

        private static IEnumerable<CommandValue> ConvertInputOutput(List<Model.InputOutput> inputOutput)
        {
            foreach(var io in inputOutput)
            {
                var newInputOutput = new CommandValue();
                var dataType = new DataType();
                dataType.Name = io.Typename;
                newInputOutput.DataType = dataType;
                newInputOutput.Description.Add(io.Description);
                yield return newInputOutput;
            }
        }

        private static SyntaxItem ConvertSyntax(Model.SyntaxItem syntax)
        {
            var newSyntax = new SyntaxItem();
            var firstSpace = syntax.CommandName.IndexOf(' ');
            if (firstSpace == -1)
            {
                newSyntax.CommandName = syntax.CommandName;
            }
            else
            {
                newSyntax.CommandName = syntax.CommandName.Substring(0, firstSpace);
            }
            foreach(var parameter in syntax.GetParametersInOrder())
            {
                newSyntax.Parameters.Add(ConvertParameter(parameter));
            }

            return newSyntax;
        }

        private static PipelineInputType GetPipelineInputType(Model.Parameter parameter)
        {

            var pipelineInput = new PipelineInputType();
            return pipelineInput;
        }

        private static ParameterValue GetParameterValue(Model.Parameter parameter)
        {
            var parameterValue = new ParameterValue();
            if (parameter is not null)
            {
                parameterValue.DataType = parameter.Type;
                parameterValue.IsVariableLength = parameter.VariableLength;
                // We just mark mandatory if one of the parameter sets is mandatory since MAML doesn't
                // have a way to disambiguate these.
                parameterValue.IsMandatory = parameter.ParameterSets.Any(x => x.IsRequired);
            }
            return parameterValue;
        }

        private static Parameter ConvertParameter(Model.Parameter parameter)
        {
            var newParameter = new MAML.Parameter();
            newParameter.Name = parameter.Name;
            newParameter.IsMandatory = parameter.ParameterSets.Any(x => x.IsRequired);
            newParameter.SupportsGlobbing = parameter.SupportsWildcards;
            var pSet = parameter.ParameterSets.FirstOrDefault();
            newParameter.Position = pSet is null ? Model.Constants.NamedString : pSet.Position;
            newParameter.Value = GetParameterValue(parameter);

            if (parameter.Description is not null)
            {
                foreach(string s in parameter.Description.Split(new string[] { "\n\n" }, StringSplitOptions.None))
                {
                    newParameter.Description.Add(s.Trim());
                }
            }

            return newParameter;
        }

        private static CommandExample ConvertExample(Example example, int exampleNumber)
        {
            var newExample = new CommandExample();
            var title = $"Example {exampleNumber}";
            newExample.Title = string.Format("--------- {0} ---------", title == example.Title ? title : $"{title}: {example.Title}");
            foreach(string s in example.Remarks.Split(new string[] { "\n\n" }, StringSplitOptions.None))
            {
                newExample.Description.Add(s.Trim());
            }
            return newExample;
        }

        private static NavigationLink ConvertLink(Links link)
        {
            var newLink = new NavigationLink();
            newLink.LinkText = link.LinkText;
            newLink.Uri = link.Uri;
            return newLink;
        }

        private static CommandDetails ConvertCommandDetails(CommandHelp commandHelp)
        {
            var details = new CommandDetails();
            details.Name = commandHelp.Title;
            string[] verbNoun = commandHelp.Title.Split('-');
            details.Verb = verbNoun[0];
            details.Noun = verbNoun[1];
            details.Synopsis.Add(commandHelp.Synopsis);
            return details;
        }

    }
}

