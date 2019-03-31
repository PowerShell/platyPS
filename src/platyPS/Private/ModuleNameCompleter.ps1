Function ModuleNameCompleter 
{

    Param (
        $commandName,
        $parameterName,
        $wordToComplete,
        $commandAst,
        $fakeBoundParameter
    )

    Get-Module -Name "$wordToComplete*" |
    ForEach-Object {
        $_.Name
    }
    
}
