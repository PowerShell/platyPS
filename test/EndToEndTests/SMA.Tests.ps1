Set-StrictMode -Version latest
$ErrorActionPreference = 'Stop'

$root = (Resolve-Path $PSScriptRoot\..\..).Path
$outFolder = "$root\out"

Import-Module $outFolder\platyPS -Force

Describe 'Microsoft.PowerShell.Core (SMA) help' {

    $markdown = cat -Raw $PSScriptRoot\SMA.Help.md
    It 'transform without errors' {
        $generatedMaml = Get-PlatyPSExternalHelp -markdown $markdown -Verbose
        $generatedMaml > $outFolder\SMA.dll-help.xml
        $generatedMaml | Should Not Be $null
    }
    
    Get-PlatyPSTextHelpFromMaml $outFolder\SMA.dll-help.xml -TextOutputPath $outFolder\SMA.generated.txt

    $originalHelp = $g.Cmdlets | Microsoft.PowerShell.Core\ForEach-Object { 
        $c = $_
        try 
        {
            Microsoft.PowerShell.Core\Get-Help "$_" -Full
        } 
        catch 
        {
            Write-Warning "Unknown comand $c"
        }
    }
    $originalHelp > $outFolder\SMA.original.txt 
}