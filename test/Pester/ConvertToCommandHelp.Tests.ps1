# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

Describe "New-CommandHelp tests" {
    BeforeAll {
        $result = New-CommandHelp New-CommandHelp
    }

    It "Should return a command help object" {
        $result | Should -BeOfType "Microsoft.PowerShell.PlatyPS.Model.CommandHelp"
    }

    It "Should have the appropriate metadata properties and values '<key> = <value>'" -TestCases @(
        @{ Key = "content type"; Value = "cmdlet" }
        @{ Key = "title"; Value = "New-CommandHelp" }
        @{ Key = "Module Name"; Value = "microsoft.powershell.platyPS" }
        @{ Key = "Locale" ; Value = "{{ fill in locale }}" }
        @{ Key = "PlatyPS schema version"; Value = "2024-05-01" }
        @{ Key = "HelpUri"; Value = "" }
        @{ Key = "ms.date"; Value = Get-Date -Format "MM/dd/yyyy" }
        @{ Key = "external help file"; Value = "Microsoft.PowerShell.PlatyPS.dll-Help.xml" }
    ) {
        param ($key, $value)
        $result.Metadata[$key] | Should -Be $value
    }

    It "Should be possible to retrieve from multiple commands" {
        $cmdInfo = Get-Command -Module Microsoft.PowerShell.PlatyPS
        $chs = New-CommandHelp $cmdInfo
        $chs.Count | Should -Be $cmdInfo.Count
        $chs.title | Should -Be $cmdInfo.Name
    }

    It "Should be possible to retrieve from multiple commands via pipeline" {
        $cmdInfo = Get-Command -Module Microsoft.PowerShell.PlatyPS
        $chs = $cmdInfo | New-CommandHelp
        $chs.Count | Should -Be $cmdInfo.Count
        $chs.title | Should -Be $cmdInfo.Name
    }

}