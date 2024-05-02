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
        public object[] ModuleFile { get; set; } = Array.Empty<string>();

        [Parameter]
        [ArgumentToEncodingTransformation]
        [ArgumentEncodingCompletions]
        public System.Text.Encoding Encoding { get; set; }  = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

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


            foreach (object o in ModuleFile)
            {
                if (o is ModuleFileInfo moduleFile)
                {
                    var markdownPath = Path.Combine($"{fullPath}", $"{moduleFile.Title}.md");
                    if (new FileInfo(markdownPath).Exists && ! Force)
                    {
                        // should be error
                        WriteWarning($"skipping {moduleFile.Title}");
                    }
                    else
                    {
                        var settings = new WriterSettings(Encoding, markdownPath);
                        var mfWrt = new ModulePageYamlWriter(settings);
                        WriteObject(this.InvokeProvider.Item.Get(mfWrt.Write(moduleFile).FullName));
                    }
                }
                else
                {
                    // should be error
                    WriteWarning(o.ToString() + " is not a ModuleFile object.");
                }
            }
        }
    }
}

