using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;

namespace Microsoft.PowerShell.PlatyPS
{
    /// <summary>
    /// Get-MarkdownMetadata reads the YAML header from a markdown file and represents it as an object as a Dictionary
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
        public string[]? Path { get; set; }

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
                if (Path is not null)
                {
                    foreach (string filePath in Path)
                    {
                        if (System.Management.Automation.WildcardPattern.ContainsWildcardCharacters(filePath))
                        {
                            FileInfo? fInfo = new FileInfo(filePath);

                            string? directoryPath = fInfo?.Directory?.FullName;
                            string? directoryName = fInfo?.Name;

                            if (directoryPath is not null && directoryName is not null)
                            {
                                foreach (string file in Directory.EnumerateFiles(directoryPath, directoryName))
                                {
                                    DeserializeAndWrite(GetMarkdownMetadataHeaderReader(File.ReadAllText(file)));
                                }
                            }
                        }
                        else if (File.Exists(filePath))
                        {
                            DeserializeAndWrite(GetMarkdownMetadataHeaderReader(File.ReadAllText(filePath)));
                        }
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
            var mdAst = Markdig.Markdown.Parse(content);

            if (mdAst.Count < 2)
            {
                return string.Empty;
            }

            if (mdAst[0] is Markdig.Syntax.ThematicBreakBlock)
            {
                if (mdAst[1] is Markdig.Syntax.HeadingBlock metadata)
                {
                    if (metadata?.Inline?.FirstChild is Markdig.Syntax.Inlines.LiteralInline metadataText)
                    {
                        return metadataText.Content.Text;
                    }
                }
            }

            return string.Empty;
        }
    }
}
