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
    /// Cmdlet to generate the markdown help for commands, all commands in a module or from a MAML file.
    /// </summary>
    [Cmdlet(VerbsData.Import, "MamlHelp", HelpUri = "")]
    [OutputType(typeof(CommandHelp[]))]
    public sealed class ImportMamlHelpCommand : PSCmdlet
    {
        #region cmdlet parameters

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string MamlFile { get; set; } = string.Empty;

        #endregion

        TransformSettings transformSettings = new TransformSettings();

        protected override void BeginProcessing()
        {
            /*
            {
            transformSettings = new TransformSettings();
                AlphabeticParamsOrder = AlphabeticParamsOrder,
                DoubleDashList = ConvertDoubleDashLists,
                ExcludeDontShow = ExcludeDontShow,
                FwLink = FwLink,
                HelpVersion = HelpVersion,
                Locale = Locale is null ? CultureInfo.GetCultureInfo("en-US") : new CultureInfo(Locale),
                ModuleGuid = ModuleGuid is null ? null : Guid.Parse(ModuleGuid),
                ModuleName = ModuleName,
                OnlineVersionUrl = OnlineVersionUrl,
                Session = Session,
                UseFullTypeName = UseFullTypeName
            };
            */
        }

        protected override void ProcessRecord()
        {
            Collection<CommandHelp>? cmdHelpObjs = null;

            foreach(var mamlPath in SessionState.Path.GetResolvedPSPathFromPSPath(MamlFile))
            {
                try
                {
                    cmdHelpObjs = new TransformMaml(transformSettings).Transform(new string[] { mamlPath.Path });

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
                    WriteError(new ErrorRecord(e, "FailedToTransformMaml", ErrorCategory.InvalidOperation, mamlPath.Path));
                }
            }
        }
    }
}

