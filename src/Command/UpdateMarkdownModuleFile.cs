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

        [Parameter(Mandatory = true, Position = 0)]
        public ModuleFileInfo ModuleFileInfo { get; set; } = new();

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

        [Parameter(Mandatory = true)]
        public string OutputFolder { get; set; } = Environment.CurrentDirectory;

        #endregion

        private string outputFolderBase = string.Empty;
        private List<CommandHelp> allCommandHelp = new();

        ModuleFileInfo? newModuleFile;
        protected override void BeginProcessing()
        {
            outputFolderBase = this.SessionState.Path.GetUnresolvedProviderPathFromPSPath(OutputFolder);
            if (File.Exists(outputFolderBase))
            {
                var exception = new InvalidOperationException(string.Format(Microsoft_PowerShell_PlatyPS_Resources.PathIsNotFolder, outputFolderBase));
                ErrorRecord err = new ErrorRecord(exception, "PathIsNotFolder", ErrorCategory.InvalidOperation, outputFolderBase);
                ThrowTerminatingError(err);
            }

            if (!Directory.Exists(outputFolderBase))
            {
                Directory.CreateDirectory(outputFolderBase);
            }
        }

        // Gather up all of the commands from modules or commands
        protected override void ProcessRecord()
        {
            allCommandHelp.AddRange(CommandHelp);
        }

        // now that the commands are all gathered, transform them into command help objects
        protected override void EndProcessing()
        {
            if (!ShouldProcess(OutputFolder))
            {
                return;
            }

            // Check to be sure that all of the command help objects are in all in the same module.
            var wrongModules = allCommandHelp.Where<CommandHelp>(ch => string.Compare(ch.ModuleName, ModuleFileInfo.Module) != 0);
            if (wrongModules.Count() != 0)
            {
                ThrowTerminatingError(
                    new ErrorRecord(
                        new InvalidOperationException("All help must be in a single module"),
                        "UpdateMarkdownModuleFile,InvalidOperation",
                        ErrorCategory.InvalidOperation,
                        wrongModules.ToArray()
                    )
                );
            }


            // work on a copy of the original
            newModuleFile = new ModuleFileInfo(ModuleFileInfo);
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
            string moduleFolder = Path.Combine(outputFolderBase, moduleName);
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
                string moduleFilePath = Path.Combine(moduleFolder, $"{moduleName}.md");
                var modulePageSettings = new WriterSettings(Encoding, moduleFilePath);
                using var modulePageWriter = new ModulePageWriter(modulePageSettings);
                if (new FileInfo(moduleFilePath).Exists && ! Force)
                {
                    WriteWarning(string.Format(Constants.skippingMessageFmt, moduleFilePath));
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