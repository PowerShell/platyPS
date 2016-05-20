Function ModuleNameCompletion {
    Param (
        $commandName, 
        $parameterName, 
        $wordToComplete, 
        $commandAst, 
        $fakeBoundParameter
    )

    Get-Module -Name "$wordToComplete*" | 
        ForEach-Object {
            New-CompletionResult -CompletionText $_.Name -ToolTip $_.Description
        }
}