cd $PSScriptRoot\EndToEndTests
invoke-pester

$r = [Markdown.MAML.Renderer.MamlRenderer]::new()
$p = [Markdown.MAML.Parser.MarkdownParser]::new()
$t = [Markdown.MAML.Transformer.ModelTransformer]::new()

$markdown = cat -Raw ..\Examples\SMA.Help.md

$model = $p.ParseString($markdown)
$maml = $t.NodeModelToMamlModel($model)
$xml = $r.MamlModelToString($maml)

cd ..\out
$xml > .\SMA.dll-help.xml

$g = New-ModuleFromMaml -MamlFilePath .\SMA.dll-help.xml

try 
{
    Import-Module $g.Path -Force -ea Stop
    $allHelp = $g.Cmdlets | Microsoft.PowerShell.Core\ForEach-Object { Microsoft.PowerShell.Core\Get-Help "$($g.Name)\$_" -Full }
    $allHelp > .\SMA.generated.txt
}
finally
{
    Microsoft.PowerShell.Core\Remove-Module $g.Name
}
