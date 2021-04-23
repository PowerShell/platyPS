$ErrorActionPreference = 'Stop'
. $PSScriptRoot/CommonFunction.ps1

Describe 'New-MarkdownHelp' {

    Context 'errors' {
        It 'throw when cannot find module' -Pending {
            { New-MarkdownHelp -Module __NON_EXISTING_MODULE -OutputFolder $TestDrive } |
            Should Throw ($LocalizedData.ModuleNotFound -f '__NON_EXISTING_MODULE')
        }

        It 'throw when cannot find module' -Pending {
            { New-MarkdownHelp -command __NON_EXISTING_COMMAND -OutputFolder $TestDrive } |
            Should Throw ($LocalizedData.CommandNotFound -f '__NON_EXISTING_COMMAND')
        }
    }

    Context 'metadata' {
        It 'generates passed metadata' {
            $file = New-MarkdownHelp -metadata @{
                FOO = 'BAR'
            } -command New-MarkdownHelp -OutputFolder $TestDrive

            $h = Get-MarkdownMetadata $file
            $h['FOO'] | Should -BeExactly 'BAR'
        }

        It 'respects -NoMetadata' {
            $file = New-MarkdownHelp -command New-MarkdownHelp -OutputFolder $TestDrive -NoMetadata -Force
            Get-MarkdownMetadata $file.FullName | Should -BeNullOrEmpty
        }

        It 'errors on -NoMetadata and -Metadata' {
            { New-MarkdownHelp -command New-MarkdownHelp -OutputFolder $TestDrive -NoMetadata -Force -Metadata @{} } |
            Should -Throw -ErrorId 'NoMetadataAndMetadata,Microsoft.PowerShell.PlatyPS.NewMarkdownHelpCommand'
        }
    }

    Context 'encoding' {
        It 'writes appropriate encoding' {
            $file = New-MarkdownHelp -command New-MarkdownHelp -OutputFolder $TestDrive -Force -Encoding ([System.Text.Encoding]::UTF32)
            $content = Get-Content -path $file -Encoding UTF32 -Raw
            Get-MarkdownMetadata -Markdown $content | Should -Not -BeNullOrEmpty
        }
    }

    Context 'from platyPS module' {
        It 'creates few help files for platyPS' {
            $files = New-MarkdownHelp -Module PlatyPS -OutputFolder "$TestDrive/platyPS" -Force
            ($files | Measure-Object).Count | Should -BeGreaterOrEqual 2
        }
    }

    Context 'from module' {
        BeforeAll {
            New-Module -Name PlatyPSTestModule -ScriptBlock {
                function Get-AAAA {

                }

                function Get-AdvancedFn {
                    [CmdletBinding()]
                    param (

                    )
                }

                function Get-SimpleFn {
                    param (

                    )
                }

                function Set-BBBB {

                }

                # Set-Alias and New-Alias provide two different results
                # when `Get-Command -module Foo` is used to list commands.
                Set-Alias aaaaalias Get-AAAA
                Set-Alias bbbbalias Get-BBBB

                New-Alias -Name 'Fork-AAAA' -Value 'Get-AAAA' -Force

                Export-ModuleMember -Alias Fork-AAAA
                Export-ModuleMember -Alias aaaaalias
                Export-ModuleMember -Alias bbbbalias
                Export-ModuleMember -Function 'Get-AAAA', 'Get-AdvancedFn', 'Get-SimpleFn'

            } | Import-Module -Force

            $files = New-MarkdownHelp -Module PlatyPSTestModule -OutputFolder "$TestDrive\PlatyPSTestModule" -Force
        }

        AfterAll {
            Remove-Module PlatyPSTestModule -ErrorAction SilentlyContinue
        }

        It 'generates markdown files only for exported functions' -Skip:$IsUnix {
            $files | Should -HaveCount 3
            $files.Name | Should -BeIn 'Get-AAAA.md', 'Get-AdvancedFn.md', 'Get-SimpleFn.md'
        }

        It 'generates markdown that includes CommonParameters in advanced functions' {
            ($files | Where-Object -FilterScript { $_.Name -eq 'Get-AdvancedFn.md' }).FullName | Should -FileContentMatch '### CommonParameters'
        }

        It 'generates markdown that excludes CommonParameters from simple functions' {
            ($files | Where-Object -FilterScript { $_.Name -eq 'Get-SimpleFn.md' }).FullName | Should -FileContentMatch -Not '### CommonParameters'
        }
    }

    Context 'from command' {
        It 'creates 2 markdown files from command names' {
            $files = New-MarkdownHelp -Command @('New-MarkdownHelp', 'Get-MarkdownMetadata') -OutputFolder "$TestDrive/commands" -Force
            $files | Should -HaveCount 2
        }
    }

    Context 'from external script' {
        It 'fully qualified path' {
            $SeedData = @"
<#
.SYNOPSIS
    Synopsis Here.
.DESCRIPTION
    Description Here.
.INPUTS
    None
.OUTPUTS
    None
.EXAMPLE
    .\Invoke-HelloWorld.ps1
#>
Write-Host 'Hello World!'
"@
            Set-Content -Value $SeedData -Path "$TestDrive\Invoke-HelloWorld.ps1" -NoNewline
            $files = New-MarkdownHelp -Command "$TestDrive\Invoke-HelloWorld.ps1" -OutputFolder "$TestDrive\output" -Force
            $files | Should -HaveCount 1
        }

        It 'relative path' {
            $SeedData = @"
<#
.SYNOPSIS
    Synopsis Here.
.DESCRIPTION
    Description Here.
.INPUTS
    None
.OUTPUTS
    None
.EXAMPLE
    .\Invoke-HelloWorld.ps1
#>
Write-Host 'Hello World!'
"@
            Set-Content -Value $SeedData -Path "$TestDrive\Invoke-HelloWorld.ps1" -NoNewline
            $Location = Get-Location
            Set-Location $TestDrive
            $files = New-MarkdownHelp -Command "$TestDrive\Invoke-HelloWorld.ps1" -OutputFolder "$TestDrive\output" -Force
            Set-Location $Location
            $files | Should -HaveCount 1
        }
    }

    Context 'AlphabeticParamsOrder' {
        function global:Get-Alpha
        {
            param(
                [Switch]
                [Parameter(Position = 1)]
                $WhatIf,
                [string]
                [Parameter(Position = 2)]
                $CCC,
                [Parameter(Position = 0)]
                [uint64]
                $First,
                [string]
                $AAA,
                [Parameter(Position = 3)]
                [uint64]
                $Skip,
                [string]
                $BBB,
                [Parameter(Position = 4)]
                [switch]
                $IncludeTotalCount,
                [Switch]
                $Confirm
            )
        }

        It 'uses alphabetic order when specified' {

            $expectedOrder = normalizeEnds @'
### -AAA
### -BBB
### -CCC
### -Confirm
### -First
### -IncludeTotalCount
### -Skip
### -WhatIf

'@
            $files = New-MarkdownHelp -Command Get-Alpha -OutputFolder "$TestDrive\alpha" -Force -AlphabeticParamsOrder
            $files | Should -HaveCount 1
            normalizeEnds (Get-Content $files | Where-Object {$_.StartsWith('### -')} | Out-String) | Should -Be $expectedOrder
        }
    }

    Context 'Generated markdown features: comment-based help' {
        function global:Test-PlatyPSFunction
        {
            # comment-based help template from https://technet.microsoft.com/en-us/library/hh847834.aspx

             <#
            .SYNOPSIS
            Adds a file name extension to a supplied name.
            .DESCRIPTION
            Adds a file name extension to a supplied name.
            Takes any strings for the file name or extension.
            .PARAMETER Second
            Second parameter help description
            .OUTPUTS
            System.String. Add-Extension returns a string with the extension or file name.
            .EXAMPLE
            PS C:\> Test-PlatyPSFunction "File"
            File.txt
            .EXAMPLE
            PS C:\> Test-PlatyPSFunction "File" -First "doc"
            File.doc
            .LINK
            http://www.fabrikam.com/extension.html
            .LINK
            Set-Item
            #>

            param(
                [Switch]$Common,
                [Parameter(ParameterSetName="First", HelpMessage = 'First parameter help description')]
                [string]$First,
                [Parameter(ParameterSetName="Second")]
                [string]$Second
            )
        }

        $file = New-MarkdownHelp -Command Test-PlatyPSFunction -OutputFolder "$TestDrive\testAll1" -Force
        $content = Get-Content $file

        It 'generates markdown with correct parameter set names' {
            $content | Where-Object {$_ -eq 'Parameter Sets: (All)'} | Should -HaveCount 1
            $content | Where-Object {$_ -eq 'Parameter Sets: First'} | Should -HaveCount 1
            $content | Where-Object {$_ -eq 'Parameter Sets: Second'} | Should -HaveCount 1
        }

        It 'generates markdown with correct synopsis' {
            $content | Where-Object {$_ -eq 'Adds a file name extension to a supplied name.'} | Should -HaveCount 2
        }

        It 'generates markdown with correct help description specified by HelpMessage attribute' {
            $content | Where-Object {$_ -eq 'First parameter help description'} | Should -HaveCount 1
        }

        It 'generates markdown with correct help description specified by comment-based help' {
            $content | Where-Object {$_ -eq 'Second parameter help description'} | Should -HaveCount 1
        }

        It 'generates markdown with placeholder for parameter with no description' {
            $content | Where-Object {$_ -eq ($LocalizedData.ParameterDescription -f 'Common')} | Should -HaveCount 1
        }
    }
}
