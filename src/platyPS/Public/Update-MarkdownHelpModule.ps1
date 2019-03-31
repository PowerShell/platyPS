Function Update-MarkdownHelpModule 
{

    [CmdletBinding()]
    [OutputType([System.IO.FileInfo[]])]
    param(
        [Parameter(Mandatory = $true,
            ValueFromPipeline = $true)]
        [SupportsWildcards()]
        [string[]]$Path,

        [System.Text.Encoding]$Encoding = $script:UTF8_NO_BOM,
        [switch]$RefreshModulePage,
        [string]$ModulePagePath,
        [string]$LogPath,
        [switch]$LogAppend,
        [switch]$AlphabeticParamsOrder,
        [switch]$UseFullTypeName,
        [switch]$UpdateInputOutput,
        [switch]$Force,
        [System.Management.Automation.Runspaces.PSSession]$Session
    )

    begin
    {
        validateWorkingProvider
        $infoCallback = GetInfoCallback $LogPath -Append:$LogAppend
        $MarkdownFiles = @()
    }

    process
    {
    }

    end
    {
        function log
        {
            param(
                [string]$message,
                [switch]$warning
            )

            $message = "[Update-MarkdownHelpModule] $([datetime]::now) $message"
            if ($warning)
            {
                Write-Warning $message
            }

            $infoCallback.Invoke($message)
        }

        foreach ($modulePath in $Path)
        {
            $module = $null
            $h = Get-MarkdownMetadata -Path $modulePath
            # this is pretty hacky and would lead to errors
            # the idea is to find module name from landing page when it's available
            if ($h.$script:MODULE_PAGE_MODULE_NAME)
            {
                $module = $h.$script:MODULE_PAGE_MODULE_NAME | Select-Object -First 1
                log ($LocalizedData.ModuleNameFromPath -f $modulePath, $module)
            }

            if (-not $module)
            {
                Write-Error -Message ($LocalizedData.ModuleNameNotFoundFromPath -f $modulePath)
                continue
            }

            # always append on this call
            log ("[Update-MarkdownHelpModule]" + (Get-Date).ToString())
            log ($LocalizedData.UpdateDocsForModule -f $module, $modulePath)
            $affectedFiles = Update-MarkdownHelp -Session $Session -Path $modulePath -LogPath $LogPath -LogAppend -Encoding $Encoding -AlphabeticParamsOrder:$AlphabeticParamsOrder -UseFullTypeName:$UseFullTypeName -UpdateInputOutput:$UpdateInputOutput -Force:$Force
            $affectedFiles # yeild

            $allCommands = GetCommands -AsNames -Module $Module
            if (-not $allCommands)
            {
                throw $LocalizedData.ModuleOrCommandNotFound -f $Module
            }

            $updatedCommands = $affectedFiles.BaseName
            $allCommands | ForEach-Object {
                if ( -not ($updatedCommands -contains $_) )
                {
                    log ($LocalizedData.CreatingNewMarkdownForCommand -f $_)
                    $newFiles = New-MarkdownHelp -Command $_ -OutputFolder $modulePath -AlphabeticParamsOrder:$AlphabeticParamsOrder -Force:$Force
                    $newFiles # yeild
                }
            }

            if ($RefreshModulePage)
            {
                $MamlModel = New-Object System.Collections.Generic.List[Markdown.MAML.Model.MAML.MamlCommand]
                $files = @()
                $MamlModel = GetMamlModelImpl $affectedFiles -ForAnotherMarkdown -Encoding $Encoding
                NewModuleLandingPage  -RefreshModulePage -ModulePagePath $ModulePagePath -Path $modulePath -ModuleName $module -Module $MamlModel -Encoding $Encoding -Force
            }
        }
    }

}
