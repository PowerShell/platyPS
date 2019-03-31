Function SetOnlineVersionUrlLink 
 {

    param(
        [Parameter(Mandatory=$true)]
        [Markdown.MAML.Model.MAML.MamlCommand]$MamlCommandObject,

        [string]$OnlineVersionUrl = $null
    )

    # Online Version URL
    $currentFirstLink = $MamlCommandObject.Links | Select-Object -First 1

    if ($OnlineVersionUrl -and ((-not $currentFirstLink) -or ($currentFirstLink.LinkUri -ne $OnlineVersionUrl))) {
        $mamlLink = New-Object -TypeName Markdown.MAML.Model.MAML.MamlLink
        $mamlLink.LinkName = $script:MAML_ONLINE_LINK_DEFAULT_MONIKER
        $mamlLink.LinkUri = $OnlineVersionUrl

        # Insert link at the beginning
        $MamlCommandObject.Links.Insert(0, $mamlLink)
    }

}
