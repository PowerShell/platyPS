Set-StrictMode -Version latest
$ErrorActionPreference = 'Stop'

$root = (Resolve-Path $PSScriptRoot\..\..).Path
$outFolder = "$root\out"

Import-Module $outFolder\platyPS -Force

#region PS Objects to MAML Model Tests

Describe 'Get-Help & Get-Command on Add-Computer to build MAML Model Object' {

    Context 'Add-Computer' {
        
        # call non-exported function in the module scope
        $mamlModelObject = & (Get-Module platyPS) { Get-PlatyPSMamlObject -Cmdlet "Add-Computer" }

        It 'Validates attributes by checking several sections of the single attributes for Add-Computer'{
            
            $mamlModelObject.Name | Should be "Add-Computer"
            $mamlModelObject.Synopsis | Should be "Add the local computer to a domain or workgroup."
            $mamlModelObject.Description.Substring(0,135) | Should be "The Add-Computer cmdlet adds the local computer or remote computers to a domain or workgroup, or moves them from one domain to another."
            $mamlModelObject.Notes.Substring(0,31) | Should be "The Cmdlet category is: Cmdlet."
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
        $mamlModelObject = & (Get-Module platyPS) { Get-PlatyPSMamlObject -Cmdlet "Add-Member" }

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

New-Item -ItemType Directory -Path "$outFolder\CabTesting\Source\Xml\" -ErrorAction SilentlyContinue | Out-Null
New-Item -ItemType Directory -Path "$outFolder\CabTesting\OutXml" -ErrorAction SilentlyContinue | Out-Null
New-Item -ItemType Directory -Path "$outFolder\CabTesting\OutXml2" -ErrorAction SilentlyContinue | Out-Null
New-Item -ItemType File -Path "$outFolder\CabTesting\Source\Xml\" -Name "HelpXml.xml" -force | Out-Null
Set-Content -Path "$outFolder\CabTesting\Source\Xml\HelpXml.xml" -Value "<node><test>Adding test content to ensure cab builds correctly.</test></node>" | Out-Null

Describe 'MakeCab.exe' {

    It 'Validates that MakeCab.exe & Expand.exe exists'{

        (Get-Command MakeCab) -ne $null | Should Be $True
        (Get-Command Expand) -ne $null | Should Be $True

    }
}

Describe 'New-PlatyPsCab' {

    It 'validates the output of Cab creation' {
        $Source = "$outFolder\CabTesting\Source\Xml\"
        $Destination = "$outFolder\CabTesting\"
        $Module = "PlatyPs" 
        $GUID = "00000000-0000-0000-0000-000000000000"
        $Locale = "en-US"
        
        New-PlatyPsCab -Source $Source -Destination $Destination -Module $Module -GUID $GUID -Locale $Locale
        expand "$Destination\PlatyPs_00000000-0000-0000-0000-000000000000_en-US_helpcontent.cab" /f:* "$outFolder\CabTesting\OutXml\HelpXml.xml" 
        
        (Get-ChildItem -Filter "*.cab" -Path "$Destination").Name | Should Be "PlatyPs_00000000-0000-0000-0000-000000000000_en-US_helpcontent.cab"
        (Get-ChildItem -Filter "*.xml" -Path "$Destination\OutXml").Name | Should Be "HelpXml.xml"
    }
}

Describe 'Format-PlatyPsHelpXml' {

    $Destination = "$outFolder\CabTesting\OutXml2\"
    $MamlFullPath = "$outFolder\CabTesting\Source\Xml\HelpXml.xml"
    $ModuleName = "CheckModule.dll"

    Format-PlatyPsHelpXml -MamlHelpXmlFullPath $MamlFullPath -ModuleSourceFileName $ModuleName -Destination  $Destination

    It 'Checks that the xml file is named properly after using the Format-PlatyPsHelpXml command' {
        
        (Get-ChildItem -Path "$outFolder\CabTesting\OutXml2\CheckModule.dll-help.xml").Name | Should Be "CheckModule.dll-help.xml"

    }

}

#endregion

Describe 'Get-PlatyPSYamlMetadata' {
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
            $d = Get-PlatyPSYamlMetadata -Path TestDrive:\foo.md
            $d.Count | Should Be 3
            $d['a'] = '1'
            $d['b'] = '2'
            $d['foo'] = 'bar'
        }
    }
}

