using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
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
            var cmdHelp = new Collection<CommandHelp>();

            foreach(var command in commandNames)
            {
                Collection<CmdletInfo> cmdletInfo = PowerShellAPI.GetCmdletInfo(command);
            }

            return cmdHelp;        
        }

        internal CommandHelp ConvertCmdletInfo(CmdletInfo cmdletInfo)
        {
            Collection<PSCustomObject> help = PowerShellAPI.GetHelpForCmdlet(cmdletInfo.Name);

            return null;
        }
    }
}