Function GetMamlModelImpl 
 {

    param(
        [Parameter(Mandatory=$true)]
        [string[]]$markdownFiles,
        [Parameter(Mandatory=$true)]
        [System.Text.Encoding]$Encoding,
        [switch]$ForAnotherMarkdown,
        [String[]]$ApplicableTag
    )

    if ($ForAnotherMarkdown -and $ApplicableTag) {
        throw $LocalizedData.ForAnotherMarkdownAndApplicableTag
    }

    # we need to pass it into .NET IEnumerable<MamlCommand> API
    $res = New-Object 'System.Collections.Generic.List[Markdown.MAML.Model.MAML.MamlCommand]'

    $markdownFiles | ForEach-Object {
        $mdText = MyGetContent $_ -Encoding $Encoding
        $schema = GetSchemaVersion $mdText
        $p = NewMarkdownParser
        $t = NewModelTransformer -schema $schema $ApplicableTag

        $parseMode = GetParserMode -PreserveFormatting:$ForAnotherMarkdown
        $model = $p.ParseString($mdText, $parseMode, $_)
        Write-Progress -Activity $LocalizedData.ParsingMarkdown -Completed
        $maml = $t.NodeModelToMamlModel($model)

        # flatten
        $maml | ForEach-Object {
            if (-not $ForAnotherMarkdown)
            {
                # we are preparing model to be transformed in MAML, need to embeed online version url
                SetOnlineVersionUrlLink -MamlCommandObject $_ -OnlineVersionUrl (GetOnlineVersion $mdText)
            }

            $res.Add($_)
        }
    }

    return @(,$res)

}
