<#
.SYNOPSIS
    Builds the MarkDown/MAML DLL and assembles the final package in out\platyPS.
#>
[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    $Configuration = "Debug",
    [switch]$SkipDocs
)

# Attempts to find the (verified to exist) path to msbuild.exe; returns an empty string if
# not found.
function Find-DotnetCli()
{
    [string] $DotnetCli = ''
    $dotnetCmd = Get-Command dotnet
    return $dotnetCmd.Path
}


# build .dll
[string] $DotnetCli = Find-DotnetCli

if (-not $DotnetCli) {
    throw "I don't know where dotnet cli is."
}

& $DotnetCli publish ./src/Markdown.MAML --output=$pwd/out /p:Configuration=$Configuration

$assemblyPaths = (
    (Resolve-Path "src/Markdown.MAML/out/Markdown.MAML.dll").Path,
    (Resolve-Path "src/Markdown.MAML/out/YamlDotNet.NetStandard.dll").Path
)

# copy artifacts
New-Item -Type Directory out -ErrorAction SilentlyContinue > $null
Copy-Item -Rec -Force src\platyPS out
foreach($assemblyPath in $assemblyPaths)
{
	$assemblyFileName = [System.IO.Path]::GetFileName($assemblyPath)
	$outputPath = "out\platyPS\$assemblyFileName"
	if ((-not (Test-Path $outputPath)) -or
		(Test-Path $outputPath -OlderThan (ls $assemblyPath).LastWriteTime))
	{
		Copy-Item $assemblyPath out\platyPS
	} else {
		Write-Host -Foreground Yellow "Skip $assemblyFileName copying"
	}
}

# copy schema file and docs
Copy-Item .\platyPS.schema.md out\platyPS
New-Item -Type Directory out\platyPS\docs -ErrorAction SilentlyContinue > $null
Copy-Item .\docs\* out\platyPS\docs\

# copy template files
New-Item -Type Directory out\platyPS\templates -ErrorAction SilentlyContinue > $null
Copy-Item .\templates\* out\platyps\templates\

# put the right module version
if ($env:APPVEYOR_REPO_TAG_NAME)
{
    $manifest = cat -raw out\platyPS\platyPS.psd1
    $manifest = $manifest -replace "ModuleVersion = '0.0.1'", "ModuleVersion = '$($env:APPVEYOR_REPO_TAG_NAME)'"
    Set-Content -Value $manifest -Path out\platyPS\platyPS.psd1 -Encoding Ascii
}

# dogfooding: generate help for the module
Remove-Module platyPS -ErrorAction SilentlyContinue
Import-Module $pwd\out\platyPS

if (-not $SkipDocs) {
    New-ExternalHelp docs -OutputPath out\platyPS\en-US -Force
    # reload module, to apply generated help
    Import-Module $pwd\out\platyPS -Force
}
