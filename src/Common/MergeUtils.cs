// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    public class MergeUtils
    {
        /// <summary>
        /// This is can be used to alter the behavior of the wildcard support.
        /// If set to false, it will use the value from the command.
        /// </summary>
        public static bool UseHelpValueForWildcard = true;

        /// <summary>
        /// Merge the new data found in the cmdlet object.
        /// </summary>
        /// <param name="fromMarkdown"></param>
        /// <param name="fromCmdlet"></param>
        /// <returns></returns>
        internal static CommandHelp MergeCommandHelp(CommandHelp fromMarkdown, CommandHelp fromCmdlet)
        {
            // Create a copy of the help from the markdown file as our base.
            // We will update this copy with new information from the cmdlet.
            CommandHelp helpCopy = new (fromMarkdown);

            // The following are not available in the cmdlet, so we will just use what was in the help.
            // - Synopsis
            // - Description
            // - Notes
            // - Examples
            // - Related Links
            // The remaining properties will be updated with the cmdlet information.

            // Syntax
            if (MergeUtils.TryGetMergedSyntax(helpCopy.Syntax, fromCmdlet.Syntax, out var mergedSyntaxList, out var syntaxDiagnostics))
            {
                helpCopy.Syntax.Clear();
                helpCopy.Syntax.AddRange(mergedSyntaxList);
            }
            syntaxDiagnostics.ForEach(d => helpCopy.Diagnostics.TryAddDiagnostic(d));

            // Alias - there are no aliases to be found in the cmdlet, but we shouldn't add the boiler plate.
            // If the boiler plate is found, we'll clear that.
            helpCopy.AliasHeaderFound = true;
            if (helpCopy.Aliases.IndexOf(Constants.AliasMessage2, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                helpCopy.Aliases = string.Empty;
            }

            // Parameters
            if (MergeUtils.TryGetMergedParameters(helpCopy.Parameters, fromCmdlet.Parameters, out var mergedParametersList, out var paramDiagnostics))
            {
                helpCopy.Parameters.Clear();
                helpCopy.Parameters.AddRange(mergedParametersList);
            }

            // We add the diagnostics even if there were no changes to the parameters.
            paramDiagnostics.ForEach(d => helpCopy.Diagnostics.TryAddDiagnostic(d));

            // Input
            if (MergeUtils.TryGetMergedInputOutputs(fromMarkdown.Inputs, fromCmdlet.Inputs, out var mergedInputs, out var inputDiagnostics))
            {
                helpCopy.Inputs.Clear();
                helpCopy.Inputs.AddRange(mergedInputs);
            }

            inputDiagnostics.ForEach(d => helpCopy.Diagnostics.TryAddDiagnostic(d));

            // Output
            if (MergeUtils.TryGetMergedInputOutputs(fromMarkdown.Outputs, fromCmdlet.Outputs, out var mergedOutputs, out var outputDiagnostics))
            {
                helpCopy.Outputs.Clear();
                helpCopy.Outputs.AddRange(mergedOutputs);
            }

            outputDiagnostics.ForEach(d => helpCopy.Diagnostics.TryAddDiagnostic(d));

            // Update the md.date metadata since we've just created this command help.
            if (helpCopy.Metadata is not null)
            {
                helpCopy.Metadata["ms.date"] = DateTime.Now.ToString("MM/dd/yyyy");
            }

            // We should have the best of both worlds now.
            return helpCopy;
        }

        /// <summary>
        /// Merge the syntaxes from the help and the command.
        /// In this case, we'll just replace the syntax for the parameter set we can identify.
        /// </summary>
        /// <param name="fromHelp"></param>
        /// <param name="fromCommand"></param>
        /// <param name="mergedSyntax"></param>
        /// <param name="diagnosticMessages"></param>
        /// <returns></returns>
        internal static bool  TryGetMergedSyntax(List<SyntaxItem>fromHelp, List<SyntaxItem>fromCommand, out List<SyntaxItem>mergedSyntax, out List<DiagnosticMessage>diagnosticMessages)
        {
            diagnosticMessages = new();
            mergedSyntax = new List<SyntaxItem>();
            // They're the same, just return false
            if (fromHelp.SequenceEqual(fromCommand))
            {
                diagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Merge, "Syntaxes are the same", DiagnosticSeverity.Information, "TryGetMergedSyntax", -1));
                return false;
            }

            // We believe the command as a source over the help.
            foreach (var syntax in fromHelp)
            {
                var cmdletSyntaxes = fromCommand.Where<SyntaxItem>(s => string.Compare(s.ParameterSetName, syntax.ParameterSetName) == 0);
                var cmdletSyntax = cmdletSyntaxes.Count() > 0 ?  cmdletSyntaxes.First() : null;
                if (cmdletSyntax is null)
                {
                    diagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Merge, $"Syntax for {syntax.ParameterSetName} is not found.", DiagnosticSeverity.Information, "TryGetMergedSyntax", -1));
                    mergedSyntax.Add(syntax);
                }
                else
                {
                    if (cmdletSyntax == syntax)
                    {
                        diagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Merge, $"Syntaxes for {syntax.ParameterSetName} are the same.", DiagnosticSeverity.Information, "TryGetMergedSyntax", -1));
                        mergedSyntax.Add(syntax);
                    }
                    else
                    {
                        diagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Merge, $"Updating syntax for {cmdletSyntax.ParameterSetName}.", DiagnosticSeverity.Information, "TryGetMergedSyntax", -1));
                        mergedSyntax.Add(cmdletSyntax);
                    }
                }
            }

            var missingList = fromCommand.Where(c => ! fromHelp.Any(h => string.Compare(c.ParameterSetName, h.ParameterSetName) == 0));
            if (missingList is not null)
            {
                foreach(var missing in missingList)
                {
                    diagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Merge, $"Adding missing syntax for {missing.ParameterSetName}", DiagnosticSeverity.Information, "TryGetMergedSyntax", -1));
                    mergedSyntax.Add(missing);
                }
            }

            return true;
        }

        /// <summary>
        /// Merge the parameters from the help and the command.
        /// </summary>
        /// <param name="fromHelp"></param>
        /// <param name="fromCommand"></param>
        /// <param name="mergedParameters"></param>
        /// <param name="diagnosticMessages"></param>
        /// <returns>boolean true if we creaetd the new parameters</returns>
        internal static bool TryGetMergedParameters(List<Parameter>fromHelp, List<Parameter>fromCommand, out List<Parameter>mergedParameters, out List<DiagnosticMessage>diagnosticMessages)
        {
            diagnosticMessages = new();
            mergedParameters = new();
            // They're the same, just return false
            if  (fromHelp.SequenceEqual(fromCommand))
            {
                diagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Merge, "Parameters are the same", DiagnosticSeverity.Information, "TryGetMergedParameters", -1));
                return false;
            }

            // Combine the names of the parameters from the help and the command.
            // we need to go through the union of these two lists to ensure we have all the parameters.
            // as a parameter may be added, but not documented in the help.
            // or parameters may be documented in the help, but not in the command (because it's dynamic).
            // We will sort the combined list of parameters by name.
            var parameterNames = new List<string>();
            parameterNames.AddRange(fromHelp.Select(p => p.Name));
            parameterNames.AddRange(fromCommand.Select(p => p.Name));
            parameterNames = parameterNames.Distinct().OrderBy(n => n).ToList();

            // dynamic parameters are currently unhandled on the command side.
            // They will still be copied if they are in the help.
            foreach(var pName in parameterNames)
            {
                // We should find 0 or 1 parameters that have the same name as the parameter in the cmdlet.
                var matchingCommandParameter = fromCommand.Where(x => string.Compare(x.Name, pName) == 0);
                var cmdParam = matchingCommandParameter.Count() > 0 ? matchingCommandParameter.First() : null;
                var foundParams = fromHelp.Where(x => string.Compare(x.Name, pName) == 0);
                var helpParam = foundParams.Count() > 0 ? foundParams.First() : null;

                // This should never happen, but if it does, we'll log it.
                if (helpParam is null && cmdParam is null)
                {
                    diagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Merge, $"No parameter found for {pName}.", DiagnosticSeverity.Information, "TryGetMergedParameters", -1));
                    continue;
                }

                // The help had the parameter, but the command did not.
                if (helpParam is not null && cmdParam is null)
                {
                    diagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Merge, $"adding {helpParam.Name}, parameter found in help.", DiagnosticSeverity.Information, "TryGetMergedParameters", -1));
                    mergedParameters.Add(helpParam);
                    continue;
                }

                // There is no help, but there is a command parameter.
                // Create a parameter from the command and add it to the list.
                if (helpParam is null && cmdParam is not null)
                {
                    diagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Merge, $"adding {pName}, not found in help.", DiagnosticSeverity.Information, "TryGetMergedParameters", -1));
                    var newParameter = new Parameter(cmdParam)
                    {
                        Description = "**FILL IN DESCRIPTION**"
                    };
                    mergedParameters.Add(cmdParam);
                    continue;
                }

                // The help is not null, but the command doesn't seem to have the parameter
                // This is the case for dynamic parameters.
                if (helpParam is not null && cmdParam is null)
                {
                    diagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Merge, $"adding {helpParam.Name}, parameter found in help but not in the command.", DiagnosticSeverity.Information, "TryGetMergedParameters", -1));
                    mergedParameters.Add(helpParam);
                    continue;
                }

                if (helpParam is not null && cmdParam is not null && helpParam == cmdParam)
                {
                    diagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Merge, $"No change to {cmdParam.Name}.", DiagnosticSeverity.Information, "TryGetMergedParameters", -1));
                    mergedParameters.Add(helpParam);
                    continue;
                }

                // The parameters exist in both the help and the command, but they are different.
                // use the text from the help, as there are more details there.
                if (helpParam is not null && cmdParam is not null && helpParam != cmdParam)
                {
                    var dm = new DiagnosticMessage(DiagnosticMessageSource.Merge, $"updating {pName}.", DiagnosticSeverity.Information, "TryGetMergedParameters", -1);
                    diagnosticMessages.Add(dm);

                    var checkTemplate = TransformUtils.GetParameterTemplateString(helpParam.Name);

                    var description = helpParam.Description;

                    if (string.Equals(helpParam.Description, checkTemplate, StringComparison.OrdinalIgnoreCase))
                    {
                        diagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Merge, $"Parameter {pName} has no description in the help.", DiagnosticSeverity.Warning, "TryGetMergedParameters", -1));
                        description = cmdParam.Description;
                    }
                    else if (!string.Equals(helpParam.Description, cmdParam.Description, StringComparison.OrdinalIgnoreCase))
                    {
                        diagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Merge, $"Parameter {pName} has the different description in the help and the command. Concatinating.", DiagnosticSeverity.Information, "TryGetMergedParameters", -1));
                        description = string.Join(Environment.NewLine, helpParam.Description, cmdParam.Description);
                    }

                    var newParameter = new Parameter(cmdParam)
                    {
                        Description = description,
                        DontShow = helpParam.DontShow,
                        DefaultValue = helpParam.DefaultValue
                    };

                    // Take the accepted values from the help
                    if (helpParam.AcceptedValues.Count > 0)
                    {
                        diagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Merge, $"Adding accepted values from help.", DiagnosticSeverity.Information, "TryGetMergedParameters", -1));
                        newParameter.AcceptedValues.Clear();
                        newParameter.AcceptedValues.AddRange(helpParam.AcceptedValues);
                    }

                    // Take SupportsWildcards from the help (unless UseHelpValueForWildcard is false)
                    // There are many examples where SupportsWildcards is not set by the attribue in the command, but is in the help.
                    // We don't want to just blindly set it to false, because the help may have it wrong.
                    // The help also might be wrong, but we'll take the help's word for it if it's true.
                    if (UseHelpValueForWildcard && helpParam.SupportsWildcards == true)
                    {
                        diagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Merge, $"Setting SupportsWildcard to true because it was in the help.", DiagnosticSeverity.Information, "TryGetMergedParameters", -1));
                        newParameter.SupportsWildcards = true;
                    }

                    mergedParameters.Add(newParameter);
                    continue;
                }

                diagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Merge, $"This should not happen: No action taken for {pName}.", DiagnosticSeverity.Error, "TryGetMergedParameters", -1));
            }

            return true;
        }

        internal static bool TryGetMergedInputOutputs(List<InputOutput>fromHelp, List<InputOutput>fromCommand, out List<InputOutput>mergedInputOutput, out List<DiagnosticMessage>diagnostics)
        {
            diagnostics = new();
            mergedInputOutput = new(fromCommand);
            if (fromHelp.SequenceEqual(fromCommand))
            {
                diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.Merge, "Input/Output are the same", DiagnosticSeverity.Information, "TryGetMergedInputOutput", -1));
                return false;
            }

            var newList = fromCommand.Where(c => ! fromHelp.Any(h => string.Compare(c.Typename, h.Typename, StringComparison.CurrentCultureIgnoreCase) == 0));
            if (newList.Count() == 0)
            {
                diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.Merge, "Input/Output names are the same", DiagnosticSeverity.Information, "TryGetMergedInputOutput", -1));
                return false;
            }

            mergedInputOutput = new(fromHelp);
            foreach(var newIO in newList)
            {
                diagnostics.Add(new DiagnosticMessage(DiagnosticMessageSource.Merge, $"Adding {newIO.Typename}", DiagnosticSeverity.Information, "TryGetMergedInputOutput", -1));
                mergedInputOutput.Add(newIO);
            }

            return true;
        }
    }
}