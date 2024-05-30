// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Management.Automation.Language;
using Microsoft.PowerShell.PlatyPS.Model;
using System.Collections.Specialized;
using YamlDotNet.Core.Tokens;
using System.Net.Configuration;
using Microsoft.PowerShell.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

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
        /// Retrieve a metadata object from a commandInfo object
        /// </summary>
        /// <param name="commandInfo"></param>
        /// <returns></returns>
        public static OrderedDictionary GetCommandHelpBaseMetadataFromCommandInfo(CommandInfo commandInfo)
        {
            OrderedDictionary metadata = new()
            {
                { "document type", "cmdlet" },
                { "title", commandInfo.Name },
                { "Module Name", commandInfo.ModuleName },
                { "Locale", "{{ fill in locale }}" },
                { "PlatyPS schema version", "2024-05-01" }, // was schema
                { "HelpUri", GetHelpCodeMethods.GetHelpUri(new PSObject(commandInfo)) }, // was online version
                { "ms.date", DateTime.Now.ToString("MM/dd/yyyy") },
                { "external help file", GetHelpFileFromCommandInfo(commandInfo) }
            };
            return metadata;
        }

        private static string GetHelpFileFromCommandInfo(CommandInfo commandInfo)
        {
            if (commandInfo is CmdletInfo cmdlet)
            {
                return cmdlet.HelpFile;
            }

            if (commandInfo is FunctionInfo function)
            {
                return function.HelpFile;
            }

            return string.Empty;
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
                { "Module Guid", Guid.Empty },
            };
            return metadata;
        }

        public static OrderedDictionary GetModuleFileBaseMetadata(PSModuleInfo moduleInfo, CultureInfo? locale)
        {
            OrderedDictionary metadata = new()
            {
                { "document type", "module" },
                { "HelpInfoUri", moduleInfo.HelpInfoUri }, // was Download Help Link
                { "Locale", locale?.Name ?? "en-us" },
                { "PlatyPS schema version", "2024-05-01" },
                { "ms.date", DateTime.Now.ToString("MM/dd/yyyy") },
                { "title", $"{moduleInfo.Name} Module" },
                { "Module Name", moduleInfo.Name },
                { "Module Guid", moduleInfo.Guid },
            };

            return metadata;
        }

        /// <summary>
        /// Retrieve the base metadata for a module help file
        /// </summary>
        /// <param name="help">A ModuleFileInfo object to use.</param>
        /// <returns>A Dictionary with the base metadata for a command help file.</returns>
        public static OrderedDictionary GetModuleFileBaseMetadata(string title, string name, CultureInfo? locale)
        {
            OrderedDictionary metadata = new()
            {
                { "document type", "module" },
                { "HelpInfoUri", string.Empty }, // was Download Help Link
                { "Locale", locale?.Name ?? "en-US" },
                { "PlatyPS schema version", "2024-05-01" },
                { "ms.date", DateTime.Now.ToString("MM/dd/yyyy") },
                { "title", title },
                { "Module Name", name },
                { "Module Guid", Guid.Empty },
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

            // Be sure that document type is correctly present.
            if (! od.Contains("document type"))
            {
                if (od.Contains("Module Guid"))
                {
                    od["document type"] = "module";
                }
                {
                    od["document type"] = "cmdlet";
                }
            }

            return od;
        }

        /// <summary>
        /// This ensures that we migrate the obsolete keys in metadata to the new versions.
        /// </summary>
        /// <param name="metadata"></param>
        public static OrderedDictionary FixUpModuleFileMetadata(OrderedDictionary metadata)
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

            // Be sure that document type is correctly present.
            if (! od.Contains("document type"))
            {
                od["document type"] = "module";
            }

            return od;
        }

        internal static bool TryGetGuidFromMetadata(OrderedDictionary metadata, string term, out Guid guid)
        {
            if (metadata.Contains(term))
            {
                if (metadata[term] is not null && Guid.TryParse(metadata[term].ToString(), out var result))
                {
                    guid = result;
                    return true;
                }
            }

            guid = Guid.Empty;
            return false;
        }

        internal static bool TryGetStringFromMetadata(OrderedDictionary metadata, string term, out string str)
        {
            if (metadata.Contains(term))
            {
                if (metadata[term] is not null)
                {
                    str = metadata[term].ToString();
                    return true;
                }
            }

            str = string.Empty;
            return false;
        }

        internal static string[] ProtectedMetadataKeys = new string[] {
                "PlatyPS schema version"
            };

        internal static List<string> WarnBadKeys(PSCmdlet cmdlet, Hashtable metadata)
        {
            List<string>badKeys = new();
            foreach(DictionaryEntry kv in metadata)
            {
                string key = kv.Key.ToString();
                if (MetadataUtils.ProtectedMetadataKeys.Any(k => string.Compare(key, k, true) == 0))
                {
                    cmdlet.WriteWarning($"Metadata key '{key}' may not be overridden");
                    badKeys.Add(key);
                }
            }
            return badKeys;
        }

        internal static OrderedDictionary MergeCommandHelpMetadataWithNewMetadata(Hashtable? metadata, CommandHelp commandHelp)
        {
            OrderedDictionary newMetadata = new();
            if (commandHelp.Metadata is not null)
            {
                foreach(var key in commandHelp.Metadata.Keys)
                {
                    newMetadata[key] = commandHelp.Metadata[key];
                }
            }

            if (metadata is not null)
            {
                foreach(string key in metadata.Keys)
                {
                    newMetadata[key] = metadata[key];
                }
            }

            return newMetadata;
        }

        internal static void MergeNewCommandHelpMetadata(Hashtable newMetadata, CommandHelp commandHelp)
        {
            if (newMetadata is null || newMetadata.Keys.Count == 0)
            {
                return;
            }

            if (commandHelp.Metadata is null)
            {
                commandHelp.Metadata = new();
            }

            // This will overwrite values in the module file
            foreach (var key in newMetadata.Keys)
            {
                commandHelp.Metadata[key] = newMetadata[key];
            }

        }

        internal static void MergeNewModulefileMetadata(Hashtable newMetadata, ModuleFileInfo moduleFile)
        {
            if (newMetadata.Keys.Count == 0)
            {
                return;
            }

            // This will overwrite values in the module file
            foreach (var key in newMetadata.Keys)
            {
                moduleFile.Metadata[key] = newMetadata[key];
            }
        }
    }


}