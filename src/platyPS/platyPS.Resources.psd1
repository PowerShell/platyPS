# Localized data for en-US

ConvertFrom-StringData -StringData @'
# Log
ModuleNameFromPath = Determined module name for '{0}' as '{1}'.
UpdateDocsForModule = Updating documents for module '{0}' in {1}

# Errors
NoMetadataAndMetadata = -NoMetadata and -Metadata parameters cannot be specified at the same time.
CommandNotFound = Command '{0}' not found in the session.
ModuleNotFound = Module '{0}' is not imported in the session. Use "Import-Module -Name {0}" to import module in the current session.
FileNotFound = File '{0}' not found.
ModuleNameNotFoundFromPath = Cannot determine module name for {0}. Use "New-MarkdownHelp -WithModulePage" to create module help.
ModuleOrCommandNotFound = Module '{0}' is not imported in the session or doesn't have any exported commands.
OutputFolderNotFound = The output folder does not exist.
PathIsNotFolder = Path '{0}' is not a folder.

# Warnings
MoreThanOneGuid = This module has more than one guid. This could impact external help creation.
NoMarkdownFiles = Path '{0}' does not contain any markdown files.
FileContainsMoreThanOneCommand = File '{0}' contains more then one command, skipping upgrade.
OneCommandPerFile = Use 'Update-Markdown -OutputFolder' to convert help to one command per file format first.
CommandNotFoundFileRemoved = Command '{0}' not found in the session, file '{1}' removed.
CommandNotFoundSkippingFile = Command '{0}' not found in the session, skipping upgrade for '{1}'.

# Verbose
InputMarkdownFile = {0} Input markdown file {1}
WritingYamlToPath = Writing Yaml help to path '{0}'

# Help Placeholders
HelpVersion = {{Please enter version of help manually (X.X.X.X) format}}
FwLink = {{Please enter FwLink manually}}
ExampleTitle = Example 1
ExampleCode = PS C:\> {{ Add example code here }}
ExampleRemark = {{ Add example description here }}
'@