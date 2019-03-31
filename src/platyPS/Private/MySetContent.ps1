Function MySetContent 
{

    [OutputType([System.IO.FileInfo])]
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path,
        [Parameter(Mandatory = $true)]
        [string]$value,
        [Parameter(Mandatory = $true)]
        [System.Text.Encoding]$Encoding,
        [switch]$Force
    )

    if (Test-Path $Path)
    {
        if (Test-Path $Path -PathType Container)
        {
            Write-Error -Message ($LocalizedData.CannotWriteFileDirectoryExists -f $Path)
            return
        }

        if ((MyGetContent -Path $Path -Encoding $Encoding) -eq $value)
        {
            Write-Verbose "Not writing to $Path, because content is not changing."
            return (Get-ChildItem $Path)
        }

        if (-not $Force)
        {
            Write-Error -Message ($LocalizedData.CannotWriteFileWithoutForce -f $Path)
            return
        }
    }
    else
    {
        $dir = Split-Path $Path
        if ($dir)
        {
            New-Item -Type Directory $dir -ErrorAction SilentlyContinue > $null
        }
    }

    Write-Verbose -Message ($LocalizedData.WritingWithEncoding -f $Path, $Encoding.EncodingName)
    # just to create a file
    Set-Content -Path $Path -Value ''
    $resolvedPath = (Get-ChildItem $Path).FullName
    [System.IO.File]::WriteAllText($resolvedPath, $value, $Encoding)
    return (Get-ChildItem $Path)

}
