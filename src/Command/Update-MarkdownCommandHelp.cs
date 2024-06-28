// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
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
    [Cmdlet(VerbsData.Update, "MarkdownCommandHelp", DefaultParameterSetName = "Path", SupportsShouldProcess = true, HelpUri = "")]
    [OutputType(typeof(CommandHelp))]
    public sealed class UpdateMarkdownHelpCommand : PSCmdlet
    {
#region cmdlet parameters

        [Parameter(Mandatory=true, Position=0, ValueFromPipeline=true, ParameterSetName= "Path")]
        [ValidateNotNullOrEmpty]
        [SupportsWildcards]
        public string[] Path { get; set; } = Array.Empty<string>();

        [Parameter(Mandatory=true, ValueFromPipeline=true, ParameterSetName= "LiteralPath")]
        [ValidateNotNullOrEmpty]
        public string[] LiteralPath { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Do not create a backup of the file.
        /// </summary>
        [Parameter]
        public SwitchParameter NoBackup { get; set; }

        /// <summary>
        /// Return the newly created CommandHelp object.
        /// </summary>
        [Parameter]
        public SwitchParameter PassThru { get; set; }

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
                    var cmdInfo = SessionState.InvokeCommand.GetCmdlet(commandName);
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
                    if (ShouldProcess(path))
                    {
                        FileInfo fi = new FileInfo(path);
                        Encoding encoding = GetFileEncoding(path);
                        if (! NoBackup)
                        {
                            fi.MoveTo($"{path}.bak");
                        }
                        else
                        {
                            fi.Delete();
                        }

                        var settings = new WriterSettings(encoding, path);
                        var cmdWrt = new CommandHelpMarkdownWriter(settings);
                        cmdWrt.Write(mergedCommandHelp, null);

                        if (PassThru)
                        {
                            WriteObject(mergedCommandHelp);
                        }
                    }
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
            CommandHelp template = new (fromMarkdown);
            // Syntax
            if (TryGetMergedSyntax(template.Syntax, fromCmdlet.Syntax, out var mergedSyntaxList, out var mergeSyntaxDiagnosticMessages))
            {
                foreach(var msg in mergeSyntaxDiagnosticMessages)
                {
                    template.Diagnostics.TryAddDiagnostic(msg);
                }

                template.Syntax.Clear();
                template.Syntax.AddRange(mergedSyntaxList);
            }

            // Parameters
            if (TryGetMergedParameters(template.Parameters, fromCmdlet.Parameters, out var mergedParametersList, out var mergeParameterDiagnosticMessages))
            {
                foreach(var msg in mergeParameterDiagnosticMessages)
                {
                    template.Diagnostics.TryAddDiagnostic(msg);
                }

                template.Parameters.Clear();
                template.Parameters.AddRange(mergedParametersList);
            }

            // TODO:
            // INPUT
            // OUTPUT

            return template;
        }

        private bool  TryGetMergedSyntax(List<SyntaxItem>fromHelp, List<SyntaxItem>fromCommand, out List<SyntaxItem>mergedSyntax, out List<DiagnosticMessage>diagnosticMessages)
        {
            diagnosticMessages = new();
            mergedSyntax = new List<SyntaxItem>();
            // They're the same, just return false
            if (fromHelp.SequenceEqual(fromCommand))
            {
                return false;
            }

            // We believe the command as a source over the help.
            mergedSyntax = fromCommand;
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

            // TODO: dynamic parameters
            foreach(var param in fromCommand)
            {
                var helpParam = fromHelp.Where(x => string.Compare(x.Name, param.Name) == 0).First<Parameter>();
                if (helpParam is null)
                {
                    diagnosticMessages.Add(new DiagnosticMessage(DiagnosticMessageSource.Merge, $"updating {param.Name}, not found in help.", DiagnosticSeverity.Information, "TryGetMergedParameters", -1));
                    var newParameter = new Parameter(param);
                    param.Description = "**FILL IN DESCRIPTION**";
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
                    var newParameter = new Parameter(param);
                    param.Description = helpParam.Description;
                    mergedParameters.Add(newParameter);
                }
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