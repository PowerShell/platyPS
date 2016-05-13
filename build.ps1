# script to create the final package in out\platyPS

param(
    [ValidateSet('Debug', 'Release')]
    $Configuration = "Debug"
)

# build .dll
msbuild Markdown.MAML.sln /p:Configuration=$Configuration
$assemblyPath = (Resolve-Path "src\Markdown.MAML\bin\$Configuration\Markdown.MAML.dll").Path

# copy artifacts
mkdir out -ErrorAction SilentlyContinue > $null
cp -Rec -Force src\platyPS out
if (-not (Test-Path out\platyPS\Markdown.MAML.dll) -or 
    (ls out\platyPS\Markdown.MAML.dll).LastWriteTime -lt (ls $assemblyPath).LastWriteTime)
{
    cp $assemblyPath out\platyPS
} else {
    Write-Host -Foreground Yellow 'Skip Markdown.MAML.dll copying'
}

cp .\platyPS.schema.md out\platyPS

# put the right module version
if ($env:APPVEYOR_BUILD_VERSION) 
{
    $manifest = cat -raw out\platyPS\platyPS.psd1
    $manifest = $manifest -replace "ModuleVersion = '1.0'", "ModuleVersion = '$($env:APPVEYOR_BUILD_VERSION)'"
    Set-Content -Value $manifest -Path out\platyPS\platyPS.psd1 -Encoding Ascii
}

# dogfooding: generate help for the module
Remove-Module platyPS -ErrorAction SilentlyContinue
Import-Module $pwd\out\platyPS
New-ExternalHelp -MarkdownFolder .\src\platyPS\docs -OutputPath out\platyPS\en-US

# reload module, to apply generated help
Import-Module $pwd\out\platyPS -Force
Show-HelpPreview .\out\platyPS\en-US\platyPS.psm1-Help.xml -TextOutputPath out\platyPS.txt
