Function GetHelpFileName 
 {

    param(
        [System.Management.Automation.CommandInfo]$CommandInfo
    )

    if ($CommandInfo)
    {
        if ($CommandInfo.HelpFile)
        {
            if ([System.IO.Path]::IsPathRooted($CommandInfo.HelpFile))
            {
                return (Split-Path -Leaf $CommandInfo.HelpFile)
            }
            else
            {
                return $CommandInfo.HelpFile
            }
        }

        # overwise, lets guess it
        $module = @($CommandInfo.Module) + ($CommandInfo.Module.NestedModules) |
            Where-Object {$_.ModuleType -ne 'Manifest'} |
            Where-Object {$_.ExportedCommands.Keys -contains $CommandInfo.Name}

        if (-not $module)
        {
            Write-Warning -Message ($LocalizedData.ModuleNotFoundFromCommand -f '[GetHelpFileName]', $CommandInfo.Name)
            return
        }

        if ($module.Count -gt 1)
        {
            Write-Warning -Message ($LocalizedData.MultipleModulesFoundFromCommand -f '[GetHelpFileName]', $CommandInfo.Name)
            $module = $module | Select-Object -First 1
        }

        if (Test-Path $module.Path -Type Leaf)
        {
            # for regular modules, we can deduct the filename from the module path file
            $moduleItem = Get-Item -Path $module.Path
            if ($moduleItem.Extension -eq '.psm1') {
                $fileName = $moduleItem.BaseName
            } else {
                $fileName = $moduleItem.Name
            }
        }
        else
        {
            # if it's something like Dynamic module,
            # we  guess the desired help file name based on the module name
            $fileName = $module.Name
        }

        return "$fileName-help.xml"
    }

}
