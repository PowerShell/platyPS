// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Microsoft.PowerShell.PlatyPS
{
    internal static class PowerShellAPI
    {
        [ThreadStatic]
        private static System.Management.Automation.PowerShell? ps;

        public static Collection<CommandInfo> GetCommandInfo(string commandName)
        {
            ps ??= System.Management.Automation.PowerShell.Create(RunspaceMode.CurrentRunspace);
            ps.Commands.Clear();

            try
            {
                var commandInfos = ps
                     .AddCommand(@"Microsoft.PowerShell.Core\Get-Command")
                     .AddParameter("Name", commandName)
                     .Invoke<CommandInfo>();

                if (commandInfos.Count > 0)
                {
                    return commandInfos;
                }
                else
                {
                    throw new CommandNotFoundException(commandName);
                }
            }
            // This happens when Get-Command throws and ErrorAction is set to Stop
            catch (System.Management.Automation.ActionPreferenceStopException apse)
            {
                if (apse.ErrorRecord.Exception is CommandNotFoundException cnfe)
                {
                    throw cnfe;
                }

                throw;
            }
        }

        public static Collection<CommandInfo> GetCmdletInfoFromModule(string moduleName)
        {
            ps ??= System.Management.Automation.PowerShell.Create(RunspaceMode.CurrentRunspace);
            ps.Commands.Clear();

            Collection<PSModuleInfo>? moduleInfo = GetModuleInfo(moduleName);

            Collection<CommandInfo> cmdletInfos = new();

            if (moduleInfo?.Count > 0)
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
            else
            {
                throw new ItemNotFoundException(moduleName);
            }

            return cmdletInfos;
        }

        public static Collection<PSModuleInfo>? GetModuleInfo(string moduleName)
        {
            if (string.IsNullOrEmpty(moduleName))
            {
                return null;
            }

            ps ??= System.Management.Automation.PowerShell.Create(RunspaceMode.CurrentRunspace);
            ps.Commands.Clear();

            Collection<PSModuleInfo>? modules = null;

            try
            {
                // first try if the module is loaded
                modules = ps
                    .AddCommand(@"Microsoft.PowerShell.Core\Get-Module")
                    .AddParameter("Name", moduleName)
                    .Invoke<PSModuleInfo>();

                if (modules.Count == 0)
                {
                    // if not found try to import first

                    modules = ps
                        .AddCommand(@"Microsoft.PowerShell.Core\Import-Module")
                        .AddParameter("Name", moduleName)
                        .AddParameter("PassThru")
                        .Invoke<PSModuleInfo>();
                }
            }
            catch (FileNotFoundException)
            {
                // swallow the exception and eventually return null;
            }

            return modules;
        }

        internal static void InitializeRemoteSession(PSSession session)
        {
            ps = System.Management.Automation.PowerShell.Create();
            ps.Runspace = session.Runspace;
        }

        internal static Collection<PSObject> GetHelpForCmdlet(string cmdletName)
        {
            ps ??= System.Management.Automation.PowerShell.Create(RunspaceMode.CurrentRunspace);
            ps.Commands.Clear();

            return ps
                .AddCommand(@"Microsoft.PowerShell.Core\Get-Help")
                .AddParameter("Name", cmdletName)
                .AddParameter("Full")
                .Invoke();
        }

        internal static void Reset()
        {
            ps?.Dispose();
            ps = null;
        }
    }
}
