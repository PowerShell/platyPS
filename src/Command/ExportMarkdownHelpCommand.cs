// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Management.Automation;
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
        public object[] Command { get; set; } = Array.Empty<string>();

        [Parameter()]
        public System.Text.Encoding Encoding { get; set; } = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        [Parameter()]
        public SwitchParameter Force { get; set; }

        [Parameter()]
        public string OutputFolder { get; set; } = Environment.CurrentDirectory;

        #endregion

        protected override void EndProcessing()
        {
            string fullPath = this.SessionState.Path.GetUnresolvedProviderPathFromPSPath(OutputFolder);

            if (File.Exists(fullPath))
            {
                var exception = new InvalidOperationException(string.Format(Microsoft_PowerShell_PlatyPS_Resources.PathIsNotFolder, fullPath));
                ErrorRecord err = new ErrorRecord(exception, "PathIsNotFolder", ErrorCategory.InvalidOperation, fullPath);
                ThrowTerminatingError(err);
            }

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }


            foreach (object o in Command)
            {
                if (o is CommandHelp cmdletHelp)
                {
                    var markdownPath = Path.Combine($"{fullPath}", $"{cmdletHelp.Title}.md");
                    if (new FileInfo(markdownPath).Exists && ! Force)
                    {
                        // should be error
                        WriteWarning($"skipping {cmdletHelp.Title}");
                    }
                    else
                    {
                        var settings = new CommandHelpWriterSettings(Encoding, markdownPath);
                        var cmdWrt = new CommandHelpMarkdownWriter(settings);
                        WriteObject(this.InvokeProvider.Item.Get(cmdWrt.Write(cmdletHelp, null).FullName));
                    }
                }
                else
                {
                    // should be error
                    WriteWarning(o.ToString() + " is not a CommandHelp object.");
                }
            }
        }
    }
}

