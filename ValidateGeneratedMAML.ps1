

$testMamlFile = "$pshome\en-us\Microsoft.PowerShell.Commands.Utility.dll-help.xml"
$testMamlFile = "Examples\Microsoft.PowerShell.Utility.psm1-help.xml"
#$testMamlFile = "C:\Francisco\MarkdownToMAML\SampleMAML\Microsoft.PowerShell.Utility3.psm1-help.xml"

# Import Transform.psm1 
Import-Module .\Transform.psm1 -Force

# get maml as a string
$maml = Get-Content $testMamlFile -Raw

# run convertion
$markdown = Convert-MamlToMarkdown -maml $maml

# Write the markdown to a file
$markdown | Out-File generatedMarkdown.txt -Force

# Load Markdown.MAML.dll

# Create the parser object
$parser = [Markdown.MAML.Parser.MarkdownParser]::new()
$doc = $parser.ParseString($markdown) 
$mamlRenderer = [Markdown.MAML.Renderer.MamlRenderer]::new()
$modelTransformer = [Markdown.MAML.Transformer.ModelTransformer]::new()

$generatedMaml = $mamlRenderer.MamlModelToString($modelTransformer).NodeModelToMamlModel($doc)

# Generate the module
$generatedModule = GeneratePSM1Module -MamlFilePath $testMamlFile

try 
{
    # Import the module
    Import-Module $generatedModule.Path -Force -ea Stop

    $propertiesToValidate = @("Name","SYNOPSIS","INPUTS")

    foreach ($cmdletName in $generatedModule.Cmdlets)
    {
        # get-help using get-help cmdletName
        $expectedHelpContent = Get-Help Add-Member | ? {$_.modulename -ne $generatedModule.Name}
        $actualHelpContent = Get-Help -Name "$($generatedModule.Name + "\" + $cmdletName)"

        # Validate the object properties
        foreach ($property in $propertiesToValidate)
        {
            Write-Verbose "Validating property $property" -Verbose
            if ($expectedHelpContent."$property" -ne $actualHelpContent."$property")
            { 
                write-host  "Expected: $($expectedHelpContent."$property") and got: $($actualHelpContent."$property")" -ForegroundColor Red
            }
        }
    }
}
finally
{
    Remove-Module $generatedModule.Path -Force -ea SilentlyContinue
    $moduleDirectory = Split-Path $generatedModule.Path
    if (Test-Path $moduleDirectory)
    {
        Remove-Item $moduleDirectory -Force -Recurse
    }
}


