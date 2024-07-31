// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.PowerShell.PlatyPS.MarkdownWriter;
using Microsoft.PowerShell.PlatyPS.Model;
using System.CodeDom;

namespace Microsoft.PowerShell.PlatyPS
{

    /// <summary>
    /// A class to represent a parse error.
    /// This might occur at any time, but especially when invalid yaml is included in the markdown.
    /// </summary>
    public class MarkdownProbe
    {
        public static MarkdownProbeInfo Identify(string path)
        {
            return new MarkdownProbeInfo(path);
        }
    }
}