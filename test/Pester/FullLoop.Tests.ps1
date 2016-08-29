Set-StrictMode -Version latest
$ErrorActionPreference = 'Stop'

$root = (Resolve-Path $PSScriptRoot\..\..).Path
$outFolder = "$root\out"

Import-Module $outFolder\platyPS -Force

Describe 'Full loop for Add-Member cmdlet' {

    $cmdlet = "Add-Member"
    # run convertion
    $file = New-MarkdownHelp -command $cmdlet -OutputFolder $outFolder -Force -Encoding ([System.Text.Encoding]::UTF8)

    It 'generate correct file name' {
        $file.FullName | Should Be "$outFolder\$cmdlet.md"
    }

    # test -MarkdownFile piping
    $generatedMaml = $file | New-ExternalHelp -Verbose -OutputPath $outFolder -Force

    It 'generate maml as a valid xml' {
        [xml]($generatedMaml | cat -raw) | Should Not Be $null
    }

    $generatedHelpObject = Get-HelpPreview $generatedMaml
    
    $originalHelpObject = Get-Help -Name "Microsoft.PowerShell.Utility\$cmdlet"
    
    Context 'markdown metadata' {
        $h = Get-MarkdownMetadata $file
        It 'online version is available in metadata and metadat is case-insensitive' {
            $h['oNline vErsion'] | Should Be $originalHelpObject.relatedLinks.navigationLink[0].uri
        }
    }

    It 'generate correct Name' {
        $generatedHelpObject.Name | Should Be $originalHelpObject.Name
    }

    It 'generate correct Synopsis' {
        $generatedHelpObject.Synopsis | Should Be $originalHelpObject.Synopsis
    }

    $originalSyntax = $originalHelpObject.syntax.syntaxItem
    $generatedSyntax = $generatedHelpObject.Syntax.syntaxItem

    It 'generate correct Syntax count' {
        $generatedSyntax.Count | Should Be $originalSyntax.Count
        # this check is too strict, we will do it one-by-one
        #($generatedHelpObject.syntax | Out-String) | Should Be ($originalHelpObject.syntax | Out-String)
    }

    It 'generate correct InputObject in syntax' {
        $originalInputObject = $originalSyntax[0].parameter | ? {$_.name -eq 'InputObject'}
        $generatedInputObject = $originalSyntax[0].parameter | ? {$_.name -eq 'InputObject'}
        ($originalInputObject | Out-String) | Should Be ($generatedInputObject | Out-String)
    }

    It 'generate correct description' {
        $generatedHelpObject.description.Count | Should Be $originalHelpObject.description.Count
        0..($generatedHelpObject.description.Count - 1) | % {
            $generatedHelpObject.description[$_].ToString() | Should Be $originalHelpObject.description[$_].ToString()
        }
    }

    It 'generate correct example count' {
        $generatedHelpObject.examples.example.Count | Should Be $originalHelpObject.examples.example.Count
    }

    Context 'examples' {
        # there is unredable character in EXAMPLE 6 in Add-Member -Force
        # this '-' before force could be screwed up
        0..($generatedHelpObject.examples.example.Count - 1) | % {
            It ('generate correct example ' + ($generatedHelpObject.examples.example[$_].title)) -Skip:($_ -eq 5) {
                ($generatedHelpObject.examples.example[$_] | Out-String).TrimEnd() | Should Be ($originalHelpObject.examples.example[$_] | Out-String).TrimEnd()
            }
            #($generatedHelpObject.examples | Out-String) | Should Be ($originalHelpObject.examples | Out-String)
        }
    }

    Context 'parameters' {
        It 'generate correct parameters count' {
            $originalParameters = $originalHelpObject.parameters.parameter
            $generatedHelpObject.parameters.parameter.Count | Should Be $originalParameters.Count
        }

        0..($generatedHelpObject.parameters.parameter.Count - 1) | % {
            $genParam = $generatedHelpObject.parameters.parameter[$_]
            $name = $genParam.name
            if(!$originalHelpObject.parameters.parameter[$_].defaultValue)
            {
                $originalHelpObject.parameters.parameter[$_].defaultValue="None"
            }
            $origParam = $originalHelpObject.parameters.parameter | ? {$_.Name -eq $name}
            # skip because of unclearaty of RequiredValue meaning for
            $skip = @('Value', 'SecondValue', 'InformationVariable', 'InformationAction') -contains $name
            It ('generate correct parameter ' + ($name)) -Skip:$skip {
                ($genParam | Out-String).TrimEnd() | Should Be ($origParam | Out-String).TrimEnd()
            }
        }
    }

    It 'generate correct relatedLinks' {
        ($generatedHelpObject.relatedLinks | Out-String) | Should Be ($originalHelpObject.relatedLinks | Out-String)
    }

    It 'generate correct inputTypes' {
        ($generatedHelpObject.inputTypes.inputType.description | Out-String).Trim() | Should Be (
            $originalHelpObject.inputTypes.inputType.description | Out-String).Trim()
    }

    It 'generate correct returnValues' {
        ($generatedHelpObject.returnValues.returnValue.description | Out-String).Trim() | Should Be (
            $originalHelpObject.returnValues.returnValue.description | Out-String).Trim()
    }

    # TODO: rest of properties!!
}

function OutFileAndStripped
{
    param([string]$path, [string]$content)
    
    $strippedContent = ((((($content) -replace '\[<', '<') -replace '\[<', '<') -replace '>\]', '>') -replace '>\]', '>')
    Set-Content -Path "$path.stripped" -Value $strippedContent
    Set-Content -Path $path -Value $content
}

Describe 'Microsoft.PowerShell.Core (SMA) help' {

    Context 'produce the real help' {
        $textOutputFile = "$outFolder\SMA.original.txt"
        $help = Get-HelpPreview $pshome\en-US\System.Management.Automation.dll-help.xml | Out-String
        OutFileAndStripped -path $textOutputFile -content $help
    }

    # parameters for New-Markdown
    @( 
        
        [psobject]@{
            MamlFile = "$pshome\en-US\System.Management.Automation.dll-help.xml"
            OutputFolder = "$outFolder\sma-maml"
            Force = $true
            ConvertNotesToList = $true
            ConvertDoubleDashLists = $true
        },

        [psobject]@{
            module = "Microsoft.PowerShell.Core"
            OutputFolder = "$outFolder\sma-model"
            Force = $true
        }

    ) | % {

        $newMarkdownArgs = $_
        
        Context "Output SMA into $($newMarkdownArgs.OutputFolder)" {
            $mdFiles = New-MarkdownHelp @newMarkdownArgs
            $IsMaml = (Split-Path -Leaf $newMarkdownArgs.OutputFolder) -eq 'sma-maml'

            It 'transforms Markdown to MAML with no errors' {
                $generatedMaml = $mdFiles | New-ExternalHelp -Verbose -OutputPath $newMarkdownArgs.OutputFolder -Force
                $generatedMaml.Name | Should Be 'System.Management.Automation.dll-help.xml'

                # add artifacts to out
                $textOutputFile = Join-Path $newMarkdownArgs.OutputFolder 'SMA.generated.txt'
                $help = Get-HelpPreview $generatedMaml.FullName | Out-String
                OutFileAndStripped -path $textOutputFile -content $help
            }

            if ($IsMaml)
            {
                It 'generates correct bullet list for NOTES inside Connect-PSSession' {
                    $file = $mdFiles | ? {$_.Name -eq 'Connect-PSSession.md'}
                    $file | Should Not Be $null

                    $content = cat $file

                    ($content | ? {$_.StartsWith('* ')} | measure).Count | Should Be 5
                    ($content | ? {$_.StartsWith('  ')} | measure).Count | Should Be 5
                } 
            }

            # this our regression suite for SMA
            $generatedHelp = Get-HelpPreview (Join-Path $newMarkdownArgs.OutputFolder 'System.Management.Automation.dll-help.xml')
            
            It 'has right number of outputs for Get-Help' {
                $h = $generatedHelp | ? {$_.Name -eq 'Get-Help'}
                ($h.returnValues.returnValue | measure).Count | Should Be 3
            }

            It 'Get-Help has ValidateSet entry in syntax block' {
                $h = $generatedHelp | ? {$_.Name -eq 'Get-Help'}
                if ($IsMaml)
                {
                    # maml doesn't have an entry there
                    $validateString = '-Category {Alias | Cmdlet | Provider | General'
                }
                else 
                {
                    $validateString = '-Category <String[]> {Alias | Cmdlet | Provider | General'    
                }

                ($h.syntax | Out-String).Contains($validateString) | Should Be $true
            }

            It 'has right type for New-PSTransportOption -IdleTimeoutSec' -Skip:$IsMaml {
                $h = $generatedHelp | ? {$_.Name -eq 'New-PSTransportOption'}
                $param = $h.parameters.parameter | ? {$_.Name -eq 'IdleTimeoutSec'}
                $param.type.name | Should Be 'Int32'
            }

            It 'Enter-PSHostProcess first argument is not -AppDomainName in all syntaxes' {
                $h = $generatedHelp | ? {$_.Name -eq 'Enter-PSHostProcess'}
                $h | Should Not BeNullOrEmpty
                $h.syntax.syntaxItem | % {
                    $_.parameter.Name[0] | Should Not Be 'AppDomainName'
                }
            }

            It 'preserve a list in Disconnect-PSSession -OutputBufferingMode' {
                $listItemMark = if ($IsMaml) {'- '} else {'-- '}
                $h = $generatedHelp | ? {$_.Name -eq 'Disconnect-PSSession'}
                $param = $h.parameters.parameter | ? {$_.Name -eq 'OutputBufferingMode'}
                ($param.description | Out-String).Contains("clear.`r`n`r`n`r`n$($listItemMark)Drop. When") | Should Be $true
                ($param.description | Out-String).Contains("discarded.`r`n`r`n`r`n$($listItemMark)None. No") | Should Be $true
            }

            if ($IsMaml)
            {
                It 'add bullet list for Connect-PSSession NOTES' {
                    $h = $generatedHelp | ? {$_.Name -eq 'Connect-PSSession'}
                    (($h.alertSet.alert.Text | Out-String).Split("`n") | ? {$_.StartsWith('* ')} | measure).Count | Should Be 5
                }
            }
            else 
            {
                It 'preserve formatting for Connect-PSSession NOTES' {

                    # We are cheating a little bit here :(
                    function NormalizeEndings
                    {
                        param(
                            [string]$text
                        )

                        $text2 = ($text -replace "`r","")
                        [Regex]::Replace($text2, "(`n *)+", "`n")
                    }

                    $h = $generatedHelp | ? {$_.Name -eq 'Connect-PSSession'}
                    $expected = NormalizeEndings ( (Get-Help Connect-PSSession).alertSet | Out-String )
                    NormalizeEndings ( $h.alertSet | Out-String ) | Should Be $expected 
                }
            }
        }
    }
}
