# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

Describe "Import-YamlCommandHelp tests" {
    BeforeAll {
        $mdFile = "${psscriptroot}/assets/get-date.md"
        $yamlFile = "${psscriptroot}/assets/get-date.yml"
        $mdch = Import-MarkdownCommandHelp "$mdFile"
        $ymlch = Import-YamlCommandHelp $yamlFile
    }

    Context "Basic Tests" {
        It "should be equivalent between yaml and markdown" {
            $mdch -eq $ymlch | Should -Be $true
        }

        It "should be able to pipe to import" {
            $yml = $yamlFile | Import-YamlCommandHelp
            $yml -eq $ymlch | Should -Be $true
        }

        It "should be able to pipe multiple files" {
            $yml = $yamlFile,$yamlFile,$yamlFile | Import-YamlCommandHelp
            $yml.Count | Should -Be 3
        }

        It "should handle literal paths" {
            $literalPath = "${testDrive}/[get-date].yml"
            copy-item $yamlFile $literalPath
            $yml = Import-YamlCommandHelp -LiteralPath $literalPath
            $yml -eq $mdch | Should -Be $true
        }

        It "Should return a dictionary with -AsDictionary" {
            $yml = Import-YamlCommandHelp -Path $yamlFile -AsDictionary
            $yml | Should -BeOfType "System.Collections.Specialized.OrderedDictionary"
        }
    }

    Context "yaml as dictionary" {
        BeforeAll {
            $yamlDict = Import-YamlCommandHelp -Path $yamlFile -AsDictionary
        }

        It "Should have the '<name>' property" -TestCases $(
            @{ Name = 'metadata' }
            @{ Name = 'title' }
            @{ Name = 'synopsis' }
            @{ Name = 'syntaxes' }
            @{ Name = 'aliases' }
            @{ Name = 'description' }
            @{ Name = 'examples' }
            @{ Name = 'parameters' }
            @{ Name = 'inputs' }
            @{ Name = 'outputs' }
            @{ Name = 'notes' }
            @{ Name = 'links' }
        ) {
            param ($name)
            $yamlDict.Contains($name) | Should -Be $true
        }
    }
}