Function IncrementHelpVersion 
{

    param(
        [string]
        $HelpVersionString
    )
    process
    {
        if ($HelpVersionString -eq $LocalizedData.HelpVersion)
        {
            return "1.0.0.0"
        }
        $lastDigitPosition = $HelpVersionString.LastIndexOf(".") + 1
        $frontDigits = $HelpVersionString.Substring(0, $lastDigitPosition)
        $frontDigits += ([int] $HelpVersionString.Substring($lastDigitPosition)) + 1
        return $frontDigits
    }

}
