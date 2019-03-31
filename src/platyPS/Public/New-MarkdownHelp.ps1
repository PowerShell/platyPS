Function New-MarkdownHelp 
{

    [CmdletBinding()]
    [OutputType([System.IO.FileInfo[]])]
    param(
        [Parameter(Mandatory = $true,
            ValueFromPipeline = $true,
            ParameterSetName = "FromModule")]
        [string[]]$Module,

        [Parameter(Mandatory = $true,
            ParameterSetName = "FromCommand")]
        [string[]]$Command,

        [Parameter(Mandatory = $true,
            ParameterSetName = "FromMaml")]
        [string[]]$MamlFile,

        [Parameter(ParameterSetName = "FromModule")]
        [Parameter(ParameterSetName = "FromCommand")]
        [System.Management.Automation.Runspaces.PSSession]$Session,

        [Parameter(ParameterSetName = "FromMaml")]
        [switch]$ConvertNotesToList,

        [Parameter(ParameterSetName = "FromMaml")]
        [switch]$ConvertDoubleDashLists,

        [switch]$Force,

        [switch]$AlphabeticParamsOrder,

        [hashtable]$Metadata,

        [Parameter(ParameterSetName = "FromCommand")]
        [string]$OnlineVersionUrl = '',

        [Parameter(Mandatory = $true)]
        [string]$OutputFolder,

        [switch]$NoMetadata,

        [switch]$UseFullTypeName,

        [System.Text.Encoding]$Encoding = $script:UTF8_NO_BOM,

        [Parameter(ParameterSetName = "FromModule")]
        [Parameter(ParameterSetName = "FromMaml")]
        [switch]$WithModulePage,

        [Parameter(ParameterSetName = "FromModule")]
        [Parameter(ParameterSetName = "FromMaml")]
        [string]$ModulePagePath,

        [Parameter(ParameterSetName = "FromModule")]
        [Parameter(ParameterSetName = "FromMaml")]
        [string]
        $Locale = "en-US",

        [Parameter(ParameterSetName = "FromModule")]
        [Parameter(ParameterSetName = "FromMaml")]
        [string]
        $HelpVersion = $LocalizedData.HelpVersion,

        [Parameter(ParameterSetName = "FromModule")]
        [Parameter(ParameterSetName = "FromMaml")]
        [string]
        $FwLink = $LocalizedData.FwLink,

        [Parameter(ParameterSetName = "FromMaml")]
        [string]
        $ModuleName = "MamlModule",

        [Parameter(ParameterSetName = "FromMaml")]
        [string]
        $ModuleGuid = "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",

        [switch]
        $ExcludeDontShow
    )

    begin
    {
        validateWorkingProvider
        New-Item -Type Directory $OutputFolder -ErrorAction SilentlyContinue > $null
    }

    process
    {
        function updateMamlObject
        {
            param(
                [Parameter(Mandatory = $true)]
                [Markdown.MAML.Model.MAML.MamlCommand]$MamlCommandObject
            )

            #
            # Here we define our misc template for new markdown to bootstrape easier
            #

            # Example
            if ($MamlCommandObject.Examples.Count -eq 0)
            {
                $MamlExampleObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlExample

                $MamlExampleObject.Title = $LocalizedData.ExampleTitle
                $MamlExampleObject.Code = @(
                    New-Object -TypeName Markdown.MAML.Model.MAML.MamlCodeBlock ($LocalizedData.ExampleCode, 'powershell')
                )
                $MamlExampleObject.Remarks = $LocalizedData.ExampleRemark

                $MamlCommandObject.Examples.Add($MamlExampleObject)
            }

            if ($AlphabeticParamsOrder)
            {
                SortParamsAlphabetically $MamlCommandObject
            }
        }

        function processMamlObjectToFile
        {
            param(
                [Parameter(ValueFromPipeline = $true)]
                [ValidateNotNullOrEmpty()]
                [Markdown.MAML.Model.MAML.MamlCommand]$mamlObject
            )

            process
            {
                # populate template
                updateMamlObject $mamlObject
                if (-not $OnlineVersionUrl)
                {
                    # if it's not passed, we should get it from the existing help
                    $onlineLink = $mamlObject.Links | Select-Object -First 1
                    if ($onlineLink)
                    {
                        $online = $onlineLink.LinkUri
                        if ($onlineLink.LinkName -eq $script:MAML_ONLINE_LINK_DEFAULT_MONIKER -or $onlineLink.LinkName -eq $onlineLink.LinkUri)
                        {
                            # if links follow standart MS convention or doesn't have name,
                            # remove it to avoid duplications
                            $mamlObject.Links.Remove($onlineLink) > $null
                        }
                    }
                }
                else
                {
                    $online = $OnlineVersionUrl
                }

                $commandName = $mamlObject.Name
                # create markdown
                if ($NoMetadata)
                {
                    $newMetadata = $null
                }
                else
                {
                    # get help file name
                    if ($MamlFile)
                    {
                        $helpFileName = Split-Path -Leaf $MamlFile
                    }
                    else
                    {
                        $a = @{
                            Name = $commandName
                        }

                        if ($module)
                        {
                            # for module case, scope it just to this module
                            $a['Module'] = $module
                        }

                        $helpFileName = GetHelpFileName (Get-Command @a)
                    }

                    Write-Verbose "Maml things module is: $($mamlObject.ModuleName)"

                    $newMetadata = ($Metadata + @{
                            $script:EXTERNAL_HELP_FILE_YAML_HEADER = $helpFileName
                            $script:ONLINE_VERSION_YAML_HEADER     = $online
                            $script:MODULE_PAGE_MODULE_NAME        = $mamlObject.ModuleName
                        })
                }

                $md = ConvertMamlModelToMarkdown -mamlCommand $mamlObject -metadata $newMetadata -NoMetadata:$NoMetadata

                MySetContent -path (Join-Path $OutputFolder "$commandName.md") -value $md -Encoding $Encoding -Force:$Force
            }
        }

        if ($NoMetadata -and $Metadata)
        {
            throw $LocalizedData.NoMetadataAndMetadata
        }

        if ($PSCmdlet.ParameterSetName -eq 'FromCommand')
        {
            $command | ForEach-Object {
                if (-not (Get-Command $_ -ErrorAction SilentlyContinue))
                {
                    throw $LocalizedData.CommandNotFound -f $_
                }

                GetMamlObject -Session $Session -Cmdlet $_ -UseFullTypeName:$UseFullTypeName -ExcludeDontShow:$ExcludeDontShow.IsPresent | processMamlObjectToFile
            }
        }
        else
        {
            if ($module)
            {
                $iterator = $module
            }
            else
            {
                $iterator = $MamlFile
            }

            $iterator | ForEach-Object {
                if ($PSCmdlet.ParameterSetName -eq 'FromModule')
                {
                    if (-not (GetCommands -AsNames -module $_))
                    {
                        throw $LocalizedData.ModuleNotFound -f $_
                    }

                    GetMamlObject -Session $Session -Module $_ -UseFullTypeName:$UseFullTypeName -ExcludeDontShow:$ExcludeDontShow.IsPresent | processMamlObjectToFile

                    $ModuleName = $_
                    $ModuleGuid = (Get-Module $ModuleName).Guid
                    $CmdletNames = GetCommands -AsNames -Module $ModuleName
                }
                else # 'FromMaml'
                {
                    if (-not (Test-Path $_))
                    {
                        throw $LocalizedData.FileNotFound -f $_
                    }

                    GetMamlObject -MamlFile $_ -ConvertNotesToList:$ConvertNotesToList -ConvertDoubleDashLists:$ConvertDoubleDashLists  -ExcludeDontShow:$ExcludeDontShow.IsPresent | processMamlObjectToFile

                    $CmdletNames += GetMamlObject -MamlFile $_ -ExcludeDontShow:$ExcludeDontShow.IsPresent | ForEach-Object { $_.Name }
                }

                if ($WithModulePage)
                {
                    if (-not $ModuleGuid)
                    {
                        $ModuleGuid = "00000000-0000-0000-0000-000000000000"
                    }
                    if ($ModuleGuid.Count -gt 1)
                    {
                        Write-Warning -Message $LocalizedData.MoreThanOneGuid
                    }
                    # yeild
                    NewModuleLandingPage  -Path $OutputFolder `
                        -ModulePagePath $ModulePagePath `
                        -ModuleName $ModuleName `
                        -ModuleGuid $ModuleGuid `
                        -CmdletNames $CmdletNames `
                        -Locale $Locale `
                        -Version $HelpVersion `
                        -FwLink $FwLink `
                        -Encoding $Encoding `
                        -Force:$Force
                }
            }
        }
    }

}
