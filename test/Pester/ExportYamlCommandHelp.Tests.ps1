# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

Describe "Export-YamlCommandHelp tests" {
    BeforeAll {
        $assetDir = "$PSScriptRoot/assets"
        $markdownFile = "$assetDir/get-date.md"
        $ch = Import-MarkdownCommandHelp $markdownFile
        $ch | Export-YamlCommandHelp -outputfolder $TESTDRIVE -Force
        $outputFile = "$TESTDRIVE/get-date.yml"
        Import-Module "$PSScriptRoot/PlatyPS.Test.psm1"
        $yamlDict = Import-CommandYaml $outputFile
    }

    Context "Metadata" {
        It "Should properly preserve the metadata type" {
            $yamlDict['metadata'] | Should -BeOfType [System.Collections.IDictionary]
        }

        It "Should properly preserve the metadata key count" {
            $yamlDict['metadata'].Keys.Count | Should -Be $ch.metadata.keys.count
        }

        It "Should property preserve the value of key '<Name>'" -TestCases $(
                $yamlDict['metadata'].Keys.Foreach({@{Name = $_; Value = $yamlDict['metadata'][$_]}})
            ) {
            param ($Name, $Value)
            $yamlVal = $yamlDict['metadata']["$name"]
            $chVal = $ch.Metadata[$name]
            $yamlVal | Should -Be $chVal
        }
    }

    Context "Title" {
        It "Should preserve the title" {
            $yamlDict['title'] | Should -Be $ch.Title
        }
    }

    Context "Synopsis" {
        It "Should preserve the synopsis" {
            $yamlDict['synopsis'] | Should -Be $ch.Synopsis
        }
    }

    Context "Syntax" {
        It "Should preserve the syntax statement count" {
            $yamlDict['syntaxes'].Count | Should -Be $ch.Syntax.Count
        }

        It "Should preserve the syntax statements for '<ParameterSetName>'" -TestCases $(
            $ch.Syntax | foreach-object { @{ ParameterSetName = $_.ParameterSetName; SyntaxObject = $_ } }
        ) {
            param ( $ParameterSetName, $SyntaxObject )
            $syntax = $yamlDict['syntaxes'] | Where-Object { $_['parameterSetName'] -eq $ParameterSetName }
            $syntax | Should -Not -BeNullOrEmpty
            $syntax['parameterSetName'] | Should -Be $ParameterSetName
            $syntax['commandName'] | Should -Be $syntaxObject.commandName
            $default = [bool]::Parse($syntax['isDefault'])
            $default | Should -Be $syntaxObject.IsDefaultParameterSet
            $expectedParameters = $syntaxObject.SyntaxParameters | foreach-object {$_.ToString()}
            $observedParameters = $syntax['parameters']
            $observedParameters | Should -Be $expectedParameters
        }
    }

    Context "Description" {
        It "The description should be preserved" {
            $yamlDict['description'] | Should -Be $ch.Description
        }
    }

    Context "Examples" {

    }

    Context "Parameters" {
        BeforeAll {
            # must remove the common parameters from the yaml representation
            $observedParameters = $yamlDict['parameters'] | Where-Object {$_['name'] -ne "CommonParameters"}
            $expectedParameters = $ch.Parameters
        }

        It "Should have the proper number of parameters" {
            $observedParameters.Count | Should -Be $expectedParameters.Count
        }

        It "Should preserve the parameter metadata for '<Name>'" -TestCases $(
            $ch.Parameters | Foreach-Object {
                @{ Name = $_.Name; Type = $_.Type ; Description = $_.Description; DefaultValue = $_.DefaultValue; DontShow = $_.DontShow; ParameterSets = $_.ParameterSets }
            }
        ) {
            param ($name, $type, $description, $defaultValue, $dontShow, $parameterSets)
                $observedParameter = $observedParameters | Where-Object { $_['name'] -eq $name }
                $observedParameter | Should -Not -BeNullOrEmpty
                $observedParameter['type'] | Should -Be $type
                $observedParameter['description'] | Should -Be $description
                $observedParameter['defaultValue'] | Should -Be $defaultValue
                [bool]::Parse($observedParameter['dontShow']) | Should -Be $dontShow

                for ($i = 0; $i -lt $parameterSets.Count; $i++) {
                    $observedParameterSet = $observedParameter['parameterSets'][$i]
                    $observedParameterSet['name'] | Should -Be $parameterSets[$i].name
                }
        }

    }

    Context "Input" {

    }

    Context "Output" {

    }

    Context "Related Links" {
        It "Should preserve the related links content for '<linktext>'" -TestCases $(
            $i = 0
            $ch.RelatedLinks | Foreach-Object {
                @{ Offset = $i++; Uri = $_.Uri; LinkText = $_.LinkText}
            }
        ) {
            param ( $offset, $uri, $linktext )
            $relatedLinks = $yamlDict['links']
            $relatedLinks[$offset]['text'] | Should -Be $linktext
            $relatedLinks[$offset]['href'] | Should -Be $uri
        }
    }

    Context "Notes" {
        It "Should preserve the notes content" {
            $yamlDict['notes'] | Should -Be $ch.Notes
        }
    }

}