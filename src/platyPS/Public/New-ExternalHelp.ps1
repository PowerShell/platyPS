Function New-ExternalHelp 
{

    [CmdletBinding()]
    [OutputType([System.IO.FileInfo[]])]
    param(
        [Parameter(Mandatory = $true,
            Position = 1,
            ValueFromPipeline = $true,
            ValueFromPipelineByPropertyName = $true)]
        [SupportsWildcards()]
        [string[]]$Path,

        [Parameter(Mandatory = $true)]
        [string]$OutputPath,

        [string[]]$ApplicableTag,

        [System.Text.Encoding]$Encoding = [System.Text.Encoding]::UTF8,

        [ValidateRange(80, [int]::MaxValue)]
        [int] $MaxAboutWidth = 80,

        [string]$ErrorLogFile,

        [switch]$Force,

        [switch]$ShowProgress
    )

    begin
    {
        validateWorkingProvider

        $MarkdownFiles = @()
        $AboutFiles = @()
        $IsOutputContainer = $true
        if ( $OutputPath.EndsWith('.xml') -and (-not (Test-Path -PathType Container $OutputPath )) )
        {
            $IsOutputContainer = $false
            Write-Verbose -Message ($LocalizedData.OutputPathAsFile -f '[New-ExternalHelp]', $OutputPath)
        }
        else
        {
            New-Item -Type Directory $OutputPath -ErrorAction SilentlyContinue > $null
            Write-Verbose -Message ($LocalizedData.OutputPathAsDirectory -f '[New-ExternalHelp]', $OutputPath)
        }

        if ( -not $ShowProgress -or $(Get-Variable -Name IsCoreClr -ValueOnly -ErrorAction SilentlyContinue) )
        {
            Function Write-Progress() { }
        }
    }

    process
    {
        $MarkdownFiles += GetMarkdownFilesFromPath $Path

        if ($MarkdownFiles)
        {
            $AboutFiles += GetAboutTopicsFromPath -Path $Path -MarkDownFilesAlreadyFound $MarkdownFiles.FullName
        }
        else
        {
            $AboutFiles += GetAboutTopicsFromPath -Path $Path
        }
    }

    end
    {
        # Tracks all warnings and errors
        $warningsAndErrors = New-Object System.Collections.Generic.List[System.Object]

        try
        {
            # write verbose output and filter out files based on applicable tag
            $MarkdownFiles | ForEach-Object {
                Write-Verbose -Message ($LocalizedData.InputMarkdownFile -f '[New-ExternalHelp]', $_)
            }

            if ($ApplicableTag)
            {
                Write-Verbose -Message ($LocalizedData.FilteringForApplicableTag -f '[New-ExternalHelp]', $ApplicableTag)
                $MarkdownFiles = $MarkdownFiles | ForEach-Object {
                    $applicableList = GetApplicableList -Path $_.FullName
                    # this Compare-Object call is getting the intersection of two string[]
                    if ((-not $applicableList) -or (Compare-Object $applicableList $ApplicableTag -IncludeEqual -ExcludeDifferent))
                    {
                        # yeild
                        $_
                    }
                    else
                    {
                        Write-Verbose -Message ($LocalizedData.SkippingMarkdownFile -f '[New-ExternalHelp]', $_)
                    }
                }
            }

            # group the files based on the output xml path metadata tag
            if ($IsOutputContainer)
            {
                $defaultPath = Join-Path $OutputPath $script:DEFAULT_MAML_XML_OUTPUT_NAME
                $groups = $MarkdownFiles | Group-Object {
                    $h = Get-MarkdownMetadata -Path $_.FullName
                    if ($h -and $h[$script:EXTERNAL_HELP_FILE_YAML_HEADER])
                    {
                        Join-Path $OutputPath $h[$script:EXTERNAL_HELP_FILE_YAML_HEADER]
                    }
                    else
                    {
                        $msgLine1 = $LocalizedData.CannotFindInMetadataFile -f $script:EXTERNAL_HELP_FILE_YAML_HEADER, $_.FullName
                        $msgLine2 = $LocalizedData.PathWillBeUsed -f $defaultPath
                        $warningsAndErrors.Add(@{
                                Severity = "Warning"
                                Message  = "$msgLine1 $msgLine2"
                                FilePath = "$($_.FullName)"
                            })

                        Write-Warning -Message "[New-ExternalHelp] $msgLine1"
                        Write-Warning -Message "[New-ExternalHelp] $msgLine2"
                        $defaultPath
                    }
                }
            }
            else
            {
                $groups = $MarkdownFiles | Group-Object { $OutputPath }
            }

            # generate the xml content
            $r = New-Object -TypeName 'Markdown.MAML.Renderer.MamlRenderer'

            foreach ($group in $groups)
            {
                $maml = GetMamlModelImpl ($group.Group | ForEach-Object { $_.FullName }) -Encoding $Encoding -ApplicableTag $ApplicableTag
                $xml = $r.MamlModelToString($maml)

                $outPath = $group.Name # group name
                Write-Verbose -Message ($LocalizedData.WritingExternalHelpToPath -f $outPath)
                MySetContent -Path $outPath -Value $xml -Encoding $Encoding -Force:$Force
            }

            # handle about topics
            if ($AboutFiles.Count -gt 0)
            {
                foreach ($About in $AboutFiles)
                {
                    $r = New-Object -TypeName 'Markdown.MAML.Renderer.TextRenderer' -ArgumentList($MaxAboutWidth)
                    $Content = Get-Content -Raw $About.FullName
                    $p = NewMarkdownParser
                    $model = $p.ParseString($Content)
                    $value = $r.AboutMarkDownToString($model)

                    $outPath = Join-Path $OutputPath ([io.path]::GetFileNameWithoutExtension($About.FullName) + ".help.txt")
                    if (!(Split-Path -Leaf $outPath).ToUpper().StartsWith("ABOUT_", $true, $null))
                    {
                        $outPath = Join-Path (Split-Path -Parent $outPath) ("about_" + (Split-Path -Leaf $outPath))
                    }
                    MySetContent -Path $outPath -Value $value -Encoding $Encoding -Force:$Force
                }
            }
        }
        catch
        {
            # Log error and rethrow
            $warningsAndErrors.Add(@{
                    Severity = "Error"
                    Message  = "$_.Exception.Message"
                    FilePath = ""
                })

            throw
        }
        finally
        {
            if ($ErrorLogFile)
            {
                ConvertTo-Json $warningsAndErrors | Out-File $ErrorLogFile
            }
        }
    }

}
