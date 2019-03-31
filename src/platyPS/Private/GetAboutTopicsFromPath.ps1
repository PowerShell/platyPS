Function GetAboutTopicsFromPath 
{

    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string[]]$Path,
        [string[]]$MarkDownFilesAlreadyFound
    )

    function ConfirmAboutBySecondHeaderText
    {
        param(
            [string]$AboutFilePath
        )

        $MdContent = Get-Content -raw $AboutFilePath
        $MdParser = New-Object -TypeName 'Markdown.MAML.Parser.MarkdownParser' `
            -ArgumentList { param([int]$current, [int]$all)
            Write-Progress -Activity $LocalizedData.ParsingMarkdown -status $LocalizedData.Progress -percentcomplete ($current / $all * 100) }
        $MdObject = $MdParser.ParseString($MdContent)

        if ($MdObject.Children[1].text.length -gt 5)
        {
            if ($MdObject.Children[1].text.substring(0, 5).ToUpper() -eq "ABOUT")
            {
                return $true
            }
        }

        return $false
    }

    $AboutMarkDownFiles = @()

    if ($Path)
    {
        $Path | ForEach-Object {
            if (Test-Path -PathType Leaf $_)
            {
                if (ConfirmAboutBySecondHeaderText($_))
                {
                    $AboutMarkdownFiles += Get-ChildItem $_
                }
            }
            elseif (Test-Path -PathType Container $_)
            {
                if ($MarkDownFilesAlreadyFound)
                {
                    $AboutMarkdownFiles += Get-ChildItem $_ -Filter '*.md' | Where-Object { ($_.FullName -notin $MarkDownFilesAlreadyFound) -and (ConfirmAboutBySecondHeaderText($_.FullName)) }
                }
                else
                {
                    $AboutMarkdownFiles += Get-ChildItem $_ -Filter '*.md' | Where-Object { ConfirmAboutBySecondHeaderText($_.FullName) }
                }
            }
            else
            {
                Write-Error -Message ($LocalizedData.AboutFileNotFound -f $_)
            }
        }
    }



    return $AboutMarkDownFiles

}
