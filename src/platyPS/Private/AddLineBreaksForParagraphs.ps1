Function AddLineBreaksForParagraphs 
{

    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $false, ValueFromPipeline = $true)]
        [string]$text
    )

    begin
    {
        $paragraphs = @()
    }

    process
    {
        $text = $text.Trim()
        $paragraphs += $text
    }

    end
    {
        $paragraphs -join "`r`n`r`n"
    }

}
