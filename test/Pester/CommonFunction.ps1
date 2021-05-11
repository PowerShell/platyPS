function normalizeEnds([string]$text)
{
    $text -replace "`r`n?|`n", "`r`n"
}