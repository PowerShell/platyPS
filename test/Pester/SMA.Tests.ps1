Set-StrictMode -Version latest
$ErrorActionPreference = 'Stop'

$root = (Resolve-Path $PSScriptRoot\..\..).Path
$outFolder = "$root\out"
$smaOutputFolder = "$outFolder\SMA"

Import-Module $outFolder\platyPS -Force

Describe 'Microsoft.PowerShell.Core (SMA) help' {

    $module = 'Microsoft.PowerShell.Core'

    It "creates Markdown for $module" {
        Get-PlatyPSMarkdown -Encoding UTF8 -module $module -OutputFolder $smaOutputFolder
    }

    It 'transforms Markdown to MAML with no errors' {
        $generatedMaml = Get-PlatyPSExternalHelp -markdownFolder $smaOutputFolder -Verbose
        $generatedMaml > $outFolder\SMA.dll-help.xml
        $generatedMaml | Should Not Be $null

        # add artifacts to out
        Get-PlatyPSTextHelpFromMaml $outFolder\SMA.dll-help.xml -TextOutputPath $outFolder\SMA.generated.txt
        Get-PlatyPSTextHelpFromMaml $pshome\en-US\System.Management.Automation.dll-help.xml -TextOutputPath $outFolder\SMA.original.txt
    }
}