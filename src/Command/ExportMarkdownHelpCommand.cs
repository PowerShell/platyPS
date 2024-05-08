// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Management.Automation.Runspaces;

using Microsoft.PowerShell.PlatyPS.MarkdownWriter;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    /// <summary>
    /// Cmdlet to generate the markdown help for commands, all commands in a module or from a MAML file.
    /// </summary>
    [Cmdlet(VerbsData.Export, "MarkdownCommandHelp", HelpUri = "")]
    [OutputType(typeof(FileInfo[]))]
    public sealed class ExportMarkdownCommandHelpCommand : PSCmdlet
    {
        #region cmdlet parameters

        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public CommandHelp[] Command { get; set; } = Array.Empty<CommandHelp>();

        [Parameter]
        [ArgumentToEncodingTransformation]
        [ArgumentEncodingCompletions]
        public System.Text.Encoding Encoding { get; set; }  = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        [Parameter()]
        public SwitchParameter Force { get; set; }

        [Parameter()]
        public string OutputFolder { get; set; } = Environment.CurrentDirectory;

        #endregion
        private string fullPath { get; set; } = string.Empty;

        protected override void BeginProcessing()
        {
            string outputPath = this.SessionState.Path.GetUnresolvedProviderPathFromPSPath(OutputFolder);

            // if there's a file that exists with the name of outputPath, that's an error and it's fatal.
            if (File.Exists(outputPath))
            {
                var exception = new InvalidOperationException(string.Format(Microsoft_PowerShell_PlatyPS_Resources.PathIsNotFolder, outputPath));
                ErrorRecord err = new ErrorRecord(exception, "PathIsNotFolder", ErrorCategory.InvalidOperation, outputPath);
                ThrowTerminatingError(err);
            }

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
        }

        protected override void ProcessRecord()
        {
            foreach (CommandHelp cmdletHelp in Command)
            {
                var markdownPath = Path.Combine($"{fullPath}", $"{cmdletHelp.Title}.md");
                if (new FileInfo(markdownPath).Exists && ! Force)
                {
                    // should be error
                    WriteWarning($"skipping {cmdletHelp.Title}, use -Force to export.");
                }
                else
                {
                    var settings = new WriterSettings(Encoding, markdownPath);
                    var cmdWrt = new CommandHelpMarkdownWriter(settings);
                    WriteObject(this.InvokeProvider.Item.Get(cmdWrt.Write(cmdletHelp, null).FullName));
                }
            }
        }
    }
}

