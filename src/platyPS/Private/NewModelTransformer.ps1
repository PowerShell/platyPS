Function NewModelTransformer 
{

    param(
        [ValidateSet('1.0.0', '2.0.0')]
        [string]$schema,
        [string[]]$ApplicableTag
    )

    if ($schema -eq '1.0.0')
    {
        throw $LocalizedData.PlatyPS100SchemaDeprecated
    }
    elseif ($schema -eq '2.0.0')
    {
        $infoCallback = {
            param([string]$message)
            Write-Verbose $message
        }
        $warningCallback = GetWarningCallback
        return New-Object -TypeName 'Markdown.MAML.Transformer.ModelTransformerVersion2' -ArgumentList ($infoCallback, $warningCallback, $ApplicableTag)
    }

}
