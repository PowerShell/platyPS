# Localized data for en-US

ConvertFrom-StringData -StringData @'
# Errors
NoMetadataAndMetadata = -NoMetadata and -Metadata parameters cannot be specified at the same time.
CommandNotFound = Command '{0}' not found in the session.
ModuleNotFound = Module '{0}' is not imported in the session. Run 'Import-Module {0}'.
FileNotFound = File '{0}' not found.
MoreThanOneGuid = This module has more than one guid. This could impact external help creation.

# Help Placeholders
HelpVersion = {{Please enter version of help manually (X.X.X.X) format}}
FwLink = {{Please enter FwLink manually}}
ExampleTitle = Example 1
ExampleCode = PS C:\> {{ Add example code here }}
ExampleRemark = {{ Add example description here }}
'@