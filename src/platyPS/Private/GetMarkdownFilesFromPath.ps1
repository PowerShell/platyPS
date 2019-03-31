Function GetMarkdownFilesFromPath 
 {

    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [SupportsWildcards()]
        [string[]]$Path,

        [switch]$IncludeModulePage
    )

    if ($IncludeModulePage)
    {
        $filter = '*.md'
    }
    else
    {
        $filter = '*-*.md'
    }

    $aboutFilePrefixPattern = 'about_*'


    $MarkdownFiles = @()
    if ($Path) {
        $Path | ForEach-Object {
            if (Test-Path -PathType Leaf $_)
            {
                if ((Split-Path -Leaf $_) -notlike $aboutFilePrefixPattern)
                {
                    $MarkdownFiles += Get-ChildItem $_
                }
            }
            elseif (Test-Path -PathType Container $_)
            {
                $MarkdownFiles += Get-ChildItem $_ -Filter $filter | Where-Object {$_.BaseName -notlike $aboutFilePrefixPattern}
            }
            else
            {
                Write-Error -Message ($LocalizedData.PathNotFound -f $_)
            }
        }
    }

    return $MarkdownFiles

}
