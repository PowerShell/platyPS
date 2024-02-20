# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

$ErrorActionPreference = 'Stop'
. $PSScriptRoot/CommonFunction.ps1

Describe 'Get-MarkdownMetadata' {
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
'@
        }

        It 'can read file with relative path' {
            try {
                Push-Location $TestDrive
                $d = Get-MarkdownMetadata "./foo.md"
                $d.Keys | Should -HaveCount 8
            }
            finally {
                Pop-Location
            }
        }

        It 'can parse out yaml snippet' {
            $d = Get-MarkdownMetadata "$TestDrive/foo.md"
            $d.Keys | Should -HaveCount 8
            $d.Keys | Should -BeIn "external help file", "keywords", "Locale", "Module Name", "ms.date", "online version", "schema", "title"
            $d["Locale"] | Should -Be 'en-US'
        }
    }
}
