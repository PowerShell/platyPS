// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
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
    [Cmdlet(VerbsData.ConvertTo, "CommandHelp")]
    public sealed class ConvertToCommandHelpCommand : PSCmdlet
    {
#region cmdlet parameters

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string[] CommandInfo { get; set; } = new string[0];

#endregion

        Collection<CommandHelp>? cmdHelpObjs = null;

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
