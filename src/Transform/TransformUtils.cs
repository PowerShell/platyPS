// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using System.Management.Automation.Language;
using System.Text;
using Markdig.Helpers;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    public class TransformUtils
    {
        private static Dictionary<string, string> TypeAbbreviations = new Dictionary<string, string> {
            { "System.Management.Automation.AliasAttribute" , "Alias" },
            { "System.Management.Automation.AllowEmptyCollectionAttribute" , "AllowEmptyCollection" },
            { "System.Management.Automation.AllowEmptyStringAttribute" , "AllowEmptyString" },
            { "System.Management.Automation.AllowNullAttribute" , "AllowNull" },
            { "System.Management.Automation.ArgumentCompleterAttribute" , "ArgumentCompleter" },
            { "System.Management.Automation.ArgumentCompletionsAttribute" , "ArgumentCompletions" },
            { "System.Array" , "array" },
            { "System.Boolean" , "bool" },
            { "System.Byte" , "byte" },
            { "System.Char" , "char" },
            { "System.Management.Automation.CmdletBindingAttribute" , "CmdletBinding" },
            { "System.DateTime" , "datetime" },
            { "System.Decimal" , "decimal" },
            { "System.Double" , "double" },
            { "System.Management.Automation.DscResourceAttribute" , "DscResource" },
            { "System.Management.Automation.ExperimentAction" , "ExperimentAction" },
            { "System.Management.Automation.ExperimentalAttribute" , "Experimental" },
            { "System.Management.Automation.ExperimentalFeature" , "ExperimentalFeature" },
            { "System.Single" , "float" },
            { "System.Guid" , "guid" },
            { "System.Collections.Hashtable" , "hashtable" },
            { "System.Int32" , "int" },
            { "System.Int16" , "short" },
            { "System.Int64" , "long" },
            { "Microsoft.Management.Infrastructure.CimInstance" , "ciminstance" },
            { "Microsoft.Management.Infrastructure.CimClass" , "cimclass" },
            { "Microsoft.Management.Infrastructure.CimType" , "cimtype" },
            { "Microsoft.Management.Infrastructure.CimConverter" , "cimconverter" },
            { "System.Net.IPEndPoint" , "IPEndpoint" },
            { "System.Management.Automation.Language.NoRunspaceAffinityAttribute" , "NoRunspaceAffinity" },
            { "System.Management.Automation.Language.NullString" , "NullString" },
            { "System.Management.Automation.OutputTypeAttribute" , "OutputType" },
            { "System.Security.AccessControl.ObjectSecurity" , "ObjectSecurity" },
            { "System.Collections.Specialized.OrderedDictionary" , "ordered" },
            { "System.Management.Automation.ParameterAttribute" , "Parameter" },
            { "System.Net.NetworkInformation.PhysicalAddress" , "PhysicalAddress" },
            { "System.Management.Automation.PSCredential" , "pscredential" },
            { "System.Management.Automation.PSDefaultValueAttribute" , "PSDefaultValue" },
            { "System.Management.Automation.PSListModifier" , "pslistmodifier" },
            { "System.Management.Automation.PSObject" , "psobject" },
            { "System.Management.Automation.PSPrimitiveDictionary" , "psprimitivedictionary" },
            { "System.Management.Automation.PSReference" , "ref" },
            { "System.Management.Automation.PSTypeNameAttribute" , "PSTypeNameAttribute" },
            { "System.Text.RegularExpressions.Regex" , "regex" },
            { "System.Management.Automation.DscPropertyAttribute" , "DscProperty" },
            { "System.SByte" , "sbyte" },
            { "System.String" , "string" },
            { "System.Management.Automation.SupportsWildcardsAttribute" , "SupportsWildcards" },
            { "System.Management.Automation.SwitchParameter" , "switch" }, // Should this be SwitchParameter?
            { "System.Globalization.CultureInfo" , "cultureinfo" },
            { "System.Numerics.BigInteger" , "bigint" },
            { "System.Security.SecureString" , "securestring" },
            { "System.TimeSpan" , "timespan" },
            { "System.UInt16" , "ushort" },
            { "System.UInt32" , "uint" },
            { "System.UInt64" , "ulong" },
            { "System.Uri" , "uri" },
            { "System.Management.Automation.ValidateCountAttribute" , "ValidateCount" },
            { "System.Management.Automation.ValidateDriveAttribute" , "ValidateDrive" },
            { "System.Management.Automation.ValidateLengthAttribute" , "ValidateLength" },
            { "System.Management.Automation.ValidateNotNullAttribute" , "ValidateNotNull" },
            { "System.Management.Automation.ValidateNotNullOrEmptyAttribute" , "ValidateNotNullOrEmpty" },
            { "System.Management.Automation.ValidateNotNullOrWhiteSpaceAttribute" , "ValidateNotNullOrWhiteSpace" },
            { "System.Management.Automation.ValidatePatternAttribute" , "ValidatePattern" },
            { "System.Management.Automation.ValidateRangeAttribute" , "ValidateRange" },
            { "System.Management.Automation.ValidateScriptAttribute" , "ValidateScript" },
            { "System.Management.Automation.ValidateSetAttribute" , "ValidateSet" },
            { "System.Management.Automation.ValidateTrustedDataAttribute" , "ValidateTrustedData" },
            { "System.Management.Automation.ValidateUserDriveAttribute" , "ValidateUserDrive" },
            { "System.Version" , "version" },
            { "System.Void" , "void" },
            { "System.Net.IPAddress" , "ipaddress" },
            { "System.Management.Automation.DscLocalConfigurationManagerAttribute" , "DscLocalConfigurationManager" },
            { "System.Management.Automation.WildcardPattern" , "WildcardPattern" },
            { "System.Security.Cryptography.X509Certificates.X509Certificate" , "X509Certificate" },
            { "System.Security.Cryptography.X509Certificates.X500DistinguishedName" , "X500DistinguishedName" },
            { "System.Xml.XmlDocument" , "xml" },
            { "Microsoft.Management.Infrastructure.CimSession" , "CimSession" },
            { "System.Net.Mail.MailAddress" , "mailaddress" },
            { "System.Management.Automation.SemanticVersion" , "semver" },
            { "System.Management.Automation.ScriptBlock" , "scriptblock" },
            { "Microsoft.PowerShell.Commands.PSPropertyExpression" , "pspropertyexpression" },
            { "System.Management.Automation.PSVariable" , "psvariable" },
            { "System.Type" , "type" },
            { "System.Management.Automation.PSModuleInfo" , "psmoduleinfo" },
            { "System.Management.Automation.PowerShell" , "powershell" },
            { "System.Management.Automation.Runspaces.RunspaceFactory" , "runspacefactory" },
            { "System.Management.Automation.Runspaces.Runspace" , "runspace" },
            { "System.Management.Automation.Runspaces.InitialSessionState" , "initialsessionstate" },
            { "System.Management.Automation.PSScriptMethod" , "psscriptmethod" },
            { "System.Management.Automation.PSScriptProperty" , "psscriptproperty" },
            { "System.Management.Automation.PSNoteProperty" , "psnoteproperty" },
            { "System.Management.Automation.PSAliasProperty" , "psaliasproperty" },
            { "System.Management.Automation.PSVariableProperty" , "psvariableproperty" },
        };

        public static bool TryGetTypeAbbreviation(string fullName, out string abbreviation)
        {
            if (TypeAbbreviations.TryGetValue(fullName, out abbreviation))
            {
                return true;
            }

            return false;
        }

        public static string GetParameterTemplateString(string paramName)
        {
            if (string.IsNullOrEmpty(paramName))
            {
                throw new ArgumentException("Parameter name cannot be null or empty.", nameof(paramName));
            }

            if (string.Equals(paramName, "Confirm", StringComparison.OrdinalIgnoreCase))
            {
                return Constants.ConfirmParameterDescription;
            }
            else if (string.Equals(paramName, "WhatIf", StringComparison.OrdinalIgnoreCase))
            {
                return Constants.WhatIfParameterDescription;
            }
            else
            {
                return string.Format(Constants.FillInParameterDescriptionTemplate, paramName);
            }
        }
    }
}