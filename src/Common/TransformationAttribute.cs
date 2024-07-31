
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace Microsoft.PowerShell.PlatyPS
{
    internal class CommandModuleTransformAttribute : ArgumentTransformationAttribute
    {
        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            throw new NotImplementedException();
        }

        internal object AttemptParameterConvert(EngineIntrinsics engineIntrinsics, string name, Type type)
        {
            using (var powershell = System.Management.Automation.PowerShell.Create(RunspaceMode.CurrentRunspace))
            {
                if (type == typeof(PSModuleInfo))
                {
                    var results = powershell.AddCommand("Get-Module").AddParameter("Name", name).Invoke<PSModuleInfo>();
                    powershell.Commands.Clear();
                    if (results.Count == 0)
                    {
                        results = powershell.AddCommand("Import-Module").AddParameter("Name", name).AddParameter("PassThru").Invoke<PSModuleInfo>();
                        powershell.Commands.Clear();
                    }

                    if (results.Count == 1 && results[0] is PSModuleInfo module)
                    {
                        return module;
                    }
                }

                if (type == typeof(CommandInfo))
                {
                    var cmd = System.Management.Automation.PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Get-Command").AddParameter("Name", name).Invoke<CommandInfo>().First();
                    if (cmd is not null)
                    {
                        return cmd;
                    }
                }
            }

            // We could not convert it, just return it for parameter binding.
            return name;
        }

    }

    internal class StringToCommandInfoTransformationAttribute : CommandModuleTransformAttribute
    {
        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            if (inputData is string cmdName)
            {
                return AttemptParameterConvert(engineIntrinsics, cmdName, typeof(CommandInfo));
            }
            if (inputData is Array cmdArray)
            {
                List<object> cmdList = new();
                foreach (var cmd in cmdArray)
                {
                    cmdList.Add(AttemptParameterConvert(engineIntrinsics, cmd.ToString(), typeof(CommandInfo)));
                }
                return cmdList.ToArray();
            }
            return inputData;
        }
    }

    internal sealed class StringToPsModuleInfoTransformationAttribute : CommandModuleTransformAttribute
    {
        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            if (inputData is string moduleName)
            {
                return AttemptParameterConvert(engineIntrinsics, moduleName, typeof(PSModuleInfo));
            }

            if (inputData is Array moduleArray)
            {
                List<object> moduleList = new();
                foreach (var mod in moduleArray)
                {
                    moduleList.Add(AttemptParameterConvert(engineIntrinsics, mod.ToString(), typeof(PSModuleInfo)));
                }
                return moduleList.ToArray();
            }

            return inputData;
        }
    }
}