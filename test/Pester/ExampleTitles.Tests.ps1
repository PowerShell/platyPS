# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

$exampleTitleCases = @(
    @{
        Name = 'comment-based help border'
        RawTitle = '-------------------------- EXAMPLE 1: Generate a report --------------------------'
        Title = 'EXAMPLE 1: Generate a report'
    }
    @{
        Name = 'trailing dash'
        RawTitle = '-------------------------- EXAMPLE 2: Title ending in - --------------------------'
        Title = 'EXAMPLE 2: Title ending in -'
    }
    @{
        Name = 'leading dash'
        RawTitle = '-------------------------- EXAMPLE 3: - Title starting with a dash --------------------------'
        Title = 'EXAMPLE 3: - Title starting with a dash'
    }
    @{
        Name = 'embedded example heading'
        RawTitle = '-------------------------- EXAMPLE 4: Compare Example 4: with Example 5 --------------------------'
        Title = 'EXAMPLE 4: Compare Example 4: with Example 5'
    }
    @{
        Name = 'colon in title'
        RawTitle = '-------------------------- EXAMPLE 5: Choose a format: JSON or XML --------------------------'
        Title = 'EXAMPLE 5: Choose a format: JSON or XML'
    }
    @{
        Name = 'PlatyPS border and mixed-case heading'
        RawTitle = '--------- Example 6: Generate a report ---------'
        Title = 'Example 6: Generate a report'
    }
    @{
        Name = 'untitled example'
        RawTitle = '-------------------------- EXAMPLE 7 --------------------------'
        Title = 'EXAMPLE 7'
    }
    @{
        Name = 'localized heading'
        RawTitle = '-------------------------- BEISPIEL 8: Example 8: Ausgabe - --------------------------'
        Title = 'BEISPIEL 8: Example 8: Ausgabe -'
    }
    @{
        Name = 'unbordered heading'
        RawTitle = 'Example 9: An unbordered heading -'
        Title = 'Example 9: An unbordered heading -'
    }
    @{
        Name = 'unnumbered title'
        RawTitle = 'Choose a value: -1'
        Title = 'Choose a value: -1'
    }
    @{
        Name = 'unbordered leading dash'
        RawTitle = '- A title'
        Title = '- A title'
    }
    @{
        Name = 'unbordered trailing dashes'
        RawTitle = 'A title --'
        Title = 'A title --'
    }
    @{
        Name = 'legacy variable-width border'
        RawTitle = '--- Example 13: Legacy help ---'
        Title = 'Example 13: Legacy help'
    }
    @{
        Name = 'legacy single-dash border'
        RawTitle = '- Example 14: Legacy help -'
        Title = 'Example 14: Legacy help'
    }
    @{
        Name = 'surrounding whitespace and title dashes'
        RawTitle = "`t--------- EXAMPLE 15: - A title - ---------`r`n"
        Title = 'EXAMPLE 15: - A title -'
    }
    @{
        Name = 'dash-only title'
        RawTitle = '--------- - ---------'
        Title = '-'
    }
    @{
        Name = 'dashes without border separators'
        RawTitle = '---A title---'
        Title = '---A title---'
    }
)
for ($index = 0; $index -lt $exampleTitleCases.Count; $index++) {
    $exampleTitleCases[$index].Index = $index
}

Describe 'Example title preservation' {
    BeforeAll {
        $moduleFolder = Join-Path -Path $TestDrive -ChildPath 'PlatyPSExampleTitles'
        $null = New-Item -Path $moduleFolder -ItemType Directory
        $modulePath = Join-Path -Path $moduleFolder -ChildPath 'PlatyPSExampleTitles.psm1'
        $mamlPath = Join-Path -Path $moduleFolder -ChildPath 'PlatyPSExampleTitles-help.xml'
        @'
$ErrorActionPreference = 'Stop'
function Get-PlatyPSExampleTitle {
    <#
    .EXTERNALHELP PlatyPSExampleTitles-help.xml
    #>
    [CmdletBinding()]
    param()
}
Export-ModuleMember -Function Get-PlatyPSExampleTitle
'@ | Set-Content -LiteralPath $modulePath -Encoding utf8

        $templatePath = Join-Path -Path $PSScriptRoot -ChildPath 'assets\legacy-maml\command-uri-in-related-links.xml'
        $fixture = [xml](Get-Content -LiteralPath $templatePath -Raw)
        $commandNamespace = 'http://schemas.microsoft.com/maml/dev/command/2004/10'
        $mamlNamespace = 'http://schemas.microsoft.com/maml/2004/10'
        $devNamespace = 'http://schemas.microsoft.com/maml/dev/2004/10'
        $namespaces = [System.Xml.XmlNamespaceManager]::new($fixture.NameTable)
        $namespaces.AddNamespace('command', $commandNamespace)
        $namespaces.AddNamespace('maml', $mamlNamespace)
        $fixture.SelectSingleNode('//command:name', $namespaces).InnerText = 'Get-PlatyPSExampleTitle'
        $fixture.SelectSingleNode('//command:noun', $namespaces).InnerText = 'PlatyPSExampleTitle'
        $fixture.SelectSingleNode('//command:syntaxItem/maml:name', $namespaces).InnerText = 'Get-PlatyPSExampleTitle'
        $examples = $fixture.SelectSingleNode('//command:examples', $namespaces)

        foreach ($testCase in $exampleTitleCases) {
            $example = $fixture.CreateElement('command', 'example', $commandNamespace)
            $title = $fixture.CreateElement('maml', 'title', $mamlNamespace)
            $title.InnerText = $testCase.RawTitle
            $null = $example.AppendChild($title)
            $code = $fixture.CreateElement('dev', 'code', $devNamespace)
            $code.InnerText = 'Get-PlatyPSExampleTitle'
            $null = $example.AppendChild($code)
            $remarks = $fixture.CreateElement('dev', 'remarks', $devNamespace)
            $paragraph = $fixture.CreateElement('maml', 'para', $mamlNamespace)
            $paragraph.InnerText = 'The example description.'
            $null = $remarks.AppendChild($paragraph)
            $null = $example.AppendChild($remarks)
            $null = $examples.AppendChild($example)
        }
        $fixture.Save($mamlPath)
        Import-Module -Name $modulePath -Force
        $liveHelp = Get-Command -Name Get-PlatyPSExampleTitle | New-CommandHelp
        $mamlHelp = Import-MamlHelp -LiteralPath $mamlPath

        $markdownFile = $liveHelp | Export-MarkdownCommandHelp -OutputFolder (Join-Path $TestDrive 'markdown')
        $markdownHelp = Import-MarkdownCommandHelp -LiteralPath $markdownFile.FullName
        $yamlFile = $markdownHelp | Export-YamlCommandHelp -OutputFolder (Join-Path $TestDrive 'yaml')
        $yamlHelp = Import-YamlCommandHelp -LiteralPath $yamlFile.FullName
        $exportedMamlFile = $markdownHelp | Export-MamlCommandHelp -OutputFolder (Join-Path $TestDrive 'maml')
        $exportedMaml = [xml](Get-Content -LiteralPath $exportedMamlFile.FullName -Raw)
        $exportedNamespaces = [System.Xml.XmlNamespaceManager]::new($exportedMaml.NameTable)
        $exportedNamespaces.AddNamespace('maml', $mamlNamespace)
        $exportedTitles = @($exportedMaml.SelectNodes('//maml:title', $exportedNamespaces))
    }

    AfterAll {
        Remove-Module -Name PlatyPSExampleTitles
    }

    It 'Should import every example through Get-Help and MAML' {
        $liveHelp.Examples.Count | Should -Be $exampleTitleCases.Count
        $mamlHelp.Examples.Count | Should -Be $exampleTitleCases.Count
    }

    It 'Should preserve <Name> from Get-Help' -TestCases $exampleTitleCases {
        param($Index, $Title)
        $liveHelp.Examples[$Index].Title | Should -BeExactly $Title
    }

    It 'Should preserve <Name> from MAML' -TestCases $exampleTitleCases {
        param($Index, $Title)
        $mamlHelp.Examples[$Index].Title | Should -BeExactly $Title
    }

    It 'Should preserve <Name> through Markdown, YAML, and MAML export' -TestCases $exampleTitleCases {
        param($Index, $Title)
        $markdownHelp.Examples[$Index].Title | Should -BeExactly $Title
        $yamlHelp.Examples[$Index].Title | Should -BeExactly $Title
        $exportedTitles[$Index].InnerText | Should -BeExactly "--------- $Title ---------"
        $markdownHelp.Examples[$Index].Remarks | Should -Match 'Get-PlatyPSExampleTitle'
    }
}
