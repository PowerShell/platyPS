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

    }
}

function Get-NameMarkdown($command)
{
@"
##$($command.details.name.Trim())
"@
}

function Get-SynopsisMarkdown($command)
{
@"
###SYNOPSIS
$($command.details.description.para | Convert-MamlLinksToMarkDownLinks)
"@
}

function Get-DescriptionMarkdown($command)
{
@"
###DESCRIPTION
"@
$command.description.para | Convert-MamlLinksToMarkDownLinks
}

function Get-ParameterMarkdown($parameter)
{
    #$parameterType = "\<$($parameter.parameterValue.'#text')\>"
    $parameterType = "$($parameter.parameterValue.'#text')"

    if ($parameter.required -eq 'false') {
        $parameterType = "[$parameterType]"
    }
@"
####$($parameter.name) $parameterType

"@
    $parameter.description.para | Convert-MamlLinksToMarkDownLinks
    $parameter.parameters.parameter | Convert-MamlLinksToMarkDownLinks
}

function Get-ParametersMarkdown($command)
{
@"
###PARAMETERS

"@
    $command.parameters.parameter | % { 
        Get-ParameterMarkdown $_ 
        ''
    }
}

function Get-InputMarkdown($command)
{
@"
###INPUTS
####$($command.inputTypes.inputType.type.name)
"@
$command.inputTypes.inputType.description.para | Convert-MamlLinksToMarkDownLinks
}

function Get-OutputMarkdown($command)
{
@"
###OUTPUTS
####$($command.returnValues.returnValue.type.name)
"@
$command.returnValues.returnValue.description.para | Convert-MamlLinksToMarkDownLinks
}

function Get-NotesMarkdown($command)
{
@"
###NOTES
####$($command.returnValues.returnValue.type.name)
"@
$command.alertSet.alert.para | Convert-MamlLinksToMarkDownLinks
}

function Get-ExampleMarkdown($example)
{
    "#### $($example.title.Trim())"

    $example.introduction.para
    '```'
    $example.code
    '```'
    $example.remarks.para
}

function Get-ExamplesMarkdown($command)
{
@"
###EXAMPLES
"@
$command.examples.example | % { Get-ExampleMarkdown $_ | Convert-MamlLinksToMarkDownLinks }
}

function Get-RelatedLinksMarkdown($command)
{
@"
###RELATED LINKS
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
        $sw = [System.IO.StringWriter]::new()
        $xmlSettings = [System.Xml.XmlWriterSettings]::new()
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

    $commands | %{ Convert-CommandToMarkdown $_ }
}

Export-ModuleMember -Function `
    Convert-MamlToMarkdown, `
    Convert-XmlElementToString, `
    Convert-MamlLinksToMarkDownLinks, `
    Convert-CommandToMarkdown
