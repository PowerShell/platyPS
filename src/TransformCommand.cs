// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.ObjectModel;
using System.Management.Automation;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    internal class TransformCommand : TransformBase
    {
        public TransformCommand(TransformSettings settings) : base(settings)
        {
        }

        internal override Collection<CommandHelp> Transform(string[] commandNames)
        {
            Collection<CommandHelp> cmdHelp = new();

            foreach (var command in commandNames)
            {
                Collection<CommandInfo> cmdletInfos = PowerShellAPI.GetCommandInfo(command, Settings.Session);

                foreach (var cmdletInfo in cmdletInfos)
                {
                    cmdHelp.Add(ConvertCmdletInfo(cmdletInfo));
                }
            }

            return cmdHelp;
        }
    }
}
