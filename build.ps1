[CmdletBinding(DefaultParameterSetName = 'Build')]
param(
    [Parameter(ParameterSetName = "Build")]
    [switch] $Clean,

    [Parameter(ParameterSetName = "Build")]
    [ValidateSet("Debug", "Release")]
    [string] $Configuration = "Debug",

    [string] $OutputDir = "$PSScriptRoot/out",

    [Parameter(ParameterSetName = "Test", Mandatory)]
    [switch] $Test,

    [Parameter(ParameterSetName = "Test")]
    [string] $XUnitLogPath = "$PSScriptRoot/xunit.tests.xml",

    [Parameter(ParameterSetName = "Test")]
    [string] $PesterLogPath = "$PSScriptRoot/pester.tests.xml"
)

if ($PSCmdlet.ParameterSetName -eq 'Build') {
    try {

        if ($Clean) {
            if (Test-Path $OutputDir) {
                Write-Verbose -Verbose "Cleaning output directory: $OutputDir"
                Remove-Item -Recurse -path $OutputDir -Force
            }

            $binFolder = "$PSScriptRoot/src/bin"
            if (Test-Path $binFolder) {
                Write-Verbose -Verbose "Cleaning output directory: $binFolder"
                Remove-Item -Recurse -path $binFolder -Force
            }

            $objFolder = "$PSScriptRoot/src/obj"
            if (Test-Path $objFolder) {
                Write-Verbose -Verbose "Cleaning output directory: $objFolder"
                Remove-Item -Recurse -path $objFolder -Force
            }
        }

        Push-Location "$PSScriptRoot/src"
        dotnet build --configuration $Configuration

        $expectedBuildPath = "./bin/$Configuration/net461/"
        $expectedDllPath = "$expectedBuildPath/Microsoft.PowerShell.PlatyPS.dll"

        if (-not (Test-Path $expectedDllPath))
        {
            throw "Build did not succeed."
        }

        $moduleDllPath = Resolve-Path $expectedDllPath

        $moduleRoot = New-Item -Item Directory -Path "$OutputDir/platyPS" -Force

        $fileToCopy = @(
            "$PSScriptRoot/src/platyPS.psd1"
            "$expectedDllPath"
            "$expectedBuildPath/Markdig.Signed.dll"
            "$expectedBuildPath/YamlDotNet.dll"
        )

        Copy-Item -Path $fileToCopy -Destination $moduleRoot -Verbose
    }
    finally {
        Pop-Location
    }
}
elseif ($PSCmdlet.ParameterSetName -eq 'Test') {
    Import-Module -Name "$OutputDir/platyPS" -Force

    $xunitTestRoot = "$PSScriptRoot/test/Microsoft.PowerShell.PlatyPS.Tests"
    Write-Verbose "Executing XUnit tests under $xunitTestRoot" -Verbose

    $xunitTestFailed = $true

    try {
        Push-Location $xunitTestRoot
        dotnet test --test-adapter-path:. "--logger:xunit;LogFilePath=$XUnitLogPath"

        if ($LASTEXITCODE -eq 0) {
            $xunitTestFailed = $false
        }
    }
    finally {
        Pop-Location
    }

    $pesterTestRoot = "$PSScriptRoot/test/Pester"
    Write-Verbose "Executing Pester tests under $pesterTestRoot"

    $results = Invoke-Pester -Script $pesterTestRoot -PassThru -Outputformat nunitxml -outputfile $PesterLogPath

    if ($results.FailedCount -ne 0 -or $xunitTestFailed)
    {
        throw "Tests failed."
    }
}
