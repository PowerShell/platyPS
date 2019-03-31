#region PlatyPS

## DEVELOPERS NOTES & CONVENTIONS
##
##  1. Non-exported functions (subroutines) should avoid using
##     PowerShell standard Verb-Noun naming convention.
##     They should use camalCase or PascalCase instead.
##  2. SMALL subroutines, used only from ONE function
##     should be placed inside the parent function body.
##     They should use camalCase for the name.
##  3. LARGE subroutines and subroutines used from MORE THEN ONE function
##     should be placed after the IMPLEMENTATION text block in the middle
##     of this module.
##     They should use PascalCase for the name.
##  4. Add comment "# yeild" on subroutine calls that write values to pipeline.
##     It would help keep code maintainable and simplify ramp up for others.
##

Import-LocalizedData -BindingVariable LocalizedData -FileName platyPS.Resources.psd1

## Script constants

$script:EXTERNAL_HELP_FILE_YAML_HEADER = 'external help file'
$script:ONLINE_VERSION_YAML_HEADER = 'online version'
$script:SCHEMA_VERSION_YAML_HEADER = 'schema'
$script:APPLICABLE_YAML_HEADER = 'applicable'

$script:UTF8_NO_BOM = New-Object System.Text.UTF8Encoding -ArgumentList $False
$script:SET_NAME_PLACEHOLDER = 'UNNAMED_PARAMETER_SET'
# TODO: this is just a place-holder, we can do better
$script:DEFAULT_MAML_XML_OUTPUT_NAME = 'rename-me-help.xml'

$script:MODULE_PAGE_MODULE_NAME = "Module Name"
$script:MODULE_PAGE_GUID = "Module Guid"
$script:MODULE_PAGE_LOCALE = "Locale"
$script:MODULE_PAGE_FW_LINK = "Download Help Link"
$script:MODULE_PAGE_HELP_VERSION = "Help Version"
$script:MODULE_PAGE_ADDITIONAL_LOCALE = "Additional Locale"

$script:MAML_ONLINE_LINK_DEFAULT_MONIKER = 'Online Version:'

# region Load of module functions 
$PublicFunctions = @( Get-ChildItem -Path $PSScriptRoot\public\*.ps1 -ErrorAction SilentlyContinue )
$PrivateFunctions = @( Get-ChildItem -Path $PSScriptRoot\private\*.ps1 -ErrorAction SilentlyContinue )

# Load the separate function files from the private and public folders.
$AllFunctions = $PublicFunctions + $PrivateFunctions
foreach ($function in $AllFunctions)
{
    try
    {
        . $function.Fullname
    }
    catch
    {
        Write-Error -Message "Failed to import function $($function.fullname): $_"
    }
}

# Register-ArgumentCompleter can be provided thru TabExpansionPlusPlus or with V5 inbox module.
# We don't care much which one it is, but the inbox one doesn't have -Description parameter
if (Get-Command -Name Register-ArgumentCompleter -Module TabExpansionPlusPlus -ErrorAction Ignore)
{
    Function ModuleNameCompleter
    {
        Param (
            $commandName,
            $parameterName,
            $wordToComplete,
            $commandAst,
            $fakeBoundParameter
        )

        Get-Module -Name "$wordToComplete*" |
        ForEach-Object {
            New-CompletionResult -CompletionText $_.Name -ToolTip $_.Description
        }
    }

    Register-ArgumentCompleter -CommandName New-MarkdownHelp -ParameterName Module -ScriptBlock $Function:ModuleNameCompleter -Description 'This argument completer handles the -Module parameter of the New-MarkdownHelp Command.'
}
elseif (Get-Command -Name Register-ArgumentCompleter -ErrorAction Ignore)
{
    Function ModuleNameCompleter
    {
        Param (
            $commandName,
            $parameterName,
            $wordToComplete,
            $commandAst,
            $fakeBoundParameter
        )

        Get-Module -Name "$wordToComplete*" |
        ForEach-Object {
            $_.Name
        }
    }

    Register-ArgumentCompleter -CommandName New-MarkdownHelp -ParameterName Module -ScriptBlock $Function:ModuleNameCompleter
}

#endregion Parameter Auto Completers

# Export the public functions
Export-ModuleMember -Function $PublicFunctions.BaseName