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
    [Cmdlet(VerbsData.Export, "YamlCommandHelp", HelpUri = "")]
    [OutputType(typeof(FileInfo[]))]
    public sealed class ExportYamlCommandHelpCommand : PSCmdlet
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
                    var yamlPath = Path.Combine($"{fullPath}", $"{cmdletHelp.Title}.yml");
                    if (new FileInfo(yamlPath).Exists && ! Force)
                    {
                        // should be error
                        WriteWarning($"skipping {cmdletHelp.Title}");
                    }
                    else
                    {
                        var settings = new CommandHelpWriterSettings(Encoding, yamlPath);
                        var cmdWrt = new CommandHelpYamlWriter(settings);
                        WriteObject(cmdWrt.Write(cmdletHelp, null));
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

