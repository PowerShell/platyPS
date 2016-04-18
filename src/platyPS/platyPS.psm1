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

#  .ExternalHelp platyPS.psm1-Help.xml
function Get-PlatyPSMarkdown
{
    [CmdletBinding()]
    [OutputType([string[]])]
    param(
        [Parameter(Mandatory=$true, 
            ValueFromPipeline=$true,
            ParameterSetName="FromModule")]
        [object]$module,

        [Parameter(Mandatory=$true, 
            ValueFromPipeline=$true,
            ParameterSetName="FromCommand")]
        [object]$command,

        [Parameter(Mandatory=$true, 
            ValueFromPipeline=$true,
            ParameterSetName="FromMaml")]
        [string]$maml,

        [switch]$OneFilePerCommand,

        [string]$OutputFolder
    )

    begin
    {
        if ($OneFilePerCommand)
        {
            if (-not $OutputFolder)
            {
                throw 'Specify -OutputFolder parameter, when you use -OneFilePerCommand'
            }
            else 
            {
                mkdir $OutputFolder -ErrorAction SilentlyContinue > $null
            }
        }
        else 
        {
            if ($OutputFolder)
            {
                Write-Warning "-OutputFolder $OutputFolder ignored. Use -OneFilePerCommand with it"
            }
        }
    }

    process
    {
        if ($PSCmdlet.ParameterSetName -eq 'FromMaml')
        {
            if (Test-Path $maml)
            {
                $maml = cat -raw $maml
            }

            if ($OneFilePerCommand)
            {
                Convert-MamlToMarkdown $maml -OutputFolder $OutputFolder
            }
            else 
            {
                Convert-MamlToMarkdown $maml | Out-String    
            }
        }
        elseif ($PSCmdlet.ParameterSetName -eq 'FromCommand')
        {
            $md = Convert-HelpToMarkdown (Get-Help $command)
            if ($OneFilePerCommand)
            {
                Out-MarkdownToFile -path (Join-Path $OutputFolder "$command.md") -value $md
            }
            else 
            {
                $md    
            }
        }
        else # "FromModule"
        {
            $commands = (get-module $module).ExportedCommands.Keys
            $commands | % {
                $command = $_
                $h = Get-Help $module\$command
                $md = Convert-HelpToMarkdown $h
                if ($OneFilePerCommand)
                {
                    Out-MarkdownToFile -path (Join-Path $OutputFolder "$command.md") -value $md
                }
                else 
                {
                    $md    
                }
            } | Out-String
        }
    }
}

#  .ExternalHelp platyPS.psm1-Help.xml
function Get-PlatyPSExternalHelp
{
    [CmdletBinding()]
    [OutputType([string])]
    param(
        [Parameter(Mandatory=$true,
            ParameterSetName="Strings")]
        [string[]]$markdown,

        [Parameter(Mandatory=$true,
            ParameterSetName="MarkdownFolder")]
        [string]$MarkdownFolder,

        [switch]$skipPreambula
    )

    # normalize input
    if ($MarkdownFolder)
    {
        $markdown = ls $MarkdownFolder -File -Filter "*.md" | % {
            cat -Raw $_.FullName
        }
    }

    Add-Type -Path $PSScriptRoot\Markdown.MAML.dll

    $r = new-object -TypeName 'Markdown.MAML.Renderer.MamlRenderer'
    $p = new-object -TypeName 'Markdown.MAML.Parser.MarkdownParser' -ArgumentList {
        param([int]$current, [int]$all) 
        Write-Progress -Activity "Parsing markdown" -status "Progress:" -percentcomplete ($current/$all*100)
    }
    $t = new-object -TypeName 'Markdown.MAML.Transformer.ModelTransformer' -ArgumentList {
        param([string]$message)
        Write-Verbose $message
    }

    $model = $p.ParseString($markdown)
    Write-Progress -Activity "Parsing markdown" -Completed    
    $maml = $t.NodeModelToMamlModel($model)
    $xml = $r.MamlModelToString($maml, [bool]$skipPreambula)

    return $xml
}

#  .ExternalHelp platyPS.psm1-Help.xml
function New-PlatyPSModuleFromMaml
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

function Get-PlatyPSTextHelpFromMaml
{
    param(
        [Parameter(Mandatory=$true)]
        [string]$MamlFilePath,

        [Parameter(Mandatory=$true)]
        [string]$TextOutputPath
    )

    $g = New-PlatyPSModuleFromMaml -MamlFilePath $MamlFilePath

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

function New-PlatyPSCab
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



#
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


function Convert-MamlLinksToMarkDownLinks
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$false, ValueFromPipeline=$true)]
        $maml
    )

    process 
    {

        function Convert([string]$s) 
        {
            ($s -replace "`n", '') -replace '<maml:navigationLink(.*?)><maml:linkText>(.*?)</maml:linkText><maml:uri>(.*?)</maml:uri></maml:navigationLink>', '[$2]($3)'
        }

        if (-not $maml) {
            return $maml
        }
        if ($maml -is [System.Xml.XmlElement]) {
            return ([xml](Convert (Convert-XmlElementToString $maml))).para.'#text'
        }
        if ($maml -is [string]) {
            return $maml
        }
        $maml
        '' # new line for <para>
    }
}

function Get-EscapedMarkdownText
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$false, ValueFromPipeline=$true)]
        [string]$text
    )

    process 
    {
        # this is kind of a crazy replacement to handle escaping properly.
        # we need to do the reverse operation in our markdown parser.
        # the last part is to make generated markdown more readable.
        (((($text -replace '\\\\','\\\\') -replace '([<>])','\$1') -replace '\\([\[\]\(\)])', '\\$1') -replace "\.( )+(\w)", ".`r`n`$2").Trim()
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
        $first = $true
    }

    process 
    {
        if (-not $text)
        {
            return $text
        }

        if ($first) {
            $first = $false
        } else {
            ""
        }

        $text
    }
}

function Get-NameMarkdown($command)
{
    if ($script:IS_HELP)
    {
        $name = $command.Name
    } 
    else 
    {
        $name = $command.details.name.Trim()
    }
    
    "# {0}" -f $name
}

function Get-SynopsisMarkdown($command)
{
    '## SYNOPSIS'
    if ($script:IS_HELP)
    {
        $text = $command.Synopsis
    } 
    else 
    {
        $text = $command.details.description.para | Convert-MamlLinksToMarkDownLinks
    }

    $text | Get-EscapedMarkdownText | Add-LineBreaksForParagraphs
}

function Get-DescriptionMarkdown($command)
{
    '## DESCRIPTION'
    if ($script:IS_HELP)
    {
        $text = $command.description
    } 
    else 
    {
        $text = $command.description.para | Convert-MamlLinksToMarkDownLinks
    }

    $text | Get-EscapedMarkdownText | Add-LineBreaksForParagraphs
}

<#
    .EXAMPLE 
    'SwitchParameter' -> [switch]
    'System.Int32' -> [int] # optional
#>
function Convert-ParameterTypeTextToType
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true, ValueFromPipeline=$true)]
        $parameter
    )

    if ($script:IS_HELP)
    {
        $typeText = $parameter.type.name
    }
    else 
    {
        $typeText = $parameter.parameterValue.'#text'
    }

    # if type is explicitly [object], we want to capture it
    # if type is not specified, then we don't want to capture it
    if (-not $typeText)
    {
        # return nothing
        return ''
    }

    if ($typeText -eq 'SwitchParameter') 
    {
        return '[switch]'
    }

    # default
    return "[$typeText]"
}

function Get-ParamMetadata
{
    param(
        [Parameter(Mandatory=$true)]
        $parameter,
        [Parameter(Mandatory=$false)]
        $paramSet
    )

    $ValidateSetGenerated = $false
    foreach ($setPair in $paramSet.GetEnumerator()) {

        $paramSetName = $setPair.Key
        $syntaxParam = $setPair.Value

        $meta = @()

        if ($syntaxParam.required -eq 'true')
        {
            $meta += 'Mandatory = $true'
        }

        if ($syntaxParam.position -ne 'named')
        {
            $meta += 'Position = ' + ($syntaxParam.position)
        }

        if ($syntaxParam.pipelineInput -eq 'True (ByValue)') {
            $meta += 'ValueFromPipeline = $true'
        } elseif ($syntaxParam.pipelineInput -eq 'True (ByPropertyName)') {
            $meta += 'ValueFromPipelineByPropertyName = $true'
        } elseif ($syntaxParam.pipelineInput -eq 'True (ByPropertyName, ByValue)') {
            # mind the order
            $meta += 'ValueFromPipelineByPropertyName = $true'
            $meta += 'ValueFromPipeline = $true'
        } elseif ($syntaxParam.pipelineInput -eq 'true (ByValue, ByPropertyName)') {
            # mind the order
            $meta += 'ValueFromPipeline = $true'
            $meta += 'ValueFromPipelineByPropertyName = $true'
        } 

        if ($paramSetName -ne '*') {
            $meta += "ParameterSetName = '$paramSetName'"
        }

        if ($meta) {
            # formatting hustle
            if ($meta.Count -eq 1) {
                "[Parameter($meta)]"    
            } else {
                "[Parameter(`n  " + ($meta -join ",`n  ") + ")]"
            }
        }

        if (-not $ValidateSetGenerated) {
            # [ValidateSet()] is a separate attribute from [Parameter()].
            # That means, we cannot specify ValidateSet per parameterSet.
            $validateSet = $syntaxParam.parameterValueGroup.parameterValue.'#text'
            if ($validateSet) {
                "[ValidateSet(`n  '" + ($validateSet -join "',`n  '") + "')]"
                $ValidateSetGenerated = $true
            }
        }
    }
}

function Get-ParameterMarkdown
{
    param(
        [Parameter(Mandatory=$true)]
        $parameter,
        [Parameter(Mandatory=$true)]
        [hashtable]$paramSets
    )

    #if (@('InformationAction', 'InformationVariable') -contains $parameter.name) 
    #{
        # ignoring common parameters
    #    return
    #}

    $parameterType = Convert-ParameterTypeTextToType $parameter
    $defaultValue = '' 
    if ($parameter.defaultValue -and $parameter.defaultValue -ne 'none' -and ($parameterType -ne '[switch]' -or $parameter.defaultValue -ne 'false')) {
        $defaultValue = " = $($parameter.defaultValue)"
    }

@"
### $($parameter.name) $parameterType$defaultValue

"@
    $parameterMetadata = Get-ParamMetadata $parameter -paramSet ($paramSets[$parameter.name]) | Out-String

    if ($parameter.globbing -eq 'true')
    {
        $parameterMetadata += "[SupportsWildCards()]`n"
    }

    if ($parameterMetadata) 
    {
        @"
``````powershell
$parameterMetadata``````

"@
    }

    $parameter.description.para | Convert-MamlLinksToMarkDownLinks | Get-EscapedMarkdownText | Add-LineBreaksForParagraphs
    $parameter.parameters.parameter | Convert-MamlLinksToMarkDownLinks | Get-EscapedMarkdownText
}

<#
.SYNOPSIS Get map 'parameterName' -> ('setName' -> parameterXml) 'Parameter sets that it belongs to -> '. 
    '*' for setName mean it belongs to default set.

Note: for some magical reason this logic works for both Maml xml and Help parameters object.

#>
function Get-ParameterSetMapping($command)
{
    function Simplify($set) 
    {
        foreach ($i in 1..($set.Count-1))
        {
            if (($set[0].required -ne $set[$i].required) -or
                ($set[0].pipelineInput -ne $set[$i].pipelineInput) -or
                ($set[0].position -ne $set[$i].position) -or
                ($set[0].variableLength -ne $set[$i].variableLength))
            {
                return $set
            }
        }

        return [ordered]@{'*' = $set[0]}
    }

    $syntax = $command.syntax
    $result = @{}
    $collection = $syntax.syntaxItem
    
    # If syntax entries are not properly filled up,
    # we are assuming that there is only one parameterSet
    # and take all values for it from parameters section.
    if (-not $collection.parameter) 
    {
        $collection = $command.parameters
    }

    if (-not $collection.parameter) 
    {
        Write-Warning ("No syntax and no parameters entries are found for command " + $command.details.name.Trim())
        return $result
    }

    try 
    {
        $i = 0
        $collection | % {
            $i++
            $paramSetName = "Set $i"
            $_.parameter | % {
                $p = $_
                if ($result[$p.name]) {
                    $result[$p.name][$paramSetName] = $p
                } else {
                    $result[$p.name] = [ordered]@{$paramSetName = $p}
                }
            }
        }

        # at this point, if parameter belongs to all parameter sets, 
        # we should try to remove any notation for parameter sets, if metadata is the same for all of them.
        @($result.Keys) | % {
            if ($i -eq ($result[$_].Count))
            {
                $result[$_] = Simplify $result[$_]
            }
        }
    } 
    catch 
    {
        Write-Warning ("Error processing syntax entries for " + $command.details.name.Trim())
        Write-Error $_
    }

    return $result
}

function Get-ParametersMarkdown($command)
{
    $paramSets = Get-ParameterSetMapping $command

    '## PARAMETERS'
    ''
    $command.parameters.parameter | % { 
        # can be null, if parameters are not populated
        if ($_) {
            Get-ParameterMarkdown $_ -paramSets $paramSets
            ''
        }
    }
}

function Get-InputMarkdown($command)
{

    '## INPUTS'

    if ($command.inputTypes.inputType.type.name)
    {
        # there is a weired difference for some reason
        if ($script:IS_HELP)
        {
            $command.inputTypes.inputType.type.name.Split() | ? {$_} | % {
                "### $($_)"
            }
        }
        else 
        {
            $command.inputTypes.inputType | % { 
                "### $($_.type.name)"
                $_.type.description.para | Convert-MamlLinksToMarkDownLinks | Get-EscapedMarkdownText | Add-LineBreaksForParagraphs
                $_.description.para | Convert-MamlLinksToMarkDownLinks | Get-EscapedMarkdownText | Add-LineBreaksForParagraphs    
            }
        }
    } 
    else 
    {
        "### None"
        $command.inputTypes.inputType.type.description.para | Convert-MamlLinksToMarkDownLinks | Get-EscapedMarkdownText | Add-LineBreaksForParagraphs
    }

}

function Get-OutputMarkdown($command)
{
    '## OUTPUTS'
    if ($command.returnValues.returnValue) 
    {
        if ($script:IS_HELP)
        {
            # didn't test it, but I guess it should be the same as Inputs
            $command.returnValues.returnValue.type.name.Split() | ? {$_} | % {
                "### $($_)"
            }
        }
        else 
        {
            $command.returnValues.returnValue | % { 
                "### $($_.type.name)"    
                $_.type.description.para | Convert-MamlLinksToMarkDownLinks | Get-EscapedMarkdownText | Add-LineBreaksForParagraphs
                $_.description.para | Convert-MamlLinksToMarkDownLinks | Get-EscapedMarkdownText | Add-LineBreaksForParagraphs
            }
        }
    }
    else 
    {
        "### None"
        $command.returnValues.returnValue.type.description.para | Convert-MamlLinksToMarkDownLinks | Get-EscapedMarkdownText | Add-LineBreaksForParagraphs   
    }

}

function Get-NotesMarkdown($command)
{
    if ($command.alertSet.alert.para -or $script:IS_HELP)
    {
        '## NOTES'
        $command.alertSet.alert.para | Convert-MamlLinksToMarkDownLinks | Get-EscapedMarkdownText | Add-LineBreaksForParagraphs
    }
}

function Get-ExampleMarkdown($example)
{
    if ($example.title) {
        "### $($example.title.Trim())"
    } else {
        "### EXAMPLE"
    }

    $example.introduction.para | Convert-MamlLinksToMarkDownLinks | Get-EscapedMarkdownText | Add-LineBreaksForParagraphs
    '```powershell'
    $example.code
    '```'
    $example.remarks.para | Convert-MamlLinksToMarkDownLinks | Get-EscapedMarkdownText | Add-LineBreaksForParagraphs
}

function Get-ExamplesMarkdown($command)
{
    if ($command.examples.example -or $script:IS_HELP)
    {     
        '## EXAMPLES'
        $command.examples.example | ? {$_} | % { Get-ExampleMarkdown $_ }
    }
}

function Get-RelatedLinksMarkdown($command)
{
    '## RELATED LINKS'
    $command.relatedLinks.navigationLink | ? {$_} | % {
        "`n[$($_.linkText)]($($_.uri))"
    }
}

function Convert-CommandToMarkdown
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true, ValueFromPipeline=$true, ParameterSetName="Maml")]
        [System.Xml.XmlElement]$command,

        [Parameter(Mandatory=$true, ValueFromPipeline=$true, ParameterSetName="HelpObject")]
        $HelpObject,

        [Parameter(ParameterSetName="HelpObject")]
        [switch]$IsHelpObject
    )

    if ($IsHelpObject)
    {
        $o = $helpObject
        $script:IS_HELP = $true
    } 
    else 
    {
        $o = $command
    }
    
    Get-NameMarkdown $o
    ''
    Get-SynopsisMarkdown $o
    ''
    Get-DescriptionMarkdown $o
    ''
    Get-ParametersMarkdown $o
    ''
    Get-InputMarkdown $o
    ''
    Get-OutputMarkdown $o
    ''
    Get-NotesMarkdown $o
    ''
    Get-ExamplesMarkdown $o
    ''
    Get-RelatedLinksMarkdown $o
    ''

    if ($IsHelpObject)
    {
        $script:IS_HELP = $false
    }
}

function Convert-XmlElementToString
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true, ValueFromPipeline=$true)]
        $xml
    )

    process
    {
        $sw = New-Object System.IO.StringWriter
        $xmlSettings = New-Object System.Xml.XmlWriterSettings
        $xmlSettings.ConformanceLevel = [System.Xml.ConformanceLevel]::Fragment
        $xmlSettings.Indent = $true
        $xw = [System.Xml.XmlWriter]::Create($sw, $xmlSettings)
        $xml.WriteTo($xw)
        $xw.Close()
        
        # return
        $sw.ToString()
    }
}

function Get-NormalizedText([string]$text)
{
    # just normize some commmon typos
    $text -replace '–','-'
}

function Out-MarkdownToFile
{
    param(
        [string]$Path,
        [string]$value
    )

    Write-Host "Writing to $Path"
    Set-Content -Path $Path -Value $md -Encoding UTF8
}

function Convert-MamlToMarkdown
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string]$maml,

        [string]$OutputFolder
    )

    $xmlMaml = [xml](Get-NormalizedText $maml)
    $commands = $xmlMaml.helpItems.command

    $commands | %{ 
        if ($OutputFolder) 
        {
            $md = Convert-CommandToMarkdown -command $_ 
            $fileName = "$($_.details.name.Trim()).md"
            Out-MarkdownToFile -path (Join-Path $OutputFolder $fileName) -value $md
        }
        else 
        {
            Convert-CommandToMarkdown -command $_    
        }
    } | Out-String
}

function Convert-HelpToMarkdown
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        $helpObject
    )

    Convert-CommandToMarkdown -helpObject $helpObject -IsHelpObject | Out-String
}


# 
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


if ($env:PESTER_EXPORT_ALL_MEMBERS)
{
    Export-ModuleMember -Function *
}
else
{
    Export-ModuleMember -Function @(
        'Get-PlatyPSMarkdown', 
        'Get-PlatyPSExternalHelp', 
        'New-PlatyPSModuleFromMaml', 
        'Get-PlatyPSTextHelpFromMaml',
        'New-PlatyPSCab'
    )
}

