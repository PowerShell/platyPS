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

        internal static StringSplitOptions stringSplitOptions = StringSplitOptions.None;

        internal override void WriteMetadataHeader(CommandHelp help, Hashtable? metadata = null)
        {
            sb.AppendLine("metadata:");

            if (help?.Metadata is null && metadata is null)
            {
                sb.AppendLine($"  external help file: {help?.ModuleName}-help.xml");
                sb.AppendLine($"  Module Name: {help?.ModuleName}");
                sb.AppendLine($"  online version: {help?.OnlineVersionUrl}");
                sb.AppendLine($"  title: {help?.Title}");
                sb.AppendLine(Constants.SchemaVersionYaml);
                return;
            }

            // Emit the metadata from the help object unless it is in the metadata hashtable.
            if (help?.Metadata is not null)
            {
                foreach (DictionaryEntry item in help.Metadata)
                {
                    if (metadata is null)
                    {
                        sb.AppendLine($"  {item.Key}: {item.Value}");
                    }
                    else if (! metadata.ContainsKey(item.Key)) // metadata provided overrides the help object
                    {
                        sb.AppendLine($"  {item.Key}: {item.Value}");
                    }
                }
            }

            // Emit the metadata from the metadata hashtable if we have any.
            if (metadata is not null)
            {
                foreach (DictionaryEntry item in metadata)
                {
                    sb.AppendLine($"  {item.Key}: {item.Value}");
                }
            }
        }

        internal override void WriteTitle(CommandHelp help)
        {
            sb.AppendLine($"title: {help.Title}");
        }

        internal override void WriteSynopsis(CommandHelp help)
        {
            sb.AppendLine(string.Format("{0} '{1}'", Constants.SynopsisYamlHeader, help.Synopsis));
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
                            foreach(var line in syntaxString.Trim().Split(Constants.LineSplitter, stringSplitOptions))
                            {
                                if (line != "```")
                                {
                                    if (line.StartsWith("-"))
                                    {
                                        sb.AppendLine(line);
                                    }
                                    else
                                    {
                                        sb.AppendLine(string.Format("  {0}", line));
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
                            foreach(var line in syntaxString.Trim().Split(Constants.LineSplitter, stringSplitOptions))
                            {
                                if (line != "```")
                                {
                                    if (line.StartsWith("-"))
                                    {
                                        sb.AppendLine(line);
                                    }
                                    else
                                    {
                                        sb.AppendLine(string.Format("  {0}", line));
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
            // check to be sure that we have a Description and if so, add "|-"
            if (help.Description is not null && help.Description.Length > 0)
            {
                sb.AppendLine($"{Constants.DescriptionYamlHeader} |-");
            }
            else
            {
                sb.AppendLine(Constants.DescriptionYamlHeader);
            }

            if (help.Description is not null)
            {
                foreach(var line in help.Description.Split(Constants.LineSplitter))
                {
                    sb.AppendLine(string.Format("  {0}", line));
                }
            }
        }

        internal override void WriteAliases(CommandHelp help)
        {
            if (help.Aliases?.Count > 0)
            {
                string aliasString = string.Join(", ", help.Aliases);
                sb.AppendLine($"{Constants.yamlAliasHeader} {aliasString}");
            }
            else
            {
                sb.AppendLine(Constants.yamlAliasHeader);
            }
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

                sb.AppendLine(string.Format("- title: \"Example {0}: {1}\"", i+1, example.Title));
                sb.AppendLine("  description: |-");
                if (example.Remarks is not null)
                {
                    foreach(var line in example.Remarks.Trim().Split(Constants.LineSplitter, stringSplitOptions))
                    {
                        sb.AppendLine(string.Format("    {0}", line?.Trim()));
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
                sb.AppendLine(YamlUtils.SerializeElement(help.Parameters));

                if (help.HasCmdletBinding)
                {
                    sb.AppendLine(Constants.CommonParametersYamlHeader);
                    sb.AppendLine("  description: |-");
                    var commonParameters = ConstantsHelper.GetCommonParametersMessage();
                    foreach(var line in commonParameters.Split(Constants.LineSplitter, stringSplitOptions))
                    {
                        sb.AppendLine($"    {line}");
                    }
                }
            }
        }

        internal override void WriteInputsOutputs(List<InputOutput> inputsoutputs, bool isInput)
        {
            if (inputsoutputs is null)
            {
                return;
            }

            if (inputsoutputs.Count == 0)
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
                sb.AppendLine($"- name: {inputoutput.Typename}");
                if (inputoutput.Description.Length > 0)
                {
                    sb.AppendLine("  description: |-");
                    foreach(var line in inputoutput.Description.Trim().Split(Constants.LineSplitter, stringSplitOptions))
                    {
                        sb.AppendLine($"    {line}");
                    }
                }
                else
                {
                    sb.AppendLine("  description:");
                }
            }
        }

        internal override void WriteNotes(CommandHelp help)
        {
            if (help.Notes is null)
            {
                sb.AppendLine(Constants.NotesYamlHeader);
                return;
            }

            if (string.IsNullOrEmpty(help.Notes.Trim()))
            {
                sb.AppendLine(Constants.NotesYamlHeader);
                return;
            }

            sb.AppendLine($"{Constants.NotesYamlHeader} |-");
            foreach(var line in help.Notes.Split(Constants.LineSplitter))
            {
                sb.AppendLine(string.Format("  {0}", line));
            }
        }

        internal override void WriteRelatedLinks(CommandHelp help)
        {
            sb.AppendLine(Constants.RelatedLinksYamlHeader);
            if (help.RelatedLinks?.Count > 0)
            {
                foreach (var link in help.RelatedLinks)
                {
                    sb.AppendLine(string.Format("- text: '{0}'", link.LinkText));
                    sb.AppendLine(string.Format("  href: {0}", link.Uri));
                }
            }
        }
    }
}
