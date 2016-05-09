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
        if ($PSCmdlet.ParameterSetName -eq 'FromCommand')
        {
            $md = Get-PlatyPSMamlObject -Cmdlet $command | Convert-MamlModelToMarkdown
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
            Get-PlatyPSMamlObject -Module $module | % { 
                $command = $_.Name
                $md = Convert-MamlModelToMarkdown $_
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

function Get-PlatyPSYamlMetadata
{
    param(
        [Parameter(Mandatory=$true)]
        [string]$MarkdownFilePath
    )

    $c = Get-Content -Raw $MarkdownFilePath
    return [Markdown.MAML.Parser.MarkdownParser]::GetYamlMetadata($c)
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
    $t = new-object -TypeName 'Markdown.MAML.Transformer.ModelTransformerVersion1' -ArgumentList {
        param([string]$message)
        Write-Verbose $message
    }

    $model = $p.ParseString($markdown)
    Write-Progress -Activity "Parsing markdown" -Completed    
    $maml = $t.NodeModelToMamlModel($model)
    $xml = $r.MamlModelToString($maml, [bool]$skipPreambula)

    return $xml
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

#endregion

#region PlatyPs Implementation
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

function Get-PlatyPSMamlObject
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
        # the case `Get-PlatyPSMamlObject -Module platyPS`
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


function Out-MarkdownToFile
{
    param(
        [string]$Path,
        [string]$value
    )

    Write-Host "Writing to $Path"
    Set-Content -Path $Path -Value $md -Encoding UTF8
}

function Convert-MamlModelToMarkdown
{
    param(
        [Parameter(ValueFromPipeline=$true, Mandatory=$true)]
        [Markdown.MAML.Model.MAML.MamlCommand]$mamlCommand
    )

    process
    {
        $r = New-Object Markdown.MAML.Renderer.MarkdownV2Renderer
        return $r.MamlModelToString($mamlCommand)
    }
}

function Convert-PsObjectsToMamlModel
{

#Notes for more information required:
   #From the Get-Command and the Get-Help Objects 
    #Still cannot access the values for Value Required
    #Still cannot access the values for ValueVariableLength
    #Still cannot access the values for ParameterValueGroup
   #Might want to update the MAML Model to conatin independant Verb Noun and Command Type entries
   #Might want to update inputs to include a parameter name and a parameter set.

[CmdletBinding()]
[OutputType([Markdown.MAML.Model.MAML.MamlCommand])]
param(
    [Parameter(Mandatory=$true)]
    $CmdletName
)


    function IsCommonParameter($Parameter)
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
        "PipelineVariable") -contains $Parameter.Name
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
$MamlCommandObject.Synopsis = "Not provided by the Get-Command object return."

#Get Description
#Not provided by the command object.
$MamlCommandObject.Description = "Not provided by the Get-Command Object return."

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

    foreach($Parameter in $ParameterSet.Parameters)
    {
        # ignore CommonParameters
        if (IsCommonParameter $Parameter) { continue }

        $ParameterObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlParameter

        $ParameterObject.Type = $Parameter.ParameterType.Name
        $ParameterObject.Name = $Parameter.Name
        $ParameterObject.Required = $Parameter.IsMandatory
        $ParameterObject.Description = "Not provided by the Get-Command return data."
        $ParameterObject.DefaultValue = "Not provided by the Get-Command return data." 
        $ParameterObject.PipelineInput = $Parameter.ValueFromPipeline
        
        foreach($Alias in $Parameter.Aliases)
        {
            $ParameterObject.Aliases += $Alias
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

                $HelpEntryDescription = $null

                foreach($ParameterDescriptionText in $HelpEntry.description)
                {
                    $HelpEntryDescription += $ParameterDescriptionText.text
                }
                
                $Parameter.Description = $HelpEntryDescription
                $Parameter.DefaultValue = $HelpEntry.defaultValue
                $Parameter.VariableLength = $HelpEntry.variableLength
                $Parameter.Globbing = $HelpEntry.globbing
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


Export-ModuleMember -Function @(
    'Get-PlatyPSMarkdown', 
    'Get-PlatyPSYamlMetadata',
    'Get-PlatyPSExternalHelp', 
    'Get-PlatyPSTextHelpFromMaml',
    'New-PlatyPSCab'
)

#endregion
