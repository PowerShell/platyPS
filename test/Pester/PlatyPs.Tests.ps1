Set-StrictMode -Version latest
$ErrorActionPreference = 'Stop'

$root = (Resolve-Path $PSScriptRoot\..\..).Path
$outFolder = "$root\out"

Import-Module $outFolder\platyPS -Force

#region PS Objects to MAML Model Tests

# call non-exported function in the module scope
$mamlModelObject = & (Get-Module platyPS) { Get-PlatyPSMamlObject -Cmdlet "Add-Computer" }

Describe 'Get-Help & Get-Command on Add-Computer to build MAML Model Object' {

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

        $Parameter = $mamlModelObject.Syntax[0].Parameters | WHERE { $_.Name -eq "ComputerName" }
        $Parameter.Name | Should be "ComputerName"
        $Parameter.Type | Should be "string[]"
        $Parameter.Required | Should be $false
        $Parameter.AttributesText | Should be "System.Management.Automation.ValidateNotNullOrEmptyAttribute System.Management.Automation.ParameterAttribute"
        

    }
}

#endregion 

New-Item -ItemType Directory -Path "$outFolder\CabTesting\Source\Xml\" -ErrorAction SilentlyContinue | Out-Null
New-Item -ItemType Directory -Path "$outFolder\CabTesting\OutXml" -ErrorAction SilentlyContinue | Out-Null
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

