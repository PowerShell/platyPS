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

        Context "Syntax parameters checks" {
            It "Add-Member cmdlet has correct number of syntax parameters" {
                $cmdlet.Syntax[0].SyntaxParameters.Count | Should -be 8
            }

            It "'<Name>' of syntax parameter for Add-Member cmdlet has correct property values" -testcases @(
                @{ Name = 'MemberType';  Type = 'System.Management.Automation.PSMemberTypes';   Position = '0';     IsMandatory = $true;  IsPositional = $true;  IsSwitchParameter = $false },
                @{ Name = 'Name';        Type = 'System.String';                                Position = '1';     IsMandatory = $true;  IsPositional = $true;  IsSwitchParameter = $false },
                @{ Name = 'Value';       Type = 'System.Object';                                Position = '2';     IsMandatory = $false; IsPositional = $true;  IsSwitchParameter = $false },
                @{ Name = 'SecondValue'; Type = 'System.Object';                                Position = '3';     IsMandatory = $false; IsPositional = $true;  IsSwitchParameter = $false },
                @{ Name = 'Force';       Type = 'System.Management.Automation.SwitchParameter'; Position = 'named'; IsMandatory = $false; IsPositional = $false; IsSwitchParameter = $true },
                @{ Name = 'InputObject'; Type = 'System.Management.Automation.PSObject';        Position = 'named'; IsMandatory = $true;  IsPositional = $false; IsSwitchParameter = $false },
                @{ Name = 'PassThru';    Type = 'System.Management.Automation.SwitchParameter'; Position = 'named'; IsMandatory = $false; IsPositional = $false; IsSwitchParameter = $true },
                @{ Name = 'TypeName';    Type = 'System.String';                                Position = 'named'; IsMandatory = $false; IsPositional = $false; IsSwitchParameter = $false }
            ) {
                param ([string]$Name, [string]$Type, [string]$Position, [bool]$IsMandatory, [bool]$IsPositional, [bool]$IsSwitchParameter)
                $syntaxParam = $cmdlet.Syntax[0].SyntaxParameters.Where({$_.ParameterName -eq $Name})
                $syntaxParam.ParameterName | Should -be $Name
                $syntaxParam.ParameterType | Should -be $Type
                $syntaxParam.Position | should -be $Position
                $syntaxParam.IsMandatory | should -be $IsMandatory
                $syntaxParam.IsPositional | should -be $IsPositional
                $syntaxParam.IsSwitchParameter | should -be $IsSwitchParameter
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
