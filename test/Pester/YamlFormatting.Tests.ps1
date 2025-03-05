# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

Describe 'Yaml formatting tests' {
    It 'Inputs output and notes should have empty array when empty' {
        Import-MarkdownCommandHelp -Path 'assets/New-EmptyCommand.md' | Export-YamlCommandHelp -OutputFolder $TestDrive
        $yaml = Get-Content "$TestDrive/New-EmptyCommand.yml" -Raw

        $inputPattern = if ($IsWindows) { 'inputs:\r\n- name: System\.String\r\n  description: ''' } else { 'inputs:\n- name: System\.String\n  description: ''' }
        $outputPattern = if ($IsWindows) { 'outputs:\r\n- name: System\.String\r\n  description: ''' } else { 'outputs:\n- name: System\.String\n  description: ''' }
        $notesPattern = "notes: ''"
        $linksPattern = "links: \[\]"

        $yaml | Should -Match $inputPattern
        $yaml | Should -Match $outputPattern
        $yaml | Should -Match $notesPattern
        $yaml | Should -Match $linksPattern
    }
}