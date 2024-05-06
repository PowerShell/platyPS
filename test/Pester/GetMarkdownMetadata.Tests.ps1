# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

$ErrorActionPreference = 'Stop'
. $PSScriptRoot/CommonFunction.ps1

Describe 'Get Metadata from CommandHelp' {
    Context 'Simple markdown file' {
        BeforeAll {
            Set-Content -Path "$TestDrive/foo.md" -Value @'
---
external help file: Microsoft.PowerShell.Archive-help.xml
keywords: powershell,cmdlet
Locale: en-US
Module Name: Microsoft.PowerShell.Archive
ms.date: 02/20/2020
online version: https://docs.microsoft.com/powershell/module/microsoft.powershell.archive/compress-archive?view=powershell-7&WT.mc_id=ps-gethelp
schema: 2.0.0
title: Compress-Archive
---

# Get-Thing

## SYNOPSIS
Gets the thing

## SYNTAX

### PSet1 (Default)

```
Get-Thing [<CommonParameters>]
```

## DESCRIPTION

The `Get-Thing` cmdlet get the thing.


## EXAMPLES

### Example 1: Get the thing

In this example, `Get-Thing` gets the thing.

```powershell
Get-Thing
```

```Output
thing
```

## PARAMETERS

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose,
-WarningAction, and -WarningVariable. For more information, see
[about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

no notes

## RELATED LINKS

'@
        }

        It 'can read file with relative path' {
            try {
                Push-Location $TestDrive
                $ch = Import-MarkdownCommandHelp "./foo.md"
                $d = $ch.Metadata
                $d.Keys | Should -HaveCount 9
            }
            finally {
                Pop-Location
            }
        }

        It 'can parse out yaml snippet' {
            $ch = Import-MarkdownCommandHelp "$TestDrive/foo.md"
            $d = $ch.Metadata
            $expectedKeys = @("content type", "external help file", "keywords", "Locale", "Module Name", "ms.date", "HelpUri", "PlatyPS schema version", "title")
            $d.Keys | Should -HaveCount $expectedKeys.Count
            $d.Keys | Should -BeIn $expectedKeys
            $d["Locale"] | Should -Be 'en-US'
        }
    }
}
