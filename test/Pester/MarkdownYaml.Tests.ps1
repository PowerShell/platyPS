# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

Describe "Create valid yaml" {
    BeforeAll {
        New-Item -Type Directory -Path "${TESTDRIVE}/markdown"
        New-Item -Type Directory -Path "${TESTDRIVE}/yaml"
        New-Item -Type Directory -Path "${TESTDRIVE}/psdocs"
        Import-Module "${PSScriptRoot}/PlatyPS.Test.psm1"
        $verbs = get-verb
        try {
            push-location "${TESTDRIVE}/psdocs"
            try {
                git clone "https://github.com/PowerShell/PowerShell-Docs"
                push-location "PowerShell-Docs/reference/7.4/Microsoft.PowerShell.Core"
                $mdfiles = Get-ChildItem -filter *.md | Where-Object { $verbs.Verb -Contains ($_.name.split("-")[0]) }
                Copy-Item $mdfiles "${TESTDRIVE}/markdown"
            }
            finally {
                pop-location
            }
        }
        finally {
            pop-location
        }

        $testCases = Get-ChildItem -Path "${TESTDRIVE}/markdown" -Filter *.md |
            foreach-object { @{ Path = $_.FullName; Name = $_.Name } }
    }

    It "Converting '<Name>' to Yaml creates readable file." -TestCases $testCases {
        param ( $Path, $Name )

        $yamlFileName = $Name -replace ".md",".yml"
        try {
            $ch = $Path | Import-MarkdownCommandHelp
            $ch | Export-YamlCommandHelp -outputFolder "${TESTDRIVE}/yaml"
        }
        catch {
            $_.Exception.Message | Should -BeNullOrEmpty
        }

        try {
            $yamlFilePath = Join-Path "${TESTDRIVE}/yaml" $yamlFileName
            $result = $yamlFilePath | Import-CommandYaml
            $result | Should -Not -BeNullOrEmpty
            $result.GetType().Name | Should -Be "hashtable"
        }
        catch {
            $_.Exception.Message | Should -BeNullOrEmpty
        }
    }
}
