Function validateWorkingProvider 
 {

    if((Get-Location).Drive.Provider.Name -ne 'FileSystem')
    {
        Write-Verbose -Message $LocalizedData.SettingFileSystemProvider
        $AvailableFileSystemDrives = Get-PSDrive | Where-Object {$_.Provider.Name -eq "FileSystem"} | Select-Object Root
        if($AvailableFileSystemDrives.Count -gt 0)
        {
           Set-Location $AvailableFileSystemDrives[0].Root
        }
        else
        {
             throw $LocalizedData.FailedSettingFileSystemProvider
        }
    }

}
