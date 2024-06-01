# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

Describe "Miscellaneous cmdlet tests" {
    BeforeAll {
        $platyPScmdlets = Get-Command -Module Microsoft.PowerShell.PlatyPS
    }

    Context "LiteralPath" {
        It 'Cmdlet <cmdlet> should have a "LiteralPath" parameter to match Path' -testcases @(
        foreach($cmd in $platyPScmdlets.Where({$_.Parameters['Path']})) {
            @{ cmdlet = $cmd }
        } 
        ) {
            param ($cmdlet)
            $cmdlet.Parameters['LiteralPath'] | Should -Not -BeNullOrEmpty
        }
    }

    Context "Output Type Attribute" {
        It "Cmdlet '<cmdlet>' should have an output type attribute" -TestCases @(
            $platyPScmdlets.ForEach({ @{ Cmdlet = $_ } })
        ) {
            param ($cmdlet)
            $outputAttribute = $cmdlet.ImplementingType.CustomAttributes.Where({$_.AttributeType -eq [System.Management.Automation.OutputTypeAttribute]})
            $outputAttribute | Should -Not -BeNullOrEmpty
        }
    }

    Context "Tab completion for encoding parameter" {
        It "Cmdlet '<cmdlet>' should have a completer for the Encoding parameter" -testcases @(
            $platyPSCmdlets.Where({$_.Parameters['Encoding']}).Foreach({ @{ Cmdlet = $_; Parameter = $_.Parameters['Encoding'] }})
        ) {
            param ($cmdlet, $parameter)
            $completerType = $parameter.Attributes.Where({$_.TypeId -eq [System.Management.Automation.ArgumentCompleterAttribute]}).Type.Name 
            $completerType | Should -Be "EncodingCompleter"
        }
    }

    Context "Metadata parameter" {
        It "Cmdlet '<cmdlet>' should have a 'metadata' parameter" -testcases @(
            $platyPSCmdlets.Where({$_.Name -match "Export-Mark|Export-Yaml|New-MarkdownCommandHelp"}).Foreach({ @{ cmdlet = $_ }})
        ) {
            param ($cmdlet)
            $cmdlet.Parameters['Metadata'] | Should -Not -BeNullOrEmpty
        }
    }
}