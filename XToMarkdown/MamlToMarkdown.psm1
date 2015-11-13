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

function Get-NameMarkdown($command)
{
@"
## $($command.details.name.Trim())
"@
}

function Get-SynopsisMarkdown($command)
{
@"
### SYNOPSIS
$($command.details.description.para | Convert-MamlLinksToMarkDownLinks)
"@
}

function Get-DescriptionMarkdown($command)
{
@"
### DESCRIPTION
"@
$command.description.para | Convert-MamlLinksToMarkDownLinks
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
        [string]
        $typeText
    )

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
        }

        if ($syntaxParam.pipelineInput -eq 'True (ByPropertyName)') {
            $meta += 'ValueFromPipelineByPropertyName = $true'
        }

        if ($paramSetName -ne '*') {
            $meta += "ParameterSetName = '$paramSetName'"
        }

        if ($meta) {
            '[Parameter(' + ($meta -join ', ') + ')]'
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

    $parameterType = "$($parameter.parameterValue.'#text')" | Convert-ParameterTypeTextToType
@"
#### $($parameter.name) $parameterType

"@
    $parameterMetadata = Get-ParamMetadata $parameter -paramSet ($paramSets[$parameter.name]) | Out-String
    if ($parameterMetadata) 
    {
        @"
``````powershell
$parameterMetadata``````

"@
    }

    $parameter.description.para | Convert-MamlLinksToMarkDownLinks
    $parameter.parameters.parameter | Convert-MamlLinksToMarkDownLinks
}

<#
.SYNOPSIS Get map 'parameterName' -> ('setName' -> parameterXml) 'Parameter sets that it belongs to -> '. 
    '*' for setName mean it belongs to default set.
#>
function Get-ParameterSetMapping($syntax)
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

    $result = @{}
    $syntaxCount = $syntax.syntaxItem.Count
    $i = 0
    $syntax.syntaxItem | % {
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

    return $result
}

function Get-ParametersMarkdown($command)
{
    $paramSets = Get-ParameterSetMapping $command.syntax
@"
### PARAMETERS

"@
    $command.parameters.parameter | % { 
        Get-ParameterMarkdown $_ -paramSets $paramSets
        ''
    }
}

function Get-InputMarkdown($command)
{

@"
### INPUTS
"@

if ($command.inputTypes.inputType.type.name)
{
@"
#### $($command.inputTypes.inputType.type.name)
"@
} else 
{
@"
#### None
"@
}

$command.inputTypes.inputType.description.para | Convert-MamlLinksToMarkDownLinks

}

function Get-OutputMarkdown($command)
{
@"
### OUTPUTS
#### $($command.returnValues.returnValue.type.name)
"@
$command.returnValues.returnValue.description.para | Convert-MamlLinksToMarkDownLinks
}

function Get-NotesMarkdown($command)
{
@"
### NOTES
"@
$command.alertSet.alert.para | Convert-MamlLinksToMarkDownLinks
}

function Get-ExampleMarkdown($example)
{
    if ($example.title) {
        "#### $($example.title.Trim())"
    } else {
        "#### EXAMPLE"
    }

    $example.introduction.para
    '```powershell'
    $example.code
    '```'
    $example.remarks.para
}

function Get-ExamplesMarkdown($command)
{
@"
### EXAMPLES
"@
$command.examples.example | % { Get-ExampleMarkdown $_ | Convert-MamlLinksToMarkDownLinks }
}

function Get-RelatedLinksMarkdown($command)
{
@"
### RELATED LINKS
"@
    $command.relatedLinks.navigationLink | % {
        "[$($_.linkText)]($($_.uri))"
    }
}

function Convert-CommandToMarkdown
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true, ValueFromPipeline=$true)]
        [System.Xml.XmlElement]$command
    )
    
    Get-NameMarkdown $command
    ''
    Get-SynopsisMarkdown $command
    ''
    Get-DescriptionMarkdown $command
    ''
    Get-ParametersMarkdown $command
    ''
    Get-InputMarkdown $command
    ''
    Get-OutputMarkdown $command
    ''
    Get-NotesMarkdown $command
    ''
    Get-ExamplesMarkdown $command
    ''
    Get-RelatedLinksMarkdown $command
    ''
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

function Convert-MamlToMarkdown
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string]$maml
    )

    $xmlMaml = [xml]$maml
    $commands = $xmlMaml.helpItems.command

    $commands | %{ Convert-CommandToMarkdown $_ } | Out-String
}


##
## export everything for test purposes
## TODO: scope it to a simple public API.
##

<#
Export-ModuleMember -Function `
    Convert-MamlToMarkdown, `
    Convert-XmlElementToString, `
    Convert-MamlLinksToMarkDownLinks, `
    Convert-CommandToMarkdown
#>