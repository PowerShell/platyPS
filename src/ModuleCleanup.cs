using System.Management.Automation;

namespace Microsoft.PowerShell.PlatyPS
{
    internal class ModuleCleanup : IModuleAssemblyCleanup
    {
        public void OnRemove(PSModuleInfo psModuleInfo)
        {
            PowerShellAPI.DisposePowerShell();
        }
    }
}
