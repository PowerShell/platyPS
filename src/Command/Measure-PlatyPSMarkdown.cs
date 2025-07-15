// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    /// <summary>
    /// Cmdlet to determine the type of markdown file.
    /// </summary>
    [Cmdlet(VerbsDiagnostic.Measure, "PlatyPSMarkdown", DefaultParameterSetName = "Path", HelpUri = "")]
    [OutputType(typeof(MarkdownProbeInfo))]
    public sealed class MeasurePlatyPSMarkdown : PSCmdlet
    {
#region cmdlet parameters

        [Parameter(Mandatory=true, Position=0, ValueFromPipeline=true, ParameterSetName= "Path")]
        [ValidateNotNullOrEmpty]
        [SupportsWildcards]
        public string[] Path { get; set; } = Array.Empty<string>();

        [Parameter(Mandatory=true, ValueFromPipeline=true, ParameterSetName= "LiteralPath")]
        [Alias("PSPath", "LP")]
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
                try
                {
                    var markdownInfo = MarkdownProbe.Identify(path);
                    WriteObject(markdownInfo);
                }
                catch (Exception e)
                {
                    WriteError(new ErrorRecord(e, "FailedToImportMarkdown", ErrorCategory.InvalidOperation, path));
                }
            }
        }
    }
}
