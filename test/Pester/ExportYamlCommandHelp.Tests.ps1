# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

Describe "Export-YamlCommandHelp tests" {
    BeforeAll {
        $assetDir = "$PSScriptRoot/assets"
        $markdownFile = "$assetDir/get-date.md"
        $yamlFile = "${psscriptroot}/assets/get-date.yml"
        $testMDFile = "$TestDrive/Get-Date.md"
        Get-Content $markdownFile | Set-Content "$testMDFile"
        $ch = Import-MarkdownCommandHelp $testMDFile
        $ch | Export-YamlCommandHelp -outputfolder $TESTDRIVE -Force
        $outputFile = "$TESTDRIVE/Get-Date.yml"
        Import-Module "$PSScriptRoot/PlatyPS.Test.psm1"
        $yamlDict = Import-CommandYaml $outputFile -PreserveOrder
    }

    Context "File Contents" {
        It "The exported file contents should match exactly" {
            $expectedContent = (Get-Content $yamlFile -Raw).Replace("`n","").Replace("`r","")
            $observedContent = (Get-Content $outputFile -Raw).Replace("`n","").Replace("`r","")
            $observedContent | Should -Be $expectedContent
        }
    }

    Context "Toplevel elements of the yaml file." {
        It "Should contain the key '<key>' in the proper offset '<offset>'" -TestCases $(
            @{ key = 'metadata'; offset = 0 }
            @{ key = 'title' ; offset = 1 }
            @{ key = 'synopsis' ; offset = 2 }
            @{ key = 'syntaxes' ; offset = 3 }
            @{ key = 'aliases' ; offset = 4 }
            @{ key = 'description' ; offset = 5 }
            @{ key = 'examples' ; offset = 6 }
            @{ key = 'parameters' ; offset = 7 }
            @{ key = 'inputs' ; offset = 8 }
            @{ key = 'outputs' ; offset = 9 }
            @{ key = 'notes' ; offset = 10 }
            @{ key = 'links' ; offset = 11 }
        ) {
            param ($key, $offset)
            $yamlDict.Contains($key) | Should -Be $true
            $yamlDict.Keys[$offset] | Should -Be $key
        }
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
        BeforeAll {
            $objectExamples = $ch.examples
            $yamlExamples = $yamlDict['examples']
        }

        It "Should contain the same count of examples" {
            $objectExamples.Count | Should -Be $yamlExamples.Count
        }

        It "Should preserve the content for example <example>, '<title>'" -TestCases $(
            $objectExamples | Foreach-Object { $exampleNumber = 1 } {
                @{ example = $exampleNumber++; title = $_.title; remarks = $_.remarks }
            }
        ) {
            param ($example, $title, $remarks)
            $observedExample = $yamlExamples[$example - 1]
            $observedExample['title'] | Should -Be $title
            if ($observedExample['description'] -ne $remarks) { wait-debugger }
            $observedExample['description'] | Should -Be $remarks
        }
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
        BeforeAll {
            $expectedInputs = $ch.Inputs
            $observedInputs = $yamlDict['inputs']
        }

        It "Should have the proper number of inputs" {
            $observedInputs.Count | Should -Be $expectedInputs.Count
        }

        It "The inputs should be properly preserved for offset '<offset>'" -TestCases $(
            $observedInputs | Foreach-Object { $number = 0 } {
                @{ inputObject = [Microsoft.PowerShell.PlatyPS.Model.InputOutput]::new( $_['name'], $_['description']); offset = $number++ }
            }
        ) {
            param ($inputObject, $offset)
            $expectedInputs[$offset] | Should -Be $inputObject
        }
    }

    Context "Output" {
        BeforeAll {
            $expectedOutputs = $ch.Outputs
            $observedOutputs = $yamlDict['outputs']
        }

        It "Should have the proper number of outputs" {
            $observedOutputs.Count | Should -Be $expectedOutputs.Count
        }

        It "The outputs should be properly preserved for offset '<offset>'" -TestCases $(
            $observedOutputs | Foreach-Object { $number = 0 } {
                @{ outputObject = [Microsoft.PowerShell.PlatyPS.Model.InputOutput]::new( $_['name'], $_['description']); offset = $number++ }
            }
        ) {
            param ($outputObject, $offset)
            $expectedOutputs[$offset] | Should -Be $outputObject
        }

    }

    Context "Notes" {
        It "Should preserve the notes content" {
            $yamlDict['notes'] | Should -Be $ch.Notes
        }
    }

    Context "Related Links" {
        It "Should preserve the count of related links" {
            $ch.RelatedLinks.Count | Should -Be $yamlDict['links'].Count
        }

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
}
