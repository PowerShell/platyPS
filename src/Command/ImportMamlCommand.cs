// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    /// <summary>
    /// Cmdlet to generate the markdown help for commands, all commands in a module or from a MAML file.
    /// </summary>
    [Cmdlet(VerbsData.Import, "MamlHelp", HelpUri = "", DefaultParameterSetName = "Path")]
    [OutputType(typeof(CommandHelp[]))]
    public sealed class ImportMamlHelpCommand : PSCmdlet
    {
        #region cmdlet parameters

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = "Path")]
        [SupportsWildcards]
        [ValidateNotNullOrEmpty]
        public string[] Path { get; set; } = new string[0];

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "LiteralPath")]
        [ValidateNotNullOrEmpty]
        public string[] LiteralPath { get; set; } = new string[0];

        TransformMaml? transformer;

        protected override void BeginProcessing ()
        {
            transformer = new TransformMaml(new TransformSettings());
        }

        #endregion

        protected override void ProcessRecord()
        {
            Collection<CommandHelp>? cmdHelpObjs = null;

            bool isLiteralPath = string.Compare(ParameterSetName, "LiteralPath") == 0;
            var paths = isLiteralPath ? LiteralPath : Path;
            foreach(var mamlPath in PathUtils.ResolvePath(this, paths, isLiteralPath))
            {
                try
                {
                    cmdHelpObjs = transformer?.Transform(new string[] { mamlPath });

                    if (cmdHelpObjs != null)
                    {
                        foreach (var cmdletHelp in cmdHelpObjs)
                        {
                            WriteObject(cmdletHelp);
                        }
                    }
                }
                catch (Exception e)
                {
                    WriteError(new ErrorRecord(e, "FailedToTransformMaml", ErrorCategory.InvalidOperation, mamlPath));
                }
            }
        }
    }
}

