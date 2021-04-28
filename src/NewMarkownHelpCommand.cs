using Microsoft.PowerShell.PlatyPS.MarkdownWriter;
using Microsoft.PowerShell.PlatyPS.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.PowerShell.PlatyPS.Tests,PublicKey=0024000004800000940000000602000000240000525341310004000001000100b5fc90e7027f67871e773a8fde8938c81dd402ba65b9201d60593e96c492651e889cc13f1415ebb53fac1131ae0bd333c5ee6021672d9718ea31a8aebd0da0072f25d87dba6fc90ffd598ed4da35e44c398c454307e8e33b8426143daec9f596836f97c8f74750e5975c64e2189f45def46b2a2b1247adc3652bf5c308055da9")]
namespace Microsoft.PowerShell.PlatyPS
{
    [Cmdlet(VerbsCommon.New, "MarkdownHelp", HelpUri="https://go.microsoft.com/fwlink/?LinkID=2096483")]
    [OutputType(typeof(FileInfo[]))]
    public sealed class NewMarkdownHelpCommand : PSCmdlet
    {
        #region cmdlet parameters

        [Parameter(Mandatory = true, ParameterSetName="FromCommand")]
        public string[]? Command { get; set; }

        [Parameter()]
        public System.Text.Encoding Encoding { get; set; } = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        [Parameter()]
        public SwitchParameter Force { get; set; }

        [Parameter(ParameterSetName="FromModule")]
        [Parameter(ParameterSetName="FromMaml")]
        public string? FwLink { get; set; }

        [Parameter(ParameterSetName="FromModule")]
        [Parameter(ParameterSetName="FromMaml")]
        public string? HelpVersion { get; set; }

        [Parameter(ParameterSetName="FromModule")]
        [Parameter(ParameterSetName="FromMaml")]
        public string? Locale { get; set; }

        [Parameter(Mandatory= true, ParameterSetName="FromMaml")]
        public string[]? MamlFile { get; set; }

        [Parameter()]
        public Hashtable? Metadata { get; set; }

        [Parameter(Mandatory=true, ValueFromPipeline=true, ParameterSetName="FromModule")]
        public string[]? Module { get; set; }

        [Parameter(ParameterSetName="FromMaml")]
        public string? ModuleGuid { get; set; }

        [Parameter(ParameterSetName="FromMaml")]
        public string? ModuleName { get; set; }

        [Parameter()]
        public SwitchParameter NoMetadata { get; set; }

        [Parameter(ParameterSetName="FromCommand")]
        public string? OnlineVersionUrl { get; set; }

        [Parameter(Mandatory=true)]
        public string? OutputFolder { get; set; }

        [Parameter(ParameterSetName="FromModule")]
        [Parameter(ParameterSetName="FromMaml")]
        public SwitchParameter WithModulePage { get; set; }

        [Parameter(ParameterSetName="FromMaml")]
        public SwitchParameter ConvertNotesToList { get; set; }

        [Parameter(ParameterSetName="FromMaml")]
        public SwitchParameter ConvertDoubleDashLists { get; set; }

        [Parameter()]
        public SwitchParameter AlphabeticParamsOrder { get; set; } = true;

        [Parameter()]
        public SwitchParameter UseFullTypeName { get; set; }

        [Parameter(ParameterSetName="FromModule")]
        [Parameter(ParameterSetName="FromCommand")]
        public PSSession? Session { get; set;}

        [Parameter(ParameterSetName="FromModule")]
        [Parameter(ParameterSetName="FromMaml")]
        public string? ModulePagePath { get; set; }

        public SwitchParameter ExcludeDontShow { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            if (Metadata is not null && NoMetadata)
            {
                var exception = new InvalidOperationException(Microsoft_PowerShell_PlatyPS_Resources.NoMetadataAndMetadata);
                ErrorRecord err = new ErrorRecord(exception, "NoMetadataAndMetadata", ErrorCategory.InvalidOperation, Metadata);
                ThrowTerminatingError(err);
            }
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
        }

        protected override void EndProcessing()
        {
            if (OutputFolder is not null && !Directory.Exists(OutputFolder))
            {
                Directory.CreateDirectory(OutputFolder);
            }

            List<FileInfo> writtentFileList = new();

            Collection<CommandHelp>? cmdHelpObjs = null;

            TransformSettings transformSettings = new TransformSettings
            {
                AlphabeticParamsOrder = AlphabeticParamsOrder,
                CreateModulePage = WithModulePage,
                DoubleDashList = ConvertDoubleDashLists,
                ExcludeDontShow = ExcludeDontShow,
                FwLink = FwLink,
                HelpVersion = HelpVersion,
                Locale = Locale is not null ? new CultureInfo(Locale) : CultureInfo.GetCultureInfo("en-US"),
                ModuleGuid = ModuleGuid is not null ? Guid.Parse(ModuleGuid) : null,
                ModuleName = ModuleName,
                OnlineVersionUrl = OnlineVersionUrl,
                Session = Session,
                UseFullTypeName = UseFullTypeName
            };

            if (string.Equals(this.ParameterSetName, "FromCommand", StringComparison.OrdinalIgnoreCase))
            {
                if (Command?.Length > 0)
                {
                    cmdHelpObjs = new TransformCommand(transformSettings).Transform(Command);
                }
            }
            else if (string.Equals(this.ParameterSetName, "FromModule", StringComparison.OrdinalIgnoreCase))
            {
                if (Module?.Length > 0)
                {
                    cmdHelpObjs = new TransformModule(transformSettings).Transform(Module);
                }
            }
            else if (string.Equals(this.ParameterSetName, "FromMaml", StringComparison.OrdinalIgnoreCase))
            {
                if (MamlFile?.Length > 0)
                {
                    cmdHelpObjs = new TransformMaml(transformSettings).Transform(MamlFile);
                }
            }

            if (cmdHelpObjs != null)
            {
                foreach (var cmdletHelp in cmdHelpObjs)
                {
                    MarkdownWriterSettings settings = new MarkdownWriterSettings(Encoding, $"{OutputFolder}{Constants.PathSeparator}{cmdletHelp.Title}.md");
                    CommandHelpMarkdownWriter cmdWrt = new(settings);
                    writtentFileList.Add(cmdWrt.Write(cmdletHelp, NoMetadata, Metadata));
                }

                if (WithModulePage)
                {
                    string? modulePagePath;

                    if (ModulePagePath is null && OutputFolder is not null)
                    {
                        modulePagePath = OutputFolder;
                    }
                    else if (ModulePagePath is not null)
                    {
                        modulePagePath = ModulePagePath;
                    }
                    else
                    {
                        throw new ArgumentNullException("ModulePagePath is null");
                    }

                    ModulePageWriter modulePageWriter = new(modulePagePath, Encoding);
                    writtentFileList.Add(modulePageWriter.Write(cmdHelpObjs));
                }

                WriteObject(writtentFileList);
            }
        }
    }
}
