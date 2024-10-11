# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

Describe "Miscellaneous tests" {
    Context "Alias preservation" {
        BeforeAll {
            $cmdHelpPath = [io.path]::Combine($PSScriptRoot, "assets", "Get-ChildItem.V2.md")
            $ch_md = Import-MarkdownCommandHelp $cmdHelpPath
            $fPath = $ch_md | Export-YamlCommandHelp -output $TESTDRIVE -Force
            $ch_yaml = Import-YamlCommandHelp $fPath
        }

        It "Should preserve the aliases" {
            $ch_md.Aliases | Should -Not -BeNullOrEmpty
            $ch_md.Aliases | Should -Be $ch_yaml.Aliases
        }

        It "Should have the correct diagnostic message from the markdown parse" {
            $message = $ch_md.Diagnostics.Messages.Where({$_.source -eq "alias" -and $_.Identifier -match "length"})
            $message | Should -Not -BeNullOrEmpty
            $message.Identifier | Should -Match "118"
        }

    }
}
