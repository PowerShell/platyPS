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
function Find-MsBuildPath()
{
    [string] $msbuildPath = ''

    $msbuildCmd = Get-Command -Name msbuild -ErrorAction Ignore

    if ($msbuildCmd) {
        $msbuildPath = $msbuildCmd.Path
    } else {
        Write-Warning 'Searching for msbuild'

        # For more info on vswhere.exe:
        #    https://blogs.msdn.microsoft.com/heaths/2017/02/25/vswhere-available/
        #    https://github.com/Microsoft/vswhere/wiki/Find-MSBuild
        $vswherePath = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
        if (Test-Path $vswherePath) {

            $vsInstallPath = & $vswherePath -latest `
                                            -requires Microsoft.VisualStudio.Component.Roslyn.Compiler, Microsoft.Component.MSBuild `
                                            -property installationPath

            if (($LASTEXITCODE -eq 0) -and $vsInstallPath) {
                $msbuildPath = Join-Path $vsInstallPath 'MSBuild\15.0\Bin\MSBuild.exe'
            }
        }

        if ($msbuildPath -and (-not (Test-Path $msbuildPath))) {
            $msbuildPath = ''
        }
    }

    return $msbuildPath
}


# build .dll
[string] $msbuildPath = Find-MsBuildPath

if (-not $msbuildPath) {
    throw "I don't know where msbuild is."
}

if (-not (Get-ChildItem "$PSScriptRoot\packages\*" -ErrorAction Ignore)) {
    # No packages... better restore or things won't go well.

    $nugetCmd = 'nuget'
    if (-not (Get-Command -Name $nugetCmd -ErrorAction Ignore)) {
        $nugetCmd = "$PSScriptRoot\.nuget\NuGet.exe"
    }

    Write-Host -Foreground Cyan 'Attempting nuget package restore'

    try
    {
        Push-Location $PSScriptRoot

        & $nugetCmd restore
    }
    finally
    {
        Pop-Location
    }
}

& $msbuildPath Markdown.MAML.sln /p:Configuration=$Configuration
$assemblyPaths = ((Resolve-Path "src\Markdown.MAML\bin\$Configuration\Markdown.MAML.dll").Path, (Resolve-Path "src\Markdown.MAML\bin\$Configuration\YamlDotNet.dll").Path)

# copy artifacts
mkdir out -ErrorAction SilentlyContinue > $null
cp -Rec -Force src\platyPS out
foreach($assemblyPath in $assemblyPaths)
{
	$assemblyFileName = [System.IO.Path]::GetFileName($assemblyPath)
	$outputPath = "out\platyPS\$assemblyFileName"
	if ((-not (Test-Path $outputPath)) -or
		(Test-Path $outputPath -OlderThan (ls $assemblyPath).LastWriteTime))
	{
		cp $assemblyPath out\platyPS
	} else {
		Write-Host -Foreground Yellow "Skip $assemblyFileName copying"
	}
}

# copy schema file and docs
cp .\platyPS.schema.md out\platyPS
mkdir out\platyPS\docs -ErrorAction SilentlyContinue > $null
cp .\docs\* out\platyPS\docs\

# copy template files
mkdir out\platyPS\templates -ErrorAction SilentlyContinue > $null
cp .\templates\* out\platyps\templates\

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
