Function ConvertMamlModelToMarkdown 
 {

    param(
        [ValidateNotNullOrEmpty()]
        [Parameter(Mandatory=$true)]
        [Markdown.MAML.Model.MAML.MamlCommand]$mamlCommand,

        [hashtable]$metadata,

        [switch]$NoMetadata,

        [switch]$PreserveFormatting
    )

    begin
    {
        $parseMode = GetParserMode -PreserveFormatting:$PreserveFormatting
        $r = New-Object Markdown.MAML.Renderer.MarkdownV2Renderer -ArgumentList $parseMode
        $count = 0
    }

    process
    {
        if (($count++) -eq 0 -and (-not $NoMetadata))
        {
            return $r.MamlModelToString($mamlCommand, $metadata)
        }
        else
        {
            return $r.MamlModelToString($mamlCommand, $true) # skip version header
        }
    }

}
