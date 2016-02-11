CHANGELOG
-------------

## 0.3.0

*   Fix issue that cause error in CI scenario https://github.com/PowerShell/platyPS/issues/27
*   Add Get-PlatyPSTextHelpFromMaml command to simplify resulted help review

## 0.2.0

*   Better support for working with big modules:
    *   `Get-PlatyPSMarkdown -OneFilePerCommand -OutputFolder`
    *   `Get-PlatyPSExternalHelp -MarkdownFolder`

## 0.1.0

*   Initial release
*   Commands: `Get-PlatyPSExternalHelp`, `Get-PlatyPSMarkdown`, `New-PlatyPSModuleFromMaml`
*   Schema: Version 1.0.0
