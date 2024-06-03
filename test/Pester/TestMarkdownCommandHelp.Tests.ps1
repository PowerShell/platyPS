# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

Describe "Test-MarkdownCommandHelp Tests" {
    It "Returns a boolean" {
        $result = Test-MarkdownCommandHelp (Join-Path $PSScriptRoot assets get-date.md)
        $result | Should -BeOfType boolean
    }

    It "Should report true with a good markdown file" {
        $result = Test-MarkdownCommandHelp (Join-Path $PSScriptRoot assets get-date.md)
        $result | Should -Be $true
    }

    It "Should report false with a bad markdown file" {
        $result = Test-MarkdownCommandHelp (Join-Path $PSScriptRoot assets bad-commandhelp.md)
        $result | Should -Be $false
    }

    It "Should report details with the -DetailView parameter" {
        $result = Test-MarkdownCommandHelp (Join-Path $PSScriptRoot assets get-date.md) -DetailView
        $result | Should -BeOfType Microsoft.PowerShell.PlatyPS.MarkdownCommandHelpValidationResult
        $result.Path | Should -BeOfType string
        $result.Path | Should -Exist
        $result.IsValid | Should -BeOfType bool
        ,$result.Messages | Should -BeOfType "System.Collections.Generic.List[string]"
    }

    It "Should report no failures in the details with a good markdown file" {
        $result = Test-MarkdownCommandHelp (Join-Path $PSScriptRoot assets get-date.md) -DetailView
        $result.Messages | Should -Not -cMatch "FAIL"
    }

    It "Should report have '<line>' when viewing details of a good markdown file"  -TestCases @(
        @{ line = 'PASS: First element is a thematic break' },
        @{ line = 'PASS: SYNOPSIS found.' },
        @{ line = 'PASS: SYNTAX found.' },
        @{ line = 'PASS: DESCRIPTION found.' },
        @{ line = 'PASS: EXAMPLES found.' },
        @{ line = 'PASS: PARAMETERS found.' },
        @{ line = 'PASS: INPUTS found.' },
        @{ line = 'PASS: OUTPUTS found.' },
        @{ line = 'PASS: NOTES found.' },
        @{ line = 'PASS: RELATED LINKS found.' }
        ) {
        param ($line)
        $result = Test-MarkdownCommandHelp (Join-Path $PSScriptRoot assets get-date.md) -DetailView
        $result.Messages | Should -Contain $line
    }
}
