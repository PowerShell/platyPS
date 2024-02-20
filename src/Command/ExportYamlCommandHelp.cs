// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

using Microsoft.PowerShell.PlatyPS.YamlWriter;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    /// <summary>
    /// Cmdlet to generate a yaml file from a CommandHelp object.
    /// </summary>
    [Cmdlet(VerbsData.Export, "YamlCommandHelp", HelpUri = "", SupportsShouldProcess = true)]
    [OutputType(typeof(FileInfo[]))]
    public sealed class ExportYamlCommandHelpCommand : PSCmdlet
    {
        #region cmdlet parameters

        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public CommandHelp[] Command { get; set; } = Array.Empty<CommandHelp>();

        [Parameter()]
        public System.Text.Encoding Encoding { get; set; } = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        [Parameter()]
        public SwitchParameter Force { get; set; }

        [Parameter()]
        public string OutputFolder { get; set; } = Environment.CurrentDirectory;

        #endregion

        private string fullPath { get; set; } = string.Empty;

        protected override void BeginProcessing()
        {
            fullPath = this.SessionState.Path.GetUnresolvedProviderPathFromPSPath(OutputFolder);

            // If the path is a file, throw an error.
            if (File.Exists(fullPath))
            {
                var exception = new InvalidOperationException(string.Format(Microsoft_PowerShell_PlatyPS_Resources.PathIsNotFolder, fullPath));
                ErrorRecord err = new ErrorRecord(exception, "PathIsNotFolder", ErrorCategory.InvalidOperation, fullPath);
                ThrowTerminatingError(err);
            }

            // Create the directory if it doesn't exist.
            if (!Directory.Exists(fullPath))
            {
                if (ShouldProcess("Create directory '{fullPath}'"))
                {
                    Directory.CreateDirectory(fullPath);
                }
            }
        }

        protected override void ProcessRecord()
        {
            foreach (CommandHelp ch in Command)
            {
                var yamlPath = Path.Combine($"{fullPath}", $"{ch.Title}.yml");
                if (new FileInfo(yamlPath).Exists && ! Force)
                {
                    // should be error
                    WriteWarning($"skipping {ch.Title}");
                }
                else
                {
                    var settings = new CommandHelpWriterSettings(Encoding, yamlPath);
                    var yamlWriter = new CommandHelpYamlWriter(settings);
                    if (ShouldProcess("Create yaml file {yamlPath}"))
                    {
                        WriteObject(yamlWriter.Write(ch, null));
                    }
                }
            }
        }
    }
}

