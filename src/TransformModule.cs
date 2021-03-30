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
        public TransformModule(TransformSettings settings) : base(settings)
        {
        }

        internal override Collection<CommandHelp> Transform(string[] moduleNames)
        {
            Collection<CommandHelp> cmdHelp = new();

            if (Settings.Session != null)
            {
                PowerShellAPI.InitializeRemoteSession(Settings.Session);
            }

            foreach (var module in moduleNames)
            {
                Collection<CommandInfo> cmdletInfos = PowerShellAPI.GetCmdletInfoFromModule(module);

                foreach (var cmdletInfo in cmdletInfos)
                {
                    cmdHelp.Add(ConvertCmdletInfo(cmdletInfo));
                }
            }

            PowerShellAPI.Reset();

            return cmdHelp;
        }
    }
}