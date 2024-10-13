function Show-HelpPreview {
    [CmdletBinding()]
    [OutputType('MamlCommandHelpInfo')]
    param(
        [Parameter(Mandatory=$true,
            ValueFromPipeline=$true,
            ParameterSetName = "Path",
            Position=1)]
        [SupportsWildcards()]
        [string[]]$Path,

        [Parameter(Mandatory=$true,
            ValueFromPipeline=$true,
            ParameterSetName = "LiteralPath",
            Position=1)]
        [Alias('PSPath', 'LP')]
        [string[]]$LiteralPath,

        [switch]$ConvertNotesToList,
        [switch]$ConvertDoubleDashLists
    )

    process {
        if ($PSCmdlet.ParameterSetName -eq 'LiteralPath') {
            $paths = $LiteralPath
        }
        else {
            $paths = $Path
        }

        foreach ($MamlFilePath in $paths) {
            if ($PSCmdlet.ParameterSetName -eq 'LiteralPath') {
                $fileArgs = @{ LiteralPath = $MamlFilePath }
            }
            else {
                $fileArgs = @{ LiteralPath = $MamlFilePath }
            }

            if (-not (Test-path -Type Leaf @fileArgs)) {
                Write-Error -Message "File '${MamlFilePath}' does not exist."
                continue
            }

            # this is Resolve-Path that resolves mounted drives (i.e. good for tests)
            $MamlFilePath = (Get-ChildItem @fileArgs -ErrorAction Stop).FullName

            # Read the maml file
            $xml = [xml](Get-Content @fileArgs -Raw -ea SilentlyContinue)
            if (-not $xml) {
                # already error-out on the conversion, no need to repeat ourselves
                continue
            }

            # we need a copy of maml file to bypass powershell cache,
            # in case we reuse the same filename few times.
            $MamlCopyPath = [System.IO.Path]::GetTempFileName()
            try {
                if ($ConvertDoubleDashLists) {
                    $null = $xml.GetElementsByTagName('maml:para') | ForEach-Object {
                        # Convert "-- "-lists into "- "-lists
                        # to make them markdown compatible
                        # as described in https://github.com/PowerShell/platyPS/issues/117
                        $newInnerXml = $_.get_InnerXml() -replace "(`n|^)-- ", '$1- '
                        $_.set_InnerXml($newInnerXml)
                    }
                }

                if ($ConvertNotesToList) {
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
                    if ($_) {
                        $_.InnerXml = '<maml:navigationLink xmlns:maml="http://schemas.microsoft.com/maml/2004/10"><maml:linkText>PLATYPS_DUMMY_LINK</maml:linkText><maml:uri>https://github.com/PowerShell/platyPS/issues/144</maml:uri></maml:navigationLink>' + $_.InnerXml
                    }
                }

                $xml.Save($MamlCopyPath)

                foreach ($command in $xml.helpItems.command.details.name) {
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
                        if ($_) {
                            $_.navigationLink = $_.navigationLink | Select-Object -Skip 1
                        }
                    }
                    $help # yeild
                }
            }
            finally {
                Remove-Item $MamlCopyPath
            }
        }
    }
}

function Assert-TargetInFilesystem {
    param ([string]$target)
    $item = Get-Item $target -ErrorAction SilentlyContinue
    if ($item.PSProvider.Name -ne 'FileSystem') {
        throw "Target '$target' is not in the filesystem."
    }
}

function Assert-Command {
    param ([string]$command)
    $cmd = Get-Command $command -ErrorAction SilentlyContinue -Type Application
    if (-not $cmd) {
        throw "Command '$command' not found."
    }
}

function MakeHelpInfoXml {
    Param(
        [Parameter(mandatory=$true)] [string] $ModuleName,
        [Parameter(mandatory=$true)] [string] $GUID,
        [Parameter(mandatory=$true)] [string] $HelpCulture,
        [Parameter(mandatory=$true)] [string] $HelpVersion,
        [Parameter(mandatory=$true)] [string] $URI,
        [Parameter(mandatory=$true)] [string] $OutputFolder
    )

    $HelpInfoFileNme = $ModuleName + "_" + $GUID + "_HelpInfo.xml"
    $OutputFullPath = Join-Path $OutputFolder $HelpInfoFileNme

    if(Test-Path $OutputFullPath -PathType Leaf) {
        [xml] $HelpInfoContent = Get-Content $OutputFullPath
    }

    #Create the base XML object for the Helpinfo.xml file.
    $xml = [xml]::new()

    $ns = "http://schemas.microsoft.com/powershell/help/2010/05"
    $declaration = $xml.CreateXmlDeclaration("1.0","utf-8",$null)

    $rootNode = $xml.CreateElement("HelpInfo",$ns)
    $null = $xml.InsertBefore($declaration,$xml.DocumentElement)
    $null = $xml.AppendChild($rootNode)

    $HelpContentUriNode = $xml.CreateElement("HelpContentURI",$ns)
    $HelpContentUriNode.InnerText = $URI
    $null = $xml["HelpInfo"].AppendChild($HelpContentUriNode)

    $HelpSupportedCulturesNode = $xml.CreateElement("SupportedUICultures",$ns)
    $null = $xml["HelpInfo"].AppendChild($HelpSupportedCulturesNode)

    #If no previous help file
    if(-not $HelpInfoContent) {
        $HelpUICultureNode = $xml.CreateElement("UICulture",$ns)
        $null = $xml["HelpInfo"]["SupportedUICultures"].AppendChild($HelpUICultureNode)

        $HelpUICultureNameNode = $xml.CreateElement("UICultureName",$ns)
        $HelpUICultureNameNode.InnerText = $HelpCulture
        $null = $xml["HelpInfo"]["SupportedUICultures"]["UICulture"].AppendChild($HelpUICultureNameNode)

        $HelpUICultureVersionNode = $xml.CreateElement("UICultureVersion",$ns)
        $HelpUICultureVersionNode.InnerText = $HelpVersion
        $null = $xml["HelpInfo"]["SupportedUICultures"]["UICulture"].AppendChild($HelpUICultureVersionNode)

        [xml] $HelpInfoContent = $xml
    }
    else {
        #Get old culture info
        $ExistingCultures = @{}
        foreach($Culture in $HelpInfoContent.HelpInfo.SupportedUICultures.UICulture) {
            $ExistingCultures.Add($Culture.UICultureName, $Culture.UICultureVersion)
        }

        #If culture exists update version, if not, add culture and version
        if(-not ($HelpCulture -in $ExistingCultures.Keys)) {
            $null = $ExistingCultures.Add($HelpCulture,$HelpVersion)
        }
        else {
            $ExistingCultures[$HelpCulture] = $HelpVersion
        }

        $cultureNames = @()
        $cultureNames += $ExistingCultures.GetEnumerator()

        #write out cultures to XML
        for($i=0;$i -lt $ExistingCultures.Count; $i++) {
            $HelpUICultureNode = $xml.CreateElement("UICulture",$ns)
            $HelpUICultureNameNode = $xml.CreateElement("UICultureName",$ns)
            $HelpUICultureNameNode.InnerText = $cultureNames[$i].Name
            $null = $HelpUICultureNode.AppendChild($HelpUICultureNameNode)

            $HelpUICultureVersionNode = $xml.CreateElement("UICultureVersion",$ns)
            $HelpUICultureVersionNode.InnerText = $cultureNames[$i].Value
            $null = $HelpUICultureNode.AppendChild($HelpUICultureVersionNode)

            $null = $xml["HelpInfo"]["SupportedUICultures"].AppendChild($HelpUICultureNode)
        }

        [xml] $HelpInfoContent = $xml
    }

    $outputItem = Get-Item $outputFullPath
    $null = $HelpInfoContent.Save($outputItem.FullName)
    # return this to the caller
    $outputItem
}

function New-HelpCabinetFile {
    [Cmdletbinding(SupportsShouldProcess=$true)]
    [OutputType([System.IO.FileInfo])]
    param(
        [parameter(Mandatory=$true)]
        [ValidateScript({(Test-Path $_ -PathType Container)})]
        [string] $CabinetFilesFolder,

        [parameter(Mandatory=$true)]
        [ValidateScript({(Test-Path $_ -PathType Leaf)})]
        [string] $MarkdownModuleFile,

        [parameter(Mandatory=$true)]
        [string] $OutputFolder
    )

    end {
        if (! $IsWindows) {
            throw "'New-HelpCabinetFile' is only supported on Windows."
        }

        if (Test-Path $outputFolder -Leaf) {
            throw "Output folder '$outputFolder' is not a directory."
        }

        $outputFiles = @()

        if (! $PSCmdlet.ShouldProcess($OutputFolder, "Create Cabinet File")) {
            return
        }

        $MODULE_PAGE_MODULE_NAME = "Module Name"
        $MODULE_PAGE_GUID = "Module Guid"
        $MODULE_PAGE_LOCALE = "Locale"
        $MODULE_PAGE_FW_LINK = "Download Help Link"
        $MODULE_PAGE_HELP_VERSION = "Help Version"
        $MODULE_PAGE_ADDITIONAL_LOCALE = "Additional Locale"

        $outputFolderInfo = Get-Item $OutputFolder -ErrorAction SilentlyContinue
        if (! $outputFolderInfo) {
            $unresolvedOutputFolder = $executioncontext.sessionstate.path.GetUnresolvedProviderPathFromPSPath($OutputFolder)
            $outputFolderDrive = Get-PSDrive ((split-path -qual $unresolvedOutputFolder).trim(":"))
            if ($outputFolderDrive.Provider.Name -ne 'FileSystem') {
                throw "Output folder '$OutputFolder' is not in the filesystem."
            }
            $null = New-Item -Type Directory $OutputFolder -ErrorAction Stop
        }
        else {
            Assert-TargetInFilesystem -target $OutputFolder
        }

        Assert-Command MakeCabe.exe
        #Testing for files in source directory
        if((Get-ChildItem -Path $CabFilesFolder).Count -le 0) {
            throw "Path '${CabFilesFolder}' does not contain any files."
        }

        #Testing for valid help file types
        $ValidHelpFileTypes = '.xml', '.txt'
        $HelpFiles = Get-ChildItem -Path $CabFilesFolder -File
        $ValidHelpFiles = $HelpFiles | Where-Object { $_.Extension -in $ValidHelpFileTypes }
        $InvalidHelpFiles = $HelpFiles | Where-Object { $_.Extension -notin $ValidHelpFileTypes }
        if(-not $ValidHelpFiles) {
            throw "No valid help files found in '${CabFilesFolder}'."
        }

        if($InvalidHelpFiles) {
            $InvalidHelpFiles | ForEach-Object {
                    Write-Warning -Message "File '${$_}' is not a valid help file type. Excluding from Cab file."
                }
        }

        ###Get Yaml Metadata here
        $mf = Get-MarkdownModuleFile -Path $LandingPagePath

        $ModuleName = $mf.Metadata[$MODULE_PAGE_MODULE_NAME]
        $Guid = $mf.Metadata[$MODULE_PAGE_GUID]
        $Locale = $mf.Metadata[$MODULE_PAGE_LOCALE]
        $FwLink = $mf.Metadata[$MODULE_PAGE_FW_LINK]
        $HelpVersion = $mf.Metadata[$MODULE_PAGE_HELP_VERSION]
        $AdditionalLocale = $mf.Metadata[$MODULE_PAGE_ADDITIONAL_LOCALE]

        #Create HelpInfo File

        Write-Verbose -Message "Creating cab for '${ModuleName}', with Guid '${Guid}', in Locale '${Locale}'"


        #Building the cabinet file name.
        $cabName = ("{0}_{1}_{2}_HelpContent.cab" -f $ModuleName,$Guid,$Locale)
        $zipName = ("{0}_{1}_{2}_HelpContent.zip" -f $ModuleName,$Guid,$Locale)
        $zipPath = (Join-Path $OutputFolder $zipName)

        #Setting Cab Directives, make a cab is turned on, compression is turned on
        Write-Verbose -Message "Creating Cabinet file directives."
        $DirectiveFile = "dir.dff"
        New-Item -ItemType File -Name $DirectiveFile -Force | Out-Null
        Add-Content $DirectiveFile ".Set Cabinet=on"
        Add-Content $DirectiveFile ".Set Compress=on"
        Add-Content $DirectiveFile ".Set MaxDiskSize=CDROM"

        #Creates an entry in the cab directive file for each file in the source directory (uses FullName to get fuly qualified file path and name)
        foreach($file in $ValidHelpFiles) {
            Add-Content $DirectiveFile ("'" + ($file).FullName +"'" )
            Compress-Archive -DestinationPath $zipPath -Path $file.FullName -Update
        }

        #Making Cab
        Write-Verbose -Message "Creating Cabinet file."
        MakeCab.exe /f $DirectiveFile | Out-Null

        #Naming CabFile
        Write-Verbose -Message "Moving the cabinet file to path '${OutputFolder}'"
        Copy-Item "disk1/1.cab" (Join-Path $OutputFolder $cabName)

        #Remove ExtraFiles created by the cabbing process
        Write-Verbose -Message "Removing unnecessary cabinet file contents."
        Remove-Item "setup.inf" -ErrorAction SilentlyContinue
        Remove-Item "setup.rpt" -ErrorAction SilentlyContinue
        Remove-Item $DirectiveFile -ErrorAction SilentlyContinue
        Remove-Item -Path "disk1" -Recurse -ErrorAction SilentlyContinue

        #Create the HelpInfo Xml
        $outputFiles += MakeHelpInfoXml -ModuleName $ModuleName -GUID $Guid -HelpCulture $Locale -HelpVersion $HelpVersion -URI $FwLink -OutputFolder $OutputFolder

        if($AdditionalLocale) {
            $allLocales = $AdditionalLocale -split ','

            foreach($loc in $allLocales) {
                #Create the HelpInfo Xml for each locale
                $locVersion = $Metadata["$loc Version"]

                if([String]::IsNullOrEmpty($locVersion)) {
                    Write-Warning -Message "No version found for Locale '{$loc}'."
                }
                else {
                    $outputFiles += MakeHelpInfoXml -ModuleName $ModuleName -GUID $Guid -HelpCulture $loc -HelpVersion $locVersion -URI $FwLink -OutputFolder $OutputFolder
                }
            }
        }
        
        $outputFiles
    }
}
