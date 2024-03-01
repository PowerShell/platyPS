// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.PowerShell.PlatyPS.Model
{
    public enum DiagnosticSeverity
    {
        /// <summary>
        ///     The diagnostic message represents an error.
        /// </summary>
        Error = 0,

        /// <summary>
        ///     The diagnostic message represents a warning.
        /// </summary>
        Warning = 1,

        /// <summary>
        ///     The diagnostic message represents an information message.
        /// </summary>
        Information = 2,

        /// <summary>
        ///     The diagnostic message represents a hint.
        /// </summary>
        Hint = 3
    }
}