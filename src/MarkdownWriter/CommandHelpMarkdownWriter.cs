// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS.MarkdownWriter
{
    /// <summary>
    /// Write the CommandHelp object to a file in markdown format.
    /// </summary>
    internal class CommandHelpMarkdownWriter
    {
        private readonly string _filePath;
        private StringBuilder sb;
        private readonly Encoding _encoding;

        /// <summary>
        /// Initialize the writer with settings.
        /// </summary>
        /// <param name="settings">Settings needs for the markdown writer.</param>
        public CommandHelpMarkdownWriter(MarkdownWriterSettings settings)
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

        /// <summary>
        /// Write the CommandHelp object to the specified path in the the settings during initialization.
        /// The file is overwritten and not appended as we do not expect more than one CommandHelp per file.
        /// </summary>
        /// <param name="help">CommandHelp object to write.</param>
        /// <param name="noMetadata">Specify if the metadata header should be written.</param>
        /// <param name="metadata">Additional metadata that is appended to the standard metadata header. If nometata is true, additional metadata is not written.</param>
        /// <returns>FileInfo object of the created file</returns>
        internal FileInfo Write(CommandHelp help, bool noMetadata, Hashtable? metadata = null)
        {
            if (!noMetadata)
            {
                WriteMetadataHeader(help, metadata);
                sb.AppendLine();
            }

            WriteTitle(help);
            sb.AppendLine();

            WriteSynopsis(help);
            sb.AppendLine();

            // this adds an empty line after all parameters. So no AppendLine needed.
            WriteSyntax(help);

            WriteDescription(help);
            sb.AppendLine();

            WriteExamples(help);
            sb.AppendLine();

            WriteParameters(help);

            if (help.Inputs != null)
            {
                WriteInputsOutputs(help.Inputs, Constants.InputsMdHeader);
            }

            if (help.Outputs != null)
            {
                WriteInputsOutputs(help.Outputs, Constants.OutputsMdHeader);
            }

            WriteNotes(help);

            WriteRelatedLinks(help);

            using StreamWriter mdFileWriter = new(_filePath, append: false, _encoding);

            mdFileWriter.Write(sb.ToString());

            Constants.StringBuilderPool.Return(sb);

            return new FileInfo(_filePath);
        }

        private void WriteMetadataHeader(CommandHelp help, Hashtable? metadata = null)
        {
            sb.AppendLine(Constants.YmlHeader);
            sb.AppendLine($"external help file: {help.ModuleName}-help.xml");
            sb.AppendLine($"Module Name: {help.ModuleName}");
            sb.AppendLine($"online version: {help.OnlineVersionUrl}");
            sb.AppendLine(Constants.SchemaVersionYml);

            if (metadata is not null)
            {
                foreach (DictionaryEntry item in metadata)
                {
                    sb.AppendFormat("{0}: {1}", item.Key, item.Value);
                    sb.AppendLine();
                }
            }

            sb.AppendLine(Constants.YmlHeader);
        }

        private void WriteTitle(CommandHelp help)
        {
            sb.AppendLine($"# {help.Title}");
        }

        private void WriteSynopsis(CommandHelp help)
        {
            sb.AppendLine(Constants.SynopsisMdHeader);
            sb.AppendLine();
            sb.AppendLine(help.Synopsis);
        }

        private void WriteSyntax(CommandHelp help)
        {
            sb.AppendLine(Constants.SyntaxMdHeader);
            sb.AppendLine();

            if (help.Syntax?.Count > 0)
            {
                foreach (SyntaxItem item in help.Syntax)
                {
                    sb.AppendLine(item.ToSyntaxString());
                }
            }
        }

        private void WriteDescription(CommandHelp help)
        {
            sb.AppendLine(Constants.DescriptionMdHeader);
            sb.AppendLine();
            sb.AppendLine(help.Description);
        }

        private void WriteExamples(CommandHelp help)
        {
            sb.AppendLine(Constants.ExamplesMdHeader);
            sb.AppendLine();

            int? totalExamples = help?.Examples?.Count;

            for(int i = 0; i < totalExamples; i++)
            {
                sb.Append(help?.Examples?[i].ToExampleItemString(i + 1));
                sb.AppendLine();
            }
        }

        private void WriteParameters(CommandHelp help)
        {
            sb.AppendLine(Constants.ParametersMdHeader);
            sb.AppendLine();

            // Sort the parameter by name before writing
            help.Parameters?.Sort((u1, u2) => u1.Name.CompareTo(u2.Name));

            if (help.Parameters?.Count > 0)
            {
                foreach (Parameter param in help.Parameters)
                {
                    string paramString = param.ToParameterString();

                    if (!string.IsNullOrEmpty(paramString))
                    {
                        sb.AppendLine(paramString);
                        sb.AppendLine();
                    }
                }

                if (help.HasCmdletBinding)
                {
                    sb.AppendLine(Constants.CommonParameters);
                }
            }
        }

        private void WriteInputsOutputs(List<InputOutput> inputsoutputs, string header)
        {
            sb.AppendLine(header);
            sb.AppendLine();

            if (inputsoutputs is null)
            {
                return;
            }

            foreach (var item in inputsoutputs)
            {
                sb.Append(item.ToInputOutputString());
            }
        }

        private void WriteNotes(CommandHelp help)
        {
            sb.AppendLine(Constants.NotesMdHeader);
            sb.AppendLine();
            sb.AppendLine(help.Notes);
            sb.AppendLine();
        }

        private void WriteRelatedLinks(CommandHelp help)
        {
            sb.AppendLine(Constants.RelatedLinksMdHeader);
            sb.AppendLine();

            if (help.RelatedLinks?.Count > 0)
            {
                foreach(var link in help.RelatedLinks)
                {
                    sb.AppendLine(link.ToRelatedLinksString());
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
