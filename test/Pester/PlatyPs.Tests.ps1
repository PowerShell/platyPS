Set-StrictMode -Version latest
$ErrorActionPreference = 'Stop'

$root = (Resolve-Path $PSScriptRoot\..\..).Path
$outFolder = "$root\out"

New-Item -ItemType Directory -Path "$outFolder\CabTesting\Source\Xml\" -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Path "$outFolder\CabTesting\OutXml" -ErrorAction SilentlyContinue
New-Item -ItemType File -Path "$outFolder\CabTesting\Source\Xml\" -Name "HelpXml.xml" -force

Import-Module $outFolder\platyPS -Force

Describe 'New-PlatyPsCab' {

    $markdown = cat -Raw $PSScriptRoot\SMA.Help.md
    It 'validates the output of Cab creation' {
        $Source = "$outFolder\CabTesting\Source\Xml\"
        $Destination = "$outFolder\CabTesting\"
        $Module = "PlatyPs" 
        $GUID = "00000000-0000-0000-0000-000000000000"
        $Locale = "en-US"
        
        New-PlatyPsCab -Source $Source -Destination $Destination -Module $Module -GUID $GUID -Locale $Locale
        expand "$Destination\PlatyPs_00000000-0000-0000-0000-000000000000_en-US_helpcontent.cab" /f:* "$outFolder\CabTesting\OutXml\" 
        
        (Get-ChildItem -Filter "*.cab" -Path "$Destination\CabTesting").Name | Should Be "PlatyPs_00000000-0000-0000-0000-000000000000_en-US_helpcontent.cab"
        (Get-ChildItem -Filter "*.xml" -Path "$Destination\CabTesting\OutXml").Name | Should Be "HelpXml.xml"
    }
    
    Get-PlatyPSTextHelpFromMaml $outFolder\SMA.dll-help.xml -TextOutputPath $outFolder\SMA.generated.txt
    Get-PlatyPSTextHelpFromMaml $pshome\en-US\System.Management.Automation.dll-help.xml -TextOutputPath $outFolder\SMA.original.txt
}