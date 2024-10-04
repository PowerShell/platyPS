﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.PowerShell.PlatyPS.Model;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace Microsoft.PowerShell.PlatyPS.MarkdownWriter
{
    internal class ModulePageWriter : IDisposable
    {
        private string _modulePagePath;
        private StringBuilder sb;
        private readonly Encoding _encoding;

        public ModulePageWriter(WriterSettings settings)
        {
            if (string.IsNullOrEmpty(settings.DestinationPath))
            {
                throw new ArgumentNullException("destinationpath");
            }

            _modulePagePath = settings.DestinationPath;
            _encoding = settings.Encoding;
            sb = Constants.StringBuilderPool.Get();
        }

        internal FileInfo Write(ModuleFileInfo moduleFileInfo)
        {
            FileInfo modulePage = new FileInfo(_modulePagePath);

            if (!string.Equals(modulePage.Extension, ".md", StringComparison.OrdinalIgnoreCase))
            {
                _modulePagePath = $"{_modulePagePath}{Constants.DirectorySeparator}{moduleFileInfo.Title}.md";
            }

            if (!string.Equals(modulePage.Extension, ".md", StringComparison.OrdinalIgnoreCase))
            {
                _modulePagePath = $"{_modulePagePath}{Constants.DirectorySeparator}{moduleFileInfo.Title}.md";
            }
            else
            {
                if (!modulePage.Exists)
                {
                    DirectoryInfo? currentDir = modulePage.Directory;

                    if (currentDir is not null)
                    {
                        bool currentDirExists = currentDir.Exists;

                        while (!currentDirExists)
                        {
                            try
                            {
                                if (currentDir?.FullName is not null)
                                {
                                    Directory.CreateDirectory(currentDir.FullName);
                                }
                                break;
                            }
                            catch (DirectoryNotFoundException)
                            {
                                currentDir = currentDir?.Parent;
                            }
                        }
                    }
                }
            }

            WriteMetadata(moduleFileInfo);
            sb.AppendLine();
            WriteModuleBlock(moduleFileInfo);
            sb.AppendLine();
            WriteCommandBlock(moduleFileInfo);
            using StreamWriter mdFileWriter = new(_modulePagePath, append: false, _encoding);
            mdFileWriter.Write(sb.ToString());

            return modulePage;
        }

        private void WriteMetadata(ModuleFileInfo moduleFileInfo)
        {
            sb.AppendLine("---");
            sb.Append(YamlUtils.SerializeMetadata(moduleFileInfo.Metadata));
            sb.AppendLine("---");
        }

        internal FileInfo Write(Collection<CommandHelp> helpItems)
        {
            if (helpItems.Count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(helpItems));
            }

            // Help items in one module page have the same module name, locale and module GUID.
            // So we can safely just get these values from the first object.
            string moduleName = helpItems[0].ModuleName;
            string localeString = helpItems[0].Locale.ToString();

            string? helpModuleGuid = helpItems[0].ModuleGuid?.ToString();
            string moduleGuid = helpModuleGuid is null ? Constants.FillInGuid : helpModuleGuid;

            FileInfo modulePage = new FileInfo(_modulePagePath);

            if (!string.Equals(modulePage.Extension, ".md", StringComparison.OrdinalIgnoreCase))
            {
                _modulePagePath = $"{_modulePagePath}{Constants.DirectorySeparator}{moduleName}.md";
            }
            else
            {
                if (!modulePage.Exists)
                {
                    DirectoryInfo? currentDir = modulePage.Directory;

                    if (currentDir is not null)
                    {
                        bool currentDirExists = currentDir.Exists;

                        while (!currentDirExists)
                        {
                            try
                            {
                                if (currentDir?.FullName is not null)
                                {
                                    Directory.CreateDirectory(currentDir.FullName);
                                }
                                break;
                            }
                            catch (DirectoryNotFoundException)
                            {
                                currentDir = currentDir?.Parent;
                            }
                        }
                    }
                }
            }

            WriteHeader(moduleName, localeString, moduleGuid);
            sb.AppendLine();
            WriteModuleBlock(moduleName);
            List<string> commandNames = new();
            foreach (var help in helpItems)
            {
                commandNames.Add(help.Title);
            }

            WriteCmdletBlock(commandNames);
            using StreamWriter mdFileWriter = new(_modulePagePath, append: false, _encoding);
            mdFileWriter.Write(sb.ToString());
            return new FileInfo(_modulePagePath);
        }

        internal void WriteModuleBlock(ModuleFileInfo moduleInfo)
        {
            sb.AppendLine($"# {moduleInfo.Title}");
            sb.AppendLine();
            sb.AppendLine("## Description");
            sb.AppendLine();
            sb.AppendLine($"{moduleInfo.Description}");
        }

        internal void WriteCommandBlock(ModuleFileInfo moduleInfo)
        {
            foreach(var group in moduleInfo.CommandGroups)
            {
                sb.AppendLine($"## {group.GroupTitle}");
                sb.AppendLine();
                foreach (var command in group.Commands)
                {
                    sb.AppendLine($"### [{command.Name}]({command.Link})");
                    sb.AppendLine();
                    if (! string.IsNullOrEmpty(command.Description))
                    {
                        sb.AppendLine($"{command.Description}");
                        sb.AppendLine();
                    }
                }
            }
        }

        internal void WriteHeader(string moduleName, string locale, string moduleGuid)
        {
            sb.AppendLine(Constants.YamlHeader);
            sb.AppendFormat(Constants.ModuleNameHeaderTemplate, moduleName);
            sb.AppendLine();
            sb.AppendFormat(Constants.ModuleGuidHeaderTemplate, moduleGuid);
            sb.AppendLine();
            sb.Append(Constants.DownloadHelpLinkTitle);
            sb.AppendLine(Constants.FillDownloadHelpLink);
            sb.Append(Constants.HelpVersionTitle);
            sb.AppendLine(Constants.FillHelpVersion);
            sb.AppendFormat(Constants.LocaleTemplate, locale);
            sb.AppendLine();
            sb.AppendLine(Constants.YamlHeader);
        }

        internal void WriteModuleBlock(string moduleName)
        {
            sb.AppendFormat(Constants.mdModulePageModuleNameHeaderTemplate, moduleName);
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine(Constants.mdModulePageDescriptionHeader);
            sb.AppendLine();
            sb.AppendLine(Constants.FillInDescription);
            sb.AppendLine();
        }

        internal void WriteCmdletBlock(List<ModuleCommandInfo> commands)
        {
            foreach(var command in commands)
            {
                sb.AppendFormat(Constants.mdModulePageCmdletLinkTemplate, command.Name, command.Link);
                sb.AppendLine();
                if (! string.IsNullOrEmpty(command.Description))
                {
                    sb.AppendLine();
                    sb.AppendLine(command.Description);
                    sb.AppendLine();
                }
            }
            }

        internal void WriteCmdletBlock(List<string> commandNames)
        {
            foreach (var command in commandNames)
            {
                sb.AppendFormat(Constants.mdModulePageCmdletLinkTemplate, command, $"{command}.md");
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine(Constants.FillInDescription);
                sb.AppendLine();
            }
        }

        public void Dispose()
        {
            Constants.StringBuilderPool.Return(sb);
        }
    }
}
