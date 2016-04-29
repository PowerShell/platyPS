Set-StrictMode -Version latest
$ErrorActionPreference = 'Stop'

$root = (Resolve-Path $PSScriptRoot\..\..).Path
$outFolder = "$root\out"

Import-Module $outFolder\platyPS -Force

#region PS Objects to MAML Model Tests

$mamlModelObject = Get-PlatyPSMamlObject -Cmdlet "Add-Computer"

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