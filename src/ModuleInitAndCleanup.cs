using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Reflection;

namespace Microsoft.PowerShell.PlatyPS
{
    public class ModuleInitAndCleanup : IModuleAssemblyInitializer, IModuleAssemblyCleanup
    {
        private static Assembly s_self = typeof(ModuleInitAndCleanup).Assembly;
        private static string s_dependencyFolder = Path.Combine(Path.GetDirectoryName(s_self.Location), "Dependencies");
        private static HashSet<string> s_dependencies = new(StringComparer.Ordinal) { "YamlDotNet", "Markdig.Signed" };

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
            var assemblyName = new AssemblyName(args.Name);
            if (args.RequestingAssembly == s_self && s_dependencies.Contains(assemblyName.Name))
            {
                string fileName = assemblyName.Name + ".dll";
                string filePath = Path.Combine(s_dependencyFolder, fileName);

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
