// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Automation;
using Microsoft.PowerShell.PlatyPS.MarkdownWriter;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    /// <summary>
    /// Cmdlet to update the markdown module file.
    /// </summary>
    [Cmdlet(VerbsData.Update, "MarkdownModuleFile", SupportsShouldProcess = true, HelpUri = "")]
    [OutputType(typeof(FileInfo[]))]
    public sealed class UpdateMarkdownModuleFileCommand : PSCmdlet
    {
        #region cmdlet parameters

        [Parameter(Mandatory = true, ParameterSetName = "path", Position = 0)]
        public string Path { get; set; } = string.Empty;

        [Parameter(Mandatory = true, ParameterSetName = "literalpath")]
        public string LiteralPath { get; set; } = string.Empty;

        [Parameter(ValueFromPipeline = true, Mandatory = true, Position = 1)]
        public CommandHelp[] CommandHelp { get; set; } = Array.Empty<CommandHelp>();

        [Parameter()]
        [ArgumentToEncodingTransformation]
        [ArgumentCompleter(typeof(EncodingCompleter))]
        public System.Text.Encoding Encoding { get; set; } = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        [Parameter()]
        public SwitchParameter Force { get; set; }

        [Parameter]
        [ValidateNotNullOrEmpty]
        public string HelpUri { get; set; } = string.Empty;

        [Parameter]
        [ValidateNotNullOrEmpty]
        public string HelpInfoUri { get; set; } = string.Empty;

        [Parameter]
        [ValidateNotNullOrEmpty]
        public Version HelpVersion { get; set; } = new Version(1, 0, 0, 0);

        [Parameter]
        public string? Locale { get; set; }

        [Parameter()]
        public Hashtable? Metadata { get; set; }

        [Parameter()]
        public SwitchParameter NoBackup { get; set; }

        #endregion

        private string resolvedModuleFilePath { get; set; } = string.Empty;
        private List<CommandHelp> allCommandHelp = new();

        protected override void BeginProcessing()
        {
            bool isLiteralPath = ParameterSetName == "literalpath" ? true : false;
            string[] modulefilepath = isLiteralPath ? new string[] { LiteralPath } : new string[] { Path };
            var resolvedPaths = PathUtils.ResolvePath(this, modulefilepath, isLiteralPath);
            if (resolvedPaths.Count != 1)
            {
                ThrowTerminatingError(
                    new ErrorRecord(
                        new InvalidOperationException("Single path required"),
                        "UpdateMarkdownModuleFile,ModuleFileError",
                        ErrorCategory.InvalidArgument, resolvedPaths)
                );
            }

            resolvedModuleFilePath = resolvedPaths.First<string>();
        }

        // Gather up all of the commands from modules or commands
        protected override void ProcessRecord()
        {
            allCommandHelp.AddRange(CommandHelp);
        }

        // now that the commands are all gathered, transform them into command help objects
        protected override void EndProcessing()
        {
            var identity = MarkdownProbe.Identify(resolvedModuleFilePath);
            if (! identity.IsModuleFile())
            {
                ThrowTerminatingError(
                    new ErrorRecord(
                        new ArgumentException($"{resolvedModuleFilePath}"),
                        "UpdateMarkdownModuleFile,InvalidModuleFile",
                        ErrorCategory.InvalidData,
                        identity)
                );
            }

            var mf = MarkdownConverter.GetModuleFileInfoFromMarkdownFile(resolvedModuleFilePath);

            // Check to be sure that all of the command help objects are in all in the same module.
            var wrongModuleCommands = allCommandHelp.Where<CommandHelp>(ch => string.Compare(ch.ModuleName, mf.Module) != 0);
            if (wrongModuleCommands.Count() != 0)
            {
                ThrowTerminatingError(
                    new ErrorRecord(
                        new InvalidOperationException($"All help must be in the '{mf.Module}' module"),
                        "UpdateMarkdownModuleFile,InvalidOperation",
                        ErrorCategory.InvalidOperation,
                        wrongModuleCommands.ToArray()
                    )
                );
            }

            // work on a copy of the original
            var newModuleFile = new ModuleFileInfo(mf);
            // Remove all of the command groups, we only use what was provided by the user.
            newModuleFile.CommandGroups.Clear();
            var moduleName = newModuleFile.Module;

            if (Metadata?.Keys.Count > 0)
            {
                // Check for non-overridable keys in the provided Metadata
                if (Metadata.Keys.Count > 0)
                {
                    var badKeys = MetadataUtils.WarnBadKeys(this, Metadata);
                    badKeys.ForEach(k => Metadata.Remove(k));
                }

                MetadataUtils.MergeNewModulefileMetadata(Metadata, newModuleFile);
            }

            newModuleFile.Metadata["ms.date"] = DateTime.Now.ToString("MM/dd/yyyy");
            CultureInfo locale = string.IsNullOrEmpty(Locale) ? newModuleFile.Locale : CultureInfo.GetCultureInfo(Locale);

            // Group the commands by Module Name
            var commandGroup = allCommandHelp.OrderBy(help => help.Title).GroupBy(help => help.ModuleName);
            foreach(var group in commandGroup)
            {
                ModuleCommandGroup mcg = new($"{moduleName} Cmdlets");
                if (group.Count() < 1)
                {
                    newModuleFile.CommandGroups.Add(mcg);
                    continue;
                }

                // Now add the commands to the command group
                // and then add the command group to the module file.
                foreach(var cmdHelp in group)
                {
                    var moduleCommandInfo = new ModuleCommandInfo()
                    {
                        Name = cmdHelp.Title,
                        Link = $"{cmdHelp.Title}.md", // JWT - should this be a property of the command help object?
                        Description = cmdHelp.Synopsis,
                    };

                    mcg.Commands.Add(moduleCommandInfo);
                }

                newModuleFile.CommandGroups.Add(mcg);

                // ready the module file writer
                var modulePageSettings = new WriterSettings(Encoding, resolvedModuleFilePath);
                using var modulePageWriter = new ModulePageWriter(modulePageSettings);
                if (! NoBackup)
                {
                    var backupPath = $"{resolvedModuleFilePath}.bak";
                    if (File.Exists(backupPath))
                    {
                        if (! Force)
                        {
                            ThrowTerminatingError(
                                new ErrorRecord(
                                    new InvalidOperationException($"Backup file '{backupPath}' exists, use -Force to overwrite."),
                                    "BackupExists",
                                    ErrorCategory.InvalidOperation,
                                    backupPath
                                )
                            );
                        }

                        File.Delete(backupPath);
                    }

                    File.Move(resolvedModuleFilePath, backupPath);
                }

                if (new FileInfo(resolvedModuleFilePath).Exists && ! Force)
                {
                    ThrowTerminatingError(
                        new ErrorRecord(
                            new InvalidOperationException($"Module file '{resolvedModuleFilePath}' exists, use -Force to overwrite."),
                            "ModuleFileExists",
                            ErrorCategory.InvalidOperation,
                            resolvedModuleFilePath
                        )
                    );
                }
                else
                {
                    // Update the help version in the module metadata
                    WriteObject(this.InvokeProvider.Item.Get(modulePageWriter.Write(newModuleFile).FullName));
                }
            }
        }

        PSModuleInfo? GetModuleInfo(string moduleName)
        {
            using (var powershell = System.Management.Automation.PowerShell.Create(RunspaceMode.CurrentRunspace))
            {
                // try to just retrieve the module
                var result = powershell.AddCommand("Get-Module").AddParameter("Name", moduleName).Invoke<PSObject>();
                if ((result is null || powershell.HadErrors) || (result is not null && result.Count == 0))
                {
                    // The module is not loaded, so try to load it.
                    powershell.Commands.Clear();
                    result = powershell.AddCommand("Import-Module").AddParameter("Name", moduleName).AddParameter("PassThru", true).Invoke<PSObject>();
                }

                if (result is not null && result.Count == 1)
                {
                    if (result[0].BaseObject is PSModuleInfo psModuleInfo)
                    {
                        return psModuleInfo;
                    }
                }
            }

            return null;
        }
    }
}