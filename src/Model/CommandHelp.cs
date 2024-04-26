// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using YamlDotNet.Serialization;

namespace Microsoft.PowerShell.PlatyPS.Model
{
    /// <summary>
    /// Model for representing data for help of a command.
    /// </summary>
    public partial class CommandHelp : IEquatable<CommandHelp>
    {
        public OrderedDictionary? Metadata { get; set; }

        public CultureInfo Locale { get; set; }

        public Guid? ModuleGuid { get; set; }

        public string? ExternalHelpFile { get; set; }

        public string? OnlineVersionUrl { get; set; }

        public string? SchemaVersion { get; set; }

        public string ModuleName { get; set; }

        public string Title { get; set; }

        public string Synopsis { get; set; }

        public List<SyntaxItem> Syntax { get; private set; }

        public List<string>? Aliases { get; private set; }

        public string? Description { get; set; }

        public List<Example>? Examples { get; private set; }

        public List<Parameter> Parameters { get; private set; }

        public List<InputOutput> Inputs { get; private set; }

        public List<InputOutput> Outputs { get; private set; }

        public string? Notes { get; set; }

        public List<Links>? RelatedLinks { get; private set; }

        [YamlIgnore]
        public bool HasCmdletBinding { get; set; }

        [YamlIgnore]
        public bool HasWorkflowCommonParameters { get; set; }

        [YamlIgnore]
        public Diagnostics Diagnostics { get; set; }

        internal Dictionary<string, SyntaxItem> SyntaxDictionary { get; private set; }

        public CommandHelp(string title, string moduleName, CultureInfo? cultureInfo)
        {
            Aliases = new();
            Title = title;
            ModuleName = moduleName;
            Locale = cultureInfo ?? CultureInfo.GetCultureInfo("en-US");
            Syntax = new();
            Parameters = new();
            Inputs = new();
            Outputs = new();
            RelatedLinks = new();
            Examples = new();
            Synopsis = string.Empty;
            Metadata = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
            SyntaxDictionary = new();
            Diagnostics = new();
        }

        public CommandHelp()
        {
            Syntax = new();
            Aliases = new();
            Locale = CultureInfo.GetCultureInfo("en-US");
            Synopsis = string.Empty;
            Examples = new();
            Parameters = new();
            Inputs = new();
            Outputs = new();
            RelatedLinks = new();
            Title = string.Empty;
            ModuleName = string.Empty;
            Metadata = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
            SyntaxDictionary = new();
            Diagnostics = new();
        }

        public override string ToString()
        {
            return Title;
        }

        internal void AddMetadata(string key, object value)
        {
            Metadata ??= new();
            Metadata.Add(key, value);
        }

        internal void AddSyntaxItem(SyntaxItem syntax)
        {
            Syntax.Add(syntax);
            // We should allow this to throw if the syntax name is not unique.
            var pSetName = syntax.ParameterSetName ?? "Default";
            SyntaxDictionary.Add(pSetName, syntax);
        }

        internal void AddSyntaxItemRange(IEnumerable<SyntaxItem> syntaxItems)
        {
            Syntax.AddRange(syntaxItems);
            // We should allow this to throw if the syntax name is not unique.
            foreach(var syntax in syntaxItems)
            {
                var pSetName = syntax.ParameterSetName ?? "Default";
                SyntaxDictionary.Add(pSetName, syntax);
            }
        }

        internal void AddAlias(string alias)
        {
            Aliases ??= new();
            Aliases.Add(alias);
        }

        internal void AddExampleItem(Example example)
        {
            Examples ??= new();
            Examples.Add(example);
        }

        internal void AddExampleItemRange(IEnumerable<Example> example)
        {
            Examples ??= new();
            Examples.AddRange(example);
        }

        internal void AddParameter(Parameter parameter)
        {
            Parameters.Add(parameter);
            foreach(var parameterSet in parameter.ParameterSets)
            {
                if (string.Compare(parameterSet.Name, "(All)", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    foreach(var syntax in SyntaxDictionary.Values)
                    {
                        try
                        {
                            syntax.AddParameter(parameter);
                        }
                        catch
                        {
                            // This is okay, we just don't want to add it to the syntax item if it's already there.
                        }
                    }
                }
                else if (SyntaxDictionary.TryGetValue(parameterSet.Name, out var syntaxItem))
                {
                    try
                    {
                        syntaxItem.AddParameter(parameter);
                    }
                    catch
                    {
                        // This is okay, we just don't want to add it to the syntax item if it's already there.
                    }
                }
            }
        }

        public bool TryGetParameter(string name, out Parameter? parameter)
        {
            var param = Parameters.Find(p => string.Compare(p.Name, name, StringComparison.CurrentCultureIgnoreCase) == 0);
            if (param is not null)
            {
                parameter = param;
                return true;
            }

            parameter = null;
            return false;
        }

        internal void AddParameterRange(IEnumerable<Parameter> parameters)
        {
            Parameters.AddRange(parameters);
        }

        internal void AddInputItem(InputOutput inputItem)
        {
            Inputs ??= new();
            Inputs.Add(inputItem);
        }

        internal void AddOutputItem(InputOutput outputItem)
        {
            Outputs ??= new();
            Outputs.Add(outputItem);
        }

        internal void AddRelatedLinks(Links relatedLink)
        {
            RelatedLinks ??= new();
            RelatedLinks.Add(relatedLink);
        }

        internal void AddRelatedLinksRange(IEnumerable<Links> relatedLinks)
        {
            RelatedLinks ??= new();
            RelatedLinks.AddRange(relatedLinks);
        }

        internal bool TryParse(IDictionary<string,object> dictionary, out CommandHelp commandHelp)
        {
            var help = new CommandHelp();
            try {
                foreach (var key in dictionary.Keys)
                {
                    if (key is string keyString)
                    {
                        switch (keyString)
                        {
                            case "locale":
                                if (dictionary[key] is string locale)
                                {
                                    help.Locale = CultureInfo.GetCultureInfo(locale);
                                }
                                break;
                            case "module_guid":
                                if (dictionary[key] is string moduleGuid)
                                {
                                    help.ModuleGuid = Guid.Parse(moduleGuid);
                                }
                                break;
                            case "external_help":
                                if (dictionary[key] is string externalHelpFile)
                                {
                                    help.ExternalHelpFile = externalHelpFile;
                                }
                                break;
                            case "online_version":
                                if (dictionary[key] is string onlineVersionUrl)
                                {
                                    help.OnlineVersionUrl = onlineVersionUrl;
                                }
                                break;
                            case "schema_version":
                                if (dictionary[key] is string schemaVersion)
                                {
                                    help.SchemaVersion = schemaVersion;
                                }
                                break;
                            case "module_name":
                                if (dictionary[key] is string moduleName)
                                {
                                    help.ModuleName = moduleName;
                                }
                                break;
                            case "title":
                                if (dictionary[key] is string title)
                                {
                                    help.Title = title;
                                }
                                break;
                            case "synopsis":
                                if (dictionary[key] is string synopsis)
                                {
                                    help.Synopsis = synopsis;
                                }
                                break;
                            case "syntax":
                                if (dictionary[key] is List<object> syntax)
                                {
                                    foreach (var syntaxItem in syntax)
                                    {
                                        if (syntaxItem is IDictionary<string, object> syntaxDictionary)
                                        {
                                            var syntaxItemModel = new SyntaxItem("name","description", false);
                                        }
                                    }
                                }
                                break;
                            case "metadata":
                                if (dictionary[key] is IDictionary<string, object> metadata)
                                {
                                    foreach (var metadataItem in metadata)
                                    {
                                        if (metadataItem.Value is string metadataValue)
                                        {
                                            help.AddMetadata(metadataItem.Key, metadataValue);
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                commandHelp = help;
                return true;
            }
            catch
            {
                commandHelp = help;
                return false;
            }
        }

    }
}
