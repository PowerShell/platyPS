# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

function normalizeEnds([string]$text)
{
    $text -replace "`r`n?|`n", "`r`n"
}
