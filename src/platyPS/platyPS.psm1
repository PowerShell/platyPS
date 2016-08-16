#region PlatyPS

## DEVELOPERS NOTES & CONVENTIONS
##
##  1. Non-exported functions (subroutines) should avoid using 
##     PowerShell standart Verb-Noun naming convention.
##     They should use camalCase or CamalCase instead.
##  2. SMALL subroutines, used only from ONE function 
##     should be placed inside the parent function body.
##     They should use camalCase for the name.
##  3. LARGE subroutines and subroutines used from MORE THEN ONE function 
##     should be placed after the IMPLEMENTATION text block in the midle of this module.
##     They should use CamalCase for the name.
##  4. Add comment "# yeild" on subroutine calls that write values to pipeline.
##     It would help keep code maintainable and simplify ramp up for others.
## 

## Script constants

$script:EXTERNAL_HELP_FILE_YAML_HEADER = 'external help file'
$script:ONLINE_VERSION_YAML_HEADER = 'online version'
$script:SCHEMA_VERSION_YAML_HEADER = 'schema'

$script:UTF8_NO_BOM = New-Object System.Text.UTF8Encoding -ArgumentList $False
$script:SET_NAME_PLACEHOLDER = 'UNNAMED_PARAMETER_SET'
# TODO: this is just a place-holder, we can do better
$script:DEFAULT_MAML_XML_OUTPUT_NAME = 'rename-me-help.xml'

$script:MODULE_PAGE_MODULE_NAME = "Module Name"
$script:MODULE_PAGE_GUID = "Module Guid"
$script:MODULE_PAGE_LOCALE = "Locale"
$script:MODULE_PAGE_FW_LINK = "Download Help Link"
$script:MODULE_PAGE_HELP_VERSION = "Help Version"

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

        [Parameter(ParameterSetName="FromMaml")]
        [switch]$ConvertNotesToList,

        [Parameter(ParameterSetName="FromMaml")]
        [switch]$ConvertDoubleDashLists,

        [switch]$Force,

        [hashtable]$Metadata,

        [Parameter( 
            ParameterSetName="FromCommand")]
        [string]$OnlineVersionUrl = '',

        [Parameter(Mandatory=$true)]
        [string]$OutputFolder,

        [switch]$NoMetadata,

        [System.Text.Encoding]$Encoding = $script:UTF8_NO_BOM,

        [Parameter(ParameterSetName="FromModule")]
        [Parameter(ParameterSetName="FromMaml")]
        [switch]$WithModulePage,

        [Parameter(ParameterSetName="FromModule")]
        [Parameter(ParameterSetName="FromMaml")]
        [string]
        $Locale = "en-US",

        [Parameter(ParameterSetName="FromModule")]
        [Parameter(ParameterSetName="FromMaml")]
        [string]
        $HelpVersion = "{{Please enter version of help manually (X.X.X.X) format}}",

        [Parameter(ParameterSetName="FromModule")]
        [Parameter(ParameterSetName="FromMaml")]
        [string]
        $FwLink = "{{Please enter FwLink manually}}",
        
        [Parameter(ParameterSetName="FromMaml")]
        [string]
        $ModuleName = "MamlModule",
        
        [Parameter(ParameterSetName="FromMaml")]
        [string]
        $ModuleGuid = "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX"

    )

    begin
    {
        validateWorkingProvider
        mkdir $OutputFolder -ErrorAction SilentlyContinue > $null
    }

    process
    {
        function ProcessMamlObjectToFile
        {
            param(
                [Parameter(ValueFromPipeline=$true)]
                [ValidateNotNullOrEmpty()]
                [Markdown.MAML.Model.MAML.MamlCommand]$mamlObject
            )

            process
            {
                # populate template
                UpdateMamlObject $mamlObject
                if (-not $OnlineVersionUrl)
                {
                    # if it's not passed, we should get it from the existing help
                    $onlineLink = $mamlObject.Links | Select -First 1
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

                        if ($module) {
                            # for module case, scope it just to this module
                            $a['Module'] = $module
                        }

                        $helpFileName = GetHelpFileName (Get-Command @a)    
                    }
                    
                    $newMetadata = ($Metadata + @{
                        $script:EXTERNAL_HELP_FILE_YAML_HEADER = $helpFileName
                        $script:ONLINE_VERSION_YAML_HEADER = $online
                    })
                }

                $md = ConvertMamlModelToMarkdown -mamlCommand $mamlObject -metadata $newMetadata -NoMetadata:$NoMetadata

                MySetContent -path (Join-Path $OutputFolder "$commandName.md") -value $md -Encoding $Encoding -Force:$Force
            }
        }

        if ($NoMetadata -and $Metadata)
        {
            throw '-NoMetadata and -Metadata cannot be specified at the same time'
        }

        if ($PSCmdlet.ParameterSetName -eq 'FromCommand')
        {
            $command | ForEach-Object {
                if (-not (Get-Command $_ -EA SilentlyContinue))
                {
                    throw "Command $_ not found in the session."
                }

                GetMamlObject -Cmdlet $_ | ProcessMamlObjectToFile
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
                        throw "Module $_ is not imported in the session. Run 'Import-Module $_'."
                    }
                    
                    GetMamlObject -Module $_ | ProcessMamlObjectToFile
                    
                    $ModuleName = $_
                    $ModuleGuid = (Get-Module $ModuleName).Guid
                    $CmdletNames = GetCommands -AsNames -Module $ModuleName
                }
                else # 'FromMaml'
                {
                    if (-not (Test-Path $_))
                    {
                        throw "No file found in $_."
                    }

                    GetMamlObject -MamlFile $_ -ConvertNotesToList:$ConvertNotesToList -ConvertDoubleDashLists:$ConvertDoubleDashLists | ProcessMamlObjectToFile
                        
                    $CmdletNames += GetMamlObject -MamlFile $_ | ForEach-Object {$_.Name}
                }
                
                if($WithModulePage)
                {
                    if(-not $ModuleGuid)
                    {
                        $ModuleGuid = "00000000-0000-0000-0000-000000000000"
                    }
                    if($ModuleGuid.Count -gt 1)
                    {
                        Write-Warning -Message "This module has more than 1 guid. This could impact external help creation."
                    }
                    # yeild
                    NewModuleLandingPage  -Path $OutputFolder `
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

function Update-MarkdownHelpSchema
{
    [CmdletBinding()]
    [OutputType([System.IO.FileInfo[]])]
    
    param(
        [Parameter(Mandatory=$true,
            ValueFromPipeline=$true)]
        [string[]]$Path,

        [Parameter(Mandatory=$true)]
        [string]$OutputFolder,

        [System.Text.Encoding]$Encoding = $script:UTF8_NO_BOM,
        
        [switch]$Force
    )
    
    begin
    {
        $MarkdownFiles = @()
        if ($OutputFolder)
        {
            mkdir $OutputFolder -ErrorAction SilentlyContinue > $null
        }
    }
    
    process
    {
        $MarkdownFiles += GetMarkdownFilesFromPath $Path
    }
    
    end
    {
        $markdown = $MarkdownFiles | ForEach-Object {
            Get-Content -Raw $_.FullName
        }

        $model = GetMamlModelImpl $markdown -ForAnotherMarkdown
        $parseMode = GetParserMode -PreserveFormatting
        $r = New-Object -TypeName Markdown.MAML.Renderer.MarkdownV2Renderer -ArgumentList $parseMode

        $model | ForEach-Object {
            $name = $_.Name
            # TODO: can we pass some metadata here?
            # skipYamlHeader -eq $false
            $md = $r.MamlModelToString($_, $false)
            $outPath = Join-Path $OutputFolder "$name.md"
            Write-Verbose "[Update-Markdown] Writing updated markdown to $outPath"
            MySetContent -path $outPath -value $md -Encoding $Encoding -Force:$Force # yeild
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
        [string[]]$Path,

        [System.Text.Encoding]$Encoding = $script:UTF8_NO_BOM,

        [string]$LogPath,
        [switch]$LogAppend
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
             log -warning "No markdown found in $Path"
            return
        }


        $MarkdownFiles | ForEach-Object {
            $file = $_

            $filePath = $file.FullName
            $oldMarkdown = Get-Content -Raw $filePath
            $oldModels = GetMamlModelImpl $oldMarkdown -ForAnotherMarkdown

            if ($oldModels.Count -gt 1)
            {
                log -warning "$filePath contains more then 1 command, skipping upgrade."
                log -warning  "Use 'Update-Markdown -OutputFolder' to convert help to one command per file format first."
                return
            }

            $oldModel = $oldModels[0]

            $name = $oldModel.Name
            $command = Get-Command $name
            if (-not $command)
            {
                log -warning  "command $name not found in the session, skipping upgrade for $filePath"
                return
            }

            # just preserve old metadata
            $metadata = Get-MarkdownMetadata $filePath
            $reflectionModel = GetMamlObject -Cmdlet $name

            $merger = New-Object Markdown.MAML.Transformer.MamlModelMerger -ArgumentList $infoCallback
            $newModel = $merger.Merge($reflectionModel, $oldModel)

            $md = ConvertMamlModelToMarkdown -mamlCommand $newModel -metadata $metadata -PreserveFormatting
            MySetContent -path $file.FullName -value $md -Encoding $Encoding -Force # yeild
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
        [string[]]$Path,
        
        [System.Text.Encoding]$Encoding = $script:UTF8_NO_BOM,
        [string]$LogPath,
        [switch]$LogAppend
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
                log "Determined module name for $modulePath as $module"
            }
            
            if (-not $module)
            {
                Write-Error "Cannot determine module name for $modulePath. You should use New-MarkdownHelp -WithModulePage to create HelpModule"
                continue
            }
        
            # always append on this call
            $affectedFiles = Update-MarkdownHelp -Path $modulePath -LogPath $LogPath -LogAppend -Encoding $Encoding
            $affectedFiles # yeild
            
            $allCommands = GetCommands -AsNames -Module $Module
            if (-not $allCommands)
            {
                throw "Module $Module is not imported in the session or doesn't have any exported commands"
            }

            $updatedCommands = $affectedFiles.BaseName
            $allCommands | ForEach-Object {
                if ( -not ($updatedCommands -contains $_) )
                {
                    log "Creating new markdown for command $_"
                    $newFiles = New-MarkdownHelp -Command $_ -OutputFolder $modulePath
                    $newFiles # yeild
                }
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
            throw "The output folder does not exist."
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
        [string[]]$Path,

        [Parameter(Mandatory=$true)]
        [string]$OutputPath,

        [System.Text.Encoding]$Encoding = $script:UTF8_NO_BOM,

        [switch]$Force
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
            Write-Verbose "[New-ExternalHelp] Use $OutputPath as path to a file"
        }
        else 
        {
            mkdir $OutputPath -ErrorAction SilentlyContinue > $null
            Write-Verbose "[New-ExternalHelp] Use $OutputPath as path to a directory"
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
        $MarkdownFiles | ForEach-Object {
            Write-Verbose "[New-ExternalHelp] Input markdown file $_"
        }

        $r = new-object -TypeName 'Markdown.MAML.Renderer.MamlRenderer'
        
        if ($IsOutputContainer)
        {
            $defaultPath = Join-Path $OutputPath $script:DEFAULT_MAML_XML_OUTPUT_NAME
            $groups = $MarkdownFiles | Group-Object { 
                $h = Get-MarkdownMetadata -Path $_.FullName
                if ($h -and $h[$script:EXTERNAL_HELP_FILE_YAML_HEADER]) 
                {
                    Join-Path $OutputPath $h[$script:EXTERNAL_HELP_FILE_YAML_HEADER]
                }
                else 
                {
                    Write-Warning "[New-ExternalHelp] cannot find '$($script:EXTERNAL_HELP_FILE_YAML_HEADER)' in metadata for file $($_.FullName)"
                    Write-Warning "[New-ExternalHelp] $defaultPath would be used"
                    $defaultPath
                }
            }
        }
        else 
        {
            $groups = $MarkdownFiles | Group-Object { $OutputPath }
        }

        foreach ($group in $groups) {
            $maml = GetMamlModelImpl ( $group.Group | ForEach-Object { Get-Content -Raw $_.FullName -Encoding UTF8} )
            $xml = $r.MamlModelToString($maml, $false) # skipPreambula is not used
            $outPath = $group.Name # group name
            Write-Verbose "Writing external help to $outPath"
            MySetContent -Path $outPath -Value $xml -Encoding $Encoding -Force:$Force
        }
        
        if($AboutFiles.Count -gt 0)
        {
            foreach($About in $AboutFiles)
            {
                $r = New-Object -TypeName 'Markdown.MAML.Renderer.TextRenderer' -ArgumentList(80)
                $Content = Get-Content -Raw $About.FullName
                $p = NewMarkdownParser
                $model = $p.ParseString($Content)
                $value = $r.AboutMarkDownToString($model)

                $outPath = Join-Path $OutputPath ([io.path]::GetFileNameWithoutExtension($About.FullName) + ".txt")
                if(!(Split-Path -Leaf $outPath).ToUpper().StartsWith("ABOUT_",$true,$null))
                {
                    $outPath = Join-Path (Split-Path -Parent $outPath) ("about_" + (Split-Path -Leaf $outPath))
                }
                MySetContent -Path $outPath -Value $value -Encoding $Encoding -Force:$Force
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
                Write-Error "$MamlFilePath is not found, skipping"
                continue
            }

            # this is Resolve-Path that resolves mounted drives (i.e. good for tests)
            $MamlFilePath = (Get-ChildItem $MamlFilePath).FullName

            # Read the malm file
            $xml = [xml](Get-Content $MamlFilePath -Raw -ea SilentlyContinue -Encoding UTF8)
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
                            $_.ChildNodes | Select -First 1 | 
                            ForEach-Object {
                                $newInnerXml = '* ' + $_.get_InnerXml()
                                $_.set_InnerXml($newInnerXml)
                            }

                            $_.ChildNodes | Select -Skip 1 | 
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
                            $_.navigationLink = $_.navigationLink | Select -Skip 1
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
                    Throw "$_ content source file folder path is not a valid directory."
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
                    Throw "$_ Module Landing Page path is nopt valid."
                }
            })]
        [string] $LandingPagePath,
        [parameter(Mandatory=$true)]
        [string] $OutputFolder
    )
    begin
    {
        validateWorkingProvider 
        mkdir $OutputFolder -ErrorAction SilentlyContinue > $null  
    }
    process
    {
        #Testing for MakeCab.exe
        Write-Verbose "Testing that MakeCab.exe is present on this machine."
        $MakeCab = Get-Command MakeCab
        if(-not $MakeCab)
        {
            throw "MakeCab.exe is not a registered command." 
        }
        #Testing for files in source directory
        if((Get-ChildItem -Path $CabFilesFolder).Count -le 0)
        {
            throw "The file count in the cab files directory is zero."
        }
        

    ###Get Yaml Metadata here
    $Metadata = Get-MarkdownMetadata -Path $LandingPagePath

    $ModuleName = $Metadata[$script:MODULE_PAGE_MODULE_NAME]
    $Guid = $Metadata[$script:MODULE_PAGE_GUID]
    $Locale = $Metadata[$script:MODULE_PAGE_LOCALE]
    $FwLink = $Metadata[$script:MODULE_PAGE_FW_LINK]
    $HelpVersion = $Metadata[$script:MODULE_PAGE_HELP_VERSION]

    #Create HelpInfo File
    


        #Testing the destination directories, creating if none exists.
        Write-Verbose "Checking the output directory"
        if(-not (Test-Path $OutputFolder))
        {
            Write-Verbose "Output directory does not exist, creating a new directory."
            New-Item -ItemType Directory -Path $OutputFolder
        }

        Write-Verbose ("Creating cab for {0}, with Guid {1}, in Locale {2}" -f $ModuleName,$Guid,$Locale)

        #Building the cabinet file name.
        $cabName = ("{0}_{1}_{2}_helpcontent.cab" -f $ModuleName,$Guid,$Locale)

        #Setting Cab Directives, make a cab is turned on, compression is turned on
        Write-Verbose "Creating Cab File"
        $DirectiveFile = "dir.dff"
        New-Item -ItemType File -Name $DirectiveFile -Force |Out-Null   
        Add-Content $DirectiveFile ".Set Cabinet=on"
        Add-Content $DirectiveFile ".Set Compress=on"
        
        #Creates an entry in the cab directive file for each file in the source directory (uses FullName to get fuly qualified file path and name)     
        foreach($file in Get-ChildItem -Path $CabFilesFolder -File)
        {
            Add-Content $DirectiveFile ("'" + ($file).FullName +"'" )
        }

        #Making Cab
        Write-Verbose "Making the cab file"
        MakeCab.exe /f $DirectiveFile | Out-Null

        #Naming CabFile
        Write-Verbose "Moving the cab to the output directory"
        Copy-Item "disk1/1.cab" (Join-Path $OutputFolder $cabName)

        #Remove ExtraFiles created by the cabbing process
        Write-Verbose "Performing cabbing cleanup"
        Remove-Item "setup.inf" -ErrorAction SilentlyContinue
        Remove-Item "setup.rpt" -ErrorAction SilentlyContinue
        Remove-Item $DirectiveFile -ErrorAction SilentlyContinue
        Remove-Item -Path "disk1" -Recurse -ErrorAction SilentlyContinue

        #Create the HelpInfo Xml 
        MakeHelpInfoXml -ModuleName $ModuleName -GUID $Guid -HelpCulture $Locale -HelpVersion $HelpVersion -URI $FwLink -OutputFolder $OutputFolder
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
                mkdir $containerFolder -ErrorAction SilentlyContinue > $null
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

        if((Get-Content $AboutFilePath)[1].length -gt 3)
        {
            if((Get-Content $AboutFilePath)[1].substring(3,5).ToUpper() -eq "ABOUT")
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
                    $AboutMarkdownFiles += Get-ChildItem $_ -Filter '*.md' | Where {($_.FullName -notin $MarkDownFilesAlreadyFound) -and (ConfirmAboutBySecondHeaderText($_.FullName))}
                }
                else
                {
                    $AboutMarkdownFiles += Get-ChildItem $_ -Filter '*.md' | Where {ConfirmAboutBySecondHeaderText($_.FullName)}
                }
            }
            else 
            {
                Write-Error "$_ about file not found"
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
    

    $MarkdownFiles = @()
    if ($Path) { 
        $Path | ForEach-Object {
            if (Test-Path -PathType Leaf $_)
            {
                $MarkdownFiles += Get-ChildItem $_
            }
            elseif (Test-Path -PathType Container $_)
            {
                $MarkdownFiles += Get-ChildItem $_ -Filter $filter
            }
            else 
            {
                Write-Error "$_ is not found"
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
        [string[]]$markdown,
        [switch]$ForAnotherMarkdown
    )

    # we need to pass it into .NET IEnumerable<MamlCommand> API
    $res = New-Object 'System.Collections.Generic.List[Markdown.MAML.Model.MAML.MamlCommand]'

    $markdown | ForEach-Object {
        $mdText = $_
        $schema = GetSchemaVersion $mdText
        $p = NewMarkdownParser -PreserveFormatting:$ForAnotherMarkdown
        $t = NewModelTransformer -schema $schema

        $parseMode = GetParserMode -PreserveFormatting:$ForAnotherMarkdown
        $model = $p.ParseString($_, $parseMode)
        Write-Progress -Activity "Parsing markdown" -Completed    
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
    return new-object -TypeName 'Markdown.MAML.Parser.MarkdownParser' -ArgumentList {
        param([int]$current, [int]$all) 
        Write-Progress -Activity "Parsing markdown" -status "Progress:" -percentcomplete ($current/$all*100)
    }
}

function NewModelTransformer
{
    param(
        [ValidateSet('1.0.0', '2.0.0')]
        [string]$schema
    )

    if ($schema -eq '1.0.0')
    {
        return new-object -TypeName 'Markdown.MAML.Transformer.ModelTransformerVersion1' -ArgumentList {
            param([string]$message)
            Write-Verbose $message
        }
    }
    elseif ($schema -eq '2.0.0')
    {
        return new-object -TypeName 'Markdown.MAML.Transformer.ModelTransformerVersion2' -ArgumentList {
            param([string]$message)
            Write-Verbose $message
        }
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
        if (-not $schema) 
        {
            # there is metadata, but schema version is not specified.
            # assume 2.0.0
            $schema = '2.0.0'
        }
    }
    else 
    {
        # if there is not metadata, then it's schema version 1.0.0
        $schema = '1.0.0'    
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

function UpdateMamlObject
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

        $MamlExampleObject.Title = 'Example 1'
        $MamlExampleObject.Code = 'PS C:\> {{ Add example code here }}'
        $MamlExampleObject.Remarks = '{{ Add example description here }}'

        $MamlCommandObject.Examples.Add($MamlExampleObject)
    }
}

function SetOnlineVersionUrlLink
{
    param(
        [Parameter(Mandatory=$true)]
        [Markdown.MAML.Model.MAML.MamlCommand]$MamlCommandObject,

        [string]$OnlineVersionUrl = $null
    )

    # Online Version URL
    $currentFirstLink = $MamlCommandObject.Links | Select -First 1

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

        # overwise, lets guess it
        $module = @($CommandInfo.Module) + ($CommandInfo.Module.NestedModules) | 
            Where-Object {$_.ModuleType -ne 'Manifest'} | 
            Where-Object {$_.ExportedCommands.Keys -contains $CommandInfo.Name}

        if (-not $module)
        {
            Write-Warning "[GetHelpFileName] Cannot find module for $($CommandInfo.Name)"
            return
        }

        if ($module.Count -gt 1)
        {
            Write-Warning "[GetHelpFileName] Found $($module.Count) modules for $($CommandInfo.Name)"
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
            Write-Error "Cannot write file to $Path, directory with the same name exists."
            return
        }

        if (-not $Force)
        {
            Write-Error "Cannot write to $Path, file exists. Use -Force to overwrite."
            return
        }
    }
    else 
    {
        $dir = Split-Path $Path
        if ($dir) 
        {
            mkdir $dir -ErrorAction SilentlyContinue > $null
        }
    }

    Write-Verbose "Writing to $Path with encoding = $($Encoding.EncodingName)"
    # just to create a file
    Set-Content -Path $Path -Value ''
    $resolvedPath = (Get-ChildItem $Path).FullName
    [System.IO.File]::WriteAllText($resolvedPath, $value, $Encoding)
    return (Get-ChildItem $Path)
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
        [Parameter(mandatory=$true)]
        [string]
        $ModuleGuid,
        [Parameter(mandatory=$true)]
        [string[]]
        $CmdletNames,
        [Parameter(mandatory=$true)]
        [string]
        $Locale,
        [Parameter(mandatory=$true)]
        [string]
        $Version,
        [Parameter(mandatory=$true)]
        [string]
        $FwLink,
        [Parameter(mandatory=$true)]
        [System.Text.Encoding]$Encoding = $script:UTF8_NO_BOM,

        [switch]$Force
    )

    begin
    {
        $LandingPageName = $ModuleName + ".md"
        $LandingPagePath = Join-Path $Path $LandingPageName
    }

    process
    {
        $Content = "---`r`nModule Name: $ModuleName`r`nModule Guid: $ModuleGuid`r`nDownload Help Link: $FwLink`r`n"
        $Content += "Help Version: $Version`r`nLocale: $Locale`r`n"
        $Content += "---`r`n`r`n# $ModuleName Module`r`n## Description`r`n"
        $Content += "{{Manually Enter Description Here}}`r`n`r`n## $ModuleName Cmdlets`r`n"
        
        $CmdletNames | ForEach-Object {
            $Content += "### [" + $_ + "](" + $_ + ".md)`r`n{{Manually Enter $_ Description Here}}`r`n`r`n"    
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
        [switch]$AsNames
    )

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
        $commands
    }
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
        [switch] $ConvertDoubleDashLists
    )

    function CommandHasAutogeneratedSynopsis
    {
        param([object]$help)

        return (Get-Command $help.Name -Syntax) -eq ($help.Synopsis)
    }

    if($Cmdlet)
    {
        Write-Verbose ("Processing: " + $Cmdlet)
        $Help = Get-Help $Cmdlet
        $Command = Get-Command $Cmdlet
        return ConvertPsObjectsToMamlModel -Command $Command -Help $Help -UsePlaceholderForSynopsis:(CommandHasAutogeneratedSynopsis $Help)
    }
    elseif ($Module)
    {
        Write-Verbose ("Processing: " + $Module)

        $commands = GetCommands $Module
        foreach ($Command in $commands)
        {
            Write-Verbose ("`tProcessing: " + $Command.Name)
            $Help = Get-Help $Command.Name
            # yeild
            ConvertPsObjectsToMamlModel -Command $Command -Help $Help -UsePlaceholderForSynopsis:(CommandHasAutogeneratedSynopsis $Help)
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
            
            # yeild
            ConvertPsObjectsToMamlModel -Command $Command -Help $Help -UseHelpForParametersMetadata
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
        [switch]$UsePlaceholderForSynopsis
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

    function getTypeString
    {
        param(
            [Parameter(ValueFromPipeline=$true)]
            [System.Reflection.TypeInfo]
            $typeObject
        )
        
        # special case for nullable value types
        if ($typeObject.Name -eq 'Nullable`1')
        {
            return $typeObject.GenericTypeArguments.Name
        }

        if ($typeObject.IsGenericType)
        {
            # keep information about generic parameters
            return $typeObject.ToString()
        }

        return $typeObject.Name
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

    #region Data not provided by the command object
    
    #Get Description
    #Not provided by the command object.
    $MamlCommandObject.Description = "{{Fill in the Description}}"

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
        $ParameterObject.Globbing = $HelpEntry.globbing -eq 'True'
        $ParameterObject.Position = $HelpEntry.position | normalizeFirstLatter
        if ($HelpEntry.description) 
        {
            if ($HelpEntry.description.text)
            {
                $ParameterObject.Description = $HelpEntry.description.text | AddLineBreaksForParagraphs
            }
            else 
            {
                # this case happens, when there is HelpMessage in 'Parameter' attribute,
                # but there is no maml or comment-based help.
                # then help engine put string outside of 'text' property
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

                $ParameterObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlParameter

                $ParameterObject.Type = $Parameter.ParameterType | getTypeString
                $ParameterObject.Name = $Parameter.Name
                $ParameterObject.Required = $Parameter.IsMandatory
                $ParameterObject.PipelineInput = getPipelineValue $Parameter

                $ParameterObject.ValueRequired = -not ($Parameter.Type -eq "SwitchParameter") # thisDefinition is a heuristic

                foreach($Alias in $Parameter.Aliases)
                {
                    $ParameterObject.Aliases += $Alias
                }
                
                $ParameterObject.Description = if ([String]::IsNullOrEmpty($Parameter.HelpMessage)) 
                {
                    # additional new-lines are needed for Update-MarkdownHelp scenario.
                    "{{Fill $($Parameter.Name) Description}}`r`n`r`n"
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
        $MamlCommandObject.Synopsis = "{{Fill in the Synopsis}}"
    }
    else 
    {
        $MamlCommandObject.Synopsis = $Help.Synopsis.Trim()    
    }

    #Get Description
    if($Help.description -ne $null)
    {
        $MamlCommandObject.Description = $Help.description.Text | AddLineBreaksForParagraphs
    }

    #Add to Notes
    #From the Help AlertSet data
    if($help.alertSet)
    {
        $MamlCommandObject.Notes = $help.alertSet.alert.Text | AddLineBreaksForParagraphs
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
        $MamlExampleObject.Code = $Example.code

        $RemarkText = $Example.remarks.text | AddLineBreaksForParagraphs
        
        $MamlExampleObject.Remarks = $RemarkText
        $MamlCommandObject.Examples.Add($MamlExampleObject)
    }

    #Get Inputs
    #Reccomend adding a Parameter Name and Parameter Set Name to each input object.
    #region Inputs
    $Inputs = @()
    
    $Help.inputTypes.inputType | ForEach-Object {
        $InputObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlInputOutput
        $InputObject.TypeName = $_.type.name
        $InputObject.Description = $_.description.Text | AddLineBreaksForParagraphs
        $Inputs += $InputObject
    }
    
    foreach($Input in $Inputs) {$MamlCommandObject.Inputs.Add($Input)}
 
    #endregion
 
    #Get Outputs
    #No Output Type description is provided from the command object.
    #region Outputs
    $Outputs = @()
    
    $Help.returnValues.returnValue | ForEach-Object {
        $OutputObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlInputOutput
        $OutputObject.TypeName = $_.type.name
        $OutputObject.Description = $_.description.Text | AddLineBreaksForParagraphs
        $Outputs += $OutputObject
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
            $MamlCommandObject.Parameters.Add($Parameter)
        }
        else 
        {
            Write-Warning "[Markdown generation] Could not find parameter object for $ParameterName in command $($Command.Name)"    
        }
    }

    # handle CommonParameters and CommonWorkflowParameters
    if ($Command.CommandType -eq 'Function' -and $Command.CmdletBinding -eq $false)
    {
        # this is a really weired case, it may never appear in real modules
        $MamlCommandObject.SupportCommonParameters = $false
    }
    else 
    {
        $MamlCommandObject.SupportCommonParameters = $false
    }

    $MamlCommandObject.IsWorkflow = $IsWorkflow

    #endregion
    ##########

    return $MamlCommandObject
}

function validateWorkingProvider
{
    if((Get-Location).Drive.Provider.Name -ne 'FileSystem')
    {
        Write-Verbose 'PlatyPS Cmdlets only work in the FileSystem Provider. PlatyPS is changing the provider of this session back to filesystem.'
        $AvailableFileSystemDrives = Get-PSDrive | Where {$_.Provider.Name -eq "FileSystem"} | Select Root
        if($AvailableFileSystemDrives.Count -gt 0)
        {
           Set-Location $AvailableFileSystemDrives[0].Root
        }
        else 
        {
             throw 'PlatyPS Cmdlets only work in the FileSystem Provider.' 
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
