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
@"
## $($command.details.name.Trim())
"@
}

function Get-SynopsisMarkdown($command)
{
@"
### SYNOPSIS
$($command.details.description.para | Convert-MamlLinksToMarkDownLinks | Get-EscapedMarkdownText | Add-LineBreaksForParagraphs)
"@
}

function Get-DescriptionMarkdown($command)
{
@"
### DESCRIPTION
"@
$command.description.para | Convert-MamlLinksToMarkDownLinks | Get-EscapedMarkdownText | Add-LineBreaksForParagraphs
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

    $typeText = $parameter.parameterValue.'#text'
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
#### $($parameter.name) $parameterType$defaultValue

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
@"
### PARAMETERS

"@
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

@"
### INPUTS
"@

if ($command.inputTypes.inputType.type.name)
{
    $command.inputTypes.inputType | % { 
        "#### $($_.type.name)"
        $_.type.description.para | Convert-MamlLinksToMarkDownLinks | Get-EscapedMarkdownText | Add-LineBreaksForParagraphs
        $_.description.para | Convert-MamlLinksToMarkDownLinks | Get-EscapedMarkdownText | Add-LineBreaksForParagraphs    
    }
} 
else 
{
    "#### None"
    $command.inputTypes.inputType.type.description.para | Convert-MamlLinksToMarkDownLinks | Get-EscapedMarkdownText | Add-LineBreaksForParagraphs
}

}

function Get-OutputMarkdown($command)
{
@"
### OUTPUTS
"@
if ($command.returnValues.returnValue) 
{
    $command.returnValues.returnValue | % { 
        "#### $($_.type.name)"    
        $_.type.description.para | Convert-MamlLinksToMarkDownLinks | Get-EscapedMarkdownText | Add-LineBreaksForParagraphs
        $_.description.para | Convert-MamlLinksToMarkDownLinks | Get-EscapedMarkdownText | Add-LineBreaksForParagraphs
    }
}
else 
{
    "#### None"
    $command.returnValues.returnValue.type.description.para | Convert-MamlLinksToMarkDownLinks | Get-EscapedMarkdownText | Add-LineBreaksForParagraphs   
}

}

function Get-NotesMarkdown($command)
{

if ($command.alertSet.alert.para)
{
@"
### NOTES
"@
$command.alertSet.alert.para | Convert-MamlLinksToMarkDownLinks | Get-EscapedMarkdownText | Add-LineBreaksForParagraphs
}

}

function Get-ExampleMarkdown($example)
{
    if ($example.title) {
        "#### $($example.title.Trim())"
    } else {
        "#### EXAMPLE"
    }

    $example.introduction.para | Convert-MamlLinksToMarkDownLinks | Get-EscapedMarkdownText | Add-LineBreaksForParagraphs
    '```powershell'
    $example.code
    '```'
    $example.remarks.para | Convert-MamlLinksToMarkDownLinks | Get-EscapedMarkdownText | Add-LineBreaksForParagraphs
}

function Get-ExamplesMarkdown($command)
{

if ($command.examples.example)
{     
@"
### EXAMPLES
"@
$command.examples.example | % { Get-ExampleMarkdown $_ }
}

}

function Get-RelatedLinksMarkdown($command)
{
@"
### RELATED LINKS
"@
    $command.relatedLinks.navigationLink | % {
        "`n[$($_.linkText)]($($_.uri))"
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

function Get-NormalizedText([string]$text)
{
    # just normize some commmon typos
    $text -replace '–','-'
}

function Convert-MamlToMarkdown
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string]$maml
    )

    $xmlMaml = [xml](Get-NormalizedText $maml)
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