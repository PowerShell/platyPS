// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

using Microsoft.PowerShell.PlatyPS;
using Microsoft.PowerShell.PlatyPS.Model;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Microsoft.PowerShell.PlatyPS
{
    /// <summary>
    /// Cmdlet to import a yaml file and convert it to a ModuleFile object.
    /// </summary>
    [Cmdlet(VerbsData.Import, "YamlModuleFile", DefaultParameterSetName = "Path", HelpUri = "")]
    public sealed class ImportYamlModuleFileCommand : PSCmdlet
    {
#region cmdlet parameters

        [Parameter(Mandatory=true, Position=0, ValueFromPipeline=true, ParameterSetName= "Path")]
        [ValidateNotNullOrEmpty]
        public string[] Path { get; set; } = Array.Empty<string>();

        [Parameter(Mandatory=true, Position=0, ValueFromPipeline=true, ParameterSetName= "LiteralPath")]
        [ValidateNotNullOrEmpty]
        public string[] LiteralPath { get; set; } = Array.Empty<string>();

#endregion

        protected override void ProcessRecord()
        {
            List<string> resolvedPaths;
            try
            {
                // This is a list because the resolution process can result in multiple paths (in the case of non-literal path).
                resolvedPaths = PathUtils.ResolvePath(this, ParameterSetName == "LiteralPath" ? LiteralPath : Path, ParameterSetName == "LiteralPath" ? true : false);
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(e, "Could not resolve Path", ErrorCategory.InvalidOperation, ParameterSetName == "LiteralPath" ? LiteralPath : Path));
                return;
            }

            // These should be resolved paths, whether -LiteralPath was used or not.
            foreach (string path in resolvedPaths)
            {
                if (YamlUtils.TryReadModuleFile(path, out ModuleFileInfo? moduleFileInfo, out Exception? deserializationError))
                {
                    WriteObject(moduleFileInfo);
                }
                else
                {
                    WriteError(new ErrorRecord(deserializationError, "FailedToImportYaml", ErrorCategory.InvalidOperation, path));
                }
            }
        }
    }
}
