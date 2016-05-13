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

$script:EXTERNAL_HELP_FILES = 'external help file'

#  .ExternalHelp platyPS.psm1-Help.xml
function New-Markdown
{
    [CmdletBinding()]
    [OutputType([System.IO.FileInfo[]])]
    param(
        [Parameter(Mandatory=$true, 
            ValueFromPipeline=$true,
            ParameterSetName="FromModule")]
        [object]$module,

        [Parameter(Mandatory=$true, 
            ValueFromPipeline=$true,
            ParameterSetName="FromCommand")]
        [object]$command,

        [Parameter(Mandatory=$true)]
        [string]$OutputFolder,

        [string]$Encoding = 'UTF8'
    )

    begin
    {
        mkdir $OutputFolder -ErrorAction SilentlyContinue > $null
    }

    process
    {
        if ($PSCmdlet.ParameterSetName -eq 'FromCommand')
        {
            $md = Get-MamlObject -Cmdlet $command | % {
                $helpFileName = Get-HelpFileName (Get-Command $command)
                Convert-MamlModelToMarkdown -mamlCommand $_ -metadata @{
                    $script:EXTERNAL_HELP_FILES = $helpFileName
                }
            }

            Out-MarkdownToFile -path (Join-Path $OutputFolder "$command.md") -value $md -Encoding $Encoding
        }
        else # "FromModule"
        {
            Get-MamlObject -Module $module | % { 
                $command = $_.Name
                $helpFileName = Get-HelpFileName (Get-Command -Name $command -Module $module)
                $md = Convert-MamlModelToMarkdown -mamlCommand $_ -metadata @{
                    $script:EXTERNAL_HELP_FILES = $helpFileName
                }
                Out-MarkdownToFile -path (Join-Path $OutputFolder "$command.md") -value $md -Encoding $Encoding
            }
        }
    }
}

#  .ExternalHelp platyPS.psm1-Help.xml
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
        [System.IO.FileInfo]$File
    )

    process
    {
        if ($Path)
        {
            $Markdown = Get-Content -Raw $Path
        }

        if ($File)
        {
            $Markdown = $File | Get-Content -Raw
        }

        return [Markdown.MAML.Parser.MarkdownParser]::GetYamlMetadata($Markdown)
    }
}

#  .ExternalHelp platyPS.psm1-Help.xml
function Update-Markdown
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true,
            ParameterSetName='SchemaUpgrade',
            ValueFromPipeline=$true)]
        [System.IO.FileInfo[]]$MarkdownFile,

        [Parameter(Mandatory=$true)]
        [string]$OutputFolder,

        [string]$Encoding = 'UTF8'
    )

    begin
    {
        if ($PSCmdlet.ParameterSetName -eq 'SchemaUpgrade')
        {
            $MarkdownFiles = @()
        }
    }

    process
    {
        $MarkdownFiles += $MarkdownFile
    }

    end 
    {

    }
}

#  .ExternalHelp platyPS.psm1-Help.xml
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
        [string]$OutputFolder,

        [string]$Encoding = 'UTF8'
    )

    begin
    {
        if ($PSCmdlet.ParameterSetName -eq 'FromFile')
        {
            $MarkdownFiles = @()
        }

        mkdir $OutputFolder -ErrorAction SilentlyContinue > $null
    }

    process
    {
        $MarkdownFiles += $MarkdownFile
    }

    end 
    { 
        if ($MarkdownFolder)
        {
            $MarkdownFiles = Get-ChildItem -File $MarkdownFolder -Filter "*.md"
        }

        $markdown = $MarkdownFiles | % {
            cat -Raw $_.FullName
        }

        #Get-MarkdownMetadata

        # metadata would be parsed only from the first one, but we are allowed to be a little bit sloppy here
        $metadata = Get-MarkdownMetadata -markdown ($markdown | Select -first 1)
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

        $r = new-object -TypeName 'Markdown.MAML.Renderer.MamlRenderer'
        $p = new-object -TypeName 'Markdown.MAML.Parser.MarkdownParser' -ArgumentList {
            param([int]$current, [int]$all) 
            Write-Progress -Activity "Parsing markdown" -status "Progress:" -percentcomplete ($current/$all*100)
        }

        if ($schema -eq '1.0.0')
        {
            $t = new-object -TypeName 'Markdown.MAML.Transformer.ModelTransformerVersion1' -ArgumentList {
                param([string]$message)
                Write-Verbose $message
            }
        }
        elseif ($schema -eq '2.0.0')
        {
            $t = new-object -TypeName 'Markdown.MAML.Transformer.ModelTransformerVersion2' -ArgumentList {
                param([string]$message)
                Write-Verbose $message
            }
        }
        else 
        {
            throw "Unknown schema version: $schema"
        }

        # TODO: this is just a place-holder, we can do better
        $defaultOutputName = 'rename-me.psm1-help.xml'
        $groups = $markdown | group { 
            $h = Get-MarkdownMetadata -Markdown $_
            if ($h -and $h.ContainsKey($script:EXTERNAL_HELP_FILES)) 
            {
                $h[$script:EXTERNAL_HELP_FILES]
            }
            else 
            {
                $defaultOutputName
            }
        }

        $groups |  % {
            $model = $p.ParseString($_.Group)
            Write-Progress -Activity "Parsing markdown" -Completed    
            $maml = $t.NodeModelToMamlModel($model)
            $xml = $r.MamlModelToString($maml, $false) # skipPreambula is not used
            $outPath = Join-Path $OutputFolder $_.Name
            Write-Verbose "Writing external help to $outPath"
            Set-Content -Path $outPath -Value $xml -Encoding $Encoding
            ls $outPath
        }
    }
}

#  .ExternalHelp platyPS.psm1-Help.xml
function Show-HelpPreview
{
    param(
        [Parameter(Mandatory=$true)]
        [string]$MamlFilePath,

        [Parameter(Mandatory=$true)]
        [string]$TextOutputPath
    )

    $g = Get-ModuleFromMaml -MamlFilePath $MamlFilePath

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
        } | Microsoft.PowerShell.Utility\Out-String
        Microsoft.PowerShell.Management\Set-Content -Path $TextOutputPath -Value $allHelp -Encoding UTF8
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

#  .ExternalHelp platyPS.psm1-Help.xml
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
                    Throw "$_ source path is not a valid directory."
                }
            })]
        [string] $Source,
        [parameter(Mandatory=$true)]
        [ValidateScript(
            {
                if(Test-Path $_ -PathType Container)
                {
                    $True
                }
                else
                {
                    Throw "$_ source path is not a valid directory."
                }
            })]
        [string] $Destination,
        [parameter(Mandatory=$true)]
        [string] $Module,
        [parameter(Mandatory=$true)]
        [ValidateScript({
                if($_ -match '[a-zA-Z0-9]{8}[-][a-zA-Z0-9]{4}[-][a-zA-Z0-9]{4}[-][a-zA-Z0-9]{4}[-][a-zA-Z0-9]{12}')
                {
                    $true
                }
                else
                {
                    Throw "$_ does not match the valid pattern for a PowerShell module GUID. The GUID consists of letters and numbers in this format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX"
                }
            
            })]
        [string] $Guid = $(throw 'GUID was not a valid format.'),
        [parameter(Mandatory=$true)]
        [ValidateScript(
            {
                if($_ -in ([System.Globalization.CultureInfo]::GetCultures([System.Globalization.CultureTypes]::AllCultures)).Name)
                {
                    $True
                }
                else
                {
                    Throw "$_ is not a valid Locale code in the .Net framework installed."
                }
            })]
        [string] $Locale
    )
    
    #Testing for MakeCab.exe
    Write-Verbose "Testing that MakeCab.exe is present on this machine."
    $MakeCab = Get-Command MakeCab
    if(-not $MakeCab)
    {
        throw "MakeCab.exe is not a registered command." 
    }

    #Testing the source directories.
    Write-Verbose "Checking the source directory."
    if(-not (Test-Path $Source))
    {
        throw "No directory found at the source provided."
    }
    if((Get-ChildItem -Path $Source).Count -le 0)
    {
        throw "The file count in the source directory is zero."
    }
    
    #Testing the destination directories, creating if none exists.
    Write-Verbose "Checking the destination directory"
    if(-not (Test-Path $Destination))
    {
        Write-Verbose "Destination does not exist, creating a new directory."
        New-Item -ItemType Directory -Path $Destination
    }

    Write-Verbose ("Creating cab for {0}, with Guid {1}, in Locale {2}" -f $Module,$Guid,$Locale)

    #Building the cabinet file name.
    $cabName = ("{0}_{1}_{2}_helpcontent.cab" -f $Module,$Guid,$Locale)

    #Setting Cab Directives, make a cab is turned on, compression is turned on
    Write-Verbose "Creating Cab File"
    $DirectiveFile = "dir.dff"
    New-Item -ItemType File -Name $DirectiveFile -Force |Out-Null   
    Add-Content $DirectiveFile ".Set Cabinet=on"
    Add-Content $DirectiveFile ".Set Compress=on"
    
    #Creates an entry in the cab directive file for each file in the source directory (uses FullName to get fuly qualified file path and name)     
    foreach($file in Get-ChildItem -Path $Source -File)
    {
        Add-Content $DirectiveFile ("'" + ($file).FullName +"'" )
    }

    #Making Cab
    Write-Verbose "Making the cab file"
    MakeCab.exe /f $DirectiveFile | Out-Null

    #Naming CabFile
    Write-Verbose "Moving the cab to the destination"
    Copy-Item "disk1/1.cab" (Join-Path $destination $cabName)

    #Remove ExtraFiles created by the cabbing process
    Write-Verbose "Performing file cleanup"
    Remove-Item "setup.inf" -ErrorAction SilentlyContinue
    Remove-Item "setup.rpt" -ErrorAction SilentlyContinue
    Remove-Item $DirectiveFile -ErrorAction SilentlyContinue
    Remove-Item -Path "disk1" -Recurse -ErrorAction SilentlyContinue
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

function Get-MamlObject
{
    Param(
        [CmdletBinding()]
        [parameter(mandatory=$true, parametersetname="Cmdlet")]
        [string] $Cmdlet,
        [parameter(mandatory=$true, parametersetname="Module")]
        [string] $Module
    )

    if($Cmdlet)
    {
        Write-Verbose ("Processing: " + $Cmdlet)

        return Convert-PsObjectsToMamlModel -CmdletName $Cmdlet
    }
    else
    {
        Write-Verbose ("Processing: " + $Module)

        # We use: & (dummy module) {...} syntax to workaround
        # the case `Get-MamlObject -Module platyPS`
        # because in this case, we are in the module context and Get-Command returns all commands,
        # not only exported ones.
        $commands = & (New-Module {}) ([scriptblock]::Create("Get-Command -Module $Module"))
        foreach ($Command in $commands)
        {
            Write-Verbose ("`tProcessing: " + $Command.Name)

            Convert-PsObjectsToMamlModel -CmdletName $Command.Name # yeild
        }
    }
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

        $fileName = Split-Path -Leaf $module.Path
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

    Write-Verbose "Writing to $Path"
    Set-Content -Path $Path -Value $md -Encoding $Encoding
    return Get-ChildItem $Path
}

function Convert-MamlModelToMarkdown
{
    param(
        [Parameter(Mandatory=$true)]
        [Markdown.MAML.Model.MAML.MamlCommand]$mamlCommand,
        
        [Parameter(Mandatory=$false)]
        [hashtable]$metadata
    )

    begin
    {
        $r = New-Object Markdown.MAML.Renderer.MarkdownV2Renderer
        $count = 0
    }

    process
    {
        if (($count++) -eq 0)
        {
            return $r.MamlModelToString($mamlCommand, $metadata)
        }
        else
        {
            return $r.MamlModelToString($mamlCommand, $true) # skip version header
        }
    }
}

function Convert-PsObjectsToMamlModel
{

#Notes for more information required:
   #From the Get-Command and the Get-Help Objects 
    #Still cannot access the values for ValueVariableLength
   #Might want to update the MAML Model to conatin independant Verb Noun and Command Type entries
   #Might want to update inputs to include a parameter name and a parameter set.

[CmdletBinding()]
[OutputType([Markdown.MAML.Model.MAML.MamlCommand])]
param(
    [Parameter(Mandatory=$true)]
    $CmdletName
)


    function IsCommonParameterName($parameterName)
    {
        @("Verbose",
        "Debug",
        "ErrorAction",
        "WarningAction",
        "InformationAction",
        "ErrorVariable",
        "WarningVariable",
        "InformationVariable",
        "OutVariable",
        "OutBuffer",
        "PipelineVariable") -contains $parameterName
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

    # This function matches $ParameterSet object that comes from (Get-Command ...).ParameterSet
    # into syntaxItem object from (Get-Help ...).syntax.syntaxItem
    function GetSyntaxForParameterSet($ParameterSet)
    {
        $commandNames = $ParameterSet.Parameters.Name | ? { -not (IsCommonParameterName $_) }
        # Compare-Object doesn't like $nulls :(
        if (-not $commandNames) { $commandNames = '__NULL' } 
        $help.syntax.syntaxItem | % {
            $helpNames = $_.parameter.Name | ? { -not (IsCommonParameterName $_) }
            if (-not $helpNames) { $helpNames = '__NULL' } 
            if (Compare-Object $helpNames $commandNames) {
                # skip
            }
            else {
                $res = $_
            }
        }

        if ($res) 
        {
            return $res    
        }
        else
        {
            Write-Warning "[Convert-PsObjectsToMamlModel] Cannot find syntaxItem that matches parameter set $($ParameterSet.Name) for $($Help.Name)"
        }
    }

$MamlCommandObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlCommand

$Help = Get-Help $CmdletName
$Command = Get-Command $CmdletName

#####GET THE SYNTAX FROM THE GET-COMMAND $Command OBJECT #####
#region Command Object Processing

#Get Name
$MamlCommandObject.Name = $Command.Name

#region Data not provided by the command object
#Get Synopsis
#Not provided by the command object.
$MamlCommandObject.Synopsis = "{{Fill the Synopsis}}"

#Get Description
#Not provided by the command object.
$MamlCommandObject.Description = "{{Fill the Description}}"

#Get Notes
#Not provided by the command object. Using the Command Type to create a note declaring it's type.
$MamlCommandObject.Notes = "The Cmdlet category is: " + $Command.CommandType + ".`nThe Cmdlet is from the " + $Command.ModuleName + " module. `n`n"


#Get Examples
#Not provided by the command object.

#Get Links
#Not provided by the command object.

#endregion 

#Get Inputs
#Reccomend adding a Parameter Name and Parameter Set Name to each input object.
#region Inputs
$Inputs = @()
foreach($ParameterSet in $Command.ParameterSets)
{
    foreach($Parameter in $ParameterSet.Parameters)
    {
        if($Parameter.ValueFromPipeline -eq "True")
        {
            $InputObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlInputOutput

            $InputObject.TypeName = $Parameter.ParameterType.Name

            $InputObject.Description = $Parameter.Name

            $Inputs += $InputObject
        }
    }   
}

$Inputs = $Inputs | Select -Unique
foreach($Input in $Inputs) {$MamlCommandObject.Inputs.Add($Input)}

#endregion

#Get Outputs
#No Output Type description is provided from the command object.
#region Outputs

$Outputs = @()
foreach($OutputType in $Command.OutputType)
{
    $OutputObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlInputOutput

    $OutputObject.TypeName = $OutputType.Type

    $Outputs += $OutputObject
    
}

$Outputs = $Outputs | Select -Unique
foreach($Output in $Outputs) {$MamlCommandObject.Outputs.Add($Output)}

#endregion

#Get Syntax
#region Get the Syntax Parameter Set objects

foreach($ParameterSet in $Command.ParameterSets)
{
    $SyntaxObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlSyntax

    $SyntaxObject.ParameterSetName = $ParameterSet.Name

    $syntax = GetSyntaxForParameterSet $ParameterSet

    foreach($Parameter in $ParameterSet.Parameters)
    {
        # ignore CommonParameters
        if (IsCommonParameterName $Parameter.Name) 
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

        $ParameterObject.Type = $Parameter.ParameterType.Name
        $ParameterObject.Name = $Parameter.Name
        $ParameterObject.Required = $Parameter.IsMandatory
        $ParameterObject.Description = "{{Fill $($Parameter.Name) Description}}"
        #$ParameterObject.DefaultValue
        $ParameterObject.PipelineInput = GetPipelineValue $Parameter
        
        foreach($Alias in $Parameter.Aliases)
        {
            $ParameterObject.Aliases += $Alias
        }

        # if we didn't find the corresponding HelpEntry, ignore this info
        if ($syntax) 
        {
            $syntaxParam = $syntax.parameter | ? {$_.Name -eq $Parameter.Name}
            foreach ($parameterValue in $syntaxParam.parameterValueGroup.parameterValue)
            {
                $ParameterObject.parameterValueGroup.Add($parameterValue)
            }
        }

        $SyntaxObject.Parameters.Add($ParameterObject)
    }

    $MamlCommandObject.Syntax.Add($SyntaxObject)
}

#endregion


#endregion
##########

#####GET THE HELP-Object Content and add it to the MAML Object#####
#region Help-Object processing

#check to make sure help content exists.
if($Command.HelpFile -ne $null -and $Help -ne $null)
{
    #Get Synopsis
    $MamlCommandObject.Synopsis = $Help.Synopsis

    #Get Description
    if($Help.description -ne $null)
    {
        $MamlCommandObject.Description = ""
        foreach($DescriptionPiece in $Help.description)
        {
            $MamlCommandObject.Description += $DescriptionPiece.Text
            $MamlCommandObject.Description += "`n"
        }
    }

    #Add to Notes
    if($help.alertSet -ne $null)
    {
       foreach($Alert in $Help.alertSet.alert)
        {
            $MamlCommandObject.Notes += $Alert.Text
            $MamlCommandObject.Notes += "`n"
        }
    }

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
    if($Help.examples.example.Count -gt 0)
    {
        foreach($Example in $Help.examples.example)
        {
            $MamlExampleObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlExample

            $MamlExampleObject.Introduction = $Example.introduction
            $MamlExampleObject.Title = $Example.title
            $MamlExampleObject.Code = $Example.code

            $RemarkText = $null
            foreach($Remark in $Example.remarks)
            {
                $RemarkText += $Remark.text + "`n"
            }
            
            $MamlExampleObject.Remarks = $RemarkText
            $MamlCommandObject.Examples.Add($MamlExampleObject)
        }
    }

    #Update Parameters
    if($help.parameters.parameter.Count -gt 0)
    {
        foreach($ParameterSet in $MamlCommandObject.Syntax)
        {
            foreach($Parameter in $ParameterSet.Parameters)
            {
                $HelpEntry = $Help.parameters.parameter | WHERE {$_.Name -eq $Parameter.Name}

                $Parameter.Description = $HelpEntry.description.text
                $Parameter.DefaultValue = $HelpEntry.defaultValue
                $Parameter.VariableLength = $HelpEntry.variableLength -eq 'True'
                $Parameter.ValueRequired = -not ($Parameter.Type -eq "SwitchParameter") # thisDefinition is a heuristic
                $Parameter.Globbing = $HelpEntry.globbing -eq 'True'
                $Parameter.Position = $HelpEntry.position
            }
        }
    }

}
#endregion
##########

#####Adding Parameters Section from Syntax block#####
#region Parameter Unique Selection from Parameter Sets
#This will only work when the Parameters member has a public set as well as a get.

$ParameterArray = @()

foreach($ParameterSet in $MamlCommandObject.Syntax)
{
    foreach($Parameter in $ParameterSet.Parameters)
    {
        $ParameterArray += $Parameter
    }
}


foreach($Parameter in $ParameterArray)
{
    if(($MamlCommandObject.Parameters | WHERE {$_.Name -eq $Parameter.Name}).Count -eq 0)
    {
        $MamlCommandObject.Parameters.Add($Parameter)
    }
}

#endregion
##########

return $MamlCommandObject

}
#endregion

#region PlatyPS Export
# EEEEEEEEEEEEEEEEEEEEEE                                                                                      tttt
# E::::::::::::::::::::E                                                                                   ttt:::t
# E::::::::::::::::::::E                                                                                   t:::::t
# EE::::::EEEEEEEEE::::E                                                                                   t:::::t
#   E:::::E       EEEEEExxxxxxx      xxxxxxxppppp   ppppppppp      ooooooooooo   rrrrr   rrrrrrrrr   ttttttt:::::ttttttt
#   E:::::E              x:::::x    x:::::x p::::ppp:::::::::p   oo:::::::::::oo r::::rrr:::::::::r  t:::::::::::::::::t
#   E::::::EEEEEEEEEE     x:::::x  x:::::x  p:::::::::::::::::p o:::::::::::::::or:::::::::::::::::r t:::::::::::::::::t
#   E:::::::::::::::E      x:::::xx:::::x   pp::::::ppppp::::::po:::::ooooo:::::orr::::::rrrrr::::::rtttttt:::::::tttttt
#   E:::::::::::::::E       x::::::::::x     p:::::p     p:::::po::::o     o::::o r:::::r     r:::::r      t:::::t
#   E::::::EEEEEEEEEE        x::::::::x      p:::::p     p:::::po::::o     o::::o r:::::r     rrrrrrr      t:::::t
#   E:::::E                  x::::::::x      p:::::p     p:::::po::::o     o::::o r:::::r                  t:::::t
#   E:::::E       EEEEEE    x::::::::::x     p:::::p    p::::::po::::o     o::::o r:::::r                  t:::::t    tttttt
# EE::::::EEEEEEEE:::::E   x:::::xx:::::x    p:::::ppppp:::::::po:::::ooooo:::::o r:::::r                  t::::::tttt:::::t
# E::::::::::::::::::::E  x:::::x  x:::::x   p::::::::::::::::p o:::::::::::::::o r:::::r                  tt::::::::::::::t
# E::::::::::::::::::::E x:::::x    x:::::x  p::::::::::::::pp   oo:::::::::::oo  r:::::r                    tt:::::::::::tt
# EEEEEEEEEEEEEEEEEEEEEExxxxxxx      xxxxxxx p::::::pppppppp       ooooooooooo    rrrrrrr                      ttttttttttt
#                                            p:::::p
#                                            p:::::p
#                                           p:::::::p
#                                           p:::::::p
#                                           p:::::::p
#                                           ppppppppp


Export-ModuleMember -Function @(
    'New-Markdown', 
    'Get-MarkdownMetadata',
    'New-ExternalHelp', 
    'Show-HelpPreview',
    'New-ExternalHelpCab',
    'Update-Markdown'
)

#endregion
