// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;
using Microsoft.PowerShell.PlatyPS.MAML;
using Microsoft.PowerShell.PlatyPS.Model;
using System.Management.Automation.Language;

namespace Microsoft.PowerShell.PlatyPS
{
    /// <summary>
    /// Cmdlet to generate the MAML help for commands.
    /// This is different than the Export-YamlCommandHelp cmdlet in that it generates expects a group of CommandHelp objects and generates a single MAML file.
    /// </summary>
    [Cmdlet(VerbsData.Export, "MamlCommandHelp", SupportsShouldProcess = true, HelpUri = "")]
    [OutputType(typeof(FileInfo[]))]
    public sealed class ExportMamlCommandHelpCommand : PSCmdlet
    {
        #region cmdlet parameters

        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public CommandHelp[] Command { get; set; } = Array.Empty<CommandHelp>();

        [Parameter()]
        [ArgumentToEncodingTransformation]
        [ArgumentCompleter(typeof(EncodingCompleter))]
        public System.Text.Encoding Encoding { get; set; } = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        [Parameter()]
        public SwitchParameter Force { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        public string OutputDirectory { get; set; } = string.Empty;

        private List<CommandHelp> _commandHelps = new List<CommandHelp>();
        #endregion

        private DirectoryInfo? outputDirectory = null;

        protected override void BeginProcessing()
        {
        }

        protected override void ProcessRecord()
        {
            foreach (CommandHelp commandHelp in Command)
            {
                _commandHelps.Add(commandHelp);
            }
        }

        protected override void EndProcessing()
        {
            if (ShouldProcess(OutputDirectory))
            {
                outputDirectory = PathUtils.CreateOrGetOutputDirectory(this, OutputDirectory, Force);
            }
            else
            {
                // Just report on the creation of the base directory.
                return;
            }

            if (outputDirectory is null)
            {
                ThrowTerminatingError(new ErrorRecord(new InvalidOperationException("file is null"), "fileInfo is null", ErrorCategory.InvalidOperation, OutputDirectory));
                throw new InvalidOperationException("fileInfo is null"); // not reached
            }

            var helpGroup = _commandHelps.GroupBy(c => c.ModuleName);

            foreach(var group in helpGroup)
            {
                string moduleName = group.First().ModuleName;
                var helpInfos = MamlConversionHelper.ConvertCommandHelpToMamlHelpItems(group.ToList<CommandHelp>());
                // Convert the command help to MAML and write the file
                // var moduleDirectory = Path.Combine(outputDirectory.FullName, moduleName);
                // Directory.CreateDirectory(moduleDirectory);
                var moduleMamlPath = Path.Combine(outputDirectory.FullName, $"{moduleName}-Help.xml");
                if (File.Exists(moduleMamlPath) && ! Force)
                {
                    WriteWarning(string.Format(Model.Constants.skippingMessageFmt, moduleMamlPath));
                }
                else
                {
                    WriteObject(this.InvokeProvider.Item.Get(MamlConversionHelper.WriteToFile(helpInfos, moduleMamlPath, Encoding).FullName));
                }
            }
        }
    }
}
