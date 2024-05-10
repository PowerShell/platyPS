# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

Describe "Import-YamlHelp tests" {
    Context "Basic tests" {
        BeforeAll {
            $mamlFile = "${PSScriptRoot}/assets/Microsoft.PowerShell.Commands.Utility.dll-Help.xml"
            $importedCmds = Import-MamlHelp $mamlFile
        }
        It "Can import a MAML file with the proper number of commands" {
            $expectedCount = (Select-String "<command:command " $mamlFile).Count
            $importedCmds.Count | Should -Be $expectedCount
        }

        It "Identifies the imported commands" {
            $expectedNames = Select-String "<command:name>" $mamlFile | foreach-object { $_.Line -replace ".*>(.*)<.*",'$1' }
            $importedCmds.title | Should -Be $expectedNames
        }

        It "Identifies the command which does not have CmdletBinding" {
            $importedCmds.Where({!$_.HasCmdletBinding}) | Should -Be "Show-Command"
        }

        It "Correctly retrieves the synopsis for Add-Member" {
            $importedCmds.Where({$_.title -eq "Add-Member"}).Synopsis | Should -Be ((Get-Help Add-Member).Synopsis)
        }
    }
}