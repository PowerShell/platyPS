// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.PowerShell.PlatyPS;
using Microsoft.PowerShell.PlatyPS.Model;
using Microsoft.PowerShell.PlatyPS.YamlWriter;

namespace Microsoft.PowerShell.PlatyPS.Test
{
    /// <summary>
    /// Write the CommandHelp object to a file in yaml format.
    /// This is used mostly for testing.
    /// </summary>
    public class CommandHelpYamlWriterHelper
    {
        /// <summary>
        /// Convert a markdown file to a yaml file.
        /// </summary>
        /// <param name="markdownPath">The path of the markdown file.</param>
        /// <param name="yamlPath">The path of the output yaml file.</param>
        public static void ConvertMarkdownToYaml(string markdownPath, string yamlPath)
        {
            var writerSettings = new WriterSettings(Encoding.UTF8, yamlPath);
            var yamlWriter = new CommandHelpYamlWriter(writerSettings);
            var commandHelp = (CommandHelp)MarkdownConverter.GetCommandHelpFromMarkdownFile(markdownPath);
            yamlWriter.Write(commandHelp, metadata: null);
        }

        /// <summary>
        /// Convert a CommandHelp object to a yaml file.
        /// </summary>
        /// <param name="commandHelp"></param>
        /// <param name="yamlPath"></param>
        public static void ConvertMarkdownToYaml(object help, string yamlPath)
        {
            if (help is CommandHelp commandHelp)
            {
                var writerSettings = new WriterSettings(Encoding.UTF8, yamlPath);
                var yamlWriter = new CommandHelpYamlWriter(writerSettings);
                yamlWriter.Write(commandHelp, metadata: null);
            }
        }

        /// <summary>
        /// Validate that the file is a valid yaml file.
        /// It does not validate that the file is a valid CommandHelp yaml file.
        /// </summary>
        /// <param name="yamlPath">The path to the yaml file.</param>
        /// <param name="result">The resultant object, in the case of an exception it will return the exception.</param>
        /// <returns>True if the file is valid yaml, false if not.</returns>
        public static bool TestYamlFile(string yamlPath, out object? result)
        {
            var yaml = File.ReadAllText(yamlPath);
            StringReader stringReader = new StringReader(yaml);
            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder().Build();

            result = null;
            bool isValid = false;

            try
            {
                var yamlObject = deserializer.Deserialize(stringReader);
                if (yamlObject is not null)
                {
                    result = yamlObject;
                    isValid = true;
                }
            }
            catch (Exception e)
            {
                result = e;
            }

            return isValid;
        }
    }

}
