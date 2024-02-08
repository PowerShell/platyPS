// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;

namespace Microsoft.PowerShell.PlatyPS.Model
{
    /// <summary>
    /// Model for representing data for help of a command.
    /// </summary>
    internal class CommandHelp
    {
        internal OrderedDictionary? Metadata { get; set; }

        internal CultureInfo Locale { get; set; }

        internal Guid? ModuleGuid { get; set; }

        internal string? ExternalHelpFile { get; set; }

        internal string? OnlineVersionUrl { get; set; }

        internal string? SchemaVersion { get; set; }

        internal string ModuleName { get; set; }

        internal string Title { get; set; }

        internal string Synopsis { get; set; }

        internal List<SyntaxItem> Syntax { get; private set; }

        internal List<string>? Aliases { get; private set; }

        internal string? Description { get; set; }

        internal List<Example>? Examples { get; private set; }

        internal List<Parameter> Parameters { get; private set; }

        internal List<InputOutput>? Inputs { get; private set; }

        internal List<InputOutput>? Outputs { get; private set; }

        internal List<Links>? RelatedLinks { get; private set; }

        internal bool HasCmdletBinding { get; set; }

        internal bool HasWorkflowCommonParameters { get; set; }

        internal string? Notes { get; set; }

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
        }

        internal void AddMetadata(string key, object value)
        {
            Metadata ??= new();
            Metadata.Add(key, value);
        }

        internal void AddSyntaxItem(SyntaxItem syntax)
        {
            Syntax.Add(syntax);
        }

        internal void AddSyntaxItemRange(IEnumerable<SyntaxItem> syntaxItems)
        {
            Syntax.AddRange(syntaxItems);
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

        internal void AddReleatedLinks(Links relatedLink)
        {
            RelatedLinks ??= new();
            RelatedLinks.Add(relatedLink);
        }

        internal void AddReleatedLinksRange(IEnumerable<Links> relatedLinks)
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
