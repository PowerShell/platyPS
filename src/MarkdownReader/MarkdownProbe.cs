// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.PowerShell.PlatyPS
{
    /// <summary>
    /// A class used for probing markdown to determine its characteristics.
    /// </summary>
    public class MarkdownProbe
    {
        /// <summary>
        /// Identify the type of platyps file based on the path.
        /// </summary>
        /// <param name="path">The path to a markdown file.</param>
        /// <returns>MarkdownProbeInfo</returns>
        public static MarkdownProbeInfo Identify(string path)
        {
            return new MarkdownProbeInfo(path);
        }
    }
}