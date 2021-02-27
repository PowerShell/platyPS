using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Microsoft.PowerShell.PlatyPS
{
    internal static class PowerShellAPI
    {
        private static System.Management.Automation.PowerShell ps;

        public static Collection<CmdletInfo> GetCmdletInfo(string commandName)
        {
            ps ??= System.Management.Automation.PowerShell.Create();
            ps.Commands.Clear();

            return ps
                .AddCommand(@"Microsoft.PowerShell.Core\Get-Command")
                .AddParameter("Name", commandName)
                .Invoke<CmdletInfo>();
        }

        public static List<CmdletInfo> GetCmdletInfoFromModule(string moduleName)
        {
            ps ??= System.Management.Automation.PowerShell.Create();
            ps.Commands.Clear();

            Collection<PSModuleInfo> moduleInfo = ps
                .AddCommand(@"Microsoft.PowerShell.Core\Get-Module")
                .AddParameter("Name", moduleName)
                .Invoke<PSModuleInfo>();

            List<CmdletInfo> cmdletInfos = new List<CmdletInfo>();

            if (moduleInfo != null)
            {
                foreach(var mod in moduleInfo)
                {
                    cmdletInfos.AddRange(mod.ExportedCmdlets.Values);
                }
            }

            return cmdletInfos;
        }

        internal static void InitializeRemoteSession(PSSession session)
        {
            ps ??= System.Management.Automation.PowerShell.Create();
            ps.Runspace = session.Runspace;
        }

        internal static Collection<PSCustomObject> GetHelpForCmdlet(string cmdletName)
        {
            ps ??= System.Management.Automation.PowerShell.Create();
            ps.Commands.Clear();

            return ps
                .AddCommand(@"Microsoft.PowerShell.Core\Get-Help")
                .AddParameter("Name", cmdletName)
                .AddParameter("Full")
                .Invoke<PSCustomObject>();
        }
    }
}