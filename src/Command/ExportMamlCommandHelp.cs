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
        public CommandHelp[] Command { get; set; } = Array.Empty<CommandHelp>();

        [Parameter()]
        public System.Text.Encoding Encoding { get; set; } = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        [Parameter()]
        public SwitchParameter Force { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        public string OutputFile { get; set; } = string.Empty;

        private List<CommandHelp> _commandHelps = new List<CommandHelp>();
        private FileInfo? fileInfo;
        #endregion

        protected override void BeginProcessing()
        {
            string fullPath = this.SessionState.Path.GetUnresolvedProviderPathFromPSPath(OutputFile);
            fileInfo = new FileInfo(fullPath);

            if (fileInfo.Exists && ! Force)
            {
                var exception = new InvalidOperationException($"File {fullPath} exists");
                ErrorRecord err = new ErrorRecord(exception, "FileExists", ErrorCategory.InvalidOperation, fullPath);
                ThrowTerminatingError(err);
            }
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
            if (fileInfo is null)
            {
                ThrowTerminatingError(new ErrorRecord(new InvalidOperationException("file is null"), "fileInfo is null", ErrorCategory.InvalidOperation, OutputFile));
                throw new InvalidOperationException("fileInfo is null");
            }

            if (ShouldProcess(fileInfo.FullName, "Export-MamlCommandHelp"))
            {
                if (! fileInfo.Directory.Exists)
                {
                    Directory.CreateDirectory(fileInfo.Directory.FullName);
                }

                // Convert the command help to MAML and write the file
                var helpInfos = MamlConversionHelper.ConvertCommandHelpToMamlHelpItems(_commandHelps);
                MamlConversionHelper.WriteToFile(helpInfos, fileInfo.FullName, Encoding);
            }
        }
    }
}
