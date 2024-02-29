# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

Describe "Export-MamlCommandHelp tests" {
    BeforeAll {
        $assetDir = Join-Path $PSScriptRoot "assets"
        $markdownFiles = 'get-date.md', 'Import-Module.md', 'Invoke-Command.md', 'Out-Null.md'
        $chObjects = $markdownFiles | Foreach-Object { Import-MarkdownCommandHelp  (Join-Path $assetDir $_) }
        $outputFile = Join-Path $TESTDRIVE "ExportMamlCommand.Help.xml"
    }

    Context "Basic Operations" {

        # needed by subesequent tests
        It "Should create a file" {
            $chObjects | Export-MamlCommandHelp -OutputFile $outputFile
            $outputFile | Should -Exist
        }

        # this test relies on the previous test to create the file
        It "Should have the proper default encoding" {
            $bytes = Get-Content -Path $outputFile -AsByteStream | Select-Object -First 2
            $bytes | Should -Be 60, 63
        }

        It "Should have an error if the file already exists" {
            { $chObjects | Export-MamlCommandHelp -OutputFile $outputFile } | Should -Throw
        }

        It "Should not have an error if the file already exists and -Force is used" {
            Start-Sleep -Seconds 5
            $chObjects | Export-MamlCommandHelp -OutputFile $outputFile -Force
            $fi = Get-ChildItem $outputFile
            $fi.LastWriteTime | Should -BeGreaterThan (Get-Date).AddSeconds(-5)
        }

        It "Should create the file with the proper encoding" {
            Remove-Item -Path $outputFile -ErrorAction SilentlyContinue
            $chObjects | Export-MamlCommandHelp -OutputFile $outputFile -Encoding ([System.Text.Encoding]::Unicode)
            $bytes = Get-Content -Path $outputFile -AsByteStream | Select-Object -First 2
            $bytes | Should -Be 255, 254
        }

        It "Using WhatIf should not result in a file" {
            Remove-Item -Path $outputFile
            $chObjects | Export-MamlCommandHelp -OutputFile $outputFile -WhatIf
            $outputFile | Should -Not -Exist
        }
    }

    Context "Content Tests" {
        BeforeAll {
            $chObjects | Export-MamlCommandHelp -OutputFile $outputFile -Force
            $xml = [xml](Get-Content -Path $outputFile)
            $namespace = $xml.helpItems.NamespaceURI
            $ns = [System.Xml.XmlNamespaceManager]::new($xml.NameTable)
            $ns.AddNamespace('maml', $xml.helpItems.maml)
            $ns.AddNamespace('command', $xml.helpItems.command[0]) # this is because we have both an attribute and a node with the same name
            $ns.AddNamespace('dev', $xml.helpItems.dev)
        }

        It "Should have the proper number of commands" {
            $xml.SelectNodes('//command:command', $ns).Count | Should -Be 4
        }

        It "Should have the proper list of commands" {
            $xml.SelectNodes('//command:command/command:details/command:name', $ns)."#text" | Should -Be 'Get-Date', 'Import-Module', 'Invoke-Command', 'Out-Null'
        }

        It "Should have the proper number of parameters for Get-Date" {
            $xml.SelectNodes('//command:command', $ns).Where({$_.details.name -eq "Get-Date"}).Parameters.parameter.Count | Should -Be 13
        }

        It "Should have the proper number of parameters for Import-Module" {
            $xml.SelectNodes('//command:command', $ns).Where({$_.details.name -eq "Import-Module"}).Parameters.parameter.Count | Should -Be 26
        }

        It "Should have the proper number of parameters for Invoke-Command" {
            $xml.SelectNodes('//command:command', $ns).Where({$_.details.name -eq "Invoke-Command"}).Parameters.parameter.Count | Should -Be 37
        }

        It "Should have the proper number of parameters for Out-Null" {
            $xml.SelectNodes('//command:command', $ns).Where({$_.details.name -eq "Out-Null"}).Parameters.parameter.Count | Should -Be 1
        }

        It "Should have the proper number of examples" {
            $xml.SelectNodes('//command:command', $ns).Where({$_.details.name -eq "Get-Date"}).examples.example.Count | Should -Be 10
        }

        It "Should have the proper number of inputs" {
            $xml.SelectNodes('//command:command', $ns).Where({$_.details.name -eq "Get-Date"}).inputTypes.inputType.Count | Should -Be 1
        }

        It "Should have the proper number of outputs" {
            $xml.SelectNodes('//command:command', $ns).Where({$_.details.name -eq "Get-Date"}).returnValues.returnValue.Count | Should -Be 2
        }

        It "Should have the proper number of relatedLinks" {
            $xml.SelectNodes('//command:command', $ns).Where({$_.details.name -eq "Get-Date"}).relatedLinks.navigationLink.Count | Should -Be 6
        }

        It "Should have the same content for the description" {
            # reconstruct the description from the objects
            $expected = $chObjects.Where({$_.title -eq "Get-Date"}).Description -replace ([environment]::newline)," " -join " " -replace "  ", " "
            $observed = $xml.SelectNodes('//command:command', $ns).Where({$_.details.name -eq "Get-Date"}).description.para -join " "
            $observed | Should -Be $expected
        }
    }
}
