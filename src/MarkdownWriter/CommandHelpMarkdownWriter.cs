// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.PowerShell.PlatyPS;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS.MarkdownWriter
{
    /// <summary>
    /// Write the CommandHelp object to a file in markdown format.
    /// </summary>
    internal class CommandHelpMarkdownWriter : CommandHelpWriterBase
    {
        /// <summary>
        /// Initialize the writer with settings.
        /// </summary>
        /// <param name="settings">Settings needs for the markdown writer.</param>
        internal CommandHelpMarkdownWriter(WriterSettings settings)
        {
            string path = settings.DestinationPath;

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            else
            {
                _filePath = path;
                _encoding = settings.Encoding;
                sb = Constants.StringBuilderPool.Get();
            }
        }

        // Write the Metadata information.
        internal override void WriteMetadataHeader(CommandHelp help, Hashtable? metadata = null)
        {
            sb.AppendLine(Constants.MarkdownMetadataHeader);
            var mergedMetadata = MetadataUtils.MergeCommandHelpMetadataWithNewMetadata (metadata, help);
            sb.Append(YamlUtils.SerializeElement(mergedMetadata));
            sb.AppendLine(Constants.MarkdownMetadataHeader);
            sb.AppendLine();
        }

        internal override void WriteTitle(CommandHelp help)
        {
            sb.AppendLine($"# {help.Title}");
            sb.AppendLine();
        }

        internal override void WriteSynopsis(CommandHelp help)
        {
            sb.AppendLine(Constants.mdSynopsisHeader);
            sb.AppendLine();
            sb.AppendLine(help.Synopsis);
            sb.AppendLine();
        }

        internal override void WriteSyntax(CommandHelp help)
        {
            sb.AppendLine(Constants.mdSyntaxHeader);
            sb.AppendLine();

            if (help.Syntax?.Count > 0)
            {
                foreach (SyntaxItem item in help.Syntax)
                {
                    if (item.IsDefaultParameterSet)
                    {
                        sb.AppendLine(item.ToSyntaxString(Constants.mdDefaultParameterSetHeaderTemplate));
                    }
                    else
                    {
                        sb.AppendLine(item.ToSyntaxString(Constants.mdParameterSetHeaderTemplate));
                    }
                }
                // sb.AppendLine();
            }
        }

        internal override void WriteDescription(CommandHelp help)
        {
            sb.AppendLine(Constants.mdDescriptionHeader);
            sb.AppendLine();
            sb.AppendLine(help.Description);
            sb.AppendLine();
        }

        internal override void WriteAliases(CommandHelp help)
        {
            sb.AppendLine(Constants.mdAliasHeader);
            sb.AppendLine();
            sb.AppendLine(Constants.AliasMessage);
            sb.AppendLine();
        }

        internal override void WriteExamples(CommandHelp help)
        {
            sb.AppendLine(Constants.mdExamplesHeader);
            sb.AppendLine();

            if (help.Examples is null)
            {
                return;
            }

            foreach (var example in help.Examples)
            {
                sb.AppendLine($"### {example.Title}");
                sb.AppendLine();
                sb.AppendLine($"{example.Remarks}");
                sb.AppendLine();
            }

            /*
            int? totalExamples = help?.Examples?.Count;

            for (int i = 0; i < totalExamples; i++)
            {
                sb.Append(help?.Examples?[i].ToExampleItemString(Constants.mdExampleItemHeaderTemplate, i + 1));
                sb.AppendLine(); // new line for ToExampleItemString
                sb.AppendLine(); // new line after each example
            }
            */
        }

        internal override void WriteParameters(CommandHelp help)
        {
            sb.AppendLine(Constants.mdParametersHeader);
            sb.AppendLine();

            // Sort the parameter by name before writing
            help.Parameters?.Sort((u1, u2) => u1.Name.CompareTo(u2.Name));

            if (help.Parameters?.Count > 0)
            {
                foreach (Parameter param in help.Parameters)
                {
                    ParameterMetadataV2 v2 = param.GetMetadata();

                    sb.AppendLine(string.Format("### -{0}", param.Name));
                    sb.AppendLine();
                    sb.AppendLine(param.Description);
                    sb.AppendLine();
                    sb.AppendLine(v2.ToYamlString());
                }
            }

            if (help.HasCmdletBinding)
            {
                sb.AppendLine(Constants.mdCommonParametersHeader);
                sb.AppendLine();
                sb.AppendLine(ConstantsHelper.GetCommonParametersMessage(ConstantsHelper.CommonParametersVersions.PS7));
                sb.AppendLine();
            }

            if (help.HasWorkflowCommonParameters)
            {
                sb.AppendLine(Constants.mdWorkflowCommonParametersHeader);
                sb.AppendLine();
                sb.AppendLine(Constants.WorkflowCommonParametersMessage);
                sb.AppendLine();
            }

        }

        internal override void WriteInputsOutputs(List<InputOutput> inputsoutputs, bool isInput)
        {
            // Open Question: should we emit something if there is no input/output?
            if (inputsoutputs is null)
            {
                return;
            }

            if (isInput)
            {
                sb.AppendLine(Constants.mdInputsHeader);
            }
            else
            {
                sb.AppendLine(Constants.mdOutputsHeader);
            }

            sb.AppendLine();

            foreach (var item in inputsoutputs)
            {
                sb.AppendLine(item.ToInputOutputString(Constants.mdNotesItemHeaderTemplate));
                sb.AppendLine();
            }
        }

        internal override void WriteNotes(CommandHelp help)
        {
            sb.AppendLine(Constants.mdNotesHeader);
            sb.AppendLine();

            if (! string.IsNullOrEmpty(help.Notes))
            {
                sb.AppendLine(help.Notes);
                sb.AppendLine();
            }
        }

        internal override void WriteRelatedLinks(CommandHelp help)
        {
            sb.AppendLine(Constants.mdRelatedLinksHeader);
            sb.AppendLine();

            if (help.RelatedLinks?.Count > 0)
            {
                foreach (var link in help.RelatedLinks)
                {
                    sb.AppendLine(link.ToRelatedLinksString(Constants.mdRelatedLinksFmt));
                    sb.AppendLine();
                }
            }
            else
            {
                sb.AppendLine(Constants.FillInRelatedLinks);
                sb.AppendLine();
            }
        }

    }
}
