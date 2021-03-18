using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.PowerShell.PlatyPS.Model
{
    internal class CommandHelp
    {
        internal string ModuleName { get; set; }

        internal string Title { get; set; }

        internal string Synopsis { get; set; }

        internal List<SyntaxItem> Syntax { get; private set; }

        internal string Description { get; set; }

        internal List<Example> Examples { get; private set; }

        internal List<Parameter> Parameters { get; private set; }

        internal List<InputOutput> Inputs { get; private set; }

        internal List<InputOutput> Outputs { get; private set; }

        internal List<Links> RelatedLinks { get; private set; }

        internal string Notes { get; set; }

        internal void AddSyntaxItem(SyntaxItem syntax)
        {
            Syntax ??= new List<SyntaxItem>();
            Syntax.Add(syntax);
        }

        internal void AddSyntaxItemRange(IEnumerable<SyntaxItem> syntaxItems)
        {
            Syntax ??= new List<SyntaxItem>();
            Syntax.AddRange(syntaxItems);
        }

        internal void AddExampleItem(Example example)
        {
            Examples ??= new List<Example>();
            Examples.Add(example);
        }
        internal void AddExampleItemRange(IEnumerable<Example> example)
        {
            Examples ??= new List<Example>();
            Examples.AddRange(example);
        }

        internal void AddParameter(Parameter parameter)
        {
            Parameters ??= new List<Parameter>();
            Parameters.Add(parameter);
        }

        internal void AddParameterRange(IEnumerable<Parameter> parameters)
        {
            Parameters ??= new List<Parameter>();
            Parameters.AddRange(parameters);
        }

        internal void AddInputItem(InputOutput inputItem)
        {
            Inputs ??= new List<InputOutput>();
            Inputs.Add(inputItem);
        }

        internal void AddOutputItem(InputOutput outputItem)
        {
            Outputs ??= new List<InputOutput>();
            Outputs.Add(outputItem);
        }

        internal void AddReleatedLinks(Links relatedLink)
        {
            RelatedLinks ??= new List<Links>();
            RelatedLinks.Add(relatedLink);
        }

        internal void AddReleatedLinksRange(IEnumerable<Links> relatedLinks)
        {
            RelatedLinks ??= new List<Links>();
            RelatedLinks.AddRange(relatedLinks);
        }
    }
}
