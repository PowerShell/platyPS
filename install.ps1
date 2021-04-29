.\build.ps1
try {
    $destination = $env:PSModulePath -split ";" | Where-Object { $_ -like "*Program Files*" }
    if (-not $destination) { $destination = $env:PSModulePath -split ";" | Select-Object -First 1 }
    Copy-Item "$PSScriptRoot\out\platyPS" -Destination $destination -Recurse
}
catch {
    try {
        Copy-Item "$PSScriptRoot\out\platyPS" -Destination ($env:PSModulePath -split ";" | Select-Object -First 1) -Recurse
    }
    catch {
        Write-Error "Could not install platyPS"
        throw $_
    }
}
