Function GetApplicableList 
 {

    param(
        [Parameter(Mandatory=$true)]
        $Path
    )

    $h = Get-MarkdownMetadata -Path $Path
    if ($h -and $h[$script:APPLICABLE_YAML_HEADER]) {
        return $h[$script:APPLICABLE_YAML_HEADER].Split(',').Trim()
    }

}
