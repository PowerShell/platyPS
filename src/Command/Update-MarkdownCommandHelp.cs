// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
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

        CommandHelp Merge(CommandHelp fromMarkdown, CommandHelp fromCmdlet)
        {
            return fromMarkdown;
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