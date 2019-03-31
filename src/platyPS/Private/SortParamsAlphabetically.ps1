Function SortParamsAlphabetically 
 {

    param(
        [Parameter(Mandatory=$true)]
        $MamlCommandObject
    )

    # sort parameters alphabetically with minor exceptions
    # https://github.com/PowerShell/platyPS/issues/142
    $confirm = $MamlCommandObject.Parameters | Where-Object { $_.Name -eq 'Confirm' }
    $whatif = $MamlCommandObject.Parameters | Where-Object { $_.Name -eq 'WhatIf' }
    $includeTotalCount = $MamlCommandObject.Parameters | Where-Object { $_.Name -eq 'IncludeTotalCount' }
    $skip = $MamlCommandObject.Parameters | Where-Object { $_.Name -eq 'Skip' }
    $first = $MamlCommandObject.Parameters | Where-Object { $_.Name -eq 'First' }

    if ($confirm)
    {
        $MamlCommandObject.Parameters.Remove($confirm) > $null
    }

    if ($whatif)
    {
        $MamlCommandObject.Parameters.Remove($whatif) > $null
    }

    if ($includeTotalCount)
    {
        $MamlCommandObject.Parameters.Remove($includeTotalCount) > $null
    }

    if ($skip)
    {
        $MamlCommandObject.Parameters.Remove($skip) > $null
    }

    if ($first)
    {
        $MamlCommandObject.Parameters.Remove($first) > $null
    }

    $sortedParams = $MamlCommandObject.Parameters | Sort-Object -Property Name
    $MamlCommandObject.Parameters.Clear()

    $sortedParams | ForEach-Object {
        $MamlCommandObject.Parameters.Add($_)
    }

    if ($confirm)
    {
        $MamlCommandObject.Parameters.Add($confirm)
    }

    if ($whatif)
    {
        $MamlCommandObject.Parameters.Add($whatif)
    }

    if ($includeTotalCount)
    {
        $MamlCommandObject.Parameters.Add($includeTotalCount)
    }

    if ($skip)
    {
        $MamlCommandObject.Parameters.Add($skip)
    }

    if ($first)
    {
        $MamlCommandObject.Parameters.Add($first)
    }

}
