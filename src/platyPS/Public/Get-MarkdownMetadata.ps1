Function Get-MarkdownMetadata 
 {

    [CmdletBinding(DefaultParameterSetName="FromPath")]

    param(
        [Parameter(Mandatory=$true,
            ValueFromPipeline=$true,
            ValueFromPipelineByPropertyName=$true,
            Position=1,
            ParameterSetName="FromPath")]
        [SupportsWildcards()]
        [string[]]$Path,

        [Parameter(Mandatory=$true,
            ParameterSetName="FromMarkdownString")]
        [string]$Markdown
    )

    process
    {
        if ($PSCmdlet.ParameterSetName -eq 'FromMarkdownString')
        {
            return [Markdown.MAML.Parser.MarkdownParser]::GetYamlMetadata($Markdown)
        }
        else # FromFile)
        {
            GetMarkdownFilesFromPath $Path -IncludeModulePage | ForEach-Object {
                $md = Get-Content -Raw $_.FullName
                [Markdown.MAML.Parser.MarkdownParser]::GetYamlMetadata($md) # yeild
            }
        }
    }

}
