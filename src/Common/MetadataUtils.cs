// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Automation;
using Microsoft.PowerShell.PlatyPS.Model;
using Microsoft.PowerShell.Commands;

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
                { "Locale", string.IsNullOrEmpty(help.Locale.Name) ? "en-US" : help.Locale.Name },
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
                { "Locale", CultureInfo.CurrentCulture.Name == string.Empty ? "en-US" : CultureInfo.CurrentCulture.Name },
                { "PlatyPS schema version", "2024-05-01" }, // was schema
                { "HelpUri", GetHelpCodeMethods.GetHelpUri(new PSObject(commandInfo)) }, // was online version
                { "ms.date", DateTime.Now.ToString("MM/dd/yyyy") },
                { "external help file", GetHelpFileFromCommandInfo(commandInfo) }
            };
            return metadata;
        }

        private static string GetHelpFileFromCommandInfo(CommandInfo commandInfo)
        {
            // We are chosing upper case for the "Help.xml" file name to avoid issues with non-Windows platforms.
            string helpFileName;
            if (commandInfo is CmdletInfo cmdlet && ! string.IsNullOrEmpty(cmdlet.HelpFile))
            {
                helpFileName = cmdlet.HelpFile;
            }
            else if (commandInfo is FunctionInfo function && ! string.IsNullOrEmpty(function.HelpFile))
            {
                helpFileName = function.HelpFile;
            }
            else if (! string.IsNullOrEmpty(commandInfo.ModuleName))
            {
                helpFileName = $"{commandInfo.ModuleName}-Help.xml";
            }
            else
            {
                helpFileName = $"{commandInfo.Name}-Help.xml";
            }

            // return only the filename, not the full path.
            return Path.GetFileName(helpFileName);
        }

        /// <summary>
        /// Retrieve the base metadata for a module help file
        /// </summary>
        /// <param name="help">A ModuleFileInfo object to use.</param>
        /// <returns>A Dictionary with the base metadata for a command help file.</returns>
        public static SortedDictionary<string, string> GetModuleFileBaseMetadata(ModuleFileInfo moduleFileInfo)
        {
            SortedDictionary<string, string> metadata = new()
            {
                { "document type", "module" },
                { "HelpInfoUri", "xxx" }, // was Download Help Link
                { "Locale", string.IsNullOrEmpty(moduleFileInfo.Locale.Name) ? "en-US" : moduleFileInfo.Locale.Name },
                { "PlatyPS schema version", "2024-05-01" },
                { "ms.date", DateTime.Now.ToString("MM/dd/yyyy") },
                { "title", moduleFileInfo.Title },
                { "Module Name", moduleFileInfo.Title },
                { "Module Guid", Guid.Empty.ToString() },
            };
            return metadata;
        }

        public static OrderedDictionary GetModuleFileBaseMetadata(PSModuleInfo moduleInfo, CultureInfo? locale)
        {
            OrderedDictionary metadata = new()
            {
                { "document type", "module" },
                { "HelpInfoUri", moduleInfo.HelpInfoUri }, // was Download Help Link
                { "Locale", locale?.Name ?? "en-US" },
                { "PlatyPS schema version", "2024-05-01" },
                { "ms.date", DateTime.Now.ToString("MM/dd/yyyy") },
                { "title", $"{moduleInfo.Name} Module" },
                { "Module Name", moduleInfo.Name },
                { "Module Guid", moduleInfo.Guid.ToString() },
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
                { "Module Guid", Guid.Empty.ToString() },
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
            { "Download Help Link", "HelpInfoUri" },
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
                    od[keysToMigrate[key].ToString()] = metadata[key] is null ? string.Empty : metadata[key];
                }
                else
                {
                    od[key.ToString()] = metadata[key] is null ? string.Empty : metadata[key];
                }
            }

            // Remove the older keys that should have been migrated.
            foreach (var key in keysToMigrate.Keys)
            {
                if (od.Contains(key.ToString()))
                {
                    od.Remove(key.ToString());
                }
            }

            // Fix the version for the new schema version
            if (od.Contains(Constants.SchemaVersionKey) && string.Compare(od[Constants.SchemaVersionKey].ToString(), "2.0.0") == 0)
            {
                od[Constants.SchemaVersionKey] = Constants.SchemaVersion;
            }
            else if (!od.Contains(Constants.SchemaVersionKey))
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
                "PlatyPS schema version",
                "document type"
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
                    newMetadata[key.ToString()] = commandHelp.Metadata[key];
                }
            }

            if (metadata is not null)
            {
                foreach(string key in metadata.Keys)
                {
                    if (! ProtectedMetadataKeys.Contains(key))
                    {
                        newMetadata[key] = metadata[key];
                    }
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
                moduleFile.Metadata[key.ToString()] = newMetadata[key].ToString();
            }
        }

        internal static OrderedDictionary DeserializeMetadataText(string metadataBlock)
        {
            if (YamlUtils.TryGetOrderedDictionaryFromText(metadataBlock, out OrderedDictionary md))
            {
                return md;
            }

            throw new InvalidOperationException("Cannot convert to dictionary");
        }
    }

}