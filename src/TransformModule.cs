using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    internal class TransformModule : TransformBase
    {
        public TransformModule(PSSession session) : base(session)
        {
        }

        internal override Collection<CommandHelp> Transform(string[] moduleNames)
        {
            Collection<CommandHelp> cmdHelp = new();

            foreach (var module in moduleNames)
            {
                Collection<CommandInfo> cmdletInfos = PowerShellAPI.GetCmdletInfoFromModule(module);

                foreach (var cmdletInfo in cmdletInfos)
                {
                    cmdHelp.Add(ConvertCmdletInfo(cmdletInfo));
                }
            }

            return cmdHelp;
        }
    }
}