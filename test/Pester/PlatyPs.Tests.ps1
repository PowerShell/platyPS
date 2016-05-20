Set-StrictMode -Version latest
$ErrorActionPreference = 'Stop'

$root = (Resolve-Path $PSScriptRoot\..\..).Path
$outFolder = "$root\out"

Import-Module $outFolder\platyPS -Force

Describe 'New-Markdown' {

    Context 'errors' {
        It 'throw when cannot find module' {
            { New-Markdown -Module __NON_EXISTING_MODULE -OutputFolder TestDrive:\ } | 
                Should Throw "Module __NON_EXISTING_MODULE is not imported in the session. Run 'Import-Module __NON_EXISTING_MODULE'."
        }

        It 'throw when cannot find module' {
            { New-Markdown -command __NON_EXISTING_COMMAND -OutputFolder TestDrive:\ } | 
                Should Throw "Command __NON_EXISTING_COMMAND not found in the session."
        }
    }

    Context 'metadata' {
        It 'generates passed metadata' {
            $file = New-Markdown -command New-Markdown -OutputFolder TestDrive:\ -metadata @{
                FOO = 'BAR'
            }

            $h = Get-MarkdownMetadata -Path $file
            $h['FOO'] | Should Be 'BAR' 
        }

        It 'respects -NoMetadata' {
            $file = New-Markdown -command New-Markdown -OutputFolder TestDrive:\ -NoMetadata
            Get-MarkdownMetadata -Path $file | Should Be $null
        }

        It 'errors on -NoMetadata and -Metadata' {
            { New-Markdown -command New-Markdown -OutputFolder TestDrive:\ -NoMetadata -Metadata @{} } |
                Should Throw '-NoMetadata and -Metadata cannot be specified at the same time'
        }
    }

    Context 'form module' {
        It 'creates few help files for platyPS' {
            $files = New-Markdown -Module PlatyPS -OutputFolder TestDrive:\platyPS
            ($files | measure).Count | Should BeGreaterThan 4
        }
    }

    Context 'Online version link' {
        
        function global:Test-PlatyPSFunction {}

        @('https://github.com/PowerShell/platyPS', '') | % {
            $uri = $_
            It "generates passed online url '$uri'" {
                $a = @{
                    command = 'Test-PlatyPSFunction'
                    OutputFolder = 'TestDrive:\'
                }
                if ($uri) {
                    $a['OnlineVersionUrl'] = $uri
                }
                $file = New-Markdown @a
                $maml = New-ExternalHelp -MarkdownFile $file -OutputPath "TestDrive:\"
                $help = Show-HelpPreview -MamlFilePath $maml -AsObject 
                $link = $help.relatedLinks.navigationLink | ? {$_.linkText -eq 'Online Version:'}
                ($link | measure).Count | Should Be 1
                $link.uri | Should Be $uri
            }
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

            $file = New-Markdown @a
            $maml = New-ExternalHelp -MarkdownFile $file -OutputPath "TestDrive:\"
            $help = Show-HelpPreview -MamlFilePath $maml -AsObject 
            $help.Syntax.syntaxItem.Count | Should Be 2
            $dynamicParam = $help.parameters.parameter | ? {$_.name -eq 'DynamicParameter'}
            ($dynamicParam | measure).Count | Should Be 1
        }
    }

    Context 'Module Landing Page'{
            
        $OutputFolder = "TestDrive:\LandingPageMD\"

        New-Item -ItemType Directory $OutputFolder


        It "generates a landing page from Module"{

            New-Markdown -Module PlatyPS -OutputFolder $OutputFolder -WithModulePage

            $LandingPage = Get-ChildItem (Join-Path $OutputFolder PlatyPS.md)

            $LandingPage | Should Not Be $null

        }

        It "generates a landing page from MAML"{


            New-Markdown -MamlFile (ls "$outFolder\platyPS\en-US\*xml") `
                        -OutputFolder $OutputFolder `
                        -WithModulePage `
                        -ModuleName "PlatyPS"

            $LandingPage = Get-ChildItem (Join-Path $OutputFolder PlatyPS.md)

            $LandingPage | Should Not Be $null

        }
    }
}

Describe 'New-ExternalHelp' {
    function global:Test-OrderFunction { 
        param ([Parameter(Position=3)]$Third, [Parameter(Position=1)]$First, [Parameter()]$Named) 
        $First
        $Third
        $Named
    }

    It "generates right order for syntax" {
        $a = @{
            command = 'Test-OrderFunction'
            OutputFolder = 'TestDrive:\'
        }

        $file = New-Markdown @a
        $maml = New-ExternalHelp -MarkdownFile $file -OutputPath "TestDrive:\"
        $help = Show-HelpPreview -MamlFilePath $maml -AsObject 
        ($help.Syntax.syntaxItem | measure).Count | Should Be 1
        $names = $help.Syntax.syntaxItem.parameter.Name
        ($names | measure).Count | Should Be 3
        $names[0] | Should Be 'First'        
        $names[1] | Should Be 'Third'
        $names[2] | Should Be 'Named'
    }
}

#region PS Objects to MAML Model Tests

Describe 'Get-Help & Get-Command on Add-Computer to build MAML Model Object' {

    Context 'Add-Computer' {
        
        # call non-exported function in the module scope
        $mamlModelObject = & (Get-Module platyPS) { Get-MamlObject -Cmdlet "Add-Computer" }

        It 'Validates attributes by checking several sections of the single attributes for Add-Computer'{
            
            $mamlModelObject.Name | Should be "Add-Computer"
            $mamlModelObject.Synopsis | Should be "Add the local computer to a domain or workgroup."
            $mamlModelObject.Description.Substring(0,135) | Should be "The Add-Computer cmdlet adds the local computer or remote computers to a domain or workgroup, or moves them from one domain to another."
            $mamlModelObject.Notes.Substring(0,31) | Should be "In Windows PowerShell 2.0, the "
        }

        It 'Validates the examples by checking Add-Computer Example 1'{

            $mamlModelObject.Examples[0].Title | Should be "-------------------------- EXAMPLE 1 --------------------------"
            $mamlModelObject.Examples[0].Code | Should be "PS C:\>Add-Computer -DomainName Domain01 -Restart"
            $mamlModelObject.Examples[0].Remarks.Substring(0,120) | Should be "This command adds the local computer to the Domain01 domain and then restarts the computer to make the change effective."

        }
        
        It 'Validates Parameters by checking Add-Computer Computer Name and Local Credential in Domain ParameterSet'{

            $Parameter = $mamlModelObject.Syntax[0].Parameters | ? { $_.Name -eq "ComputerName" }
            $Parameter.Name | Should be "ComputerName"
            $Parameter.Type | Should be "string[]"
            $Parameter.Required | Should be $false
        }        
    }

    Context 'Add-Member' {
        # call non-exported function in the module scope
        $mamlModelObject = & (Get-Module platyPS) { Get-MamlObject -Cmdlet "Add-Member" }

        It 'Fetch MemberSet set name' {
            $MemberSet = $mamlModelObject.Syntax | ? {$_.ParameterSetName -eq 'MemberSet'}
            ($MemberSet | measure).Count | Should Be 1
        }

        It 'populates ParameterValueGroup for MemberType' {
            $Parameters = $mamlModelObject.Syntax.Parameters | ? { $_.Name -eq "MemberType" }
            ($Parameters | measure).Count | Should Be 1
            $Parameters | % {
                $_.Name | Should be "MemberType"
                $_.ParameterValueGroup.Count | Should be 16
            }
        }
    }
}

#endregion 

#region Checking Cab and File Naming Cmdlets

Remove-Item -path "$outFolder\CabTesting\" -Recurse -ErrorAction SilentlyContinue | Out-Null
New-Item -ItemType Directory -Path "$outFolder\CabTesting\Source\Xml\" -ErrorAction SilentlyContinue | Out-Null
New-Item -ItemType Directory -Path "$outFolder\CabTesting\Source\ModuleMd\" -ErrorAction SilentlyContinue | Out-Null
New-Item -ItemType Directory -Path "$outFolder\CabTesting\OutXml" -ErrorAction SilentlyContinue | Out-Null
New-Item -ItemType Directory -Path "$outFolder\CabTesting\OutXml2" -ErrorAction SilentlyContinue | Out-Null
New-Item -ItemType File -Path "$outFolder\CabTesting\Source\Xml\" -Name "HelpXml.xml" -force | Out-Null
New-Item -ItemType File -Path "$outFolder\CabTesting\Source\ModuleMd\" -Name "Module.md" -ErrorAction SilentlyContinue | Out-Null
Set-Content -Path "$outFolder\CabTesting\Source\Xml\HelpXml.xml" -Value "<node><test>Adding test content to ensure cab builds correctly.</test></node>" | Out-Null
Set-Content -Path "$outFolder\CabTesting\Source\ModuleMd\Module.md" -Value "---`r`nModule Name: PlatyPs`r`nModule Guid: 00000000-0000-0000-0000-000000000000`r`nDownload Help Link: Somesite.com`r`nHelp Version: 5.0.0.1`r`nLocale: en-US`r`n---" | Out-Null

Describe 'MakeCab.exe' {

    It 'Validates that MakeCab.exe & Expand.exe exists'{

        (Get-Command MakeCab) -ne $null | Should Be $True
        (Get-Command Expand) -ne $null | Should Be $True

    }
}

Describe 'New-ExternalHelpCab' {

    It 'validates the output of Cab creation' {
        $CmdletContentFolder = "$outFolder\CabTesting\Source\Xml\"
        $OutputPath = "$outFolder\CabTesting\"
        $ModuleMdPageFullPath = "$outFolder\CabTesting\Source\ModuleMd\Module.md"
        
        New-ExternalHelpCab -CmdletContentFolder $CmdletContentFolder -OutputPath $OutputPath -ModuleMdPageFullPath $ModuleMdPageFullPath
        expand "$OutputPath\PlatyPs_00000000-0000-0000-0000-000000000000_en-US_helpcontent.cab" /f:* "$outFolder\CabTesting\OutXml\HelpXml.xml" 
        
        (Get-ChildItem -Filter "*.cab" -Path "$OutputPath").Name | Should Be "PlatyPs_00000000-0000-0000-0000-000000000000_en-US_helpcontent.cab"
        (Get-ChildItem -Filter "*.xml" -Path "$OutputPath\OutXml").Name | Should Be "HelpXml.xml"
    }
}

Describe 'HelpInfo'{
    $OutputPath = "$outFolder\CabTesting\"
    $CmdletContentFolder = "$outFolder\CabTesting\Source\Xml\"

    It 'Creates a help info file'{
        $OutputPath = "$outFolder\CabTesting\"
        $CmdletContentFolder = "$outFolder\CabTesting\Source\Xml\"
        [xml] $PlatyPSHelpInfo = Get-Content  (Join-Path $OutputPath "PlatyPs_00000000-0000-0000-0000-000000000000_helpinfo.xml")

        $PlatyPSHelpInfo | Should Not Be $null
        $PlatyPSHelpInfo.HelpInfo.SupportedUICultures.UICulture.UICultureName | Should Be "en-US"
        $PlatyPSHelpInfo.HelpInfo.SupportedUICultures.UICulture.UICultureVersion | Should Be "5.0.0.1"
    }

    It 'Adds another help locale'{
        $OutputPath = "$outFolder\CabTesting\"
        $CmdletContentFolder = "$outFolder\CabTesting\Source\Xml\"
        $ModuleMdPageFullPath = "$outFolder\CabTesting\Source\ModuleMd\Module.md"
    
        Set-Content -Path "$outFolder\CabTesting\Source\ModuleMd\Module.md" -Value "---`r`nModule Name: PlatyPs`r`nModule Guid: 00000000-0000-0000-0000-000000000000`r`nDownload Help Link: Somesite.com`r`nHelp Version: 5.0.0.1`r`nLocale: fr-FR`r`n---" | Out-Null
        New-ExternalHelpCab -CmdletContentFolder $CmdletContentFolder -OutputPath $OutputPath -ModuleMdPageFullPath $ModuleMdPageFullPath
        [xml] $PlatyPSHelpInfo = Get-Content  (Join-Path $OutputPath "PlatyPs_00000000-0000-0000-0000-000000000000_helpinfo.xml")
        $Count = 0
        $PlatyPSHelpInfo.HelpInfo.SupportedUICultures.UICulture | % {$Count++}
        
        $Count | Should Be 2
    }
}

#endregion

Describe 'Test Log on Update-Markdown'{
    
    It 'checks the log exists'{
    $drop = Join-Path $outFolder "\MD\SingleCommand"
    Remove-Item -Recurse $drop -ErrorAction SilentlyContinue
    New-Markdown -Command Add-History -OutputFolder $drop | Out-Null
    $MDs = Get-ChildItem $drop
    Update-Markdown -MarkdownFile $MDs -LogPath "$drop\platyPSLog.txt"

    $result = Get-Childitem $drop\platyPsLog.txt | Select Name

    $result | Should Not Be $null
    
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
            $d = Get-MarkdownMetadata -Path TestDrive:\foo.md
            $d.Count | Should Be 3
            $d['a'] = '1'
            $d['b'] = '2'
            $d['foo'] = 'bar'
        }
    }
}

Describe 'Update-Markdown with new-markdown inlined functionality' {
    $OutputFolder = 'TestDrive:\update-new'

    $originalFiles = New-Markdown -Module platyPS -OutputFolder $OutputFolder
        
    It 'creates markdown at the first place' {
        $originalFiles | Should Not Be $null
        $originalFiles | Select -First 2 | rm
    }

    It 'updates markdown and creates removed files again' {
        $updatedFiles = Update-Markdown -MarkdownFolder $OutputFolder -Module platyPS
        ($updatedFiles | measure).Count | Should Be (($originalFiles | measure).Count)
    }
}

Describe 'Update-Markdown upgrade schema scenario' {
    $v1md = ls $PSScriptRoot\..\..\Examples\PSReadline.dll-help.md
    $OutputFolder = 'TestDrive:\PSReadline'

    $v1maml = New-ExternalHelp -MarkdownFile $v1md -OutputPath "$OutputFolder\v1"
    $v2md = Update-Markdown -MarkdownFile $v1md -OutputFolder $outFolder -SchemaUpgrade
    $v2maml = New-ExternalHelp -MarkdownFile $v2md -OutputPath "$OutputFolder\v2"

    It 'help preview is the same before and after upgrade' {
        $v1file = Show-HelpPreview -MamlFilePath $v1maml -TextOutputPath "$outFolder\PSReadline.v1.txt"
        $v2file = Show-HelpPreview -MamlFilePath $v2maml -TextOutputPath "$outFolder\PSReadline.v2.txt"
    
        $v1txt = $v1file | cat -Raw
        $v2txt = $v2file | cat -Raw

        $v2txt | Should Be $v1txt
    }
}

Describe 'Update-Markdown reflection scenario' {
    
    $OutputFolder = 'TestDrive:\CoolStuff'

    # bootstraping docs from some code
    function global:Get-MyCoolStuff
    {
        param(
            [string]$Foo
        )
    }

    $v1md = New-Markdown -command Get-MyCoolStuff -OutputFolder $OutputFolder

    It 'produces original stub' {
        $v1md.Name | Should Be 'Get-MyCoolStuff.md'
    }

    It 'produce a dummy example' {
        $v1md.FullName | Should Contain '### Example 1'
    }

    $v1markdown = $v1md | cat -Raw

    $newFooDescription = 'ThisIsFooDescription'

    It 'can update stub' {
        $v15markdown = $v1markdown -replace '{{Fill Foo Description}}', $newFooDescription
        $v15markdown | Should BeLike "*$newFooDescription*"
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

    $v2md = Update-Markdown -MarkdownFile $v1md -Verbose

    It 'upgrades stub' {
        $v2md.Name | Should Be 'Get-MyCoolStuff.md'
    }

    $v2maml = New-ExternalHelp -MarkdownFile $v2md -OutputPath "$OutputFolder\v2"
    $help = Show-HelpPreview -MamlFilePath $v2maml -AsObject 
    
    It 'has both parameters' {
        $names = $help.Parameters.parameter.Name
        ($names | measure).Count | Should Be 2
        $names[0] | Should Be 'Bar'
        $names[1] | Should Be 'Foo'
    }

    It 'has updated description for Foo' {
        $fooParam = $help.Parameters.parameter | ? {$_.Name -eq 'Foo'}
        $fooParam.Description.Text | Should Be $newFooDescription
    }

    It 'has a placeholder for example' {
        ($Help.examples.example | measure).Count | Should Be 1
        $e = $Help.examples.example
        $e.Title | Should Be 'Example 1'
        $e.Code | Should Match 'PS C:\>*'
    }
}

