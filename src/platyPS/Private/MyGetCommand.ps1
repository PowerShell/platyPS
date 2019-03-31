<#
    This function proxies Get-Command call.

    In case of the Remote module, we need to jump thru some hoops
    to get the actual Command object with proper fields.
    Remoting doesn't properly serialize command objects, so we need to be creative
    while extracting all the required metadata from the remote session
    See https://github.com/PowerShell/platyPS/issues/338 for historical context.
#>
Function MyGetCommand 
 {

    Param(
        [CmdletBinding()]
        [parameter(mandatory=$true, parametersetname="Cmdlet")]
        [string] $Cmdlet,
        [System.Management.Automation.Runspaces.PSSession]$Session
    )
    # if there is no remoting, just proxy to Get-Command
    if (-not $Session) {
        return Get-Command $Cmdlet
    }

    # Here is the structure that we use in ConvertPsObjectsToMamlModel
    # we fill it up from the remote with some workarounds
    #
    # $Command.CommandType
    # $Command.Name
    # $Command.ModuleName
    # $Command.DefaultParameterSet
    # $Command.CmdletBinding
    # $ParameterSet in $Command.ParameterSets
    #     $ParameterSet.Name
    #     $ParameterSet.IsDefault
    #     $Parameter in $ParameterSet.Parameters
    #         $Parameter.Name
    #         $Parameter.IsMandatory
    #         $Parameter.Aliases
    #         $Parameter.HelpMessage
    #         $Parameter.Type
    #         $Parameter.ParameterType
    #            $Parameter.ParameterType.Name
    #            $Parameter.ParameterType.GenericTypeArguments.Name
    #            $Parameter.ParameterType.IsGenericType
    #            $Parameter.ParameterType.ToString() - we get that for free from expand

    # expand first layer of properties
    function expand([string]$property) {
        Invoke-Command -Session $Session -ScriptBlock {
            Get-Command $using:Cmdlet |
            Select-Object -ExpandProperty $using:property
        }
    }

    # expand second layer of properties on the selected item
    function expand2([string]$property1, [int]$num, [string]$property2) {
        Invoke-Command -Session $Session -ScriptBlock {
            Get-Command $using:Cmdlet |
            Select-Object -ExpandProperty $using:property1 |
            Select-Object -Index $using:num -Wait |
            Select-Object -ExpandProperty $using:property2
        }
    }

    # expand second and 3rd layer of properties on the selected item
    function expand3(
        [string]$property1,
        [int]$num,
        [string]$property2,
        [string]$property3
        ) {
        Invoke-Command -Session $Session -ScriptBlock {
            Get-Command $using:Cmdlet |
            Select-Object -ExpandProperty $using:property1 |
            Select-Object -Index $using:num -Wait |
            Select-Object -ExpandProperty $using:property2 |
            Select-Object -ExpandProperty $using:property3
        }
    }

    function local([string]$property) {
        Get-Command $Cmdlet | select-object -ExpandProperty $property
    }

    # helper function to fill up the parameters metadata
    function getParams([int]$num) {
        # this call we need to fill-up ParameterSets.Parameters.ParameterType with metadata
        $parameterType = expand3 'ParameterSets' $num 'Parameters' 'ParameterType'
        # this call we need to fill-up ParameterSets.Parameters with metadata
        $parameters = expand2 'ParameterSets' $num 'Parameters'
        if ($parameters.Length -ne $parameterType.Length) {
            Write-Error -Message ($LocalizedData.MetadataDoesNotMatchLength -f $Cmdlet)
        }

        foreach ($i in (GetRange $parameters.Length)) {
            $typeObjectHash = New-Object -TypeName pscustomobject -Property @{
                Name = $parameterType[$i].Name
                IsGenericType = $parameterType[$i].IsGenericType
                # almost .ParameterType.GenericTypeArguments.Name
                # TODO: doesn't it worth another round-trip to make it more accurate
                # and query for the Name?
                GenericTypeArguments = @{ Name = $parameterType[$i].GenericTypeArguments }
            }
            Add-Member -Type NoteProperty -InputObject $parameters[$i] -Name 'ParameterTypeName' -Value (GetTypeString -TypeObjectHash $typeObjectHash)
        }
        return $parameters
    }

    # we cannot use the nested properties from this $remote command.
    # ps remoting doesn't serialize all of them properly.
    # but we can use the top-level onces
    $remote = Invoke-Command -Session $Session { Get-Command $using:Cmdlet }

    $psets = expand 'ParameterSets'
    $psetsArray = @()
    foreach ($i in (GetRange ($psets | measure-object).Count)) {
        $parameters = getParams $i
        $psetsArray += @(New-Object -TypeName pscustomobject -Property @{
            Name = $psets[$i].Name
            IsDefault = $psets[$i].IsDefault
            Parameters = $parameters
        })
    }

    $commandHash = @{
        Name = $Cmdlet
        CommandType = $remote.CommandType
        DefaultParameterSet = $remote.DefaultParameterSet
        CmdletBinding = $remote.CmdletBinding
        # for office we cannot get the module name from the remote, grab the local one instead
        ModuleName = local 'ModuleName'
        ParameterSets = $psetsArray
    }

    return New-Object -TypeName pscustomobject -Property $commandHash

}
