// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

using Microsoft.PowerShell.PlatyPS.MarkdownWriter;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    /// <summary>
    /// Cmdlet to import a markdown file and convert it to a CommandHelp object.
    /// </summary>
    [Cmdlet(VerbsData.Import, "MarkdownCommandHelp", HelpUri = "")]
    public sealed class ImportMarkdownHelpCommand : PSCmdlet
    {
#region cmdlet parameters

        [Parameter(Mandatory=true, Position=0, ValueFromPipeline=true)]
        public string[] Path { get; set; } = Array.Empty<string>();

#endregion

        protected override void ProcessRecord()
        {
            foreach (string path in Path)
            {
                try
                {
                    foreach (var filePath in this.SessionState.Path.GetResolvedPSPathFromPSPath(path))
                    {
                        string fullPath = filePath.Path;
                        try
                        {
                            WriteObject(MarkdownConverter.GetCommandHelpFromMarkdownFile(fullPath));
                        }
                        catch (Exception e)
                        {
                            WriteError(new ErrorRecord(e, "FailedToImportMarkdown", ErrorCategory.InvalidOperation, fullPath));
                        }
                    }
                }
                catch (Exception e)
                {
                    WriteError(new ErrorRecord(e, "FailedToImportMarkdown", ErrorCategory.InvalidOperation, path));
                }
                
            }
        }
    }
}

