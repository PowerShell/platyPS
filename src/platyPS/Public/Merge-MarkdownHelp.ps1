Function Merge-MarkdownHelp 
 {

    [CmdletBinding()]
    [OutputType([System.IO.FileInfo[]])]
    param(
        [Parameter(Mandatory=$true,
            ValueFromPipeline=$true)]
        [SupportsWildcards()]
        [string[]]$Path,

        [Parameter(Mandatory=$true)]
        [string]$OutputPath,

        [System.Text.Encoding]$Encoding = $script:UTF8_NO_BOM,

        [Switch]$ExplicitApplicableIfAll,

        [Switch]$Force,

        [string]$MergeMarker = "!!! "
    )

    begin
    {
        validateWorkingProvider
        $MarkdownFiles = @()
    }

    process
    {
        $MarkdownFiles += GetMarkdownFilesFromPath $Path
    }

    end
    {
        function log
        {
            param(
                [string]$message,
                [switch]$warning
            )

            $message = "[Update-MarkdownHelp] $([datetime]::now) $message"
            if ($warning)
            {
                Write-Warning $message
            }
            else
            {
                Write-Verbose $message
            }
        }

        if (-not $MarkdownFiles)
        {
            log -warning ($LocalizedData.NoMarkdownFiles -f $Path)
            return
        }

        function getTags
        {
            param($files)

            ($files | Split-Path | Split-Path -Leaf | Group-Object).Name
        }

        # use parent folder names as tags
        $allTags = getTags $MarkdownFiles
        log "Using following tags for the merge: $tags"
        $fileGroups = $MarkdownFiles | Group-Object -Property Name
        log "Found $($fileGroups.Count) file groups"

        $fileGroups | ForEach-Object {
            $files = $_.Group
            $groupName = $_.Name

            $dict = New-Object 'System.Collections.Generic.Dictionary[string, Markdown.MAML.Model.MAML.MamlCommand]'
            $files | ForEach-Object {
                $model = GetMamlModelImpl $_.FullName -ForAnotherMarkdown -Encoding $Encoding
                # unwrap List of 1 element
                $model = $model[0]
                $tag = getTags $_
                log "Adding tag $tag and $model"
                $dict[$tag] = $model
            }

            $tags = $dict.Keys
            if (($allTags | measure-object).Count -gt ($tags | measure-object).Count -or $ExplicitApplicableIfAll)
            {
                $newMetadata = @{ $script:APPLICABLE_YAML_HEADER = $tags -join ', ' }
            }
            else
            {
                $newMetadata = @{}
            }

            $merger = New-Object Markdown.MAML.Transformer.MamlMultiModelMerger -ArgumentList $null, (-not $ExplicitApplicableIfAll), $MergeMarker
            $newModel = $merger.Merge($dict)

            $md = ConvertMamlModelToMarkdown -mamlCommand $newModel -metadata $newMetadata -PreserveFormatting
            $outputFilePath = Join-Path $OutputPath $groupName
            MySetContent -path $outputFilePath -value $md -Encoding $Encoding -Force:$Force # yeild
        }
    }

}
