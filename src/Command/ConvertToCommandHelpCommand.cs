// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    /// <summary>
    /// Cmdlet to import a markdown file and convert it to a CommandHelp object.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "CommandHelp")]
    [OutputType(typeof(Microsoft.PowerShell.PlatyPS.Model.CommandHelp[]))]
    public sealed class NewCommandHelpCommand : PSCmdlet
    {
#region cmdlet parameters
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty]
        public CommandInfo[] CommandInfo { get; set; } = new CommandInfo[0];
#endregion

        List<CommandHelp>? cmdHelpObjs = null;
        TransformSettings transformSettings = new();

        protected override void BeginProcessing()
        {
            transformSettings = new TransformSettings()
            {
                CreateModulePage = false,
                DoubleDashList = false,
                ExcludeDontShow = true,
                HelpVersion = "3.0.0",
                Locale = CultureInfo.GetCultureInfo("en-US"),
                UseFullTypeName = true
            };
        }

        protected override void ProcessRecord()
        {
            cmdHelpObjs = new TransformCommand(transformSettings).Transform(CommandInfo);
            foreach (var cHelp in cmdHelpObjs)
            {
                WriteObject(cHelp);
            }
        }
    }
}
