Function Update-MarkdownHelp 
 {

    [CmdletBinding()]
    [OutputType([System.IO.FileInfo[]])]
    param(
        [Parameter(Mandatory=$true,
            ValueFromPipeline=$true)]
        [SupportsWildcards()]
        [string[]]$Path,

        [System.Text.Encoding]$Encoding = $script:UTF8_NO_BOM,

        [string]$LogPath,
        [switch]$LogAppend,
        [switch]$AlphabeticParamsOrder,
        [switch]$UseFullTypeName,
        [switch]$UpdateInputOutput,
        [Switch]$Force,
        [System.Management.Automation.Runspaces.PSSession]$Session,
        [switch]$ExcludeDontShow
    )

    begin
    {
        validateWorkingProvider
        $infoCallback = GetInfoCallback $LogPath -Append:$LogAppend
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

            $infoCallback.Invoke($message)
        }

        if (-not $MarkdownFiles)
        {
            log -warning ($LocalizedData.NoMarkdownFiles -f $Path)
            return
        }


        $MarkdownFiles | ForEach-Object {
            $file = $_

            $filePath = $file.FullName
            $oldModels = GetMamlModelImpl $filePath -ForAnotherMarkdown -Encoding $Encoding

            if ($oldModels.Count -gt 1)
            {
                log -warning ($LocalizedData.FileContainsMoreThanOneCommand -f $filePath)
                log -warning $LocalizedData.OneCommandPerFile
                return
            }

            $oldModel = $oldModels[0]

            $name = $oldModel.Name
            [Array]$loadedModulesBefore = $(Get-Module | Select-Object -Property Name)
            $command = Get-Command $name -ErrorAction SilentlyContinue
            if (-not $command)
            {
                if ($Force) {
                    if (Test-Path $filePath) {
                        Remove-Item -Path $filePath -Confirm:$false
                        log -warning ($LocalizedData.CommandNotFoundFileRemoved -f $name, $filePath)
                        return
                    }
                } else {
                    log -warning ($LocalizedData.CommandNotFoundSkippingFile -f $name, $filePath)
                    return
                }
            }
            elseif (($null -ne $command.ModuleName) -and ($loadedModulesBefore.Name -notcontains $command.ModuleName))
            {
                log -warning ($LocalizedData.ModuleImporteAutomaticaly -f $($command.ModuleName))
            }

            # update the help file entry in the metadata
            $metadata = Get-MarkdownMetadata $filePath
            $metadata["external help file"] = GetHelpFileName $command
            $reflectionModel = GetMamlObject -Session $Session -Cmdlet $name -UseFullTypeName:$UseFullTypeName -ExcludeDontShow:$ExcludeDontShow.IsPresent
            $metadata[$script:MODULE_PAGE_MODULE_NAME] = $reflectionModel.ModuleName

            $merger = New-Object Markdown.MAML.Transformer.MamlModelMerger -ArgumentList $infoCallback
            $newModel = $merger.Merge($reflectionModel, $oldModel, $UpdateInputOutput)

            if ($AlphabeticParamsOrder)
            {
                SortParamsAlphabetically $newModel
            }

            $md = ConvertMamlModelToMarkdown -mamlCommand $newModel -metadata $metadata -PreserveFormatting
            MySetContent -path $file.FullName -value $md -Encoding $Encoding -Force # yield
        }
    }

}
