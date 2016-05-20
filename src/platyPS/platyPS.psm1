#region PlatyPS
#  
#   
#   PPPPPPPPPPPPPPPPP   lllllll                           tttt                             PPPPPPPPPPPPPPPPP      SSSSSSSSSSSSSSS
#   P::::::::::::::::P  l:::::l                        ttt:::t                             P::::::::::::::::P   SS:::::::::::::::S
#   P::::::PPPPPP:::::P l:::::l                        t:::::t                             P::::::PPPPPP:::::P S:::::SSSSSS::::::S
#   PP:::::P     P:::::Pl:::::l                        t:::::t                             PP:::::P     P:::::PS:::::S     SSSSSSS
#     P::::P     P:::::P l::::l   aaaaaaaaaaaaa  ttttttt:::::tttttttyyyyyyy           yyyyyyyP::::P     P:::::PS:::::S
#     P::::P     P:::::P l::::l   a::::::::::::a t:::::::::::::::::t y:::::y         y:::::y P::::P     P:::::PS:::::S
#     P::::PPPPPP:::::P  l::::l   aaaaaaaaa:::::at:::::::::::::::::t  y:::::y       y:::::y  P::::PPPPPP:::::P  S::::SSSS
#     P:::::::::::::PP   l::::l            a::::atttttt:::::::tttttt   y:::::y     y:::::y   P:::::::::::::PP    SS::::::SSSSS
#     P::::PPPPPPPPP     l::::l     aaaaaaa:::::a      t:::::t          y:::::y   y:::::y    P::::PPPPPPPPP        SSS::::::::SS
#     P::::P             l::::l   aa::::::::::::a      t:::::t           y:::::y y:::::y     P::::P                   SSSSSS::::S
#     P::::P             l::::l  a::::aaaa::::::a      t:::::t            y:::::y:::::y      P::::P                        S:::::S
#     P::::P             l::::l a::::a    a:::::a      t:::::t    tttttt   y:::::::::y       P::::P                        S:::::S
#   PP::::::PP          l::::::la::::a    a:::::a      t::::::tttt:::::t    y:::::::y      PP::::::PP          SSSSSSS     S:::::S
#   P::::::::P          l::::::la:::::aaaa::::::a      tt::::::::::::::t     y:::::y       P::::::::P          S::::::SSSSSS:::::S
#   P::::::::P          l::::::l a::::::::::aa:::a       tt:::::::::::tt    y:::::y        P::::::::P          S:::::::::::::::SS
#   PPPPPPPPPP          llllllll  aaaaaaaaaa  aaaa         ttttttttttt     y:::::y         PPPPPPPPPP           SSSSSSSSSSSSSSS
#                                                                         y:::::y
#                                                                        y:::::y
#                                                                       y:::::y
#                                                                      y:::::y
#                                                                     yyyyyyy
#   

$script:EXTERNAL_HELP_FILE_YAML_HEADER = 'external help file'
$script:DEFAULT_ENCODING = 'UTF8'
$script:UTF8_NO_BOM = 'UTF8_NO_BOM'
$script:SET_NAME_PLACEHOLDER = 'UNNAMED_PARAMETER_SET'


function New-Markdown
{
    [CmdletBinding()]
    [OutputType([System.IO.FileInfo[]])]
    param(
        [Parameter(Mandatory=$true, 
            ValueFromPipeline=$true,
            ParameterSetName="FromModule")]
        [string]$Module,

        [Parameter(Mandatory=$true, 
            ValueFromPipeline=$true,
            ParameterSetName="FromCommand")]
        [string]$Command,

        [Parameter(Mandatory=$true, 
            ValueFromPipeline=$true,
            ParameterSetName="FromMaml")]
        [string]$MamlFile,

        [hashtable]$Metadata,

        [Parameter( 
            ParameterSetName="FromCommand")]
        [string]$OnlineVersionUrl = '',

        [Parameter(Mandatory=$true)]
        [string]$OutputFolder,

        [switch]$NoMetadata,

        [string]$Encoding = 'UTF8_NO_BOM',

        [Parameter(ParameterSetName="FromModule")]
        [Parameter(ParameterSetName="FromMaml")]
        [switch]$WithModulePage,

        [Parameter(Mandatory=$false,ParameterSetName="FromModule")]
        [Parameter(Mandatory=$false,ParameterSetName="FromMaml")]
        [string]
        $Locale = "en-US",

        [Parameter(Mandatory=$false,ParameterSetName="FromModule")]
        [Parameter(Mandatory=$false,ParameterSetName="FromMaml")]
        [string]
        $HelpVersion = "{{Please enter version of help manually (X.X.X.X) format}}",

        [Parameter(Mandatory=$false,ParameterSetName="FromModule")]
        [Parameter(Mandatory=$false,ParameterSetName="FromMaml")]
        [string]
        $FwLink = "{{Please enter FwLink manually}}",
        
        [Parameter(Mandatory=$false,ParameterSetName="FromMaml")]
        [string]
        $ModuleName = "MamlModule",
        
        [Parameter(Mandatory=$false,ParameterSetName="FromMaml")]
        [string]
        $ModuleGuid = "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX"

    )

    begin
    {
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
                Update-MamlObject $mamlObject -OnlineVersionUrl $OnlineVersionUrl
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

                        $helpFileName = Get-HelpFileName (Get-Command @a)    
                    }
                    
                    $newMetadata = ($Metadata + @{
                        $script:EXTERNAL_HELP_FILE_YAML_HEADER = $helpFileName
                    })
                }

                $md = Convert-MamlModelToMarkdown -mamlCommand $mamlObject -metadata $newMetadata -NoMetadata:$NoMetadata

                Out-MarkdownToFile -path (Join-Path $OutputFolder "$commandName.md") -value $md -Encoding $Encoding
            }
        }

        if ($NoMetadata -and $Metadata)
        {
            throw '-NoMetadata and -Metadata cannot be specified at the same time'
        }

        if ($PSCmdlet.ParameterSetName -eq 'FromCommand')
        {
            if (-not (Get-Command $command -EA SilentlyContinue))
            {
                throw "Command $command not found in the session."
            }

            Get-MamlObject -Cmdlet $command | ProcessMamlObjectToFile
        }
        elseif ($PSCmdlet.ParameterSetName -eq 'FromModule')
        {
            if (-not (Get-Commands -AsNames -module $module))
            {
                throw "Module $module is not imported in the session. Run 'Import-Module $module'."
            }

            Get-MamlObject -Module $module | ProcessMamlObjectToFile
        }
        else # 'FromMaml'
        {
            if (-not (Test-Path $MamlFile))
            {
                throw "No file found in $MamlFile."
            }

            Get-MamlObject -MamlFile $MamlFile | ProcessMamlObjectToFile
        }

        if($WithModulePage)
        {
            if($Module)
            {
                $ModuleName = $module
                $ModuleGuid = (Get-Module $ModuleName).Guid
                $CmdletNames = Get-Commands -AsNames -Module $ModuleName
            }
            else 
            {
                $CmdletNames += Get-MamlObject -MamlFile $MamlFile | % {$_.Name}
            }
            New-ModuleLandingPage  -Path $OutputFolder `
                                   -ModuleName $ModuleName `
                                   -ModuleGuid $ModuleGuid `
                                   -CmdletNames $CmdletNames `
                                   -Locale $Locale `
                                   -Version $HelpVersion `
                                   -FwLink $FwLink `
                                   -Encoding $Encoding
        }
    }
}


function Get-MarkdownMetadata
{
    param(
        [Parameter(Mandatory=$true,
            ParameterSetName="MarkdownPath")]
        [string]$Path,

        [Parameter(Mandatory=$true,
            ParameterSetName="MarkdownContent")]
        [string]$Markdown,

        [Parameter(Mandatory=$true,
            ParameterSetName="MarkdownFileInfo")]
        [System.IO.FileInfo]$FileInfo
    )

    process
    {
        if ($Path)
        {
            $Markdown = Get-Content -Raw $Path
        }

        if ($FileInfo)
        {
            $Markdown = $FileInfo | Get-Content -Raw
        }

        return [Markdown.MAML.Parser.MarkdownParser]::GetYamlMetadata($Markdown)
    }
}


function Update-Markdown
{
    [CmdletBinding()]
    [OutputType([System.IO.FileInfo[]])]
    param(
        [Parameter(Mandatory=$true,
            ValueFromPipeline=$true,
            ParameterSetName='FileUpdate')]
        [Parameter(Mandatory=$true,
            ValueFromPipeline=$true,
            ParameterSetName='SchemaUpgrade')]
        [object[]]$MarkdownFile,

        [Parameter(Mandatory=$true,
            ParameterSetName='SchemaUpgrade')]
        [string]$OutputFolder,

        [Parameter(ParameterSetName='FileUpdate')]
        [Parameter(ParameterSetName='SchemaUpgrade')]
        [Parameter(ParameterSetName='FolderUpdate')]
        [string]$Encoding = $script:UTF8_NO_BOM,

        [Parameter(Mandatory=$true,
            ParameterSetName='SchemaUpgrade')]
        [switch]$SchemaUpgrade,
        
        [string]$LogPath,

        [Parameter(ParameterSetName='FolderUpdate')]
        [string]$Module,
        [Parameter(ParameterSetName='FolderUpdate')]
        [string]$MarkdownFolder
    )

    begin
    {
        $MarkdownFiles = @()
        if ($OutputFolder)
        {
            mkdir $OutputFolder -ErrorAction SilentlyContinue > $null
        }

        if ($LogPath) {
            if (-not (Test-Path $LogPath -PathType Leaf))
            {
                $containerFolder = Split-Path $LogPath
                if ($containerFolder)
                {
                    # this if is for $LogPath -eq foo.log  case
                    mkdir $containerFolder -ErrorAction SilentlyContinue > $null
                }

                # wipe the file, so it can be reused
                Set-Content -Path $LogPath -value '' -Encoding UTF8
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

        $MarkdownFiles += Get-MarkdowFilesFromFolder $MarkdownFolder
    }

    process
    {
        $MarkdownFile | ? {$_} | % {
            if ($_ -is [System.IO.FileInfo])
            {
                $MarkdownFiles += $_
            }
            else 
            {
                # treat as a string
                $MarkdownFiles += ls $_
            }
        }
    }

    end 
    {
        function log
        {
            param(
                [string]$message,
                [switch]$warning
            )

            if ($warning)
            {
                Write-Warning $message
            }

            $infoCallback.Invoke($message)
        }


        function Update-MarkdownFileWithReflection
        {
            param(
                [Parameter(ValueFromPipeline = $true)]
                [System.IO.FileInfo]
                $file
            )


            $filePath = $file.FullName
            $oldMarkdown = cat -Raw $filePath
            $oldModels = Get-MamlModelImpl $oldMarkdown

            if ($oldModels.Count -gt 1)
            {
                log -warning "[Update-Markdown] $filePath contains more then 1 command, skipping upgrade."
                log -warning  "[Update-Markdown] Use 'Update-Markdown -OutputFolder' to convert help to one command per file format first."
                return
            }

            $oldModel = $oldModels[0]

            $name = $oldModel.Name
            $command = Get-Command $name
            if (-not $command)
            {
                log -warning  "[Update-Markdown] command $name not found in the session, skipping upgrade for $filePath"
                return
            }

            # just preserve old metadata
            $metadata = Get-MarkdownMetadata -FileInfo $file
            $reflectionModel = Get-MamlObject -Cmdlet $name

            $merger = New-Object Markdown.MAML.Transformer.MamlModelMerger -ArgumentList $infoCallback
            $newModel = $merger.Merge($reflectionModel, $oldModel)

            $md = Convert-MamlModelToMarkdown -mamlCommand $newModel -metadata $metadata
            Out-MarkdownToFile -path $file.FullName -value $md -Encoding $Encoding # yeild
        }

        if ($PSCmdlet.ParameterSetName -eq 'SchemaUpgrade')
        {
            $markdown = $MarkdownFiles | % {
                cat -Raw $_.FullName
            }

            $model = Get-MamlModelImpl $markdown
            $r = New-Object -TypeName Markdown.MAML.Renderer.MarkdownV2Renderer

            $model | % {
                $name = $_.Name
                # TODO: can we pass some metadata here?
                # skipYamlHeader -eq $false
                $md = $r.MamlModelToString($_, $false)
                $outPath = Join-Path $OutputFolder "$name.md"
                log "[Update-Markdown] Writing updated markdown to $outPath"
                Out-MarkdownToFile -path $outPath -value $md -Encoding $Encoding # yeild
            }
        }
        else # Reflection
        {
            $affectedFiles = $MarkdownFiles | % { Update-MarkdownFileWithReflection $_ }
            
            $affectedFiles # yield 

            if ($PSCmdlet.ParameterSetName -eq 'FolderUpdate')
            {
                $allCommands = Get-Commands -AsNames -Module $Module
                if (-not $allCommands)
                {
                    throw "Module $Module is not imported in the session or doesn't have any exported commands"
                }

                $updatedCommands = $affectedFiles.BaseName
                $allCommands | % {
                    if ( -not ($updatedCommands -contains $_) )
                    {
                        log "[Update-Markdown] Creating new markdown for command $_"
                        $newFiles = New-Markdown -Command $_ -OutputFolder $MarkdownFolder
                        $newFiles # yeild
                    }
                }
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
            ParameterSetName='FromFolder')]
        [string]$MarkdownFolder,

        [Parameter(Mandatory=$true,
            ParameterSetName='FromFile',
            ValueFromPipeline=$true)]
        [System.IO.FileInfo[]]$MarkdownFile,

        [Parameter(Mandatory=$true)]
        [string]$OutputPath,

        [string]$Encoding = $script:DEFAULT_ENCODING
    )

    begin
    {
        if ($PSCmdlet.ParameterSetName -eq 'FromFile')
        {
            $MarkdownFiles = @()
        }

        mkdir $OutputPath -ErrorAction SilentlyContinue > $null
    }

    process
    {
        $MarkdownFiles += $MarkdownFile
    }

    end 
    { 
        $MarkdownFiles += Get-MarkdowFilesFromFolder $MarkdownFolder

        $r = new-object -TypeName 'Markdown.MAML.Renderer.MamlRenderer'
        
        # TODO: this is just a place-holder, we can do better
        $defaultOutputName = 'rename-me-help.xml'
        $groups = $MarkdownFiles | group { 
            $h = Get-MarkdownMetadata -FileInfo $_
            if ($h -and $h[$script:EXTERNAL_HELP_FILE_YAML_HEADER]) 
            {
                Join-Path $OutputPath $h[$script:EXTERNAL_HELP_FILE_YAML_HEADER]
            }
            else 
            {
                $defaultPath = Join-Path $OutputPath $defaultOutputName
                Write-Warning "[New-ExternalHelp] cannot find '$($script:EXTERNAL_HELP_FILE_YAML_HEADER)' in metadata for file $($_.FullName)"
                Write-Warning "[New-ExternalHelp] $defaultPath would be used"
                $defaultPath
            }
        }

        foreach ($group in $groups) {
            $maml = Get-MamlModelImpl ( $group.Group | % { cat -Raw $_.FullName } )
            $xml = $r.MamlModelToString($maml, $false) # skipPreambula is not used
            $outPath = $group.Name # group name
            Write-Verbose "Writing external help to $outPath"
            Set-Content -Path $outPath -Value $xml -Encoding $Encoding
            ls $outPath
        }
    }
}

function Show-HelpPreview
{
    [CmdletBinding()]
    [OutputType([System.IO.FileInfo[]], [MamlCommandHelpInfo])]
    param(
        [Parameter(Mandatory=$true,
            Position=1)]
        [string[]]$MamlFilePath,

        [Parameter(Mandatory=$true,
            ParameterSetName='FileOutput')]
        [string]$TextOutputPath,

        [Parameter(Mandatory=$true,
            ParameterSetName='AsObject')]
        [switch]$AsObject,
        
        [Parameter(ParameterSetName='FileOutput')]
        [string]$Encoding = $script:DEFAULT_ENCODING
    )

    $MamlFilePath | % {
        $g = Get-ModuleFromMaml -MamlFilePath $_

        try 
        {
            Import-Module $g.Path -Force -ea Stop
            $allHelp = $g.Cmdlets | Microsoft.PowerShell.Core\ForEach-Object { 
                $c = $_
                try
                {
                    Microsoft.PowerShell.Core\Get-Help "$($g.Name)\$c" -Full 
                }
                catch 
                {
                    Write-Warning "Exception happens on Get-Help $($g.Name)\$c : $_"
                }
            }

            if ($AsObject)
            {
                $allHelp
            }
            else {
                $allHelp = $allHelp | Microsoft.PowerShell.Utility\Out-String
                Microsoft.PowerShell.Management\Set-Content -Path $TextOutputPath -Value $allHelp -Encoding $Encoding
                Get-ChildItem $TextOutputPath    
            }
        }
        finally
        {
            Microsoft.PowerShell.Core\Remove-Module $g.Name -Force -ea SilentlyContinue
            $moduleDirectory = Split-Path $g.Path
            if (Test-Path $moduleDirectory)
            {
                Remove-Item $moduleDirectory -Force -Recurse
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
        [string] $CmdletContentFolder,
        [parameter(Mandatory=$true)]
        [ValidateScript(
            {
                if(Test-Path $_ -PathType Container)
                {
                    $True
                }
                else
                {
                    Throw "$_ output path is not a valid directory."
                }
            })]
        [string] $OutputPath,
        [parameter(Mandatory=$true)]
        [ValidateScript(
            {
                if(Test-Path $_ -PathType Leaf)
                {
                    $True
                }
                else
                {
                    Throw "$_ Module Page fill path and name are not valid."
                }
            })]
        [string] $ModuleMdPageFullPath
    )
    
    #Testing for MakeCab.exe
    Write-Verbose "Testing that MakeCab.exe is present on this machine."
    $MakeCab = Get-Command MakeCab
    if(-not $MakeCab)
    {
        throw "MakeCab.exe is not a registered command." 
    }
    #Testing for files in source directory
    if((Get-ChildItem -Path $Source).Count -le 0)
    {
        throw "The file count in the content source directory is zero."
    }
    

   ###Get Yaml Metadata here
   $Metadata = Get-MarkdownMetadata -Path $ModuleMdPageFullPath

   $ModuleName = $Metadata["Module Name"]
   $Guid = $Metadata["Module Guid"]
   $Locale = $Metadata["Locale"]
   $FwLink = $Metadata["Download Help Link"]
   $HelpVersion = $Metadata["Help Version"]

   #Create HelpInfo File
   


    #Testing the destination directories, creating if none exists.
    Write-Verbose "Checking the output directory"
    if(-not (Test-Path $OutputPath))
    {
        Write-Verbose "Output directory does not exist, creating a new directory."
        New-Item -ItemType Directory -Path $OutputPath
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
    foreach($file in Get-ChildItem -Path $CmdletContentFolder -File)
    {
        Add-Content $DirectiveFile ("'" + ($file).FullName +"'" )
    }

    #Making Cab
    Write-Verbose "Making the cab file"
    MakeCab.exe /f $DirectiveFile | Out-Null

    #Naming CabFile
    Write-Verbose "Moving the cab to the output directory"
    Copy-Item "disk1/1.cab" (Join-Path $OutputPath $cabName)

    #Remove ExtraFiles created by the cabbing process
    Write-Verbose "Performing cabbing cleanup"
    Remove-Item "setup.inf" -ErrorAction SilentlyContinue
    Remove-Item "setup.rpt" -ErrorAction SilentlyContinue
    Remove-Item $DirectiveFile -ErrorAction SilentlyContinue
    Remove-Item -Path "disk1" -Recurse -ErrorAction SilentlyContinue

    #Create the HelpInfo Xml 
    Make-HelpInfoXml -ModuleName $ModuleName -GUID $Guid -HelpCulture $Locale -HelpVersion $HelpVersion -URI $FwLink -OutputFolder $OutputPath

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

function Get-MarkdowFilesFromFolder
{
    param([string]$MarkdownFolder)

    if ($MarkdownFolder)
    {
        return (Get-ChildItem -File $MarkdownFolder -Filter "*.md")
    }
}

function Get-MamlModelImpl
{
    param(
        [string[]]$markdown
    )

    # we need to pass it into .NET IEnumerable<MamlCommand> API
    $res = New-Object 'System.Collections.Generic.List[Markdown.MAML.Model.MAML.MamlCommand]'

    $markdown | % {
        $schema = Get-SchemaVersion $_
        $p = New-MarkdownParser
        $t = New-ModelTransformer -schema $schema

        $model = $p.ParseString($_)
        Write-Progress -Activity "Parsing markdown" -Completed    
        $maml = $t.NodeModelToMamlModel($model)

        # flatten
        $maml | % { $res.Add($_) }
    }

    return @(,$res)
}

function New-MarkdownParser
{
    return new-object -TypeName 'Markdown.MAML.Parser.MarkdownParser' -ArgumentList {
        param([int]$current, [int]$all) 
        Write-Progress -Activity "Parsing markdown" -status "Progress:" -percentcomplete ($current/$all*100)
    }
}

function New-ModelTransformer
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

function Get-SchemaVersion
{
    param(
        [string]$markdown
    )

    $metadata = Get-MarkdownMetadata -markdown $markdown
    if ($metadata)
    {
        $schema = $metadata['schema']
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

function Update-MamlObject
{
    param(
        [Parameter(Mandatory=$true)]
        [Markdown.MAML.Model.MAML.MamlCommand]$MamlCommandObject,

        [string]$OnlineVersionUrl = $null
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

    # Online Version URL
    if (-not ($MamlCommandObject.Links | ? {$_.LinkName -eq 'Online Version:'} )) {
        $mamlLink = New-Object -TypeName Markdown.MAML.Model.MAML.MamlLink
        $mamlLink.LinkName = 'Online Version:'
        if ($OnlineVersionUrl)
        {
            $mamlLink.LinkUri = $OnlineVersionUrl
        }

        $MamlCommandObject.Links.Add($mamlLink)
    }
}

function Make-HelpInfoXml
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
    $OutputFullPath = Join-Path $OutputFolder ($ModuleName + "_" + $GUID + "_HelpInfo.xml")

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
    $HelpInfoContent.Save($OutputFullPath)

}


function Get-HelpFileName
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
            ? {$_.ModuleType -ne 'Manifest'} | 
            ? {$_.ExportedCommands.Keys -contains $CommandInfo.Name}

        if (-not $module)
        {
            Write-Warning "[Get-HelpFileName] Cannot find module for $($CommandInfo.Name)"
            return
        }

        if ($module.Count -gt 1)
        {
            Write-Warning "[Get-HelpFileName] Found $($module.Count) modules for $($CommandInfo.Name)"
            $module = $module | Select -First 1
        }

        $moduleItem = Get-Item -Path $module.Path
        if ($moduleItem.Extension -eq '.psm1') {
            $fileName = $moduleItem.BaseName
        } else {
            $fileName = $moduleItem.Name
        }

        return "$fileName-help.xml"
    }
}

function Get-ModuleFromMaml
{
    param 
    (
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]
        $MamlFilePath,
        [ValidateNotNullOrEmpty()]
        [String]
        $DestinationPath = "$env:TEMP\Modules"
    )

    if (-not (Test-Path $MamlFilePath))
    {
        throw "'$MamlFilePath' does not exist."        
    }

    # Get the file name
    $originalHelpFileName = (Get-Item $MamlFilePath).Name
    
    # Read the malm file
    $xml = [xml](Get-Content $MamlFilePath -Raw -ea SilentlyContinue)
    if (-not $xml)
    {
        throw "Failed to read '$MamlFilePath'" 
    }

    # The information for the module to be generated
    $currentCulture = (Get-UICulture).Name
    $moduleName = $originalHelpFileName + "_" + (Get-Random).ToString()
    $moduleFolder = "$destinationPath\$moduleName"
    $helpFileFolder = "$destinationPath\$moduleName\$currentCulture"
    $moduleFilePath = $moduleFolder + "\" + $moduleName + ".psm1"

    # The help file will be renamed to this name
    $helpFileNewName = $moduleName + ".psm1-help.xml"

    # The result object to be generated
    $result = @{
        Name = $null
        Cmdlets = @()
        Path = $null
    }

    $writeFile = $false
    $moduleDefintion = ""
    $template = @'

<#
.ExternalHelp $helpFileName
#>
function $cmdletName
{
    [CmdletBinding()]
    Param
    (
        $Param1,
        $Param2
    )
}
'@

    foreach ($command in $xml.helpItems.command.details)
    {
        $thisDefinition = $template
        $thisDefinition = $thisDefinition.Replace("`$helpFileName", $helpFileNewName)
        $thisDefinition = $thisDefinition.Replace("`$cmdletName", $command.name)        
        $moduleDefintion += "`r`n" + $thisDefinition + "`r`n"
        $writeFile = $true
        $result.Cmdlets += $command.name
    }

    if (-not $writeFile)
    {
        Write-Verbose "There aren't any cmdlets definitions on '$MamlFilePath'." -Verbose
        return 
    }

    # Create the module and help content folders.
    #New-Item -Path $moduleFolder -ItemType Directory -Force | Out-Null
    New-Item -Path $helpFileFolder -ItemType Directory -Force | Out-Null

    # Copy the help file
    Copy-Item -Path $MamlFilePath -Destination $helpFileFolder -Force

    # Rename the copied help file
    $filePath = Join-Path $helpFileFolder (Split-Path $MamlFilePath -Leaf)
    Rename-Item -Path $filePath -NewName $helpFileNewName -Force

    # Create the module file
    Set-Content -Value $moduleDefintion -Path $moduleFilePath

    $result.Name = $moduleName
    $result.Path = $moduleFilePath
    return $result
}


function Out-MarkdownToFile
{
    [OutputType([System.IO.FileInfo])]        
    param(
        [string]$Path,
        [string]$value,
        [string]$Encoding
    )

    Write-Verbose "Writing to $Path with encoding $Encoding"
    if ($Encoding -eq $UTF8_NO_BOM)
    {
        # just to create a file
        Set-Content -Path $Path -Value ''
        $Utf8NoBomEncoding = New-Object System.Text.UTF8Encoding -ArgumentList $False
        $Path = (ls $Path).FullName
        [System.IO.File]::WriteAllLines($Path, $value, $Utf8NoBomEncoding)
    }
    else 
    {
        Set-Content -Path $Path -Value $md -Encoding $Encoding
    }

    return (Get-ChildItem $Path)
}

function New-ModuleLandingPage
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
        [string]
        $Encoding
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
        
        $CmdletNames | % {
            $Content += "### [" + $_ + "](" + $_ + ".md)`r`n{{Manually Enter $_ Description Here}}`r`n`r`n"    
        }

        Out-MarkdownToFile -Path $LandingPagePath -value $Content -Encoding $Encoding
    }

}

function Convert-MamlModelToMarkdown
{
    param(
        [ValidateNotNullOrEmpty()]
        [Parameter(Mandatory=$true)]
        [Markdown.MAML.Model.MAML.MamlCommand]$mamlCommand,
        
        [Parameter(Mandatory=$false)]
        [hashtable]$metadata,

        [switch]$NoMetadata
    )

    begin
    {
        $r = New-Object Markdown.MAML.Renderer.MarkdownV2Renderer
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

function Get-Commands
{
    param(
        [Parameter(Mandatory=$true)]
        [string]$Module,
        # return names, instead of objects
        [switch]$AsNames
    )

    # Get-Module doesn't know about Microsoft.PowerShell.Core, so we don't use (Get-Module).ExportedCommands

    # We use: & (dummy module) {...} syntax to workaround
    # the case `Get-MamlObject -Module platyPS`
    # because in this case, we are in the module context and Get-Command returns all commands,
    # not only exported ones.
    $commands = & (New-Module {}) ([scriptblock]::Create("Get-Command -Module $Module"))
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
    and passes it to Convert-PsObjectsToMamlModel, then return results
#>
function Get-MamlObject
{
    Param(
        [CmdletBinding()]
        [parameter(mandatory=$true, parametersetname="Cmdlet")]
        [string] $Cmdlet,
        [parameter(mandatory=$true, parametersetname="Module")]
        [string] $Module,
        [parameter(mandatory=$true, parametersetname="Maml")]
        [string] $MamlFile
    )

    if($Cmdlet)
    {
        Write-Verbose ("Processing: " + $Cmdlet)
        $Help = Get-Help $Cmdlet
        $Command = Get-Command $Cmdlet
        return Convert-PsObjectsToMamlModel -Command $Command -Help $Help
    }
    elseif ($Module)
    {
        Write-Verbose ("Processing: " + $Module)

        $commands = & (New-Module {}) ([scriptblock]::Create("Get-Command -Module $Module"))
        foreach ($Command in $commands)
        {
            Write-Verbose ("`tProcessing: " + $Command.Name)
            $Help = Get-Help $Command.Name
            # yeild
            Convert-PsObjectsToMamlModel -Command $Command -Help $Help
        }
    }
    else # Maml
    {
        $HelpCollection = Show-HelpPreview -MamlFilePath $MamlFile -AsObject

        #Provides Name, CommandType, and Empty Module name from MAML generated module in the $command object.
        #Otherwise loads the results from Get-Command <Cmdlet> into the $command object

        $HelpCollection | % { 
            
            $Help = $_

            $Command = [PsObject] @{
                Name = $Help.Name
                CommandType = $Help.Category
                HelpFile = (Split-Path $MamlFile -Leaf)
            }
            
            # yeild
            Convert-PsObjectsToMamlModel -Command $Command -Help $Help -UseHelpForParametersMetadata
        }
    }
}

function Add-LineBreaksForParagraphs
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
function Convert-PsObjectsToMamlModel
{
    [CmdletBinding()]
    [OutputType([Markdown.MAML.Model.MAML.MamlCommand])]
    param(
        [Parameter(Mandatory=$true)]
        [object]$Command,
        [Parameter(Mandatory=$true)]
        [object]$Help,
        [switch]$UseHelpForParametersMetadata
    )

    function IsCommonParameterName
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

    function GetPipelineValue($Parameter)
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

    function Get-TypeString
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

    #endregion

    $MamlCommandObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlCommand

    #region Command Object Values Processing

    $IsWorkflow = $Command.CommandType -eq 'Workflow'

    #Get Name
    $MamlCommandObject.Name = $Command.Name

    #region Data not provided by the command object
    #Get Synopsis
    #Not provided by the command object.
    $MamlCommandObject.Synopsis = "{{Fill in the Synopsis}}"

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

        $HelpEntry = $Help.parameters.parameter | WHERE {$_.Name -eq $ParameterObject.Name}

        $ParameterObject.DefaultValue = $HelpEntry.defaultValue
        $ParameterObject.VariableLength = $HelpEntry.variableLength -eq 'True'
        $ParameterObject.Globbing = $HelpEntry.globbing -eq 'True'
        $ParameterObject.Position = $HelpEntry.position

        if ($HelpEntry.description) 
        {
            $ParameterObject.Description = $HelpEntry.description.text | Add-LineBreaksForParagraphs
        }

        $syntaxParam = $Help.syntax.syntaxItem.parameter |  ? {$_.Name -eq $Parameter.Name} | Select -First 1
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

            foreach($Parameter in $ParameterSet.Parameters)
            {
                # ignore CommonParameters
                if (IsCommonParameterName $Parameter.Name -Workflow:$IsWorkflow) 
                { 
                    # but don't ignore them, if they have explicit help entries
                    if ($Help.parameters.parameter | ? {$_.Name -eq $Parameter.Name})
                    {
                    }
                    else 
                    {
                        continue
                    } 
                }

                $ParameterObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlParameter

                $ParameterObject.Type = $Parameter.ParameterType | Get-TypeString
                $ParameterObject.Name = $Parameter.Name
                $ParameterObject.Required = $Parameter.IsMandatory
                $ParameterObject.PipelineInput = GetPipelineValue $Parameter

                $ParameterObject.ValueRequired = -not ($Parameter.Type -eq "SwitchParameter") # thisDefinition is a heuristic

                foreach($Alias in $Parameter.Aliases)
                {
                    $ParameterObject.Aliases += $Alias
                }
                
                $ParameterObject.Description = if ([String]::IsNullOrEmpty($Parameter.HelpMessage)) 
                {
                    "{{Fill $($Parameter.Name) Description}}"
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
                $ParameterObject.PipelineInput = $Parameter.pipelineInput

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
    $MamlCommandObject.Synopsis = $Help.Synopsis.Trim()

    #Get Description
    if($Help.description -ne $null)
    {
        $MamlCommandObject.Description = $Help.description.Text | Add-LineBreaksForParagraphs
    }

    #Add to Notes
    #From the Help AlertSet data
    if($help.alertSet)
    {
        $MamlCommandObject.Notes = $help.alertSet.alert.Text | Add-LineBreaksForParagraphs
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

        $RemarkText = $Example.remarks.text | Add-LineBreaksForParagraphs
        
        $MamlExampleObject.Remarks = $RemarkText
        $MamlCommandObject.Examples.Add($MamlExampleObject)
    }

    #Get Inputs
    #Reccomend adding a Parameter Name and Parameter Set Name to each input object.
    #region Inputs
    $Inputs = @()
    
    $Help.inputTypes.inputType | % {
        $InputObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlInputOutput
        $InputObject.TypeName = $_.type.name
        $InputObject.Description = $_.description.Text | Add-LineBreaksForParagraphs
        $Inputs += $InputObject
    }
    
    foreach($Input in $Inputs) {$MamlCommandObject.Inputs.Add($Input)}
 
    #endregion
 
    #Get Outputs
    #No Output Type description is provided from the command object.
    #region Outputs
    $Outputs = @()
    
    $Help.returnValues.returnValue | % {
        $OutputObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlInputOutput
        $OutputObject.TypeName = $_.type.name
        $OutputObject.Description = $_.description.Text | Add-LineBreaksForParagraphs
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

        $defaultSyntax = $MamlCommandObject.Syntax | ? { $Command.DefaultParameterSet -eq $_.ParameterSetName }
        # default syntax should have a priority
        $syntaxes = @($defaultSyntax) + $MamlCommandObject.Syntax

        foreach ($s in $syntaxes) 
        {
            $param = $s.Parameters | ? { $_.Name -eq $Name }
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
#endregion

#region Parameter Auto Completers
# Completers are storted in .\psplaty.AutoCompleters.ps1

# Add all completers in this IF block to ensure TabExpansionPlusPlus is installed, else there is no -Description Param in the V5 Register-ArgumentCompleter Command.
if (Get-Command -Name Register-ArgumentCompleter -Module TabExpansionPlusPlus -ErrorAction Ignore) {
    Get-Item -Path $PSScriptRoot\platyPS.AutoCompleters.ps1 | 
        Resolve-Path |
            ForEach-Object {
                . $_.ProviderPath
            }

    Register-ArgumentCompleter -CommandName New-Markdown, Update-Markdown -ParameterName Module -ScriptBlock $Function:ModuleNameCompletion -Description 'This argument completer handles the -Module parameter of the New-Markdown Command.'
}

#endregion Parameter Auto Completers
