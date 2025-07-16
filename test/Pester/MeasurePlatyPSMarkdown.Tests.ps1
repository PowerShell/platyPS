# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

Describe "Export-MarkdownModuleFile" {
    BeforeAll {
        $files = Get-ChildItem $PSScriptRoot/assets -filter *.md
        $idents = $files.FullName | Measure-PlatyPSMarkdown
        $goodFile1 = $idents.Where({$_.FilePath -match "get-date.md$"})
        $goodFile2 = $idents.Where({$_.FilePath -match "Compare-CommandHelp.md$"})
        $mfPath = Import-MarkdownModuleFile $PSScriptRoot/assets/Microsoft.PowerShell.Archive.md |
            Export-MarkdownModuleFile -outputFolder $TESTDRIVE
    }

    It "Should identify all the '<fileType>' assets" -TestCases @(
        @{ fileType = "unknown"; expectedCount = 2 }
        @{ fileType = "CommandHelp"; expectedCount = 41 }
        @{ fileType = "ModuleFile"; expectedCount = 16 }
        @{ fileType = "V1Schema"; expectedCount = 52 }
        @{ fileType = "V2Schema"; expectedCount = 5 }
    ) {
        param ($fileType, $expectedCount)
        $idents.Where({($_.FileType -band $fileType) -eq $fileType}).Count | Should -Be $expectedCount
    }

    It "Should have proper diagnostics for get-date.md" {
        $goodFile1.DiagnosticMessages.Count | Should -Be 4
        $goodFile1.FileType | Should -match "v1schema"
        $goodFile1.DiagnosticMessages[-1].Message | Should -Match "PlatyPS.*schema.*marking as v1"
    }

    It "Should have proper diagnostics for Compare-CommandHelp.md" {
        $goodFile2.DiagnosticMessages.Count | Should -Be 2
        ($goodFile2.FileType -band "v2schema") -eq "v2schema" | Should -Be $true
        $goodFile2.DiagnosticMessages[-1].Message | Should -Be "document type found: cmdlet"
    }

    It "Should recognise a V2 module file" {
        $v2ModuleFile = Measure-PlatyPSMarkdown -Path $mfPath.Fullname
        $v2ModuleFile.Filetype | Should -Be "ModuleFile, V2Schema"
    }

}