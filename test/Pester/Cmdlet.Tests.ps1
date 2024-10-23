# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

Describe "Miscellaneous cmdlet tests" {
    BeforeAll {
        $platyPScmdlets = Get-Command -Module Microsoft.PowerShell.PlatyPS
    }

    Context "LiteralPath" {
        It 'Cmdlet <cmdlet> should have a "LiteralPath" parameter to match Path' -testcases @(
        foreach($cmd in $platyPScmdlets.Where({$_.Parameters['Path']})) {
            @{ cmdlet = $cmd }
        }
        ) {
            param ($cmdlet)
            $cmdlet.Parameters['LiteralPath'] | Should -Not -BeNullOrEmpty
        }
    }

    Context "Output Type Attribute" {
        It "Cmdlet '<cmdlet>' should have an output type attribute" -TestCases @(
            $platyPScmdlets.ForEach({ @{ Cmdlet = $_ } })
        ) {
            param ($cmdlet)
            if ($cmdlet -is [System.Management.Automation.FunctionInfo]) {
                $outputAttribute = $cmdlet.ScriptBlock.Attributes.Where({$_ -is [System.Management.Automation.OutputTypeAttribute]})
            }
            else {
                $outputAttribute = $cmdlet.ImplementingType.CustomAttributes.Where({$_.AttributeType -eq [System.Management.Automation.OutputTypeAttribute]})
            }
            $outputAttribute | Should -Not -BeNullOrEmpty
        }
    }

    Context "Parameter name/type tests" {
        It "Parameters which accept command help objects should be named CommandHelp" -TestCases @(
            $platyPScmdlets.
                Foreach({$_.Parameters.Values}).
                Where({$_.parametertype -match "CommandHelp"}).
                Foreach({ @{ Parameter = $_} })
        ) {
            param ($parameter)
            $parameter.Name | Should -Match "CommandHelp"
        }
    }

    Context "Tab completion for encoding parameter" {
        It "Cmdlet '<cmdlet>' should have a completer for the Encoding parameter" -testcases @(
            $platyPSCmdlets.Where({$_.Parameters['Encoding']}).Foreach({ @{ Cmdlet = $_; Parameter = $_.Parameters['Encoding'] }})
        ) {
            param ($cmdlet, $parameter)
            $completerType = $parameter.Attributes.Where({$_.TypeId -eq [System.Management.Automation.ArgumentCompleterAttribute]}).Type.Name
            $completerType | Should -Be "EncodingCompleter"
        }
    }

    Context "Metadata parameter" {
        It "Cmdlet '<cmdlet>' should have a 'metadata' parameter" -testcases @(
            $platyPSCmdlets.Where({$_.Name -match "Export-Mark|Export-Yaml|New-MarkdownCommandHelp"}).Foreach({ @{ cmdlet = $_ }})
        ) {
            param ($cmdlet)
            $cmdlet.Parameters['Metadata'] | Should -Not -BeNullOrEmpty
        }
    }

    Context "Cmdlets which change state" {
        It "Cmdlet '<name>' should support ShouldProcess" -testcase @(
            $platyPScmdlets.
                where({$_.verb -match "Export|New|Update"}).
                Where({$_.noun -ne "CommandHelp"}).
                foreach({ @{ Cmdlet = $_ ; name = $_.Name } })
        ) {
            param ($cmdlet)
            if ($cmdlet -is [System.Management.Automation.FunctionInfo]) {
                $cmdletAttribute = $cmdlet.ScriptBlock.Attributes
            }
            else {
                $cmdletAttribute = $cmdlet.ImplementingType.GetCustomAttributes($true).Where({$_.TypeId.Name -eq "CmdletAttribute"})
            }
            $cmdletAttribute.SupportsShouldProcess | Should -Be $true
        }

    }

    Context "Cabinet file tests" {
        BeforeAll {
            $skipTest = $false
            Import-Module Microsoft.PowerShell.Archive
            if (! $IsWindows) {
               $skipTest = $true
               return 
            }

            $OutputPath = "$TestDrive\CabTesting"

            $null = New-Item -ItemType Directory -Path (Join-Path $OutputPath "\Source\Xml\") -ErrorAction SilentlyContinue
            $null = New-Item -ItemType Directory -Path (Join-Path $OutputPath "\Source\ModuleMd\") -ErrorAction SilentlyContinue
            $null = New-Item -ItemType Directory -Path (Join-Path $OutputPath "\OutXml") -ErrorAction SilentlyContinue
            $null = New-Item -ItemType Directory -Path (Join-Path $OutputPath "\OutXml2") -ErrorAction SilentlyContinue
            $null = New-Item -ItemType File -Path (Join-Path $OutputPath "\Source\Xml\") -Name "HelpXml.xml" -force
            $null = New-Item -ItemType File -Path (Join-Path $OutputPath "\Source\Xml\") -Name "Module.resources.psd1"
            $null = New-Item -ItemType File -Path (Join-Path $OutputPath "\Source\ModuleMd\") -Name "Module.md" -ErrorAction SilentlyContinue
            $null = New-Item -ItemType File -Path $OutputPath -Name "PlatyPs_00000000-0000-0000-0000-000000000000_helpinfo.xml" -ErrorAction SilentlyContinue
            Set-Content -Path (Join-Path $OutputPath "\Source\Xml\HelpXml.xml") -Value "<node><test>Adding test content to ensure cab builds correctly.</test></node>"
            Set-Content -Path (Join-Path $OutputPath "\Source\ModuleMd\Module.md") -Value "---","Module Name: PlatyPs","Module Guid: 00000000-0000-0000-0000-000000000000","Download Help Link: Somesite.com","Help Version: 5.0.0.1","Locale: en-US","---"
            $CmdletContentFolder = (Join-Path $OutputPath "\Source\Xml\")
            $ModuleMdPageFullPath = (Join-Path $OutputPath "\Source\ModuleMd\Module.md")
            }

        AfterAll {
            Remove-Module Microsoft.PowerShell.Archive -ErrorAction Ignore
        }

        It 'validates the output of Cab creation' -skip:$skipTest {
            New-HelpCabinetFile -CabinetFilesFolder $CmdletContentFolder -OutputFolder $OutputPath -MarkdownModuleFile $ModuleMdPageFullPath -WarningAction SilentlyContinue
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

        It 'Creates a help info file' -skip:$skipTest {
            [xml] $PlatyPSHelpInfo = Get-Content  (Join-Path $OutputPath "PlatyPs_00000000-0000-0000-0000-000000000000_helpinfo.xml")

            $PlatyPSHelpInfo | Should Not Be $null
            $PlatyPSHelpInfo.HelpInfo.SupportedUICultures.UICulture.UICultureName | Should Be "en-US"
            $PlatyPSHelpInfo.HelpInfo.SupportedUICultures.UICulture.UICultureVersion | Should Be "5.0.0.1"
        }

        It 'validates the version is incremented when the switch is used' -skip {
            New-HelpCabinetFile -CabinetFilesFolder $CmdletContentFolder -OutputFolder $OutputPath -MarkdownModuleFile $ModuleMdPageFullPath -IncrementHelpVersion -WarningAction SilentlyContinue
            [xml] $PlatyPSHelpInfo = Get-Content  (Join-Path $OutputPath "PlatyPs_00000000-0000-0000-0000-000000000000_helpinfo.xml")
            $PlatyPSHelpInfo | Should Not Be $null
            $PlatyPSHelpInfo.HelpInfo.SupportedUICultures.UICulture.UICultureName | Should Be "en-US"
            $PlatyPSHelpInfo.HelpInfo.SupportedUICultures.UICulture.UICultureVersion | Should Be "5.0.0.2"
        }

        It 'Adds another help locale' -skip:$skipTest {
            Set-Content -Path (Join-Path $OutputPath "\Source\ModuleMd\Module.md") -Value "---","Module Name: PlatyPs","Module Guid: 00000000-0000-0000-0000-000000000000","Download Help Link: Somesite.com","Help Version: 5.0.0.1","Locale: en-US","Additional Locale: [fr-FR, ja-JP]","fr-FR Version: 1.2.3.4","ja-JP Version: 2.3.4.5","---"
            New-HelpCabinetFile -CabinetFilesFolder $CmdletContentFolder -OutputFolder $OutputPath -MarkdownModuleFile $ModuleMdPageFullPath -WarningAction SilentlyContinue
            [xml] $PlatyPSHelpInfo = Get-Content  (Join-Path $OutputPath "PlatyPs_00000000-0000-0000-0000-000000000000_helpinfo.xml")
            $Count = 0
            $PlatyPSHelpInfo.HelpInfo.SupportedUICultures.UICulture | ForEach-Object {$Count++}

            $Count | Should Be 3
        }
    }

    Context 'Show-HelpPreview tests' {
        BeforeAll {
            $ch = Import-MarkdownCommandHelp "${PSScriptRoot}/assets/Get-ChildItem.V2.md"
            $mamlPath = $ch | Export-MamlCommandHelp -OutputFolder $TESTDRIVE
        }

        It "Should present a help object when Show-HelpPreview is run" {
            $help = Show-HelpPreview -Path $mamlPath
            $help.psobject.typenames | Should -Contain "HelpInfo"
        }

        It "Should have the proper command name 'Get-ChildItem'" {
            $help = Show-HelpPreview -Path $mamlPath
            $help.name | Should -BeExactly "Get-ChildItem"
        }


    }
}
