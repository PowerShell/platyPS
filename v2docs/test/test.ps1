<#
    .SYNOPSIS
        This script is for testing the new PlatyPS module.

    .DESCRIPTION
        This script is for testing the new PlatyPS module.

    .PARAMETER SecretServer
        The URL of the Secret Server instance.
    .PARAMETER Credential
        The credentials to use to authenticate to Secret Server.
    .EXAMPLE
        PS> $session = New-TssSession -SecretServer https://alpha -Credential $ssCred
        PS> Disable-TssSecret -TssSession $session -Id 93

        Disables secret 93
#>
function New-TssSession {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory)]
        [string]$SecretServer,
        [Parameter(Mandatory)]
        [PSCredential]$Credential
    )

    $session = @{
        SecretServer = $SecretServer
        Credential = $Credential
    }

    return $session
}