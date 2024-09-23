// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace Microsoft.PowerShell.PlatyPS
{
    [Cmdlet(VerbsDiagnostic.Test, "MarkdownCommandHelp")]
    [OutputType(typeof(bool))]
    [OutputType(typeof(MarkdownCommandHelpValidationResult))]
    public class TestMarkdownCommandHelpCommand : PSCmdlet
    {
        /// <summary>
        /// Gets or Sets the path to the item.
        /// </summary>
        [Parameter(Position = 0, ParameterSetName = "Item", Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [SupportsWildcards]
        public string[] Path { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or Sets the literal path to the item.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "Literal")]
        [Alias("PSPath", "LP")]
        public string[] LiteralPath { get; set; } = Array.Empty<string>();

        [Parameter]
        public SwitchParameter DetailView { get; set; }

        protected override void ProcessRecord()
        {
            List<string> resolvedPaths;
            try
            {
                // This is a list because the resolution process can result in multiple paths (in the case of non-literal path).
                resolvedPaths = PathUtils.ResolvePath(this, (ParameterSetName == "LiteralPath" ? LiteralPath : Path), ParameterSetName == "LiteralPath" ? true : false);
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(e, "Could not resolve Path", ErrorCategory.InvalidOperation, ParameterSetName == "LiteralPath" ? LiteralPath : Path));
                return;
            }

            foreach (var resolvedPath in resolvedPaths)
            {
                var result = MarkdownConverter.ValidateMarkdownFile(resolvedPath, out var Detail);
                if (DetailView)
                {
                    WriteObject(new MarkdownCommandHelpValidationResult(resolvedPath, result, Detail));
                }
                else
                {
                    WriteObject(result);
                }
            }
        }
    }
}
