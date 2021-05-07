// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Microsoft.PowerShell.PlatyPS
{
    internal class PowerShellAPI
    {
        [ThreadStatic]
        private static System.Management.Automation.PowerShell? ps;

        internal static Collection<CommandInfo> GetCommandInfo(string commandName, PSSession? session = null)
        {
            System.Management.Automation.PowerShell selectedPS = GetPowerShell(session);

            try
            {
                var commandInfos = selectedPS
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

        internal static Collection<CommandInfo> GetCmdletInfoFromModule(string moduleName, PSSession? session = null)
        {
            Collection<PSModuleInfo>? moduleInfo = GetModuleInfo(moduleName, session);

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

        internal static Collection<PSModuleInfo>? GetModuleInfo(string moduleName, PSSession? session = null)
        {
            System.Management.Automation.PowerShell selectedPS = GetPowerShell(session);

            Collection<PSModuleInfo>? modules = null;

            try
            {
                // first try if the module is loaded
                modules = selectedPS
                    .AddCommand(@"Microsoft.PowerShell.Core\Get-Module")
                    .AddParameter("Name", moduleName)
                    .Invoke<PSModuleInfo>();

                if (modules.Count == 0)
                {
                    // if not found try to import first

                    selectedPS.Commands.Clear();

                    modules = selectedPS
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

        internal static Collection<PSObject> GetHelpForCmdlet(string cmdletName, PSSession? session = null)
        {
            System.Management.Automation.PowerShell selectedPS = GetPowerShell(session);

            return selectedPS
                .AddCommand(@"Microsoft.PowerShell.Core\Get-Help")
                .AddParameter("Name", cmdletName)
                .AddParameter("Full")
                .Invoke();
        }

        private static System.Management.Automation.PowerShell GetPowerShell(PSSession? session)
        {
            System.Management.Automation.PowerShell selectedPS;

            if (session is null)
            {
                ps ??= System.Management.Automation.PowerShell.Create(RunspaceMode.CurrentRunspace);
                ps.Commands.Clear();
                selectedPS = ps;
            }
            else
            {
                selectedPS = System.Management.Automation.PowerShell.Create();
                selectedPS.Runspace = session.Runspace;
            }

            return selectedPS;
        }
    }
}
