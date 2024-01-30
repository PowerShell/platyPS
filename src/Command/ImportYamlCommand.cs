// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using YamlDotNet;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    /// <summary>
    /// Import a yaml command help file.
    /// </summary>
    [Cmdlet(VerbsData.Import, "YamlCommandHelp", HelpUri = "", DefaultParameterSetName = "FromPath")]
    [OutputType(typeof(Dictionary<object, object>))]
    public sealed class ImportYamlMetadataCommand : PSCmdlet
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
        /// Yaml content provided as a string.
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
                    var result = yamlDeserializer?.Deserialize<CommandHelp>(Yaml);
                    WriteObject(result);
                }
            }
            else if (string.Equals(this.ParameterSetName, "FromPath", StringComparison.OrdinalIgnoreCase))
            {
                foreach (string filePath in Path)
                {
                    Collection<PathInfo> resolvedPaths = this.SessionState.Path.GetResolvedPSPathFromPSPath(filePath);

                    foreach (var resolvedPath in resolvedPaths)
                    {
                        var result = yamlDeserializer?.Deserialize<object>(File.ReadAllText(resolvedPath.Path));
                        // TODO: this should really be a CommandHelp object, but that has yet to be coded.
                        WriteObject(result);
                    }
                }
            }
        }
    }
}
