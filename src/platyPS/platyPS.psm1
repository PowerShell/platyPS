function Get-PlatyMarkdown
{
    param(
        [Parameter(Mandatory=$true, ValueFromPipeline=$true)]
        [object]$module
    )

    $commands = (get-module platyPS).ExportedCommands.Keys
    $commands | % {
        $command = $_
        $h = Get-Help $module\$command
    }
}
