Function GetOnlineVersion 
{

    param(
        [string]$markdown
    )

    $metadata = Get-MarkdownMetadata -markdown $markdown
    $onlineVersionUrl = $null
    if ($metadata)
    {
        $onlineVersionUrl = $metadata[$script:ONLINE_VERSION_YAML_HEADER]
    }

    return $onlineVersionUrl

}
