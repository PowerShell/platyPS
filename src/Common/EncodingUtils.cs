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

namespace Microsoft.PowerShell.PlatyPS
{
    internal static class EncodingConversion
    {
        internal const string ANSI = "ansi";
        internal const string Ascii = "ascii";
        internal const string BigEndianUnicode = "bigendianunicode";
        internal const string BigEndianUtf32 = "bigendianutf32";
        internal const string Default = "default";
        internal const string OEM = "oem";
        internal const string String = "string";
        internal const string Unicode = "unicode";
        internal const string Unknown = "unknown";
        internal const string Utf7 = "utf7";
        internal const string Utf8 = "utf8";
        internal const string Utf8Bom = "utf8BOM";
        internal const string Utf8NoBom = "utf8NoBOM";
        internal const string Utf32 = "utf32";

        internal static readonly string[] TabCompletionResults = {
                ANSI, Ascii, BigEndianUnicode, BigEndianUtf32, /* OEM, */ Unicode, Utf7, Utf8, Utf8Bom, Utf8NoBom, Utf32
            };

        internal static readonly Dictionary<string, Encoding> encodingMap = new(StringComparer.OrdinalIgnoreCase)
        {
            { ANSI, Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.ANSICodePage) },
            { Ascii, Encoding.ASCII },
            { BigEndianUnicode, Encoding.BigEndianUnicode },
            { BigEndianUtf32, new UTF32Encoding(bigEndian: true, byteOrderMark: true) },
            { Default, Encoding.Default },
            // { OEM, ClrFacade.GetOEMEncoding() },
            { String, Encoding.Unicode },
            { Unicode, Encoding.Unicode },
            { Unknown, Encoding.Unicode },
#pragma warning disable SYSLIB0001
            { Utf7, Encoding.UTF7 },
#pragma warning restore SYSLIB0001
            { Utf8, Encoding.Default },
            { Utf8Bom, Encoding.UTF8 },
            { Utf8NoBom, Encoding.Default },
            { Utf32, Encoding.UTF32 },
        };

        /// <summary>
        /// Retrieve the encoding parameter from the command line
        /// it throws if the encoding does not match the known ones.
        /// </summary>
        /// <returns>A System.Text.Encoding object (null if no encoding specified).</returns>
        internal static Encoding? Convert(Cmdlet cmdlet, string encoding)
        {
            if (string.IsNullOrEmpty(encoding))
            {
                // no parameter passed, default to UTF8
                return Encoding.Default;
            }

            if (encodingMap.TryGetValue(encoding, out Encoding foundEncoding))
            {
                // Write a warning if using utf7 as it is obsolete in .NET5
                if (string.Equals(encoding, Utf7, StringComparison.OrdinalIgnoreCase))
                {
                    cmdlet.WriteWarning("Utf7 encoding is obsolete");
                }

                return foundEncoding;
            }

            // error condition: unknown encoding value
            string validEncodingValues = string.Join(", ", TabCompletionResults);
            string msg = @"Encoding '{Encoding.Name} is not valid.";

            ErrorRecord errorRecord = new ErrorRecord(
                new ArgumentException("Encoding"),
                "WriteToFileEncodingUnknown",
                ErrorCategory.InvalidArgument,
                null);

            errorRecord.ErrorDetails = new ErrorDetails(msg);
            cmdlet.ThrowTerminatingError(errorRecord);

            return null;
        }

        /// <summary>
        /// Warn if the encoding has been designated as obsolete.
        /// </summary>
        /// <param name="cmdlet">A cmdlet instance which is used to emit the warning.</param>
        /// <param name="encoding">The encoding to check for obsolescence.</param>
        internal static void WarnIfObsolete(Cmdlet cmdlet, Encoding encoding)
        {
            // Check for UTF-7 by checking for code page 65000
            // See: https://learn.microsoft.com/dotnet/core/compatibility/corefx#utf-7-code-paths-are-obsolete
            if (encoding != null && encoding.CodePage == 65000)
            {
                cmdlet.WriteWarning("Utf7 encoding is obsolete.");
            }
        }
    }

    /// <summary>
    /// To make it easier to specify -Encoding parameter, we add an ArgumentTransformationAttribute here.
    /// When the input data is of type string and is valid to be converted to System.Text.Encoding, we do
    /// the conversion and return the converted value. Otherwise, we just return the input data.
    /// </summary>
    internal sealed class ArgumentToEncodingTransformationAttribute : ArgumentTransformationAttribute
    {
        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            switch (inputData)
            {
                case string stringName:
                    if (EncodingConversion.encodingMap.TryGetValue(stringName, out Encoding foundEncoding))
                    {
                        return foundEncoding;
                    }
                    else
                    {
                        return Encoding.GetEncoding(stringName);
                    }
                case int intName:
                    return Encoding.GetEncoding(intName);
            }

            return inputData;
        }
    }

    /// <summary>
    /// Provides the set of Encoding values for tab completion of an Encoding parameter.
    /// </summary>
    internal sealed class ArgumentEncodingCompletionsAttribute : ArgumentCompletionsAttribute
    {
        public ArgumentEncodingCompletionsAttribute() : base(
            EncodingConversion.ANSI,
            EncodingConversion.Ascii,
            EncodingConversion.BigEndianUnicode,
            EncodingConversion.BigEndianUtf32,
            // EncodingConversion.OEM,
            EncodingConversion.Unicode,
            EncodingConversion.Utf7,
            EncodingConversion.Utf8,
            EncodingConversion.Utf8Bom,
            EncodingConversion.Utf8NoBom,
            EncodingConversion.Utf32
        )
        { }
    }

        /// <summary>
    /// This attribute is used to specify an argument completions for a parameter of a cmdlet or function
    /// based on string array.
    /// <example>
    ///     [Parameter()]
    ///     [ArgumentCompletions("Option1","Option2","Option3")]
    ///     public string Noun { get; set; }
    /// </example>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ArgumentCompletionsAttribute : Attribute
    {
        private readonly string[] _completions;

        /// <summary>
        /// Initializes a new instance of the ArgumentCompletionsAttribute class.
        /// </summary>
        /// <param name="completions">List of complete values.</param>
        /// <exception cref="ArgumentNullException">For null arguments.</exception>
        /// <exception cref="ArgumentOutOfRangeException">For invalid arguments.</exception>
        public ArgumentCompletionsAttribute(params string[] completions)
        {
            if (completions == null)
            {
                throw new ArgumentNullException(nameof(completions));
            }

            if (completions.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(completions), string.Join(", ", completions));
            }

            _completions = completions;
        }

        /// <summary>
        /// The function returns completions for arguments.
        /// </summary>
        public IEnumerable<CompletionResult> CompleteArgument(string commandName, string parameterName, string wordToComplete, CommandAst commandAst, IDictionary fakeBoundParameters)
        {
            var wordToCompletePattern = WildcardPattern.Get(string.IsNullOrWhiteSpace(wordToComplete) ? "*" : wordToComplete + "*", WildcardOptions.IgnoreCase);

            foreach (var str in _completions)
            {
                if (wordToCompletePattern.IsMatch(str))
                {
                    yield return new CompletionResult(str, str, CompletionResultType.ParameterValue, str);
                }
            }
        }
    }

    internal class EncodingCompleter : IArgumentCompleter
    {
        IEnumerable<CompletionResult> IArgumentCompleter.CompleteArgument(string commandName, string parameterName, string wordToComplete, CommandAst commandAst, IDictionary fakeBoundParameters)
        {
            return GetAllowedNames().
                Where(new WildcardPattern(wordToComplete+"*",WildcardOptions.IgnoreCase).IsMatch).
                Select(s => new CompletionResult(s));
        }

        private static string[] GetAllowedNames() {
            return new string[] { "ansi", "ascii", "bigendianunicode", "bigendianutf32", "unicode", "utf7", "utf8", "utf8BOM", "utf8NoBOM", "utf32" };
        }
    }
}
