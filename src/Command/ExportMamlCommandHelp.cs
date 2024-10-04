// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using Microsoft.PowerShell.PlatyPS.MAML;
using Microsoft.PowerShell.PlatyPS.Model;

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
        public CommandHelp[] CommandHelp { get; set; } = Array.Empty<CommandHelp>();

        [Parameter()]
        [ArgumentToEncodingTransformation]
        [ArgumentCompleter(typeof(EncodingCompleter))]
        public System.Text.Encoding Encoding { get; set; } = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        [Parameter()]
        public SwitchParameter Force { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        public string OutputFolder { get; set; } = string.Empty;

        private List<CommandHelp> _commandHelps = new List<CommandHelp>();
        #endregion

        private DirectoryInfo? outputDirectory = null;

        protected override void BeginProcessing()
        {
        }

        protected override void ProcessRecord()
        {
            foreach (CommandHelp commandHelp in CommandHelp)
            {
                _commandHelps.Add(commandHelp);
            }
        }

        protected override void EndProcessing()
        {
            bool outputDirectoryCheck = true;
            if (ShouldProcess(OutputFolder))
            {
                outputDirectory = PathUtils.CreateOrGetOutputDirectory(this, OutputFolder, Force);
            }
            else
            {
                outputDirectoryCheck = false;
            }

            if (outputDirectoryCheck && outputDirectory is null)
            {
                ThrowTerminatingError(new ErrorRecord(new InvalidOperationException("outputDirectory is null"), "OutputFolderNotFound", ErrorCategory.InvalidOperation, OutputFolder));
            }

            // emit a MAML file for each group of CommandHelp objects
            var helpGroup = _commandHelps.GroupBy(c => c.ModuleName);
            foreach(var group in helpGroup)
            {
                string moduleName = group.First().ModuleName;
                string helpFileName = $"{moduleName}-Help.xml";

                if (!ShouldProcess(helpFileName))
                {
                    continue;
                }

                var moduleMamlBasePath = Path.Combine(outputDirectory?.FullName, moduleName);
                var moduleMamlPath = Path.Combine(moduleMamlBasePath, helpFileName);
                var helpInfos = MamlConversionHelper.ConvertCommandHelpToMamlHelpItems(group.ToList<CommandHelp>());
                // Convert the command help to MAML and write the file
                if (!Directory.Exists(moduleMamlBasePath))
                {
                    Directory.CreateDirectory(moduleMamlBasePath);
                }

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
