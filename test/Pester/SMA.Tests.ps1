Set-StrictMode -Version latest
$ErrorActionPreference = 'Stop'

$root = (Resolve-Path $PSScriptRoot\..\..).Path
$outFolder = "$root\out"

Import-Module $outFolder\platyPS -Force

Describe 'Microsoft.PowerShell.Core (SMA) help' {

    $markdown = cat -Raw $PSScriptRoot\SMA.Help.md
    It 'transform without errors' -Skip:(-not $env:APPVEOR) {
        $generatedMaml = Get-PlatyPSExternalHelp -markdown $markdown -Verbose
        $generatedMaml > $outFolder\SMA.dll-help.xml
        $generatedMaml | Should Not Be $null

        # add artifacts to out
        Get-PlatyPSTextHelpFromMaml $outFolder\SMA.dll-help.xml -TextOutputPath $outFolder\SMA.generated.txt
        Get-PlatyPSTextHelpFromMaml $pshome\en-US\System.Management.Automation.dll-help.xml -TextOutputPath $outFolder\SMA.original.txt
    }
}