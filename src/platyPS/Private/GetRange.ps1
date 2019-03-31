<#
    You cannot just write 0..($n-1) because if $n == 0 you are screwed.
    Hence this helper.
#>
Function GetRange 
 {

    Param(
        [CmdletBinding()]
        [parameter(mandatory=$true)]
        [int]$n
    )
    if ($n -lt 0) {
        throw $LocalizedData.RangeIsLessThanZero -f $n
    }
    if ($n -eq 0) {
        return
    }
    0..($n - 1)

}
