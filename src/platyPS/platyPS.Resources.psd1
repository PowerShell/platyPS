# Localized data for en-US

ConvertFrom-StringData -StringData @'
# Errors
NoMetadataAndMetadata = -NoMetadata and -Metadata parameters cannot be specified at the same time.
CommandNotFound = Command '{0}' not found in the session.
ModuleNotFound = Module '{0}' is not imported in the session. Run 'Import-Module {0}'.
FileNotFound = File '{0}' not found.

# Warnings
MoreThanOneGuid = This module has more than one guid. This could impact external help creation.
NoMarkdownFiles = Path '{0}' does not contain any markdown files.
FileContainsMoreThanOneCommand = File '{0}' contains more then one command, skipping upgrade.
OneCommandPerFile = Use 'Update-Markdown -OutputFolder' to convert help to one command per file format first.
CommandNotFoundFileRemoved = Command '{0}' not found in the session, file '{1}' removed.
CommandNotFoundSkippingFile = Command '{0}' not found in the session, skipping upgrade for '{1}'.

# Help Placeholders
HelpVersion = {{Please enter version of help manually (X.X.X.X) format}}
FwLink = {{Please enter FwLink manually}}
ExampleTitle = Example 1
ExampleCode = PS C:\> {{ Add example code here }}
ExampleRemark = {{ Add example description here }}
'@