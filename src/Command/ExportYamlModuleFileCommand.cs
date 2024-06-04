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
    [Cmdlet(VerbsData.Export, "YamlModuleFile", SupportsShouldProcess = true, HelpUri = "")]
    [OutputType(typeof(FileInfo[]))]
    public sealed class ExportYamlModuleFileCommand : PSCmdlet
    {
        #region cmdlet parameters

        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public ModuleFileInfo[] ModuleFile { get; set; } = Array.Empty<ModuleFileInfo>();

        [Parameter]
        [ArgumentToEncodingTransformation]
        [ArgumentCompleter(typeof(EncodingCompleter))]
        public System.Text.Encoding Encoding { get; set; }  = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        [Parameter()]
        public SwitchParameter Force { get; set; }

        [Parameter()]
        public string OutputFolder { get; set; } = Environment.CurrentDirectory;

        [Parameter()]
        public Hashtable Metadata { get; set; } = new();

        #endregion

        private DirectoryInfo? outputFolder;

        protected override void BeginProcessing()
        {
            outputFolder = PathUtils.CreateOrGetOutputDirectory(this, OutputFolder, Force);
        }

        protected override void ProcessRecord()
        {
            string moduleName;
            if (outputFolder is null)
            {
                var missingOutputFolder = new ItemNotFoundException(OutputFolder);
                ThrowTerminatingError(new ErrorRecord(missingOutputFolder, "ExportYamlModuleFile,MissingOutputFolder", ErrorCategory.ObjectNotFound, OutputFolder));
                throw missingOutputFolder; // not reached
            }

            foreach (ModuleFileInfo moduleFile in ModuleFile)
            {
                if (moduleFile.Metadata.ContainsKey("Module Name"))
                {
                    moduleName = moduleFile.Metadata["Module Name"].ToString();
                }
                else
                {
                    WriteError(new ErrorRecord(new InvalidDataException(moduleFile.Module), "ExportYamlModuleFile,BadModuleInfo", ErrorCategory.InvalidData, moduleFile));
                    continue;
                }

                MetadataUtils.MergeNewModulefileMetadata(Metadata, moduleFile);

                var yamlPath = Path.Combine($"{outputFolder.FullName}", $"{moduleName}.yml");
                if (new FileInfo(yamlPath).Exists && ! Force)
                {
                    // should be error?
                    WriteWarning(string.Format(Constants.skippingMessageFmt, moduleName));
                }
                else
                {
                    var settings = new WriterSettings(Encoding, yamlPath);
                    var mfWrt = new ModulePageYamlWriter(settings);
                    if (ShouldProcess(yamlPath))
                    {
                        WriteObject(this.InvokeProvider.Item.Get(mfWrt.Write(moduleFile).FullName));
                    }
                }
            }
        }
    }
}

