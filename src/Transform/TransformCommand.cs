// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    internal class TransformCommand : TransformBase
    {
        public TransformCommand(TransformSettings settings) : base(settings)
        {
        }

        internal List<CommandHelp> Transform(IEnumerable<CommandInfo> command)
        {
            List<CommandHelp> cmdHelpList = new();
            foreach(var cmd in command)
            {
                cmdHelpList.Add(Transform(cmd));
            }
            return cmdHelpList;
        }

        internal CommandHelp Transform(CommandInfo command)
        {
            return ConvertCmdletInfo(command);
        }

        internal Collection<CommandHelp> Transform(string command)
        {
            Collection<CommandHelp> cmdHelp = new();
            Collection<CommandInfo> cmdletInfos = PowerShellAPI.GetCommandInfo(command, Settings.Session);

            foreach(var cmdletInfo in cmdletInfos)
            {
                cmdHelp.Add(ConvertCmdletInfo(cmdletInfo));
            }

            return cmdHelp;
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
