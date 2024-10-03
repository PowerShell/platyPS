// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.IO;
using System.Management.Automation;
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
        public CommandHelp[] CommandHelp { get; set; } = Array.Empty<CommandHelp>();

        [Parameter()]
        [ArgumentToEncodingTransformation]
        [ArgumentCompleter(typeof(EncodingCompleter))]
        public System.Text.Encoding Encoding { get; set; } = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        [Parameter()]
        public SwitchParameter Force { get; set; }

        [Parameter()]
        public string OutputFolder { get; set; } = Environment.CurrentDirectory;

        [Parameter()]
        public Hashtable Metadata { get; set; } = new Hashtable();

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
            foreach (CommandHelp ch in CommandHelp)
            {
                if (! ShouldProcess(ch.ToString()))
                {
                    continue;
                }

                var yamlBasePath = Path.Combine($"{fullPath}", $"{ch.ModuleName}");
                if (! Directory.Exists(yamlBasePath))
                {
                    Directory.CreateDirectory(yamlBasePath);
                }

                var yamlPath = Path.Combine($"{yamlBasePath}", $"{ch.Title}.yml");
                if (new FileInfo(yamlPath).Exists && ! Force)
                {
                    // should be error?
                    WriteWarning(string.Format(Constants.skippingMessageFmt, ch.Title));
                }
                else
                {
                    var settings = new WriterSettings(Encoding, yamlPath);
                    var yamlWriter = new CommandHelpYamlWriter(settings);
                    // Check for non-overridable keys in the provided Metadata
                    if (Metadata.Keys.Count > 0)
                    {
                        var badKeys = MetadataUtils.WarnBadKeys(this, Metadata);
                        badKeys.ForEach(k => Metadata.Remove(k));
                    }

                    if (ShouldProcess("Create yaml file {yamlPath}"))
                    {
                        WriteObject(this.InvokeProvider.Item.Get(yamlWriter.Write(ch, Metadata).FullName));
                    }
                }
            }
        }
    }
}

