Function DescriptionToPara 
{

    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $false, ValueFromPipeline = $true)]
        $description
    )

    process
    {
        # on some old maml modules description uses Tag to store *-bullet-points
        # one example of it is Exchange
        $description.Tag + "" + $description.Text
    }

}
