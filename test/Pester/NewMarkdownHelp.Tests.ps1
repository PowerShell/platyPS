# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

$ErrorActionPreference = 'Stop'
. $PSScriptRoot/CommonFunction.ps1

Describe 'New-MarkdownHelp' {
    BeforeAll {
        $defaultMetadata = @{
            "external help file" = (Get-Command New-MarkdownHelp).HelpFile
            "Module Name" = (Get-Command New-MarkdownHelp).ModuleName
            "online version" = "1.2.3"
            aliases = "testalias"
            schema = "2.0.0"
        }

        $commonParameterNames = [System.Management.Automation.Internal.CommonParameters].GetProperties().ForEach({$_.Name})
    }

    Context 'errors' {
        It 'throw when cannot find module' {
            { New-MarkdownHelp -Module '__NON_EXISTING_MODULE' -OutputFolder $TestDrive } |
            Should -Throw -ErrorId 'ModuleNotFound,Microsoft.PowerShell.PlatyPS.NewMarkdownHelpCommand'
        }

        It 'throw when cannot find command' {
            { New-MarkdownHelp -Command '__NON_EXISTING_COMMAND' -OutputFolder $TestDrive } |
            Should -Throw -ErrorId 'CommandNotFound,Microsoft.PowerShell.PlatyPS.NewMarkdownHelpCommand'
        }

        It 'throw when cannot find maml file' {
            { New-MarkdownHelp -MamlFile '__NON_EXISTING_FILE' -OutputFolder $TestDrive } |
            Should -Throw -ErrorId 'FileNotFound,Microsoft.PowerShell.PlatyPS.NewMarkdownHelpCommand'
        }

        It 'throw when OutputFolder is not a folder' {
            $null = New-Item -ItemType File -Path "$TestDrive/somefile.txt"
            { New-MarkdownHelp -Command 'New-MarkdownHelp' -OutputFolder "$TestDrive/somefile.txt" } |
            Should -Throw -ErrorId 'PathIsNotFolder,Microsoft.PowerShell.PlatyPS.NewMarkdownHelpCommand'
        }
    }

    Context 'metadata' {
        It 'generates passed metadata' {
            $customMetadata = $defaultMetadata
            $customMetadata['FOO'] = 'BAR'
            $file = New-MarkdownHelp -metadata $defaultMetadata -command New-MarkdownHelp -OutputFolder $TestDrive

            $h = (Import-MarkdownCommandHelp $file).Metadata
            $h['FOO'] | Should -BeExactly 'BAR'
        }

        It 'Duplicate keys in metadata should use the provided metadata value' -Pending {
            $mdArgs = @{
                Command = "New-MarkdownHelp"
                OutputFolder = $TestDrive
                Metadata = @{ "Module Name" = 'FOO' }
            }
            $expectedErrorId = 'NonUniqueMetadataKey,Microsoft.PowerShell.PlatyPS.NewMarkdownHelpCommand'
            { New-MarkdownHelp @mdArgs } | Should -Throw -ErrorId $expectedErrorId
        }

        It "Metadata should contain the <Name> key" -testCases @(
            @{ Name = "external help file" }
            @{ Name = "Module Name" }
            @{ Name = "online version" }
            @{ Name = "aliases" }
            @{ Name = "schema" }
        ) {
            param ($Name)
            $file = New-MarkdownHelp -Command New-MarkdownHelp -OutputFolder $TestDrive -Force -Metadata $defaultMetadata
            $md = (Import-MarkdownCommandHelp $file).Metadata
            $md.Keys | Should -Contain $Name
        }
    }

    Context 'encoding' {
        It 'writes appropriate encoding' {
            $file = New-MarkdownHelp -command New-MarkdownHelp -OutputFolder $TestDrive -Force -Encoding ([System.Text.Encoding]::UTF32) -Metadata $defaultMetadata
            Get-Content -AsByteStream $file | Select-Object -First 4 | Should -Be ([byte[]](255, 254, 0, 0))
        }
    }

    Context 'from PlatyPS module' {
        It 'creates few help files for PlatyPS' {
            $files = New-MarkdownHelp -Module Microsoft.PowerShell.PlatyPS -OutputFolder "$TestDrive/platyPS" -Force
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

            $files = New-MarkdownHelp -Module PlatyPSTestModule -OutputFolder "$TestDrive/PlatyPSTestModule" -Force
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
            $files = New-MarkdownHelp -Command @('New-MarkdownHelp', 'Import-MarkdownCommandHelp') -OutputFolder "$TestDrive/commands" -Force
            $files | Should -HaveCount 2
        }

        It 'creates the proper content from a command' {
            $files = New-MarkdownHelp -Command 'Get-Date' -OutputFolder "$TestDrive/commands" -Force
        }
    }

    Context 'Command content from command' {
        BeforeAll {
            $file = New-MarkdownHelp -Command 'Get-Date' -OutputFolder "$TestDrive/commands" -Force
            $helpInfo = Import-MarkdownCommandHelp $file
            $cmdInfo = Get-Command Get-Date
        }

        It 'Should have the proper number of syntax entries' {
            $syntaxCount = $cmdInfo.Definition.Split([environment]::newline).Where({$_}).Count
            $helpInfo.Syntax.Count | Should -Be $syntaxCount
        }

        It 'Should have the correct module name' {
            $helpInfo.ModuleName | Should -Be $cmdInfo.Source
        }

        It 'Should have identified the correct default parameter set name' {
            $helpInfo.Syntax.Where({$_.IsDefaultParameterSet}).ParameterSetName | Should -Be $cmdInfo.DefaultParameterSet
        }

        It 'Should have identified an alias for the "Date" parameter' {
            $helpInfo.Parameters.Where({$_.name -eq "Date"}).Aliases | Should -Be ($cmdInfo.Parameters['Date'].Aliases)
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
            Set-Content -Value $SeedData -Path "$TestDrive/Invoke-HelloWorld.ps1" -NoNewline
            $files = New-MarkdownHelp -Command "$TestDrive/Invoke-HelloWorld.ps1" -OutputFolder "$TestDrive/output" -Force
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
            Set-Content -Value $SeedData -Path "$TestDrive/Invoke-HelloWorld.ps1" -NoNewline
            $Location = Get-Location
            Set-Location $TestDrive
            $files = New-MarkdownHelp -Command "$TestDrive/Invoke-HelloWorld.ps1" -OutputFolder "$TestDrive/output" -Force
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
            $files = New-MarkdownHelp -Command Get-Alpha -OutputFolder "$TestDrive/alpha" -Force -AlphabeticParamsOrder
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

        $file = New-MarkdownHelp -Command Test-PlatyPSFunction -OutputFolder "$TestDrive/testAll1" -Force
        $content = Import-MarkdownCommandHelp $file

        It 'generates markdown with correct parameter set names' {
            $content.Parameters.ParameterSets.Count | Should -Be 3
            $content.Parameters[0].ParameterSets[0].Name | Should -Be "(All)"
            $content.Parameters[1].ParameterSets[0].Name | Should -Be "First"
            $content.Parameters[2].ParameterSets[0].Name | Should -Be "Second"
        }

        It 'generates markdown with correct synopsis' {
            $content.Synopsis | Should -Be "Adds a file name extension to a supplied name."
        }

        It 'generates markdown with correct help description specified by HelpMessage attribute' {
            $content.Parameters.Where({$_.Name -eq "First"}).Description | Should -Be "First parameter help description"
        }

        It 'generates markdown with correct help description specified by comment-based help' {
            $content.Parameters.Where({$_.Name -eq "Second"}).Description | Should -Be "Second parameter help description"
        }

        It 'generates markdown with placeholder for parameter with no description' {
            $expectedString = "{{ Fill Common Description }}"
            $content.Parameters.Where({$_.Name -eq "Common"}).Description | Should -Be $expectedString
        }

        It 'Description can contain multiple code blocks and text' {
            set-itresult -pending -because "test unimplemented"
        }
    }

    Context 'Generated markdown features: no comment-based help' {
        function global:Test-PlatyPSFunction
        {
            # there is a help-engine behavior difference for functions with comment-based help (or maml help)
            # and no-comment based help, we test both
            param(
                [Switch]$Common,
                [Parameter(ParameterSetName="First", HelpMessage = 'First parameter help description')]
                [string]$First,
                [Parameter(ParameterSetName="Second")]
                [string]$Second
            )
        }

        $file = New-MarkdownHelp -Command Test-PlatyPSFunction -OutputFolder "$TestDrive/testAll2" -Force
        $content = Get-Content $file

        It 'generates markdown with correct synopsis placeholder' {
            $content | Where-Object {$_ -eq '{{ Fill in the Synopsis }}'} |  Should -HaveCount 1
        }

        It 'generates markdown with correct help description specified by HelpMessage attribute' {
            $content | Where-Object {$_ -eq 'First parameter help description'} | Should -HaveCount 1
        }

        It 'generates markdown with placeholder for parameter with no description' {
            $expectedString = '{{{{ Fill {0} Description }}}}' -f 'Common'
            $content | Where-Object {$_ -eq $expectedString } | Should -HaveCount 1
        }
    }

    Context 'Dynamic parameters' {

        function global:Test-DynamicParameterSet {
            [CmdletBinding()]
            [OutputType()]

            Param (
                [Parameter(
                    ParameterSetName = 'Static'
                )]
                [Switch]
                $StaticParameter
            )

            DynamicParam {
                $dynamicParamAttributes = New-Object -TypeName System.Management.Automation.ParameterAttribute -Property @{
                    ParameterSetName = 'Dynamic'
                }
                $dynamicParamCollection = New-Object -TypeName System.Collections.ObjectModel.Collection[System.Attribute]
                $dynamicParamCollection.Add($dynamicParamAttributes)
                $dynamicParam = New-Object -TypeName System.Management.Automation.RuntimeDefinedParameter  -ArgumentList ('DynamicParameter', [Switch], $dynamicParamCollection)
                $dictionary = New-Object -TypeName System.Management.Automation.RuntimeDefinedParameterDictionary
                $dictionary.Add('DynamicParameter', $dynamicParam)
                return $dictionary
            }

            Process {
                Write-Output -InputObject $PSCmdlet.ParameterSetName
            }
        }

        It "generates DynamicParameter" {
            $a = @{
                command = 'Test-DynamicParameterSet'
                OutputFolder = "$TestDrive"
            }

            $file = New-MarkdownHelp @a
            $content = Get-Content $file

            # Validate parameterset names
            $content | Where-Object {$_ -eq "### Static" } | Should -HaveCount 1
            $content | Where-Object {$_ -eq "### Dynamic" } | Should -HaveCount 1

            # Validate parameter details
            $content | Where-Object {$_ -eq "### -DynamicParameter" } | Should -HaveCount 1
            $content | Where-Object {$_ -eq "### -StaticParameter" } | Should -HaveCount 1
        }
    }

    Context 'Module Landing Page'{

        BeforeAll {
            $OutputFolder = "$TestDrive/LandingPageMD"
            $OutputFolderReadme = "$TestDrive/LandingPageMD-ReadMe/Readme.md"
            $null = New-Item -ItemType Directory $OutputFolder
        }

        It "generates a landing page from Module" {
            New-MarkdownHelp -Module Microsoft.PowerShell.PlatyPS -OutputFolder $OutputFolder -WithModulePage -Force
            "$OutputFolder/Microsoft.PowerShell.PlatyPS.md" | Should -Exist
        }

        It "generates a landing page from MAML" -Pending {
            New-MarkdownHelp -MamlFile (Get-ChildItem "$outFolder/Microsoft.PowerShell.PlatyPS/en-US/platy*xml") `
                        -OutputFolder $OutputFolder `
                        -WithModulePage `
                        -ModuleName "Microsoft.PowerShell.PlatyPS" `
                        -Force

            $LandingPage = Get-ChildItem (Join-Path $OutputFolder Microsoft.PowerShell.PlatyPS.md)
            $LandingPage | Should -Exist
        }

        it 'generate a landing page from Module with parameter ModulePagePath' {
            New-MarkdownHelp -Module Microsoft.PowerShell.PlatyPS -OutputFolder $OutputFolder -WithModulePage -ModulePagePath $OutputFolderReadme -Force
            $OutputFolderReadme | Should -Exist
        }

        It 'generates a landing page from module at correct output folder' {
            try {
                Push-Location $TestDrive
                $files = New-MarkdownHelp -Module Microsoft.PowerShell.PlatyPS -OutputFolder . -UseFullTypeName -WithModulePage -ModulePagePath . -Force
                $landingPage = $files | Where-Object { $_.Name -eq 'Microsoft.PowerShell.PlatyPS.md' }
                $landingPage.FullName | Should -BeExactly (Join-Path "$TestDrive" "Microsoft.PowerShell.PlatyPS.md")
            }
            finally {
                Pop-Location
            }
        }
    }

    Context 'Full Type Name' {
        function global:Get-Alpha
        {
            param(
                [Switch]
                [Parameter(Position=1)]
                $WhatIf,
                [string]
                [Parameter(Position=2)]
                $CCC,
                [System.Nullable`1[System.Int32]]
                [Parameter(Position=3)]
                $ddd
            )
        }

        It 'use full type name when specified' -Pending {
            $expectedParameters = normalizeEnds @'
Type: System.String
Type: System.Nullable<System.Int32>
Type: System.Management.Automation.SwitchParameter

'@
            $expectedSyntax = normalizeEnds @'
Get-Alpha [-WhatIf] [[-CCC] <String>] [[-ddd] <Int32>] [<CommonParameters>]

'@

            $files = New-MarkdownHelp -Command Get-Alpha -OutputFolder "$TestDrive/alpha" -Force -AlphabeticParamsOrder -UseFullTypeName
            $files | Should -HaveCount 1
            normalizeEnds(Get-Content $files | Where-Object {$_.StartsWith('Type: ')} | Out-String) | Should -Be $expectedParameters
            normalizeEnds(Get-Content $files | Where-Object {$_.StartsWith('Get-Alpha')} | Out-String) | Should -Be $expectedSyntax
        }

        It 'not use full type name when specified' -Pending {
            $expectedParameters = normalizeEnds @'
Type: String
Type: Int32
Type: SwitchParameter

'@
            $expectedSyntax = normalizeEnds @'
Get-Alpha [-WhatIf] [[-CCC] <String>] [[-ddd] <Int32>] [<CommonParameters>]

'@

            $files = New-MarkdownHelp -Command Get-Alpha -OutputFolder "$TestDrive/alpha" -Force -AlphabeticParamsOrder
            $files | Should -HaveCount 1
            normalizeEnds(Get-Content $files | Where-Object {$_.StartsWith('Type: ')} | Out-String) | Should Be $expectedParameters
            normalizeEnds(Get-Content $files | Where-Object {$_.StartsWith('Get-Alpha')} | Out-String) | Should Be $expectedSyntax
        }
    }

    Context 'Markdown Content' {
        BeforeAll {
            $file = New-MarkdownHelp -Command 'New-MarkdownHelp' -OutputFolder "$TestDrive" -Force
            $lines = Get-Content $file
        }
        It "Should contain the header '<line>'" -TestCases @(
            @{ line = "## SYNOPSIS" }
            @{ line = "## SYNTAX" }
            @{ line = "## ALIASES" }
            @{ line = "## DESCRIPTION" }
            @{ line = "## EXAMPLES" }
            @{ line = "## PARAMETERS" }
            @{ line = "## INPUTS" }
            @{ line = "## OUTPUTS" }
            @{ line = "## NOTES" }
            @{ line = "## RELATED LINKS" }
        ) {
            param ($line)
            $observedLine = ""
            for($i = 0; $i -lt $lines.count; $i++) {
                if ($lines[$i] -eq $line) {
                    $observedLine = $lines[$i]
                    break
                }
            }
            $observedLine | Should -BeExactly $line
        }

        It "The order of the header lines should be correct" {
            $correctOrder = "## SYNOPSIS",
                "## SYNTAX",
                "## ALIASES",
                "## DESCRIPTION",
                "## EXAMPLES",
                "## PARAMETERS",
                "## INPUTS",
                "## OUTPUTS",
                "## NOTES",
                "## RELATED LINKS"
            $observedOrder = $lines.Where({$_ -match "^## "})
            $observedOrder | Should -Be $correctOrder
        }

        It "The alias section should contain the proper boiler-plate" {
            $expectedMessage = "This cmdlet has the following aliases,"
            $observedLine = ""
            for($i = 0; $i -lt $lines.count; $i++) {
                if ($lines[$i] -eq "## ALIASES") {
                    $observedLine = $lines[$i+2]
                    break
                }
            }
            $observedLine | Should -BeExactly $expectedMessage
        }
    }

    Context 'SupportsWildCards attribute tests' {
        BeforeAll {
            function global:Test-WildCardsAttribute {
                param (
                    [Parameter()]
                    [SupportsWildcards()]
                    [string] $Name,

                    [Parameter()]
                    [string] $NameNoWildCard
                )
            }

            $file = New-MarkdownHelp -Command 'Test-WildCardsAttribute' -OutputFolder "$TestDrive/NewMarkDownHelp"
        }

        It 'sets accepts wildcards property on parameters as expected' {
            $file | Should -FileContentMatch 'Globbing: true'
        }
    }
}
