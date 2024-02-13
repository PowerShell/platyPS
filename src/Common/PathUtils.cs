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
    /// <summary>
    /// This class contains utility functions for working with file paths.
    /// </summary>
    internal class PathUtils
    {
        /// <summary>
        /// This will resolve literal or wildcard paths to a list of file paths.
        /// </summary>
        /// <param name="cmdlet">The cmdlet that is calling this function.</param>
        /// <param name="paths">The paths to resolve.</param>
        /// <param name="isLiteralPath">True if the paths are literal paths, false if they are wildcard paths.</param>
        public static List<string> ResolvePath(PSCmdlet cmdlet, string[] paths, bool isLiteralPath)
        {
            // If we can keep them in order, we should.
            // An OrderedSet removes duplicates and maintains order.
            OrderedSet<string> resolvedPaths;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Windows will not be case-sensitive
                resolvedPaths = new OrderedSet<string>(StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                // Mac is case-preserving but can be case-insensitive (depending on the file system type)
                // Linux systems are case-sensitive
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
                    // This literal path will not be expanded to potentially multiple paths if wildcards are present.
                    filePaths.Add(cmdlet.SessionState.Path.GetUnresolvedProviderPathFromPSPath(path, out provider, out drive));
                }
                else
                {
                    // If wildcards are present, this will be expanded to potentially multiple paths.
                    filePaths.AddRange(cmdlet.SessionState.Path.GetResolvedProviderPathFromPSPath(path, out provider));
                }

                // If the provider is not the file system, we will not resolve the path.
                if (provider.Name != "FileSystem")
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "ProviderNotFileSystem", provider.Name));
                }

                // check for each of the files and add them to the list, if we can't find the file, throw an error.
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
