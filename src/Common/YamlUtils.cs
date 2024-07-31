// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Runtime.InteropServices;
using Markdig.Parsers;
using Microsoft.PowerShell.PlatyPS;
using Microsoft.PowerShell.PlatyPS.Model;
using YamlDotNet;
using YamlDotNet.Core.Tokens;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Microsoft.PowerShell.PlatyPS
{
    /// <summary>
    /// This class contains utility functions for working with yaml.
    /// </summary>
    internal class YamlUtils
    {
        private static Deserializer deserializer = (Deserializer)new DeserializerBuilder().Build();
        private static Deserializer camelCaseDeserializer = (Deserializer)new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        private static Serializer serializer = (Serializer)new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

        // we have a specific serializer for metadata as it is not a simple string/string dictionary
        // and we want it to look like: no-loc: [Cmdlet, -Parameter]
        // rather than
        // no-loc:
        //   - Cmdlet
        //   - -Parameter
        private static Serializer metadataSerializer = (Serializer)new SerializerBuilder()
            .WithTypeConverter(new LayoutSequenceStyle())
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();


        /// <summary>
        /// This does not use the yaml deserializer but attempts to parse the bare text into a dictionary.
        /// </summary>
        /// <param name="text">A string which represents a non-conforming yaml block</param>
        /// <param name="result">dictionary</param>
        /// <returns></returns>
        internal static bool TryConvertYamlToDictionary(string text, out Dictionary<string, string>result)
        {
            Dictionary<string, string> valuePairs = new();
            foreach(string line in text.Split(Constants.LineSplitter))
            {
                var trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine))
                {
                    continue;
                }

                var keyValue = trimmedLine.Split(new char[] {':'}, 2);
                if (keyValue is null || keyValue.Length == 0 || string.IsNullOrEmpty(keyValue[0]))
                {
                    continue;
                }
                else if (keyValue.Length == 1)
                {
                    valuePairs.Add(keyValue[0].Trim(), string.Empty);
                }
                else if (keyValue.Length == 2)
                {
                    valuePairs.Add(keyValue[0].Trim(), keyValue[1].Trim());
                }
            }

            result = valuePairs;
            if (valuePairs.Keys.Count > 0)
            {
                return true;
            }

            return false;
        }

        internal static bool TryFixYaml(string badYaml, out string goodYaml)
        {
            if (TryConvertYamlToDictionary(badYaml, out var dict))
            {
                try
                {
                    var result = serializer.Serialize(dict);
                    goodYaml = result;
                    return true;
                }
                catch
                {
                    ;
                }
            }

            goodYaml = string.Empty;
            return false;
        }

        internal static bool TryLastChance(string badYaml, out ParameterMetadataV2 metadata)
        {
            if (TryFixYaml(badYaml, out var goodYaml))
            {
                if (ParameterMetadataV1.TryConvertToV1(goodYaml, out var v1metadata))
                {
                    if (v1metadata.TryConvertMetadataToV2(out var v2metadata))
                    {
                        metadata = v2metadata;
                        return true;
                    }
                }
            }

            metadata = new ParameterMetadataV2();
            return false;
        }

        internal static string SerializeElement(object o)
        {
            return serializer.Serialize(o);
        }

        /// <summary>
        /// Serialize the metadata for exporting.
        /// We convert the ordered dictionary into a sorted dictionary
        /// because we want the keys to be more easily findable.
        /// </summary>
        /// <param name="metadata">An ordered dictionary representing the metadata</param>
        /// <returns>System.String</returns>
        internal static string SerializeMetadata(OrderedDictionary metadata)
        {
            SortedDictionary<string, object> sortedMetadata = new();
            foreach (string key in metadata.Keys)
            {
                sortedMetadata[key] = metadata[key];
            }

            return metadataSerializer.Serialize(sortedMetadata);
        }

        /// <summary>
        /// Try to read a file and convert it to a moduleInfoFile.
        /// The expectation is that path is a fully qualified path to the yaml file.
        /// </summary>
        internal static bool TryReadModuleFile(string path, out ModuleFileInfo? moduleFileInfo, out Exception? deserializationException)
        {
            deserializationException = null;
            moduleFileInfo = null;
            try
            {
                var output = camelCaseDeserializer?.Deserialize<ModuleFileInfo>(File.ReadAllText(path));
                if (output is not null)
                {
                    if (output.Metadata.Contains("Module Name"))
                    {
                        output.Module = output.Metadata["Module Name"].ToString();
                    }

                    if (output.Metadata.Contains("Module Guid"))
                    {
                        if (Guid.TryParse(output.Metadata["Module Guid"].ToString(), out Guid mGuid))
                        {
                            output.ModuleGuid = mGuid;
                        }
                    }

                    moduleFileInfo = output;
                    return true;
                }
            }
            catch (Exception e)
            {
                deserializationException = e;
            }

            return false;
        }

        internal static CommandHelp ConvertDictionaryToCommandHelp(OrderedDictionary? dictionary)
        {
            CommandHelp help = new CommandHelp();
            if (dictionary is null)
            {
                help.Diagnostics.HadErrors = true;
                help.Diagnostics.TryAddDiagnostic(DiagnosticMessageSource.General, "null dictionary", DiagnosticSeverity.Error, "null dictionary", -1);
                return help;
            }

            if (dictionary["metadata"] is IDictionary<object, object> metadata)
            {
                help.Metadata = GetMetadataFromDictionary(metadata);
            }

            if (dictionary["synopsis"] is string synopsis)
            {
                help.Synopsis = synopsis;
            }

            if (dictionary["title"] is string title)
            {
                help.Title = title;
            }

            if (dictionary["description"] is string description)
            {
                help.Description = description;
            }

            if (dictionary["notes"] is string notes)
            {
                help.Notes = notes;
            }

            help.Syntax.AddRange(GetSyntaxFromDictionary(dictionary));
            help.Examples?.AddRange(GetExamplesFromDictionary(dictionary));
            help.Parameters.AddRange(GetParametersFromDictionary(dictionary));
            help.Inputs.AddRange(GetInputsFromDictionary(dictionary));
            help.Outputs.AddRange(GetOutputsFromDictionary(dictionary));
            help.RelatedLinks?.AddRange(GetRelatedLinksFromDictionary(dictionary));

            help.HasCmdletBinding = GetHasCmdletBinding(dictionary);
            help.Syntax.ForEach(s => s.HasCmdletBinding = help.HasCmdletBinding);
            help.HasWorkflowCommonParameters = GetHasWorkflowParameters(dictionary);

            if (help.Metadata is not null)
            {
                help.ModuleGuid = help.Metadata.Contains("ModuleGuid") ? new Guid(help.Metadata["ModuleGuid"].ToString()) : null;
                // help.ExternalHelpFile = help.Metadata.Contains("external help file") ? help.Metadata["external help file"].ToString() : string.Empty;
                // help.OnlineVersionUrl = help.Metadata.Contains("HelpUri") ? help.Metadata["HelpUri"].ToString() : string.Empty;
                // help.SchemaVersion = help.Metadata.Contains("PlatyPS schema version") ? help.Metadata["PlatyPS schema version"].ToString() : string.Empty;
                help.ModuleName = help.Metadata.Contains("Module Name") ? help.Metadata["Module Name"].ToString() : string.Empty;
            }

            return help;
        }

        private static bool GetHasWorkflowParameters(OrderedDictionary dictionary)
        {
            if (dictionary["parameters"] is List<object> parameterList)
            {
                foreach(var parameter in parameterList)
                {
                    if (parameter is IDictionary<object, object> parameterDictionary)
                    {
                        if (string.Compare(parameterDictionary["name"]?.ToString(), "WorkflowParameters", true) == 0)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static bool GetHasCmdletBinding(OrderedDictionary dictionary)
        {
            if (dictionary["parameters"] is List<object> parameterList)
            {
                foreach(var parameter in parameterList)
                {
                    if (parameter is IDictionary<object, object> parameterDictionary)
                    {
                        if (string.Compare(parameterDictionary["name"]?.ToString(), "CommonParameters", true) == 0)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static List<Parameter> GetParametersFromDictionary(OrderedDictionary dictionary)
        {
            List<Parameter> parameters = new();
            if (dictionary["parameters"] is List<object> pList)
            {
                foreach(var parameter in pList)
                {
                    if (parameter is Dictionary<object, object> pDictionary)
                    {
                        var p = GetParameterFromDictionary(pDictionary);
                        if (p is not null)
                        {
                            parameters.Add(p);
                        }
                    }
                }
            }
            return parameters;
        }

        private static Parameter? GetParameterFromDictionary(Dictionary<object, object> pDictionary)
        {
            Parameter? p = null;
            if (pDictionary.TryGetValue("name", out var name))
            {
                // Skip common parameters
                if (string.Compare(name.ToString(), "CommonParameters", true) == 0)
                {
                    return null;
                }

                // Skip workflow parameters
                if (string.Compare(name.ToString(), "WorkflowParameters", true) == 0)
                {
                    return null;
                }

                if (pDictionary.TryGetValue("type", out var type))
                {
                    p = new Parameter(name.ToString(), type.ToString());
                }
            }

            if (p is null)
            {
                return null;
            }

            if (pDictionary.TryGetValue("description", out var desc))
            {
                p.Description = desc.ToString();
            }
            else
            {
                p.Description = string.Empty;
            }

            if (pDictionary.TryGetValue("defaultValue", out var defaultVal))
            {
                p.DefaultValue = defaultVal.ToString();
            }
            else
            {
                p.DefaultValue = string.Empty;
            }

            if (pDictionary.TryGetValue("variableLength", out object varLength))
            {
                if (bool.TryParse(varLength.ToString(), out bool result))
                {
                    p.VariableLength = result;
                }
            } 

            if (pDictionary.TryGetValue("helpMessage", out object helpMsg))
            {
                p.HelpMessage = helpMsg.ToString();
            }

            if (pDictionary.TryGetValue("supportsWildcards", out object supportsWildcards))
            {
                if (bool.TryParse(supportsWildcards?.ToString(), out bool result))
                {
                    p.SupportsWildcards = result;
                }
            }

            if (pDictionary.TryGetValue("globbing", out object canGlob))
            {
                if (bool.TryParse(canGlob?.ToString(), out bool result))
                {
                    p.SupportsWildcards = result;
                }
            }

            if (pDictionary.TryGetValue("aliases", out object aliases))
            {
                if (aliases is string aliasStr)
                {
                    p.Aliases = aliasStr;
                }
                else if (aliases is List<object> aliasList)
                {
                    List<string> aList = new();
                    foreach (var alias in aliasList)
                    {
                        aList.Add(alias.ToString());
                    }
                    p.Aliases = string.Join(", ", aList);
                }
            }

            if (pDictionary.TryGetValue("parameterValue", out var pValue))
            {
                if (pValue is List<object> pValues)
                {
                    foreach (var val in pValues)
                    {
                        p.ParameterValue.Add(val.ToString());
                    }
                }
            }

            if (pDictionary.TryGetValue("acceptedValues", out var aValue))
            {
                if (aValue is List<object> aValues)
                {
                    foreach (var val in aValues)
                    {
                        p.AcceptedValues.Add(val.ToString());
                    }
                }
            }

            if (pDictionary.TryGetValue("dontShow", out object hide))
            {
                if (bool.TryParse(hide?.ToString(), out bool result))
                {
                    p.DontShow = result;
                }
            }

            p.ParameterSets.AddRange(GetParameterSetsFromDictionary(pDictionary));

            return p;
        }

        private static List<ParameterSet> GetParameterSetsFromDictionary(Dictionary<object, object> pDictionary)
        {
            List<ParameterSet> pSetList = new();
            if (pDictionary.TryGetValue("parameterSets", out var pSetDictionary))
            {
                if (pSetDictionary is List<object> pSets)
                {
                    foreach(var pSet in pSets)
                    {
                        if (pSet is Dictionary<object, object> pSetDetail)
                        {
                            ParameterSet set = new();
                            if (pSetDetail.TryGetValue("name", out var name))
                            {
                                set.Name = name.ToString();
                            }
                            else
                            {
                                set.Name = string.Empty;
                            }

                            if (pSetDetail.TryGetValue("position", out var position))
                            {
                                set.Position = position.ToString();
                            }
                            else
                            {
                                set.Position = string.Empty;
                            }

                            if (pSetDetail.TryGetValue("isRequired", out var required))
                            {
                                if (bool.TryParse(required.ToString(), out bool result))
                                {
                                    set.IsRequired = result;
                                }
                            }

                            if (pSetDetail.TryGetValue("valueFromPipeline", out var pipeline))
                            {
                                if (bool.TryParse(pipeline.ToString(), out bool result))
                                {
                                    set.ValueFromPipeline = result;
                                }
                            }

                            if (pSetDetail.TryGetValue("valueFromPipelineByPropertyName", out var propName))
                            {
                                if (bool.TryParse(propName.ToString(), out bool result))
                                {
                                    set.ValueFromPipelineByPropertyName = result;
                                }
                            }

                            if (pSetDetail.TryGetValue("valueFromRemainingArguments", out var remaining))
                            {
                                if (bool.TryParse(remaining.ToString(), out bool result))
                                {
                                    set.ValueFromRemainingArguments = result;
                                }
                            }

                            pSetList.Add(set);
                        }
                    }
                }
            }
            return pSetList;
        }

        private static List<InputOutput> GetInputsFromDictionary(OrderedDictionary dictionary)
        {
            List<InputOutput> inputs = new();
            if (dictionary["inputs"] is List<object> inputList)
            {
                foreach (var input in inputList)
                {
                    if (input is IDictionary<object, object> inputDictionary)
                    {
                        // If we can't get the name, then we can't create the object.
                        // However, the description may be null, so just use an empty string in that case
                        if (inputDictionary.TryGetValue("name", out var name))
                        {
                            if (inputDictionary.TryGetValue("description", out var description))
                            {
                                inputs.Add(new InputOutput(name.ToString(), description is not null ? description.ToString() : string.Empty));
                            }
                            else
                            {
                                inputs.Add(new InputOutput(name.ToString(), string.Empty));
                            }
                        } 
                    }
                }
            }
            return inputs;
        }

        private static List<InputOutput> GetOutputsFromDictionary(OrderedDictionary dictionary)
        {
            List<InputOutput> outputs = new();
            if (dictionary["outputs"] is List<object> outputList)
            {
                foreach (var output in outputList)
                {
                    if (output is IDictionary<object, object> outputDictionary)
                    {
                        if (outputDictionary.TryGetValue("name", out var name))
                        {
                            if (outputDictionary.TryGetValue("description", out var description))
                            {
                                outputs.Add(new InputOutput(name.ToString(), description is not null ? description.ToString() : string.Empty));
                            }
                            else
                            {
                                outputs.Add(new InputOutput(name.ToString(), string.Empty));
                            }
                        }
                    }
                }
            }
            return outputs;
        }

        private static List<Links> GetRelatedLinksFromDictionary(OrderedDictionary dictionary)
        {
            List<Links> links = new();
            if (dictionary["links"] is List<object> linkList)
            {
                foreach(var link in linkList)
                {
                    if (link is IDictionary<object, object> linkDictionary)
                    {
                        links.Add(new Links(linkDictionary["href"].ToString(), linkDictionary["text"].ToString()));
                    }
                }
            }
            return links;
        }

        private static List<Example> GetExamplesFromDictionary(OrderedDictionary dictionary)
        {
            List<Example> examples = new();
            if (dictionary["examples"] is List<object> exampleList)
            {
                foreach(var example in exampleList)
                {
                    if (example is IDictionary<object, object> exampleDictionary)
                    {
                        examples.Add(new Example(exampleDictionary["title"].ToString(), exampleDictionary["description"].ToString()));
                    }
                }
            }
            return examples;
        }

        private static List<SyntaxItem> GetSyntaxFromDictionary(OrderedDictionary dictionary)
        {
            List<SyntaxItem> syntaxes = new();
            if (dictionary["syntaxes"] is List<object> syntaxList)
            foreach (var syntax in syntaxList)
            {
                if (syntax is IDictionary<object, object> syntaxDictionary)
                {
                    SyntaxItem si;
                    var cName = syntaxDictionary["commandName"].ToString();
                    var pSetName = syntaxDictionary["parameterSetName"].ToString();
                    if (bool.TryParse(syntaxDictionary["isDefault"].ToString(), out var isDefault))
                    {
                        si = new SyntaxItem(cName, pSetName, isDefault);
                    }
                    else
                    {
                        si = new SyntaxItem(cName, pSetName, false);
                    }

                    if (syntaxDictionary["parameters"] is List<object> parameterList)
                    {
                        // Now go through the parameters
                        int position = 0;
                        foreach(var syntaxParameter in parameterList)
                        {
                            if (syntaxParameter is string parameterString)
                            {
                                SyntaxParameter sp = GetSyntaxParameterFromParameterString(parameterString, ref position);
                                si.SyntaxParameters.Add(sp);
                            }
                        }
                    }

                    // add the parameter name
                    si.Parameters.ForEach(p => si.AddParameter(p));
                    syntaxes.Add(si);
                }
            }
            return syntaxes;
        }

        private static SyntaxParameter GetSyntaxParameterFromParameterString(string syntaxParameter, ref int position)
        {
            SyntaxParameter sp = new();
            // SwitchParameter
            if (syntaxParameter.IndexOf(' ') == -1)
            {
                sp.IsSwitchParameter = true;
                sp.ParameterType = "SwitchParameter";
                sp.Position = "named";
                if (!(syntaxParameter.StartsWith("[") && syntaxParameter.EndsWith("]")))
                {
                    sp.IsMandatory = true;
                }

                sp.ParameterName = syntaxParameter.Trim(new char[] {'[', ']', '-'});
                return sp;
            }

            var nameAndType = syntaxParameter.Split(' ');
            string pName = nameAndType[0];
            string pType = nameAndType[1];

            // Parameter Name
            if (pName[0] == '-') // -Pname <type>
            {
                sp.IsMandatory = true;
                sp.Position = "named";
            }
            else if (pName[0] == '[' && pName[1] == '[') // [[-Thing] <type>] optional, positional
            {
                sp.IsPositional = true;
                sp.Position = position.ToString();
                position++;
            }
            else // [-Thing <type>] optional, but named
            {
                sp.Position = "named";
            }

            sp.ParameterName = pName.Trim(new char[] {'[', ']', '-'});

            // Parameter Type
            if (pType.EndsWith("]"))
            {
                pType = pType.Remove(pType.Length - 1);
            }

            if (pType.StartsWith("<") && pType.EndsWith(">"))
            {
                pType = pType.Remove(pType.Length-1).Remove(0,1);
            }

            sp.ParameterType = pType;
            return sp;
        }

        private static OrderedDictionary GetMetadataFromDictionary(IDictionary<object, object> metadata)
        {
            OrderedDictionary od = new();
            foreach (var k in metadata.Keys)
            {
                od[k.ToString()] = metadata[k];
            }

            return od;
        }

        internal static bool TryGetMetadataFromText(string text, out OrderedDictionary metadata)
        {
            try
            {
                metadata = deserializer.Deserialize<OrderedDictionary>(text);
                return true;
            }
            catch
            {
                metadata = new();
                return false;
            }
        }
    }
}
