// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    /// <summary>
    /// The abstract class for writing the CommandHelp object to a file in markdown format.
    /// It can be used by any command help writer that writes to markdown format.
    /// It ensures that all writers will emit all the sections in the proper order.
    /// </summary>
    internal abstract class CommandHelpWriterBase : IDisposable
    {
        internal string _filePath;
        internal StringBuilder sb;
        internal Encoding _encoding;

        internal CommandHelpWriterBase()
        {
            _encoding = Encoding.UTF8;
            _filePath = string.Empty;
            sb = Constants.StringBuilderPool.Get();
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
            }

            WriteTitle(help);

            WriteSynopsis(help);

            WriteSyntax(help);

            WriteAliases(help);

            WriteDescription(help);

            WriteExamples(help);

            WriteParameters(help);

            if (help.Inputs != null)
            {
                WriteInputsOutputs(help.Inputs, isInput: true);
            }

            if (help.Outputs != null)
            {
                WriteInputsOutputs(help.Outputs, isInput: false);
            }

            WriteNotes(help);

            WriteRelatedLinks(help);

            using (StreamWriter fileWriter = new(_filePath, append: false, _encoding))
            {
                fileWriter.Write(sb.ToString());
                return new FileInfo(_filePath);
            }
        }

        public void Dispose()
        {
            Constants.StringBuilderPool.Return(sb);
        }

        internal abstract void WriteMetadataHeader(CommandHelp help, Hashtable? metadata = null);

        internal abstract void WriteTitle(CommandHelp help);

        internal abstract void WriteSynopsis(CommandHelp help);

        internal abstract void WriteSyntax(CommandHelp help);

        internal abstract void WriteAliases(CommandHelp help);

        internal abstract void WriteDescription(CommandHelp help);

        internal abstract void WriteExamples(CommandHelp help);

        internal abstract void WriteParameters(CommandHelp help);

        internal abstract void WriteInputsOutputs(List<InputOutput> inputsoutputs, bool isInput);

        internal abstract void WriteNotes(CommandHelp help);

        internal abstract void WriteRelatedLinks(CommandHelp help);

    }
}
