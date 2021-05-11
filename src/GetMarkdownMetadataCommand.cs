// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;

namespace Microsoft.PowerShell.PlatyPS
{
    /// <summary>
    /// Get-MarkdownMetadata reads the YAML header from a markdown file and represents it as a Dictionary object.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MarkdownMetadata", HelpUri = "", DefaultParameterSetName = "FromPath")]
    [OutputType(typeof(Dictionary<object, object>))]
    public sealed class GetMarkdownMetadataCommand : PSCmdlet
    {
        #region cmdlet parameters

        /// <summary>
        /// An array of paths to get the markdown metadata from.
        /// </summary>
        [Parameter(
            Mandatory = true,
            ParameterSetName = "FromPath",
            ValueFromPipelineByPropertyName = true,
            Position = 1)]
        [SupportsWildcards]
        public string[] Path { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Markdown content provided as a string.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "FromMarkdownString")]
        [ValidateNotNullOrEmpty()]
        public string? Markdown { get; set; }

        #endregion

        protected override void ProcessRecord()
        {
            if (string.Equals(this.ParameterSetName, "FromMarkdownString", StringComparison.OrdinalIgnoreCase))
            {
                if (Markdown is not null)
                {
                    DeserializeAndWrite(GetMarkdownMetadataHeaderReader(Markdown));
                }
            }
            else if (string.Equals(this.ParameterSetName, "FromPath", StringComparison.OrdinalIgnoreCase))
            {
                foreach (string filePath in Path)
                {
                    Collection<PathInfo> resolvedPaths = this.SessionState.Path.GetResolvedPSPathFromPSPath(filePath);

                    foreach (var resolvedPath in resolvedPaths)
                    {
                        DeserializeAndWrite(GetMarkdownMetadataHeaderReader(File.ReadAllText(resolvedPath.Path)));
                    }
                }
            }
        }

        private void DeserializeAndWrite(string headerContent)
        {
            StringReader stringReader = new StringReader(headerContent);

            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder().Build();
            WriteObject(deserializer.Deserialize(stringReader));
        }

        private string GetMarkdownMetadataHeaderReader(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return string.Empty;
            }

            var mdAst = Markdig.Markdown.Parse(content);

            if (mdAst.Count < 2)
            {
                return string.Empty;
            }

            if (mdAst[0] is Markdig.Syntax.ThematicBreakBlock)
            {
                if (mdAst[1] is Markdig.Syntax.HeadingBlock metadata)
                {
                    if (metadata.Inline?.FirstChild is Markdig.Syntax.Inlines.LiteralInline metadataText)
                    {
                        return metadataText.Content.Text;
                    }
                }
            }

            return string.Empty;
        }
    }
}
