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
using System.Runtime.Serialization;
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
        [ArgumentEncodingCompletions]
        public System.Text.Encoding Encoding { get; set; } = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        [Parameter()]
        public SwitchParameter Force { get; set; }

        [Parameter]
        public string HelpUri { get; set; } = string.Empty;

        [Parameter]
        public string HelpInfoUri { get; set; } = string.Empty;

        [Parameter]
        public string HelpVersion { get; set; } = string.Empty;

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
        public SwitchParameter AlphabeticParamsOrder { get; set; } = true;

        [Parameter]
        public SwitchParameter UseFullTypeName { get; set; }

        public PSSession? Session { get; set; }

        #endregion

        List<CommandInfo> cmdCollection = new();
        private string outputFolderBase = string.Empty;
        private Dictionary<string, Guid> moduleGuids = new();

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
                foreach(var cmd in Command)
                {
                    if (cmd?.Module?.Guid is not null)
                    {
                        moduleGuids[cmd.ModuleName] = cmd.Module.Guid;
                    }
                }
                cmdCollection.AddRange(Command);
            }

            if (Module.Length > 0)
            {
                foreach (var mod in Module)
                {
                    moduleGuids[mod.Name] = mod.Guid;
                    cmdCollection.AddRange(mod.ExportedCommands.Values.Where<CommandInfo>(c => c.CommandType != CommandTypes.Alias));
                }
            }
        }

        // now that the commands are all gathered, transform them into command help objects
        protected override void EndProcessing()
        {
            List<CommandHelp> cmdHelpObjs = new List<CommandHelp>();

            if (cmdCollection.Count > 0)
            {
                int currentOffset = 0;
                foreach(var cmd in cmdCollection)
                {

                    TransformSettings transformSettings = new TransformSettings
                    {
                        AlphabeticParamsOrder = AlphabeticParamsOrder,
                        CreateModulePage = WithModulePage,
                        DoubleDashList = false,
                        ExcludeDontShow = false,
                        FwLink = HelpInfoUri,
                        HelpVersion = HelpVersion,
                        Locale = Locale is null ? CultureInfo.GetCultureInfo("en-US") : new CultureInfo(Locale),
                        ModuleGuid = cmd.Module?.Guid is null ? null : cmd.Module.Guid,
                        ModuleName = cmd.ModuleName is null ? string.Empty : cmd.ModuleName,
                        OnlineVersionUrl = HelpUri,
                        Session = Session,
                        UseFullTypeName = UseFullTypeName
                    };

                    try
                    {
                        if (cmdCollection.Count > 10)
                        {
                            var pr = new ProgressRecord(0, "Scanning cmdlet", $"{cmd.ModuleName}\\{cmd.Name}");
                            pr.PercentComplete = (int)Math.Round(((double)currentOffset++ / (double)(cmdCollection.Count)) * 100);
                            WriteProgress(pr);
                        }
                        cmdHelpObjs.Add(new TransformCommand(transformSettings).Transform(cmd));
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
                string moduleTitle = $"{moduleName} Module";
                string moduleFolder = Path.Combine(outputFolderBase, moduleName);
                ModuleFileInfo moduleFileInfo = new(moduleTitle, moduleName, group.First<CommandHelp>().Locale);
                if (moduleFileInfo.Description == string.Empty)
                {
                    moduleFileInfo.Description = "{{ Add module description here. }}";
                }

                foreach(var cmdHelp in group)
                {

                    moduleFolder = Path.Combine(outputFolderBase, moduleName);
                    var mci = new ModuleCommandInfo()
                    {
                        Name = cmdHelp.Title,
                        Link = $"{cmdHelp.Title}.md",
                        Description = "{{ Add description here }}"
                    };
                    moduleFileInfo.Commands.Add(mci);

                    if (! Directory.Exists(moduleFolder))
                    {
                        Directory.CreateDirectory(moduleFolder);
                    }

                    var settings = new WriterSettings(Encoding, Path.Combine(moduleFolder, $"{cmdHelp.Title}.md"));
                    using var cmdWrt = new CommandHelpMarkdownWriter(settings);
                    WriteObject(this.InvokeProvider.Item.Get(cmdWrt.Write(cmdHelp, Metadata).FullName));
                }

                if (WithModulePage)
                {
                    string moduleFilePath = Path.Combine(moduleFolder, $"{moduleName}.md");
                    var modulePageSettings = new WriterSettings(Encoding, moduleFilePath);
                    using var modulePageWriter = new ModulePageWriter(modulePageSettings);
                    WriteObject(this.InvokeProvider.Item.Get(modulePageWriter.Write(moduleFileInfo).FullName));
                }
            }
        }

        internal class CommandModuleTransformAttribute : ArgumentTransformationAttribute
        {
            public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
            {
                throw new NotImplementedException();
            }

            internal object AttemptParameterConvert(EngineIntrinsics engineIntrinsics, string name, Type type)
            {
                if (type == typeof(PSModuleInfo))
                {
                    var module = System.Management.Automation.PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Get-Module").AddParameter("Name", name).Invoke<PSModuleInfo>().First();
                    if (module is not null)
                    {
                        return module;
                    }
                }
                if (type == typeof(CommandInfo))
                {
                    var cmd = System.Management.Automation.PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Get-Command").AddParameter("Name", name).Invoke<CommandInfo>().First();
                    if (cmd is not null)
                    {
                        return cmd;
                    }
                }
                return name;
            }

        }

        internal class StringToCommandInfoTransformationAttribute : CommandModuleTransformAttribute
        {
            public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
            {
                if (inputData is string cmdName)
                {
                    return AttemptParameterConvert(engineIntrinsics, cmdName, typeof(CommandInfo));
                }
                if (inputData is Array cmdArray)
                {
                    List<object> cmdList = new();
                    foreach (var cmd in cmdArray)
                    {
                        cmdList.Add(AttemptParameterConvert(engineIntrinsics, cmd.ToString(), typeof(CommandInfo)));
                    }
                    return cmdList.ToArray();
                }
                return inputData;
            }

        }

        internal sealed class StringToPsModuleInfoTransformationAttribute : CommandModuleTransformAttribute
        {
            public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
            {
                if (inputData is string moduleName)
                {
                    return AttemptParameterConvert(engineIntrinsics, moduleName, typeof(PSModuleInfo));
                }

                if (inputData is Array moduleArray)
                {
                    List<object> moduleList = new();
                    foreach (var mod in moduleArray)
                    {
                        moduleList.Add(AttemptParameterConvert(engineIntrinsics, mod.ToString(), typeof(PSModuleInfo)));
                    }
                    return moduleList.ToArray();
                }

                return inputData;
            }
        }
    }
}
