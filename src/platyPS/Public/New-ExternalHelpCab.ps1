Function New-ExternalHelpCab 
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
