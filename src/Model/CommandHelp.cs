// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.PowerShell.PlatyPS.Model
{
    /// <summary>
    /// Model for representing data for help of a command.
    /// </summary>
    internal class CommandHelp
    {
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

        internal string? Notes { get; set; }

        public CommandHelp(string title, string moduleName, CultureInfo? cultureInfo)
        {
            Title = title;
            ModuleName = moduleName;
            Locale = cultureInfo ?? CultureInfo.GetCultureInfo("en-US");
            Syntax = new();
            Parameters = new();
            Synopsis = string.Empty;
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

    }
}
