Function GetInfoCallback 
 {

    param(
        [string]$LogPath,
        [switch]$Append
    )

    if ($LogPath)
    {
        if (-not (Test-Path $LogPath -PathType Leaf))
        {
            $containerFolder = Split-Path $LogPath
            if ($containerFolder)
            {
                # this if is for $LogPath -eq foo.log  case
                New-Item -Type Directory $containerFolder -ErrorAction SilentlyContinue > $null
            }

            if (-not $Append)
            {
                # wipe the file, so it can be reused
                Set-Content -Path $LogPath -value '' -Encoding UTF8
            }
        }

        $infoCallback = {
            param([string]$message)
            Add-Content -Path $LogPath -value $message -Encoding UTF8
        }
    }
    else
    {
        $infoCallback = {
            param([string]$message)
            Write-Verbose $message
        }
    }
    return $infoCallback

}
