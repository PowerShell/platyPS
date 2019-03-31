Function New-MarkdownAboutHelp 
 {

    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string] $OutputFolder,
        [string] $AboutName
    )

    begin
    {
        if ($AboutName.StartsWith('about_')) { $AboutName = $AboutName.Substring('about_'.Length)}
        validateWorkingProvider
        $templatePath =  Join-Path $PSScriptRoot "templates\aboutTemplate.md"
    }

    process
    {
        if(Test-Path $OutputFolder)
        {
            $AboutContent = Get-Content $templatePath
            $AboutContent = $AboutContent.Replace("{{FileNameForHelpSystem}}",("about_" + $AboutName))
            $AboutContent = $AboutContent.Replace("{{TOPIC NAME}}",$AboutName)
            $NewAboutTopic = New-Item -Path $OutputFolder -Name "about_$($AboutName).md"
            Set-Content -Value $AboutContent -Path $NewAboutTopic -Encoding UTF8
        }
        else
        {
            throw $LocalizedData.OutputFolderNotFound
        }
    }

}
