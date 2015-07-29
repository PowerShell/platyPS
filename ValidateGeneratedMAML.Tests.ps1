
# This function takes a PSMAML file and generates a script module at the given location. In addition, this cmdlet generates an object
# which contains the module name, module path and the list of cmdlets for the generated module
#
function GeneratePSM1Module
{
    param 
    (
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]
        $MamlFilePath,
        [ValidateNotNullOrEmpty()]
        [String]
        $DestinationPath = "$env:TEMP\Modules"
    )

    if (-not (Test-Path $MamlFilePath))
    {
        throw "'$MamlFilePath' does not exist."        
    }

    # Get the file name
    $originalHelpFileName = (Get-Item $MamlFilePath).Name
    
    # Read the malm file
    $xml = [xml](Get-Content $MamlFilePath -Raw -ea SilentlyContinue)
    if (-not $xml)
    {
        throw "Failed to read '$MamlFilePath'" 
    }

    # This logic is for the generate a module name
    $moduleType = $null
    if ($originalHelpFileName.EndsWith(".psm1-help.xml"))
    {
        $moduleType =  ".psm1-help.xml"
    }
    elseif ($originalHelpFileName.EndsWith(".dll-help.xml"))
    {
        $moduleType =  ".dll-help.xml"
    }
    else
    {
        throw "invalid PowerShell module help file $originalHelpFileName"
    }

    # The information for the module to be generated
    $currentCulture = (Get-UICulture).Name
    $moduleName = $originalHelpFileName.Replace($moduleType, "") + "_" + (Get-Random).ToString()
    $moduleFolder = "$destinationPath\$moduleName"
    $helpFileFolder = "$destinationPath\$moduleName\$currentCulture"
    $moduleFilePath = $moduleFolder + "\" + $moduleName + ".psm1"

    # The help file will be renamed to this name
    $helpFileNewName = $moduleName + $moduleType

    # The result object to be generated
    $result = @{
        Name = $null
        Cmdlets = @()
        Path = $null
    }

    $writeFile = $false
    $moduleDefintion = ""
    $template = @'

<#
.ExternalHelp $helpFileName
#>
function $cmdletName
{
    [CmdletBinding()]
    Param
    (
        $Param1,
        $Param2
    )
}
'@

    foreach ($command in $xml.helpItems.command)
    {
        $thisDefinition = $template
        $thisDefinition = $thisDefinition.Replace("`$helpFileName", $helpFileNewName)
        $thisDefinition = $thisDefinition.Replace("`$cmdletName", $command.details.name)        
        $moduleDefintion += "`r`n" + $thisDefinition + "`r`n"
        $writeFile = $true
        $result.Cmdlets += $command.details.name
    }

    if (-not $writeFile)
    {
        Write-Verbose "There aren't any cmdlets definitions on '$MamlFilePath'." -Verbose
        return 
    }

    # Create the module and help content folders.
    #New-Item -Path $moduleFolder -ItemType Directory -Force | Out-Null
    New-Item -Path $helpFileFolder -ItemType Directory -Force | Out-Null

    # Copy the help file
    Copy-Item -Path $MamlFilePath -Destination $helpFileFolder -Force

    # Rename the copied help file
    $filePath = Join-Path $helpFileFolder (Split-Path $MamlFilePath -Leaf)
    Rename-Item -Path $filePath -NewName $helpFileNewName -Force

    # Create the module file
    Set-Content -Value $moduleDefintion -Path $moduleFilePath

    $result.Name = $moduleName
    $result.Path = $moduleFilePath
    return $result
}

# $testMamlFile = "$pshome\en-us\Microsoft.PowerShell.Commands.Utility.dll-help.xml"
$testMamlFile = "C:\Francisco\MarkdownToMAML\SampleMAML\Microsoft.PowerShell.Utility3.psm1-help.xml"

# Import Transform.psm1 
Import-Module .\Transform.psm1 -Force

# get maml as a string
$maml = Get-Content $testMamlFile -Raw

# run convertion
$markdown = Convert-MamlToMarkdown -maml $maml

# Write the markdown to a file
$markdown | Out-File generatedMarkdown.txt -Force

# Load Markdown.MAML.dll
$assemblyPath = "C:\Users\frangom.REDMOND\Documents\GitHub\Markdown.MAML\src\Markdown.MAML\bin\Debug\Markdown.MAML.dll"
Add-Type -Path $assemblyPath

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



