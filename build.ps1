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

    [Parameter(ParameterSetName = "Test", Position = 0)]
    [string]$TestPath,

    [Parameter(ParameterSetName = "Test")]
    [string] $XUnitLogPath = "$PSScriptRoot/xunit.tests.xml",

    [Parameter(ParameterSetName = "Test")]
    [string] $PesterLogPath = "$PSScriptRoot/pester.tests.xml",

    [Parameter(ParameterSetName = "Package")]
    [switch]$Package
)

$ModuleName = "Microsoft.PowerShell.PlatyPS"

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
        if ($LASTEXITCODE -ne 0) {
            throw "Build failure."
        }

        $expectedBuildPath = "./bin/$Configuration/net472/"
        $expectedDllPath = "$expectedBuildPath/${ModuleName}.dll"
        $expectedPdbPath = "$expectedBuildPath/${ModuleName}.pdb"

        if (-not (Test-Path $expectedDllPath)) {
            throw "Build did not succeed."
        }

        $moduleRoot = New-Item -Item Directory -Path "$OutputDir/${ModuleName}" -Force
        $depsFolder = New-Item -Item Directory -Path "$moduleRoot/Dependencies" -Force

        $moduleFiles = "$PSScriptRoot/src/${ModuleName}.psd1", "$PSScriptRoot/src/${ModuleName}.Format.ps1xml",$expectedDllPath
        if ($configuration -eq "debug") {
            $moduleFiles += $expectedPdbPath
        }

        $neededAssemblies = "System.Buffers.dll", "System.Memory.dll",
            "System.Numerics.Vectors.dll", "System.Runtime.CompilerServices.Unsafe.dll",
            "Markdig.Signed.dll","YamlDotNet.dll"
        
        $neededAssemblyLocations = $neededAssemblies | ForEach-Object { "${expectedBuildPath}/${_}" }

        Copy-Item -Path $neededAssemblyLocations -Destination $depsFolder -Verbose
        Copy-Item -Path $moduleFiles -Destination $moduleRoot -Verbose
    }
    finally {
        Pop-Location
    }
}
elseif ($PSCmdlet.ParameterSetName -eq 'Test') {
    $pesterTestRoot = "$PSScriptRoot/test/Pester"
    Write-Verbose "Executing Pester tests under $pesterTestRoot" -Verbose

    $sb = "Import-Module -Max 4.99 Pester
        `$PSModuleAutoloadingPreference = 'none'
        `$env:PSModulePath = '${OutputDir}$([io.path]::PathSeparator)${env:PSModulePath}'
        Import-Module -Name '${ModuleName}' -Force
        Push-Location $pesterTestRoot
        Invoke-Pester -Outputformat nunitxml -outputfile $PesterLogPath $TestPath"

    write-verbose -verbose -message "$sb"

    # we need to run on both pwsh and powershell
    $PSEXE = (Get-Process -Id $PID).MainModule.FileName
    Write-Verbose -Verbose -Message ("Running tests on PowerShell Version: " + $PSVersionTable.PSVersion)
    & $PSEXE -noprofile -c "$sb"

    $results = [xml](Get-Content $PesterLogPath)
    if ($results."test-results".failures -ne 0) {
        throw "Pester Tests failed."
    }
}

if ($Package) {
    if (! (Test-Path "$PSScriptRoot/out/Microsoft.PowerShell.PlatyPS")) {
        throw "Module is missing, run build first"
    }

    $moduleDir = Join-Path $OutputDir $ModuleName
    $localRepoName = [guid]::newguid().ToString("N")
    try {
        Register-PSRepository -Name $localRepoName -SourceLocation $PSScriptRoot -PublishLocation $PSScriptRoot -InstallationPolicy Trusted
        Publish-Module -Repository $localRepoName -Path $moduleDir
    }
    finally {
        Unregister-PSRepository -Name $localRepoName -ErrorAction SilentlyContinue
    }
    Get-ChildItem -Path $PSScriptRoot/*.nupkg
}
