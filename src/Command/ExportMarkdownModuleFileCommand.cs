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
using System.Management.Automation.Language;
using System.Management.Automation.Runspaces;

using Microsoft.PowerShell.PlatyPS.MarkdownWriter;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    /// <summary>
    /// Cmdlet to generate the markdown help for commands, all commands in a module or from a MAML file.
    /// </summary>
    [Cmdlet(VerbsData.Export, "MarkdownModuleFile", HelpUri = "")]
    [OutputType(typeof(FileInfo[]))]
    public sealed class ExportMarkdownModuleFileCommand : PSCmdlet
    {
        #region cmdlet parameters

        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public ModuleFileInfo[] ModuleFileInfo { get; set; } = Array.Empty<ModuleFileInfo>();

        [Parameter]
        [ArgumentToEncodingTransformation]
        [ArgumentCompleter(typeof(EncodingCompleter))]
        public System.Text.Encoding Encoding { get; set; }  = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        [Parameter()]
        public SwitchParameter Force { get; set; }

        [Parameter]
        public Hashtable Metadata { get; set; } = new();

        [Parameter()]
        public string OutputFolder { get; set; } = Environment.CurrentDirectory;

        #endregion

        private string fullPath = string.Empty;

        protected override void BeginProcessing()
        {

            fullPath = this.SessionState.Path.GetUnresolvedProviderPathFromPSPath(OutputFolder);

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
        }

        protected override void ProcessRecord()
        {

            foreach (var moduleFile in ModuleFileInfo)
            {
                var markdownPath = Path.Combine($"{fullPath}", $"{moduleFile.Module}.md");
                if (new FileInfo(markdownPath).Exists && ! Force)
                {
                    // should be error
                    WriteWarning($"skipping {moduleFile.Module}");
                }
                else
                {
                    var settings = new WriterSettings(Encoding, markdownPath);
                    var mfWrt = new ModulePageWriter(settings);
                    // Add any additional supplied metadata
                    if (Metadata.Keys.Count > 0)
                    {
                        foreach(DictionaryEntry kv in Metadata)
                        {
                            string key = kv.Key.ToString();
                            if (MetadataUtils.ProtectedMetadataKeys.Any(k => string.Compare(key, k, true) == 0))
                            {
                                WriteWarning($"Metadata key '{key}' may not be overridden");
                            }
                            else
                            {
                                moduleFile.Metadata[key] = (string)kv.Value;
                            }
                        }
                    }

                    WriteObject(this.InvokeProvider.Item.Get(mfWrt.Write(moduleFile).FullName));
                }
            }
        }
    }
}

