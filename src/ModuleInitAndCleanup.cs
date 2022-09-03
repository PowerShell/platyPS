using System;
using System.IO;
using System.Management.Automation;
using System.Reflection;

namespace Microsoft.PowerShell.PlatyPS
{
    public class ModuleInitAndCleanup : IModuleAssemblyInitializer, IModuleAssemblyCleanup
    {
        private static string dependencyFolder = Path.Combine(
            Path.GetDirectoryName(typeof(ModuleInitAndCleanup).Assembly.Location),
            "Dependencies");

        public void OnImport()
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolvingHandler;
        }

        public void OnRemove(PSModuleInfo psModuleInfo)
        {
            AppDomain.CurrentDomain.AssemblyResolve -= ResolvingHandler;
            PowerShellAPI.DisposePowerShell();
        }

        internal static Assembly? ResolvingHandler(object sender, ResolveEventArgs args)
        {
            string name = args.Name;
            if (name.Equals("YamlDotNet, Version=11.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e", StringComparison.Ordinal) ||
                name.Equals("Markdig.Signed, Version=0.18.3.0, Culture=neutral, PublicKeyToken=870da25a133885f8", StringComparison.Ordinal))
            {
                string fileName = name.Substring(0, name.IndexOf(',')) + ".dll";
                string filePath = Path.Combine(dependencyFolder, fileName);

                if (File.Exists(filePath))
                {
                    if (Environment.Version.Major > 4)
                    {
                        // In .NET, the 'LoadFile' API uses an anonymous 'AssemblyLoadContext' instance to load the assembly file.
                        // But it maintains a cache to guarantee that the same assembly instance is returned for the same assembly file path.
                        // For details, see the .NET code at https://source.dot.net/#System.Private.CoreLib/Assembly.cs,239
                        return Assembly.LoadFile(filePath);
                    }
                    else
                    {
                        // In .NET Framework, assembly conflict is not a problem, so we load the assembly by 'Assembly.LoadFrom',
                        // the same as how PowerShell would do.
                        return Assembly.LoadFrom(filePath);
                    }
                }
            }

            return null;
        }
    }
}
