#region PlatyPS

## DEVELOPERS NOTES & CONVENTIONS
##
##  1. Non-exported functions (subroutines) should avoid using
##     PowerShell standard Verb-Noun naming convention.
##     They should use camalCase or PascalCase instead.
##  2. SMALL subroutines, used only from ONE function
##     should be placed inside the parent function body.
##     They should use camalCase for the name.
##  3. LARGE subroutines and subroutines used from MORE THEN ONE function
##     should be placed after the IMPLEMENTATION text block in the middle
##     of this module.
##     They should use PascalCase for the name.
##  4. Add comment "# yeild" on subroutine calls that write values to pipeline.
##     It would help keep code maintainable and simplify ramp up for others.
##

Import-LocalizedData -BindingVariable LocalizedData -FileName platyPS.Resources.psd1

## Script constants

$script:EXTERNAL_HELP_FILE_YAML_HEADER = 'external help file'
$script:ONLINE_VERSION_YAML_HEADER = 'online version'
$script:SCHEMA_VERSION_YAML_HEADER = 'schema'
$script:APPLICABLE_YAML_HEADER = 'applicable'

$script:UTF8_NO_BOM = New-Object System.Text.UTF8Encoding -ArgumentList $False
$script:SET_NAME_PLACEHOLDER = 'UNNAMED_PARAMETER_SET'
# TODO: this is just a place-holder, we can do better
$script:DEFAULT_MAML_XML_OUTPUT_NAME = 'rename-me-help.xml'

$script:MODULE_PAGE_MODULE_NAME = "Module Name"
$script:MODULE_PAGE_GUID = "Module Guid"
$script:MODULE_PAGE_LOCALE = "Locale"
$script:MODULE_PAGE_FW_LINK = "Download Help Link"
$script:MODULE_PAGE_HELP_VERSION = "Help Version"
$script:MODULE_PAGE_ADDITIONAL_LOCALE = "Additional Locale"

$script:MAML_ONLINE_LINK_DEFAULT_MONIKER = 'Online Version:'

function New-MarkdownHelp
{
    [CmdletBinding()]
    [OutputType([System.IO.FileInfo[]])]
    param(
        [Parameter(Mandatory=$true,
            ValueFromPipeline=$true,
            ParameterSetName="FromModule")]
        [string[]]$Module,

        [Parameter(Mandatory=$true,
            ParameterSetName="FromCommand")]
        [string[]]$Command,

        [Parameter(Mandatory=$true,
            ParameterSetName="FromMaml")]
        [string[]]$MamlFile,

        [Parameter(ParameterSetName="FromModule")]
        [Parameter(ParameterSetName="FromCommand")]
        [System.Management.Automation.Runspaces.PSSession]$Session,

        [Parameter(ParameterSetName="FromMaml")]
        [switch]$ConvertNotesToList,

        [Parameter(ParameterSetName="FromMaml")]
        [switch]$ConvertDoubleDashLists,

        [switch]$Force,

        [switch]$AlphabeticParamsOrder,

        [hashtable]$Metadata,

        [Parameter(ParameterSetName="FromCommand")]
        [string]$OnlineVersionUrl = '',

        [Parameter(Mandatory=$true)]
        [string]$OutputFolder,

        [switch]$NoMetadata,

        [switch]$UseFullTypeName,

        [System.Text.Encoding]$Encoding = $script:UTF8_NO_BOM,

        [Parameter(ParameterSetName="FromModule")]
        [Parameter(ParameterSetName="FromMaml")]
        [switch]$WithModulePage,

        [Parameter(ParameterSetName="FromModule")]
        [Parameter(ParameterSetName="FromMaml")]
        [string]$ModulePagePath,

        [Parameter(ParameterSetName="FromModule")]
        [Parameter(ParameterSetName="FromMaml")]
        [string]
        $Locale = "en-US",

        [Parameter(ParameterSetName="FromModule")]
        [Parameter(ParameterSetName="FromMaml")]
        [string]
        $HelpVersion = $LocalizedData.HelpVersion,

        [Parameter(ParameterSetName="FromModule")]
        [Parameter(ParameterSetName="FromMaml")]
        [string]
        $FwLink = $LocalizedData.FwLink,

        [Parameter(ParameterSetName="FromMaml")]
        [string]
        $ModuleName = "MamlModule",

        [Parameter(ParameterSetName="FromMaml")]
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
                [Parameter(Mandatory=$true)]
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
                [Parameter(ValueFromPipeline=$true)]
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
                        # Get-Command requires that script input be a path
                        if ($mamlObject.Name.EndsWith(".ps1"))
                        {
                            $getCommandName = Resolve-Path $Command
                        }
                        # For cmdlets, nothing needs to be done
                        else
                        {
                            $getCommandName = $commandName
                        }

                        $a = @{
                            Name = $getCommandName
                        }
                        if ($module) {
                            # for module case, scope it just to this module
                            $a['Module'] = $module
                        }

                        $helpFileName = GetHelpFileName (Get-Command @a)
                    }

                    Write-Verbose "Maml things module is: $($mamlObject.ModuleName)"

                    $newMetadata = ($Metadata + @{
                        $script:EXTERNAL_HELP_FILE_YAML_HEADER = $helpFileName
                        $script:ONLINE_VERSION_YAML_HEADER = $online
                        $script:MODULE_PAGE_MODULE_NAME = $mamlObject.ModuleName
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

                    $CmdletNames += GetMamlObject -MamlFile $_ -ExcludeDontShow:$ExcludeDontShow.IsPresent | ForEach-Object {$_.Name}
                }

                if($WithModulePage)
                {
                    if(-not $ModuleGuid)
                    {
                        $ModuleGuid = "00000000-0000-0000-0000-000000000000"
                    }
                    if($ModuleGuid.Count -gt 1)
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


function Get-MarkdownMetadata
{
    [CmdletBinding(DefaultParameterSetName="FromPath")]

    param(
        [Parameter(Mandatory=$true,
            ValueFromPipeline=$true,
            ValueFromPipelineByPropertyName=$true,
            Position=1,
            ParameterSetName="FromPath")]
        [SupportsWildcards()]
        [string[]]$Path,

        [Parameter(Mandatory=$true,
            ParameterSetName="FromMarkdownString")]
        [string]$Markdown
    )

    process
    {
        if ($PSCmdlet.ParameterSetName -eq 'FromMarkdownString')
        {
            return [Markdown.MAML.Parser.MarkdownParser]::GetYamlMetadata($Markdown)
        }
        else # FromFile)
        {
            GetMarkdownFilesFromPath $Path -IncludeModulePage | ForEach-Object {
                $md = Get-Content -Raw $_.FullName
                [Markdown.MAML.Parser.MarkdownParser]::GetYamlMetadata($md) # yeild
            }
        }
    }
}

function Update-MarkdownHelp
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

function Merge-MarkdownHelp
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

function Update-MarkdownHelpModule
{
    [CmdletBinding()]
    [OutputType([System.IO.FileInfo[]])]
    param(
        [Parameter(Mandatory=$true,
            ValueFromPipeline=$true)]
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
            $affectedFiles = Update-MarkdownHelp -Session $Session -Path $modulePath -LogPath $LogPath -LogAppend -Encoding $Encoding -AlphabeticParamsOrder:$AlphabeticParamsOrder -UseFullTypeName:$UseFullTypeName -UpdateInputOutput:$UpdateInputOutput -Force:$Force -ExcludeDontShow:$ExcludeDontShow
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
                    $newFiles = New-MarkdownHelp -Command $_ -OutputFolder $modulePath -AlphabeticParamsOrder:$AlphabeticParamsOrder -Force:$Force -ExcludeDontShow:$ExcludeDontShow
                    $newFiles # yeild
                }
            }

            if($RefreshModulePage)
            {
                $MamlModel = New-Object System.Collections.Generic.List[Markdown.MAML.Model.MAML.MamlCommand]
                $files = @()
                $MamlModel = GetMamlModelImpl $affectedFiles -ForAnotherMarkdown -Encoding $Encoding
                NewModuleLandingPage  -RefreshModulePage -ModulePagePath $ModulePagePath -Path $modulePath -ModuleName $module -Module $MamlModel -Encoding $Encoding -Force
            }
        }
    }
}

function New-MarkdownAboutHelp
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string] $OutputFolder,
        [string] $AboutName
    )

    begin
    {
        if ($AboutName.StartsWith('about_')) { $AboutName = $AboutName.Substring('about_'.Length)}
        validateWorkingProvider
        $templatePath =  Join-Path $PSScriptRoot "templates\aboutTemplate.md"
    }

    process
    {
        if(Test-Path $OutputFolder)
        {
            $AboutContent = Get-Content $templatePath
            $AboutContent = $AboutContent.Replace("{{FileNameForHelpSystem}}",("about_" + $AboutName))
            $AboutContent = $AboutContent.Replace("{{TOPIC NAME}}",$AboutName)
            $NewAboutTopic = New-Item -Path $OutputFolder -Name "about_$($AboutName).md"
            Set-Content -Value $AboutContent -Path $NewAboutTopic -Encoding UTF8
        }
        else
        {
            throw $LocalizedData.OutputFolderNotFound
        }
    }
}

function New-YamlHelp
{
    [CmdletBinding()]
    [OutputType([System.IO.FileInfo[]])]
    param(
        [Parameter(Mandatory=$true,
            Position=1,
            ValueFromPipeline=$true,
            ValueFromPipelineByPropertyName=$true)]
        [string[]]$Path,

        [Parameter(Mandatory=$true)]
        [string]$OutputFolder,

        [System.Text.Encoding]$Encoding = [System.Text.Encoding]::UTF8,

        [switch]$Force
    )
    begin
    {
        validateWorkingProvider

        $MarkdownFiles = @()

        if(-not (Test-Path $OutputFolder))
        {
            New-Item -Type Directory $OutputFolder -ErrorAction SilentlyContinue > $null
        }

        if(-not (Test-Path -PathType Container $OutputFolder))
        {
            throw $LocalizedData.PathIsNotFolder -f $OutputFolder
        }
    }
    process
    {
        $MarkdownFiles += GetMarkdownFilesFromPath $Path
    }
    end
    {
        $MarkdownFiles | ForEach-Object {
            Write-Verbose -Message ($LocalizedData.InputMarkdownFile -f '[New-YamlHelp]', $_)
        }

        foreach($markdownFile in $MarkdownFiles)
        {
            $mamlModels = GetMamlModelImpl $markdownFile.FullName -Encoding $Encoding
            foreach($mamlModel in $mamlModels)
            {
                $markdownMetadata = Get-MarkdownMetadata -Path $MarkdownFile.FullName

                ## We set the module here in the PowerShell since the Yaml block is not read by the parser
                $mamlModel.ModuleName = $markdownMetadata[$script:MODULE_PAGE_MODULE_NAME]

                $yaml = [Markdown.MAML.Renderer.YamlRenderer]::MamlModelToString($mamlModel)
                $outputFilePath = Join-Path $OutputFolder ($mamlModel.Name + ".yml")
                Write-Verbose -Message ($LocalizedData.WritingYamlToPath -f $outputFilePath)
                MySetContent -Path $outputFilePath -Value $yaml -Encoding $Encoding -Force:$Force
            }
        }
    }
}

function New-ExternalHelp
{
    [CmdletBinding()]
    [OutputType([System.IO.FileInfo[]])]
    param(
        [Parameter(Mandatory=$true,
            Position=1,
            ValueFromPipeline=$true,
            ValueFromPipelineByPropertyName=$true)]
        [SupportsWildcards()]
        [string[]]$Path,

        [Parameter(Mandatory=$true)]
        [string]$OutputPath,

        [string[]]$ApplicableTag,

        [System.Text.Encoding]$Encoding = [System.Text.Encoding]::UTF8,

        [ValidateRange(80, [int]::MaxValue)]
        [int] $MaxAboutWidth = 80,

        [string]$ErrorLogFile,

        [switch]$Force,

        [switch]$ShowProgress
    )

    begin
    {
        validateWorkingProvider

        $MarkdownFiles = @()
        $AboutFiles = @()
        $IsOutputContainer = $true
        if ( $OutputPath.EndsWith('.xml') -and (-not (Test-Path -PathType Container $OutputPath )) )
        {
            $IsOutputContainer = $false
            Write-Verbose -Message ($LocalizedData.OutputPathAsFile -f '[New-ExternalHelp]', $OutputPath)
        }
        else
        {
            New-Item -Type Directory $OutputPath -ErrorAction SilentlyContinue > $null
            Write-Verbose -Message ($LocalizedData.OutputPathAsDirectory -f '[New-ExternalHelp]', $OutputPath)
        }

        if ( -not $ShowProgress -or $(Get-Variable -Name IsCoreClr -ValueOnly -ErrorAction SilentlyContinue) )
        {
            Function Write-Progress() {}
        }
    }

    process
    {
        $MarkdownFiles += GetMarkdownFilesFromPath $Path

        if($MarkdownFiles)
        {
            $AboutFiles += GetAboutTopicsFromPath -Path $Path -MarkDownFilesAlreadyFound $MarkdownFiles.FullName
        }
        else
        {
            $AboutFiles += GetAboutTopicsFromPath -Path $Path
        }
    }

    end
    {
       # Tracks all warnings and errors
       $warningsAndErrors = New-Object System.Collections.Generic.List[System.Object]

       try {
         # write verbose output and filter out files based on applicable tag
         $MarkdownFiles | ForEach-Object {
            Write-Verbose -Message ($LocalizedData.InputMarkdownFile -f '[New-ExternalHelp]', $_)
         }

         if ($ApplicableTag) {
            Write-Verbose -Message ($LocalizedData.FilteringForApplicableTag -f '[New-ExternalHelp]', $ApplicableTag)
            $MarkdownFiles = $MarkdownFiles | ForEach-Object {
               $applicableList = GetApplicableList -Path $_.FullName
               # this Compare-Object call is getting the intersection of two string[]
               if ((-not $applicableList) -or (Compare-Object $applicableList $ApplicableTag -IncludeEqual -ExcludeDifferent)) {
                  # yeild
                  $_
               }
               else {
                  Write-Verbose -Message ($LocalizedData.SkippingMarkdownFile -f '[New-ExternalHelp]', $_)
               }
            }
         }

         # group the files based on the output xml path metadata tag
         if ($IsOutputContainer) {
            $defaultPath = Join-Path $OutputPath $script:DEFAULT_MAML_XML_OUTPUT_NAME
            $groups = $MarkdownFiles | Group-Object {
               $h = Get-MarkdownMetadata -Path $_.FullName
               if ($h -and $h[$script:EXTERNAL_HELP_FILE_YAML_HEADER]) {
                  Join-Path $OutputPath $h[$script:EXTERNAL_HELP_FILE_YAML_HEADER]
               }
               else {
                  $msgLine1 = $LocalizedData.CannotFindInMetadataFile -f $script:EXTERNAL_HELP_FILE_YAML_HEADER, $_.FullName
                  $msgLine2 = $LocalizedData.PathWillBeUsed -f $defaultPath
                  $warningsAndErrors.Add(@{
                        Severity = "Warning"
                        Message  = "$msgLine1 $msgLine2"
                        FilePath = "$($_.FullName)"
                     })

                  Write-Warning -Message "[New-ExternalHelp] $msgLine1"
                  Write-Warning -Message "[New-ExternalHelp] $msgLine2"
                  $defaultPath
               }
            }
         }
         else {
            $groups = $MarkdownFiles | Group-Object { $OutputPath }
         }

         # generate the xml content
         $r = new-object -TypeName 'Markdown.MAML.Renderer.MamlRenderer'

         foreach ($group in $groups) {
            $maml = GetMamlModelImpl ($group.Group | ForEach-Object {$_.FullName}) -Encoding $Encoding -ApplicableTag $ApplicableTag
            $xml = $r.MamlModelToString($maml)

            $outPath = $group.Name # group name
            Write-Verbose -Message ($LocalizedData.WritingExternalHelpToPath -f $outPath)
            MySetContent -Path $outPath -Value $xml -Encoding $Encoding -Force:$Force
         }

         # handle about topics
         if ($AboutFiles.Count -gt 0) {
            foreach ($About in $AboutFiles) {
               $r = New-Object -TypeName 'Markdown.MAML.Renderer.TextRenderer' -ArgumentList($MaxAboutWidth)
               $Content = Get-Content -Raw $About.FullName
               $p = NewMarkdownParser
               $model = $p.ParseString($Content)
               $value = $r.AboutMarkDownToString($model)

               $outPath = Join-Path $OutputPath ([io.path]::GetFileNameWithoutExtension($About.FullName) + ".help.txt")
               if (!(Split-Path -Leaf $outPath).ToUpper().StartsWith("ABOUT_", $true, $null)) {
                  $outPath = Join-Path (Split-Path -Parent $outPath) ("about_" + (Split-Path -Leaf $outPath))
               }
               MySetContent -Path $outPath -Value $value -Encoding $Encoding -Force:$Force
            }
         }
       }
       catch {
          # Log error and rethrow
          $warningsAndErrors.Add(@{
               Severity = "Error"
               Message  = "$_.Exception.Message"
               FilePath = ""
            })

         throw
       }
       finally {
         if ($ErrorLogFile) {
            ConvertTo-Json $warningsAndErrors | Out-File $ErrorLogFile
         }
       }
    }
}

function Get-HelpPreview
{
    [CmdletBinding()]
    [OutputType('MamlCommandHelpInfo')]
    param(
        [Parameter(Mandatory=$true,
            ValueFromPipeline=$true,
            Position=1)]
        [SupportsWildcards()]
        [string[]]$Path,

        [switch]$ConvertNotesToList,
        [switch]$ConvertDoubleDashLists
    )

    process
    {
        foreach ($MamlFilePath in $Path)
        {
            if (-not (Test-path -Type Leaf $MamlFilePath))
            {
                Write-Error -Message ($LocalizedData.FileNotFoundSkipping -f $MamlFilePath)
                continue
            }

            # this is Resolve-Path that resolves mounted drives (i.e. good for tests)
            $MamlFilePath = (Get-ChildItem $MamlFilePath).FullName

            # Read the malm file
            $xml = [xml](Get-Content $MamlFilePath -Raw -ea SilentlyContinue)
            if (-not $xml)
            {
                # already error-out on the convertion, no need to repeat ourselves
                continue
            }

            # we need a copy of maml file to bypass powershell cache,
            # in case we reuse the same filename few times.
            $MamlCopyPath = [System.IO.Path]::GetTempFileName()
            try
            {
                if ($ConvertDoubleDashLists)
                {
                    $p = $xml.GetElementsByTagName('maml:para') | ForEach-Object {
                        # Convert "-- "-lists into "- "-lists
                        # to make them markdown compatible
                        # as described in https://github.com/PowerShell/platyPS/issues/117
                        $newInnerXml = $_.get_InnerXml() -replace "(`n|^)-- ", '$1- '
                        $_.set_InnerXml($newInnerXml)
                    }
                }

                if ($ConvertNotesToList)
                {
                    # Add inline bullet-list, as described in https://github.com/PowerShell/platyPS/issues/125
                    $xml.helpItems.command.alertSet.alert |
                        ForEach-Object {
                            # make first <para> a list item
                            # add indentations to other <para> to make them continuation of list item
                            $_.ChildNodes | Select-Object -First 1 |
                            ForEach-Object {
                                $newInnerXml = '* ' + $_.get_InnerXml()
                                $_.set_InnerXml($newInnerXml)
                            }

                            $_.ChildNodes | Select-Object -Skip 1 |
                            ForEach-Object {
                                # this character is not a valid space.
                                # We have to use some odd character here, becasue help engine strips out
                                # all legetimate whitespaces.
                                # Note: powershell doesn't render it properly, it will appear as a non-writable char.
                                $newInnerXml = ([string][char]0xc2a0) * 2 + $_.get_InnerXml()
                                $_.set_InnerXml($newInnerXml)
                            }
                        }
                }

                # in PS v5 help engine is not happy, when first non-empty link (== Online version link) is not a valid URI
                # User encounter this problem too oftern to ignore it, hence this workaround in platyPS:
                # always add a dummy link with a valid URI into xml and then remove the first link from the help object.
                # for more context see https://github.com/PowerShell/platyPS/issues/144
                $xml.helpItems.command.relatedLinks | ForEach-Object {
                    if ($_)
                    {
                        $_.InnerXml = '<maml:navigationLink xmlns:maml="http://schemas.microsoft.com/maml/2004/10"><maml:linkText>PLATYPS_DUMMY_LINK</maml:linkText><maml:uri>https://github.com/PowerShell/platyPS/issues/144</maml:uri></maml:navigationLink>' + $_.InnerXml
                    }
                }

                $xml.Save($MamlCopyPath)

                foreach ($command in $xml.helpItems.command.details.name)
                {
                    #PlatyPS will have trouble parsing a command with space around the name.
                    $command = $command.Trim()
                    $thisDefinition = @"

<#
.ExternalHelp $MamlCopyPath
#>
filter $command
{
    [CmdletBinding()]
    Param
    (
        [Parameter(Mandatory=`$true)]
        [switch]`$platyPSHijack
    )

    Microsoft.PowerShell.Utility\Write-Warning 'PlatyPS hijacked your command $command.'
    Microsoft.PowerShell.Utility\Write-Warning 'We are sorry for that. It means, there is a bug in our Get-HelpPreview logic.'
    Microsoft.PowerShell.Utility\Write-Warning 'Please report this issue https://github.com/PowerShell/platyPS/issues'
    Microsoft.PowerShell.Utility\Write-Warning 'Restart PowerShell to fix the problem.'
}

# filter is rare enough to distinguish with other commands
`$innerHelp = Microsoft.PowerShell.Core\Get-Help $command -Full -Category filter

Microsoft.PowerShell.Core\Export-ModuleMember -Function @()
"@
                    $m = New-Module ( [scriptblock]::Create( "$thisDefinition" ))
                    $help = & $m { $innerHelp }
                    # this is the second part of the workaround for https://github.com/PowerShell/platyPS/issues/144
                    # see comments above for context
                    $help.relatedLinks | ForEach-Object {
                        if ($_)
                        {
                            $_.navigationLink = $_.navigationLink | Select-Object -Skip 1
                        }
                    }
                    $help # yeild
                }
            }
            finally
            {
                Remove-Item $MamlCopyPath
            }
        }
    }
}


function New-ExternalHelpCab
{
    [Cmdletbinding()]
    param(
        [parameter(Mandatory=$true)]
        [ValidateScript(
            {
                if(Test-Path $_ -PathType Container)
                {
                    $True
                }
                else
                {
                    Throw $LocalizedData.PathIsNotFolder -f $_
                }
            })]
        [string] $CabFilesFolder,
        [parameter(Mandatory=$true)]
        [ValidateScript(
            {
                if(Test-Path $_ -PathType Leaf)
                {
                    $True
                }
                else
                {
                    Throw $LocalizedData.PathIsNotFile -f $_
                }
            })]
        [string] $LandingPagePath,
        [parameter(Mandatory=$true)]
        [string] $OutputFolder,

        [parameter()]
        [switch] $IncrementHelpVersion
    )
    begin
    {
        validateWorkingProvider
        New-Item -Type Directory $OutputFolder -ErrorAction SilentlyContinue > $null
    }
    process
    {
        #Testing for MakeCab.exe
        Write-Verbose -Message ($LocalizedData.TestCommandExists -f 'MakeCab.exe')
        $MakeCab = Get-Command MakeCab
        if(-not $MakeCab)
        {
            throw $LocalizedData.CommandNotFound -f 'MakeCab.exe'
        }
        #Testing for files in source directory
        if((Get-ChildItem -Path $CabFilesFolder).Count -le 0)
        {
            throw $LocalizedData.FilesNotFoundInFolder -f $CabFilesFolder
        }
        #Testing for valid help file types
        $ValidHelpFileTypes = '.xml', '.txt'
        $HelpFiles = Get-ChildItem -Path $CabFilesFolder -File
        $ValidHelpFiles = $HelpFiles | Where-Object { $_.Extension -in $ValidHelpFileTypes }
        $InvalidHelpFiles = $HelpFiles | Where-Object { $_.Extension -notin $ValidHelpFileTypes }
        if(-not $ValidHelpFiles)
        {
            throw $LocalizedData.NoValidHelpFiles
        }
        if($InvalidHelpFiles)
        {
            $InvalidHelpFiles | ForEach-Object { Write-Warning -Message ($LocalizedData.FileNotValidHelpFileType -f $_.FullName) }
        }


    ###Get Yaml Metadata here
    $Metadata = Get-MarkdownMetadata -Path $LandingPagePath

    $ModuleName = $Metadata[$script:MODULE_PAGE_MODULE_NAME]
    $Guid = $Metadata[$script:MODULE_PAGE_GUID]
    $Locale = $Metadata[$script:MODULE_PAGE_LOCALE]
    $FwLink = $Metadata[$script:MODULE_PAGE_FW_LINK]
    $OldHelpVersion = $Metadata[$script:MODULE_PAGE_HELP_VERSION]
    $AdditionalLocale = $Metadata[$script:MODULE_PAGE_ADDITIONAL_LOCALE]

    if($IncrementHelpVersion)
    {
        #IncrementHelpVersion
        $HelpVersion = IncrementHelpVersion -HelpVersionString $OldHelpVersion
        $MdContent = Get-Content -raw $LandingPagePath
        $MdContent = $MdContent.Replace($OldHelpVersion,$HelpVersion)
        Set-Content -path $LandingPagePath -value $MdContent
    }
    else
    {
        $HelpVersion = $OldHelpVersion
    }

    #Create HelpInfo File

        #Testing the destination directories, creating if none exists.
        if(-not (Test-Path $OutputFolder))
        {
            Write-Verbose -Message ($LocalizedData.FolderNotFoundCreating -f $OutputFolder)
            New-Item -ItemType Directory -Path $OutputFolder | Out-Null
        }

        Write-Verbose -Message ($LocalizedData.CabFileInfo -f $ModuleName, $Guid, $Locale)

        #Building the cabinet file name.
        $cabName = ("{0}_{1}_{2}_HelpContent.cab" -f $ModuleName,$Guid,$Locale)
        $zipName = ("{0}_{1}_{2}_HelpContent.zip" -f $ModuleName,$Guid,$Locale)
        $zipPath = (Join-Path $OutputFolder $zipName)

        #Setting Cab Directives, make a cab is turned on, compression is turned on
        Write-Verbose -Message ($LocalizedData.CreatingCabFileDirectives)
        $DirectiveFile = "dir.dff"
        New-Item -ItemType File -Name $DirectiveFile -Force | Out-Null
        Add-Content $DirectiveFile ".Set Cabinet=on"
        Add-Content $DirectiveFile ".Set Compress=on"
        Add-Content $DirectiveFile ".Set MaxDiskSize=CDROM"

        #Creates an entry in the cab directive file for each file in the source directory (uses FullName to get fuly qualified file path and name)
        foreach($file in $ValidHelpFiles)
        {
            Add-Content $DirectiveFile ("'" + ($file).FullName +"'" )
            Compress-Archive -DestinationPath $zipPath -Path $file.FullName -Update
        }

        #Making Cab
        Write-Verbose -Message ($LocalizedData.CreatingCabFile)
        MakeCab.exe /f $DirectiveFile | Out-Null

        #Naming CabFile
        Write-Verbose -Message ($LocalizedData.MovingCabFile -f $OutputFolder)
        Copy-Item "disk1/1.cab" (Join-Path $OutputFolder $cabName)

        #Remove ExtraFiles created by the cabbing process
        Write-Verbose -Message ($LocalizedData.RemovingExtraCabFileContents)
        Remove-Item "setup.inf" -ErrorAction SilentlyContinue
        Remove-Item "setup.rpt" -ErrorAction SilentlyContinue
        Remove-Item $DirectiveFile -ErrorAction SilentlyContinue
        Remove-Item -Path "disk1" -Recurse -ErrorAction SilentlyContinue

        #Create the HelpInfo Xml
        MakeHelpInfoXml -ModuleName $ModuleName -GUID $Guid -HelpCulture $Locale -HelpVersion $HelpVersion -URI $FwLink -OutputFolder $OutputFolder

        if($AdditionalLocale)
        {
            $allLocales = $AdditionalLocale -split ','

            foreach($loc in $allLocales)
            {
                #Create the HelpInfo Xml for each locale
                $locVersion = $Metadata["$loc Version"]

                if([String]::IsNullOrEmpty($locVersion))
                {
                    Write-Warning -Message ($LocalizedData.VersionNotFoundForLocale -f $loc)
                }
                else
                {
                    MakeHelpInfoXml -ModuleName $ModuleName -GUID $Guid -HelpCulture $loc -HelpVersion $locVersion -URI $FwLink -OutputFolder $OutputFolder
                }
            }
        }
    }
}

#endregion

#region Implementation
# IIIIIIIIII                                            lllllll                                                                                            tttt                                    tttt            iiii
# I::::::::I                                            l:::::l                                                                                         ttt:::t                                 ttt:::t           i::::i
# I::::::::I                                            l:::::l                                                                                         t:::::t                                 t:::::t            iiii
# II::::::II                                            l:::::l                                                                                         t:::::t                                 t:::::t
#   I::::I     mmmmmmm    mmmmmmm   ppppp   ppppppppp    l::::l     eeeeeeeeeeee       mmmmmmm    mmmmmmm       eeeeeeeeeeee    nnnn  nnnnnnnn    ttttttt:::::ttttttt      aaaaaaaaaaaaa  ttttttt:::::ttttttt    iiiiiii    ooooooooooo   nnnn  nnnnnnnn
#   I::::I   mm:::::::m  m:::::::mm p::::ppp:::::::::p   l::::l   ee::::::::::::ee   mm:::::::m  m:::::::mm   ee::::::::::::ee  n:::nn::::::::nn  t:::::::::::::::::t      a::::::::::::a t:::::::::::::::::t    i:::::i  oo:::::::::::oo n:::nn::::::::nn
#   I::::I  m::::::::::mm::::::::::mp:::::::::::::::::p  l::::l  e::::::eeeee:::::eem::::::::::mm::::::::::m e::::::eeeee:::::een::::::::::::::nn t:::::::::::::::::t      aaaaaaaaa:::::at:::::::::::::::::t     i::::i o:::::::::::::::on::::::::::::::nn
#   I::::I  m::::::::::::::::::::::mpp::::::ppppp::::::p l::::l e::::::e     e:::::em::::::::::::::::::::::me::::::e     e:::::enn:::::::::::::::ntttttt:::::::tttttt               a::::atttttt:::::::tttttt     i::::i o:::::ooooo:::::onn:::::::::::::::n
#   I::::I  m:::::mmm::::::mmm:::::m p:::::p     p:::::p l::::l e:::::::eeeee::::::em:::::mmm::::::mmm:::::me:::::::eeeee::::::e  n:::::nnnn:::::n      t:::::t              aaaaaaa:::::a      t:::::t           i::::i o::::o     o::::o  n:::::nnnn:::::n
#   I::::I  m::::m   m::::m   m::::m p:::::p     p:::::p l::::l e:::::::::::::::::e m::::m   m::::m   m::::me:::::::::::::::::e   n::::n    n::::n      t:::::t            aa::::::::::::a      t:::::t           i::::i o::::o     o::::o  n::::n    n::::n
#   I::::I  m::::m   m::::m   m::::m p:::::p     p:::::p l::::l e::::::eeeeeeeeeee  m::::m   m::::m   m::::me::::::eeeeeeeeeee    n::::n    n::::n      t:::::t           a::::aaaa::::::a      t:::::t           i::::i o::::o     o::::o  n::::n    n::::n
#   I::::I  m::::m   m::::m   m::::m p:::::p    p::::::p l::::l e:::::::e           m::::m   m::::m   m::::me:::::::e             n::::n    n::::n      t:::::t    tttttta::::a    a:::::a      t:::::t    tttttt i::::i o::::o     o::::o  n::::n    n::::n
# II::::::IIm::::m   m::::m   m::::m p:::::ppppp:::::::pl::::::le::::::::e          m::::m   m::::m   m::::me::::::::e            n::::n    n::::n      t::::::tttt:::::ta::::a    a:::::a      t::::::tttt:::::ti::::::io:::::ooooo:::::o  n::::n    n::::n
# I::::::::Im::::m   m::::m   m::::m p::::::::::::::::p l::::::l e::::::::eeeeeeee  m::::m   m::::m   m::::m e::::::::eeeeeeee    n::::n    n::::n      tt::::::::::::::ta:::::aaaa::::::a      tt::::::::::::::ti::::::io:::::::::::::::o  n::::n    n::::n
# I::::::::Im::::m   m::::m   m::::m p::::::::::::::pp  l::::::l  ee:::::::::::::e  m::::m   m::::m   m::::m  ee:::::::::::::e    n::::n    n::::n        tt:::::::::::tt a::::::::::aa:::a       tt:::::::::::tti::::::i oo:::::::::::oo   n::::n    n::::n
# IIIIIIIIIImmmmmm   mmmmmm   mmmmmm p::::::pppppppp    llllllll    eeeeeeeeeeeeee  mmmmmm   mmmmmm   mmmmmm    eeeeeeeeeeeeee    nnnnnn    nnnnnn          ttttttttttt    aaaaaaaaaa  aaaa         ttttttttttt  iiiiiiii   ooooooooooo     nnnnnn    nnnnnn
#                                    p:::::p
#                                    p:::::p
#                                   p:::::::p
#                                   p:::::::p
#                                   p:::::::p
#                                   ppppppppp

# parse out the list "applicable" tags from yaml header
function GetApplicableList
{
    param(
        [Parameter(Mandatory=$true)]
        $Path
    )

    $h = Get-MarkdownMetadata -Path $Path
    if ($h -and $h[$script:APPLICABLE_YAML_HEADER]) {
        return $h[$script:APPLICABLE_YAML_HEADER].Split(',').Trim()
    }
}

function SortParamsAlphabetically
{
    param(
        [Parameter(Mandatory=$true)]
        $MamlCommandObject
    )

    # sort parameters alphabetically with minor exceptions
    # https://github.com/PowerShell/platyPS/issues/142
    $confirm = $MamlCommandObject.Parameters | Where-Object { $_.Name -eq 'Confirm' }
    $whatif = $MamlCommandObject.Parameters | Where-Object { $_.Name -eq 'WhatIf' }
    $includeTotalCount = $MamlCommandObject.Parameters | Where-Object { $_.Name -eq 'IncludeTotalCount' }
    $skip = $MamlCommandObject.Parameters | Where-Object { $_.Name -eq 'Skip' }
    $first = $MamlCommandObject.Parameters | Where-Object { $_.Name -eq 'First' }

    if ($confirm)
    {
        $MamlCommandObject.Parameters.Remove($confirm) > $null
    }

    if ($whatif)
    {
        $MamlCommandObject.Parameters.Remove($whatif) > $null
    }

    if ($includeTotalCount)
    {
        $MamlCommandObject.Parameters.Remove($includeTotalCount) > $null
    }

    if ($skip)
    {
        $MamlCommandObject.Parameters.Remove($skip) > $null
    }

    if ($first)
    {
        $MamlCommandObject.Parameters.Remove($first) > $null
    }

    $sortedParams = $MamlCommandObject.Parameters | Sort-Object -Property Name
    $MamlCommandObject.Parameters.Clear()

    $sortedParams | ForEach-Object {
        $MamlCommandObject.Parameters.Add($_)
    }

    if ($confirm)
    {
        $MamlCommandObject.Parameters.Add($confirm)
    }

    if ($whatif)
    {
        $MamlCommandObject.Parameters.Add($whatif)
    }

    if ($includeTotalCount)
    {
        $MamlCommandObject.Parameters.Add($includeTotalCount)
    }

    if ($skip)
    {
        $MamlCommandObject.Parameters.Add($skip)
    }

    if ($first)
    {
        $MamlCommandObject.Parameters.Add($first)
    }
}

# If LogPath not provided, use -Verbose output for logs
function GetInfoCallback
{
    param(
        [string]$LogPath,
        [switch]$Append
    )

    if ($LogPath)
    {
        if (-not (Test-Path $LogPath -PathType Leaf))
        {
            $containerFolder = Split-Path $LogPath
            if ($containerFolder)
            {
                # this if is for $LogPath -eq foo.log  case
                New-Item -Type Directory $containerFolder -ErrorAction SilentlyContinue > $null
            }

            if (-not $Append)
            {
                # wipe the file, so it can be reused
                Set-Content -Path $LogPath -value '' -Encoding UTF8
            }
        }

        $infoCallback = {
            param([string]$message)
            Add-Content -Path $LogPath -value $message -Encoding UTF8
        }
    }
    else
    {
        $infoCallback = {
            param([string]$message)
            Write-Verbose $message
        }
    }
    return $infoCallback
}

function GetWarningCallback
{
    $warningCallback = {
        param([string]$message)
        Write-Warning $message
    }

    return $warningCallback
}

function GetAboutTopicsFromPath
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string[]]$Path,
        [string[]]$MarkDownFilesAlreadyFound
    )

    function ConfirmAboutBySecondHeaderText
    {
        param(
            [string]$AboutFilePath
        )

        $MdContent = Get-Content -raw $AboutFilePath
        $MdParser = new-object -TypeName 'Markdown.MAML.Parser.MarkdownParser' `
                                -ArgumentList { param([int]$current, [int]$all)
                                Write-Progress -Activity $LocalizedData.ParsingMarkdown -status $LocalizedData.Progress -percentcomplete ($current/$all*100)}
        $MdObject = $MdParser.ParseString($MdContent)

        if($MdObject.Children[1].text.length -gt 5)
        {
            if($MdObject.Children[1].text.substring(0,5).ToUpper() -eq "ABOUT")
            {
                return $true
            }
        }

        return $false
    }

    $AboutMarkDownFiles = @()

    if ($Path) {
        $Path | ForEach-Object {
            if (Test-Path -PathType Leaf $_)
            {
                if(ConfirmAboutBySecondHeaderText($_))
                {
                    $AboutMarkdownFiles += Get-ChildItem $_
                }
            }
            elseif (Test-Path -PathType Container $_)
            {
                if($MarkDownFilesAlreadyFound)
                {
                    $AboutMarkdownFiles += Get-ChildItem $_ -Filter '*.md' | Where-Object {($_.FullName -notin $MarkDownFilesAlreadyFound) -and (ConfirmAboutBySecondHeaderText($_.FullName))}
                }
                else
                {
                    $AboutMarkdownFiles += Get-ChildItem $_ -Filter '*.md' | Where-Object {ConfirmAboutBySecondHeaderText($_.FullName)}
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

function GetMarkdownFilesFromPath
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

function GetParserMode
{
    param(
        [switch]$PreserveFormatting
    )

    if ($PreserveFormatting)
    {
        return [Markdown.MAML.Parser.ParserMode]::FormattingPreserve
    }
    else
    {
        return [Markdown.MAML.Parser.ParserMode]::Full
    }
}

function GetMamlModelImpl
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

function NewMarkdownParser
{
    $warningCallback = GetWarningCallback
    $progressCallback = {
        param([int]$current, [int]$all)
        Write-Progress -Activity $LocalizedData.ParsingMarkdown -status $LocalizedData.Progress -percentcomplete ($current/$all*100)
    }
    return new-object -TypeName 'Markdown.MAML.Parser.MarkdownParser' -ArgumentList ($progressCallback, $warningCallback)
}

function NewModelTransformer
{
    param(
        [ValidateSet('1.0.0', '2.0.0')]
        [string]$schema,
        [string[]]$ApplicableTag
    )

    if ($schema -eq '1.0.0')
    {
        throw $LocalizedData.PlatyPS100SchemaDeprecated
    }
    elseif ($schema -eq '2.0.0')
    {
        $infoCallback = {
            param([string]$message)
            Write-Verbose $message
        }
        $warningCallback = GetWarningCallback
        return new-object -TypeName 'Markdown.MAML.Transformer.ModelTransformerVersion2' -ArgumentList ($infoCallback, $warningCallback, $ApplicableTag)
    }
}

function GetSchemaVersion
{
    param(
        [string]$markdown
    )

    $metadata = Get-MarkdownMetadata -markdown $markdown
    if ($metadata)
    {
        $schema = $metadata[$script:SCHEMA_VERSION_YAML_HEADER]
    }

    if (-not $schema)
    {
        # either there is no metadata, or schema version is not specified.
        # assume 2.0.0
        $schema = '2.0.0'
    }

    return $schema
}

function GetOnlineVersion
{
    param(
        [string]$markdown
    )

    $metadata = Get-MarkdownMetadata -markdown $markdown
    $onlineVersionUrl = $null
    if ($metadata)
    {
        $onlineVersionUrl = $metadata[$script:ONLINE_VERSION_YAML_HEADER]
    }

    return $onlineVersionUrl
}

function SetOnlineVersionUrlLink
{
    param(
        [Parameter(Mandatory=$true)]
        [Markdown.MAML.Model.MAML.MamlCommand]$MamlCommandObject,

        [string]$OnlineVersionUrl = $null
    )

    # Online Version URL
    $currentFirstLink = $MamlCommandObject.Links | Select-Object -First 1

    if ($OnlineVersionUrl -and ((-not $currentFirstLink) -or ($currentFirstLink.LinkUri -ne $OnlineVersionUrl))) {
        $mamlLink = New-Object -TypeName Markdown.MAML.Model.MAML.MamlLink
        $mamlLink.LinkName = $script:MAML_ONLINE_LINK_DEFAULT_MONIKER
        $mamlLink.LinkUri = $OnlineVersionUrl

        # Insert link at the beginning
        $MamlCommandObject.Links.Insert(0, $mamlLink)
    }
}

function MakeHelpInfoXml
{
    Param(
        [Parameter(mandatory=$true)]
        [string]
        $ModuleName,
        [Parameter(mandatory=$true)]
        [string]
        $GUID,
        [Parameter(mandatory=$true)]
        [string]
        $HelpCulture,
        [Parameter(mandatory=$true)]
        [string]
        $HelpVersion,
        [Parameter(mandatory=$true)]
        [string]
        $URI,
        [Parameter(mandatory=$true)]
        [string]
        $OutputFolder


    )

    $HelpInfoFileNme = $ModuleName + "_" + $GUID + "_HelpInfo.xml"
    $OutputFullPath = Join-Path $OutputFolder $HelpInfoFileNme

    if(Test-Path $OutputFullPath -PathType Leaf)
    {
        [xml] $HelpInfoContent = Get-Content $OutputFullPath
    }

    #Create the base XML object for the Helpinfo.xml file.
    $xml = new-object xml

    $ns = "http://schemas.microsoft.com/powershell/help/2010/05"
    $declaration = $xml.CreateXmlDeclaration("1.0","utf-8",$null)

    $rootNode = $xml.CreateElement("HelpInfo",$ns)
    $xml.InsertBefore($declaration,$xml.DocumentElement)
    $xml.AppendChild($rootNode)

    $HelpContentUriNode = $xml.CreateElement("HelpContentURI",$ns)
    $HelpContentUriNode.InnerText = $URI
    $xml["HelpInfo"].AppendChild($HelpContentUriNode)

    $HelpSupportedCulturesNode = $xml.CreateElement("SupportedUICultures",$ns)
    $xml["HelpInfo"].AppendChild($HelpSupportedCulturesNode)


    #If no previous help file
    if(-not $HelpInfoContent)
    {
        $HelpUICultureNode = $xml.CreateElement("UICulture",$ns)
        $xml["HelpInfo"]["SupportedUICultures"].AppendChild($HelpUICultureNode)

        $HelpUICultureNameNode = $xml.CreateElement("UICultureName",$ns)
        $HelpUICultureNameNode.InnerText = $HelpCulture
        $xml["HelpInfo"]["SupportedUICultures"]["UICulture"].AppendChild($HelpUICultureNameNode)

        $HelpUICultureVersionNode = $xml.CreateElement("UICultureVersion",$ns)
        $HelpUICultureVersionNode.InnerText = $HelpVersion
        $xml["HelpInfo"]["SupportedUICultures"]["UICulture"].AppendChild($HelpUICultureVersionNode)

        [xml] $HelpInfoContent = $xml

    }
    else
    {
        #Get old culture info
        $ExistingCultures = @{}
        foreach($Culture in $HelpInfoContent.HelpInfo.SupportedUICultures.UICulture)
        {
            $ExistingCultures.Add($Culture.UICultureName, $Culture.UICultureVersion)
        }

        #If culture exists update version, if not, add culture and version
        if(-not ($HelpCulture -in $ExistingCultures.Keys))
        {
            $ExistingCultures.Add($HelpCulture,$HelpVersion)
        }
        else
        {
            $ExistingCultures[$HelpCulture] = $HelpVersion
        }

        $cultureNames = @()
        $cultureNames += $ExistingCultures.GetEnumerator()

        #write out cultures to XML
        for($i=0;$i -lt $ExistingCultures.Count; $i++)
        {
            $HelpUICultureNode = $xml.CreateElement("UICulture",$ns)


            $HelpUICultureNameNode = $xml.CreateElement("UICultureName",$ns)
            $HelpUICultureNameNode.InnerText = $cultureNames[$i].Name
            $HelpUICultureNode.AppendChild($HelpUICultureNameNode)

            $HelpUICultureVersionNode = $xml.CreateElement("UICultureVersion",$ns)
            $HelpUICultureVersionNode.InnerText = $cultureNames[$i].Value
            $HelpUICultureNode.AppendChild($HelpUICultureVersionNode)

            $xml["HelpInfo"]["SupportedUICultures"].AppendChild($HelpUICultureNode)
        }

        [xml] $HelpInfoContent = $xml
    }

    #Commit Help
        if(!(Test-Path $OutputFullPath))
    {
        New-Item -Path $OutputFolder -ItemType File -Name $HelpInfoFileNme

    }

    $HelpInfoContent.Save((Get-ChildItem $OutputFullPath).FullName)

}


function GetHelpFileName
{
    param(
        [System.Management.Automation.CommandInfo]$CommandInfo
    )

    if ($CommandInfo)
    {
        if ($CommandInfo.HelpFile)
        {
            if ([System.IO.Path]::IsPathRooted($CommandInfo.HelpFile))
            {
                return (Split-Path -Leaf $CommandInfo.HelpFile)
            }
            else
            {
                return $CommandInfo.HelpFile
            }
        }
        # only run module evaluations if the input command isn't a script
        if ($CommandInfo.CommandType -ne "ExternalScript")
        {
            # overwise, lets guess it
            $module = @($CommandInfo.Module) + ($CommandInfo.Module.NestedModules) |
                Where-Object {$_.ModuleType -ne 'Manifest'} |
                Where-Object {$_.ExportedCommands.Keys -contains $CommandInfo.Name}

            if (-not $module)
            {
                Write-Warning -Message ($LocalizedData.ModuleNotFoundFromCommand -f '[GetHelpFileName]', $CommandInfo.Name)
                return
            }

            if ($module.Count -gt 1)
            {
                Write-Warning -Message ($LocalizedData.MultipleModulesFoundFromCommand -f '[GetHelpFileName]', $CommandInfo.Name)
                $module = $module | Select-Object -First 1
            }

            if (Test-Path $module.Path -Type Leaf)
            {
                # for regular modules, we can deduct the filename from the module path file
                $moduleItem = Get-Item -Path $module.Path
                if ($moduleItem.Extension -eq '.psm1') {
                    $fileName = $moduleItem.BaseName
                } else {
                    $fileName = $moduleItem.Name
                }
            }
            else
            {
                # if it's something like Dynamic module,
                # we  guess the desired help file name based on the module name
                $fileName = $module.Name
            }
        }

        return "$fileName-help.xml"
    }
}

function MySetContent
{
    [OutputType([System.IO.FileInfo])]
    param(
        [Parameter(Mandatory=$true)]
        [string]$Path,
        [Parameter(Mandatory=$true)]
        [string]$value,
        [Parameter(Mandatory=$true)]
        [System.Text.Encoding]$Encoding,
        [switch]$Force
    )

    if (Test-Path $Path)
    {
        if (Test-Path $Path -PathType Container)
        {
            Write-Error -Message ($LocalizedData.CannotWriteFileDirectoryExists -f $Path)
            return
        }

        if ((MyGetContent -Path $Path -Encoding $Encoding) -eq $value)
        {
            Write-Verbose "Not writing to $Path, because content is not changing."
            return (Get-ChildItem $Path)
        }

        if (-not $Force)
        {
            Write-Error -Message ($LocalizedData.CannotWriteFileWithoutForce -f $Path)
            return
        }
    }
    else
    {
        $dir = Split-Path $Path
        if ($dir)
        {
            New-Item -Type Directory $dir -ErrorAction SilentlyContinue > $null
        }
    }

    Write-Verbose -Message ($LocalizedData.WritingWithEncoding -f $Path, $Encoding.EncodingName)
    # just to create a file
    Set-Content -Path $Path -Value ''
    $resolvedPath = (Get-ChildItem $Path).FullName
    [System.IO.File]::WriteAllText($resolvedPath, $value, $Encoding)
    return (Get-ChildItem $Path)
}

function MyGetContent
{
    [OutputType([System.String])]
    param(
        [Parameter(Mandatory=$true)]
        [string]$Path,
        [Parameter(Mandatory=$true)]
        [System.Text.Encoding]$Encoding
    )

    if (-not(Test-Path $Path))
    {
        throw $LocalizedData.FileNotFound
        return
    }
    else
    {
        if (Test-Path $Path -PathType Container)
        {
            throw $LocalizedData.PathIsNotFile
            return
        }
    }

    Write-Verbose -Message ($LocalizedData.ReadingWithEncoding -f $Path, $Encoding.EncodingName)
    $resolvedPath = (Get-ChildItem $Path).FullName
    return [System.IO.File]::ReadAllText($resolvedPath, $Encoding)
}

function NewModuleLandingPage
{
    Param(
        [Parameter(mandatory=$true)]
        [string]
        $Path,
        [Parameter(mandatory=$true)]
        [string]
        $ModuleName,
        [Parameter(mandatory=$true,ParameterSetName="NewLandingPage")]
        [string]
        $ModuleGuid,
        [Parameter(mandatory=$true,ParameterSetName="NewLandingPage")]
        [string[]]
        $CmdletNames,
        [Parameter(mandatory=$true,ParameterSetName="NewLandingPage")]
        [string]
        $Locale,
        [Parameter(mandatory=$true,ParameterSetName="NewLandingPage")]
        [string]
        $Version,
        [Parameter(mandatory=$true,ParameterSetName="NewLandingPage")]
        [string]
        $FwLink,
        [Parameter(ParameterSetName="UpdateLandingPage")]
        [switch]
        $RefreshModulePage,
        [string]$ModulePagePath,
        [Parameter(mandatory=$true,ParameterSetName="UpdateLandingPage")]
        [System.Collections.Generic.List[Markdown.MAML.Model.MAML.MamlCommand]]
        $Module,
        [Parameter(mandatory=$true)]
        [System.Text.Encoding]$Encoding = $script:UTF8_NO_BOM,
        [switch]$Force
    )

    begin
    {
        if ($ModulePagePath) {
            $LandingPagePath = $ModulePagePath
        } else {
            $LandingPageName = $ModuleName + ".md"
            $LandingPagePath = Join-Path $Path $LandingPageName
        }
    }

    process
    {
        $Description = $LocalizedData.Description

        if($RefreshModulePage)
        {
            if(Test-Path $LandingPagePath)
            {
                $OldLandingPageContent = Get-Content -Raw $LandingPagePath
                $OldMetaData = Get-MarkdownMetadata -Markdown $OldLandingPageContent
                $ModuleGuid = $OldMetaData["Module Guid"]
                $FwLink = $OldMetaData["Download Help Link"]
                $Version = $OldMetaData["Help Version"]
                $Locale = $OldMetaData["Locale"]

                $p = NewMarkdownParser
                $model = $p.ParseString($OldLandingPageContent)
                $index = $model.Children.IndexOf(($model.Children | Where-Object {$_.Text -eq "Description"}))
                $i = 1
                $stillParagraph = $true
                $Description = ""
                while($stillParagraph -eq $true)
                {
                    $Description += $model.Children[$index + $i].spans.text
                    $i++

                    if($model.Children[$i].NodeType -eq "Heading")
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

        if($RefreshModulePage)
        {
            $Module | ForEach-Object {
                $command = $_
                if(-not $command.Synopsis)
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

function ConvertMamlModelToMarkdown
{
    param(
        [ValidateNotNullOrEmpty()]
        [Parameter(Mandatory=$true)]
        [Markdown.MAML.Model.MAML.MamlCommand]$mamlCommand,

        [hashtable]$metadata,

        [switch]$NoMetadata,

        [switch]$PreserveFormatting
    )

    begin
    {
        $parseMode = GetParserMode -PreserveFormatting:$PreserveFormatting
        $r = New-Object Markdown.MAML.Renderer.MarkdownV2Renderer -ArgumentList $parseMode
        $count = 0
    }

    process
    {
        if (($count++) -eq 0 -and (-not $NoMetadata))
        {
            return $r.MamlModelToString($mamlCommand, $metadata)
        }
        else
        {
            return $r.MamlModelToString($mamlCommand, $true) # skip version header
        }
    }
}

function GetCommands
{
    param(
        [Parameter(Mandatory=$true)]
        [string]$Module,
        # return names, instead of objects
        [switch]$AsNames,
        # use Session for remoting support
        [System.Management.Automation.Runspaces.PSSession]$Session
    )

    process {
        # Get-Module doesn't know about Microsoft.PowerShell.Core, so we don't use (Get-Module).ExportedCommands

        # We use: & (dummy module) {...} syntax to workaround
        # the case `GetMamlObject -Module platyPS`
        # because in this case, we are in the module context and Get-Command returns all commands,
        # not only exported ones.
        $commands = & (New-Module {}) ([scriptblock]::Create("Get-Command -Module '$Module'")) |
            Where-Object {$_.CommandType -ne 'Alias'}  # we don't want aliases in the markdown output for a module

        if ($AsNames)
        {
            $commands.Name
        }
        else
        {
            if ($Session) {
                $commands.Name | ForEach-Object {
                    # yeild
                    MyGetCommand -Cmdlet $_ -Session $Session
                }
            } else {
                $commands
            }
        }
    }
}

<#
    Get a compact string representation from TypeInfo or TypeInfo-like object

    The typeObjectHash api is provided for the remoting support.
    We use two different parameter sets ensure the tupe of -TypeObject
#>
function GetTypeString
{
    param(
        [Parameter(ValueFromPipeline=$true, ParameterSetName='typeObject')]
        [System.Reflection.TypeInfo]
        $TypeObject,

        [Parameter(ValueFromPipeline=$true, ParameterSetName='typeObjectHash')]
        [PsObject]
        $TypeObjectHash
    )

    if ($TypeObject) {
        $TypeObjectHash = $TypeObject
    }

    # special case for nullable value types
    if ($TypeObjectHash.Name -eq 'Nullable`1')
    {
        return $TypeObjectHash.GenericTypeArguments.Name
    }

    if ($TypeObjectHash.IsGenericType)
    {
        # keep information about generic parameters
        return $TypeObjectHash.ToString()
    }

    return $TypeObjectHash.Name
}

<#
    You cannot just write 0..($n-1) because if $n == 0 you are screwed.
    Hence this helper.
#>
function GetRange
{
    Param(
        [CmdletBinding()]
        [parameter(mandatory=$true)]
        [int]$n
    )
    if ($n -lt 0) {
        throw $LocalizedData.RangeIsLessThanZero -f $n
    }
    if ($n -eq 0) {
        return
    }
    0..($n - 1)
}

<#
    This function proxies Get-Command call.

    In case of the Remote module, we need to jump thru some hoops
    to get the actual Command object with proper fields.
    Remoting doesn't properly serialize command objects, so we need to be creative
    while extracting all the required metadata from the remote session
    See https://github.com/PowerShell/platyPS/issues/338 for historical context.
#>
function MyGetCommand
{
    Param(
        [CmdletBinding()]
        [parameter(mandatory=$true, parametersetname="Cmdlet")]
        [string] $Cmdlet,
        [System.Management.Automation.Runspaces.PSSession]$Session
    )
    # if there is no remoting, just proxy to Get-Command
    if (-not $Session) {
        return Get-Command $Cmdlet
    }

    # Here is the structure that we use in ConvertPsObjectsToMamlModel
    # we fill it up from the remote with some workarounds
    #
    # $Command.CommandType
    # $Command.Name
    # $Command.ModuleName
    # $Command.DefaultParameterSet
    # $Command.CmdletBinding
    # $ParameterSet in $Command.ParameterSets
    #     $ParameterSet.Name
    #     $ParameterSet.IsDefault
    #     $Parameter in $ParameterSet.Parameters
    #         $Parameter.Name
    #         $Parameter.IsMandatory
    #         $Parameter.Aliases
    #         $Parameter.HelpMessage
    #         $Parameter.Type
    #         $Parameter.ParameterType
    #            $Parameter.ParameterType.Name
    #            $Parameter.ParameterType.GenericTypeArguments.Name
    #            $Parameter.ParameterType.IsGenericType
    #            $Parameter.ParameterType.ToString() - we get that for free from expand

    # expand first layer of properties
    function expand([string]$property) {
        Invoke-Command -Session $Session -ScriptBlock {
            Get-Command $using:Cmdlet |
            Select-Object -ExpandProperty $using:property
        }
    }

    # expand second layer of properties on the selected item
    function expand2([string]$property1, [int]$num, [string]$property2) {
        Invoke-Command -Session $Session -ScriptBlock {
            Get-Command $using:Cmdlet |
            Select-Object -ExpandProperty $using:property1 |
            Select-Object -Index $using:num -Wait |
            Select-Object -ExpandProperty $using:property2
        }
    }

    # expand second and 3rd layer of properties on the selected item
    function expand3(
        [string]$property1,
        [int]$num,
        [string]$property2,
        [string]$property3
        ) {
        Invoke-Command -Session $Session -ScriptBlock {
            Get-Command $using:Cmdlet |
            Select-Object -ExpandProperty $using:property1 |
            Select-Object -Index $using:num -Wait |
            Select-Object -ExpandProperty $using:property2 |
            Select-Object -ExpandProperty $using:property3
        }
    }

    function local([string]$property) {
        Get-Command $Cmdlet | select-object -ExpandProperty $property
    }

    # helper function to fill up the parameters metadata
    function getParams([int]$num) {
        # this call we need to fill-up ParameterSets.Parameters.ParameterType with metadata
        $parameterType = expand3 'ParameterSets' $num 'Parameters' 'ParameterType'
        # this call we need to fill-up ParameterSets.Parameters with metadata
        $parameters = expand2 'ParameterSets' $num 'Parameters'
        if ($parameters.Length -ne $parameterType.Length) {
            Write-Error -Message ($LocalizedData.MetadataDoesNotMatchLength -f $Cmdlet)
        }

        foreach ($i in (GetRange $parameters.Length)) {
            $typeObjectHash = New-Object -TypeName pscustomobject -Property @{
                Name = $parameterType[$i].Name
                IsGenericType = $parameterType[$i].IsGenericType
                # almost .ParameterType.GenericTypeArguments.Name
                # TODO: doesn't it worth another round-trip to make it more accurate
                # and query for the Name?
                GenericTypeArguments = @{ Name = $parameterType[$i].GenericTypeArguments }
            }
            Add-Member -Type NoteProperty -InputObject $parameters[$i] -Name 'ParameterTypeName' -Value (GetTypeString -TypeObjectHash $typeObjectHash)
        }
        return $parameters
    }

    # we cannot use the nested properties from this $remote command.
    # ps remoting doesn't serialize all of them properly.
    # but we can use the top-level onces
    $remote = Invoke-Command -Session $Session { Get-Command $using:Cmdlet }

    $psets = expand 'ParameterSets'
    $psetsArray = @()
    foreach ($i in (GetRange ($psets | measure-object).Count)) {
        $parameters = getParams $i
        $psetsArray += @(New-Object -TypeName pscustomobject -Property @{
            Name = $psets[$i].Name
            IsDefault = $psets[$i].IsDefault
            Parameters = $parameters
        })
    }

    $commandHash = @{
        Name = $Cmdlet
        CommandType = $remote.CommandType
        DefaultParameterSet = $remote.DefaultParameterSet
        CmdletBinding = $remote.CmdletBinding
        # for office we cannot get the module name from the remote, grab the local one instead
        ModuleName = local 'ModuleName'
        ParameterSets = $psetsArray
    }

    return New-Object -TypeName pscustomobject -Property $commandHash
}

<#
    This function prepares help and command object (possibly do mock)
    and passes it to ConvertPsObjectsToMamlModel, then return results
#>
function GetMamlObject
{
    Param(
        [CmdletBinding()]
        [parameter(mandatory=$true, parametersetname="Cmdlet")]
        [string] $Cmdlet,
        [parameter(mandatory=$true, parametersetname="Module")]
        [string] $Module,
        [parameter(mandatory=$true, parametersetname="Maml")]
        [string] $MamlFile,
        [parameter(parametersetname="Maml")]
        [switch] $ConvertNotesToList,
        [parameter(parametersetname="Maml")]
        [switch] $ConvertDoubleDashLists,
        [switch] $UseFullTypeName,
        [parameter(parametersetname="Cmdlet")]
        [parameter(parametersetname="Module")]
        [System.Management.Automation.Runspaces.PSSession]$Session,
        [switch]$ExcludeDontShow
    )

    function CommandHasAutogeneratedSynopsis
    {
        param([object]$help)

        return (Get-Command $help.Name -Syntax) -eq ($help.Synopsis)
    }

    if($Cmdlet)
    {
        Write-Verbose -Message ($LocalizedData.Processing -f $Cmdlet)
        $Help = Get-Help $Cmdlet
        $Command = MyGetCommand -Session $Session -Cmdlet $Cmdlet
        return ConvertPsObjectsToMamlModel -Command $Command -Help $Help -UsePlaceholderForSynopsis:(CommandHasAutogeneratedSynopsis $Help) -UseFullTypeName:$UseFullTypeName -ExcludeDontShow:$ExcludeDontShow
    }
    elseif ($Module)
    {
        Write-Verbose -Message ($LocalizedData.Processing -f $Module)

        # GetCommands is slow over remoting, piping here is important for good UX
        GetCommands $Module -Session $Session | ForEach-Object {
            $Command = $_
            Write-Verbose -Message ("`t" + ($LocalizedData.Processing -f $Command.Name))
            $Help = Get-Help $Command.Name
            # yield
            ConvertPsObjectsToMamlModel -Command $Command -Help $Help -UsePlaceholderForSynopsis:(CommandHasAutogeneratedSynopsis $Help)  -UseFullTypeName:$UseFullTypeName -ExcludeDontShow:$ExcludeDontShow
        }
    }
    else # Maml
    {
        $HelpCollection = Get-HelpPreview -Path $MamlFile -ConvertNotesToList:$ConvertNotesToList -ConvertDoubleDashLists:$ConvertDoubleDashLists

        #Provides Name, CommandType, and Empty Module name from MAML generated module in the $command object.
        #Otherwise loads the results from Get-Command <Cmdlet> into the $command object

        $HelpCollection | ForEach-Object {

            $Help = $_

            $Command = [PsObject] @{
                Name = $Help.Name
                CommandType = $Help.Category
                HelpFile = (Split-Path $MamlFile -Leaf)
            }

            # yield
            ConvertPsObjectsToMamlModel -Command $Command -Help $Help -UseHelpForParametersMetadata  -UseFullTypeName:$UseFullTypeName -ExcludeDontShow:$ExcludeDontShow
        }
    }
}

function AddLineBreaksForParagraphs
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$false, ValueFromPipeline=$true)]
        [string]$text
    )

    begin
    {
        $paragraphs = @()
    }

    process
    {
        $text = $text.Trim()
        $paragraphs += $text
    }

    end
    {
        $paragraphs -join "`r`n`r`n"
    }
}

function DescriptionToPara
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$false, ValueFromPipeline=$true)]
        $description
    )

    process
    {
        # on some old maml modules description uses Tag to store *-bullet-points
        # one example of it is Exchange
        $description.Tag + "" + $description.Text
    }
}

function IncrementHelpVersion
{
    param(
        [string]
        $HelpVersionString
    )
    process
    {
        if($HelpVersionString -eq $LocalizedData.HelpVersion)
        {
            return "1.0.0.0"
        }
        $lastDigitPosition = $HelpVersionString.LastIndexOf(".") + 1
        $frontDigits = $HelpVersionString.Substring(0,$lastDigitPosition)
        $frontDigits += ([int] $HelpVersionString.Substring($lastDigitPosition)) + 1
        return $frontDigits
    }
}

<#
    This function converts help and command object (possibly mocked) into a Maml Model
#>
function ConvertPsObjectsToMamlModel
{
    [CmdletBinding()]
    [OutputType([Markdown.MAML.Model.MAML.MamlCommand])]
    param(
        [Parameter(Mandatory=$true)]
        [object]$Command,
        [Parameter(Mandatory=$true)]
        [object]$Help,
        [switch]$UseHelpForParametersMetadata,
        [switch]$UsePlaceholderForSynopsis,
        [switch]$UseFullTypeName,
        [switch]$ExcludeDontShow
    )

    function isCommonParameterName
    {
        param([string]$parameterName, [switch]$Workflow)

        if (@(
                'Verbose',
                'Debug',
                'ErrorAction',
                'WarningAction',
                'InformationAction',
                'ErrorVariable',
                'WarningVariable',
                'InformationVariable',
                'OutVariable',
                'OutBuffer',
                'PipelineVariable'
        ) -contains $parameterName) {
            return $true
        }

        if ($Workflow)
        {
            return @(
                'PSParameterCollection',
                'PSComputerName',
                'PSCredential',
                'PSConnectionRetryCount',
                'PSConnectionRetryIntervalSec',
                'PSRunningTimeoutSec',
                'PSElapsedTimeoutSec',
                'PSPersist',
                'PSAuthentication',
                'PSAuthenticationLevel',
                'PSApplicationName',
                'PSPort',
                'PSUseSSL',
                'PSConfigurationName',
                'PSConnectionURI',
                'PSAllowRedirection',
                'PSSessionOption',
                'PSCertificateThumbprint',
                'PSPrivateMetadata',
                'AsJob',
                'JobName'
            ) -contains $parameterName
        }

        return $false
    }

    function getPipelineValue($Parameter)
    {
        if ($Parameter.ValueFromPipeline)
        {
            if ($Parameter.ValueFromPipelineByPropertyName)
            {
                return 'True (ByPropertyName, ByValue)'
            }
            else
            {
                return 'True (ByValue)'
            }
        }
        else
        {
            if ($Parameter.ValueFromPipelineByPropertyName)
            {
                return 'True (ByPropertyName)'
            }
            else
            {
                return 'False'
            }
        }
    }

    function normalizeFirstLatter
    {
        param(
            [Parameter(ValueFromPipeline=$true)]
            [string]$value
        )

        if ($value -and $value.Length -gt 0)
        {
            return $value.Substring(0,1).ToUpperInvariant() + $value.substring(1)
        }

        return $value
    }

    #endregion

    $MamlCommandObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlCommand

    #region Command Object Values Processing

    $IsWorkflow = $Command.CommandType -eq 'Workflow'

    #Get Name
    $MamlCommandObject.Name = $Command.Name

    $MamlCommandObject.ModuleName = $Command.ModuleName

    #region Data not provided by the command object

    #Get Description
    #Not provided by the command object.
    $MamlCommandObject.Description = New-Object -TypeName Markdown.MAML.Model.Markdown.SectionBody ($LocalizedData.Description)

    #endregion

    #Get Syntax
    #region Get the Syntax Parameter Set objects

    function FillUpParameterFromHelp
    {
        param(
            [Parameter(Mandatory=$true)]
            [Markdown.MAML.Model.MAML.MamlParameter]$ParameterObject
        )

        $HelpEntry = $Help.parameters.parameter | Where-Object {$_.Name -eq $ParameterObject.Name}

        $ParameterObject.DefaultValue = $HelpEntry.defaultValue | normalizeFirstLatter
        $ParameterObject.VariableLength = $HelpEntry.variableLength -eq 'True'
        $ParameterObject.Position = $HelpEntry.position | normalizeFirstLatter
        if ($HelpEntry.description)
        {
            if ($HelpEntry.description.text)
            {
                $ParameterObject.Description = $HelpEntry.description |
                    DescriptionToPara |
                    AddLineBreaksForParagraphs
            }
            else
            {
                # this case happens, when there is HelpMessage in 'Parameter' attribute,
                # but there is no maml or comment-based help.
                # then help engine put string outside of 'text' property
                # In this case there is no DescriptionToPara call needed
                $ParameterObject.Description = $HelpEntry.description | AddLineBreaksForParagraphs
            }
        }

        $syntaxParam = $Help.syntax.syntaxItem.parameter |  Where-Object {$_.Name -eq $Parameter.Name} | Select-Object -First 1
        if ($syntaxParam)
        {
            # otherwise we could potentialy get it from Reflection but not doing it for now
            foreach ($parameterValue in $syntaxParam.parameterValueGroup.parameterValue)
            {
                $ParameterObject.parameterValueGroup.Add($parameterValue)
            }
        }
    }

    function FillUpSyntaxFromCommand
    {
        foreach($ParameterSet in $Command.ParameterSets)
        {
            $SyntaxObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlSyntax

            $SyntaxObject.ParameterSetName = $ParameterSet.Name
            $SyntaxObject.IsDefault = $ParameterSet.IsDefault

            foreach($Parameter in $ParameterSet.Parameters)
            {
                # ignore CommonParameters
                if (isCommonParameterName $Parameter.Name -Workflow:$IsWorkflow)
                {
                    # but don't ignore them, if they have explicit help entries
                    if ($Help.parameters.parameter | Where-Object {$_.Name -eq $Parameter.Name})
                    {
                    }
                    else
                    {
                        continue
                    }
                }

                $hasDontShow = $false
                $hasSupportsWildsCards = $false

                foreach ($Attribute in $Parameter.Attributes)
                {
                    if ($ExcludeDontShow)
                    {
                        if ($Attribute.TypeId.ToString() -eq 'System.Management.Automation.ParameterAttribute' -and $Attribute.DontShow)
                        {
                            $hasDontShow = $true
                        }
                    }

                    if ($Attribute.TypeId.ToString() -eq 'System.Management.Automation.SupportsWildcardsAttribute')
                    {
                        $hasSupportsWildsCards = $true
                    }
                }

                if ($hasDontShow)
                {
                    continue
                }

                $ParameterObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlParameter
                $ParameterObject.Name = $Parameter.Name
                $ParameterObject.Required = $Parameter.IsMandatory
                $ParameterObject.PipelineInput = getPipelineValue $Parameter
                $ParameterObject.Globbing = $hasSupportsWildsCards
                # the ParameterType could be just a string in case of remoting
                # or a TypeInfo object, in the regular case
                if ($Session) {
                    # in case of remoting we already pre-calcuated the Type string
                    $ParameterObject.Type = $Parameter.ParameterTypeName
                } else {
                    $ParameterObject.Type = GetTypeString -TypeObject $Parameter.ParameterType
                }
                # ToString() works in both cases
                $ParameterObject.FullType = $Parameter.ParameterType.ToString()

                $ParameterObject.ValueRequired = -not ($Parameter.Type -eq "SwitchParameter") # thisDefinition is a heuristic

                foreach($Alias in $Parameter.Aliases)
                {
                    $ParameterObject.Aliases += $Alias
                }

                $ParameterObject.Description = if ([String]::IsNullOrEmpty($Parameter.HelpMessage))
                {
                    # additional new-lines are needed for Update-MarkdownHelp scenario.
                    switch ($Parameter.Name)
                    {
                        # we have well-known parameters and can generate a reasonable description for them
                        # https://github.com/PowerShell/platyPS/issues/211
                        'Confirm' { $LocalizedData.Confirm + "`r`n`r`n" }
                        'WhatIf' { $LocalizedData.WhatIf + "`r`n`r`n" }
                        'IncludeTotalCount' { $LocalizedData.IncludeTotalCount + "`r`n`r`n" }
                        'Skip' { $LocalizedData.Skip + "`r`n`r`n" }
                        'First' { $LocalizedData.First + "`r`n`r`n" }
                        default { ($LocalizedData.ParameterDescription -f $Parameter.Name) + "`r`n`r`n" }
                    }
                }
                else
                {
                    $Parameter.HelpMessage
                }

                FillUpParameterFromHelp $ParameterObject

                $SyntaxObject.Parameters.Add($ParameterObject)
            }

            $MamlCommandObject.Syntax.Add($SyntaxObject)
        }
    }

    function FillUpSyntaxFromHelp
    {
        function GuessTheType
        {
            param([string]$type)

            if (-not $type)
            {
                # weired, but that's how it works
                return 'SwitchParameter'
            }

            return $type
        }

        $ParamSetCount = 0
        foreach($ParameterSet in $Help.syntax.syntaxItem)
        {
            $SyntaxObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlSyntax

            $ParamSetCount++
            $SyntaxObject.ParameterSetName = $script:SET_NAME_PLACEHOLDER + "_" + $ParamSetCount

            foreach($Parameter in $ParameterSet.Parameter)
            {
                $ParameterObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlParameter

                $ParameterObject.Type = GuessTheType $Parameter.parameterValue

                $ParameterObject.Name = $Parameter.Name
                $ParameterObject.Required = $Parameter.required -eq 'true'
                $ParameterObject.PipelineInput = $Parameter.pipelineInput | normalizeFirstLatter

                $ParameterObject.ValueRequired = -not ($ParameterObject.Type -eq "SwitchParameter") # thisDefinition is a heuristic

                if ($parameter.Aliases -ne 'None')
                {
                    $ParameterObject.Aliases = $parameter.Aliases
                }

                FillUpParameterFromHelp $ParameterObject

                $SyntaxObject.Parameters.Add($ParameterObject)
            }

            $MamlCommandObject.Syntax.Add($SyntaxObject)
        }
    }

    if ($UseHelpForParametersMetadata)
    {
        FillUpSyntaxFromHelp
    }
    else
    {
        FillUpSyntaxFromCommand
    }

    #endregion
    ##########

    #####GET THE HELP-Object Content and add it to the MAML Object#####
    #region Help-Object processing

    #Get Synopsis
    if ($UsePlaceholderForSynopsis)
    {
        # Help object ALWAYS contains SYNOPSIS.
        # If it's not available, it's auto-generated.
        # We don't want to include auto-generated SYNOPSIS (see https://github.com/PowerShell/platyPS/issues/110)
        $MamlCommandObject.Synopsis = New-Object -TypeName Markdown.MAML.Model.Markdown.SectionBody ($LocalizedData.Synopsis)
    }
    else
    {
        $MamlCommandObject.Synopsis = New-Object -TypeName Markdown.MAML.Model.Markdown.SectionBody (
            # $Help.Synopsis only contains the first paragraph
            # https://github.com/PowerShell/platyPS/issues/328
            $Help.details.description |
            DescriptionToPara |
            AddLineBreaksForParagraphs
        )
    }

    #Get Description
    if($Help.description -ne $null)
    {
        $MamlCommandObject.Description =  New-Object -TypeName Markdown.MAML.Model.Markdown.SectionBody (
            $Help.description |
            DescriptionToPara |
            AddLineBreaksForParagraphs
        )
    }

    #Add to Notes
    #From the Help AlertSet data
    if($help.alertSet)
    {
        $MamlCommandObject.Notes =  New-Object -TypeName Markdown.MAML.Model.Markdown.SectionBody (
            $help.alertSet.alert |
            DescriptionToPara |
            AddLineBreaksForParagraphs
        )
    }

    # Not provided by the command object. Using the Command Type to create a note declaring it's type.
    # We can add this placeholder


    #Add to relatedLinks
    if($help.relatedLinks)
    {
       foreach($link in $Help.relatedLinks.navigationLink)
        {
            $mamlLink = New-Object -TypeName Markdown.MAML.Model.MAML.MamlLink
            $mamlLink.LinkName = $link.linkText
            $mamlLink.LinkUri = $link.uri
            $MamlCommandObject.Links.Add($mamlLink)
        }
    }

    #Add Examples
    foreach($Example in $Help.examples.example)
    {
        $MamlExampleObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlExample

        $MamlExampleObject.Introduction = $Example.introduction
        $MamlExampleObject.Title = $Example.title
        $MamlExampleObject.Code = @(
            New-Object -TypeName Markdown.MAML.Model.MAML.MamlCodeBlock ($Example.code, '')
        )

        $RemarkText = $Example.remarks |
            DescriptionToPara |
            AddLineBreaksForParagraphs

        $MamlExampleObject.Remarks = $RemarkText
        $MamlCommandObject.Examples.Add($MamlExampleObject)
    }

    #Get Inputs
    #Reccomend adding a Parameter Name and Parameter Set Name to each input object.
    #region Inputs
    $Inputs = @()

    $Help.inputTypes.inputType | ForEach-Object {
        $InputDescription = $_.description
        $inputtypes = $_.type.name
        if ($_.description -eq $null -and $_.type.name -ne $null)
        {
            $inputtypes = $_.type.name.split("`n", [System.StringSplitOptions]::RemoveEmptyEntries)
        }

        $inputtypes | ForEach-Object {
            $InputObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlInputOutput
            $InputObject.TypeName = $_
            $InputObject.Description = $InputDescription |
                DescriptionToPara |
                AddLineBreaksForParagraphs
            $Inputs += $InputObject
        }
    }

    foreach($Input in $Inputs) {$MamlCommandObject.Inputs.Add($Input)}

    #endregion

    #Get Outputs
    #No Output Type description is provided from the command object.
    #region Outputs
    $Outputs = @()

    $Help.returnValues.returnValue | ForEach-Object {
        $OuputDescription = $_.description
        $Outputtypes = $_.type.name
        if ($_.description -eq $null -and $_.type.name -ne $null)
        {
            $Outputtypes = $_.type.name.split("`n", [System.StringSplitOptions]::RemoveEmptyEntries)
        }

        $Outputtypes | ForEach-Object {
            $OutputObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlInputOutput
            $OutputObject.TypeName = $_
            $OutputObject.Description = $OuputDescription |
                DescriptionToPara |
                AddLineBreaksForParagraphs
            $Outputs += $OutputObject
        }
    }

    foreach($Output in $Outputs) {$MamlCommandObject.Outputs.Add($Output)}
    #endregion
    ##########

    #####Adding Parameters Section from Syntax block#####
    #region Parameter Unique Selection from Parameter Sets
    #This will only work when the Parameters member has a public set as well as a get.

    function Get-ParameterByName
    {
        param(
            [string]$Name
        )

        $defaultSyntax = $MamlCommandObject.Syntax | Where-Object { $Command.DefaultParameterSet -eq $_.ParameterSetName }
        # default syntax should have a priority
        $syntaxes = @($defaultSyntax) + $MamlCommandObject.Syntax

        foreach ($s in $syntaxes)
        {
            $param = $s.Parameters | Where-Object { $_.Name -eq $Name }
            if ($param)
            {
                return $param
            }
        }
    }

    function Get-ParameterNamesOrder()
    {
        # we want to keep original order for existing help
        # if something changed:
        #   - remove it from it's position
        #   - add to the end

        $helpNames = $Help.parameters.parameter.Name
        if (-not $helpNames) { $helpNames = @() }

        # sort-object unique does case-insensiteve unification
        $realNames = $MamlCommandObject.Syntax.Parameters.Name | Sort-object -Unique
        if (-not $realNames) { $realNames = @() }

        $realNamesList = New-Object 'System.Collections.Generic.List[string]'
        $realNamesList.AddRange( ( [string[]] $realNames) )

        foreach ($name in $helpNames)
        {
            if ($realNamesList.Remove($name))
            {
                # yeild
                $name
            }
            # Otherwise it didn't exist
        }

        foreach ($name in $realNamesList)
        {
            # yeild
            $name
        }

    }

    foreach($ParameterName in (Get-ParameterNamesOrder))
    {
        $Parameter = Get-ParameterByName $ParameterName
        if ($Parameter)
        {
            if ($UseFullTypeName)
            {
                $Parameter = $Parameter.Clone()
                $Parameter.Type = $Parameter.FullType
            }
            $MamlCommandObject.Parameters.Add($Parameter)
        }
        else
        {
            Write-Warning -Message ($LocalizedData.ParameterNotFound -f '[Markdown generation]', $ParameterName, $Command.Name)
        }
    }

    # Handle CommonParameters, default for MamlCommand is SupportCommonParameters = $true
    if ($Command.CmdletBinding -eq $false)
    {
        # Remove CommonParameters by exception
        $MamlCommandObject.SupportCommonParameters = $false
    }

    # Handle CommonWorkflowParameters
    $MamlCommandObject.IsWorkflow = $IsWorkflow

    #endregion
    ##########

    return $MamlCommandObject
}

function validateWorkingProvider
{
    if((Get-Location).Drive.Provider.Name -ne 'FileSystem')
    {
        Write-Verbose -Message $LocalizedData.SettingFileSystemProvider
        $AvailableFileSystemDrives = Get-PSDrive | Where-Object {$_.Provider.Name -eq "FileSystem"} | Select-Object Root
        if($AvailableFileSystemDrives.Count -gt 0)
        {
           Set-Location $AvailableFileSystemDrives[0].Root
        }
        else
        {
             throw $LocalizedData.FailedSettingFileSystemProvider
        }
    }
}
#endregion

#region Parameter Auto Completers


#                                       bbbbbbbb
# TTTTTTTTTTTTTTTTTTTTTTT               b::::::b                                     CCCCCCCCCCCCC                                                             lllllll                              tttt            iiii
# T:::::::::::::::::::::T               b::::::b                                  CCC::::::::::::C                                                             l:::::l                           ttt:::t           i::::i
# T:::::::::::::::::::::T               b::::::b                                CC:::::::::::::::C                                                             l:::::l                           t:::::t            iiii
# T:::::TT:::::::TT:::::T                b:::::b                               C:::::CCCCCCCC::::C                                                             l:::::l                           t:::::t
# TTTTTT  T:::::T  TTTTTTaaaaaaaaaaaaa   b:::::bbbbbbbbb                      C:::::C       CCCCCC   ooooooooooo      mmmmmmm    mmmmmmm   ppppp   ppppppppp    l::::l     eeeeeeeeeeee    ttttttt:::::ttttttt    iiiiiii    ooooooooooo   nnnn  nnnnnnnn
#         T:::::T        a::::::::::::a  b::::::::::::::bb                   C:::::C               oo:::::::::::oo  mm:::::::m  m:::::::mm p::::ppp:::::::::p   l::::l   ee::::::::::::ee  t:::::::::::::::::t    i:::::i  oo:::::::::::oo n:::nn::::::::nn
#         T:::::T        aaaaaaaaa:::::a b::::::::::::::::b                  C:::::C              o:::::::::::::::om::::::::::mm::::::::::mp:::::::::::::::::p  l::::l  e::::::eeeee:::::eet:::::::::::::::::t     i::::i o:::::::::::::::on::::::::::::::nn
#         T:::::T                 a::::a b:::::bbbbb:::::::b --------------- C:::::C              o:::::ooooo:::::om::::::::::::::::::::::mpp::::::ppppp::::::p l::::l e::::::e     e:::::etttttt:::::::tttttt     i::::i o:::::ooooo:::::onn:::::::::::::::n
#         T:::::T          aaaaaaa:::::a b:::::b    b::::::b -:::::::::::::- C:::::C              o::::o     o::::om:::::mmm::::::mmm:::::m p:::::p     p:::::p l::::l e:::::::eeeee::::::e      t:::::t           i::::i o::::o     o::::o  n:::::nnnn:::::n
#         T:::::T        aa::::::::::::a b:::::b     b:::::b --------------- C:::::C              o::::o     o::::om::::m   m::::m   m::::m p:::::p     p:::::p l::::l e:::::::::::::::::e       t:::::t           i::::i o::::o     o::::o  n::::n    n::::n
#         T:::::T       a::::aaaa::::::a b:::::b     b:::::b                 C:::::C              o::::o     o::::om::::m   m::::m   m::::m p:::::p     p:::::p l::::l e::::::eeeeeeeeeee        t:::::t           i::::i o::::o     o::::o  n::::n    n::::n
#         T:::::T      a::::a    a:::::a b:::::b     b:::::b                  C:::::C       CCCCCCo::::o     o::::om::::m   m::::m   m::::m p:::::p    p::::::p l::::l e:::::::e                 t:::::t    tttttt i::::i o::::o     o::::o  n::::n    n::::n
#       TT:::::::TT    a::::a    a:::::a b:::::bbbbbb::::::b                   C:::::CCCCCCCC::::Co:::::ooooo:::::om::::m   m::::m   m::::m p:::::ppppp:::::::pl::::::le::::::::e                t::::::tttt:::::ti::::::io:::::ooooo:::::o  n::::n    n::::n
#       T:::::::::T    a:::::aaaa::::::a b::::::::::::::::b                     CC:::::::::::::::Co:::::::::::::::om::::m   m::::m   m::::m p::::::::::::::::p l::::::l e::::::::eeeeeeee        tt::::::::::::::ti::::::io:::::::::::::::o  n::::n    n::::n
#       T:::::::::T     a::::::::::aa:::ab:::::::::::::::b                        CCC::::::::::::C oo:::::::::::oo m::::m   m::::m   m::::m p::::::::::::::pp  l::::::l  ee:::::::::::::e          tt:::::::::::tti::::::i oo:::::::::::oo   n::::n    n::::n
#       TTTTTTTTTTT      aaaaaaaaaa  aaaabbbbbbbbbbbbbbbb                            CCCCCCCCCCCCC   ooooooooooo   mmmmmm   mmmmmm   mmmmmm p::::::pppppppp    llllllll    eeeeeeeeeeeeee            ttttttttttt  iiiiiiii   ooooooooooo     nnnnnn    nnnnnn
#                                                                                                                                           p:::::p
#                                                                                                                                           p:::::p
#                                                                                                                                          p:::::::p
#                                                                                                                                          p:::::::p
#                                                                                                                                          p:::::::p
#                                                                                                                                          ppppppppp


# Register-ArgumentCompleter can be provided thru TabExpansionPlusPlus or with V5 inbox module.
# We don't care much which one it is, but the inbox one doesn't have -Description parameter
if (Get-Command -Name Register-ArgumentCompleter -Module TabExpansionPlusPlus -ErrorAction Ignore) {
    Function ModuleNameCompleter {
        Param (
            $commandName,
            $parameterName,
            $wordToComplete,
            $commandAst,
            $fakeBoundParameter
        )

        Get-Module -Name "$wordToComplete*" |
            ForEach-Object {
                New-CompletionResult -CompletionText $_.Name -ToolTip $_.Description
            }
    }

    Register-ArgumentCompleter -CommandName New-MarkdownHelp -ParameterName Module -ScriptBlock $Function:ModuleNameCompleter -Description 'This argument completer handles the -Module parameter of the New-MarkdownHelp Command.'
}
elseif (Get-Command -Name Register-ArgumentCompleter -ErrorAction Ignore) {
    Function ModuleNameCompleter {
        Param (
            $commandName,
            $parameterName,
            $wordToComplete,
            $commandAst,
            $fakeBoundParameter
        )

        Get-Module -Name "$wordToComplete*" |
            ForEach-Object {
                $_.Name
            }
    }

    Register-ArgumentCompleter -CommandName New-MarkdownHelp -ParameterName Module -ScriptBlock $Function:ModuleNameCompleter
}

#endregion Parameter Auto Completers
