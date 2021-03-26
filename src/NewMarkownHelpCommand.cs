using Microsoft.PowerShell.PlatyPS.MarkdownWriter;
using Microsoft.PowerShell.PlatyPS.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Microsoft.PowerShell.PlatyPS
{
    [Cmdlet(VerbsCommon.New, "MarkdownHelp", HelpUri="https://go.microsoft.com/fwlink/?LinkID=2096483")]
    [OutputType(typeof(FileInfo[]))]
    public sealed class NewMarkdownHelpCommand : PSCmdlet
    {
        public NewMarkdownHelpCommand()
        {
        }

        #region cmdlet parameters

        [Parameter(Mandatory = true, ParameterSetName="FromCommand")]
        public string[] Command { get; set; }

        [Parameter()]
        public System.Text.Encoding Encoding { get; set; } = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        [Parameter()]
        public SwitchParameter Force { get; set; }

        [Parameter(ParameterSetName="FromModule")]
        [Parameter(ParameterSetName="FromMaml")]
        public string FwLink { get; set; }

        [Parameter(ParameterSetName="FromModule")]
        [Parameter(ParameterSetName="FromMaml")]
        public string HelpVersion { get; set; }

        [Parameter(ParameterSetName="FromModule")]
        [Parameter(ParameterSetName="FromMaml")]
        public string Locale { get; set; }

        [Parameter(Mandatory= true, ParameterSetName="FromMaml")]
        public string[] MamlFile { get; set; }

        public Hashtable Metadata { get; set; }

        [Parameter(Mandatory=true, ValueFromPipeline=true, ParameterSetName="FromModule")]
        public string[] Module { get; set; }

        [Parameter(ParameterSetName="FromMaml")]
        public string ModuleGuid { get; set; }

        [Parameter(ParameterSetName="FromMaml")]
        public string ModuleName { get; set; }

        [Parameter()]
        public SwitchParameter NoMetadata { get; set; }

        [Parameter(ParameterSetName="FromCommand")]
        public string OnlineVersionUrl { get; set; } = string.Empty;

        [Parameter(Mandatory=true)]
        public string OutputFolder { get; set; }

        [Parameter(ParameterSetName="FromModule")]
        [Parameter(ParameterSetName="FromMaml")]
        public SwitchParameter WithModulePage { get; set; }

        [Parameter(ParameterSetName="FromMaml")]
        public SwitchParameter ConvertNotesToList { get; set; }

        [Parameter(ParameterSetName="FromMaml")]
        public SwitchParameter ConvertDoubleDashLists { get; set; }

        [Parameter()]
        public SwitchParameter AlphabeticParamsOrder { get; set; }

        [Parameter()]
        public SwitchParameter UseFullTypeName { get; set; }

        [Parameter(ParameterSetName="FromModule")]
        [Parameter(ParameterSetName="FromCommand")]
        public PSSession Session { get; set;}

        [Parameter(ParameterSetName="FromModule")]
        [Parameter(ParameterSetName="FromMaml")]
        public string ModulePagePath { get; set; }

        public SwitchParameter ExcludeDontShow { get; set; }

        #endregion

        protected override void BeginProcessing()
        {

        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
        }

        protected override void EndProcessing()
        {
            if (!Directory.Exists(OutputFolder))
            {
                Directory.CreateDirectory(OutputFolder);
            }

            CommandHelp cmdHlp = GetMockCommandHelp();

            //CommandHelpMarkdownWriter cmdWrt = new CommandHelpMarkdownWriter(@"D:\temp\Get-Help.md", cmdHlp);
            //cmdWrt.Write();

            List<FileInfo> writtentFileList = new();

            if (string.Equals(this.ParameterSetName, "FromCommand", StringComparison.OrdinalIgnoreCase))
            {
                TransformCommand transformCommand = new(session: Session);

                foreach (var cmdletHelp in transformCommand.Transform(Command))
                {
                    CommandHelpMarkdownWriter cmdWrt = new($"{OutputFolder}\\{cmdletHelp.Title}.md", cmdletHelp);
                    writtentFileList.Add(cmdWrt.Write());
                }
            }

            if (string.Equals(this.ParameterSetName, "FromModule", StringComparison.OrdinalIgnoreCase))
            {
                TransformModule transformModule = new(session: Session);

                foreach (var cmdletHelp in transformModule.Transform(Module))
                {
                    CommandHelpMarkdownWriter cmdWrt = new($"{OutputFolder}\\{cmdletHelp.Title}.md", cmdletHelp);
                    writtentFileList.Add(cmdWrt.Write());
                }
            }

            if (string.Equals(this.ParameterSetName, "FromMaml", StringComparison.OrdinalIgnoreCase))
            {
                TransformMaml transformMaml = new(session: Session);

                foreach (var cmdletHelp in transformMaml.Transform(MamlFile))
                {
                    CommandHelpMarkdownWriter cmdWrt = new($"{OutputFolder}\\{cmdletHelp.Title}.md", cmdletHelp);
                    writtentFileList.Add(cmdWrt.Write());
                }
            }

            WriteObject(writtentFileList);
        }

        private static CommandHelp GetMockCommandHelp()
        {
            CommandHelp cmdHlp = new();
            cmdHlp.Title = "Get-Help";
            cmdHlp.Synopsis = "Displays information about PowerShell commands and concepts.";

            var nameParam = new Parameter {
                Name = "Name",
                Type = "string",
                Required = true,
                Position = "Named",
                DefaultValue = "None",
                PipelineInput = false,
                Description = @"Displays help only for items in the specified category and their aliases.
Conceptual articles are in the HelpFile category."
            };

            nameParam.AddAcceptedValueRange(new string[] { "Alias", "Cmdlet", "Provider" });
            nameParam.AddRequiredParameterSetsRange(true, new string[] { "a", "b" });
            nameParam.AddRequiredParameterSetsRange(true, new string[] { "c" });
            nameParam.AddParameterSetsRange(new string[] { "a", "b", "c" });

            var pathParam = new Parameter {
                Name = "Path",
                Type = "string",
                Required = false,
                Position = "0",
                DefaultValue = "None",
                PipelineInput = true,
                Description = @"Displays commands with the specified component value, such as Exchange .
Enter a component name.
Wildcard characters are permitted.
This parameter has no effect on displays of conceptual ( About_ ) help."
            };

            pathParam.AddRequiredParameterSetsRange(true, new string[] { "a" });
            pathParam.AddParameterSet("a");

            SyntaxItem syntax1 = new("Get-Help", "AllUsersView", isDefaultParameterSet: true);
            syntax1.AddParameter(pathParam);
            syntax1.AddParameter(nameParam);

            SyntaxItem syntax2 = new ("Get-Help", "DetailedView", isDefaultParameterSet: false);
            syntax2.AddParameter(pathParam);
            syntax2.AddParameter(nameParam);

            cmdHlp.AddSyntaxItem(syntax1);
            cmdHlp.AddSyntaxItem(syntax2);

            cmdHlp.AddParameter(pathParam);
            cmdHlp.AddParameter(nameParam);

            cmdHlp.Description = @"The \`Get-Help\` cmdlet displays information about PowerShell concepts and commands, including cmdlets, functions, Common Information Model (CIM) commands, workflows, providers, aliases, and scripts.

To get help for a PowerShell cmdlet, type \`Get-Help\` followed by the cmdlet name, such as: \`Get-Help Get-Process\`.

Conceptual help articles in PowerShell begin with about_ , such as about_Comparison_Operators .
To see all about_ articles, type \`Get-Help about_*\`.
To see a particular article, type \`Get-Help about_\<article-name\>\`, such as \`Get-Help about_Comparison_Operators\`.";

            string code = @"Get-Help Format-Table
Get-Help -Name Format-Table
Format-Table -?";

            string remarks = @"\`Get-Help \<cmdlet-name\>\` is the simplest and default syntax of \`Get-Help\` cmdlet.
You can omit the Name parameter.

The syntax \`\<cmdlet-name\> -?\` works only for cmdlets.";

            cmdHlp.AddExampleItem(new Example() { Title = "Display basic help information about a cmdlet", Code = code, Remarks = remarks });

            InputOutput input = new();
            input.AddInputOutputItem("System.String", "Some string parameter");

            InputOutput output = new();
            output.AddInputOutputItem("System.String", "Some string parameter");
            output.AddInputOutputItem("MamlCommandHelpInfo", @"If you get a command that has a help file, \`Get-Help\` returns a MamlCommandHelpInfo object.");
            cmdHlp.AddInputItem(input);
            cmdHlp.AddOutputItem(output);

            return cmdHlp;
        }
    }
}
