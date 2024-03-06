// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

namespace Microsoft.PowerShell.PlatyPS
{
    public class MarkdownCommandHelpValidationResult
    {
        public string Path { get; set; }
        public bool IsValid { get; set; }
        public List<string> Messages { get; set; }
        public MarkdownCommandHelpValidationResult()
        {
            Path = string.Empty;
            IsValid = true;
            Messages = new List<string>();
        }

        public MarkdownCommandHelpValidationResult(string path, bool isValid, List<string> messages)
        {
            Path = path;
            IsValid = isValid;
            Messages = messages;
        }   

        public override string ToString()
        {
            return $"Path: {Path}, IsValid: {IsValid}";
        }

    }


    [Cmdlet(VerbsDiagnostic.Test, "MarkdownCommandHelp")]
    public class TestMarkdownCommandHelpCommand : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public string[] FullName { get; set; } = Array.Empty<string>();

        [Parameter]
        public SwitchParameter DetailView { get; set; }

        protected override void ProcessRecord()
        {
            foreach (var name in FullName)
            {
                foreach(var globbedPath in this.SessionState.Path.GetResolvedPSPathFromPSPath(name))
                {
                    List<string> Detail;
                    if (globbedPath.Provider.Name.Equals("FileSystem"))
                    {
                        var result = MarkdownConverter.ValidateMarkdownFile(globbedPath.Path, out Detail);
                        if (DetailView)
                        {
                            WriteObject(new MarkdownCommandHelpValidationResult(globbedPath.Path, result, Detail));
                        }
                        else
                        {
                            WriteObject(result);
                        }
                    }
                    else
                    {
                        WriteError(new ErrorRecord(new ArgumentException("Path must be a file path"), "NotAFilePath", ErrorCategory.InvalidArgument, globbedPath.Path));
                    }
                }
            }
        }

    }
}
