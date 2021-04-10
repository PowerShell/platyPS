using Microsoft.PowerShell.PlatyPS.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace Microsoft.PowerShell.PlatyPS.MarkdownWriter
{
    internal class ModulePageWriter
    {
        private string _modulePagePath;
        private StringBuilder sb = null;
        private readonly Encoding _encoding;

        public ModulePageWriter(string modulePagePath, Encoding encoding)
        {
            _modulePagePath = modulePagePath;
            _encoding = encoding;
        }

        internal FileInfo Write(Collection<CommandHelp> helpItems)
        {
            if (helpItems.Count < 1 )
            {
                throw new ArgumentException("Not enough command help items");
            }

            string moduleName = helpItems[0].ModuleName;
            string localeString = helpItems[0].Locale == null ? Constants.LocaleEnUs : helpItems[0].Locale.ToString();
            string moduleGuid = helpItems[0].ModuleGuid == null ? Constants.FillInGuid : helpItems[0].ModuleGuid.ToString();

            var modulePage = new FileInfo(_modulePagePath);

            if (!string.Equals(modulePage.Extension, ".xml", StringComparison.OrdinalIgnoreCase))
            {
                _modulePagePath = $"{_modulePagePath}\\{moduleName}.md";
            }

            sb ??= new StringBuilder();

            using StreamWriter mdFileWriter = new(_modulePagePath, append: false, _encoding);

            WriteHeader(moduleName, localeString, moduleGuid);
            sb.AppendLine();
            WriteModuleBlock(moduleName);

            List<string> commandNames = new();

            foreach(var help in helpItems)
            {
                commandNames.Add(help.Title);
            }

            WriteCmdletBlock(commandNames);

            mdFileWriter.Write(sb.ToString());

            return new FileInfo(_modulePagePath);
        }

        internal void WriteHeader(string moduleName, string locale, string moduleGuid)
        {
            sb.AppendLine(Constants.YmlHeader);
            sb.AppendFormat(Constants.ModuleNameHeaderTemplate, moduleName);
            sb.AppendLine();
            sb.AppendFormat(Constants.ModuleGuidHeaderTemplate, moduleGuid);
            sb.AppendLine();
            sb.Append(Constants.DownladHelpLinkTitle);
            sb.AppendLine(Constants.FillDownloadHelpLink);
            sb.Append(Constants.HelpVersionTitle);
            sb.AppendLine(Constants.FillHelpVersion);
            sb.AppendFormat(Constants.LocaleTemplate, locale);
            sb.AppendLine();
            sb.AppendLine(Constants.YmlHeader);
        }

        internal void WriteModuleBlock(string moduleName)
        {
            sb.AppendFormat(Constants.ModulePageModuleNameHeaderTemplate, moduleName);
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine(Constants.ModulePageDescriptionHeader);
            sb.AppendLine();
            sb.AppendLine(Constants.FillInDescription);
            sb.AppendLine();
        }

        internal void WriteCmdletBlock(List<string> commandNames)
        {
            foreach(var command in commandNames)
            {
                sb.AppendFormat(Constants.ModulePageCmdletLinkTemplate, command, $"{command}.md");
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine(Constants.FillInDescription);
                sb.AppendLine();
            }
        }

    }
}
