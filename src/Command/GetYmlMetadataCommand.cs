// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

using YamlDotNet;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Microsoft.PowerShell.PlatyPS
{
    /// <summary>
    /// Cmdlet to generate the markdown help for commands, all commands in a module or from a MAML file.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "YamlMetadata", HelpUri = "https://go.microsoft.com/fwlink/?LinkID=2096483")]
    [OutputType(typeof(FileInfo[]))]
    public sealed class GetYamlMetadataCommand : PSCmdlet
    {
        #region cmdlet parameters

        /// <summary>
        /// An array of paths to get the markdown metadata from.
        /// </summary>
        [Parameter(
            Mandatory = true,
            ParameterSetName = "FromPath",
            ValueFromPipelineByPropertyName = true,
            Position = 0)]
        [SupportsWildcards]
        public string[] Path { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Markdown content provided as a string.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "FromYamlString")]
        [ValidateNotNullOrEmpty()]
        public string? Yaml { get; set; }

        private IDeserializer? yamlDeserializer;
        #endregion

        protected override void BeginProcessing()
        {
            yamlDeserializer = new DeserializerBuilder().Build();
        }

        protected override void ProcessRecord()
        {
            if (string.Equals(this.ParameterSetName, "FromYamlString", StringComparison.OrdinalIgnoreCase))
            {
                if (Yaml is not null)
                {
                    DeserializeAndWrite(Yaml);
                }
            }
            else if (string.Equals(this.ParameterSetName, "FromPath", StringComparison.OrdinalIgnoreCase))
            {
                foreach (string filePath in Path)
                {
                    Collection<PathInfo> resolvedPaths = this.SessionState.Path.GetResolvedPSPathFromPSPath(filePath);

                    foreach (var resolvedPath in resolvedPaths)
                    {
                        DeserializeAndWrite(File.ReadAllText(resolvedPath.Path));
                    }
                }
            }
        }

        private void DeserializeAndWrite(string yamlContent)
        {
            var result = yamlDeserializer?.Deserialize<object>(yamlContent);
            var resultDict = result as IDictionary;
            if (resultDict is not null)
            {
                if (resultDict.Contains("metadata"))
                {
                    var metadata = resultDict["metadata"];
                    WriteObject(metadata);
                }
            }
        }
    }
}
