# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

Describe "Export-MarkdownModuleFile" {
    BeforeAll {
        $modFiles = Get-ChildItem -File "${PSScriptRoot}/assets" | Where-Object { $_.extension -eq ".md" -and $_.name -notmatch "-" -and $_.Name -notmatch "Bad.Metadata.Order" }
        $modFileNames = $modFiles.Foreach({$_.Name})
        $modObjects = $modFiles | Import-MarkdownModuleFile
        $testModuleFile = $modObjects | Where-Object { $_.title -match 'Microsoft.PowerShell.Archive' }
    }

    Context "Basic Operation" {
        It "Should be able to export a module file" {
            $null = $modObjects[0] | Export-MarkdownModuleFile -outputFolder ${TestDrive}
            $expectedFile = "${TestDrive}/" + $modObjects[0].Module + ".md"
            $expectedFile | Should -Exist
        }

        It "Should produce a warning if the file already exists" {
            $testModuleFile | Export-MarkdownModuleFile -outputFolder ${TestDrive}
            $result = $testModuleFile | Export-MarkdownModuleFile -outputFolder ${TestDrive} 3>&1
            # this is a warning object, make it a string
            $result | Should -BeOfType System.Management.Automation.WarningRecord
            $result.Message | Should -Be "'Microsoft.PowerShell.Archive' exists, skipping. Use -Force to overwrite."
        }

        It "Should be able to export a number of module files" {
            $outputFiles = $modObjects | Export-MarkdownModuleFile -outputFolder "${TestDrive}/group"
            $outputFiles.Count | Should -Be $modObjects.Count
        }
    }

    Context "File Contents" {
        BeforeAll {
            $outputFile = $modObjects[3] | Export-MarkdownModuleFile -outputFolder ${TestDrive}
            $testMF = Import-MarkdownModuleFile $outputFile
        }

        # This relies on implementation of IEquatable
        It "An exported module file should be equivalent to its source using IEquatable" {
            $modObjects[3] -eq $testMF | Should -Be $true
        }

        # Now check the hard way
        It "Should have the same metadata value for key '<Name>'" -TestCases $(
            @{ Name = "Download Help Link" }
            @{ Name = "Help Version" }
            @{ Name = "Locale" }
            @{ Name = "Module Guid" }
            @{ Name = "Module Name" }
            @{ Name = "ms.date" }
            @{ Name = "PlatyPS schema version" }
            @{ Name = "title" }
            @{ Name = "document type" }
        ) {
            param ($name)
            $testMF.Metadata[$name] | Should -Be $modObjects[3].Metadata[$name]
        }

        It "Should have the same value for the property '<name>'" -TestCases $(
            @{ Name = "Description" }
            @{ Name = "Locale" }
            @{ Name = "Module" }
            @{ Name = "ModuleGuid" }
            @{ Name = "Title" }
        ) {
            param ($name)
            $testMF.$name | Should -Be $modObjects[3].$name
        }

        It "Should have the same values for the command '<name>'" -TestCases $(
            $offset = 0
            $modObjects[3].CommandGroups[0].Commands | foreach-object {
                @{ Offset = $offset; Name = $_.Name; Link = $_.Link; Description = $_.Description }
                $offset++
                }
        ) {
            param ($offset, $name, $link, $description)
            $testMF.CommandGroups[0].Commands[$offset].Name | Should -Be $name
            $testMF.CommandGroups[0].Commands[$offset].Link | Should -Be $link
            $testMF.CommandGroups[0].Commands[$offset].Description | Should -Be $description
        }
    }
}