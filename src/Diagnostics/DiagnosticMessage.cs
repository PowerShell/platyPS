// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.PowerShell.PlatyPS.Model
{
    /// <summary>
    ///     A diagnostic message.
    /// </summary>
    public class DiagnosticMessage
    {
        /// <summary>
        /// The message source.
        /// </summary>
        public DiagnosticMessageSource Source { get; set; }

        /// <summary>
        ///     The diagnostic message.
        /// </summary>
        public List<string> Message { get; set; }

        /// <summary>
        ///     The diagnostic message level.
        /// </summary>
        public DiagnosticSeverity Severity { get; set; }

        /// <summary>
        ///     The diagnostic message identifier; Something that identifies the the diagnostic message.
        ///     This could be the name of a parameter, or a specific piece of syntax.
        ///     This is useful for diagnostics that are common across multiple elements.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        ///     The line number to associate with the message.
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        ///    Create a new diagnostic message.
        /// </summary>
        public DiagnosticMessage()
        {
            Identifier = string.Empty;
            Source = DiagnosticMessageSource.General;
            Identifier = string.Empty;
            Severity = DiagnosticSeverity.Information;
            Message = new List<string>();
            Line = -1;
        }

        /// <summary>
        ///     Create a new diagnostic message.
        /// </summary>
        /// <param name="source">The Diagnostic message source.</param>
        /// <param name="message">The diagnostic message.</param>
        /// <param name="severity">The diagnostic message level.</param>
        /// <param name="identifier">The diagnostic message identifier.</param>
        /// <param name="line">The line of source to associate with the diagnostic.</param>
        public DiagnosticMessage(DiagnosticMessageSource source, string message, DiagnosticSeverity severity, string identifier, int line)
        {
            Source = source;
            Message = new List<string>();
            Message.Add(message);
            Severity = severity;
            Identifier = identifier;
            Line = line;
        }
    }
}