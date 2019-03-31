Function NewMarkdownParser 
{

    $warningCallback = GetWarningCallback
    $progressCallback = {
        param([int]$current, [int]$all)
        Write-Progress -Activity $LocalizedData.ParsingMarkdown -status $LocalizedData.Progress -percentcomplete ($current / $all * 100)
    }
    return New-Object -TypeName 'Markdown.MAML.Parser.MarkdownParser' -ArgumentList ($progressCallback, $warningCallback)

}
