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

        $outMdFilePath = "$outFolder\Add-Member.md"
        $outMamlFilePath = "$outFolder\Add-Member.dll-help.xml"
        $outOriginalHelp = "$outFolder\Add-Member.original.txt"
        $outGeneratedHelp = "$outFolder\Add-Member.generated.txt"

        $testMamlFile = "$PSScriptRoot\Add-Member.help.xml"

        $maml = Get-Content $testMamlFile -Raw

        # run convertion
        $markdown = Convert-MamlToMarkdown -maml $maml

        # Write the markdown to a file
        $markdown | Out-File $outMdFilePath -Force -Encoding utf8
        
        # TODO: there are some weired problems with line endings in Parser.
        # For now we just re-read file from disk to normalize them.
        $markdown = cat $outMdFilePath
        Set-Content -Path $outMdFilePath -Value ($markdown | Out-String)
        $markdown = cat $outMdFilePath -Raw

        $generatedMaml = [Markdown.MAML.Renderer.MamlRenderer]::MarkdownStringToMamlString($markdown)
        $generatedMaml | Out-File $outMamlFilePath

        # create a test module
        try 
        {
            $generatedModule = New-ModuleFromMaml -MamlFilePath $outMamlFilePath
            # Import the module
            Import-Module $generatedModule.Path -Force -ea Stop

            #$propertiesToValidate = @("Name","SYNOPSIS","INPUTS")
            foreach ($cmdletName in $generatedModule.Cmdlets)
            {
                # get-help using get-help cmdletName
                $originalHelpContent = Get-Help -Name "Microsoft.PowerShell.Utility\$cmdletName" -Full | Out-String
                $generatedHelpContent = Get-Help -Name "$($generatedModule.Name)\$cmdletName" -Full | Out-String
                
                Set-Content -Value $originalHelpContent -Path $outOriginalHelp
                Set-Content -Value $generatedHelpContent -Path $outGeneratedHelp

                $originalHelpContent | Should Be $generatedHelpContent
                <#
                # Validate the object properties
                foreach ($property in $propertiesToValidate)
                {
                    Write-Verbose "Validating property $property" -Verbose
                    if ($expectedHelpContent."$property" -ne $actualHelpContent."$property")
                    { 
                        write-host  "Expected: $($expectedHelpContent."$property") and got: $($actualHelpContent."$property")" -ForegroundColor Red
                    }
                }
                #>
            }
        }
        finally
        {
            Remove-Module $generatedModule.Name -Force -ea SilentlyContinue
            $moduleDirectory = Split-Path $generatedModule.Path
            if (Test-Path $moduleDirectory)
            {
                Remove-Item $moduleDirectory -Force -Recurse
            }
        }
    }
}