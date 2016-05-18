Set-StrictMode -Version latest
$ErrorActionPreference = 'Stop'

$root = (Resolve-Path $PSScriptRoot\..\..).Path
$outFolder = "$root\out"

Import-Module $outFolder\platyPS -Force

function normalize([string]$text)
{
    $text -replace '–','-'
}

Describe 'Full loop for Add-Member cmdlet' {

    $outOriginalHelp = "$outFolder\Add-Member.original.txt"
    $outGeneratedHelp = "$outFolder\Add-Member.generated.txt"

    It 'creates markdown from Add-Member command' {
        # run convertion
        New-Markdown -Encoding UTF8 -command Add-Member -OutputFolder $outFolder
    }

    # test -MarkdownFile piping
    $generatedMaml = ls $outFolder\Add-Member.md | New-ExternalHelp -Verbose -OutputPath $outFolder

    It 'generate maml as a valid xml' {
        [xml]($generatedMaml | cat -raw) | Should Not Be $null
    }

    try 
    {
        $generatedModule = & (Get-Module platyPS) ([scriptblock]::Create("Get-ModuleFromMaml -MamlFilePath $outFolder\Microsoft.PowerShell.Commands.Utility.dll-help.xml"))
        Import-Module $generatedModule.Path -Force -ea Stop

        foreach ($cmdletName in $generatedModule.Cmdlets)
        {
            $originalHelpContent = normalize (Get-Help -Name "Microsoft.PowerShell.Utility\$cmdletName" -Full | Out-String)
            $generatedHelpContent = Get-Help -Name "$($generatedModule.Name)\$cmdletName" -Full | Out-String
            
            Set-Content -Value $originalHelpContent -Path $outOriginalHelp
            Set-Content -Value $generatedHelpContent -Path $outGeneratedHelp

            It 'generate maml that produce the same text output when used in the help engine' -Skip {
                $originalHelpContent | Should Be $generatedHelpContent
            }

            $originalHelpObject = Get-Help -Name "Microsoft.PowerShell.Utility\$cmdletName"
            # normalize fixes unredable character in EXAMPLE 6 in Add-Member
            $originalHelpObject.examples.example | % {$_.code = normalize $_.code}
            $generatedHelpObject = Get-Help -Name "$($generatedModule.Name)\$cmdletName"

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
                0..($generatedHelpObject.examples.example.Count - 1) | % {
                    It ('generate correct example ' + ($generatedHelpObject.examples.example[$_].title)) {
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
    }
    finally
    {
        Remove-Module $generatedModule.Name -Force -ea SilentlyContinue
        $moduleDirectory = Split-Path $generatedModule.Path
        if (Test-Path $moduleDirectory)
        {
            Remove-Item $moduleDirectory -Force -Recurse
        }
    }
}

$smaOutputFolder = "TestDrive:\SMA"

Describe 'Microsoft.PowerShell.Core (SMA) help' {

    It 'transforms Markdown to MAML with no errors' {

        $module = 'Microsoft.PowerShell.Core'
        $mdFiles = New-Markdown -Encoding UTF8 -module $module -OutputFolder $outFolder

        $generatedMaml = New-ExternalHelp -markdownFile $mdFiles -Verbose -OutputPath $outFolder
        $generatedMaml.Name | Should Be 'System.Management.Automation.dll-help.xml'

        # add artifacts to out
        Show-HelpPreview $generatedMaml.FullName -TextOutputPath $outFolder\SMA.generated.txt
        Show-HelpPreview $pshome\en-US\System.Management.Automation.dll-help.xml -TextOutputPath $outFolder\SMA.original.txt
    }

    # this our regression suite for SMA
    $generatedHelp = Show-HelpPreview $outFolder\System.Management.Automation.dll-help.xml -AsObject

    It 'has right number of outputs for Get-Help' {
        $h = $generatedHelp | ? {$_.Name -eq 'Get-Help'}
        ($h.returnValues.returnValue | measure).Count | Should Be 3
    }

    It 'has right type for New-PSTransportOption -IdleTimeoutSec' {
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
}
