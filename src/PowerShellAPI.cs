using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Microsoft.PowerShell.PlatyPS
{
    internal static class PowerShellAPI
    {
        private static System.Management.Automation.PowerShell ps;

        public static Collection<CommandInfo> GetCommandInfo(string commandName)
        {
            ps ??= System.Management.Automation.PowerShell.Create(RunspaceMode.CurrentRunspace);
            ps.Commands.Clear();

            return ps
                .AddCommand(@"Microsoft.PowerShell.Core\Get-Command")
                .AddParameter("Name", commandName)
                .Invoke<CommandInfo>();
        }

        public static Collection<CommandInfo> GetCmdletInfoFromModule(string moduleName)
        {
            ps ??= System.Management.Automation.PowerShell.Create(RunspaceMode.CurrentRunspace);
            ps.Commands.Clear();

            Collection<PSModuleInfo> moduleInfo = GetModuleInfo(moduleName);

            Collection<CommandInfo> cmdletInfos = new();

            if (moduleInfo != null)
            {
                foreach (var mod in moduleInfo)
                {
                    foreach (var cmdletInfo in mod.ExportedCommands.Values)
                    {
                        if (cmdletInfo.CommandType != CommandTypes.Alias)
                        {
                            cmdletInfos.Add(cmdletInfo);
                        }
                    }
                }
            }

            return cmdletInfos;
        }

        public static Collection<PSModuleInfo> GetModuleInfo(string moduleName)
        {
            ps ??= System.Management.Automation.PowerShell.Create(RunspaceMode.CurrentRunspace);
            ps.Commands.Clear();

            return ps
                .AddCommand(@"Microsoft.PowerShell.Core\Import-Module")
                .AddParameter("Name", moduleName)
                .AddStatement()
                .AddCommand(@"Microsoft.PowerShell.Core\Get-Module")
                .AddParameter("Name", moduleName)
                .Invoke<PSModuleInfo>();
        }

        internal static void InitializeRemoteSession(PSSession session)
        {
            ps ??= System.Management.Automation.PowerShell.Create();
            ps.Runspace = session.Runspace;
        }

        internal static Collection<PSObject> GetHelpForCmdlet(string cmdletName)
        {
            ps ??= System.Management.Automation.PowerShell.Create();
            ps.Commands.Clear();

            return ps
                .AddCommand(@"Microsoft.PowerShell.Core\Get-Help")
                .AddParameter("Name", cmdletName)
                .AddParameter("Full")
                .Invoke();
        }

        internal static void Reset()
        {
            if (ps != null)
            {
                ps.Dispose();
                ps = null;
            }
        }


    }
}