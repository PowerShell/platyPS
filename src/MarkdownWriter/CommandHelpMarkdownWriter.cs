using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS.MarkdownWriter
{
    internal class CommandHelpMarkdownWriter
    {
        private readonly string _filePath;
        private StringBuilder sb = null;
        private readonly Encoding _encoding;

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
            }
        }

        internal FileInfo Write(CommandHelp help, bool includeMetadata)
        {
            sb ??= new StringBuilder();

            if (includeMetadata)
            {
                WriteMetadataHeader(help);
                sb.AppendLine();
            }

            WriteTitle(help);
            sb.AppendLine();

            WriteSynopsis(help);
            sb.AppendLine();

            // this adds an empty line after all parameters
            WriteSyntax(help);

            WriteDescription(help);
            sb.AppendLine();

            WriteExamples(help);
            sb.AppendLine();

            WriteParameters(help);

            WriteInputsOutputs(help.Inputs, Constants.InputsMdHeader);

            WriteInputsOutputs(help.Outputs, Constants.OutputsMdHeader);

            WriteNotes(help);

            WriteRelatedLinks(help);

            using (StreamWriter mdFileWriter = new(_filePath, append: false, _encoding))
            {
                mdFileWriter.Write(sb.ToString());

                return new FileInfo(_filePath);
            }
        }

        private void WriteMetadataHeader(CommandHelp help)
        {
            sb.AppendLine(Constants.YmlHeader);
            sb.AppendLine($"external help file: {help.ModuleName}-help.xml");
            sb.AppendLine($"Module Name: {help.ModuleName}");
            sb.AppendLine($"online version: {help.OnlineVersionUrl}");
            sb.AppendLine(Constants.SchemaVersionYml);
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

            foreach(SyntaxItem item in help.Syntax)
            {
                sb.AppendLine(item.ToSyntaxString());
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

            int totalExamples = help.Examples.Count;

            for(int i = 0; i < totalExamples; i++)
            {
                sb.Append(help.Examples[i].ToExampleItemString(i + 1));
                sb.AppendLine();
            }
        }

        private void WriteParameters(CommandHelp help)
        {
            sb.AppendLine(Constants.ParametersMdHeader);
            sb.AppendLine();

            // Sort the parameter by name before writing
            help.Parameters.Sort((u1, u2) => u1.Name.CompareTo(u2.Name));

            foreach(var param in help.Parameters)
            {
                string paramString = param.ToParameterString();

                if (!string.IsNullOrEmpty(paramString))
                {
                    sb.AppendLine(paramString);
                    sb.AppendLine();
                }
            }

            sb.AppendLine(Constants.CommonParameters);
        }

        private void WriteInputsOutputs(List<InputOutput> inputsoutputs, string header)
        {
            sb.AppendLine(header);
            sb.AppendLine();

            if (inputsoutputs == null)
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
                sb.AppendLine("{{ Fill Related Links Here}}");
                sb.AppendLine();
            }
        }
    }
}
