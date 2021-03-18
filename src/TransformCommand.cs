using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    internal class TransformCommand : TransformBase
    {
        public TransformCommand(PSSession session) : base(session)
        {
        }

        internal override Collection<CommandHelp> Transform(string[] commandNames)
        {
            Collection<CommandHelp> cmdHelp = new();

            foreach (var command in commandNames)
            {
                Collection<CommandInfo> cmdletInfos = PowerShellAPI.GetCommandInfo(command);

                foreach (var cmdletInfo in cmdletInfos)
                {
                    cmdHelp.Add(ConvertCmdletInfo(cmdletInfo));
                }
            }

            return cmdHelp;
        }
    }
}