// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;

using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    internal abstract class TransformBase
    {
        protected readonly TransformSettings Settings;

        public TransformBase(TransformSettings settings) => Settings = settings;

        internal abstract Collection<CommandHelp> Transform(string[] source);

        protected CommandHelp ConvertCmdletInfo(CommandInfo? commandInfo)
        {
            if (commandInfo is null)
            {
                throw new ArgumentNullException();
            }

            string cmdName = commandInfo is ExternalScriptInfo ? commandInfo.Source : commandInfo.Name;

            Collection<PSObject> help = PowerShellAPI.GetHelpForCmdlet(cmdName, Settings.Session);

            bool addDefaultStrings = false;
            dynamic? helpItem = null;

            if (help.Count == 1)
            {
                helpItem = help[0];

                // If the description and examples are empty the help is auto-generated.
                // So assume that no existing help content is available.
                if (string.IsNullOrEmpty(GetStringFromDescriptionArray(helpItem.description)) &&
                    string.IsNullOrEmpty(helpItem.examples))
                {
                    addDefaultStrings = true;
                }
            }
            else
            {
                addDefaultStrings = true;
            }


            CommandHelp cmdHelp = new(commandInfo.Name, commandInfo.ModuleName, Settings.Locale);
            cmdHelp.Metadata = MetadataUtils.GetCommandHelpBaseMetadataFromCommandInfo(commandInfo);
            cmdHelp.OnlineVersionUrl = Settings.OnlineVersionUrl;
            cmdHelp.Synopsis = GetSynopsis(helpItem, addDefaultStrings);
            cmdHelp.AddSyntaxItemRange(GetSyntaxItem(commandInfo, helpItem));
            cmdHelp.Description = GetDescription(helpItem, addDefaultStrings);
            cmdHelp.AddExampleItemRange(GetExamples(helpItem, addDefaultStrings));
            cmdHelp.AddParameterRange(GetParameters(commandInfo, helpItem, addDefaultStrings));

            cmdHelp.HasCmdletBinding = (commandInfo is FunctionInfo funcInfo && funcInfo.CmdletBinding) ||
                commandInfo is CmdletInfo ||
                (commandInfo is ExternalScriptInfo extInfo && extInfo.ScriptBlock.Attributes.Contains(new CmdletBindingAttribute()));

            if (!string.IsNullOrEmpty(cmdHelp.ModuleName))
            {
                var moduleInfos = PowerShellAPI.GetModuleInfo(cmdHelp.ModuleName, Settings.Session);

                if (moduleInfos?.Count > 0)
                {
                    cmdHelp.ModuleGuid = moduleInfos[0].Guid;
                }
            }

            // Sometime the help content does not have any input type
            if (helpItem?.inputTypes?.inputType is not null)
            {
                var ioItem = GetInputOutputItem(
                        helpItem.inputTypes.inputType,
                        addDefaultStrings ? Constants.NoneString : string.Empty,
                        addDefaultStrings ? Constants.FillInDescription : string.Empty);
                if (ioItem is not null)
                {
                    cmdHelp.AddInputItem(ioItem);
                }
            }

            // Sometime the help content does not have any output type
            if (helpItem?.returnValues?.returnValue is not null)
            {
                var ioItem = GetInputOutputItem(
                        helpItem.returnValues.returnValue,
                        addDefaultStrings ? Constants.SystemObjectTypename : string.Empty,
                        addDefaultStrings ? Constants.FillInDescription : string.Empty);
                if (ioItem is not null)
                {
                    cmdHelp.AddOutputItem(ioItem);
                }
            }

            cmdHelp.Notes = GetNotes(helpItem, addDefaultStrings);
            cmdHelp.AddRelatedLinksRange(GetRelatedLinks(helpItem));

            return cmdHelp;
        }

        protected IEnumerable<Parameter> GetParameters(CommandInfo cmdletInfo, dynamic? helpItem, bool addDefaultString)
        {
            List<Parameter> parameters = new();

            if (cmdletInfo.Parameters is null || cmdletInfo.Parameters.Count < 1)
            {
                return parameters;
            }

            foreach (KeyValuePair<string, ParameterMetadata> parameterMetadata in cmdletInfo.Parameters)
            {
                string parameterName = parameterMetadata.Key;
                if (Constants.CommonParametersNames.Contains(parameterName))
                {
                    continue;
                }

                var paramAttribInfo = GetParameterAtributeInfo(parameterMetadata.Value.Attributes);
                string typeName = GetParameterTypeName(parameterMetadata.Value.ParameterType);

                Parameter param = new(parameterMetadata.Value.Name, typeName); 
                param.DontShow = paramAttribInfo.DontShow;
                param.SupportsWildcards = paramAttribInfo.Globbing;
                param.HelpMessage = paramAttribInfo.HelpMessage ?? string.Empty;

                foreach (KeyValuePair<string, ParameterSetMetadata> paramSet in parameterMetadata.Value.ParameterSets)
                {
                    string parameterSetName = string.Compare(paramSet.Key, Constants.ParameterSetsAllName) == 0 ? Constants.ParameterSetsAll : paramSet.Key;
                    ParameterSetMetadata metadata = paramSet.Value;
                    var pSet = new Model.ParameterSet(parameterSetName);
                    pSet.Position = metadata.Position == int.MinValue ? Constants.NamedString : paramSet.Value.Position.ToString();
                    pSet.IsRequired = metadata.IsMandatory;
                    pSet.ValueByPipeline = metadata.ValueFromPipeline;
                    pSet.ValueByPipelineByPropertyName = metadata.ValueFromPipelineByPropertyName;
                    pSet.ValueFromRemainingArguments = metadata.ValueFromRemainingArguments;
                    param.ParameterSets.Add(pSet);
                }

                param.DefaultValue = GetParameterDefaultValueFromHelp(helpItem, param.Name);
                param.Aliases = string.Join(",", parameterMetadata.Value.Aliases);

                string descriptionFromHelp = GetParameterDescriptionFromHelp(helpItem, param.Name) ?? param.HelpMessage ?? string.Empty;
                param.Description = string.IsNullOrEmpty(descriptionFromHelp) ?
                    string.Format(Constants.FillInParameterDescriptionTemplate, param.Name) :
                    descriptionFromHelp.Trim();

                parameters.Add(param);
            }

            return parameters.OrderBy(param => param.Name);
        }

        protected static IEnumerable<Example> GetExamples(dynamic? helpItem, bool addDefaultString)
        {
            List<Example> examples = new();

            if (addDefaultString)
            {
                Example exp = new(
                    Constants.Example1,
                    Constants.FillInExampleDescription
                    );
                examples.Add(exp);
            }
            else
            {
                int exampleCounter = 1;

                var examplesArray = helpItem?.examples?.example;

                if (examplesArray is not null)
                {
                    Collection<PSObject> examplesAsCollection = MakePSObjectEnumerable(examplesArray);

                    foreach (dynamic item in examplesAsCollection)
                    {
                        string title = item.title.ToString().Trim(' ', '-').Replace($"Example {exampleCounter}: ", string.Empty);

                        Example exp = new(
                            title,
                            GetStringFromDescriptionArray(item.remarks)
                            );

                        examples.Add(exp);
                        exampleCounter++;
                    }
                }
            }

            return examples;
        }

        protected static List<Links> GetRelatedLinks(dynamic? helpItem)
        {
            List<Links> links = new();

            if (helpItem?.relatedLinks?.navigationLink is not null)
            {
                Collection<PSObject> navigationLinkCollection = MakePSObjectEnumerable(helpItem.relatedLinks.navigationLink);

                foreach (dynamic navlink in navigationLinkCollection)
                {
                    var uri = navlink?.uri is null ? string.Empty : navlink.uri.ToString();
                    var linkText = navlink?.linkText is null ? string.Empty : navlink.linkText.ToString();
                    links.Add(new Links(uri, linkText));
                }
            }

            return links;
        }

        protected IEnumerable<SyntaxItem> GetSyntaxItem(CommandInfo? cmdletInfo, dynamic? helpItem)
        {
            List<SyntaxItem> syntaxItems = new();

            if (cmdletInfo is null)
            {
                return syntaxItems;
            }

            foreach (CommandParameterSetInfo parameterSetInfo in cmdletInfo.ParameterSets)
            {
                SyntaxItem syn = new(cmdletInfo.Name, parameterSetInfo.Name, parameterSetInfo.IsDefault);

                foreach (CommandParameterInfo paramInfo in parameterSetInfo.Parameters)
                    {
                    if (IsNotCommonParameter(paramInfo.Name)) {
                        syn.SyntaxParameters.Add(
                            new SyntaxParameter(
                                paramInfo.Name,
                                GetParameterTypeName(paramInfo.ParameterType),
                                paramInfo.Position == int.MinValue ? "named" : paramInfo.Position.ToString(),
                                paramInfo.IsMandatory,
                                paramInfo.Position != int.MinValue,
                                string.Compare(paramInfo.ParameterType.Name, "SwitchParameter", true) == 0)
                        );
                    }
                    Parameter param = GetParameterInfo(cmdletInfo, helpItem, paramInfo);
                    syn.AddParameter(param);
                }

                syntaxItems.Add(syn);
            }

            return syntaxItems;
        }

        private bool IsNotCommonParameter(string name)
        {
            return ! Constants.CommonParametersNames.Contains(name);
        }

        private string GetParameterTypeName(Type type)
        {
            string typeName = string.Empty;

            if (type.IsGenericType)
            {
                StringBuilder sb = Constants.StringBuilderPool.Get();

                try
                {
                    string genericName = Settings.UseFullTypeName.HasValue && Settings.UseFullTypeName.Value ?
                        type.GetGenericTypeDefinition().FullName ?? string.Empty :
                        type.GetGenericTypeDefinition().Name;

                    if (genericName.Contains(Constants.GenericParameterBackTick))
                    {
                        // This removes `2 from type name like: System.Collections.Generic.Dictionary`2
                        sb.Append(genericName.Remove(genericName.IndexOf(Constants.GenericParameterBackTick)));
                    }
                    else
                    {
                        sb.Append(genericName);
                    }

                    sb.Append(Constants.GenericParameterTypeNameStart);

                    List<string> genericParameters = new();

                    foreach (var name in type.GenericTypeArguments)
                    {
                        if (name.FullName is not null)
                        {
                            genericParameters.Add(name.FullName);
                        }
                    }

                    sb.Append(string.Join(",", genericParameters));

                    sb.Append(Constants.GenericParameterTypeNameEnd);

                    typeName = sb.ToString();
                }
                finally
                {
                    Constants.StringBuilderPool.Return(sb);
                }
            }
            else
            {
                typeName = Settings.UseFullTypeName.HasValue && Settings.UseFullTypeName.Value ?
                    type.FullName ?? string.Empty :
                    type.Name;
            }

            return typeName;
        }

        protected Parameter GetParameterInfo(CommandInfo? cmdletInfo, dynamic? helpItem, CommandParameterInfo paramInfo)
        {
            var paramAttribInfo = GetParameterAtributeInfo(paramInfo.Attributes);

            string typeName = GetParameterTypeName(paramInfo.ParameterType);

            Parameter param = new(paramInfo.Name, typeName);

            string descriptionFromHelp = GetParameterDescriptionFromHelp(helpItem, param.Name) ?? paramAttribInfo.HelpMessage ?? string.Empty;

            param.Description = string.IsNullOrEmpty(descriptionFromHelp) ?
                string.Format(Constants.FillInParameterDescriptionTemplate, param.Name) :
                descriptionFromHelp;

            param.Aliases = string.Join("-", paramInfo.Aliases);
            param.ParameterSets.ForEach(x => x.IsRequired = paramInfo.IsMandatory);

            string defaultValueFromHelp = GetParameterDefaultValueFromHelp(helpItem, param.Name);

            param.DefaultValue = string.IsNullOrEmpty(defaultValueFromHelp) ?
                Constants.NoneString :
                defaultValueFromHelp;

            return param;
        }

        internal class ParameterAttributeInfo
        {
            internal bool DontShow { get; set; }
            internal bool PipelineInput { get; set; }
            internal bool Required { get; set; }
            internal bool Globbing { get; set; }
            internal string Position { get; set; }
            internal string? HelpMessage { get; set; }

            public ParameterAttributeInfo(
                bool dontShow,
                bool pipelineInput,
                bool required,
                string position,
                string? helpMessage,
                bool globbing)
            {
                DontShow = dontShow;
                PipelineInput = pipelineInput;
                Required = required;
                Position = position;
                HelpMessage = helpMessage;
                Globbing = globbing;
            }
        }

        protected static ParameterAttributeInfo GetParameterAtributeInfo(IEnumerable<Attribute> attributes)
        {
            bool dontShow = false;
            bool pipelineInput = false;
            string position = Constants.NamedString;
            bool required = false;
            string? helpMessage = null;
            bool globbing = false;
            IList<string> acceptedValues;

            foreach (var attrib in attributes)
            {
                switch (attrib)
                {
                    case ParameterAttribute parameterAttribute:
                        dontShow = parameterAttribute.DontShow;
                        pipelineInput = parameterAttribute.ValueFromPipeline | parameterAttribute.ValueFromPipelineByPropertyName;
                        position = parameterAttribute.Position == int.MinValue ? Constants.NamedString : parameterAttribute.Position.ToString();
                        required = parameterAttribute.Mandatory;
                        helpMessage = parameterAttribute.HelpMessage;

                        break;

                    case SupportsWildcardsAttribute:
                        globbing = true;
                        break;

                    case ValidateSetAttribute validateSetAttribute:
                        acceptedValues = validateSetAttribute.ValidValues;
                        break;
                }
            }

            return new ParameterAttributeInfo(dontShow, pipelineInput, required, position, helpMessage, globbing);
        }

        protected static IEnumerable<string> GetParameterSetsOfParameter(string parameterName, CommandInfo cmdletInfo)
        {
            if (cmdletInfo.Parameters.TryGetValue(parameterName, out ParameterMetadata? paramMetadata))
            {
                if (paramMetadata is not null)
                {
                    return paramMetadata.ParameterSets.Keys;
                }
                else
                {
                    return Constants.EmptyStringList;
                }
            }

            return Constants.EmptyStringList;
        }

        protected static string? GetParameterDescriptionFromHelp(dynamic? helpItem, string parameterName)
        {
            if (helpItem?.parameters?.parameter == null)
            {
                return null;
            }

            Collection<PSObject>? parameterAsCollection = MakePSObjectEnumerable(helpItem.parameters.parameter);

            foreach (dynamic parameter in parameterAsCollection)
            {
                if (string.Equals(parameter.name.ToString(), parameterName, StringComparison.OrdinalIgnoreCase))
                {
                    var paramDescription = GetStringFromDescriptionArray(parameter.description);
                    return paramDescription == string.Empty ? null : paramDescription;
                }
            }

            return null;
        }

        protected static string GetParameterDefaultValueFromHelp(dynamic? helpItem, string parameterName)
        {
            if (helpItem?.parameters?.parameter == null)
            {
                return string.Empty;
            }

            Collection<PSObject>? parameterAsCollection = MakePSObjectEnumerable(helpItem.parameters.parameter);

            foreach (dynamic parameter in parameterAsCollection)
            {
                if (string.Equals(parameter.name.ToString(), parameterName, StringComparison.OrdinalIgnoreCase))
                {
                    return parameter.defaultValue is null ? string.Empty : parameter.defaultValue.ToString();
                }
            }

            return string.Empty;
        }

        protected static string? GetNotes(dynamic? helpItem, bool addDefaultString)
        {
            if (addDefaultString)
            {
                return Constants.FillInNotes;
            }
            else
            {
                return helpItem?.alertSet?.alert is not null ?
                    GetStringFromDescriptionArray(helpItem.alertSet.alert) :
                    string.Empty;
            }
        }

        protected static string GetDescription(dynamic? helpItem, bool addDefaultStrings)
        {
            if (addDefaultStrings)
            {
                return Constants.FillInDescription;
            }
            else
            {
                if (helpItem is null)
                {
                    throw new ArgumentNullException(nameof(helpItem));
                }

                return GetStringFromDescriptionArray(helpItem.description);
            }
        }

        protected static string GetSynopsis(dynamic? helpItem, bool addDefaultStrings)
        {
            if (addDefaultStrings)
            {
                return Constants.FillInSynopsis;
            }
            else
            {
                return helpItem is not null ? helpItem.Synopsis.ToString() : throw new ArgumentNullException(nameof(helpItem));
            }
        }

        protected InputOutput? GetInputOutputItem(dynamic typesInfo, string defaultTypeName, string defaultDescription)
        {
            var inputOutput = new InputOutput(defaultTypeName, defaultDescription);
            if (string.IsNullOrEmpty(defaultTypeName) && string.IsNullOrEmpty(defaultDescription))
            {
                dynamic ioTypes = typesInfo;

                if (ioTypes is IEnumerable<PSObject>)
                {
                    foreach (dynamic ioType in typesInfo)
                    {
                        string typeName = ioType.type.ToString();
                        inputOutput = new InputOutput(typeName, GetStringFromDescriptionArray(ioType.description));
                    }
                }
                else if (ioTypes is PSObject)
                {
                    inputOutput = new InputOutput(ioTypes.type.name.ToString(), GetStringFromDescriptionArray(ioTypes.description));
                }
            }

            return inputOutput;
        }

        protected static string GetStringFromDescriptionArray(dynamic? description)
        {
            if (description == null)
            {
                return string.Empty;
            }

            if (description is string)
            {
                return description;
            }

            StringBuilder sb = Constants.StringBuilderPool.Get();

            try
            {
                foreach (dynamic line in description)
                {
                    if (line is not char)
                    {
                        string text = line.text.ToString();

                        // Add semantic line break.
                        sb.AppendLine(text.Replace(". ", $".{Environment.NewLine}"));
                    }
                }

                return sb.ToString();
            }
            finally
            {
                Constants.StringBuilderPool.Return(sb);
            }
        }

        private static Collection<PSObject> MakePSObjectEnumerable(dynamic psObject)
        {
            Collection<PSObject> forceEnumerable = new();

            if (psObject is PSObject)
            {
                forceEnumerable = new Collection<PSObject>
                {
                    psObject
                };
            }
            else if (psObject is PSObject[])
            {
                forceEnumerable = new Collection<PSObject>();

                foreach (var item in psObject)
                {
                    forceEnumerable.Add(item);
                }
            }
            else if (psObject is Collection<PSObject>)
            {
                return psObject;
            }
            else if (psObject is object[])
            {
                forceEnumerable = new Collection<PSObject>();

                foreach (var item in psObject)
                {
                    forceEnumerable.Add(new PSObject(item));
                }
            }

            return forceEnumerable;
        }
    }
}
