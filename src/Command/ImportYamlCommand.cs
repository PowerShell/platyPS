// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Management.Automation;
using YamlDotNet;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Microsoft.PowerShell.PlatyPS.Model;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Microsoft.PowerShell.PlatyPS
{
    /// <summary>
    /// Import a yaml command help file.
    /// </summary>
    [Cmdlet(VerbsData.Import, "YamlCommandHelp", HelpUri = "", DefaultParameterSetName = "Path")]
    [OutputType(typeof(OrderedDictionary))]
    [OutputType(typeof(CommandHelp))]
    public sealed class ImportYamlMetadataCommand : PSCmdlet
    {
        #region cmdlet parameters

        [Parameter]
        public SwitchParameter AsDictionary { get; set; }

        /// <summary>
        /// An array of paths to get the markdown metadata from.
        /// </summary>
        [Parameter(
            Mandatory = true,
            ParameterSetName = "Path",
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = true,
            Position = 0)]
        [SupportsWildcards]
        [Alias("FullName")]
        public string[] Path
        {
            get
            {
                return _paths;
            }
            set
            {
                _paths = value;
            }
        }

        /// <summary>
        /// An array of literal paths to get the markdown metadata from.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "LiteralPath")]
        public string[] LiteralPath
        {
            get
            {
                return _paths;
            }
            set
            {
                useLiteralPath = true;
                _paths = value;
            }
        }

        private string[] _paths = Array.Empty<string>();

        private bool useLiteralPath { get; set; }

        private IDeserializer? yamlDeserializer;
        #endregion

        protected override void BeginProcessing()
        {
            yamlDeserializer = new DeserializerBuilder().Build();
        }

        protected override void ProcessRecord()
        {
            foreach (string filePath in _paths)
            {
                Collection<string>? resolvedPaths = resolvePaths(filePath);

                if (resolvedPaths is null)
                {
                    WriteError(new ErrorRecord(
                        new FileNotFoundException(filePath),
                        "ImportYamlMetadata:InvalidPath",
                        ErrorCategory.ObjectNotFound,
                        filePath
                        ));
                    continue;
                }

                foreach (var resolvedPath in resolvedPaths)
                {
                    OrderedDictionary? result = null;
                    try
                    {
                        result = yamlDeserializer?.Deserialize<OrderedDictionary>(File.ReadAllText(resolvedPath));
                    }
                    catch(Exception serializationFailure)
                    {
                        WriteError(new ErrorRecord(serializationFailure, "ImportYamlCommandHelp,SerializationFailure", ErrorCategory.OperationStopped, resolvedPath));
                        continue;
                    }

                    if (result is null)
                    {
                        WriteError(new ErrorRecord(new SerializationException(), "ImportYamlCommandHelp,SerializationNullResult", ErrorCategory.OperationStopped, resolvedPath));
                        continue;
                    }

                    var metadata = result["metadata"] as IDictionary<object, object>;
                    if (metadata is null || metadata.ContainsKey("Module Guid"))
                    {
                        WriteError(new ErrorRecord(new SerializationException("File is not CommandHelp."), "ImportYamlCommandHelp,FileNotCommandHelp", ErrorCategory.OperationStopped, resolvedPath));
                        continue;
                    }

                    if (AsDictionary)
                    {
                        WriteObject(result);
                    }
                    else
                    {
                        try
                        {
                            var shadow = YamlUtils.ConvertDictionaryToCommandHelp(result);
                            WriteObject(shadow);
                        }
                        catch (Exception e)
                        {
                            WriteError(new ErrorRecord(e, "ImportYamlCommandHelp,ConversionError", ErrorCategory.InvalidData, resolvedPath));
                        }
                    }
                }
            }
        }

        private Collection<string>? resolvePaths(string filePath)
        {
            Collection<string> paths = new();
            if (useLiteralPath)
            {
                var fi = new FileInfo(this.SessionState.Path.GetUnresolvedProviderPathFromPSPath(filePath));
                if (fi.Exists)
                {
                    paths.Add(fi.FullName);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                foreach(var pi in this.SessionState.Path.GetResolvedPSPathFromPSPath(filePath))
                {
                    paths.Add(pi.Path);
                }
            }

            return paths;
        }
    }
}
