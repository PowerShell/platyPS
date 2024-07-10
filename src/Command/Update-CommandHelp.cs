// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using Microsoft.PowerShell.PlatyPS.MarkdownWriter;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    /// <summary>
    /// Cmdlet to import a markdown file and merge it with the session cmdlet of the same name.
    /// </summary>
    [Cmdlet(VerbsData.Update, "CommandHelp", DefaultParameterSetName = "Path", SupportsShouldProcess = true, HelpUri = "")]
    [OutputType(typeof(CommandHelp))]
    public sealed class UpdateHelpCommand : PSCmdlet
    {
#region cmdlet parameters

        [Parameter(Mandatory=true, Position=0, ValueFromPipeline=true, ParameterSetName= "Path")]
        [ValidateNotNullOrEmpty]
        [SupportsWildcards]
        public string[] Path { get; set; } = Array.Empty<string>();

        [Parameter(Mandatory=true, ValueFromPipeline=true, ParameterSetName= "LiteralPath")]
        [ValidateNotNullOrEmpty]
        public string[] LiteralPath { get; set; } = Array.Empty<string>();

#endregion

        private TransformSettings transformSettings = new TransformSettings() {
                CreateModulePage = false,
                DoubleDashList = false,
                ExcludeDontShow = true,
                HelpVersion = "3.0.0",
                Locale = CultureInfo.GetCultureInfo("en-US"),
                UseFullTypeName = true
            };

        protected override void ProcessRecord()
        {
            List<string> resolvedPaths;
            try
            {
                // This is a list because the resolution process can result in multiple paths (in the case of non-literal path).
                resolvedPaths = PathUtils.ResolvePath(this, ParameterSetName == "LiteralPath" ? LiteralPath : Path, ParameterSetName == "LiteralPath" ? true : false);
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(e, "Could not resolve Path", ErrorCategory.InvalidOperation, ParameterSetName == "LiteralPath" ? LiteralPath : Path));
                return;
            }

            // These should be resolved paths, whether -LiteralPath was used or not.
            foreach (string path in resolvedPaths)
            {
                try
                {
                    var commandHelpObject = MarkdownConverter.GetCommandHelpFromMarkdownFile(path);
                    var commandName = commandHelpObject.Title;
                    var cmdInfo = SessionState.InvokeCommand.GetCommand(commandName, CommandTypes.Function|CommandTypes.Filter|CommandTypes.Cmdlet);
                    if (cmdInfo is null)
                    {
                        var err = new ErrorRecord(new CommandNotFoundException(commandName), "UpdateMarkdownCommandHelp,CommandNotFound", ErrorCategory.ObjectNotFound, commandName); 
                        err.ErrorDetails = new($"Did you import the module which includes '{commandName}'?");
                        WriteError(err);
                        continue;
                    }

                    var helpObjectFromCmdlet = new TransformCommand(transformSettings).Transform(cmdInfo);
                    if (helpObjectFromCmdlet is null)
                    {
                        var err = new ErrorRecord(new InvalidOperationException(commandName), "UpdateMarkdownCommandHelp,CmdletConversion", ErrorCategory.InvalidResult, commandName); 
                        err.ErrorDetails = new($"Could not convert {commandName} to CommandHelp.");
                        WriteError(err);
                        continue;
                    }

                    var mergedCommandHelp = Merge(commandHelpObject, helpObjectFromCmdlet);
                    WriteObject(mergedCommandHelp);
                }
                catch (Exception e)
                {
                    WriteError(new ErrorRecord(e, "FailedToImportMarkdown", ErrorCategory.InvalidOperation, path));
                }
            }
        }

        /// <summary>
        /// Merge the new data found in the cmdlet object.
        /// </summary>
        /// <param name="fromMarkdown"></param>
        /// <param name="fromCmdlet"></param>
        /// <returns></returns>
        CommandHelp Merge(CommandHelp fromMarkdown, CommandHelp fromCmdlet)
        {
            CommandHelp helpCopy = new (fromMarkdown);
            // Syntax
            if (TryGetMergedSyntax(helpCopy.Syntax, fromCmdlet.Syntax, out var mergedSyntaxList, out var syntaxDiagnostics))
            {
                helpCopy.Syntax.Clear();
                helpCopy.Syntax.AddRange(mergedSyntaxList);
            }
            syntaxDiagnostics.ForEach(d => helpCopy.Diagnostics.TryAddDiagnostic(d));

            // Parameters
            if (TryGetMergedParameters(helpCopy.Parameters, fromCmdlet.Parameters, out var mergedParametersList, out var paramDiagnostics))
            {
                helpCopy.Parameters.Clear();
                helpCopy.Parameters.AddRange(mergedParametersList);
            }
            paramDiagnostics.ForEach(d => helpCopy.Diagnostics.TryAddDiagnostic(d));

            // Input
            if (TryGetMergedInputOutputs(fromMarkdown.Inputs, fromCmdlet.Inputs, out var mergedInputs, out var inputDiagnostics))
            {
                helpCopy.Inputs.Clear();
                helpCopy.Inputs.AddRange(mergedInputs);
            }
            inputDiagnostics.ForEach(d => helpCopy.Diagnostics.TryAddDiagnostic(d));

            // Output
            if (TryGetMergedInputOutputs(fromMarkdown.Outputs, fromCmdlet.Outputs, out var mergedOutputs, out var outputDiagnostics))
            {
                helpCopy.Outputs.Clear();
                helpCopy.Outputs.AddRange(mergedOutputs);
            }
            outputDiagnostics.ForEach(d => helpCopy.Diagnostics.TryAddDiagnostic(d));

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
        private bool  TryGetMergedSyntax(List<SyntaxItem>fromHelp, List<SyntaxItem>fromCommand, out List<SyntaxItem>mergedSyntax, out List<DiagnosticMessage>diagnosticMessages)
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

        private bool TryGetMergedParameters(List<Parameter>fromHelp, List<Parameter>fromCommand, out List<Parameter>mergedParameters, out List<DiagnosticMessage>diagnosticMessages)
        {
            diagnosticMessages = new();
            mergedParameters = new();
            // They're the same, just return false
            if  (fromHelp.SequenceEqual(fromCommand))
            {
                diagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Merge, "Parameters are the same", DiagnosticSeverity.Information, "TryGetMergedParameters", -1));
                return false;
            }

            // dynamic parameters are currently unhandled on the command side.
            // They will still be copied if they are in the help.
            foreach(var param in fromCommand)
            {
                // We should find 0 or 1 parameters that have the same name as the parameter in the cmdlet.
                var foundParams = fromHelp.Where(x => string.Compare(x.Name, param.Name) == 0);
                var helpParam = foundParams.Count() > 0 ? foundParams.First() : null;
                if (helpParam is null)
                {
                    diagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Merge, $"updating {param.Name}, not found in help.", DiagnosticSeverity.Information, "TryGetMergedParameters", -1));
                    var newParameter = new Parameter(param)
                    {
                        Description = "**FILL IN DESCRIPTION**"
                    };
                    mergedParameters.Add(param);
                }
                else if (helpParam == param)
                {
                    diagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Merge, $"No change to {param.Name}.", DiagnosticSeverity.Information, "TryGetMergedParameters", -1));
                    mergedParameters.Add(helpParam);
                }
                else
                {
                    diagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Merge, $"updating {param.Name}.", DiagnosticSeverity.Information, "TryGetMergedParameters", -1));
                    var newParameter = new Parameter(param)
                    {
                        Description = helpParam.Description
                    };
                    mergedParameters.Add(newParameter);
                }
            }


            // Be sure that any parameters documented (which may not be in the command) are included.
            var missingList = fromHelp.Where(c => ! fromCommand.Any(h => string.Compare(c.Name, h.Name) == 0));
            foreach (var param in missingList)
            {
                diagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Merge, $"adding {param.Name}, parameter found in help.", DiagnosticSeverity.Information, "TryGetMergedParameters", -1));
                mergedParameters.Add(param);
            }

            return true;
        }

        private bool TryGetMergedInputOutputs(List<InputOutput>fromHelp, List<InputOutput>fromCommand, out List<InputOutput>mergedInputOutput, out List<DiagnosticMessage>diagnostics)
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

        Encoding GetFileEncoding(string filePath)
        {
            Encoding encoding;
            using (var reader = new StreamReader(filePath, Encoding.UTF8, true))
            {
                reader.Peek();
                encoding = reader.CurrentEncoding;
            }
            return encoding;
        }
    }

}