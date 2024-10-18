# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

Describe 'Import-MarkdownCommandHelp Tests' {
    BeforeAll {
        $assetDir = Join-Path $PSScriptRoot 'assets'
        $badMarkdownPath = Join-Path $assetDir 'bad-commandhelp.md'
        $goodMarkdownPath = Join-Path $assetDir 'get-date.md'
    }

    It 'Should import a valid markdown file' {
        $result = Import-MarkdownCommandHelp -Path $goodMarkdownPath
        $result | Should -Not -BeNullOrEmpty
        $result.ToString() | Should -Be "Get-Date"
    }

    It 'Should throw an error for an invalid markdown file' {
        Import-MarkdownCommandHelp -ErrorVariable BadMarkdown -Path $badMarkdownPath -ErrorAction SilentlyContinue
        $badMarkdown | Should -BeOfType [System.Management.Automation.ErrorRecord]
        $badMarkdown.FullyQualifiedErrorId | Should -Be "FailedToImportMarkdown,Microsoft.PowerShell.PlatyPS.ImportMarkdownHelpCommand"
    }

    Context 'Validate metadata of the imported commandhelp object' {
        BeforeAll {
            $result = Import-MarkdownCommandHelp -Path $goodMarkdownPath
            $metadata = $result.GetType().GetProperty('Metadata', [System.Reflection.BindingFlags]'Public,NonPublic,Instance').GetValue($result, $null)
        }

        It 'Should be a valid CommandHelp object' {
            $result.GetType().FullName | Should -Be "Microsoft.PowerShell.PlatyPS.Model.CommandHelp"
        }

        It "Should have the correct metadata value for '<name>'" -TestCases @(
            @{ name = "external help file"; expectedValue = "Microsoft.PowerShell.Commands.Utility.dll-Help.xml" }
            @{ name = "Locale"; expectedValue = "en-US" }
            @{ name = "Module Name"; expectedValue = "Microsoft.PowerShell.Utility" }
            @{ name = "ms.date"; expectedValue = "12/12/2022" }
            @{ name = "HelpUri"; expectedValue = "https://learn.microsoft.com/powershell/module/microsoft.powershell.utility/get-date?view=powershell-7.4&WT.mc_id=ps-gethelp" }
            @{ name = "PlatyPS schema version"; expectedValue = "2024-05-01" }
            @{ name = "title"; expectedValue = "Get-Date" }
        ) {
            param ($name, $expectedValue)
            $metadata[$name] | Should -Be $expectedValue

        }
    }

    Context "Validate Aliases" {
        It "Should handle empty aliases" {
            $ch = Import-MarkdownCommandHelp "$PSScriptRoot/assets/Get-Date.V2.md"
            $ch.Aliases | Should -BeNullOrEmpty
        }

        It "Should handle present aliases" {
            $ch = Import-MarkdownCommandHelp "$PSScriptRoot/assets/Get-ChildItem.V2.md"
            $regex = ([regex]::new("^PowerShell.*dir.*gci.*ls", "SingleLine"))
            $ch.Aliases.Count | Should -Be 1    
            $ch.Aliases[0] | Should -Match $regex

        }
    }

    Context 'Validate RelatedLinks' {
        BeforeAll {
            $ch = Import-MarkdownCommandHelp "$PSScriptRoot/assets/Get-ChildItem.md"
        }

        It "Should correct related link '<link>' (offset <offset>)" -TestCases @(
                @{ offset = 0; name = "about_Certificate_Provider"; link = "../Microsoft.PowerShell.Security/About/about_Certificate_Provider.md" }
                @{ offset = 1; name = "about_Providers"; link = "../Microsoft.PowerShell.Core/About/about_Providers.md" }
                @{ offset = 2; name = "about_Quoting_Rules"; link = "../Microsoft.Powershell.Core/About/about_Quoting_Rules.md" }
                @{ offset = 3; name = "about_Registry_Provider"; link = "../Microsoft.PowerShell.Core/About/about_Registry_Provider.md" }
                @{ offset = 4; name = "ForEach-Object"; link = "../Microsoft.PowerShell.Core/ForEach-Object.md" }
                @{ offset = 5; name = "Get-Alias"; link = "../Microsoft.PowerShell.Utility/Get-Alias.md" }
                @{ offset = 6; name = "Get-Item"; link = "Get-Item.md" }
                @{ offset = 7; name = "Get-Location"; link = "Get-Location.md" }
                @{ offset = 8; name = "Get-Process"; link = "Get-Process.md" }
                @{ offset = 9; name = "Get-PSProvider"; link = "Get-PSProvider.md" }
                @{ offset = 10; name = "Split-Path"; link = "Split-Path.md" }
            ) {
            param ($offset, $name, $link)
            $ch.RelatedLinks[$offset].LinkText | Should -Be $name
            $ch.RelatedLinks[$offset].Uri | Should -Be $link
        }
    }

    Context 'Validate Input and Output' {
        BeforeAll {
            $ch1 = Import-MarkdownCommandHelp "$PSScriptRoot/assets/Compare-CommandHelp.md"
            $ch2 = Import-MarkdownCommandHelp "$PSScriptRoot/assets/Get-ChildItem.md"
        }

        It "Should have the proper input content" {
            $ch1.Inputs.Count | Should -Be 1
            $ch1.Inputs[0].TypeName | Should -BeExactly "System.Management.Automation.HiThere"
            $ch1.Inputs[0].Description | Should -Be ""
        }

        It "Should have the proper output content" {
            $ch1.Outputs.Count | Should -Be 1
            $ch1.Outputs[0].TypeName | Should -BeExactly "System.Object"
            $ch1.Outputs[0].Description | Should -Be ""
        }

        It "Should be correct for output '<Typename>' (offset <offset>)" -TestCases @(
                    @{ Offset = 0; Typename = "System.Management.Automation.AliasInfo"; description = 'The cmdlet outputs this type when accessing the `Alias:` drive.' }
                    @{ Offset = 1; Typename = "Microsoft.PowerShell.Commands.X509StoreLocation"; description = "" }
                    @{ Offset = 2; Typename = "System.Security.Cryptography.X509Certificates.X509Store"; description = "" }
                    @{ Offset = 3; Typename = "System.Security.Cryptography.X509Certificates.X509Certificate2"; description = 'The cmdlet outputs these types when accessing the `Cert:` drive.' }
                    @{ Offset = 4; Typename = "System.Collections.DictionaryEntry"; description = 'The cmdlet outputs this type when accessing the `Env:` drive.' }
                    @{ Offset = 5; Typename = "System.IO.DirectoryInfo"; description = "" }
                    @{ Offset = 6; Typename = "System.IO.FileInfo"; description = 'The cmdlet outputs these types when accessing the Filesystem drives.' }
                    @{ Offset = 7; Typename = "System.Management.Automation.FunctionInfo"; description = "" }
                    @{ Offset = 8; Typename = "System.Management.Automation.FilterInfo"; description = 'The cmdlet outputs these types when accessing the `Function:` drives.' }
                    @{ Offset = 9; Typename = "Microsoft.Win32.RegistryKey"; description = "The cmdlet outputs this type when accessing the Registry drives." }
                    @{ Offset = 10; Typename = "System.Management.Automation.PSVariable"; description = 'The cmdlet outputs this type when accessing the `Variable:` drives.' }
                    @{ Offset = 11; Typename = "Microsoft.WSMan.Management.WSManConfigContainerElement"; description = "" }
                    @{ Offset = 12; Typename = "Microsoft.WSMan.Management.WSManConfigLeafElement"; description = 'The cmdlet outputs these types when accessing the `WSMan:` drives.' }
                    @{ Offset = 13; Typename = "System.String"; description = 'When you use the **Name** parameter, this cmdlet returns the object names as strings.' }
                ) {
            param ($offset, $typename, $description)
            $ch2.outputs[$offset].Typename | Should -Be $typename
            $ch2.outputs[$offset].Description | Should -Be $description
        }
    }

    Context 'Validate Parameters' {
        BeforeAll {
            $ch = Import-MarkdownCommandHelp "$PSScriptRoot/assets/Get-ChildItem.md"
        }

        It "Should have correct type information for parameter '<name>'" -TestCases @(
                @{ Offset = 0; Name = 'Attributes'; Type = 'System.Management.Automation.FlagsExpression`1[System.IO.FileAttributes]' }
                @{ Offset = 1; Name = 'CodeSigningCert'; Type = 'System.Management.Automation.SwitchParameter' }
                @{ Offset = 2; Name = 'Depth'; Type = 'System.UInt32' }
                @{ Offset = 3; Name = 'Directory'; Type = 'System.Management.Automation.SwitchParameter' }
                @{ Offset = 4; Name = 'DnsName'; Type = 'Microsoft.PowerShell.Commands.DnsNameRepresentation' }
                @{ Offset = 5; Name = 'DocumentEncryptionCert'; Type = 'System.Management.Automation.SwitchParameter' }
                @{ Offset = 6; Name = 'Eku'; Type = 'System.String' }
                @{ Offset = 7; Name = 'Exclude'; Type = 'System.String[]' }
                @{ Offset = 8; Name = 'ExpiringInDays'; Type = 'System.Int32' }
                @{ Offset = 9; Name = 'File'; Type = 'System.Management.Automation.SwitchParameter' }
                @{ Offset = 10; Name = 'Filter'; Type = 'System.String' }
                @{ Offset = 11; Name = 'FollowSymlink'; Type = 'System.Management.Automation.SwitchParameter' }
                @{ Offset = 12; Name = 'Force'; Type = 'System.Management.Automation.SwitchParameter' }
                @{ Offset = 13; Name = 'Hidden'; Type = 'System.Management.Automation.SwitchParameter' }
                @{ Offset = 14; Name = 'Include'; Type = 'System.String[]' }
                @{ Offset = 15; Name = 'LiteralPath'; Type = 'System.String[]' }
                @{ Offset = 16; Name = 'Name'; Type = 'System.Management.Automation.SwitchParameter' }
                @{ Offset = 17; Name = 'Path'; Type = 'System.String[]' }
                @{ Offset = 18; Name = 'ReadOnly'; Type = 'System.Management.Automation.SwitchParameter' }
                @{ Offset = 19; Name = 'Recurse'; Type = 'System.Management.Automation.SwitchParameter' }
                @{ Offset = 20; Name = 'SSLServerAuthentication'; Type = 'System.Management.Automation.SwitchParameter' }
                @{ Offset = 21; Name = 'System'; Type = 'System.Management.Automation.SwitchParameter' }
        ) {
            param ($offset, $name, $type)
            $ch.Parameters[$offset].Name | Should -Be $name
            $ch.Parameters[$offset].Type | Should -Be $Type
        }
    }

    Context 'Validate Examples' {
        BeforeAll {
            $ch = Import-MarkdownCommandHelp "$PSScriptRoot/assets/Get-ChildItem.md"
        }

        It "Should have the proper example title for offset <offset>" -TestCases @(
            @{ offset = 0; title = 'Example 1: Get child items from a file system directory' }
            @{ offset = 1; title = 'Example 2: Get child item names in a directory' }
            @{ offset = 2; title = 'Example 3: Get child items in the current directory and subdirectories' }
            @{ offset = 3; title = 'Example 4: Get child items using the Include parameter' }
            @{ offset = 4; title = 'Example 5: Get child items using the Exclude parameter' }
            @{ offset = 5; title = 'Example 6: Get the registry keys from a registry hive' }
            @{ offset = 6; title = 'Example 7: Get all certificates with code-signing authority' }
            @{ offset = 7; title = 'Example 8: Get items using the Depth parameter' }
            @{ offset = 8; title = 'Example 9: Getting hard link information' }
            @{ offset = 9; title = 'Example 10: Output for Non-Windows Operating Systems' }
            @{ offset = 10; title = 'Example 11: Get the link target for a junction point' }
            @{ offset = 11; title = 'Example 12: Get the link target for an AppX reparse point' }
        ) {
            param ($offset, $title)
            $ch.Examples[$offset].Title | Should -Be $title
        }
    }

    Context 'Syntax' {
        BeforeAll {
            $ch = Import-MarkdownCommandHelp "$PSScriptRoot/assets/Get-ChildItem.md"
        }

        It "Should have the proper syntax for '<parametersetname>' (offset <offset>)" -TestCases @(
            @{
                offset = 0
                parametersetname = 'Items'
                string = 'Get-ChildItem [[-Path] <string[]>] [[-Filter] <string>] [-Include <string[]>] [-Exclude <string[]>] [-Recurse] [-Depth <uint>] [-Force] [-Name] [<CommonParameters>]'
            }
            @{
                offset = 1
                parametersetname = 'LiteralItems'
                string = 'Get-ChildItem [[-Filter] <string>] -LiteralPath <string[]> [-Include <string[]>] [-Exclude <string[]>] [-Recurse] [-Depth <uint>] [-Force] [-Name] [<CommonParameters>]'
            }
            @{
                offset = 2
                parametersetname = 'Items (Default) - Certificate provider'
                string = 'Get-ChildItem [[-Path] <string[]>] [[-Filter] <string>] [-Include <string[]>] [-Exclude <string[]>] [-Recurse] [-Depth <uint>] [-Force] [-Name] [-CodeSigningCert] [-DocumentEncryptionCert] [-SSLServerAuthentication] [-DnsName <string>] [-Eku <string[]>] [-ExpiringInDays <int>] [<CommonParameters>]'
            }
            @{
                offset = 3
                parametersetname = 'LiteralItems - Certificate provider'
                string = 'Get-ChildItem [[-Filter] <string>] -LiteralPath <string[]> [-Include <string[]>] [-Exclude <string[]>] [-Recurse] [-Depth <uint>] [-Force] [-Name] [-CodeSigningCert] [-DocumentEncryptionCert] [-SSLServerAuthentication] [-DnsName <string>] [-Eku <string[]>] [-ExpiringInDays <int>] [<CommonParameters>]'
            }
            @{
                offset = 4
                parametersetname = 'Items (Default) - Filesystem provider'
                string = 'Get-ChildItem [[-Path] <string[]>] [[-Filter] <string>] [-Include <string[]>] [-Exclude <string[]>] [-Recurse] [-Depth <uint>] [-Force] [-Name] [-Attributes <FlagsExpression[FileAttributes]>] [-FollowSymlink] [-Directory] [-File] [-Hidden] [-ReadOnly] [-System] [<CommonParameters>]'
            }
            @{
                offset = 5
                parametersetname = 'LiteralItems - FileSystem provider'
                string = 'Get-ChildItem [[-Filter] <string>] -LiteralPath <string[]> [-Include <string[]>] [-Exclude <string[]>] [-Recurse] [-Depth <uint>] [-Force] [-Name] [-Attributes <FlagsExpression[FileAttributes]>] [-FollowSymlink] [-Directory] [-File] [-Hidden] [-ReadOnly] [-System] [<CommonParameters>]'
            }
        ) {
            param ($offset, $parametersetname, $string)
            $ch.Syntax[$offset].ParameterSetName | Should -Be $parametersetname
            $ch.Syntax[$offset].ToString() | Should -Be $string
        }
    }
}
