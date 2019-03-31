<#
    Get a compact string representation from TypeInfo or TypeInfo-like object

    The typeObjectHash api is provided for the remoting support.
    We use two different parameter sets ensure the tupe of -TypeObject
#>
Function GetTypeString 
 {

    param(
        [Parameter(ValueFromPipeline=$true, ParameterSetName='typeObject')]
        [System.Reflection.TypeInfo]
        $TypeObject,

        [Parameter(ValueFromPipeline=$true, ParameterSetName='typeObjectHash')]
        [PsObject]
        $TypeObjectHash
    )

    if ($TypeObject) {
        $TypeObjectHash = $TypeObject
    }

    # special case for nullable value types
    if ($TypeObjectHash.Name -eq 'Nullable`1')
    {
        return $TypeObjectHash.GenericTypeArguments.Name
    }

    if ($TypeObjectHash.IsGenericType)
    {
        # keep information about generic parameters
        return $TypeObjectHash.ToString()
    }

    return $TypeObjectHash.Name

}
