# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

Describe "Import-YamlHelp tests" {
    Context "Basic tests" {
        BeforeAll {
            $mamlFile = "${PSScriptRoot}/assets/Microsoft.PowerShell.Commands.Utility.dll-Help.xml"
            $importedCmds = Import-MamlHelp $mamlFile
            $cmdlet = $importedCmds.Where({$_.title -eq "add-member"})
        }

        Context "General tests" {
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
        }

        Context "Metadata checks" {
            It "has the proper metadata value for '<key>' for the Add-Member cmdlet" -testcases @(
                @{ Key = 'document type'; Value = 'cmdlet' }
                @{ Key = 'title'; Value = 'Add-Member' }
                @{ Key = 'Module Name'; Value = 'Microsoft.PowerShell.Commands.Utility' }
                @{ Key = 'Locale'; Value = 'en-US' }
                @{ Key = 'PlatyPS schema version'; Value = '2024-05-01' }
                @{ Key = 'HelpUri'; Value = 'https://learn.microsoft.com/powershell/module/microsoft.powershell.utility/add-member?view=powershell-7.3&WT.mc_id=ps-gethelp' }
                @{ Key = 'ms.date'; Value = Get-Date -f "MM/dd/yyyy" }
                @{ Key = 'external help file'; Value = 'Microsoft.PowerShell.Commands.Utility.dll-Help.xml' }
            ) {
                param ([string]$Key, [string]$Value)
                $cmdlet.Metadata[$key] | Should -Be $Value
            }
        }

        Context "Inputs/Outputs check" {
            It "has the proper Inputs for the '<name>' cmdlet" -testcases @(
                @{ name = 'Add-Member'; expectedValues = 'System.Management.Automation.PSObject' },
                @{ name = 'Get-PSBreakpoint'; expectedValues = 'System.Int32', 'Microsoft.PowerShell.Commands.BreakpointType' }
            ) {
                param ([string]$name, [string[]]$expectedValues)

                $values = $importedCmds.Where({$_.Title -eq $name}).Inputs.Typename
                $values | Should -BeExactly $expectedValues
            }

            It "has the proper Outputs for the '<name>' cmdlet" -testcases @(
                @{ name = 'Add-Member'; expectedValues = 'None', 'System.Object' },
                @{ name = 'Get-PSBreakpoint'; expectedValues = 'System.Management.Automation.CommandBreakpoint',
                                                               'System.Management.Automation.LineBreakpoint',
                                                               'System.Management.Automation.VariableBreakpoint',
                                                               'System.Management.Automation.Breakpoint' }
            ) {
                param ([string]$name, [string[]]$expectedValues)

                $values = $importedCmds.Where({$_.Title -eq $name}).Outputs.Typename
                $values | Should -BeExactly $expectedValues
            }
        }
    }
}
