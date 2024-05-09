# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

Describe "Export-MarkdownCommandHelp" {
    BeforeAll {
        $ch = Import-MarkdownCommandHelp "${PSScriptRoot}/assets/get-date.md"
    }

    Context "File creation" {
        BeforeAll {
            $outputFolder = ${TestDrive}.FullName
        }

        It "Creates a file if one does not exist" {
            Export-MarkdownCommandHelp -Command $ch -OutputFolder $outputFolder
            "${outputFolder}/Get-Date.md" | Should -Exist
        }

        It "Doesn't create a file if it already exists" {
            $mdPath = "${TestDrive}/Get-Date.md"
            if (test-path $mdPath)
            {
                Remove-Item -Force $mdPath
            }

            $file = New-Item -Type file "${TestDrive}/Get-Date.md"
            $w = Export-MarkdownCommandHelp -Command $ch -OutputFolder $outputFolder 3>&1
            $w | Should -BeOfType System.Management.Automation.WarningRecord
            "$w" | Should -Be "skipping Get-Date, use -Force to export."
            $fi = Get-ChildItem $file
            $fi.Length | Should -Be 0
        }

        It "Creates a file if it exists and -force is used" {
            $mdPath = "${TestDrive}/Get-Date.md"
            if (test-path $mdPath)
            {
                Remove-Item -Force $mdPath
            }

            $result1 = Export-MarkdownCommandHelp -Command $ch -OutputFolder $outputFolder
            start-sleep 1
            $result2 = Export-MarkdownCommandHelp -Command $ch -OutputFolder $outputFolder -Force
            $result1.Length | Should -Be $result2.Length
            $result1.LastWriteTime | Should -Not -Be $result2.LastWriteTime
        }
    }

    Context "File Content - Metadata" {
        BeforeAll {
            $ch | Export-MarkdownCommandHelp -OutputFolder $TestDrive
            $newCh = Import-MarkdownCommandHelp "${TestDrive}/Get-Date.md"
            $observedMetadata = $newCh.Metadata
        }

        It "Should have the correct number of Keys" {
            $observedMetadata.Keys.Count | Should -Be 8
        }

        It "Should have the correct keys and values '<key>'" -TestCases @(
            @{ Key = "external help file"; Value = "Microsoft.PowerShell.Commands.Utility.dll-Help.xml" }
            @{ Key = "Locale"; Value = "en-US" }
            @{ Key = "Module Name"; Value = "Microsoft.PowerShell.Utility" }
            @{ Key = "ms.date"; Value = "12/12/2022" }
            @{ Key = "HelpUri"; Value = "https://learn.microsoft.com/powershell/module/microsoft.powershell.utility/get-date?view=powershell-7.4&WT.mc_id=ps-gethelp" }
            @{ Key = "PlatyPS schema version"; Value = "2024-05-01" }
            @{ Key = "title"; Value = "Get-Date" }
            @{ Key = "content type"; Value = "cmdlet" }
        ) {
            param ($key, $value)
            $observedMetadata[$key] | Should -Be $value
        }

        It "Should add new metadata if supplied" {
            $newValue = "additional metadata!"
            $ch | Export-MarkdownCommandHelp -OutputFolder ${TestDrive} -Metadata @{ BAR = $newValue } -Force
            $ch2 = Import-MarkdownCommandHelp "${TestDrive}/Get-Date.md"
            $ch2.Metadata.Keys.Count | Should -Be 9
            $ch2.Metadata['BAR'] | Should -Be $newValue
        }

        It "Should not overwrite a protected metadata key" {
            $newValue = "additional metadata!"
            $ch | Export-MarkdownCommandHelp -OutputFolder ${TestDrive} -Metadata @{ "PlatyPS schema version" = $newValue } -Force
            $ch2 = Import-MarkdownCommandHelp "${TestDrive}/Get-Date.md"
            $ch2.Metadata.Keys.Count | Should -Be 9
            $ch2.Metadata['PlatyPS schema version'] | Should -Be $ch.Metadata['PlatyPS schema version']
        }
    }

    Context "File Content - Syntax" {

    }

    Context "File Content - Notes" {

    }

    Context "File Content - Parameters" {

    }

    Context "File Content - Input/Output" {

    }

    Context "File Content - Related Links" {

    }
}