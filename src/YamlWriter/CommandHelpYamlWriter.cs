// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.PowerShell.PlatyPS;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS.YamlWriter
{
    /// <summary>
    /// Write the CommandHelp object to a file in yaml format.
    /// </summary>
    internal sealed class CommandHelpYamlWriter : CommandHelpWriterBase
    {
        /// <summary>
        /// Initialize the writer with settings.
        /// </summary>
        /// <param name="settings">Settings needs for the yaml writer.</param>
        internal CommandHelpYamlWriter(CommandHelpWriterSettings settings)
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

        internal override void WriteMetadataHeader(CommandHelp help, Hashtable? metadata = null)
        {
            sb.AppendLine("metadata:"); // Constants.YamlHeader);
            sb.AppendLine($"  external help file: {help.ModuleName}-help.xml");
            sb.AppendLine($"  Module Name: {help.ModuleName}");
            sb.AppendLine($"  online version: {help.OnlineVersionUrl}");
            sb.AppendLine($"  title: {help.Title}");
            sb.AppendLine($"  {Constants.SchemaVersionYaml}");

            if (metadata is not null)
            {
                foreach (DictionaryEntry item in metadata)
                {
                    sb.AppendFormat("  {0}: {1}", item.Key, item.Value);
                }
            }
        }

        internal override void WriteTitle(CommandHelp help)
        {
            sb.AppendLine($"title: {help.Title}");
        }

        internal override void WriteSynopsis(CommandHelp help)
        {
            sb.AppendFormat("{0} '{1}'\n", Constants.SynopsisYamlHeader, help.Synopsis);
        }

        internal override void WriteSyntax(CommandHelp help)
        {
            sb.AppendLine(Constants.SyntaxYamlHeader);

            if (help.Syntax?.Count > 0)
            {
                foreach (SyntaxItem item in help.Syntax)
                {
                    if (item.IsDefaultParameterSet)
                    {
                        var syntaxString = item.ToSyntaxString(Constants.DefaultSyntaxYamlTemplate);
                        if (syntaxString is not null)
                        {
                            foreach(var line in syntaxString.Split(Constants.LineSplitter, StringSplitOptions.RemoveEmptyEntries))
                            {
                                if (line != "```")
                                {
                                    if (line.StartsWith("-"))
                                    {
                                        sb.AppendFormat("{0}\n", line);
                                    }
                                    else
                                    {
                                        sb.AppendFormat("  {0}\n", line);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var syntaxString = item.ToSyntaxString(Constants.SyntaxYamlTemplate);
                        if (syntaxString is not null)
                        {
                            foreach(var line in syntaxString.Split(Constants.LineSplitter, StringSplitOptions.RemoveEmptyEntries))
                            {
                                if (line != "```")
                                {
                                    if (line.StartsWith("-"))
                                    {
                                        sb.AppendFormat("{0}\n", line);
                                    }
                                    else
                                    {
                                        sb.AppendFormat("  {0}\n", line);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        internal override void WriteDescription(CommandHelp help)
        {
            sb.AppendLine(Constants.DescriptionYamlHeader);
            if (help.Description is not null)
            {
                foreach(var line in help.Description.Split(Constants.LineSplitter))
                {
                    sb.AppendFormat("  {0}\n", line);
                }
            }
        }

        internal override void WriteAliases(CommandHelp help)
        {
            sb.AppendLine(Constants.yamlAliasHeader);
            sb.AppendLine("  " + Constants.AliasMessage);
        }

        internal override void WriteExamples(CommandHelp help)
        {
            sb.AppendLine(Constants.ExamplesYamlHeader);
            int? totalExamples = help?.Examples?.Count;
            for (int i = 0; i < totalExamples; i++)
            {
                var example = help?.Examples?[i];
                if (example is null)
                {
                    continue;
                }

                sb.AppendFormat("- title: 'Example {0}: {1}'\n", i+1, example.Title);
                sb.AppendLine("  description: |-");
                if (example.Remarks is not null)
                {
                    foreach(var line in example.Remarks.Split(Constants.LineSplitter, StringSplitOptions.RemoveEmptyEntries))
                    {
                        sb.AppendFormat("    {0}\n", line?.Trim());
                    }
                }
                sb.AppendLine("  summary: \"\"");
            }
        }

        internal override void WriteParameters(CommandHelp help)
        {
            sb.AppendLine(Constants.ParametersYamlHeader);

            // Sort the parameter by name before writing
            help.Parameters?.Sort((u1, u2) => u1.Name.CompareTo(u2.Name));

            if (help.Parameters?.Count > 0)
            {
                foreach (Parameter param in help.Parameters)
                {
                    /*
                    The metadata that we write out for each parameter is:
                    name
                    type
                    description
                    defaultValue
                    pipelineInput
                    position (as string)
                    aliases
                    parameterValueGroup
                    */
                    sb.AppendFormat("- name: {0}\n", param.Name);
                    sb.AppendFormat("  type: {0}\n", param.Type);
                    sb.AppendLine("  description: |+");
                    if (param.Description is not null)
                    {
                        foreach(var line in param.Description.Split(Constants.LineSplitter))
                        {
                            sb.AppendFormat("    {0}\n", line);
                        }
                    }
                    sb.AppendFormat("  defaultValue: {0}\n", param.DefaultValue ?? "None");
                    sb.AppendFormat("  pipelineInput: {0}\n", param.PipelineInput);
                    sb.AppendFormat("  position: \"{0}\"\n", param.Position);
                    sb.AppendFormat("  aliases: {0}\n", param.Aliases);
                    sb.AppendFormat("  parameterValueGroup: \"{0}\"\n", string.Empty); // ????
                }

                if (help.HasCmdletBinding)
                {
                    sb.AppendLine(Constants.CommonParametersYamlHeader);
                    sb.AppendFormat("  {0}\n", ConstantsHelper.GetCommonParametersMessage());
                }
            }
        }

        internal override void WriteInputsOutputs(List<InputOutput> inputsoutputs, bool isInput)
        {
            if (inputsoutputs is null)
            {
                return;
            }

            if (isInput)
            {
                sb.AppendLine(Constants.InputsYamlHeader);
            }
            else
            {
                sb.AppendLine(Constants.OutputsYamlHeader); 
            }

            foreach (var inputoutput in inputsoutputs)
            {
                foreach(var item in inputoutput._inputOutputItems)
                {
                    sb.AppendFormat("- name: {0}\n", item.Item1);
                    sb.AppendLine("  description: |-");
                    foreach(var line in item.Item2.Split(Constants.LineSplitter, StringSplitOptions.RemoveEmptyEntries))
                    {
                        sb.AppendFormat("    {0}\n", line);
                    }
                }
            }
        }

        internal override void WriteNotes(CommandHelp help)
        {
            sb.AppendLine(Constants.NotesYamlHeader);
            if (help.Notes is not null)
            {
                foreach(var line in help.Notes.Split(Constants.LineSplitter))
                {
                    sb.AppendFormat("  {0}\n", line);
                }
            }
        }

        internal override void WriteRelatedLinks(CommandHelp help)
        {
            sb.AppendLine(Constants.RelatedLinksYamlHeader);
            if (help.RelatedLinks?.Count > 0)
            {
                foreach (var link in help.RelatedLinks)
                {
                    sb.AppendFormat("- text: '{0}'\n", link.LinkText);
                    sb.AppendFormat("  href: {0}\n", link.Uri);
                }
            }
            else
            {
                sb.AppendLine(Constants.FillInRelatedLinks);
            }
        }
    }
}
