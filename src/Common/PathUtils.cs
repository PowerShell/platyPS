// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Management.Automation;
using System.Runtime.InteropServices;

namespace Microsoft.PowerShell.PlatyPS
{
    public class PathUtils
    {
        public static List<string> ResolvePath(PSCmdlet cmdlet, string[] paths, bool isLiteralPath)
        {
            OrderedSet<string> resolvedPaths;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                resolvedPaths = new OrderedSet<string>(StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                resolvedPaths = new OrderedSet<string>(StringComparer.Ordinal);
            }

            ProviderInfo provider;
            PSDriveInfo drive;

            foreach (string path in paths)
            {
                // Try to resolve the path.
                List<string> filePaths = new List<string>();
                if (isLiteralPath)
                {
                    filePaths.Add(cmdlet.SessionState.Path.GetUnresolvedProviderPathFromPSPath(path, out provider, out drive));
                }
                else
                {
                    filePaths.AddRange(cmdlet.SessionState.Path.GetResolvedProviderPathFromPSPath(path, out provider));
                }

                if (provider.Name != "FileSystem")
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "ProviderNotFileSystem", provider.Name));
                }

                foreach (var filePath in filePaths)
                {
                    var fileInfo = new FileInfo(filePath);
                    if (! fileInfo.Exists)
                    {
                        throw new FileNotFoundException(string.Format(CultureInfo.InvariantCulture, "FileNotFound", filePath));
                    }
                    resolvedPaths.Add(filePath);
                }
            }

            return new List<string>(resolvedPaths);
        }
    }
}
