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
    /// Cmdlet to generate the markdown help for commands, all commands in a module.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "MarkdownModuleFile", HelpUri = "")]
    [OutputType(typeof(FileInfo[]))]
    public sealed class NewMarkdownModuleFileCommand : PSCmdlet
    {
        #region cmdlet parameters

        [Parameter(ValueFromPipeline = true)]
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

            // Group the commands by Module Name
            var commandGroup = allCommandHelp.GroupBy(c => c.ModuleName);
            foreach(var group in commandGroup)
            {
                if (group.Count() < 1)
                {
                    continue;
                }

                string moduleName = group.First<CommandHelp>().ModuleName;
                string moduleFolder = Path.Combine(outputFolderBase, moduleName);
                CultureInfo locale = group.First<CommandHelp>().Locale;
                ModuleFileInfo moduleFileInfo;

                var psModuleInfo = GetModuleInfo(moduleName);
                // Prefer psModuleInfo to create the moduleFileInfo.
                if (psModuleInfo is not null)
                {
                    moduleFileInfo = new ModuleFileInfo(psModuleInfo, locale);
                    if (string.IsNullOrEmpty(moduleFileInfo.Description.Trim()))
                    {
                        moduleFileInfo.Description = Constants.FillInDescription;
                    }
                }
                else
                {
                    moduleFileInfo = new($"{moduleName} Module", moduleName, group.First<CommandHelp>().Locale)
                    {
                        Description = "{{ Add module description here. }}"
                    };

                    if (group.First<CommandHelp>().ModuleGuid is not null)
                    {
                        moduleFileInfo.Metadata["Module Guid"] = group.First<CommandHelp>().ModuleGuid.ToString();
                    }
                }

                // Now add the commands to the command group
                ModuleCommandGroup mcg = new(moduleName);
                foreach(var cmdHelp in group)
                {
                    var moduleCommandInfo = new ModuleCommandInfo()
                    {
                        Name = cmdHelp.Title,
                        Link = $"{cmdHelp.Title}.md",
                        Description = Constants.FillInDescription,
                    };

                    mcg.Commands.Add(moduleCommandInfo);
                }
                moduleFileInfo.CommandGroups.Add(mcg);

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
                    moduleFileInfo.Metadata["Help Version"] = HelpVersion.ToString();
                    WriteObject(this.InvokeProvider.Item.Get(modulePageWriter.Write(moduleFileInfo).FullName));
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