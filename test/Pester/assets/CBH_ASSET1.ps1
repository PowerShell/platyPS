<#
.SYNOPSIS
Adds a file name extension to a supplied name.
.DESCRIPTION
Adds a file name extension to a supplied name.
Takes any strings for the file name or extension.

```powershell
PS> echo 'code block 1'
```

more explanation

```powershell
PS> echo 'code block 2'
```

even more explanations

.PARAMETER Second
Second parameter help description
.OUTPUTS
System.String. Add-Extension returns a string with the extension or file name.
.EXAMPLE
PS C:\> Test-PlatyPSFunction "File"
File.txt
.EXAMPLE
PS C:\> Test-PlatyPSFunction "File" -First "doc"
File.doc
.LINK
http://www.fabrikam.com/extension.html
.LINK
Set-Item
#>

param(
    [Parameter(Mandatory=$true)]
    [String]$MustHave,
    [Switch]$Common,
    [Parameter(ParameterSetName="First", HelpMessage = 'First parameter help description')]
    [string]$First,
    [Parameter(ParameterSetName="Second")]
    [string]$Second
)

Write-Output "Common: $Common - Second: $Second"
