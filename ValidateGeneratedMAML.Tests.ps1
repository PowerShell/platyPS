
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
        Cmldets = @()
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
        $result.Cmldets += $command.details.name
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
    Rename-Item -Path $filePath -NewName $helpFileNewName -Force -Verbose

    # Create the module file
    Set-Content -Value $moduleDefintion -Path $moduleFilePath

    $result.Name = $moduleName
    $result.Path = $moduleFilePath
    return $result
}

# Generate the module
$testModule = GeneratePSM1Module -MamlFilePath C:\Francisco\MarkdownToMAML\SampleMAML\Microsoft.PowerShell.Utility2.psm1-help.xml
$testModule

<#
#Describe "Validate PSMAML" {



try 
{
    # Import the module
    Import-Module $testModule.Path -Force -ea Stop

    
    # get-help using get-help cmdletName
    # get-help using get-help ModuleName\cmdletName
    foreach ($cmdletName in $testModule.Cmldets)
    {
        $expectedHelpContent = Get-Help -Name $cmdletName
        $actualHelpContent = Get-Help -Name "$($testModule.Name + "\" + $cmdletName)"

        # Compare both object
    }
}
finally
{
    Remove-Module $testModule.Path -Force -ea SilentlyContinue
    $moduleDirectory = Split-Path $testModule.Path
    if (Test-Path $moduleDirectory )
    {
        Remove-Item $moduleDirectory -Force -Recurse
    }
}

#>


