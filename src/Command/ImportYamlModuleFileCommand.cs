// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    /// <summary>
    /// Cmdlet to import a yaml file and convert it to a ModuleFile object.
    /// </summary>
    [Cmdlet(VerbsData.Import, "YamlModuleFile", DefaultParameterSetName = "Path", HelpUri = "")]
    [OutputType(typeof(ModuleFileInfo))]
    public sealed class ImportYamlModuleFileCommand : PSCmdlet
    {
#region cmdlet parameters

        [Parameter(Mandatory=true, Position=0, ValueFromPipeline=true, ParameterSetName= "Path")]
        [ValidateNotNullOrEmpty]
        [SupportsWildcards]
        public string[] Path { get; set; } = Array.Empty<string>();

        [Parameter(Mandatory=true, ValueFromPipeline=true, ParameterSetName= "LiteralPath")]
        [ValidateNotNullOrEmpty]
        public string[] LiteralPath { get; set; } = Array.Empty<string>();

        [Parameter]
        public SwitchParameter AsDictionary { get; set; }

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
                if (AsDictionary)
                {
                    if (YamlUtils.TryGetOrderedDictionaryFromText(File.ReadAllText(path), out var dictionaryResult))
                    {
                        WriteObject(dictionaryResult);
                    }
                    else
                    {
                        WriteError(new ErrorRecord(new InvalidOperationException("DeserializationError"), "ImportYamlModuleFile,FailedToConvertYamlToDictionary", ErrorCategory.InvalidOperation, path));
                    }
                    continue;
                }
                
                if (YamlUtils.TryReadModuleFile(path, out ModuleFileInfo? moduleFileInfo, out Exception? deserializationError))
                {
                    WriteObject(moduleFileInfo);
                }
                else
                {
                    var wrappedException = new InvalidDataException($"Could not parse file as a module file. '{path}'", deserializationError);
                    WriteError(new ErrorRecord(wrappedException, "ImportYamlModuleFile,FailedToConvertYamltoModuleFileInfo", ErrorCategory.InvalidOperation, path));
                }
            }
        }
    }
}
