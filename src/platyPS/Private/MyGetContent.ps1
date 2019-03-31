Function MyGetContent 
{

    [OutputType([System.String])]
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path,
        [Parameter(Mandatory = $true)]
        [System.Text.Encoding]$Encoding
    )

    if (-not(Test-Path $Path))
    {
        throw $LocalizedData.FileNotFound
        return
    }
    else
    {
        if (Test-Path $Path -PathType Container)
        {
            throw $LocalizedData.PathIsNotFile
            return
        }
    }

    Write-Verbose -Message ($LocalizedData.ReadingWithEncoding -f $Path, $Encoding.EncodingName)
    $resolvedPath = (Get-ChildItem $Path).FullName
    return [System.IO.File]::ReadAllText($resolvedPath, $Encoding)

}
