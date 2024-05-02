// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Management.Automation;
using System.Management.Automation.Language;
using Microsoft.PowerShell.PlatyPS.Model;
using System.Collections.Specialized;
using YamlDotNet.Core.Tokens;
using System.Net.Configuration;

namespace Microsoft.PowerShell.PlatyPS
{
    public class MetadataUtils
    {
        /// <summary>
        /// Retrieve the base metadata for a command help file
        /// </summary>
        /// <param name="help">A CommandHelp object to use.</param>
        /// <returns>A dictionary with the base metadata for a command help file.</returns>
        public static OrderedDictionary GetCommandHelpBaseMetadata(CommandHelp help)
        {
            OrderedDictionary metadata = new()
            {
                { "document type", "cmdlet" },
                { "title", help.Title },
                { "Module Name", help.ModuleName },
                { "Locale", help.Locale.Name },
                { "PlatyPS schema version", "2024-05-01" }, // was schema
                { "HelpUri", help.OnlineVersionUrl }, // was online version
                { "ms.date", DateTime.Now.ToString("MM/dd/yyyy") },
                { "external help file", help.ExternalHelpFile }
            };
            return metadata;
        }

        /// <summary>
        /// Retrieve the base metadata for a module help file
        /// </summary>
        /// <param name="help">A ModuleFileInfo object to use.</param>
        /// <returns>A Dictionary with the base metadata for a command help file.</returns>
        public static OrderedDictionary GetModuleFileBaseMetadata(ModuleFileInfo moduleFileInfo)
        {
            OrderedDictionary metadata = new()
            {
                { "document type", "module" },
                { "HelpInfoUri", "xxx" }, // was Download Help Link
                { "Locale", moduleFileInfo.Locale.Name },
                { "PlatyPS schema version", "2024-05-01" },
                { "ms.date", DateTime.Now.ToString("MM/dd/yyyy") },
                { "title", moduleFileInfo.Title },
                { "Module Name", moduleFileInfo.Title },
            };
            return metadata;
        }

        private static string[] requiredCommandHelpMetadataKeys = {
            "document type",
            "title",
            "Module Name",
            "Locale",
            "PlatyPS schema version",
            "HelpUri",
            "ms.date",
            "external help file"
        };

        private static Hashtable keysToMigrate = new Hashtable()
        {
            { "online version", "HelpUri" },
            { "schema", "PlatyPS schema version" },
        };

        /// <summary>
        /// This ensures that we migrate the obsolete keys in metadata to the new versions.
        /// </summary>
        /// <param name="metadata"></param>
        public static OrderedDictionary FixUpCommandHelpMetadata(OrderedDictionary metadata)
        {
            OrderedDictionary od = new();
            foreach (var key in metadata.Keys)
            {
                if (keysToMigrate.ContainsKey(key))
                {
                    // Create the new key and ignore the old key
                    od[keysToMigrate[key]] = metadata[key];
                }
                else
                {
                    od[key] = metadata[key];
                }
            }

            // Remove the older keys that should have been migrated.
            foreach (var key in keysToMigrate.Keys)
            {
                if (od.Contains(key))
                {
                    od.Remove(key);
                }
            }

            // Fix the version for the new schema version
            if (od.Contains(Constants.SchemaVersionKey) && string.Compare(od[Constants.SchemaVersionKey].ToString(), "2.0.0") == 0)
            {
                od[Constants.SchemaVersionKey] = Constants.SchemaVersion;
            }

            return od;
        }
    }
}