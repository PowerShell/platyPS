// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using Microsoft.PowerShell.PlatyPS.Model;
using Microsoft.PowerShell.PlatyPS.YamlWriter;

namespace Microsoft.PowerShell.PlatyPS
{
    /// <summary>
    /// Cmdlet to generate the markdown help for commands, all commands in a module or from a MAML file.
    /// </summary>
    [Cmdlet(VerbsData.Export, "YamlModuleFile", HelpUri = "")]
    [OutputType(typeof(FileInfo[]))]
    public sealed class ExportYamlModuleFileCommand : PSCmdlet
    {
        #region cmdlet parameters

        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public ModuleFileInfo[] ModuleFile { get; set; } = Array.Empty<ModuleFileInfo>();

        [Parameter]
        [ArgumentToEncodingTransformation]
        [ArgumentEncodingCompletions]
        public System.Text.Encoding Encoding { get; set; }  = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        [Parameter()]
        public SwitchParameter Force { get; set; }

        [Parameter()]
        public string OutputFolder { get; set; } = Environment.CurrentDirectory;

        #endregion

        private string outputFolderPath = string.Empty;

        protected override void BeginProcessing()
        {
            outputFolderPath = this.SessionState.Path.GetUnresolvedProviderPathFromPSPath(OutputFolder);

            if (File.Exists(outputFolderPath))
            {
                var exception = new InvalidOperationException(string.Format(Microsoft_PowerShell_PlatyPS_Resources.PathIsNotFolder, outputFolderPath));
                ErrorRecord err = new ErrorRecord(exception, "PathIsNotFolder", ErrorCategory.InvalidOperation, outputFolderPath);
                ThrowTerminatingError(err);
            }

            if (!Directory.Exists(outputFolderPath))
            {
                Directory.CreateDirectory(outputFolderPath);
            }
        }

        protected override void ProcessRecord()
        {
            string moduleName;
            foreach (ModuleFileInfo moduleFile in ModuleFile)
            {
                if (moduleFile.Metadata.Contains("Module Name"))
                {
                    moduleName = moduleFile.Metadata["Module Name"].ToString();
                }
                else
                {
                    WriteError(new ErrorRecord(new InvalidDataException(moduleFile.Module), "ExportYamlModuleFile,BadModuleInfo", ErrorCategory.InvalidData, moduleFile));
                    continue;
                }

                var yamlPath = Path.Combine($"{outputFolderPath}", $"{moduleName}.yml");
                if (new FileInfo(yamlPath).Exists && ! Force)
                {
                    // should be error?
                    WriteWarning($"skipping {moduleName}");
                }
                else
                {
                    var settings = new WriterSettings(Encoding, yamlPath);
                    var mfWrt = new ModulePageYamlWriter(settings);
                    WriteObject(this.InvokeProvider.Item.Get(mfWrt.Write(moduleFile).FullName));
                }
            }
        }
    }
}

