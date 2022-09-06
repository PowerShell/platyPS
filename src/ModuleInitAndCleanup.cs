using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Reflection;

namespace Microsoft.PowerShell.PlatyPS
{
    public class ModuleInitAndCleanup : IModuleAssemblyInitializer, IModuleAssemblyCleanup
    {
        private static readonly Assembly s_self;
        private static readonly bool s_isDotNetCore;
        private static readonly string s_dependencyFolder;
        private static readonly HashSet<string> s_dependencies;

        static ModuleInitAndCleanup()
        {
            s_self = typeof(ModuleInitAndCleanup).Assembly;
            s_isDotNetCore = Environment.Version.Major > 4;
            s_dependencyFolder = Path.Combine(Path.GetDirectoryName(s_self.Location), "Dependencies");
            s_dependencies = new(StringComparer.Ordinal);

            foreach (string filePath in Directory.EnumerateFiles(s_dependencyFolder, "*.dll"))
            {
                s_dependencies.Add(AssemblyName.GetAssemblyName(filePath).FullName);
            }
        }

        public void OnImport()
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolvingHandler;
        }

        public void OnRemove(PSModuleInfo psModuleInfo)
        {
            AppDomain.CurrentDomain.AssemblyResolve -= ResolvingHandler;
            PowerShellAPI.DisposePowerShell();
        }

        private static bool IsAssemblyMatching(AssemblyName assemblyName, Assembly? requestingAssembly)
        {
            // The requesting assembly is always available in .NET, but could be null in .NET Framework.
            // - When the requesting assembly is available, we check whether the loading request came from this
            //   module, so as to make sure we only act on the request from this module.
            // - When the requesting assembly is not available, we just have to depend on the assembly name only.
            return requestingAssembly is not null
                ? requestingAssembly == s_self && s_dependencies.Contains(assemblyName.FullName)
                : s_dependencies.Contains(assemblyName.FullName);
        }

        internal static Assembly? ResolvingHandler(object sender, ResolveEventArgs args)
        {
            var assemblyName = new AssemblyName(args.Name);
            if (IsAssemblyMatching(assemblyName, args.RequestingAssembly))
            {
                string fileName = assemblyName.Name + ".dll";
                string filePath = Path.Combine(s_dependencyFolder, fileName);

                if (File.Exists(filePath))
                {
                    // - In .NET, the 'LoadFile' API uses an anonymous 'AssemblyLoadContext' instance to load the assembly file.
                    //   But it maintains a cache to guarantee that the same assembly instance is returned for the same assembly file path.
                    //   For details, see the .NET code at https://source.dot.net/#System.Private.CoreLib/Assembly.cs,239.
                    // - In .NET Framework, assembly conflict is not a problem, so we load the assembly by 'Assembly.LoadFrom',
                    //   the same as what powershell.exe would do.
                    return s_isDotNetCore
                        ? Assembly.LoadFile(filePath)
                        : Assembly.LoadFrom(filePath);
                }
            }

            return null;
        }
    }
}
