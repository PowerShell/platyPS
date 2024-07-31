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
            # this is the string from the MAML file.
            $expected = "Adds custom properties and methods to an instance of a PowerShell object."
            $importedCmds.Where({$_.title -eq "Add-Member"}).Synopsis | Should -Be $expected
        }

        Context "Metadata checks" {
            It "has the proper metadata value for '<key>' for the Add-Member cmdlet" -testcases @(
                @{ Key = 'document type'; Value = 'cmdlet' }
                @{ Key = 'title'; Value = 'Add-Member' }
                @{ Key = 'Module Name'; Value = 'Microsoft.PowerShell.Commands.Utility' }
                @{ Key = 'Locale'; Value = 'en-US' }
                @{ Key = 'PlatyPS schema version'; Value = '2024-05-01' }
                @{ Key = 'HelpUri'; Value = 'https://learn.microsoft.com/powershell/module/microsoft.powershell.utility/add-member?view=powershell-7.3&WT.mc_id=ps-gethelp' }
                @{ Key = 'ms.date'; Value = '07/31/2024' }
                @{ Key = 'external help file'; Value = 'Microsoft.PowerShell.Commands.Utility.dll-Help.xml' }
            ) {
                param ([string]$Key, [string]$Value)
                $cmdlet = $importedCmds.Where({$_.title -eq "add-member"})
                $cmdlet.Metadata[$key] | Should -Be $Value
            }
        }
    }
}