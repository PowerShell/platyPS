// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.PowerShell.PlatyPS;
using Microsoft.PowerShell.PlatyPS.Model;
using YamlDotNet;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Microsoft.PowerShell.PlatyPS
{
    /// <summary>
    /// This class contains utility functions for working with yaml.
    /// </summary>
    internal class YamlUtils
    {
        private static Deserializer deserializer = (Deserializer)new DeserializerBuilder().Build();
        private static Serializer serializer = (Serializer)new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();


        /// <summary>
        /// This does not use the yaml deserializer but attempts to parse the bare text into a dictionary.
        /// </summary>
        /// <param name="text">A string which represents a non-conforming yaml block</param>
        /// <param name="result">dictionary</param>
        /// <returns></returns>
        internal static bool TryConvertYamlToDictionary(string text, out Dictionary<string, string>result)
        {
            Dictionary<string, string> valuePairs = new();
            foreach(string line in text.Split(Constants.LineSplitter))
            {
                var trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine))
                {
                    continue;
                }

                var keyValue = trimmedLine.Split(new char[] {':'}, 2);
                if (keyValue is null || keyValue.Length == 0 || string.IsNullOrEmpty(keyValue[0]))
                {
                    continue;
                }
                else if (keyValue.Length == 1)
                {
                    valuePairs.Add(keyValue[0].Trim(), string.Empty);
                }
                else if (keyValue.Length == 2)
                {
                    valuePairs.Add(keyValue[0].Trim(), keyValue[1].Trim());
                }
            }

            result = valuePairs;
            if (valuePairs.Keys.Count > 0)
            {
                return true;
            }

            return false;
        }

        internal static bool TryFixYaml(string badYaml, out string goodYaml)
        {
            if (TryConvertYamlToDictionary(badYaml, out var dict))
            {
                try
                {
                    var result = serializer.Serialize(dict);
                    goodYaml = result;
                    return true;
                }
                catch
                {
                    ;
                }
            }

            goodYaml = string.Empty;
            return false;
        }

        internal static bool TryLastChance(string badYaml, out ParameterMetadataV2 metadata)
        {
            if (TryFixYaml(badYaml, out var goodYaml))
            {
                if (ParameterMetadataV1.TryConvertToV1(goodYaml, out var v1metadata))
                {
                    if (v1metadata.TryConvertMetadataToV2(out var v2metadata))
                    {
                        metadata = v2metadata;
                        return true;
                    }
                }
            }

            metadata = new ParameterMetadataV2();
            return false;
        }

        internal static string SerializeElement(object o)
        {
            return serializer.Serialize(o);
        }
    }
}
