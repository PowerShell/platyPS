Function GetWarningCallback 
 {

    $warningCallback = {
        param([string]$message)
        Write-Warning $message
    }

    return $warningCallback

}
