Function GetCommands 
 {

    param(
        [Parameter(Mandatory=$true)]
        [string]$Module,
        # return names, instead of objects
        [switch]$AsNames,
        # use Session for remoting support
        [System.Management.Automation.Runspaces.PSSession]$Session
    )

    process {
        # Get-Module doesn't know about Microsoft.PowerShell.Core, so we don't use (Get-Module).ExportedCommands

        # We use: & (dummy module) {...} syntax to workaround
        # the case `GetMamlObject -Module platyPS`
        # because in this case, we are in the module context and Get-Command returns all commands,
        # not only exported ones.
        $commands = & (New-Module {}) ([scriptblock]::Create("Get-Command -Module '$Module'")) |
            Where-Object {$_.CommandType -ne 'Alias'}  # we don't want aliases in the markdown output for a module

        if ($AsNames)
        {
            $commands.Name
        }
        else
        {
            if ($Session) {
                $commands.Name | ForEach-Object {
                    # yeild
                    MyGetCommand -Cmdlet $_ -Session $Session
                }
            } else {
                $commands
            }
        }
    }

}
