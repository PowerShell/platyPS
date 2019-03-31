Function GetParserMode 
{

    param(
        [switch]$PreserveFormatting
    )

    if ($PreserveFormatting)
    {
        return [Markdown.MAML.Parser.ParserMode]::FormattingPreserve
    }
    else
    {
        return [Markdown.MAML.Parser.ParserMode]::Full
    }

}
