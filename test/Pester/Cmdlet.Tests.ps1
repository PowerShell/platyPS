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

    Context "Parameter name/type tests" {
        It "Parameters which accept command help objects should be named CommandHelp" -TestCases @(
            $platyPScmdlets.
                Foreach({$_.Parameters.Values}).
                Where({$_.parametertype -match "CommandHelp"}).
                Foreach({ @{ Parameter = $_} })
        ) {
            param ($parameter)
            $parameter.Name | Should -Match "CommandHelp"
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

    Context "Cmdlets which change state" {
        It "Cmdlet '<name>' should support ShouldProcess" -testcase @(
            $platyPScmdlets.
                where({$_.verb -match "Export|New|Update"}).
                Where({$_.noun -ne "CommandHelp"}).
                foreach({ @{ Cmdlet = $_ ; name = $_.Name } })
        ) {
            param ($cmdlet)
            $cmdletAttribute = $cmdlet.ImplementingType.GetCustomAttributes($true).Where({$_.TypeId.Name -eq "CmdletAttribute"})
            $cmdletAttribute.SupportsShouldProcess | Should -Be $true
        }

    }
}