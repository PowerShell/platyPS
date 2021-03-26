using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    internal class TransformMaml : TransformBase
    {
        public TransformMaml(PSSession session) : base(session)
        {
        }

        internal override Collection<CommandHelp> Transform(string[] mamlFileNames)
        {
            Collection<CommandHelp> cmdHelp = new();

            foreach (var file in mamlFileNames)
            {
                if (!File.Exists(file))
                {
                    throw new ArgumentNullException($"File {file} does not exist");
                }

                foreach (var command in ReadMaml(file))
                {
                    cmdHelp.Add(command);
                }
            }

            return cmdHelp;
        }

        private Collection<CommandHelp> ReadMaml(string mamlFile)
        {
            Collection<CommandHelp> commandHelps = new();

            using StreamReader stream = new(mamlFile);

            XmlReaderSettings settings = new();

            using XmlReader reader = XmlReader.Create(stream, settings);

            if (reader.MoveToContent() != XmlNodeType.Element)
            {
                throw new InvalidOperationException(reader.NodeType.ToString() + "is invalid XmlNode");
            }

            while (reader.ReadToFollowing(Constants.MamlCommandCommandTag))
            {
                commandHelps.Add(ReadCommand(reader.ReadSubtree()));
            }

            return commandHelps;
        }

        private CommandHelp ReadCommand(XmlReader reader)
        {
            CommandHelp cmdHelp = new();

            if (reader.ReadToFollowing(Constants.MamlCommandNameTag))
            {
                cmdHelp.Title = reader.ReadElementContentAsString();
                cmdHelp.Synopsis = ReadSynopsis(reader);
                cmdHelp.Description = ReadDescription(reader);
                cmdHelp.AddSyntaxItemRange(ReadSyntaxItems(reader));
                cmdHelp.AddParameterRange(ReadParameters(reader));
                cmdHelp.AddInputItem(ReadInput(reader));
                cmdHelp.AddOutputItem(ReadOutput(reader));
                cmdHelp.Notes = ReadNotes(reader);
                cmdHelp.AddExampleItemRange(ReadExamples(reader));
                cmdHelp.AddReleatedLinksRange(ReadRelatedLinks(reader));
            }
            else
            {
                throw new InvalidOperationException("maml file is invalid");
            }

            return cmdHelp;
        }

        private Collection<Links> ReadRelatedLinks(XmlReader reader)
        {
            Collection<Links> relatedLinks = new();

            if (reader.ReadToFollowing(Constants.MamlCommandRelatedLinksTag))
            {
                if (reader.ReadToDescendant(Constants.MamlNavigationLinkTag))
                {
                    do
                    {
                        string linkText = null;
                        string uri = null;

                        if (reader.ReadToFollowing(Constants.MamlLinkTextTag))
                        {
                            linkText = reader.ReadElementContentAsString();
                        }

                        if (reader.ReadToFollowing(Constants.MamlUriTag))
                        {
                            uri = reader.ReadElementContentAsString();
                        }

                        relatedLinks.Add(new Links(uri, linkText));

                        reader.ReadEndElement();

                    } while (reader.ReadToNextSibling(Constants.MamlNavigationLinkTag));
                }
            }

            return relatedLinks;
        }

        private Collection<Example> ReadExamples(XmlReader reader)
        {
            Collection<Example> examples = new();

            if (reader.ReadToFollowing(Constants.MamlCommandExamplesTag))
            {
                int exampleCounter = 1;

                if (reader.ReadToDescendant(Constants.MamlCommandExampleTag))
                {
                    do
                    {
                        examples.Add(ReadExample(reader.ReadSubtree(), exampleCounter));

                        reader.ReadEndElement();

                        exampleCounter++;
                    } while (reader.ReadToNextSibling(Constants.MamlCommandExampleTag));
                }
            }

            return examples;
        }

        private Example ReadExample(XmlReader reader, int exampleCounter)
        {
            Example exp = new();

            if (reader.ReadToFollowing(Constants.MamlTitleTag))
            {
                string title = reader.ReadElementContentAsString();
                exp.Title = title.Trim(' ', '-').Replace($"Example {exampleCounter}: ", "");
            }

            if (reader.ReadToFollowing(Constants.MamlDevCodeTag))
            {
                exp.Code = reader.ReadElementContentAsString();
            }

            if (reader.ReadToFollowing(Constants.MamlDevRemarksTag))
            {
                StringBuilder remarks = new();

                if (reader.ReadToDescendant(Constants.MamlParaTag))
                {
                    do
                    {
                        remarks.AppendLine(reader.ReadElementContentAsString());
                        remarks.AppendLine();
                    } while (reader.ReadToNextSibling(Constants.MamlParaTag));

                    exp.Remarks = remarks.ToString();
                }

                if (reader.ReadState != ReadState.EndOfFile)
                {
                    reader.ReadEndElement();
                }
            }

            return exp;
        }


        private string ReadNotes(XmlReader reader)
        {
            StringBuilder notes = new();

            if (reader.ReadToFollowing(Constants.MamlAlertSetTag))
            {
                if (reader.ReadToDescendant(Constants.MamlAlertTag))
                {
                    if (reader.ReadToDescendant(Constants.MamlParaTag))
                    {
                        do
                        {
                            notes.AppendLine(reader.ReadElementContentAsString());
                            notes.AppendLine();
                        } while (reader.ReadToNextSibling(Constants.MamlParaTag));
                    }
                }
            }

            return notes.ToString();
        }

        private InputOutput ReadInput(XmlReader reader)
        {
            InputOutput inputItem = new();

            if (reader.ReadToFollowing(Constants.MamlCommandInputTypesTag))
            {
                if (reader.ReadToDescendant(Constants.MamlCommandInputTypeTag))
                {
                    do
                    {
                        string typeName = null;
                        string typeDescription = null;

                        if (reader.ReadToFollowing(Constants.MamlNameTag))
                        {
                            typeName = reader.ReadElementContentAsString();
                        }

                        if (reader.ReadToFollowing(Constants.MamlParaTag))
                        {
                            typeDescription = reader.ReadElementContentAsString();
                        }

                        inputItem.AddInputOutputItem(typeName, typeDescription);

                    } while (reader.ReadToNextSibling(Constants.MamlCommandInputTypeTag));

                }
            }

            return inputItem;
        }

        private InputOutput ReadOutput(XmlReader reader)
        {
            InputOutput outputItem = new();

            if (reader.ReadToFollowing(Constants.MamlCommandReturnValuesTag))
            {
                if (reader.ReadToDescendant(Constants.MamlCommandReturnValueTag))
                {
                    do
                    {
                        string typeName = null;
                        string typeDescription = null;

                        if (reader.ReadToFollowing(Constants.MamlNameTag))
                        {
                            typeName = reader.ReadElementContentAsString();
                        }

                        if (reader.ReadToFollowing(Constants.MamlParaTag))
                        {
                            typeDescription = reader.ReadElementContentAsString();
                        }

                        outputItem.AddInputOutputItem(typeName, typeDescription);

                    } while (reader.ReadToNextSibling(Constants.MamlCommandReturnValueTag));
                }
            }

            return outputItem;
        }

        private Collection<Parameter> ReadParameters(XmlReader reader)
        {
            Collection<Parameter> parameters = new();

            if (reader.ReadToFollowing(Constants.MamlCommandParametersTag))
            {
                if (reader.ReadToDescendant(Constants.MamlCommandParameterTag))
                {
                    do
                    {
                        parameters.Add(ReadParameter(reader.ReadSubtree()));
                    } while (reader.ReadToNextSibling(Constants.MamlCommandParameterTag));
                }
            }

            return parameters;
        }

        private Collection<SyntaxItem> ReadSyntaxItems(XmlReader reader)
        {
            Collection<SyntaxItem> items = new();

            int unnamedParameterSetIndex = 1;

            if (reader.ReadToFollowing(Constants.MamlSyntaxTag))
            {
                if(reader.ReadToDescendant(Constants.MamlSyntaxItemTag))
                {
                    do
                    {
                        items.Add(ReadSyntaxItem(reader.ReadSubtree(), unnamedParameterSetIndex));

                        // needed to go to next command:syntaxitem
                        reader.MoveToElement();

                        unnamedParameterSetIndex++;

                    } while (reader.ReadToNextSibling(Constants.MamlSyntaxItemTag));
                }
            }

            return items;
        }

        private SyntaxItem ReadSyntaxItem(XmlReader reader, int unnamedParameterSetIndex)
        {
            if (reader.ReadToDescendant(Constants.MamlNameTag))
            {
                string commandName = reader.ReadElementContentAsString();

                SyntaxItem syntaxItem = new SyntaxItem(
                    commandName,
                    string.Format(Constants.UnnamedParameterSetTemplate, unnamedParameterSetIndex),
                    isDefaultParameterSet: false);

                while (reader.ReadToNextSibling(Constants.MamlCommandParameterTag))
                {
                    syntaxItem.AddParameter(ReadParameter(reader.ReadSubtree()));
                }

                return syntaxItem;
            }

            return null;
        }

        private Parameter ReadParameter(XmlReader reader)
        {
            Parameter parameter = new Parameter();

            reader.Read();

            if (reader.HasAttributes)
            {
                if (reader.MoveToAttribute("required"))
                {
                    bool required;
                    if (bool.TryParse(reader.Value, out required))
                    {
                        parameter.Required = required;
                    }
                }

                if (reader.MoveToAttribute("variableLength"))
                {
                    bool variableLength;
                    if (bool.TryParse(reader.Value, out variableLength))
                    {
                        parameter.VariableLength = variableLength;
                    }
                }

                if (reader.MoveToAttribute("globbing"))
                {
                    bool globbing;
                    if (bool.TryParse(reader.Value, out globbing))
                    {
                        parameter.Globbing = globbing;
                    }
                }

                if (reader.MoveToAttribute("pipelineInput"))
                {
                    // Value is like 'True (ByPropertyName, ByValue)' or 'False'
                    parameter.PipelineInput = reader.Value.StartsWith(Constants.TrueString, StringComparison.OrdinalIgnoreCase) ? true : false;
                }

                if (reader.MoveToAttribute("position"))
                {
                    // Value is like '0' or 'named'
                    parameter.Position = reader.Value;
                }

                if (reader.MoveToAttribute("aliases"))
                {
                    parameter.Aliases = reader.Value;
                }

                reader.MoveToElement();
            }

            if (reader.ReadToDescendant(Constants.MamlNameTag))
            {
                parameter.Name = reader.ReadElementContentAsString();
            }

            parameter.Description = ReadDescription(reader);

            // We read the next element and check the name as it could parameterValue or parameterGroup or dev:type
            while (reader.Read())
            {
                if (string.Equals(reader.Name, Constants.MamlCommandParameterValueTag, StringComparison.OrdinalIgnoreCase))
                {
                    // needed to move the reader ahead. The element has type name but not always. dev:type is more reliable.
                    _ = reader.ReadElementContentAsString();
                }
                else if (string.Equals(reader.Name, Constants.MamlCommandParameterValueGroupTag, StringComparison.OrdinalIgnoreCase))
                {
                    if (reader.ReadToDescendant(Constants.MamlCommandParameterValueTag))
                    {
                        do
                        {
                            parameter.AddAcceptedValue(reader.ReadElementContentAsString());
                        } while (reader.ReadToNextSibling(Constants.MamlCommandParameterValueTag));
                    }
                }
                else if (string.Equals(reader.Name, Constants.MamlDevTypeTag, StringComparison.OrdinalIgnoreCase))
                {
                    if (reader.ReadToDescendant(Constants.MamlNameTag))
                    {
                        parameter.Type = reader.ReadElementContentAsString();
                    }
                }
                else if (string.Equals(reader.Name, Constants.MamlDevDefaultValueTag, StringComparison.OrdinalIgnoreCase))
                {
                    parameter.DefaultValue = reader.ReadElementContentAsString();
                }
            }

            // need to go the end of command:parameter
            if (reader.ReadState != ReadState.EndOfFile)
            {
                reader.ReadEndElement();
            }

            return parameter;
        }

        private string ReadSynopsis(XmlReader reader)
        {
            string synopsis = null;

            if (reader.ReadToNextSibling(Constants.MamlDescriptionTag))
            {
                if (reader.ReadToDescendant(Constants.MamlParaTag))
                {
                    synopsis = reader.ReadElementContentAsString();
                }
            }

            return synopsis;
        }

        private string ReadDescription(XmlReader reader)
        {
            StringBuilder description = new();

            if (reader.ReadToFollowing(Constants.MamlDescriptionTag))
            {
                if (reader.ReadToDescendant(Constants.MamlParaTag))
                {
                    do
                    {
                        description.AppendLine(reader.ReadElementContentAsString());
                        description.AppendLine();
                    } while (reader.ReadToNextSibling(Constants.MamlParaTag));
                }
            }

            reader.ReadEndElement();

            return description.ToString().TrimEnd(Environment.NewLine.ToCharArray());
        }
    }
}
