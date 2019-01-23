<#
.SYNOPSIS
    Builds the MarkDown/MAML DLL and assembles the final package in out\platyPS.
#>
[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    $Configuration = "Debug",
    [switch]$SkipDocs,
    [string]$DotnetCli
)

function Find-DotnetCli() {
    [string] $DotnetCli = ''
    $dotnetCmd = Get-Command dotnet
    return $dotnetCmd.Path
}


if (-not $DotnetCli) {
    $DotnetCli = Find-DotnetCli
}

if (-not $DotnetCli) {
    throw "dotnet cli is not found in PATH, install it from https://docs.microsoft.com/en-us/dotnet/core/tools"
} else {
    Write-Host "Using dotnet from $DotnetCli"
}

if (Get-Variable -Name IsCoreClr -ValueOnly -ErrorAction SilentlyContinue) {
    $framework = 'netstandard1.6'
} else {
    $framework = 'net451'
}

[string] $OutFolder = "$PSScriptRoot\out"
[string] $PublishFolder = "$PSScriptRoot\publish"
[string] $SourceFolder = "$PSScriptRoot\src"

& $DotnetCli publish $PSScriptRoot/src/Markdown.MAML -f $framework --output=$PublishFolder /p:Configuration=$Configuration

$assemblyPaths = (
    (Resolve-Path "$PublishFolder/Markdown.MAML.dll").Path,
    (Resolve-Path "$PublishFolder/YamlDotNet.dll").Path
)

# copy artifacts
New-Item -Type Directory out -ErrorAction SilentlyContinue > $null
Copy-Item -Rec -Force $SourceFolder\platyPS out
foreach ($assemblyPath in $assemblyPaths) {
    $assemblyFileName = [System.IO.Path]::GetFileName($assemblyPath)
    $outputPath = "$OutFolder\platyPS\$assemblyFileName"
    if ((-not (Test-Path $outputPath)) -or
        (Test-Path $outputPath -OlderThan (Get-ChildItem $assemblyPath).LastWriteTime)) {
        Copy-Item $assemblyPath $OutFolder\platyPS
    } else {
        Write-Host -Foreground Yellow "Skip $assemblyFileName copying"
    }
}

# copy schema file and docs
Copy-Item $PSScriptRoot\platyPS.schema.md $OutFolder\platyPS
New-Item -Type Directory $OutFolder\platyPS\docs -ErrorAction SilentlyContinue > $null
Copy-Item $PSScriptRoot\docs\* $OutFolder\platyPS\docs\

# copy template files
New-Item -Type Directory $OutFolder\platyPS\templates -ErrorAction SilentlyContinue > $null
Copy-Item $PSScriptRoot\templates\* $OutFolder\platyPS\templates\

# put the right module version
if ($env:APPVEYOR_REPO_TAG_NAME) {
    $manifest = cat -raw $OutFolder\platyPS\platyPS.psd1
    $manifest = $manifest -replace "ModuleVersion = '0.0.1'", "ModuleVersion = '$($env:APPVEYOR_REPO_TAG_NAME)'"
    Set-Content -Value $manifest -Path $OutFolder\platyPS\platyPS.psd1 -Encoding Ascii
}

# dogfooding: generate help for the module
Remove-Module platyPS -ErrorAction SilentlyContinue
Import-Module $OutFolder\platyPS

if (-not $SkipDocs) {
    New-ExternalHelp $PSScriptRoot\docs -OutputPath $OutFolder\platyPS\en-US -Force
    # reload module, to apply generated help
    Import-Module $OutFolder\platyPS -Force
}
