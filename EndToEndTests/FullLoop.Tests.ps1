Set-StrictMode -Version latest
$ErrorActionPreference = 'Stop'

$root = (Resolve-Path $PSScriptRoot\..).Path

Import-Module $root\MamlToMarkdown.psm1 -Force
Import-Module $root\MamlUtils.psm1 -Force

# create folder for test run artifacts (intermediate markdown)
$outFolder = "$PSScriptRoot\out"
mkdir $outFolder -ErrorAction SilentlyContinue > $null

# we assume dll is already built
$assemblyPath = (Resolve-Path "$root\src\Markdown.MAML\bin\Debug\Markdown.MAML.dll").Path
Add-Type -Path $assemblyPath

Describe 'Full loop' {
    Context 'Add-Member cmdlet' {

        $testMamlFile = "$PSScriptRoot\Add-Member.help.xml"

        $maml = Get-Content $testMamlFile -Raw

        # run convertion
        $markdown = Convert-MamlToMarkdown -maml $maml

        # Write the markdown to a file
        $markdown | Out-File $outFolder\Add-Member.md -Force

        $generatedMaml = [Markdown.MAML.Renderer.MamlRenderer]::MarkdownStringToMamlString($markdown)

    }
}