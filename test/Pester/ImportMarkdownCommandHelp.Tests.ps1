Describe 'Import-MarkdownCommandHelp Tests' {
    BeforeAll {
        $assetDir = Join-Path $PSScriptRoot 'assets'
        $badMarkdownPath = Join-Path $assetDir 'bad-commandhelp.md'
        $goodMarkdownPath = Join-Path $assetDir 'get-date.md'
    }

    It 'Should import a valid markdown file' {
        $result = Import-MarkdownCommandHelp -Path $goodMarkdownPath
        $result | Should -Not -BeNullOrEmpty
        $result.ToString() | Should -Be "Get-Date"
    }


    It 'Should throw an error for an invalid markdown file' {
        Import-MarkdownCommandHelp -ErrorVariable BadMarkdown -Path $badMarkdownPath -ErrorAction SilentlyContinue
        $badMarkdown | Should -BeOfType [System.Management.Automation.ErrorRecord]
        $badMarkdown.FullyQualifiedErrorId | Should -Be "FailedToImportMarkdown,Microsoft.PowerShell.PlatyPS.ImportMarkdownHelpCommand"
    }

    Context 'Validate elements of the imported commandhelp object' {
        BeforeAll {
            $result = Import-MarkdownCommandHelp -Path $goodMarkdownPath
            $metadata = $result.GetType().GetProperty('Metadata', [System.Reflection.BindingFlags]'NonPublic,Instance').GetValue($result, $null)
        }

        It 'Should be a valid CommandHelp object' {
            $result.GetType().FullName | Should -Be "Microsoft.PowerShell.PlatyPS.Model.CommandHelp"
        }

        It "Should have the correct metadata value for '<name>'" -TestCases @(
            @{ name = "external help file"; expectedValue = "Microsoft.PowerShell.Commands.Utility.dll-Help.xml" }
            @{ name = "Locale"; expectedValue = "en-US" }
            @{ name = "Module Name"; expectedValue = "Microsoft.PowerShell.Utility" }
            @{ name = "ms.date"; expectedValue = "12/12/2022" }
            @{ name = "online version"; expectedValue = "https://learn.microsoft.com/powershell/module/microsoft.powershell.utility/get-date?view=powershell-7.2&WT.mc_id=ps-gethelp" }
            @{ name = "schema"; expectedValue = "2.0.0" }
            @{ name = "title"; expectedValue = "Get-Date" }
        ) {
            param ($name, $expectedValue)
            $metadata[$name] | Should -Be $expectedValue

        }
    }
}
