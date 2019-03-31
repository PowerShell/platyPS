Function Get-HelpPreview 
{

    [CmdletBinding()]
    [OutputType('MamlCommandHelpInfo')]
    param(
        [Parameter(Mandatory = $true,
            ValueFromPipeline = $true,
            Position = 1)]
        [SupportsWildcards()]
        [string[]]$Path,

        [switch]$ConvertNotesToList,
        [switch]$ConvertDoubleDashLists
    )

    process
    {
        foreach ($MamlFilePath in $Path)
        {
            if (-not (Test-Path -Type Leaf $MamlFilePath))
            {
                Write-Error -Message ($LocalizedData.FileNotFoundSkipping -f $MamlFilePath)
                continue
            }

            # this is Resolve-Path that resolves mounted drives (i.e. good for tests)
            $MamlFilePath = (Get-ChildItem $MamlFilePath).FullName

            # Read the malm file
            $xml = [xml](Get-Content $MamlFilePath -Raw -ea SilentlyContinue)
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
                            $_.navigationLink = $_.navigationLink | Select-Object -Skip 1
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
