// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Xml;

using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    internal class TransformMaml : TransformBase
    {
        // Dictionary of parameter name -> parameterset which it belongs
        private Dictionary<string, List<string>> _paramSetMap = new();

        public TransformMaml(TransformSettings settings) : base(settings)
        {
        }

        internal override Collection<CommandHelp> Transform(string[] mamlFileNames)
        {
            Collection<CommandHelp> cmdHelp = new();

            foreach (var file in mamlFileNames)
            {
                if (!File.Exists(file))
                {
                    throw new FileNotFoundException(file, file);
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

            var mamlFileInfo = new FileInfo(mamlFile);
            string moduleName = mamlFileInfo.Name.Remove(mamlFileInfo.Name.Length - Constants.MamlFileExtensionSuffix.Length);

            using StreamReader stream = new(mamlFile);

            XmlReaderSettings settings = new();

            using XmlReader reader = XmlReader.Create(stream, settings);

            if (reader.MoveToContent() != XmlNodeType.Element)
            {
                throw new InvalidOperationException(reader.NodeType.ToString() + "is invalid XmlNode");
            }

            while (reader.ReadToFollowing(Constants.MamlCommandCommandTag))
            {
                var cmdHelp = ReadCommand(moduleName, reader.ReadSubtree());

                if (cmdHelp is not null)
                {
                    cmdHelp.ModuleName = moduleName;
                    commandHelps.Add(cmdHelp);
                }
            }

            return commandHelps;
        }

        private CommandHelp? ReadCommand(string moduleName, XmlReader reader)
        {
            CommandHelp cmdHelp;

            if (reader.ReadToFollowing(Constants.MamlCommandNameTag))
            {
                cmdHelp = new(reader.ReadElementContentAsString(), moduleName, Settings.Locale);
                cmdHelp.Synopsis = ReadSynopsis(reader) ?? string.Empty;
                cmdHelp.Description = ReadDescription(reader);
                cmdHelp.AddSyntaxItemRange(ReadSyntaxItems(reader));
                cmdHelp.AddParameterRange(ReadParameters(reader, cmdHelp.Syntax.Count));
                cmdHelp.AddInputItem(ReadInput(reader));
                cmdHelp.AddOutputItem(ReadOutput(reader));
                cmdHelp.Notes = ReadNotes(reader);
                cmdHelp.AddExampleItemRange(ReadExamples(reader));
                cmdHelp.AddRelatedLinksRange(ReadRelatedLinks(reader));
                cmdHelp.ModuleGuid = Settings.ModuleGuid;

                _paramSetMap.Clear();
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
                        string? linkText = null;
                        string? uri = null;

                        if (reader.ReadToFollowing(Constants.MamlLinkTextTag))
                        {
                            linkText = reader.ReadElementContentAsString();
                        }

                        if (reader.ReadToFollowing(Constants.MamlUriTag))
                        {
                            uri = reader.ReadElementContentAsString();
                        }

                        if (linkText != null && uri != null)
                        {
                            relatedLinks.Add(new Links(uri, linkText));
                        }

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
            string? title = null;
            string? code = null;

            if (reader.ReadToFollowing(Constants.MamlTitleTag))
            {
                title = reader.ReadElementContentAsString().Trim(' ', '-').Replace($"Example {exampleCounter}: ", string.Empty);
            }

            if (reader.ReadToFollowing(Constants.MamlDevCodeTag))
            {
                code = reader.ReadElementContentAsString();
            }

            StringBuilder remarks = Constants.StringBuilderPool.Get();

            try
            {
                if (reader.ReadToFollowing(Constants.MamlDevRemarksTag))
                {
                    if (reader.ReadToDescendant(Constants.MamlParaTag))
                    {
                        do
                        {
                            remarks.AppendLine(reader.ReadElementContentAsString());
                            remarks.AppendLine();
                        } while (reader.ReadToNextSibling(Constants.MamlParaTag));
                    }

                    if (reader.ReadState != ReadState.EndOfFile)
                    {
                        reader.ReadEndElement();
                    }
                }

                if (title == null || code == null)
                {
                    throw new InvalidDataException("Invalid example data");
                }

                Example exp = new(
                    title,
                    remarks.ToString()
                    );

                return exp;
            }
            finally
            {
                Constants.StringBuilderPool.Return(remarks);
            }
        }


        private string ReadNotes(XmlReader reader)
        {
            StringBuilder notes = Constants.StringBuilderPool.Get();

            try
            {
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
            finally
            {
                Constants.StringBuilderPool.Return(notes);
            }
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
                        string? typeName = null;
                        string? typeDescription = null;

                        if (reader.ReadToFollowing(Constants.MamlNameTag))
                        {
                            typeName = reader.ReadElementContentAsString();
                        }

                        if (reader.ReadToFollowing(Constants.MamlParaTag))
                        {
                            typeDescription = reader.ReadElementContentAsString();
                        }

                        if (typeName is not null && typeDescription is not null)
                        {
                            inputItem.AddInputOutputItem(typeName, typeDescription);
                        }

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
                        string? typeName = null;
                        string? typeDescription = null;

                        if (reader.ReadToFollowing(Constants.MamlNameTag))
                        {
                            typeName = reader.ReadElementContentAsString();
                        }

                        if (reader.ReadToFollowing(Constants.MamlParaTag))
                        {
                            typeDescription = reader.ReadElementContentAsString();
                        }

                        if (typeName is not null && typeDescription is not null)
                        {
                            outputItem.AddInputOutputItem(typeName, typeDescription);
                        }

                    } while (reader.ReadToNextSibling(Constants.MamlCommandReturnValueTag));
                }
            }

            return outputItem;
        }

        private Collection<Parameter> ReadParameters(XmlReader reader, int parameterSetCount)
        {
            Collection<Parameter> parameters = new();

            if (reader.ReadToFollowing(Constants.MamlCommandParametersTag))
            {
                if (reader.ReadToDescendant(Constants.MamlCommandParameterTag))
                {
                    do
                    {
                        parameters.Add(ReadParameter(reader.ReadSubtree(), parameterSetCount));
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
                        var unnamedParameterSetName = string.Format(Constants.UnnamedParameterSetTemplate, unnamedParameterSetIndex);

                        var syn = ReadSyntaxItem(reader.ReadSubtree(), unnamedParameterSetName);

                        if (syn is not null)
                        {
                            items.Add(syn);
                        }

                        // needed to go to next command:syntaxitem
                        reader.MoveToElement();

                        unnamedParameterSetIndex++;

                    } while (reader.ReadToNextSibling(Constants.MamlSyntaxItemTag));
                }
            }

            return items;
        }

        private SyntaxItem? ReadSyntaxItem(XmlReader reader, string unnamedParameterSetName)
        {
            if (reader.ReadToDescendant(Constants.MamlNameTag))
            {
                string commandName = reader.ReadElementContentAsString();

                SyntaxItem syntaxItem = new SyntaxItem(
                    commandName,
                    unnamedParameterSetName,
                    isDefaultParameterSet: false);

                while (reader.ReadToNextSibling(Constants.MamlCommandParameterTag))
                {
                    var parameter = ReadParameter(reader.ReadSubtree(), parameterSetCount: -1);
                    try
                    {
                        // This may possibly throw because the position is duplicated.
                        // This is because we don't have a way to disambiguate between a positional parameter
                        // in one parameter set and a non-positional parameter in another parameter set.
                        // In this case, we will try to add the parameter with a negative position to show we
                        // could not assign it appropriately.
                        syntaxItem.AddParameter(parameter);
                    }
                    catch
                    {
                        int minKey = syntaxItem.PositionalParameterKeys.Min(x => x);
                        if (minKey >= 0)
                        {
                            minKey = -1;
                        }
                        else
                        {
                            minKey--;
                        }

                        parameter.ParameterSets.ForEach(x => x.Position = minKey.ToString());
                        try
                        {
                            syntaxItem.AddParameter(parameter);
                        }
                        catch (Exception exception)
                        {
                            throw new InvalidOperationException($"Error adding parameter '{parameter.Name}' to syntax item {commandName}", exception);
                        }
                    }
                }

                foreach(var paramName in syntaxItem.ParameterNames)
                {
                    if (_paramSetMap.ContainsKey(paramName))
                    {
                        _paramSetMap[paramName].Add(unnamedParameterSetName);
                    }
                    else
                    {
                        _paramSetMap.Add(paramName, new List<string>() { unnamedParameterSetName });
                    }
                }

                return syntaxItem;
            }

            return null;
        }

        private Parameter ReadParameter(XmlReader reader, int parameterSetCount)
        {
            string name = string.Empty;
            string type = string.Empty;
            string position = Constants.NamedString;
            string? defaultValue = null;
            List<string> acceptedValues = new();
            bool required = false;
            bool variableLength = false;
            bool globbing = false;
            bool pipelineInput = false;
            string? aliases = null;

            reader.Read();

            if (reader.HasAttributes)
            {
                if (reader.MoveToAttribute("required"))
                {
                    bool.TryParse(reader.Value, out required);
                }

                if (reader.MoveToAttribute("variableLength"))
                {
                    bool.TryParse(reader.Value, out variableLength);
                }

                if (reader.MoveToAttribute("globbing"))
                {
                    bool.TryParse(reader.Value, out globbing);
                }

                if (reader.MoveToAttribute("pipelineInput"))
                {
                    // Value is like 'True (ByPropertyName, ByValue)' or 'False'
                    pipelineInput = reader.Value.StartsWith(Constants.TrueString, StringComparison.OrdinalIgnoreCase) ? true : false;
                }

                if (reader.MoveToAttribute("position"))
                {
                    // Value is like '0' or 'named'
                    position = reader.Value;
                }

                if (reader.MoveToAttribute("aliases"))
                {
                    aliases = reader.Value;
                }

                reader.MoveToElement();
            }

            if (reader.ReadToDescendant(Constants.MamlNameTag))
            {
                name = reader.ReadElementContentAsString();
            }

            string description = ReadDescription(reader);

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
                            acceptedValues.Add(reader.ReadElementContentAsString());
                        } while (reader.ReadToNextSibling(Constants.MamlCommandParameterValueTag));
                    }
                }
                else if (string.Equals(reader.Name, Constants.MamlDevTypeTag, StringComparison.OrdinalIgnoreCase))
                {
                    if (reader.ReadToDescendant(Constants.MamlNameTag))
                    {
                        type = reader.ReadElementContentAsString();
                    }
                }
                else if (string.Equals(reader.Name, Constants.MamlDevDefaultValueTag, StringComparison.OrdinalIgnoreCase))
                {
                    defaultValue = reader.ReadElementContentAsString();
                }
            }

            // Parameter parameter = new Parameter(name, type, position);
            Parameter parameter = new Parameter(name, type);
            parameter.DefaultValue = defaultValue;
            parameter.AddAcceptedValueRange(acceptedValues);
            parameter.Description = description;
            // we will set the required attributed to all parameter sets since MAML doesn't have a way to disambiguate.
            parameter.ParameterSets.ForEach(x => x.IsRequired = required);
            parameter.VariableLength = variableLength;
            parameter.Globbing = globbing;
            parameter.Aliases = aliases;

            // need to go the end of command:parameter
            if (reader.ReadState != ReadState.EndOfFile)
            {
                reader.ReadEndElement();
            }

            return parameter;
        }

        private string? ReadSynopsis(XmlReader reader)
        {
            string? synopsis = null;

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
            StringBuilder description = Constants.StringBuilderPool.Get();

            try
            {
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

                var descText = description.ToString().TrimEnd(Environment.NewLine.ToCharArray());
                return descText;
            }
            finally
            {
                Constants.StringBuilderPool.Return(description);
            }
        }
    }
}
