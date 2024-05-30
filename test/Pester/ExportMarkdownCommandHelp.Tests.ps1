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
            @{ Key = "document type"; Value = "cmdlet" }
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
            $ch | Export-MarkdownCommandHelp -OutputFolder ${TestDrive} -Metadata @{ "PlatyPS schema version" = $newValue } -Force -WarningVariable warnVar
            $ch2 = Import-MarkdownCommandHelp "${TestDrive}/Get-Date.md"
            $ch2.Metadata['PlatyPS schema version'] | Should -Be $ch.Metadata['PlatyPS schema version']
            $ch2.Metadata.Keys.Count | Should -Be $ch.Metadata.Keys.Count
            $warnVar[0].Message | Should -Be "Metadata key 'PlatyPS schema version' may not be overridden"
        }
    }

    Context "File Content - Syntax" {
        BeforeAll {
            $ch | export-markdowncommandhelp -force -outputfolder ${TestDrive}
            $ch2 = Import-MarkdownCommandHelp "${TestDrive}/Get-Date.md"
            $testCases = 0..3 | Foreach-object { @{ Number = $_ } }
        }

        It "Should have the same number of syntax statements" {
            $expected = $ch.Syntax.Count
            $ch2.Syntax.Count | Should -Be $expected
        }

        It "The syntax objects should be the same for syntax '<number>'" -TestCases $testCases {
            param ($number)
            $ch.syntax[$number] -eq $ch2.syntax[$number] | Should -Be $true
        }

        It "The string of the syntax object should be the same for '<number>'" -TestCases $testCases {
            param ($number)
            $expected = $ch.syntax[$number].ToString()
            $observed = $ch2.syntax[$number].ToString()
            $expected | Should -Be $observed
        }
    }

    Context "File Content - Notes" {
        BeforeAll {
            $ch | export-markdowncommandhelp -force -outputfolder ${TestDrive}
            $ch2 = Import-MarkdownCommandHelp "${TestDrive}/Get-Date.md"
        }

        It "Exported notes should be the same as the source" {
            $ch.Notes | Should -Be $ch2.Notes
        }
    }

    Context "File Content - Examples" {
        BeforeAll {
            $ch | export-markdowncommandhelp -force -outputfolder ${TestDrive}
            $ch2 = Import-MarkdownCommandHelp "${TestDrive}/Get-Date.md"
            $expectedCount = $ch.Examples.Count
            $testCases = 0..($expectedCount - 1) | Foreach-Object {
                @{ number = $_ }
            }
        }

        It "Should have the same number of examples" {
            $expectedCount | Should -Be $ch2.Examples.Count
        }

        It "Example '<number>' should be the same." -TestCases $testCases {
            param ($number)
            $ch.Examples[$number] -eq $ch2.Examples[$number] | Should -Be $true
        }

    }

    Context "File Content - Parameters" {
        BeforeAll {
            $ch | export-markdowncommandhelp -force -outputfolder ${TestDrive}
            $ch2 = Import-MarkdownCommandHelp "${TestDrive}/Get-Date.md"
            $expectedCount = $ch.Parameters.Count
            $testCases = 0..($expectedCount - 1) | Foreach-Object {
                @{ number = $_ }
            }
        }

        It "Should have the same number of parameters" {
            $expectedCount | Should -Be $ch2.Parameters.Count
        }

        It "Parameter '<number>' should be the same." -TestCases $testCases {
            param ($number)
            $ch.Parameters[$number] -eq $ch2.Parameters[$number] | Should -Be $true
        }
    }

    Context "File Content - Input/Output" {
        BeforeAll {
            $ch | export-markdowncommandhelp -force -outputfolder ${TestDrive}
            $ch2 = Import-MarkdownCommandHelp "${TestDrive}/Get-Date.md"
            $expectedInputCount = $ch.Inputs.Count
            $expectedOutputCount = $ch.Outputs.Count
            $inputTestCases = 0..($expectedInputCount - 1) | Foreach-Object { @{ number = $_ } }
            $outputTestCases = 0..($expectedOutputCount - 1) | Foreach-Object { @{ number = $_ } }
        }

        It "Should have the same number of Inputs" {
            $ch2.Inputs.Count | Should -Be $expectedInputCount
        }

        It "Input '<number>' should be the same." -TestCases $inputTestCases {
            param ($number)
            $ch.Inputs[$number] -eq $ch2.Inputs[$number] | Should -Be $true
        }

        It "Should have the same number of Outputs" {
            $ch2.Outputs.Count | Should -Be $expectedOutputCount
        }

        It "Output '<number>' should be the same." -TestCases $outputTestCases {
            param ($number)
            $ch.Outputs[$number] -eq $ch2.Outputs[$number] | Should -Be $true
        }
    }

    Context "File Content - Related Links" {
        BeforeAll {
            $ch | export-markdowncommandhelp -force -outputfolder ${TestDrive}
            $ch2 = Import-MarkdownCommandHelp "${TestDrive}/Get-Date.md"
            $expectedCount = $ch.RelatedLinks.Count
            $testCases = 0..($expectedCount - 1) | Foreach-Object {
                @{ number = $_ }
            }
        }

        It "Should have the same number of Links" {
            $expectedCount | Should -Be $ch2.RelatedLinks.Count
        }

        It "Link '<number>' should be the same." -TestCases $testCases {
            param ($number)
            $ch.RelatedLinks[$number] -eq $ch2.RelatedLinks[$number] | Should -Be $true
        }
    }
}
