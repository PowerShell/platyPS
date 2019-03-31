Function NewModuleLandingPage 
{

    Param(
        [Parameter(mandatory = $true)]
        [string]
        $Path,
        [Parameter(mandatory = $true)]
        [string]
        $ModuleName,
        [Parameter(mandatory = $true, ParameterSetName = "NewLandingPage")]
        [string]
        $ModuleGuid,
        [Parameter(mandatory = $true, ParameterSetName = "NewLandingPage")]
        [string[]]
        $CmdletNames,
        [Parameter(mandatory = $true, ParameterSetName = "NewLandingPage")]
        [string]
        $Locale,
        [Parameter(mandatory = $true, ParameterSetName = "NewLandingPage")]
        [string]
        $Version,
        [Parameter(mandatory = $true, ParameterSetName = "NewLandingPage")]
        [string]
        $FwLink,
        [Parameter(ParameterSetName = "UpdateLandingPage")]
        [switch]
        $RefreshModulePage,
        [string]$ModulePagePath,
        [Parameter(mandatory = $true, ParameterSetName = "UpdateLandingPage")]
        [System.Collections.Generic.List[Markdown.MAML.Model.MAML.MamlCommand]]
        $Module,
        [Parameter(mandatory = $true)]
        [System.Text.Encoding]$Encoding = $script:UTF8_NO_BOM,
        [switch]$Force
    )

    begin
    {
        if ($ModulePagePath)
        {
            $LandingPagePath = $ModulePagePath
        }
        else
        {
            $LandingPageName = $ModuleName + ".md"
            $LandingPagePath = Join-Path $Path $LandingPageName
        }
    }

    process
    {
        $Description = $LocalizedData.Description

        if ($RefreshModulePage)
        {
            if (Test-Path $LandingPagePath)
            {
                $OldLandingPageContent = Get-Content -Raw $LandingPagePath
                $OldMetaData = Get-MarkdownMetadata -Markdown $OldLandingPageContent
                $ModuleGuid = $OldMetaData["Module Guid"]
                $FwLink = $OldMetaData["Download Help Link"]
                $Version = $OldMetaData["Help Version"]
                $Locale = $OldMetaData["Locale"]

                $p = NewMarkdownParser
                $model = $p.ParseString($OldLandingPageContent)
                $index = $model.Children.IndexOf(($model.Children | Where-Object { $_.Text -eq "Description" }))
                $i = 1
                $stillParagraph = $true
                $Description = ""
                while ($stillParagraph -eq $true)
                {
                    $Description += $model.Children[$index + $i].spans.text
                    $i++

                    if ($model.Children[$i].NodeType -eq "Heading")
                    {
                        $stillParagraph = $false
                    }
                }
            }
            else
            {
                $ModuleGuid = $LocalizedData.ModuleGuid
                $FwLink = $LocalizedData.FwLink
                $Version = $LocalizedData.Version
                $Locale = $LocalizedData.Locale
                $Description = $LocalizedData.Description
            }
        }

        $Content = "---`r`nModule Name: $ModuleName`r`nModule Guid: $ModuleGuid`r`nDownload Help Link: $FwLink`r`n"
        $Content += "Help Version: $Version`r`nLocale: $Locale`r`n"
        $Content += "---`r`n`r`n"
        $Content += "# $ModuleName Module`r`n## Description`r`n"
        $Content += "$Description`r`n`r`n## $ModuleName Cmdlets`r`n"

        if ($RefreshModulePage)
        {
            $Module | ForEach-Object {
                $command = $_
                if (-not $command.Synopsis)
                {
                    $Content += "### [" + $command.Name + "](" + $command.Name + ".md)`r`n" + $LocalizedData.Description + "`r`n`r`n"
                }
                else
                {
                    $Content += "### [" + $command.Name + "](" + $command.Name + ".md)`r`n" + $command.Synopsis + "`r`n`r`n"
                }
            }
        }
        else
        {
            $CmdletNames | ForEach-Object {
                $Content += "### [" + $_ + "](" + $_ + ".md)`r`n" + $LocalizedData.Description + "`r`n`r`n"
            }
        }

        MySetContent -Path $LandingPagePath -value $Content -Encoding $Encoding -Force:$Force # yeild
    }


}
