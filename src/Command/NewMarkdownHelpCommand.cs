// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.PowerShell.Commands;
using Microsoft.PowerShell.PlatyPS.MarkdownWriter;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    /// <summary>
    /// Cmdlet to generate the markdown help for commands, all commands in a module.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "MarkdownCommandHelp", HelpUri = "")]
    [OutputType(typeof(FileInfo[]))]
    public sealed class NewMarkdownHelpCommand : PSCmdlet
    {
        #region cmdlet parameters

        [Parameter(ValueFromPipeline = true)]
        [StringToCommandInfoTransformation]
        public CommandInfo[] Command { get; set; } = Array.Empty<CommandInfo>();

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

        [Parameter(ValueFromPipeline = true)]
        [StringToPsModuleInfoTransformation]
        public PSModuleInfo[] Module { get; set; } = Array.Empty<PSModuleInfo>();

        [Parameter(Mandatory = true)]
        public string OutputFolder { get; set; } = Environment.CurrentDirectory;

        [Parameter]
        public SwitchParameter WithModulePage { get; set; }

        [Parameter]
        public SwitchParameter AbbreviateParameterTypename { get; set; }

        public PSSession? Session { get; set; }

        #endregion

        List<CommandInfo> cmdCollection = new();
        private string outputFolderBase = string.Empty;

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
            if (Command.Length > 0)
            {
                cmdCollection.AddRange(Command);
            }

            if (Module.Length > 0)
            {
                foreach (var mod in Module)
                {
                    cmdCollection.AddRange(mod.ExportedCommands.Values.Where<CommandInfo>(c => c.CommandType != CommandTypes.Alias));
                }
            }
        }

        // now that the commands are all gathered, transform them into command help objects
        protected override void EndProcessing()
        {
            if (!ShouldProcess(OutputFolder))
            {
                return;
            }

            List<CommandHelp> cmdHelpObjs = new List<CommandHelp>();
            Dictionary<string, PSModuleInfo> moduleTable = new();
            foreach (var providedModule in Module)
            {
                moduleTable[providedModule.Name] = providedModule;
            }

            if (cmdCollection.Count > 0)
            {
                int currentOffset = 0;
                foreach(var cmd in cmdCollection)
                {
                    if (cmd.Module is not null && ! string.IsNullOrEmpty(cmd.Module.Name) && ! moduleTable.ContainsKey(cmd.Module.Name))
                    {
                        if (HelpInfoUri == string.Empty && ! string.IsNullOrEmpty(cmd.Module.HelpInfoUri))
                        {
                            HelpInfoUri = cmd.Module.HelpInfoUri;
                        }
                    }


                    TransformSettings transformSettings = new TransformSettings
                    {
                        CreateModulePage = WithModulePage,
                        DoubleDashList = false,
                        ExcludeDontShow = false,
                        FwLink = HelpInfoUri,
                        HelpVersion = HelpVersion.ToString(),
                        Locale = Locale is null ? CultureInfo.CurrentUICulture : new CultureInfo(Locale),
                        ModuleName = cmd.ModuleName is null ? string.Empty : cmd.ModuleName,
                        ModuleGuid = cmd.Module?.Guid is null ? Guid.Empty : cmd.Module.Guid,
                        OnlineVersionUrl = HelpUri,
                        Session = Session,
                        UseFullTypeName = ! AbbreviateParameterTypename
                    };

                    try
                    {
                        var pr = new ProgressRecord(0, "Transforming cmdlet", $"{cmd.ModuleName}\\{cmd.Name}");
                        pr.PercentComplete = (int)Math.Round(((double)currentOffset++ / (double)(cmdCollection.Count)) * 100);
                        WriteProgress(pr);
                        var transformedCommand = new TransformCommand(transformSettings).Transform(cmd);

                        if (transformedCommand is null)
                        {
                            throw new InvalidDataException("Command transformation failed");
                        }

                        if (transformedCommand.Metadata is null)
                        {
                            transformedCommand.Metadata = new();
                        }

                        // update the HelpUri if the parameter was used.
                        if (! string.IsNullOrEmpty(HelpUri))
                        {
                            transformedCommand.Metadata["HelpUri"] = HelpUri;
                        }

                        cmdHelpObjs.Add(transformedCommand);
                    }
                    catch (Exception e)
                    {
                        WriteError(new ErrorRecord(e, "NewMarkdownCommandHelp,TransformError", ErrorCategory.InvalidData, cmd));
                    }
                }
            }

            // Group the commands by Module Name
            var commandGroup = cmdHelpObjs.GroupBy(c => c.ModuleName);
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

                // Prefer psModuleInfo to create the moduleFileInfo.
                if (moduleTable.TryGetValue(moduleName, out var psModuleInfo))
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

                if (! string.IsNullOrEmpty(HelpInfoUri))
                {
                    moduleFileInfo.Metadata["HelpInfoUri"] = HelpInfoUri;
                }

                List<ModuleCommandInfo> mciList = new();
                foreach(var cmdHelp in group)
                {
                    moduleName = cmdHelp.ModuleName;
                    moduleFolder = Path.Combine(outputFolderBase, moduleName);
                    string synopsis = cmdHelp.Synopsis;
                    var mci = new ModuleCommandInfo()
                    {
                        Name = cmdHelp.Title,
                        Link = $"{cmdHelp.Title}.md",
                        Description = string.IsNullOrEmpty(synopsis) ? Constants.FillInDescription : synopsis
                    };
                    mciList.Add(mci);

                    if (! Directory.Exists(moduleFolder))
                    {
                        Directory.CreateDirectory(moduleFolder);
                    }

                    var helpFilePath = Path.Combine(moduleFolder, $"{cmdHelp.Title}.md");
                    if (new FileInfo(helpFilePath).Exists && ! Force)
                    {
                        WriteWarning(string.Format(Constants.skippingMessageFmt, helpFilePath));
                        continue;
                    }

                    var settings = new WriterSettings(Encoding, helpFilePath);
                    using var cmdWrt = new CommandHelpMarkdownWriter(settings);
                    WriteObject(this.InvokeProvider.Item.Get(cmdWrt.Write(cmdHelp, Metadata).FullName));
                }

                if (WithModulePage)
                {
                    ModuleCommandGroup mcg = new(moduleName);
                    mcg.Commands.AddRange(mciList);
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
        }
    }
}