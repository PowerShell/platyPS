Function GetSchemaVersion 
{

    param(
        [string]$markdown
    )

    $metadata = Get-MarkdownMetadata -markdown $markdown
    if ($metadata)
    {
        $schema = $metadata[$script:SCHEMA_VERSION_YAML_HEADER]
        if (-not $schema)
        {
            # there is metadata, but schema version is not specified.
            # assume 2.0.0
            $schema = '2.0.0'
        }
    }
    else
    {
        # if there is not metadata, then it's schema version 1.0.0
        $schema = '1.0.0'
    }

    return $schema

}
