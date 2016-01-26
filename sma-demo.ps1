$originalFolder = $pwd

try 
{
    cd $PSScriptRoot\EndToEndTests

    $root = $PSScriptRoot

    # create folder for test run artifacts (intermediate markdown)
    $outFolder = "$PSScriptRoot\out"
    mkdir $outFolder -ErrorAction SilentlyContinue > $null

    Import-Module $root\MamlUtils.psm1 -Force

    # we assume dll is already built
    $assemblyPath = (Resolve-Path "$root\src\Markdown.MAML\bin\Debug\Markdown.MAML.dll").Path
    Add-Type -Path $assemblyPath

    $r = [Markdown.MAML.Renderer.MamlRenderer]::new()
    $p = [Markdown.MAML.Parser.MarkdownParser]::new()
    $t = [Markdown.MAML.Transformer.ModelTransformer]::new()

    $markdown = cat -Raw .\SMA.Help.md

    $model = $p.ParseString($markdown)
    $maml = $t.NodeModelToMamlModel($model)
    $xml = $r.MamlModelToString($maml)

    cd $outFolder
    $xml > .\SMA.dll-help.xml

    $g = New-ModuleFromMaml -MamlFilePath .\SMA.dll-help.xml

    try 
    {
        Import-Module $g.Path -Force -ea Stop
        $allHelp = $g.Cmdlets | Microsoft.PowerShell.Core\ForEach-Object { 
            Microsoft.PowerShell.Core\Get-Help "$($g.Name)\$_" -Full 
        }
        
        $allHelp > .\SMA.generated.txt
    }
    finally
    {
        Microsoft.PowerShell.Core\Remove-Module $g.Name
    }

    $originalHelp = $g.Cmdlets | Microsoft.PowerShell.Core\ForEach-Object { 
        $c = $_
        try 
        {
            Microsoft.PowerShell.Core\Get-Help "$_" -Full
        } 
        catch 
        {
            Write-Warning "Unknown comand $c"
        }
    }
    $originalHelp > .\SMA.original.txt    

}
finally
{
    # just return back
    cd $originalFolder
}