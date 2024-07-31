# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

Describe "Scenario testing" {

    Context "New-MarkdownCommandHelp with module file without additional metadata" {
        BeforeAll {
            $moduleName = "Microsoft.PowerShell.Archive"
            $files = New-MarkdownCommandHelp -Module $moduleName -WithModulePage -output "${TESTDRIVE}/nometadata"
            $moduleFile = Import-MarkdownModuleFile -Path ($files[2])
            $commandHelp = $files[0,1] | Import-MarkdownCommandHelp
            $moduleInfo = Get-Module -List $moduleName
        }

        It "Should have created 3 files" {
            $files.Count | Should -Be 3
        }

        It "Should have created the files in the correct directory" {
            $files.Directory.Name | Sort-Object -unique | Should -Be $moduleName
        }

        It "Should have created the correct file '<name>'" -testcases @(
                @{ name = "Compress-Archive.md"; offset = 0 }
                @{ name = "Expand-Archive.md"; offset = 1 }
                @{ name = "${moduleName}.md"; offset = 2 }
            ) {
            param ($name, $offset)
            $files[$offset].Name | Should -Be $name
        }

        It "The module file should have the correct number of commands" {
            $moduleFile.CommandGroups[0].Commands.Count | Should -Be 2
        }

        It "The module file should include the correct command name '<name>'" -testcases @(
            @{ name = "Compress-Archive"; offset = 0 }
            @{ name = "Expand-Archive"; offset = 1 }
        ) {
            param ($name, $offset)
            $commandHelp[$offset].title | Should -Be $name
            $moduleFile.CommandGroups[0].Commands[$offset].name | Should -Be $name
        }

        It "The metadata should know the proper file content type for '<name>'" -testcases @(
            @{ name = $files[0].name; metadata = $commandHelp[0].Metadata; filetype = "cmdlet" }
            @{ name = $files[1].name; metadata = $commandHelp[1].Metadata; filetype = "cmdlet" }
            @{ name = $files[2].name; metadata = $moduleFile.Metadata; filetype = "module" }
        ) {
            param ( $name, $metadata, $filetype )
            $metadata['document type'] | Should -Be $filetype
        }

        It "Should have the proper metadata for <property> in the module file " -TestCases @(
            @{ property = "Help Version"; expectedValue = "1.0.0.0" }
            @{ property = "HelpInfoUri"; expectedValue = $moduleInfo.HelpInfoUri }
            @{ property = "Module Guid"; expectedValue = $moduleInfo.Guid.ToString() }
            @{ property = "PlatyPS schema version"; expectedValue = "2024-05-01" }
            @{ property = "title"; expectedValue = "Microsoft.PowerShell.Archive Module" }
        ) {
            param ($property, $expectedValue)
            $moduleFile.Metadata[$property] | Should -Be $expectedValue
        }
    } 

    Context "New-MarkdownCommandHelp with module file without additional metadata" {
        BeforeAll {
            $moduleName = "Microsoft.PowerShell.Archive"
            $files = New-MarkdownCommandHelp -Module $moduleName -WithModulePage -output "${TESTDRIVE}/addmetadata" -Metadata @{ p1 = 1; p2 = "two"; p3 = "ab","cd","ef"}
            $moduleFile = Import-MarkdownModuleFile -Path ($files[2])
            $commandHelp = $files[0,1] | Import-MarkdownCommandHelp
            $moduleInfo = Get-Module -List $moduleName
        }

        It "Should have created 3 files" {
            $files.Count | Should -Be 3
        }

        It "Should have created the files in the correct directory" {
            $files.Directory.Name | Sort-Object -unique | Should -Be $moduleName
        }

        It "Should have created the correct file '<name>'" -testcases @(
                @{ name = "Compress-Archive.md"; offset = 0 }
                @{ name = "Expand-Archive.md"; offset = 1 }
                @{ name = "${moduleName}.md"; offset = 2 }
            ) {
            param ($name, $offset)
            $files[$offset].Name | Should -Be $name
        }

        It "The module file should have the correct number of commands" {
            $moduleFile.CommandGroups[0].Commands.Count | Should -Be 2
        }

        It "The module file should include the correct command name '<name>'" -testcases @(
            @{ name = "Compress-Archive"; offset = 0 }
            @{ name = "Expand-Archive"; offset = 1 }
        ) {
            param ($name, $offset)
            $commandHelp[$offset].title | Should -Be $name
            $moduleFile.CommandGroups[0].Commands[$offset].name | Should -Be $name
        }

        It "The metadata should know the proper file content type for '<name>'" -testcases @(
            @{ name = $files[0].name; metadata = $commandHelp[0].Metadata; filetype = "cmdlet" }
            @{ name = $files[1].name; metadata = $commandHelp[1].Metadata; filetype = "cmdlet" }
            @{ name = $files[2].name; metadata = $moduleFile.Metadata; filetype = "module" }
        ) {
            param ( $name, $metadata, $filetype )
            $metadata['document type'] | Should -Be $filetype
        }

        It "Should have the proper metadata for '<property>' in the command help files" -TestCases @(
            @{ property = "PlatyPS schema version"; expectedValue = "2024-05-01" }
            @{ property = "p1"; expectedValue = 1 }
            @{ property = "p2"; expectedValue = "two" }
            @{ property = "p3"; expectedValue = "ab","cd","ef" }
        ) {
            param ($property, $expectedValue)
            $commandHelp[0].Metadata[$property] | Should -Be $expectedValue
            $commandHelp[1].Metadata[$property] | Should -Be $expectedValue
        }
    }   
}
