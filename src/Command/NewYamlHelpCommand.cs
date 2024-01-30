// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;

using Microsoft.PowerShell.PlatyPS.YamlWriter;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    /// <summary>
    /// Cmdlet to generate the Yaml help files.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "YamlHelp", HelpUri = "")]
    [OutputType(typeof(FileInfo[]))]
    public sealed class NewYamlHelpCommand : PSCmdlet
    {

        #region parameters
        [Parameter(Mandatory=true, Position=1, ValueFromPipeline=true, ValueFromPipelineByPropertyName=true)]
        public string[]? Path { get; set; }

        [Parameter(Mandatory=true)]
        public string? OutputFolder { get; set; }

        [Parameter]
        public Encoding Encoding { get; set;} = Encoding.UTF8;

        [Parameter]
        SwitchParameter Force { get; set; }
        #endregion

        private string fullPath = string.Empty;
        private List<CommandHelp> CommandHelpCollection = new List<CommandHelp>();
        // private TransformSettings transformSettings = null;

        protected override void BeginProcessing()
        {
            ProviderInfo provider;
            var resolvedOutputFolder = this.SessionState.Path.GetResolvedProviderPathFromPSPath(OutputFolder, out provider);
            if (resolvedOutputFolder is null)
            {
                if (WildcardPattern.ContainsWildcardCharacters(OutputFolder))
                {
                    // This error message will only be shown if the user specifies a wildcard path that doesn't resolve to a directory.
                    ThrowTerminatingError(new ErrorRecord(new ArgumentException("OutputFolder must not include wildcards"), "WildcardInOutputFolder", ErrorCategory.InvalidArgument, OutputFolder));
                }
                else
                {
                    try
                    {
                        fullPath = this.SessionState.Path.GetUnresolvedProviderPathFromPSPath(OutputFolder);
                        Directory.CreateDirectory(fullPath);
                    }
                    catch (Exception e)
                    {
                        ThrowTerminatingError(new ErrorRecord(e, "WildcardInOutputFolder", ErrorCategory.InvalidOperation, fullPath));
                    }
                }
            }
            else
            {
                if (provider.Name != "FileSystem")
                {
                    ThrowTerminatingError(new ErrorRecord(new ArgumentException("OutputFolder must be a directory"), "OutputNotInFileSystem", ErrorCategory.InvalidArgument, OutputFolder));
                }
                else if (resolvedOutputFolder.Count > 1)
                {
                    ThrowTerminatingError(new ErrorRecord(new ArgumentException("OutputFolder must be a single path"), "OutputIsMultiplePaths", ErrorCategory.InvalidArgument, OutputFolder));
                }
                else if (File.Exists(resolvedOutputFolder[0]))
                {
                    ThrowTerminatingError(new ErrorRecord(new ArgumentException("OutputFolder must be a directory"), "OutputFolderIsFile", ErrorCategory.InvalidArgument, OutputFolder));
                }
                else
                {
                    fullPath = resolvedOutputFolder[0];
                }
            }
        }

        protected override void ProcessRecord()
        {
            if (Path is null)
            {
                return;
            }

            foreach (var path in Path)
            {
                var resolvedPaths = this.SessionState.Path.GetResolvedProviderPathFromPSPath(path, out _);
                foreach (var resolvedPath in resolvedPaths)
                {
                    foreach(var markdownFile in GetMarkdownFilesFromPath(resolvedPath))
                    {
                        // parse the markdown and create the CommandHelp object
                        // if the file can't be parsed emit an error an continue
                        try
                        {
                            WriteVerbose($"Parsing markdown file {markdownFile}");
                            CommandHelp cHelp = (CommandHelp)MarkdownConverter.GetCommandHelpFromMarkdownFile(markdownFile);
                            CommandHelpCollection.Add(cHelp);
                        }
                        catch (Exception e)
                        {
                            WriteError(new ErrorRecord(e, "MarkdownFileParserError", ErrorCategory.InvalidData, markdownFile));
                        }
                    }
                }
            }
        }

        private IEnumerable<string> GetMarkdownFilesFromPath(string resolvedPath)
        {
            if (Directory.Exists(resolvedPath))
            {
                foreach (var file in Directory.EnumerateFiles(resolvedPath, "*.md", SearchOption.AllDirectories))
                {
                    yield return file;
                }
            }
            else if (File.Exists(resolvedPath))
            {
                yield return resolvedPath;
            }
            else
            {
                WriteError(new ErrorRecord(new FileNotFoundException($"File or directory not found: {resolvedPath}"), "FileNotFound", ErrorCategory.ObjectNotFound, resolvedPath));
            }
        }

        protected override void EndProcessing()
        {
            foreach(var commandHelpObject in CommandHelpCollection)
            {
                if (OutputFolder is null)
                {
                    continue;
                }

                var settings = new CommandHelpWriterSettings(Encoding, OutputFolder);
                using (var writer = new CommandHelpYamlWriter(settings))
                {
                    writer.Write(commandHelpObject, metadata: null);
                }
            }
        }
    }
}
