# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

Describe "Interactive experience tests" {
    Context "Encoding parameter tab completion" {
        BeforeAll {
            $cmdletsWithEncodingParameter = Get-Command -Module Microsoft.PowerShell.PlatyPS |
                Where-Object { $_.Parameters['Encoding'] }
            $expectedEncoding = "ansi", "ascii", "bigendianunicode", "bigendianutf32", "unicode", "utf7", "utf8", "utf8BOM", "utf8NoBOM", "utf32"
        }
    
        It "'<cmdlet>' has the appropriate attributes on the Encoding parameter" -TestCases $(
            $cmdletsWithEncodingParameter | Foreach-Object {
                @{ Cmdlet = $_.Name; Parameter = $_.Parameters['Encoding'] }
            } 
        ) {
            param ($cmdlet, $parameter)
            $parameter.Attributes.Where({$_ -is [System.Management.Automation.ArgumentCompleterAttribute]}) | Should -Not -BeNullOrEmpty
        }

        It "tabexpansion for '<cmdlet>' completes the Encoding parameter correctly" -TestCases $(
            $cmdletsWithEncodingParameter | Foreach-Object {
                @{ Cmdlet = $_.Name }
            } 
        ) {
            param ($cmdlet)
            $cmdString = "$cmdlet -Encoding "
            $result = TabExpansion2 $cmdString $cmdString.Length
            $result.CompletionMatches.CompletionText | Should -Be $expectedEncoding
        }
    }
}