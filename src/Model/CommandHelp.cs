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

        internal List<Links> RelatedLinks { get; set; }

        internal string Notes { get; set; }

        internal void AddSyntaxItem(SyntaxItem syntax)
        {
            Syntax ??= new List<SyntaxItem>();
            Syntax.Add(syntax);
        }

        internal void AddExampleItem(Example example)
        {
            Examples ??= new List<Example>();
            Examples.Add(example);
        }

        internal void AddParameter(Parameter parameter)
        {
            Parameters ??= new List<Parameter>();
            Parameters.Add(parameter);
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
    }
}
