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

namespace Microsoft.PowerShell.PlatyPS
{
    public class MetadataUtils
    {

        /// <summary>
        /// Retrieve the base metadata for a command help file
        /// </summary>
        /// <param name="help">A CommandHelp object to use.</param>
        /// <returns>A hashtable with the base metadata for a command help file.</returns>
        public static OrderedDictionary GetCommandHelpBaseMetadata(CommandHelp help)
        {
            OrderedDictionary metadata = new();
            metadata.Add("document type", "cmdlet");
            metadata.Add("title", help.Title);
            metadata.Add("Module Name", help.ModuleName);
            metadata.Add("Locale", help.Locale.Name);
            metadata.Add("PlatyPS schema version", "2024-05-01");
            metadata.Add("HelpUri", help.OnlineVersionUrl); // was online version
            metadata.Add("ms.date", DateTime.Now.ToString("MM/dd/yyyy"));
            metadata.Add("external help file", help.ExternalHelpFile);
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
                    od.Add(keysToMigrate[key], metadata[key]);
                }
                else
                {
                    od.Add(key, metadata[key]);
                }
            }

            return od;
        }
    }
}