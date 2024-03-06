# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

Describe "Test Diagnostic Messages" {
    BeforeAll {
        $filePath = Join-Path $PSScriptRoot assets get-date.md
        $ch = Import-MarkdownCommandHelp $filePath
    }

    It "The CommandHelp object should have diagnostic messages" {
        $ch.Diagnostics | Should -Not -BeNullOrEmpty
    }

    It "Should properly report on the state of the parse" {
        $ch.Diagnostics.HadErrors | Should -Be $False
    }

    It "Should properly report the path to the file" {
        $ch.Diagnostics.FileName | Should -Be $filePath
    }

    It "Should populate the Messages property" {
        $ch.Diagnostics.Messages.Count | Should -Be 33
    }

    It "Should have the correct number of Information entries" {
        $ch.Diagnostics.Messages.Where({$_.Severity -eq "Information"}).Count | Should -Be 33
    }

    # this is 13 parameter names and the initial parameter finding which reports the number
    # plus the common parameters
    It "Should find the correct number of parameters" {
        $ch.Diagnostics.Messages.Where({$_.Source -eq "Parameter"}).Count | Should be 15
    }

    It "Should find the common parameters" {
        $ch.Diagnostics.Messages.Where({$_.Message -match "CommonParameters"}).Count | Should -Be 1
    }

    It "Should find other parameter names" {
        $parameterNames = "AsUTC", "Date", "Day", "DisplayHint", "Format", "Hour",
            "Millisecond", "Minute", "Month", "Second", "UFormat", "UnixTimeSeconds", "Year"
        $pattern = $parameterNames -join "|"
        $ch.Diagnostics.Messages.Where({$_.Message -match $pattern}).Count | Should -Be 13
    }

    Context "Error Conditions" {
        BeforeAll {
            $errorPath = Join-Path $PSScriptRoot assets get-datebad.md
            $errorCh = Import-MarkdownCommandHelp $errorPath
        }

        It "Should report on the missing metadata" {
            $errorCh.Diagnostics.Messages.Where({$_.Source -eq "Metadata" -and $_.Identifier -match "schema"}).Severity | Should -Be "Error"
        }

        It "Should report on the missing Synopsis" {
            $errorCh.Diagnostics.Messages.Where({$_.Source -eq "Synopsis"}).Severity | Should -Be "Error"

        }
    }
}
