// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using System.Management.Automation.Language;
using System.Text;
using Markdig.Helpers;
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
            cmdHelp.ExternalHelpFile = cmdHelp.Metadata["external help file"].ToString() ?? string.Empty;
            cmdHelp.OnlineVersionUrl = Settings.OnlineVersionUrl ?? cmdHelp.Metadata["HelpUri"] as string;
            cmdHelp.SchemaVersion = cmdHelp.Metadata["PlatyPS schema version"] as string ?? string.Empty;
            cmdHelp.Synopsis = GetSynopsis(helpItem, addDefaultStrings);
            cmdHelp.AddSyntaxItemRange(GetSyntaxItem(commandInfo, helpItem));
            cmdHelp.Description = GetDescription(helpItem, addDefaultStrings).Trim();
            cmdHelp.AddExampleItemRange(GetExamples(helpItem, addDefaultStrings));
            var parameters = GetParameters(commandInfo, helpItem, addDefaultStrings);
            cmdHelp.AddParameterRange(parameters);

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

            foreach(var input in GetInputInfo(commandInfo, helpItem, addDefaultStrings))
            {
                cmdHelp.AddInputItem(input);
            }

            foreach(var output in GetOutputInfo(commandInfo, helpItem, addDefaultStrings))
            {
                cmdHelp.AddOutputItem(output);
            }

            cmdHelp.Notes = GetNotes(helpItem, addDefaultStrings);
            cmdHelp.AddRelatedLinksRange(GetRelatedLinks(helpItem));

            return cmdHelp;
        }


        // We don't want to return any arrays, so trim the last "[]" if it is found;
        // We can also simplify the string if desired.
        private string GetAdjustedTypename(Type t, bool simplify = false)
        {
            var t2 = Nullable.GetUnderlyingType(t) ?? t;
            string tName = simplify ?  LanguagePrimitives.ConvertTo<string>(t2) : t2.FullName;
            return tName;
        }

        // We need to inspect both the help and the parameters for those that take pipeline input.
        private List<InputOutput> GetInputInfo(CommandInfo commandInfo, dynamic? helpItem, bool addDefaultStrings)
        {
            IEnumerable<ParameterMetadata> parameters;
            // It is possible that the Parameters member is null, so protect against that.
            if (commandInfo.Parameters is null)
            {
                parameters = new List<ParameterMetadata>();
            }
            else
            {
                parameters = commandInfo.Parameters.Values;
            }
            List<InputOutput> inputList = new();
            HashSet<string> inputTypeNames = new();

            // Sometime the help content does not have any input type
            if (helpItem?.inputTypes?.inputType is not null)
            {
                List<InputOutput> ioItems = GetInputOutputItemsFromHelp(helpItem.inputTypes.inputType);
                foreach(var ioItem in ioItems)
                {
                    if (! inputTypeNames.Contains(ioItem.Typename))
                    {
                        inputList.Add(ioItem);
                        inputTypeNames.Add(ioItem.Typename);

                    }
                }
            }

            // Check the parameters for ValueFromPipeline or ValueFromPipelineByPropertyName
            foreach(var parameter in parameters)
            {
                string parameterType = GetAdjustedTypename(parameter.ParameterType);
                foreach(var pSet in parameter.ParameterSets)
                {
                    if (pSet.Value.ValueFromPipeline || pSet.Value.ValueFromPipelineByPropertyName)
                    {
                        if (! inputTypeNames.Contains(parameterType))
                        {
                            inputList.Add(new InputOutput(parameterType, Constants.FillInDescription));
                            inputTypeNames.Add(parameterType);
                        }
                    }
                }
            }

            return inputList;
        }

        private List<InputOutput> GetOutputInfo(CommandInfo commandInfo, dynamic? helpItem, bool addDefaultStrings)
        {
            List<InputOutput> outputList = new();
            HashSet<string> outputTypeNames = new();

            // Sometime the help content does not have any output type
            if (helpItem?.returnValues?.returnValue is not null)
            {
                List<InputOutput> outputItems = GetInputOutputItemsFromHelp(helpItem.returnValues.returnValue);
                foreach(var o in outputItems)
                {
                    if (! outputTypeNames.Contains(o.Typename))
                    {
                        outputList.Add(o);
                        outputTypeNames.Add(o.Typename);
                    }
                }
            }

            // Check for the output on the CommandInfo object
            foreach(var outputType in commandInfo.OutputType) {
                string outputName = outputType.Name;
                if (outputName.EndsWith("[]"))
                {
                    outputName = FixUpTypeName(outputName);
                }

                if (!outputTypeNames.Contains(outputName))
                {
                    outputList.Add(new InputOutput(outputName, Constants.FillInDescription));
                    outputTypeNames.Add(outputName);
                }
            }

            return outputList;
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
                    pSet.ValueFromPipeline = metadata.ValueFromPipeline;
                    pSet.ValueFromPipelineByPropertyName = metadata.ValueFromPipelineByPropertyName;
                    pSet.ValueFromRemainingArguments = metadata.ValueFromRemainingArguments;
                    param.ParameterSets.Add(pSet);
                }

                param.DefaultValue = GetParameterDefaultValueFromHelp(helpItem, param.Name);
                param.Aliases = parameterMetadata.Value.Aliases.ToList();

                string descriptionFromHelp = GetParameterDescriptionFromHelp(helpItem, param.Name) ?? param.HelpMessage ?? string.Empty;
                param.Description = string.IsNullOrEmpty(descriptionFromHelp) ?
                    TransformUtils.GetParameterTemplateString(param.Name) :
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
                            GetExampleDetailFromItem(item)
                            );

                        examples.Add(exp);
                        exampleCounter++;
                    }
                }
            }

            return examples;
        }



        /// <summary>
        /// Retrieve the example string from an item.
        /// This checks 3 different possible properties:
        /// - code
        /// - remarks
        /// and constructs a string representing the example.
        /// introduction is excluded (it seems to be just 'PS >')
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        static string GetExampleDetailFromItem(dynamic? item)
        {
            StringBuilder sb = Constants.StringBuilderPool.Get();
            if (item?.code is not null)
            {
                sb.AppendLine(item.code.ToString().Trim());
            }


            if (item?.remarks is not null)
            {
                var description = GetStringFromDescriptionArray(item.remarks);
                // If we found something in code, be sure to separate it with a newline.
                if (sb.Length > 0)
                {
                    sb.AppendLine();
                }

                sb.AppendLine(description);
            }

            try
            {
                return sb.ToString().Trim();
            }
            finally
            {
                Constants.StringBuilderPool.Return(sb);
            }
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

                // Take the positional parameters first, and order them by position.
                foreach (CommandParameterInfo paramInfo in parameterSetInfo.Parameters.Where(p => p.Position != int.MinValue).OrderBy(p => p.Position))
                {
                    if (IsNotCommonParameter(paramInfo.Name)) {
                        syn.SyntaxParameters.Add(
                            new SyntaxParameter(
                                paramInfo.Name,
                                GetParameterTypeNameForSyntax(paramInfo.ParameterType, paramInfo.Attributes),
                                paramInfo.Position == int.MinValue ? "named" : paramInfo.Position.ToString(),
                                paramInfo.IsMandatory,
                                paramInfo.Position != int.MinValue,
                                string.Compare(paramInfo.ParameterType.Name, "SwitchParameter", true) == 0)
                        );
                    }
                    Parameter param = GetParameterInfo(cmdletInfo, helpItem, paramInfo);
                    syn.AddParameter(param);
                }

                // now take the named parameters.
                foreach (CommandParameterInfo paramInfo in parameterSetInfo.Parameters.Where(p => p.Position == int.MinValue))
                {
                    if (IsNotCommonParameter(paramInfo.Name)) {
                        var sParm = new SyntaxParameter(
                            paramInfo.Name,
                            GetParameterTypeNameForSyntax(paramInfo.ParameterType, paramInfo.Attributes),
                            paramInfo.Position == int.MinValue ? "named" : paramInfo.Position.ToString(),
                            paramInfo.IsMandatory,
                            paramInfo.Position != int.MinValue,
                            string.Compare(paramInfo.ParameterType.Name, "SwitchParameter", true) == 0);
                        syn.SyntaxParameters.Add(sParm);
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

        private string GetParameterTypeNameForSyntax(Type type, IEnumerable<Attribute> attributes)
        {
            string parameterTypeString;
            PSTypeNameAttribute typeName;
            if (attributes != null && (typeName = attributes.OfType<PSTypeNameAttribute>().FirstOrDefault()) != null)
            {
                // If we have a PSTypeName specified on the class, we assume it has a more useful type than the actual
                // parameter type.  This is a reasonable assumption, the parameter binder does honor this attribute.
                //
                // This typename might be long, e.g.:
                //     Microsoft.Management.Infrastructure.CimInstance#root/cimv2/Win32_Process
                //     System.Management.ManagementObject#root\cimv2\Win32_Process
                // To shorten this, we will drop the namespaces, both on the .Net side and the CIM/WMI side:
                //     CimInstance#Win32_Process
                // If our regex doesn't match, we'll just use the full name.
                var match = Regex.Match(typeName.PSTypeName, "(.*\\.)?(?<NetTypeName>.*)#(.*[/\\\\])?(?<CimClassName>.*)");
                if (match.Success)
                {
                    parameterTypeString = match.Groups["NetTypeName"].Value + "#" + match.Groups["CimClassName"].Value;
                }
                else
                {
                    parameterTypeString = typeName.PSTypeName;

                    // Drop the namespace from the typename, if any.
                    var lastDotIndex = parameterTypeString.LastIndexOf('.');
                    if (lastDotIndex != -1 && lastDotIndex + 1 < parameterTypeString.Length)
                    {
                        parameterTypeString = parameterTypeString.Substring(lastDotIndex + 1);
                    }
                }

                // If the type is really an array, but the typename didn't include [], then add it.
                if (type.IsArray && !parameterTypeString.Contains("[]"))
                {
                    var t = type;
                    while (t.IsArray)
                    {
                        parameterTypeString += "[]";
                        t = t.GetElementType();
                    }
                }
            }
            else
            {
                Type parameterType = Nullable.GetUnderlyingType(type) ?? type;
                // don't over abbreviate the type if it's a switch parameter, since we don't print it in the syntax.
                if (parameterType == typeof(System.Management.Automation.SwitchParameter))
                {
                    parameterTypeString = "SwitchParameter";
                }
                else
                {
                    parameterTypeString = GetAbbreviatedType(parameterType);
                }
            }

            return parameterTypeString;
        }

        private void AddGenericArguments(StringBuilder sb, Type[] genericArguments)
        {
            sb.Append($"`{genericArguments.Length}[");
            for (int i = 0; i < genericArguments.Length; i++)
            {
                if (i > 0) { sb.Append(','); }

                sb.Append(GetAbbreviatedType(genericArguments[i]));
            }

            sb.Append(']');
        }

        private string GetAbbreviatedType(Type type)
        {
            if (type is null)
            {
                return string.Empty;
            }

            string result;
            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                string genericDefinition = GetAbbreviatedType(type.GetGenericTypeDefinition());
                // For regular generic types, we find the backtick character, for example:
                //      System.Collections.Generic.List`1[T] ->
                //      System.Collections.Generic.List[string]
                // For nested generic types, we find the left bracket character, for example:
                //      System.Collections.Generic.Dictionary`2+Enumerator[TKey, TValue] ->
                //      System.Collections.Generic.Dictionary`2+Enumerator[string,string]
                int backtickOrLeftBracketIndex = genericDefinition.LastIndexOf(type.IsNested ? '[' : '`');
                var sb = new StringBuilder(genericDefinition, 0, backtickOrLeftBracketIndex, 512);
                AddGenericArguments(sb, type.GetGenericArguments());
                result = sb.ToString();
            }
            else if (type.IsArray)
            {
                string elementDefinition = GetAbbreviatedType(type.GetElementType());
                var sb = new StringBuilder(elementDefinition, elementDefinition.Length + 10);
                sb.Append('[');
                for (int i = 0; i < type.GetArrayRank() - 1; ++i)
                {
                    sb.Append(',');
                }

                sb.Append(']');
                result = sb.ToString();
            }
            else
            {
                if (TransformUtils.TryGetTypeAbbreviation(type.FullName, out string abbreviation))
                {
                    return abbreviation;
                }

                if (type == typeof(PSCustomObject))
                {
                    return type.Name;
                }

                if (type.IsNested)
                {
                    // For nested types, we should return OuterType+InnerType. For example,
                    //  System.Environment+SpecialFolder ->  Environment+SpecialFolder
                    string fullName = type.ToString();
                    result = type.Namespace == null
                                ? fullName
                                : fullName.Substring(type.Namespace.Length + 1);
                }
                else
                {
                    result = type.Name;
                }
            }

            return result;
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

                    sb.Append(string.Format(Constants.GenericParameterTypeNameStart, type.GetGenericArguments().Length));

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
                TransformUtils.GetParameterTemplateString(param.Name) :
                descriptionFromHelp;

            param.Aliases = paramInfo.Aliases.ToList();
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

        protected List<InputOutput> GetInputOutputItemsFromHelp(dynamic typesInfo)
        {
            dynamic ioTypes = typesInfo;
            List<InputOutput> itemList = new();

            if (ioTypes is IEnumerable<PSObject>)
            {
                foreach (dynamic ioType in typesInfo)
                {
                    string typeName = FixUpTypeName(ioType.type.name?.Split()?[0] ?? string.Empty);
                    if (! string.IsNullOrEmpty(typeName) && string.Compare(typeName, "None", true) != 0)
                    {
                        string description = GetStringFromDescriptionArray(ioType.description)?.Trim() ?? string.Empty;
                        itemList.Add(new InputOutput(typeName, string.IsNullOrEmpty(description) ? Constants.FillInDescription : description));
                    }
                }
            }
            else if (ioTypes is PSObject)
            {
                if  (ioTypes.type.name is string name)
                {
                    name = name.Trim();
                    // Sometimes, help will return lines which have embedded newlines.
                    // these are really multiple entries, so split them here.
                    if (name.IndexOf("\n") == -1 && string.Compare(name, "None", true) != 0)
                    {
                        itemList.Add(new InputOutput(FixUpTypeName(name), Constants.FillInDescription));
                    }
                    else
                    {
                        foreach(var tName in name.Replace("\\r","").Split('\n'))
                        {
                            if (string.Compare(tName, "None", true) != 0)
                            {
                                itemList.Add(new InputOutput(FixUpTypeName(tName), Constants.FillInDescription));
                            }
                        }
                    }
                }
                else
                {
                    string typeName = FixUpTypeName(ioTypes.type.name.ToString());
                    if (! string.IsNullOrEmpty(typeName) && string.Compare(typeName, "None", true) != 0)
                    {
                        string description = GetStringFromDescriptionArray(ioTypes.description).Trim();
                        itemList.Add(new InputOutput(typeName, string.IsNullOrEmpty(description) ? Constants.FillInDescription : description));
                    }
                }
            }

            return itemList;
        }

        // We have to remove carriage returns that might be present from help
        // We also will remove trailing [] because we should generally return singletons
        private string FixUpTypeName(string typename)
        {
            // If the type is a generic type, we need to remove the backtick and the number.
            string fixedString = typename.Replace("System.Nullable`1[[", string.Empty).Trim();
            int commaIndex = fixedString.IndexOf(',');
            if (commaIndex >= 0)
            {
                fixedString = fixedString.Substring(0, commaIndex).Trim();
            }

            if (fixedString.EndsWith("[]"))
            {
                fixedString = fixedString.Remove(fixedString.Length - 2);
            }

            return fixedString;
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

            if (description is not IEnumerable && description is PSObject)
            {
                return description.ToString();
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
