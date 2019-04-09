Set-StrictMode -Version latest
$ErrorActionPreference = 'Stop'

$root = (Resolve-Path $PSScriptRoot\..\..).Path
$outFolder = "$root\out"
$moduleFolder = "$outFolder\platyPS"

Import-Module $moduleFolder -Force
$MyIsLinux = Get-Variable -Name IsLinux -ValueOnly -ErrorAction SilentlyContinue
$MyIsMacOS = Get-Variable -Name IsMacOS -ValueOnly -ErrorAction SilentlyContinue
$global:IsUnix = $MyIsLinux -or $MyIsMacOS

Import-LocalizedData -BindingVariable LocalizedData -BaseDirectory $moduleFolder -FileName platyPS.Resources.psd1

Describe 'New-MarkdownHelp' {
    function normalizeEnds([string]$text)
    {
        $text -replace "`r`n?|`n", "`r`n"
    }

    Context 'errors' {
        It 'throw when cannot find module' {
            { New-MarkdownHelp -Module __NON_EXISTING_MODULE -OutputFolder TestDrive:\ } |
                Should Throw ($LocalizedData.ModuleNotFound -f '__NON_EXISTING_MODULE')
        }

        It 'throw when cannot find module' {
            { New-MarkdownHelp -command __NON_EXISTING_COMMAND -OutputFolder TestDrive:\ } |
                Should Throw ($LocalizedData.CommandNotFound -f '__NON_EXISTING_COMMAND')
        }
    }

    Context 'metadata' {
        It 'generates passed metadata' {
            $file = New-MarkdownHelp -metadata @{
                FOO = 'BAR'
            } -command New-MarkdownHelp -OutputFolder TestDrive:\

            $h = Get-MarkdownMetadata $file
            $h['FOO'] | Should Be 'BAR'
        }

        It 'respects -NoMetadata' {
            $file = New-MarkdownHelp -command New-MarkdownHelp -OutputFolder TestDrive:\ -NoMetadata -Force
            Get-MarkdownMetadata $file.FullName | Should Be $null
        }

        It 'errors on -NoMetadata and -Metadata' {
            { New-MarkdownHelp -command New-MarkdownHelp -OutputFolder TestDrive:\ -NoMetadata -Force -Metadata @{} } |
                Should Throw $LocalizedData.NoMetadataAndMetadata
        }
    }

    Context 'encoding' {
        $file = New-MarkdownHelp -command New-MarkdownHelp -OutputFolder TestDrive:\ -Force -Encoding ([System.Text.Encoding]::UTF32)
        $content = $file | Get-Content -Encoding UTF32 -Raw
        Get-MarkdownMetadata -Markdown $content | Should Not Be $null
    }

    Context 'from platyPS module' {
        It 'creates few help files for platyPS' {
            $files = New-MarkdownHelp -Module PlatyPS -OutputFolder TestDrive:\platyPS -Force
            ($files | Measure-Object).Count | Should BeGreaterThan 4
        }
    }

    Context 'from module' {
        try
        {
            New-Module -Name PlatyPSTestModule -ScriptBlock {
                function Get-AAAA
                {

                }

                function Get-AdvancedFn
                {
                    [CmdletBinding()]
                    param (

                    )
                }

                function Get-SimpleFn
                {
                    param (

                    )
                }

                function Set-BBBB
                {

                }


                if (-not $global:IsUnix) {
                    # just declaring workflow is a parse-time error on unix
                    Invoke-Expression "Workflow FromCommandWorkflow {}"
                    Export-ModuleMember -Function 'FromCommandWorkflow'
                }

                # Set-Alias and New-Alias provide two different results
                # when `Get-Command -module Foo` is used to list commands.
                Set-Alias aaaaalias Get-AAAA
                Set-Alias bbbbalias Get-BBBB

                New-Alias -Name 'Fork-AAAA' -Value 'Get-AAAA'

                Export-ModuleMember -Alias Fork-AAAA
                Export-ModuleMember -Alias aaaaalias
                Export-ModuleMember -Alias bbbbalias
                Export-ModuleMember -Function 'Get-AAAA','Get-AdvancedFn','Get-SimpleFn'

            } | Import-Module -Force

            $files = New-MarkdownHelp -Module PlatyPSTestModule -OutputFolder TestDrive:\PlatyPSTestModule -Force
        }
        finally
        {
            Remove-Module PlatyPSTestModule -ErrorAction SilentlyContinue
        }

        It 'generates markdown files only for exported functions' -Skip:$IsUnix {
            ($files | Measure-Object).Count | Should Be 4
            $files.Name | Should -BeIn 'Get-AAAA.md','Get-AdvancedFn.md','Get-SimpleFn.md','FromCommandWorkflow.md'
        }

        It 'generates markdown that includes CommonParameters in advanced functions' {
            ($files | Where-Object -FilterScript { $_.Name -eq 'Get-AdvancedFn.md' }).FullName | Should -FileContentMatch '### CommonParameters'
        }

        It 'generates markdown that excludes CommonParameters from simple functions' {
            ($files | Where-Object -FilterScript { $_.Name -eq 'Get-SimpleFn.md' }).FullName | Should -FileContentMatch -Not '### CommonParameters'
        }

        It 'generates markdown for workflows with CommonParameters' -Skip:$IsUnix {
            ($files | Where-Object -FilterScript { $_.Name -eq 'FromCommandWorkflow.md' }).FullName | Should -FileContentMatch '### CommonParameters'
        }
    }

    Context 'from command' {
        It 'creates 2 markdown files from command names' {
            $files = New-MarkdownHelp -Command @('New-MarkdownHelp', 'Get-MarkdownMetadata') -OutputFolder TestDrive:\commands -Force
            ($files | Measure-Object).Count | Should Be 2
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
            Set-Content -Value $SeedData -Path TestDrive:\Invoke-HelloWorld.ps1 -NoNewline
            $files = New-MarkdownHelp -Command "TestDrive:\Invoke-HelloWorld.ps1" -OutputFolder TestDrive:\output -Force
            ($files | Measure-Object).Count | Should Be 1
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
            Set-Content -Value $SeedData -Path TestDrive:\Invoke-HelloWorld.ps1 -NoNewline
            $Location = Get-Location
            Set-Location TestDrive:\
            $files = New-MarkdownHelp -Command "TestDrive:\Invoke-HelloWorld.ps1" -OutputFolder TestDrive:\output -Force
            Set-Location $Location
            ($files | Measure-Object).Count | Should Be 1
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
### -WhatIf
### -IncludeTotalCount
### -Skip
### -First

'@
            $files = New-MarkdownHelp -Command Get-Alpha -OutputFolder TestDrive:\alpha -Force -AlphabeticParamsOrder
            ($files | Measure-Object).Count | Should Be 1
            normalizeEnds (Get-Content $files | Where-Object {$_.StartsWith('### -')} | Out-String) | Should Be $expectedOrder
        }
    }

    Context 'Online version link' {

        function global:Test-PlatyPSFunction {
            <#
            .LINK
            http://www.fabrikam.com/extension.html
            #>
        }

        @('https://github.com/PowerShell/platyPS', 'http://www.fabrikam.com/extension.html', $null) | ForEach-Object {
            $uri = $_
            It "generates passed online url '$uri'" {
                $a = @{
                    command = 'Test-PlatyPSFunction'
                    OutputFolder = 'TestDrive:\'
                    Force = $true
                    OnlineVersionUrl = $uri
                }

                $file = New-MarkdownHelp @a
                $maml = $file | New-ExternalHelp -OutputPath "TestDrive:\" -Force
                $help = Get-HelpPreview -Path $maml
                $link = $help.relatedLinks.navigationLink
                if ($uri) {
                    if ($uri -eq 'http://www.fabrikam.com/extension.html') {
                        ($link | Measure-Object).Count | Should Be 1
                        $link[0].linkText | Should Be $uri
                        $link[0].uri | Should Be 'http://www.fabrikam.com/extension.html'
                    }
                    else {
                        ($link | Measure-Object).Count | Should Be 2
                        $link[0].linkText | Should Be 'Online Version:'
                        $link[0].uri | Should Be $uri
                    }
                }
            }
        }
    }

    Context 'Generates well-known stub descriptions for parameters' {
        function global:Test-PlatyPSFunction {
            param(
                [string]$Foo,
                [switch]$Confirm,
                [switch]$WhatIf,
                [switch]$IncludeTotalCount,
                [uint64]$Skip,
                [uint64]$First
            )
        }

        $file = New-MarkdownHelp -Command Test-PlatyPSFunction -OutputFolder TestDrive:\ -Force -AlphabeticParamsOrder
        $maml = $file | New-ExternalHelp -OutputPath "TestDrive:\" -Force
        $help = Get-HelpPreview -Path $maml

        It 'generates well-known stub descriptions for -WhatIf' {
            $param = $help.parameters.parameter | Where-Object { $_.Name -eq 'WhatIf' }
            $param.description.text | Should Be $LocalizedData.WhatIf
        }

        It 'generates well-known stub descriptions for -Confirm' {
            $param = $help.parameters.parameter | Where-Object { $_.Name -eq 'Confirm' }
            $param.description.text | Should Be $LocalizedData.Confirm
        }

        It 'generates well-known stub descriptions for -IncludeTotalCount' {
            $param = $help.parameters.parameter | Where-Object { $_.Name -eq 'IncludeTotalCount' }
            $param.description.text | Should Be $LocalizedData.IncludeTotalCount
        }

        It 'generates well-known stub descriptions for -Skip' {
            $param = $help.parameters.parameter | Where-Object { $_.Name -eq 'Skip' }
            $param.description.text | Should Be $LocalizedData.Skip
        }

        It 'generates well-known stub descriptions for -First' {
            $param = $help.parameters.parameter | Where-Object { $_.Name -eq 'First' }
            $param.description.text | Should Be $LocalizedData.First
        }

        It 'generates well-known stub descriptions for -Foo' {
            $param = $help.parameters.parameter | Where-Object { $_.Name -eq 'Foo' }
            $param.description.text | Should Be ($LocalizedData.ParameterDescription -f 'Foo')
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

        $file = New-MarkdownHelp -Command Test-PlatyPSFunction -OutputFolder TestDrive:\testAll1 -Force
        $content = Get-Content $file

        It 'generates markdown with correct parameter set names' {
            ($content | Where-Object {$_ -eq 'Parameter Sets: (All)'} | Measure-Object).Count | Should Be 1
            ($content | Where-Object {$_ -eq 'Parameter Sets: First'} | Measure-Object).Count | Should Be 1
            ($content | Where-Object {$_ -eq 'Parameter Sets: Second'} | Measure-Object).Count | Should Be 1
        }

        It 'generates markdown with correct synopsis' {
            ($content | Where-Object {$_ -eq 'Adds a file name extension to a supplied name.'} | Measure-Object).Count | Should Be 2
        }

        It 'generates markdown with correct help description specified by HelpMessage attribute' {
            ($content | Where-Object {$_ -eq 'First parameter help description'} | Measure-Object).Count | Should Be 1
        }

        It 'generates markdown with correct help description specified by comment-based help' {
            ($content | Where-Object {$_ -eq 'Second parameter help description'} | Measure-Object).Count | Should Be 1
        }

        It 'generates markdown with placeholder for parameter with no description' {
            ($content | Where-Object {$_ -eq ($LocalizedData.ParameterDescription -f 'Common')} | Measure-Object).Count | Should Be 1
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

        $file = New-MarkdownHelp -Command Test-PlatyPSFunction -OutputFolder TestDrive:\testAll2 -Force
        $content = Get-Content $file

        It 'generates markdown with correct synopsis placeholder' {
            ($content | Where-Object {$_ -eq $LocalizedData.Synopsis} | Measure-Object).Count | Should Be 1
        }

        It 'generates markdown with correct help description specified by HelpMessage attribute' {
            ($content | Where-Object {$_ -eq 'First parameter help description'} | Measure-Object).Count | Should Be 1
        }

        It 'generates markdown with placeholder for parameter with no description' {
            ($content | Where-Object {$_ -eq ($LocalizedData.ParameterDescription -f 'Common')} | Measure-Object).Count | Should Be 1
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
                OutputFolder = 'TestDrive:\'
            }

            $file = New-MarkdownHelp @a
            $maml = $file | New-ExternalHelp -OutputPath "TestDrive:\"
            $help = Get-HelpPreview -Path $maml
            $help.Syntax.syntaxItem.Count | Should Be 2
            $dynamicParam = $help.parameters.parameter | Where-Object {$_.name -eq 'DynamicParameter'}
            ($dynamicParam | Measure-Object).Count | Should Be 1
        }
    }

    Context 'Module Landing Page'{

        $OutputFolder = "TestDrive:\LandingPageMD\"
        $OutputFolderReadme = 'TestDrive:\LandingPageMD-ReadMe\Readme.md'

        New-Item -ItemType Directory $OutputFolder


        It "generates a landing page from Module"{

            New-MarkdownHelp -Module PlatyPS -OutputFolder $OutputFolder -WithModulePage -Force

            $LandingPage = Get-ChildItem (Join-Path $OutputFolder PlatyPS.md)

            $LandingPage | Should Not Be $null

        }

        It "generates a landing page from MAML"{

            New-MarkdownHelp -MamlFile (Get-ChildItem "$outFolder\platyPS\en-US\platy*xml") `
                        -OutputFolder $OutputFolder `
                        -WithModulePage `
                        -ModuleName "PlatyPS" `
                        -Force

            $LandingPage = Get-ChildItem (Join-Path $OutputFolder PlatyPS.md)

            $LandingPage | Should Not Be $null

        }

        it 'generate a landing page from Module with parameter ModulePagePath' {
            New-MarkdownHelp -Module PlatyPS -OutputFolder $OutputFolder -WithModulePage -ModulePagePath $OutputFolderReadme -Force

            $LandingPage = Test-Path -Path $OutputFolderReadme
            $LandingPage | Should Not Be $null
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

        It 'use full type name when specified' {
            $expectedParameters = normalizeEnds @'
Type: System.String
Type: System.Nullable`1[System.Int32]
Type: System.Management.Automation.SwitchParameter

'@
            $expectedSyntax = normalizeEnds @'
Get-Alpha [-WhatIf] [[-CCC] <String>] [[-ddd] <Int32>] [<CommonParameters>]

'@

            $files = New-MarkdownHelp -Command Get-Alpha -OutputFolder TestDrive:\alpha -Force -AlphabeticParamsOrder -UseFullTypeName
            ($files | Measure-Object).Count | Should Be 1
            normalizeEnds(Get-Content $files | Where-Object {$_.StartsWith('Type: ')} | Out-String) | Should Be $expectedParameters
            normalizeEnds(Get-Content $files | Where-Object {$_.StartsWith('Get-Alpha')} | Out-String) | Should Be $expectedSyntax
        }

        It 'not use full type name when specified' {
            $expectedParameters = normalizeEnds @'
Type: String
Type: Int32
Type: SwitchParameter

'@
            $expectedSyntax = normalizeEnds @'
Get-Alpha [-WhatIf] [[-CCC] <String>] [[-ddd] <Int32>] [<CommonParameters>]

'@

            $files = New-MarkdownHelp -Command Get-Alpha -OutputFolder TestDrive:\alpha -Force -AlphabeticParamsOrder
            ($files | Measure-Object).Count | Should Be 1
            normalizeEnds(Get-Content $files | Where-Object {$_.StartsWith('Type: ')} | Out-String) | Should Be $expectedParameters
            normalizeEnds(Get-Content $files | Where-Object {$_.StartsWith('Get-Alpha')} | Out-String) | Should Be $expectedSyntax
        }
    }

    Context 'DontShow parameter' {
        BeforeAll {
            function global:Test-DontShowParameter {
                [CmdletBinding()]
                [OutputType()]

                Param (
                    [Parameter()]
                    [Switch]
                    $ShowAll,

                    [Parameter(DontShow)]
                    [Switch]
                    $DontShowAll,

                    [Parameter(ParameterSetName = 'Set1', DontShow)]
                    [Parameter(ParameterSetName = 'Set2')]
                    [Switch]
                    $DontShowSet1,

                    [Parameter(ParameterSetName = 'Set1', DontShow)]
                    [Parameter(ParameterSetName = 'Set2', DontShow)]
                    [Switch]
                    $DontShowSetAll
                )

                Process {
                    Write-Output -InputObject $PSCmdlet.ParameterSetName
                }
            }

            $a = @{
                command = 'Test-DontShowParameter'
                OutputFolder = 'TestDrive:\'
            }

            $fileWithoutDontShowSwitch = New-MarkdownHelp @a -Force
            $file = New-MarkdownHelp @a -ExcludeDontShow -Force

            $maml = $file | New-ExternalHelp -OutputPath "TestDrive:\"
            $help = Get-HelpPreview -Path $maml
            $mamlModelObject = & (Get-Module platyPS) { GetMamlObject -Cmdlet "Test-DontShowParameter" }

            $updatedFile = Update-MarkdownHelp -Path $fileWithoutDontShowSwitch -ExcludeDontShow
            $null = New-Item -ItemType Directory "$TestDrive\UpdateMarkdown"
            $updatedMaml = $file | New-ExternalHelp -OutputPath "TestDrive:\UpdateMarkdown"
            $updatedHelp = Get-HelpPreview -Path $updatedMaml
            $updateMamlModelObject = & (Get-Module platyPS) { GetMamlObject -Cmdlet "Test-DontShowParameter" }
        }

        Context "New-MarkdownHelp with -ExcludeDontShow" {
            It "includes ShowAll" {
                $showAll = $help.parameters.parameter | Where-Object {$_.name -eq 'ShowAll'}
                ($showAll | Measure-Object).Count | Should Be 1
            }

            It "excludes DontShowAll" {
                $dontShowAll = $help.parameters.parameter | Where-Object {$_.name -eq 'DontShowAll'}
                ($dontShowAll | Measure-Object).Count | Should Be 0
            }

            It 'includes DontShowSet1 excludes Set1' -Skip {
                $dontShowSet1 = $help.parameters.parameter | Where-Object {$_.name -eq 'DontShowSet1'}
                ($dontShowSet1 | Measure-Object).Count | Should Be 1

                $set1 = $mamlModelObject.Syntax | Where-Object {$_.ParameterSetName -eq 'Set1'}
                ($set1 | Measure-Object).Count | Should Be 0
            }

            It 'excludes DontShowSetAll includes Set2' {
                $dontShowAll = $help.parameters.parameter | Where-Object {$_.name -eq 'DontShowSetAll'}
                ($dontShowAll | Measure-Object).Count | Should Be 0

                $set2 = $mamlModelObject.Syntax | Where-Object {$_.ParameterSetName -eq 'Set2'}
                ($set2 | Measure-Object).Count | Should Be 1
            }
        }

        Context "Update-MarkdownHelp with -ExcludeDontShow" {
            It "includes ShowAll" {
                $showAll = $updatedHelp.parameters.parameter | Where-Object {$_.name -eq 'ShowAll'}
                ($showAll | Measure-Object).Count | Should Be 1
            }

            It "excludes DontShowAll" {
                $dontShowAll = $updatedHelp.parameters.parameter | Where-Object {$_.name -eq 'DontShowAll'}
                ($dontShowAll | Measure-Object).Count | Should Be 0
            }

            It 'includes DontShowSet1 excludes Set1' -Skip {
                $dontShowSet1 = $updatedHelp.parameters.parameter | Where-Object {$_.name -eq 'DontShowSet1'}
                ($dontShowSet1 | Measure-Object).Count | Should Be 1

                $set1 = $mamlModelObject.Syntax | Where-Object {$_.ParameterSetName -eq 'Set1'}
                ($set1 | Measure-Object).Count | Should Be 0
            }

            It 'excludes DontShowSetAll includes Set2' {
                $dontShowAll = $updatedHelp.parameters.parameter | Where-Object {$_.name -eq 'DontShowSetAll'}
                ($dontShowAll | Measure-Object).Count | Should Be 0

                $set2 = $mamlModelObject.Syntax | Where-Object {$_.ParameterSetName -eq 'Set2'}
                ($set2 | Measure-Object).Count | Should Be 1
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

                $file = New-MarkdownHelp -Command 'Test-WildCardsAttribute' -OutputFolder "$TestDrive\NewMarkDownHelp"
                $maml = $file | New-ExternalHelp -OutputPath "$TestDrive\NewMarkDownHelp"
                $help = Get-HelpPreview -Path $maml
            }

            It 'sets accepts wildcards property on parameters as expected' {
                $help.parameters.parameter | Where-Object { $_.Name -eq 'Name' } | ForEach-Object { $_.globbing -eq 'true'}
                $help.parameters.parameter | Where-Object { $_.Name -eq 'NameNoWildCard' } | ForEach-Object { $_.globbing -eq 'false'}
            }
        }
    }
}


###############################################################
###############################################################


Describe 'New-ExternalHelp' {
    BeforeAll {
        function global:Test-OrderFunction {
          param ([Parameter(Position=3)]$Third, [Parameter(Position=1)]$First, [Parameter()]$Named)
          $First
          $Third
          $Named
        }
        $file = New-MarkdownHelp -Command 'Test-OrderFunction' -OutputFolder $TestDrive -Force
        $maml = $file | New-ExternalHelp -OutputPath "$TestDrive\TestOrderFunction.xml" -Force
    }

    It "generates right order for syntax" {
        $help = Get-HelpPreview -Path $maml
        ($help.Syntax.syntaxItem | Measure-Object).Count | Should Be 1
        $names = $help.Syntax.syntaxItem.parameter.Name
        ($names | Measure-Object).Count | Should Be 3
        $names[0] | Should Be 'First'
        $names[1] | Should Be 'Third'
        $names[2] | Should Be 'Named'
    }

    It "checks that xmlns 'http://msh' is present" {
        $xml = [xml] (Get-Content (Join-Path $TestDrive 'TestOrderFunction.xml'))
        $xml.helpItems.namespaceuri | Should Be 'http://msh'
    }
}

Describe 'New-ExternalHelp -ErrorLogFile' {
   BeforeAll {
      function global:Test-OrderFunction {
         param ([Parameter(Position = 3)]$Third, [Parameter(Position = 1)]$First, [Parameter()]$Named)
         $First
         $Third
         $Named
      }

      try {
         # Don't add metadata so the file has an error
         $file = New-MarkdownHelp -Command 'Test-OrderFunction' -OutputFolder $TestDrive -Force -NoMetadata
         $maml = $file | New-ExternalHelp -OutputPath "$TestDrive\TestOrderFunction.xml" -ErrorLogFile "$TestDrive\warningsAndErrors.json" -Force
      }
      catch {
         # Ignore the error. I just needed an error to write to the log file.
         # If I don't catch this the test will fail.
      }
   }

   It "generates error log file" {
      Test-Path  "$TestDrive\warningsAndErrors.json" | Should Be $true
   }
}

Describe 'New-ExternalHelp -ApplicableTag for cmdlet level' {
    BeforeAll {
        function global:Test-Applicable12 {}
        function global:Test-ApplicableNone {}
        function global:Test-Applicable32 {}
        function global:Test-Applicable4 {}
    }

    It 'ignores cmdlet when applicable tag doesn''t match' {
        $files = @()
        $files += New-MarkdownHelp -Metadata @{ applicable = 'tag 1, tag 2'} -Command Test-Applicable12 -OutputFolder $TestDrive -Force -WarningAction SilentlyContinue
        $files += New-MarkdownHelp -Metadata @{ applicable = 'tag 3, tag 2'} -Command Test-Applicable32 -OutputFolder $TestDrive -Force -WarningAction SilentlyContinue
        $files += New-MarkdownHelp -Command Test-ApplicableNone -OutputFolder $TestDrive -Force -WarningAction SilentlyContinue
        $files += New-MarkdownHelp -Metadata @{ applicable = 'tag 4'} -Command Test-Applicable4 -OutputFolder $TestDrive -Force -WarningAction SilentlyContinue

        $maml = $files | New-ExternalHelp -ApplicableTag @('tag 1', 'tag 4') -OutputPath "$TestDrive\TestApplicable.xml" -Force
        $help = Get-HelpPreview -Path $maml
        ($help | Measure-Object).Count | Should Be 3
        $names = $help.Name
        $names[0] | Should Be 'Test-Applicable12'
        $names[1] | Should Be 'Test-ApplicableNone'
        $names[2] | Should Be 'Test-Applicable4'
    }
}

Describe 'New-ExternalHelp -ApplicableTag for parameters level' {
    It 'ignores parameters when applicable tag doesn''t match' {
        $md = @'
---
schema: 2.0.0
---

# Get-Foo
## PARAMETERS

### -P1

```yaml
Type: String
Applicable: tag 1, tag 2
```

### -P2

```yaml
Type: String
Applicable: tag 3
```

### -P3

```yaml
Type: String
```

### -P4

```yaml
Type: String
Applicable: tag 4
```
'@
        $path = "$outFolder\Get-Foo.md"
        Set-Content -Path $path -Value $md
        $maml = $path | New-ExternalHelp -ApplicableTag @('tag 1', 'tag 4') -OutputPath "$TestDrive\TestApplicable.xml" -Force
        $help = Get-HelpPreview -Path $maml
        $names = $help.Parameters.parameter.Name
        ($names | Measure-Object).Count | Should Be 3
        $names[0] | Should Be 'P1'
        $names[1] | Should Be 'P3'
        $names[2] | Should Be 'P4'
    }
}

if (-not $global:IsUnix) {
#region PS Objects to MAML Model Tests
    Describe 'Get-Help & Get-Command on Add-Computer to build MAML Model Object' {

        Context 'Add-Computer' {

            It 'Checks that Help Exists on Computer Running Tests' {

                $Command = Get-Command Add-Computer
                $HelpFileName = Split-Path $Command.HelpFile -Leaf
                $foundHelp = @()
                $paths = $env:PsModulePath.Split(';')
                foreach($path in $paths)
                {
                    $path = Split-Path $path -Parent
                    $foundHelp += Get-ChildItem -ErrorAction SilentlyContinue -Path $path -Recurse |
                        Where-Object { $_.Name -like "*$HelpFileName"} | Select-Object Name
                }

                $foundHelp.Count | Should BeGreaterThan 0
            }

            # call non-exported function in the module scope
            $mamlModelObject = & (Get-Module platyPS) { GetMamlObject -Cmdlet "Add-Computer" }

            It 'Validates attributes by checking several sections of the single attributes for Add-Computer' {

                $mamlModelObject.Name | Should be "Add-Computer"
                $mamlModelObject.Synopsis.Text | Should be "Add the local computer to a domain or workgroup."
                $mamlModelObject.Description.Text.Substring(0,135) | Should be "The Add-Computer cmdlet adds the local computer or remote computers to a domain or workgroup, or moves them from one domain to another."
                $mamlModelObject.Notes.Text.Substring(0,31) | Should be "In Windows PowerShell 2.0, the "
            }

            It 'Validates the examples by checking Add-Computer Example 1' {

                $mamlModelObject.Examples[0].Title | Should be "Example 1: Add a local computer to a domain then restart the computer"
                $mamlModelObject.Examples[0].Code[0].Text | Should be "PS C:\>Add-Computer -DomainName `"Domain01`" -Restart"
                $mamlModelObject.Examples[0].Remarks.Substring(0,120) | Should be "This command adds the local computer to the Domain01 domain and then restarts the computer to make the change effective."

            }

            It 'Validates Parameters by checking Add-Computer Computer Name and Local Credential in Domain ParameterSet'{

                $Parameter = $mamlModelObject.Syntax[0].Parameters | Where-Object { $_.Name -eq "ComputerName" }
                $Parameter.Name | Should be "ComputerName"
                $Parameter.Type | Should be "string[]"
                $Parameter.Required | Should be $false
            }

            It 'Validates there is only 1 default parameterset and that it is the domain parameterset for Add-Computer'{

                $DefaultParameterSet = $mamlModelObject.Syntax | Where-Object {$_.IsDefault}
                $count = 0
                foreach($set in $DefaultParameterSet)
                {
                    $count = $count +1
                }
                $count | Should be 1

                $DefaultParameterSetName = $mamlModelObject.Syntax | Where-Object {$_.IsDefault} | Select-Object ParameterSetName
                $DefaultParameterSetName.ParameterSetName | Should be "Domain"
            }
        }

        Context 'Add-Member' {
            # call non-exported function in the module scope
            $mamlModelObject = & (Get-Module platyPS) { GetMamlObject -Cmdlet "Add-Member" }

            It 'Fetch MemberSet set name' {
                $MemberSet = $mamlModelObject.Syntax | Where-Object {$_.ParameterSetName -eq 'MemberSet'}
                ($MemberSet | Measure-Object).Count | Should Be 1
            }

            It 'populates ParameterValueGroup for MemberType' {
                $Parameters = $mamlModelObject.Syntax.Parameters | Where-Object { $_.Name -eq "MemberType" }
                ($Parameters | Measure-Object).Count | Should Be 1
                $Parameters | ForEach-Object {
                    $_.Name | Should be "MemberType"
                    $_.ParameterValueGroup.Count | Should be 16
                }
            }
        }
    }

#endregion
#########################################################
#region Checking Cab and File Naming Cmdlets

    Describe 'New-ExternalHelpCab' {
        $OutputPath = "$TestDrive\CabTesting"

        New-Item -ItemType Directory -Path (Join-Path $OutputPath "\Source\Xml\") -ErrorAction SilentlyContinue | Out-Null
        New-Item -ItemType Directory -Path (Join-Path $OutputPath "\Source\ModuleMd\") -ErrorAction SilentlyContinue | Out-Null
        New-Item -ItemType Directory -Path (Join-Path $OutputPath "\OutXml") -ErrorAction SilentlyContinue | Out-Null
        New-Item -ItemType Directory -Path (Join-Path $OutputPath "\OutXml2") -ErrorAction SilentlyContinue | Out-Null
        New-Item -ItemType File -Path (Join-Path $OutputPath "\Source\Xml\") -Name "HelpXml.xml" -force | Out-Null
        New-Item -ItemType File -Path (Join-Path $OutputPath "\Source\Xml\") -Name "Module.resources.psd1" | Out-Null
        New-Item -ItemType File -Path (Join-Path $OutputPath "\Source\ModuleMd\") -Name "Module.md" -ErrorAction SilentlyContinue | Out-Null
        New-Item -ItemType File -Path $OutputPath -Name "PlatyPs_00000000-0000-0000-0000-000000000000_helpinfo.xml" -ErrorAction SilentlyContinue | Out-Null
        Set-Content -Path (Join-Path $OutputPath "\Source\Xml\HelpXml.xml") -Value "<node><test>Adding test content to ensure cab builds correctly.</test></node>" | Out-Null
        Set-Content -Path (Join-Path $OutputPath "\Source\ModuleMd\Module.md") -Value "---`r`nModule Name: PlatyPs`r`nModule Guid: 00000000-0000-0000-0000-000000000000`r`nDownload Help Link: Somesite.com`r`nHelp Version: 5.0.0.1`r`nLocale: en-US`r`n---" | Out-Null

        Context 'MakeCab.exe' {

            It 'Validates that MakeCab.exe & Expand.exe exists'{

                (Get-Command MakeCab) -ne $null | Should Be $True
                (Get-Command Expand) -ne $null | Should Be $True

            }
        }

        Context 'New-ExternalHelpCab function, External Help & HelpInfo' {

            $CmdletContentFolder = (Join-Path $OutputPath "\Source\Xml\")
            $ModuleMdPageFullPath = (Join-Path $OutputPath "\Source\ModuleMd\Module.md")

            It 'validates the output of Cab creation' {

                New-ExternalHelpCab -CabFilesFolder $CmdletContentFolder -OutputFolder $OutputPath -LandingPagePath $ModuleMdPageFullPath -WarningAction SilentlyContinue
                $cab = (Get-ChildItem (Join-Path $OutputPath "PlatyPs_00000000-0000-0000-0000-000000000000_en-US_HelpContent.cab")).FullName
                $cabExtract = (Join-Path (Split-Path $cab -Parent) "OutXml")

                $cabExtract = Join-Path $cabExtract "HelpXml.xml"

                expand $cab /f:* $cabExtract

                (Get-ChildItem -Filter "*.cab" -Path "$OutputPath").Name | Should BeExactly "PlatyPs_00000000-0000-0000-0000-000000000000_en-US_HelpContent.cab"
                (Get-ChildItem -Filter "*.xml" -Path "$OutputPath").Name | Should Be "PlatyPs_00000000-0000-0000-0000-000000000000_helpinfo.xml"
                (Get-ChildItem -Filter "*.xml" -Path "$OutputPath\OutXml").Name | Should Be "HelpXml.xml"
                (Get-ChildItem -Filter "*.zip" -Path "$OutputPath").Name | Should BeExactly "PlatyPs_00000000-0000-0000-0000-000000000000_en-US_HelpContent.zip"
                Get-ChildItem -Filter "*.psd1" -Path "$OutputPath\OutXml" | Should BeNullOrEmpty
            }

            It 'Creates a help info file'{
                [xml] $PlatyPSHelpInfo = Get-Content  (Join-Path $OutputPath "PlatyPs_00000000-0000-0000-0000-000000000000_helpinfo.xml")

                $PlatyPSHelpInfo | Should Not Be $null
                $PlatyPSHelpInfo.HelpInfo.SupportedUICultures.UICulture.UICultureName | Should Be "en-US"
                $PlatyPSHelpInfo.HelpInfo.SupportedUICultures.UICulture.UICultureVersion | Should Be "5.0.0.1"
            }

            It 'validates the version is incremented when the switch is used' {
                New-ExternalHelpCab -CabFilesFolder $CmdletContentFolder -OutputFolder $OutputPath -LandingPagePath $ModuleMdPageFullPath -IncrementHelpVersion -WarningAction SilentlyContinue
                [xml] $PlatyPSHelpInfo = Get-Content  (Join-Path $OutputPath "PlatyPs_00000000-0000-0000-0000-000000000000_helpinfo.xml")
                $PlatyPSHelpInfo | Should Not Be $null
                $PlatyPSHelpInfo.HelpInfo.SupportedUICultures.UICulture.UICultureName | Should Be "en-US"
                $PlatyPSHelpInfo.HelpInfo.SupportedUICultures.UICulture.UICultureVersion | Should Be "5.0.0.2"
            }

            It 'Adds another help locale'{

                Set-Content -Path (Join-Path $OutputPath "\Source\ModuleMd\Module.md") -Value "---`r`nModule Name: PlatyPs`r`nModule Guid: 00000000-0000-0000-0000-000000000000`r`nDownload Help Link: Somesite.com`r`nHelp Version: 5.0.0.1`r`nLocale: en-US`r`nAdditional Locale: fr-FR,ja-JP`r`nfr-FR Version: 1.2.3.4`r`nja-JP Version: 2.3.4.5`r`n---" | Out-Null
                New-ExternalHelpCab -CabFilesFolder $CmdletContentFolder -OutputFolder $OutputPath -LandingPagePath $ModuleMdPageFullPath -WarningAction SilentlyContinue
                [xml] $PlatyPSHelpInfo = Get-Content  (Join-Path $OutputPath "PlatyPs_00000000-0000-0000-0000-000000000000_helpinfo.xml")
                $Count = 0
                $PlatyPSHelpInfo.HelpInfo.SupportedUICultures.UICulture | ForEach-Object {$Count++}

                $Count | Should Be 3
            }
        }
    }

#endregion
}

Describe 'Update-MarkdownHelp -LogPath' {

    It 'checks the log exists' {
        $drop = "TestDrive:\MD\SingleCommand"
        Remove-Item -rec $drop -ErrorAction SilentlyContinue
        New-MarkdownHelp -Command Add-History -OutputFolder $drop | Out-Null
        Update-MarkdownHelp -Path $drop -LogPath "$drop\platyPsLog.txt"
        (Get-Childitem $drop\platyPsLog.txt).Name | Should Be 'platyPsLog.txt'
    }
}


Describe 'Get-MarkdownMetadata' {
    Context 'Simple markdown file' {
        Set-Content -Path TestDrive:\foo.md -Value @'

---
a : 1

b: 2
foo: bar
---

this text would be ignored
'@
        It 'can parse out yaml snippet' {
            $d = Get-MarkdownMetadata TestDrive:\foo.md
            $d.Count | Should Be 3
            $d['a'] = '1'
            $d['b'] = '2'
            $d['foo'] = 'bar'
        }
    }
}

Describe 'Update-MarkdownHelp with New-MarkdownHelp inlined functionality' {
    $OutputFolder = 'TestDrive:\update-new'

    $originalFiles = New-MarkdownHelp -Module platyPS -OutputFolder $OutputFolder -WithModulePage

    It 'creates markdown in the first place' {
        $originalFiles | Should Not Be $null
        $originalFiles | Select-Object -First 2 | Remove-Item
    }

    It 'updates markdown and creates removed files again' {
        $updatedFiles = Update-MarkdownHelpModule -Path $OutputFolder
        ($updatedFiles | Measure-Object).Count | Should Be (($originalFiles | Measure-Object).Count - 1)
    }

    $OutputFolderDiff = 'TestDrive:\update-new\Test-RefreshModuleFunctionality.md'

    it 'remove platyPS.md and make sure its gone' {
        $File = $originalFiles | Where { $_ -like '*platyPS.md' }
        Remove-Item -Path $File -Confirm:$false
        $FileList = Get-ChildItem -Path $OutputFolder | % { Write-Output $_.FullName }
        ($FileList | Measure-Object).Count | Should Be (($originalFiles | Measure-Object).Count - 1)
    }

    it 'update MarkdownHelpFile with -RefreshModulePage' {
        $UpdatedFiles = Update-MarkdownHelpModule -Path $OutputFolder -RefreshModulePage
        ($UpdatedFiles | Measure-Object).Count | Should Be (($originalFiles | Measure-Object).Count)
        $UpdatedFiles | Where { $_ -like '*platyPS.md' } | Should -BeLike '*platyPS.md'
    }
    it 'update MarkdownHelpFile with -RefreshModulePage with parameter ModulePagePath' {
        $UpdatedFiles = Update-MarkdownHelpModule -Path $OutputFolder -RefreshModulePage -ModulePagePath $OutputFolderDiff
        ($UpdatedFiles | Measure-Object).Count | Should Be (($originalFiles | Measure-Object).Count)
        $UpdatedFiles | Where { $_ -like '*platyPS.md' } | Should -Be $Null
        $UpdatedFiles | Where { $_ -like '*Test-RefreshModuleFunctionality.md' } | Should -Not -Be $Null
    }
}

Describe 'Update-MarkdownHelp reflection scenario' {

    function normalizeEnds([string]$text)
    {
        $text -replace "`r`n?|`n", "`r`n"
    }

    $OutputFolder = 'TestDrive:\CoolStuff'

    # bootstraping docs from some code
    function global:Get-MyCoolStuff
    {
        param(
            [string]$Foo
        )
    }

    $v1md = New-MarkdownHelp -command Get-MyCoolStuff -OutputFolder $OutputFolder

    It 'produces original stub' {
        $v1md.Name | Should Be 'Get-MyCoolStuff.md'
    }

    It 'produce a dummy example' {
        $v1md.FullName | Should FileContentMatch '### Example 1'
    }

    $v1markdown = $v1md | Get-Content -Raw

    $newFooDescription = normalizeEnds @'
ThisIsFooDescription

It has mutlilines.
And [hyper](http://link.com).

- And a list. Yeap.
- Good stuff?
'@

    It 'can update stub' {
        $v15markdown = $v1markdown -replace ($LocalizedData.ParameterDescription -f 'Foo'), $newFooDescription
        $v15markdown | Should BeLike "*ThisIsFooDescription*"
        Set-Content -Encoding UTF8 -Path $v1md -Value $v15markdown
    }

    # change definition of the function with additional parameter
    function global:Get-MyCoolStuff
    {
        param(
            [string]$Foo,
            [string]$Bar
        )
    }

    $v2md = Update-MarkdownHelp $v1md -Verbose -AlphabeticParamsOrder

    It 'upgrades stub' {
        $v2md.Name | Should Be 'Get-MyCoolStuff.md'
    }

    $v2maml = New-ExternalHelp -Path $v2md.FullName -OutputPath "$OutputFolder\v2"
    $v2markdown = $v2md | Get-Content -raw
    $help = Get-HelpPreview -Path $v2maml

    It 'has both parameters' {
        $names = $help.Parameters.parameter.Name
        ($names | Measure-Object).Count | Should Be 2
        $names[0] | Should Be 'Bar'
        $names[1] | Should Be 'Foo'
    }

    It 'preserves hyperlinks' {
        $v2markdown.Contains($newFooDescription) | Should Be $true
    }

    It 'has updated description for Foo' {
        $fooParam = $help.Parameters.parameter | Where-Object {$_.Name -eq 'Foo'}
        normalizeEnds($fooParam.Description.Text | Out-String) | Should Be (normalizeEnds @'
ThisIsFooDescription

It has mutlilines. And hyper (http://link.com).

- And a list. Yeap.

- Good stuff?

'@)
    }

    It 'has a placeholder for example' {
        ($Help.examples.example | Measure-Object).Count | Should Be 1
        $e = $Help.examples.example
        $e.Title | Should Match '-+ Example 1 -+'
        $e.Code | Should Match 'PS C:\>*'
    }

    It 'Confirms that Update-MarkdownHelp correctly populates the Default Parameterset' -Skip:$global:IsUnix {
        $outputOriginal = "TestDrive:\MarkDownOriginal"
        $outputUpdated = "TestDrive:\MarkDownUpdated"
        New-Item -ItemType Directory -Path $outputOriginal
        New-Item -ItemType Directory -Path $outputUpdated
        New-MarkdownHelp -Command "Add-Computer" -OutputFolder $outputOriginal -Force
        Copy-Item -Path (Join-Path $outputOriginal Add-Computer.md) -Destination (Join-Path $outputUpdated Add-Computer.md)
        Update-MarkdownHelp -Path $outputFolder
        (Get-Content (Join-Path $outputOriginal Add-Computer.md)) | Should Be (Get-Content (Join-Path $outputUpdated Add-Computer.md))
    }

    It 'parameter type should not be fullname' {
        $expectedParameters = normalizeEnds @'
Type: String
Type: String

'@
        $expectedSyntax = normalizeEnds @'
Get-MyCoolStuff [[-Foo] <String>] [[-Bar] <String>] [<CommonParameters>]

'@
        normalizeEnds($v2md | Get-Content | Where-Object {$_.StartsWith('Type: ')} | Out-String) | Should Be $expectedParameters
        normalizeEnds($v2md | Get-Content | Where-Object {$_.StartsWith('Get-MyCoolStuff')} | Out-String) | Should Be $expectedSyntax
    }

    $v3md = Update-MarkdownHelp $v2md -Verbose -AlphabeticParamsOrder -UseFullTypeName

    $v3markdown = $v3md | Get-Content

    It 'parameter type should be fullname' {
        $expectedParameters = normalizeEnds @'
Type: System.String
Type: System.String

'@
        $expectedSyntax = normalizeEnds @'
Get-MyCoolStuff [[-Foo] <String>] [[-Bar] <String>] [<CommonParameters>]

'@
        normalizeEnds($v3markdown | Where-Object {$_.StartsWith('Type: ')} | Out-String) | Should Be $expectedParameters
        normalizeEnds($v3markdown | Where-Object {$_.StartsWith('Get-MyCoolStuff')} | Out-String) | Should Be $expectedSyntax
    }

    It 'all the other part should be the same except line with parameters' {
        $expectedContent = $v2md | Get-Content | Select-String -pattern "Type: |Get-MyCoolStuff" -notmatch | Out-String
        $v3markdown | Select-String -pattern "Type: |Get-MyCoolStuff" -notmatch | Out-String | Should Be $expectedContent

    }
}

Describe 'Update Markdown Help' {

    $output = "TestDrive:\"

    $ValidHelpFileName = "Microsoft.PowerShell.Archive-help.xml"
    $md = @'
---
external help file: ABadFileName-Help.xml
online version: http://go.microsoft.com/fwlink/?LinkId=821655
schema: 2.0.0
---

# Expand-Archive
## SYNOPSIS
Extracts files from a specified archive (zipped) file.

## SYNTAX

### Path (Default)
```
Expand-Archive [-Path] <String> [[-DestinationPath] <String>] [-Force] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

### LiteralPath
```
Expand-Archive -LiteralPath <String> [[-DestinationPath] <String>] [-Force] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

## DESCRIPTION
The Expand-Archive cmdlet extracts files from a specified zipped archive file to a specified destination folder.
An archive file allows multiple files to be packaged, and optionally compressed, into a single zipped file for easier distribution and storage.

## EXAMPLES

### Example 1: Extract the contents of an archive
```
PS C:\>Expand-Archive -LiteralPath C:\Archives\Draft.Zip -DestinationPath C:\Reference
```

This command extracts the contents of an existing archive file, Draft.zip, into the folder specified by the DestinationPath parameter, C:\Reference.

### Example 2: Extract the contents of an archive in the current folder
```
PS C:\>Expand-Archive -Path Draft.Zip -DestinationPath C:\Reference
```

This command extracts the contents of an existing archive file in the current folder, Draft.zip, into the folder specified by the DestinationPath parameter, C:\Reference.

## PARAMETERS

### -DestinationPath
Specifies the path to the folder in which you want the command to save extracted files.
Enter the path to a folder, but do not specify a file name or file name extension.
This parameter is required.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 2
Default value:
Accept pipeline input: False
Accept wildcard characters: False
```

### -Force
Forces the command to run without asking for user confirmation.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value:
Accept pipeline input: False
Accept wildcard characters: False
```

### -LiteralPath
Specifies the path to an archive file.
Unlike the Path parameter, the value of LiteralPath is used exactly as it is typed.
Wildcard characters are not supported.
If the path includes escape characters, enclose each escape character in single quotation marks, to instruct Windows PowerShell not to interpret any characters as escape sequences.

```yaml
Type: String
Parameter Sets: LiteralPath
Aliases: PSPath

Required: True
Position: Named
Default value:
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Path
Specifies the path to the archive file.

```yaml
Type: String
Parameter Sets: Path
Aliases:

Required: True
Position: 1
Default value:
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -Confirm
Prompts you for confirmation before running the cmdlet.Prompts you for confirmation before running the cmdlet.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: cf

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -WhatIf
Shows what would happen if the cmdlet runs.
The cmdlet is not run.Shows what would happen if the cmdlet runs.
The cmdlet is not run.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: wi

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String
You can pipe a string that contains a path to an existing archive file.

## OUTPUTS

### System.IO.FileInfo or System.IO.DirectoryInfo

## NOTES

## RELATED LINKS

[Compress-Archive]()
'@

    Set-Content -Path "$outFolder\Expand-Archive.md" -Value $md
    Update-MarkdownHelp -Path "$outFolder\Expand-Archive.md"
    $updatedMd = Get-Content "$outFolder\Expand-Archive.md"

    It 'Verifies that a bad metadata value for the help file is fixed on update' {
        $MetaData = Get-MarkdownMetadata -Markdown ($updatedMd | Out-String)
        $MetaData["external help file"] | Should Be $ValidHelpFileName
    }

    @('## ', '### ') | ForEach-Object {
        It "use a single spacing for $_ sections" {
            $lineStart = $_
            for ($i=2; $i -lt $updatedMd.Count; $i++)
            {
                if ($updatedMd[$i].StartsWith($lineStart))
                {
                    $updatedMd[$i - 1] | Should Be ''
                    $updatedMd[$i - 2] | Should Not Be ''
                }
            }
        }
    }
}

Describe 'Create About Topic Markdown and Txt' {

    $output = "TestDrive:\"
    $aboutTopicName = "PlatyPS"
    $templateLocation = (Split-Path ((Get-Module $aboutTopicName).Path) -Parent) + "\templates\aboutTemplate.md"
    $AboutTopicsOutputFolder = Join-Path $output "About"
    New-Item -Path $AboutTopicsOutputFolder -ItemType Directory


    It 'Checks the about topic is created with proper file name, and the content is correctly written' {

        $aboutContent = Get-Content $templateLocation
        $aboutContent = $aboutContent.Replace("{{FileNameForHelpSystem}}",("about_" + $aboutTopicName))
        $aboutContent = $aboutContent.Replace("{{TOPIC NAME}}",$aboutTopicName)

        New-MarkdownAboutHelp -OutputFolder $output -AboutName $aboutTopicName

        Test-Path (Join-Path $output ("about_$($aboutTopicName).md")) | Should Be $true
        Get-Content (Join-Path $output ("about_$($aboutTopicName).md")) | Should Be $aboutContent
    }

    It 'Checks the about topic is created with proper file name, and the content is correctly written - avoiding doubled about' {

        $aboutTopicName = "PlatyPS_about_doubled"

        $aboutContent = Get-Content $templateLocation
        $aboutContent = $aboutContent.Replace("{{FileNameForHelpSystem}}",("about_" + $aboutTopicName))
        $aboutContent = $aboutContent.Replace("{{TOPIC NAME}}",$aboutTopicName)

        New-MarkdownAboutHelp -OutputFolder $output -AboutName $("about_" + $aboutTopicName)

        Test-Path (Join-Path $output ("about_$($aboutTopicName).md")) | Should Be $true
        Get-Content (Join-Path $output ("about_$($aboutTopicName).md")) | Should Be $aboutContent
    }

    It 'Can generate external help for a directly-specified "about" markdown file' {

        New-MarkdownAboutHelp -OutputFolder $output -AboutName 'JustOne'

        $aboutMdPath = Join-Path $output "about_JustOne.md"

        New-ExternalHelp -Path $aboutMdPath -OutputPath $AboutTopicsOutputFolder

        $aboutExternalHelpPath = Join-Path $AboutTopicsOutputFolder 'about_JustOne.help.txt'

        Test-Path $aboutExternalHelpPath | Should Be $true
    }

    It 'Can generate external help for a directly-specified "about" markdown file - avoiding doubled about_' {

        New-MarkdownAboutHelp -OutputFolder $output -AboutName 'about_JustSecond'

        $aboutMdPath = Join-Path $output "about_JustSecond.md"

        New-ExternalHelp -Path $aboutMdPath -OutputPath $AboutTopicsOutputFolder

        $aboutExternalHelpPath = Join-Path $AboutTopicsOutputFolder 'about_JustSecond.help.txt'

        Test-Path $aboutExternalHelpPath | Should Be $true
    }

    It 'Takes constructed markdown about topics and converts them to text with proper character width'{

        New-MarkdownAboutHelp -OutputFolder $AboutTopicsOutputFolder -AboutName "AboutTopic"

        New-ExternalHelp -Path $AboutTopicsOutputFolder -OutputPath $AboutTopicsOutputFolder

        $lineWidthCheck = $true;

        $AboutTxtFilePath = Join-Path $AboutTopicsOutputFolder "about_AboutTopic.help.txt"

        $AboutContent = Get-Content $AboutTxtFilePath

        $AboutContent | ForEach-Object {
            if($_.Length -gt 80)
            {
                $lineWidthCheck = $false
            }
        }

        (Get-ChildItem $AboutTxtFilePath | Measure-Object).Count | Should Be 1
        $lineWidthCheck | Should Be $true
    }

    It 'Adds a yaml block to the AboutTopic and verifies build as expected'{

        $content = Get-Content (Join-Path $output ("about_$($aboutTopicName).md"))
        $content = ("---Yaml: Stuff---" + $content)
        Set-Content -Value $content -Path (Join-Path $output ("about_$($aboutTopicName).md")) -Force

        Test-Path (Join-Path $output ("about_$($aboutTopicName).md")) | Should Be $true
        Get-Content (Join-Path $output ("about_$($aboutTopicName).md")) | Should Be $content
    }
}

Describe 'Merge-MarkdownHelp' {

    Context 'single file, ignore examples' {
        function global:Test-PlatyPSMergeFunction
        {
            param(
                [string]$First
            )
        }

        $file1 = New-MarkdownHelp -Command Test-PlatyPSMergeFunction -OutputFolder TestDrive:\mergeFile1 -Force
        $maml1 = $file1 | New-ExternalHelp -OutputPath TestDrive:\1.xml -Force
        $help1 = Get-HelpPreview -Path $maml1
        $help1.examples = @()

        function global:Test-PlatyPSMergeFunction
        {
            param(
                [string]$Second
            )
        }

        $file2 = New-MarkdownHelp -Command Test-PlatyPSMergeFunction -OutputFolder TestDrive:\mergeFile2 -Force
        $maml2 = $file2 | New-ExternalHelp -OutputPath TestDrive:\2.xml -Force
        $help2 = Get-HelpPreview -Path $maml2
        $help2.examples = @()

        It 'generates merged markdown with applicable tags' {
            $fileNew = Merge-MarkdownHelp -Path @($file1, $file2) -OutputPath TestDrive:\mergeFileOut -Force

            $mamlNew1 = $fileNew | New-ExternalHelp -OutputPath TestDrive:\1new.xml -Force -ApplicableTag('mergeFile1')
            $helpNew1 = Get-HelpPreview -Path $mamlNew1
            $helpNew1.examples = @()

            $mamlNew2 = $fileNew | New-ExternalHelp -OutputPath TestDrive:\2new.xml -Force -ApplicableTag('mergeFile2')
            $helpNew2 = Get-HelpPreview -Path $mamlNew2
            $helpNew2.examples = @()

            ($helpNew1 | Out-String) | Should Be ($help1 | Out-String)
            ($helpNew2 | Out-String) | Should Be ($help2 | Out-String)
        }
    }

    Context 'two file' {
        function global:Test-PlatyPSMergeFunction1() {}
        function global:Test-PlatyPSMergeFunction2() {}

        $files = @()
        $files += New-MarkdownHelp -Command Test-PlatyPSMergeFunction1 -OutputFolder TestDrive:\mergePath1 -Force
        $files += New-MarkdownHelp -Command Test-PlatyPSMergeFunction1, Test-PlatyPSMergeFunction2 -OutputFolder TestDrive:\mergePath2 -Force

        It 'generates merged markdown with applicable tags' {
            $fileNew = Merge-MarkdownHelp -Path $files -OutputPath TestDrive:\mergePathOut -Force

            $mamlNew1 = $fileNew | New-ExternalHelp -OutputPath TestDrive:\1new.xml -Force -ApplicableTag('mergePath1')
            $helpNew1 = Get-HelpPreview -Path $mamlNew1

            $mamlNew2 = $fileNew | New-ExternalHelp -OutputPath TestDrive:\2new.xml -Force -ApplicableTag('mergePath2')
            $helpNew2 = Get-HelpPreview -Path $mamlNew2

            $names1 = $helpNew1.Name
            $names2 = $helpNew2.Name

            ($names1 | measure).Count | Should Be 1
            $names1 | Should Be 'Test-PlatyPSMergeFunction1'

            ($names2 | measure).Count | Should Be 2
            $names2[0] | Should Be 'Test-PlatyPSMergeFunction1'
            $names2[1] | Should Be 'Test-PlatyPSMergeFunction2'
        }
    }
}

Describe 'New-YamlHelp' {

    New-YamlHelp "$root\docs\New-YamlHelp.md" -OutputFolder "$outFolder\yaml" -Force

    $yamlContent = Get-Content "$outFolder\yaml\New-YamlHelp.yml" -Raw

    $deserializer = (New-Object -TypeName 'YamlDotNet.Serialization.DeserializerBuilder').WithNamingConvention((New-Object -TypeName 'YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention')).Build()

    $yamlModel = $deserializer.Deserialize($yamlContent, [type]"Markdown.MAML.Model.YAML.YamlCommand")

    It 'serializes key properties correctly' {
        $yamlModel.Name | Should Be 'New-YamlHelp'
        $yamlModel.Module.Name | Should Be 'platyPS'

        $yamlModel.RequiredParameters.Count | Should Be 2

        $yamlModel.RequiredParameters[0].Name | Should Be 'Path'
        $yamlModel.RequiredParameters[1].Name | Should Be 'OutputFolder'

        $yamlModel.OptionalParameters.Count | Should Be 2

        $yamlModel.OptionalParameters[0].Name | Should Be 'Encoding'
        $yamlModel.OptionalParameters[1].Name | Should Be 'Force'
    }

    It 'throws for OutputFolder that is a file'{
        { New-YamlHelp "$root\docs\New-YamlHelp.md" -OutputFolder "$outFolder\yaml\New-YamlHelp.yml" } | Should Throw
    }
}
