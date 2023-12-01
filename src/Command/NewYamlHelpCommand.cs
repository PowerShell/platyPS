using System.Net;
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
    /// Cmdlet to generate the markdown help for commands, all commands in a module or from a MAML file.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "YamlHelp", HelpUri = "https://go.microsoft.com/fwlink/?LinkID=2096483")]
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
                throw new ArgumentException("OutputFolder must be a single path");
            }
            else
            {
                Directory.CreateDirectory(OutputFolder);
            }
        }
        else
        {
            if (provider.Name != "FileSystem")
            {
                throw new ArgumentException("OutputFolder must be a directory");
            }
            else if (resolvedOutputFolder.Count > 1)
            {
                throw new ArgumentException("OutputFolder must be a single path");
            }
            else if (File.Exists(resolvedOutputFolder[0]))
            {
                throw new ArgumentException("OutputFolder must be a directory");
            }
            else
            {
                fullPath = resolvedOutputFolder[0];
            }
            Directory.CreateDirectory(fullPath);
        }

        /*
        transformSettings = new TransformSettings
        {
            AlphabeticParamsOrder = AlphabeticParamsOrder,
            CreateModulePage = WithModulePage,
            DoubleDashList = ConvertDoubleDashLists,
            ExcludeDontShow = ExcludeDontShow,
            FwLink = FwLink,
            HelpVersion = HelpVersion,
            Locale = Locale is null ? CultureInfo.GetCultureInfo("en-US") : new CultureInfo(Locale),
            ModuleGuid = ModuleGuid is null ? null : Guid.Parse(ModuleGuid),
            ModuleName = ModuleName,
            OnlineVersionUrl = OnlineVersionUrl,
            Session = Session,
            UseFullTypeName = UseFullTypeName
        };
        */

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
                        var cHelp = ParseMarkdownFile(markdownFile);
                        CommandHelpCollection.Add(cHelp);
                    }
                    catch (Exception e)
                    {
                        WriteError(new ErrorRecord(e, "Failed to parse markdown file", ErrorCategory.InvalidData, markdownFile));
                    }
                }
            }
        }
    }

    private CommandHelp ParseMarkdownFile(string markdownFile)
    {
        var cHelp = new CommandHelp();
        var mdAst = Markdig.Markdown.Parse(File.ReadAllText(markdownFile));
        return cHelp;
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
                writer.Write(commandHelpObject, noMetadata: false, metadata: null);
            }
        }
    }


        /*
        protected override void EndProcessing()
        {
            Collection<CommandHelp>? cmdHelpObjs = null;

            try
            {
                if (string.Equals(this.ParameterSetName, "FromCommand", StringComparison.OrdinalIgnoreCase))
                {
                    if (Command.Length > 0)
                    {
                        cmdHelpObjs = new TransformCommand(transformSettings).Transform(Command);
                    }
                }
                else if (string.Equals(this.ParameterSetName, "FromModule", StringComparison.OrdinalIgnoreCase))
                {
                    if (Module.Length > 0)
                    {
                        cmdHelpObjs = new TransformModule(transformSettings).Transform(Module);
                    }
                }
                else if (string.Equals(this.ParameterSetName, "FromMaml", StringComparison.OrdinalIgnoreCase))
                {
                    if (MamlFile.Length > 0)
                    {
                        cmdHelpObjs = new TransformMaml(transformSettings).Transform(MamlFile);
                    }
                }
            }
            catch (ItemNotFoundException infe)
            {
                var exception = new ItemNotFoundException(string.Format(Microsoft_PowerShell_PlatyPS_Resources.ModuleNotFound, infe.Message));
                ErrorRecord err = new ErrorRecord(exception, "ModuleNotFound", ErrorCategory.ObjectNotFound, infe.Message);
                ThrowTerminatingError(err);
            }
            catch (CommandNotFoundException cnfe)
            {
                var exception = new CommandNotFoundException(string.Format(Microsoft_PowerShell_PlatyPS_Resources.CommandNotFound, cnfe.CommandName));
                ErrorRecord err = new ErrorRecord(exception, "CommandNotFound", ErrorCategory.ObjectNotFound, cnfe.CommandName);
                ThrowTerminatingError(err);
            }
            catch (FileNotFoundException fnfe)
            {
                var exception = new CommandNotFoundException(string.Format(Microsoft_PowerShell_PlatyPS_Resources.FileNotFound, fnfe.FileName));
                ErrorRecord err = new ErrorRecord(exception, "FileNotFound", ErrorCategory.ObjectNotFound, fnfe.FileName);
                ThrowTerminatingError(err);
            }

            if (cmdHelpObjs != null)
            {
                foreach (var cmdletHelp in cmdHelpObjs)
                {
                    var settings = new CommandHelpWriterSettings(Encoding, $"{fullPath}{Constants.DirectorySeparator}{cmdletHelp.Title}.{Constants.YamlExtension}");
                    using var cmdWrt = new CommandHelpYamlWriter(settings);
                    WriteObject(cmdWrt.Write(cmdletHelp, NoMetadata, Metadata));
                }

                if (WithModulePage)
                {
                    string modulePagePath = ModulePagePath ?? fullPath;

                    string resolvedPathModulePagePath = this.SessionState.Path.GetUnresolvedProviderPathFromPSPath(modulePagePath);

                    var modulePageSettings = new YamlWriterSettings(Encoding, resolvedPathModulePagePath);
                    using var modulePageWriter = new ModulePageWriter(modulePageSettings);

                    WriteObject(modulePageWriter.Write(cmdHelpObjs));
                }
            }
        }
        */
    }
}

