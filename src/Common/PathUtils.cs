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

        internal static DirectoryInfo CreateOrGetOutputDirectory(PSCmdlet cmdlet, string outputDirectory, bool Force)
        {
            string fullPath = cmdlet.SessionState.Path.GetUnresolvedProviderPathFromPSPath(outputDirectory);
            var fileInfo = new FileInfo(fullPath);

            if (fileInfo.Exists && ! Force)
            {
                var exception = new InvalidOperationException($"File {fullPath} exists");
                ErrorRecord err = new ErrorRecord(exception, "FileExists", ErrorCategory.InvalidOperation, fullPath);
                cmdlet.ThrowTerminatingError(err);
            }

            DirectoryInfo? outputDir = null;

            try
            {
                outputDir = Directory.CreateDirectory(fullPath);
            }
            catch (Exception e)
            {
                ErrorRecord err = new ErrorRecord(e, "CreateDirectory", ErrorCategory.OpenError, fullPath);
                cmdlet.ThrowTerminatingError(err);
            }

            if (outputDir is null)
            {
                var exception = new InvalidOperationException("Error creating directory.");
                ErrorRecord err = new ErrorRecord(exception, "CreateDirectory", ErrorCategory.InvalidOperation, fullPath);
                cmdlet.ThrowTerminatingError(err);
                throw exception; // not reached
            }

            return outputDir;
        }
    }
}
