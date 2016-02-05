# script to create the final package in out\platyPS

param(
    $Configuration = "Debug"
)

# build .dll
msbuild Markdown.MAML.sln /p:Configuration=$Configuration
$assemblyPath = (Resolve-Path "src\Markdown.MAML\bin\$Configuration\Markdown.MAML.dll").Path

# copy artifacts
mkdir out -ErrorAction SilentlyContinue > $null
cp -Rec -Force src\platyPS out
cp $assemblyPath out\platyPS
cp .\platyPS.schema.md out\platyPS

# put the right module version
if ($env:APPVEYOR_BUILD_VERSION) 
{
    $manifest = cat -raw out\platyPS\platyPS.psd1
    $manifest = $manifest -replace "ModuleVersion = '1.0'", "ModuleVersion = '$($env:APPVEYOR_BUILD_VERSION)'"
    Set-Content -Value $manifest -Path out\platyPS\platyPS.psd1 -Encoding Ascii
}

# dogfooding: generate help for the module
Import-Module $pwd\out\platyPS
$maml = Get-PlatyExternalHelp -markdown (cat -raw .\src\platyPS\platyPS.md)
mkdir out\platyPS\en-US -ErrorAction SilentlyContinue > $null
Set-Content -path out\platyPS\en-US\platyPS.psm1-Help.xml -Value $maml -Encoding UTF8

# reload module, to apply generated help
Import-Module $pwd\out\platyPS -Force
$help = (get-module platyPS).ExportedCommands.Keys | %{
    $_ | % { Get-Help -Name "platyPS\$_" -Full | Out-String }
} | Out-String

# put generated help in out\ directory
Set-Content -Value $help -Encoding Ascii -Path out\platyPS.txt
