Function New-YamlHelp 
{

    [CmdletBinding()]
    [OutputType([System.IO.FileInfo[]])]
    param(
        [Parameter(Mandatory = $true,
            Position = 1,
            ValueFromPipeline = $true,
            ValueFromPipelineByPropertyName = $true)]
        [string[]]$Path,

        [Parameter(Mandatory = $true)]
        [string]$OutputFolder,

        [System.Text.Encoding]$Encoding = [System.Text.Encoding]::UTF8,

        [switch]$Force
    )
    begin
    {
        validateWorkingProvider

        $MarkdownFiles = @()

        if (-not (Test-Path $OutputFolder))
        {
            New-Item -Type Directory $OutputFolder -ErrorAction SilentlyContinue > $null
        }

        if (-not (Test-Path -PathType Container $OutputFolder))
        {
            throw $LocalizedData.PathIsNotFolder -f $OutputFolder
        }
    }
    process
    {
        $MarkdownFiles += GetMarkdownFilesFromPath $Path
    }
    end
    {
        $MarkdownFiles | ForEach-Object {
            Write-Verbose -Message ($LocalizedData.InputMarkdownFile -f '[New-YamlHelp]', $_)
        }

        foreach ($markdownFile in $MarkdownFiles)
        {
            $mamlModels = GetMamlModelImpl $markdownFile.FullName -Encoding $Encoding
            foreach ($mamlModel in $mamlModels)
            {
                $markdownMetadata = Get-MarkdownMetadata -Path $MarkdownFile.FullName

                ## We set the module here in the PowerShell since the Yaml block is not read by the parser
                $mamlModel.ModuleName = $markdownMetadata[$script:MODULE_PAGE_MODULE_NAME]

                $yaml = [Markdown.MAML.Renderer.YamlRenderer]::MamlModelToString($mamlModel)
                $outputFilePath = Join-Path $OutputFolder ($mamlModel.Name + ".yml")
                Write-Verbose -Message ($LocalizedData.WritingYamlToPath -f $outputFilePath)
                MySetContent -Path $outputFilePath -Value $yaml -Encoding $Encoding -Force:$Force
            }
        }
    }

}
