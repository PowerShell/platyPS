Set-StrictMode -Version latest
$ErrorActionPreference = 'Stop'

$root = (Resolve-Path $PSScriptRoot\..\..).Path
$outFolder = "$root\out"

Import-Module $outFolder\platyPS -Force

Describe 'New-MarkdownHelp' {

    Context 'errors' {
        It 'throw when cannot find module' {
            { New-MarkdownHelp -Module __NON_EXISTING_MODULE -OutputFolder TestDrive:\ } | 
                Should Throw "Module __NON_EXISTING_MODULE is not imported in the session. Run 'Import-Module __NON_EXISTING_MODULE'."
        }

        It 'throw when cannot find module' {
            { New-MarkdownHelp -command __NON_EXISTING_COMMAND -OutputFolder TestDrive:\ } | 
                Should Throw "Command __NON_EXISTING_COMMAND not found in the session."
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
                Should Throw '-NoMetadata and -Metadata cannot be specified at the same time'
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
            ($files | measure).Count | Should BeGreaterThan 4
        }
    }

    Context 'from module' {
        try
        {
            New-Module -Name PlatyPSTestModule -ScriptBlock { 
                function Get-AAAA 
                {

                }

                function Set-BBBB 
                {

                }

                # Set-Alias and New-Alias provide two different results
                # when `Get-Command -module Foo` is used to list commands.
                Set-Alias aaaaalias Get-AAAA
                Set-Alias bbbbalias Get-BBBB

                New-Alias -Name 'Fork-AAAA' -Value 'Get-AAAA'

                Export-ModuleMember -Alias Fork-AAAA
                Export-ModuleMember -Alias aaaaalias
                Export-ModuleMember -Alias bbbbalias
                Export-ModuleMember -Function Get-AAAA

            } | Import-Module -Force

            $files = New-MarkdownHelp -Module PlatyPSTestModule -OutputFolder TestDrive:\PlatyPSTestModule -Force
        }
        finally
        {
            Remove-Module PlatyPSTestModule -ErrorAction SilentlyContinue
        }

        It 'generates markdown files only for exported functions' {
            ($files | measure).Count | Should Be 1
            $files.Name | Should Be 'Get-AAAA.md'
        }        
    }
    
    Context 'from command' {
        It 'creates 2 markdown files from command names' {
            $files = New-MarkdownHelp -Command @('New-MarkdownHelp', 'Get-MarkdownMetadata') -OutputFolder TestDrive:\commands -Force
            ($files | measure).Count | Should Be 2
        }
    }

    Context 'Online version link' {
        
        function global:Test-PlatyPSFunction {
            <#
            .LINK
            http://www.fabrikam.com/extension.html
            #>
        }

        @('https://github.com/PowerShell/platyPS', 'http://www.fabrikam.com/extension.html', $null) | % {
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
                        ($link | measure).Count | Should Be 1
                        $link[0].linkText | Should Be $uri
                        $link[0].uri | Should Be 'http://www.fabrikam.com/extension.html'
                    }
                    else {
                        ($link | measure).Count | Should Be 2
                        $link[0].linkText | Should Be 'Online Version:'
                        $link[0].uri | Should Be $uri
                    }
                }
            }
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
            C:\PS> Test-PlatyPSFunction "File"
            File.txt

            .EXAMPLE
            C:\PS> Test-PlatyPSFunction "File" -First "doc"
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
        $content = cat $file

        It 'generates markdown with correct parameter set names' {
            ($content | ? {$_ -eq 'Parameter Sets: (All)'} | measure).Count | Should Be 1
            ($content | ? {$_ -eq 'Parameter Sets: First'} | measure).Count | Should Be 1
            ($content | ? {$_ -eq 'Parameter Sets: Second'} | measure).Count | Should Be 1
        }

        It 'generates markdown with correct synopsis' {
            ($content | ? {$_ -eq 'Adds a file name extension to a supplied name.'} | measure).Count | Should Be 1
        }

        It 'generates markdown with correct help description specified by HelpMessage attribute' {
            ($content | ? {$_ -eq 'First parameter help description'} | measure).Count | Should Be 1
        }

        It 'generates markdown with correct help description specified by comment-based help' {
            ($content | ? {$_ -eq 'Second parameter help description'} | measure).Count | Should Be 1
        }

        It 'generates markdown with placeholder for parameter with no description' {
            ($content | ? {$_ -eq '{{Fill Common Description}}'} | measure).Count | Should Be 1
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
        $content = cat $file

        It 'generates markdown with correct synopsis placeholder' {
            ($content | ? {$_ -eq '{{Fill in the Synopsis}}'} | measure).Count | Should Be 1
        }

        It 'generates markdown with correct help description specified by HelpMessage attribute' {
            ($content | ? {$_ -eq 'First parameter help description'} | measure).Count | Should Be 1
        }

        It 'generates markdown with placeholder for parameter with no description' {
            ($content | ? {$_ -eq '{{Fill Common Description}}'} | measure).Count | Should Be 1
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
            $dynamicParam = $help.parameters.parameter | ? {$_.name -eq 'DynamicParameter'}
            ($dynamicParam | measure).Count | Should Be 1
        }
    }

    Context 'Module Landing Page'{
            
        $OutputFolder = "TestDrive:\LandingPageMD\"

        New-Item -ItemType Directory $OutputFolder


        It "generates a landing page from Module"{

            New-MarkdownHelp -Module PlatyPS -OutputFolder $OutputFolder -WithModulePage -Force

            $LandingPage = Get-ChildItem (Join-Path $OutputFolder PlatyPS.md)

            $LandingPage | Should Not Be $null

        }

        It "generates a landing page from MAML"{

            New-MarkdownHelp -MamlFile (ls "$outFolder\platyPS\en-US\platy*xml") `
                        -OutputFolder $OutputFolder `
                        -WithModulePage `
                        -ModuleName "PlatyPS" `
                        -Force

            $LandingPage = Get-ChildItem (Join-Path $OutputFolder PlatyPS.md)

            $LandingPage | Should Not Be $null

        }
    }
}


###############################################################
###############################################################


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
            Force = $true
        }

        $file = New-MarkdownHelp @a
        $maml = $file | New-ExternalHelp -OutputPath "TestDrive:\" -Force
        $help = Get-HelpPreview -Path $maml 
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
        
        It 'Checks that Help Exists on Computer Running Tests' {

            $Command = Get-Command Add-Computer
            $HelpFileName = Split-Path $Command.HelpFile -Leaf
            $foundHelp = @()
            $paths = $env:PsModulePath.Split(';')
            foreach($path in $paths)
            {
            $path = Split-Path $path -Parent
            $foundHelp += Get-ChildItem -Path $path -Recurse | Where { $_.Name -like "*$HelpFileName"} | Select Name
            }

            $foundHelp.Count | Should BeGreaterThan 0
        }

        # call non-exported function in the module scope
        $mamlModelObject = & (Get-Module platyPS) { GetMamlObject -Cmdlet "Add-Computer" }

        It 'Validates attributes by checking several sections of the single attributes for Add-Computer' {
            
            $mamlModelObject.Name | Should be "Add-Computer"
            $mamlModelObject.Synopsis | Should be "Add the local computer to a domain or workgroup."
            $mamlModelObject.Description.Substring(0,135) | Should be "The Add-Computer cmdlet adds the local computer or remote computers to a domain or workgroup, or moves them from one domain to another."
            $mamlModelObject.Notes.Substring(0,31) | Should be "In Windows PowerShell 2.0, the "
        }

        It 'Validates the examples by checking Add-Computer Example 1' {

            $mamlModelObject.Examples[0].Title | Should be "Example 1: Add a local computer to a domain then restart the computer"
            $mamlModelObject.Examples[0].Code | Should be "PS C:\>Add-Computer -DomainName `"Domain01`" -Restart"
            $mamlModelObject.Examples[0].Remarks.Substring(0,120) | Should be "This command adds the local computer to the Domain01 domain and then restarts the computer to make the change effective."

        }
        
        It 'Validates Parameters by checking Add-Computer Computer Name and Local Credential in Domain ParameterSet'{

            $Parameter = $mamlModelObject.Syntax[0].Parameters | ? { $_.Name -eq "ComputerName" }
            $Parameter.Name | Should be "ComputerName"
            $Parameter.Type | Should be "string[]"
            $Parameter.Required | Should be $false
        }
        
        It 'Validates there is only 1 default parameterset and that it is the domain parameterset for Add-Computer'{
            
            $DefaultParameterSet = $mamlModelObject.Syntax | ? {$_.IsDefault}
            $count = 0
            foreach($set in $DefaultParameterSet)
            {
                $count = $count +1
            }
            $count | Should be 1
            
            $DefaultParameterSetName = $mamlModelObject.Syntax | ? {$_.IsDefault} | Select ParameterSetName
            $DefaultParameterSetName.ParameterSetName | Should be "Domain"
        }        
    }

    Context 'Add-Member' {
        # call non-exported function in the module scope
        $mamlModelObject = & (Get-Module platyPS) { GetMamlObject -Cmdlet "Add-Member" }

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
#########################################################
#region Checking Cab and File Naming Cmdlets

Describe 'New-ExternalHelpCab' {
    $OutputPath = "TestDrive:\CabTesting"

    New-Item -ItemType Directory -Path (Join-Path $OutputPath "\Source\Xml\") -ErrorAction SilentlyContinue | Out-Null
    New-Item -ItemType Directory -Path (Join-Path $OutputPath "\Source\ModuleMd\") -ErrorAction SilentlyContinue | Out-Null
    New-Item -ItemType Directory -Path (Join-Path $OutputPath "\OutXml") -ErrorAction SilentlyContinue | Out-Null
    New-Item -ItemType Directory -Path (Join-Path $OutputPath "\OutXml2") -ErrorAction SilentlyContinue | Out-Null
    New-Item -ItemType File -Path (Join-Path $OutputPath "\Source\Xml\") -Name "HelpXml.xml" -force | Out-Null
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

            New-ExternalHelpCab -CabFilesFolder $CmdletContentFolder -OutputFolder $OutputPath -LandingPagePath $ModuleMdPageFullPath
            $cab = (Get-ChildItem (Join-Path $OutputPath "PlatyPs_00000000-0000-0000-0000-000000000000_en-US_helpcontent.cab")).FullName
            $cabExtract = (Join-Path (Split-Path $cab -Parent) "OutXml")

            $cabExtract = Join-Path $cabExtract "HelpXml.xml"

            expand $cab /f:* $cabExtract
            
            (Get-ChildItem -Filter "*.cab" -Path "$OutputPath").Name | Should Be "PlatyPs_00000000-0000-0000-0000-000000000000_en-US_helpcontent.cab"
            (Get-ChildItem -Filter "*.xml" -Path "$OutputPath").Name | Should Be "PlatyPs_00000000-0000-0000-0000-000000000000_helpinfo.xml"
            (Get-ChildItem -Filter "*.xml" -Path "$OutputPath\OutXml").Name | Should Be "HelpXml.xml"
        }

        It 'Creates a help info file'{
            [xml] $PlatyPSHelpInfo = Get-Content  (Join-Path $OutputPath "PlatyPs_00000000-0000-0000-0000-000000000000_helpinfo.xml")

            $PlatyPSHelpInfo | Should Not Be $null
            $PlatyPSHelpInfo.HelpInfo.SupportedUICultures.UICulture.UICultureName | Should Be "en-US"
            $PlatyPSHelpInfo.HelpInfo.SupportedUICultures.UICulture.UICultureVersion | Should Be "5.0.0.1"
        }

        It 'Adds another help locale'{
        
            Set-Content -Path (Join-Path $OutputPath "\Source\ModuleMd\Module.md") -Value "---`r`nModule Name: PlatyPs`r`nModule Guid: 00000000-0000-0000-0000-000000000000`r`nDownload Help Link: Somesite.com`r`nHelp Version: 5.0.0.1`r`nLocale: fr-FR`r`n---" | Out-Null
            New-ExternalHelpCab -CabFilesFolder $CmdletContentFolder -OutputFolder $OutputPath -LandingPagePath $ModuleMdPageFullPath
            [xml] $PlatyPSHelpInfo = Get-Content  (Join-Path $OutputPath "PlatyPs_00000000-0000-0000-0000-000000000000_helpinfo.xml")
            $Count = 0
            $PlatyPSHelpInfo.HelpInfo.SupportedUICultures.UICulture | % {$Count++}
            
            $Count | Should Be 2
        }
    }
}

#endregion

Describe 'Update-MarkdownHelp -LogPath' {
    
    It 'checks the log exists' {
        $drop = "TestDrive:\MD\SingleCommand"
        rm -rec $drop -ErrorAction SilentlyContinue
        New-MarkdownHelp -Command Add-History -OutputFolder $drop | Out-Null
        Update-MarkdownHelp -Path $drop -LogPath "$drop\platyPSLog.txt"
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
        $originalFiles | Select -First 2 | rm
    }

    It 'updates markdown and creates removed files again' {
        $updatedFiles = Update-MarkdownHelpModule -Path $OutputFolder
        ($updatedFiles | measure).Count | Should Be (($originalFiles | measure).Count - 1)
    }
}

Describe 'Update-MarkdownHelpSchema' {
    $v1md = ls $PSScriptRoot\..\..\Examples\PSReadline.dll-help.md
    $OutputFolder = 'TestDrive:\PSReadline'

    $v1maml = New-ExternalHelp -Path $v1md.FullName -OutputPath "$OutputFolder\v1.xml"
    $v2md = Update-MarkdownHelpSchema -Path $v1md -OutputFolder $outFolder -Force
    $v2maml = New-ExternalHelp $v2md.FullName -OutputPath "$OutputFolder\v2.xml"

    It 'help preview is the same before and after upgrade' {
        Get-HelpPreview -Path $v1maml > TestDrive:\1.txt
        Get-HelpPreview -Path $v2maml > TestDrive:\2.txt
    
        $v1txt = cat -Raw TestDrive:\1.txt
        $v2txt = cat -Raw TestDrive:\2.txt

        $v2txt | Should Be $v1txt
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
        $v1md.FullName | Should Contain '### Example 1'
    }

    $v1markdown = $v1md | cat -Raw

    $newFooDescription = normalizeEnds @'
ThisIsFooDescription

It has mutlilines.
And [hyper](http://link.com).

- And a list. Yeap.
- Good stuff?
'@

    It 'can update stub' {
        $v15markdown = $v1markdown -replace '{{Fill Foo Description}}', $newFooDescription
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

    $v2md = Update-MarkdownHelp $v1md -Verbose

    It 'upgrades stub' {
        $v2md.Name | Should Be 'Get-MyCoolStuff.md'
    }

    $v2maml = New-ExternalHelp -Path $v2md.FullName -OutputPath "$OutputFolder\v2"
    $v2markdown = $v2md | cat -raw 
    $help = Get-HelpPreview -Path $v2maml 
    
    It 'has both parameters' {
        $names = $help.Parameters.parameter.Name
        ($names | measure).Count | Should Be 2
        $names[0] | Should Be 'Bar'
        $names[1] | Should Be 'Foo'
    }
    
    It 'preserves hyperlinks' {
        $v2markdown.Contains($newFooDescription) | Should Be $true
    }

    It 'has updated description for Foo' {
        $fooParam = $help.Parameters.parameter | ? {$_.Name -eq 'Foo'}
        $fooParam.Description.Text | Out-String | Should Be (normalizeEnds @'
ThisIsFooDescription

It has mutlilines. And hyper (http://link.com).

- And a list. Yeap.

- Good stuff?

'@)
    }

    It 'has a placeholder for example' {
        ($Help.examples.example | measure).Count | Should Be 1
        $e = $Help.examples.example
        $e.Title | Should Be 'Example 1'
        $e.Code | Should Match 'PS C:\>*'
    }
    
    It 'Confirms that Update-MarkdownHelp correctly populates the Default Parameterset' {
        $outputOriginal = "TestDrive:\MarkDownOriginal"
        $outputUpdated = "TestDrive:\MarkDownUpdated"
        New-Item -ItemType Directory -Path $outputOriginal
        New-Item -ItemType Directory -Path $outputUpdated
        New-MarkdownHelp -Command "Add-Computer" -OutputFolder $outputOriginal -Force
        Copy-Item -Path (Join-Path $outputOriginal Add-Computer.md) -Destination (Join-Path $outputUpdated Add-Computer.md)
        Update-MarkdownHelp -Path $outputFolder
        (Get-Content (Join-Path $outputOriginal Add-Computer.md)) | Should Be (Get-Content (Join-Path $outputUpdated Add-Computer.md))
    }
}

Describe 'Create About Topic Markdown and Txt' {
    
    $output = "TestDrive:\"
    $aboutTopicName = "PlatyPS"
    $templateLocation = (Split-Path ((Get-Module $aboutTopicName).Path) -Parent) + "\templates\aboutTemplate.md"
    
    
    It 'Checks the about topic is created with proper file name, and the content is correctly written' {
        
        $aboutContent = Get-Content $templateLocation
        $aboutContent = $aboutContent.Replace("{{FileNameForHelpSystem}}",("about_" + $aboutTopicName))
        $aboutContent = $aboutContent.Replace("{{TOPIC NAME}}",$aboutTopicName)

        New-MarkdownAboutHelp -OutputFolder $output -AboutName $aboutTopicName

        Test-Path (Join-Path $output ("about_$($aboutTopicName).md")) | Should Be $true
        Get-Content (Join-Path $output ("about_$($aboutTopicName).md")) | Should Be $aboutContent
    }
    
    It 'Takes constructed markdown about topics and converts them to text with proper character width'{

        $AboutTopicsOutputFolder = Join-Path $output "About"

        New-Item -Path $AboutTopicsOutputFolder -ItemType Directory

        New-MarkdownAboutHelp -OutputFolder $AboutTopicsOutputFolder -AboutName "AboutTopic"

        New-ExternalHelp -Path $AboutTopicsOutputFolder -OutputPath $AboutTopicsOutputFolder
        
        $lineWidthCheck = $true;
        
        $AboutTxtFilePath = Join-Path $AboutTopicsOutputFolder "about_AboutTopic.txt"

        $AboutContent = Get-Content $AboutTxtFilePath
        
        $AboutContent | % {
            if($_.Length -gt 80)
            {
                $lineWidthCheck = $false
            } 
        }
        
        (Get-ChildItem $AboutTxtFilePath | measure).Count | Should Be 1 
        $lineWidthCheck | Should Be $true
    }

    It 'Adds a yaml block to the AboutTopic and verifies buidl as expected'{

        $content = Get-Content (Join-Path $output ("about_$($aboutTopicName).md"))
        $content = ("---Yaml: Stuff---" + $content)
        Set-Content -Value $content -Path (Join-Path $output ("about_$($aboutTopicName).md")) -Force

        Test-Path (Join-Path $output ("about_$($aboutTopicName).md")) | Should Be $true
        Get-Content (Join-Path $output ("about_$($aboutTopicName).md")) | Should Be $content
    }
}