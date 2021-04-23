using System;
using System.Collections.Generic;
using System.Globalization;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.PowerShell.PlatyPS.Tests,PublicKey=0024000004800000940000000602000000240000525341310004000001000100b5fc90e7027f67871e773a8fde8938c81dd402ba65b9201d60593e96c492651e889cc13f1415ebb53fac1131ae0bd333c5ee6021672d9718ea31a8aebd0da0072f25d87dba6fc90ffd598ed4da35e44c398c454307e8e33b8426143daec9f596836f97c8f74750e5975c64e2189f45def46b2a2b1247adc3652bf5c308055da9")]
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
            Parameters ??= new();
            Parameters.Add(parameter);
        }

        internal void AddParameterRange(IEnumerable<Parameter> parameters)
        {
            Parameters ??= new();
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
