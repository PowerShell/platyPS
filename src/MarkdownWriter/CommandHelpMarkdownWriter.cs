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
        private string filePath;
        private StringBuilder sb = null;

        private CommandHelp Help { get; set; }

        public CommandHelpMarkdownWriter(string path, CommandHelp commandHelp)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            else
            {
                filePath = path;
                Help = commandHelp;
            }
        }

        internal FileInfo Write()
        {
            sb ??= new StringBuilder();

            WriteMetadataHeader();
            sb.AppendLine();

            WriteTitle();
            sb.AppendLine();

            WriteSynopsis();
            sb.AppendLine();

            // this adds an empty line after all parameters
            WriteSyntax();

            WriteDescription();
            sb.AppendLine();

            WriteExamples();
            sb.AppendLine();

            WriteParameters();

            WriteInputsOutputs(Help.Inputs, Constants.InputsMdHeader);

            WriteInputsOutputs(Help.Outputs, Constants.OutputsMdHeader);

            WriteNotes();

            WriteRelatedLinks();

            using (StreamWriter mdFileWriter = new(filePath))
            {
                mdFileWriter.Write(sb.ToString());

                return new FileInfo(filePath);
            }
        }

        private void WriteMetadataHeader()
        {
            sb.AppendLine(Constants.YmlHeader);
            sb.AppendLine($"external help file: {Help.ModuleName}-help.xml");
            sb.AppendLine($"Module Name: {Help.ModuleName}");
            sb.AppendLine("online version:");
            sb.AppendLine(Constants.SchemaVersionYml);
            sb.AppendLine(Constants.YmlHeader);
        }

        private void WriteTitle()
        {
            sb.AppendLine($"# {Help.Title}");
        }

        private void WriteSynopsis()
        {
            sb.AppendLine(Constants.SynopsisMdHeader);
            sb.AppendLine();
            sb.AppendLine(Help.Synopsis);
        }

        private void WriteSyntax()
        {
            sb.AppendLine(Constants.SyntaxMdHeader);
            sb.AppendLine();

            foreach(SyntaxItem item in Help.Syntax)
            {
                sb.AppendLine(item.ToSyntaxString());
            }
        }

        private void WriteDescription()
        {
            sb.AppendLine(Constants.DescriptionMdHeader);
            sb.AppendLine();
            sb.AppendLine(Help.Description);
        }

        private void WriteExamples()
        {
            sb.AppendLine(Constants.ExamplesMdHeader);
            sb.AppendLine();

            int totalExamples = Help.Examples.Count;

            for(int i = 0; i < totalExamples; i++)
            {
                sb.Append(Help.Examples[i].ToExampleItemString(i + 1));
                sb.AppendLine();
            }
        }

        private void WriteParameters()
        {
            sb.AppendLine(Constants.ParametersMdHeader);
            sb.AppendLine();

            // Sort the parameter by name before writing
            Help.Parameters.Sort((u1, u2) => u1.Name.CompareTo(u2.Name));

            foreach(var param in Help.Parameters)
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

        private void WriteNotes()
        {
            sb.AppendLine(Constants.NotesMdHeader);
            sb.AppendLine();
            sb.AppendLine(Help.Notes);
            sb.AppendLine();
        }

        private void WriteRelatedLinks()
        {
            sb.AppendLine(Constants.RelatedLinksMdHeader);
            sb.AppendLine();

            if (Help.RelatedLinks?.Count > 0)
            {
                foreach(var link in Help.RelatedLinks)
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
