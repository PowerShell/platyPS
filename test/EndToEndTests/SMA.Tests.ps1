Set-StrictMode -Version latest
$ErrorActionPreference = 'Stop'

$root = (Resolve-Path $PSScriptRoot\..\..).Path
$outFolder = "$root\out"

Import-Module $outFolder\platyPS\MamlToMarkdown.psm1 -Force
Import-Module $outFolder\platyPS\MamlUtils.psm1 -Force

# we assume dll is already built
$assemblyPath = (Resolve-Path "$outFolder\platyPS\Markdown.MAML.dll").Path
Add-Type -Path $assemblyPath

Describe 'Microsoft.PowerShell.Core (SMA) help' {

    $markdown = cat -Raw $PSScriptRoot\SMA.Help.md
    It 'transform without errors' {
        $generatedMaml = [Markdown.MAML.Renderer.MamlRenderer]::MarkdownStringToMamlString($markdown)
        $generatedMaml > $outFolder\SMA.dll-help.xml
        $generatedMaml | Should Not Be $null
    }
    
    $g = New-ModuleFromMaml -MamlFilePath $outFolder\SMA.dll-help.xml

    try 
    {
        Import-Module $g.Path -Force -ea Stop
        $allHelp = $g.Cmdlets | Microsoft.PowerShell.Core\ForEach-Object { Microsoft.PowerShell.Core\Get-Help "$($g.Name)\$_" -Full }
        $allHelp > $outFolder\SMA.generated.txt
    }
    finally
    {
        Microsoft.PowerShell.Core\Remove-Module $g.Name -Force -ea SilentlyContinue
        $moduleDirectory = Split-Path $g.Path
        if (Test-Path $moduleDirectory)
        {
            Remove-Item $moduleDirectory -Force -Recurse
        }
    }

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