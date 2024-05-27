# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

Describe "Export-MamlCommandHelp tests" {
    BeforeAll {
        $assetDir = Join-Path $PSScriptRoot "assets"
        $markdownFiles = 'get-date.md', 'Import-Module.md', 'Invoke-Command.md', 'Out-Null.md'
        $chObjects = $markdownFiles | Foreach-Object { Import-MarkdownCommandHelp  (Join-Path $assetDir $_) }
        $outputDirectory = Join-Path $TESTDRIVE MamlBase
        $f1 = "$outputDirectory/Microsoft.PowerShell.Core-Help.xml"
        $f2 = "$outputDirectory/Microsoft.PowerShell.Utility-Help.xml"
    }

    Context "Basic Operations" {

        # needed by subesequent tests
        It "Should create a file" {
            $chObjects | Export-MamlCommandHelp -OutputDirectory $outputDirectory
            $outputDirectory | Should -Exist
            $f1 | Should -Exist
            $f2 | Should -Exist
        }

        # this test relies on the previous test to create the file
        It "Should have the proper default encoding" {
            $bytes = Get-Content -Path $f1 -AsByteStream | Select-Object -First 2
            $bytes | Should -Be 60, 63
            $bytes = Get-Content -Path $f2 -AsByteStream | Select-Object -First 2
            $bytes | Should -Be 60, 63
        }

        It "Should have an error if the file already exists" {
            { $chObjects | Export-MamlCommandHelp -OutputDirectory $outputDirectory -ErrorAction stop } | Should -Throw
        }

        It "Should not have an error if the file already exists and -Force is used" {
            Start-Sleep -Seconds 5
            $chObjects | Export-MamlCommandHelp -OutputDirectory $outputDirectory -Force
            $fi = Get-ChildItem $f1
            $fi.LastWriteTime | Should -BeGreaterThan (Get-Date).AddSeconds(-5)
        }

        It "Should create the file with the proper encoding" {
            Remove-Item -Path $outputDirectory -ErrorAction SilentlyContinue -Recurse
            $chObjects | Export-MamlCommandHelp -OutputDirectory $outputDirectory -Encoding ([System.Text.Encoding]::Unicode)
            $bytes = Get-Content -Path $f1 -AsByteStream | Select-Object -First 2
            $bytes | Should -Be 255, 254
            $bytes = Get-Content -Path $f1 -AsByteStream | Select-Object -First 2
            $bytes | Should -Be 255, 254
        }

        It "Using WhatIf should not result in a file" {
            Remove-Item -Path $outputDirectory -Recurse
            $chObjects | Export-MamlCommandHelp -OutputDirectory $outputDirectory -WhatIf
            $outputDirectory | Should -Not -Exist
        }
    }

    Context "Content Tests" {
        BeforeAll {
            $chObjects | Export-MamlCommandHelp -OutputDirectory $outputDirectory -Force
            $xml = [xml](Get-Content -Path $f1)
            $namespace = $xml.helpItems.NamespaceURI
            $ns = [System.Xml.XmlNamespaceManager]::new($xml.NameTable)
            $ns.AddNamespace('maml', $xml.helpItems.maml)
            $ns.AddNamespace('command', $xml.helpItems.command[0]) # this is because we have both an attribute and a node with the same name
            $ns.AddNamespace('dev', $xml.helpItems.dev)
            $xml2 = [xml](Get-Content -Path $f2)
            $namespace = $xml2.helpItems.NamespaceURI
            $ns2 = [System.Xml.XmlNamespaceManager]::new($xml2.NameTable)
            $ns2.AddNamespace('maml', $xml2.helpItems.maml)
            $ns2.AddNamespace('command', $xml2.helpItems.command[0]) # this is because we have both an attribute and a node with the same name
            $ns2.AddNamespace('dev', $xml2.helpItems.dev)
        }

        It "Should have the proper number of commands" {
            $xml.SelectNodes('//command:command', $ns).Count | Should -Be 3
        }

        It "Should have the proper list of commands" {
            $xml.SelectNodes('//command:command/command:details/command:name', $ns)."#text" | Should -Be 'Import-Module', 'Invoke-Command', 'Out-Null'
        }

        It "Should have the proper number of parameters for Get-Date" {
            $xml2.SelectNodes('//command:command', $ns2).Where({$_.details.name -eq "Get-Date"}).Parameters.parameter.Count | Should -Be 13
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
            $xml2.SelectNodes('//command:command', $ns2).Where({$_.details.name -eq "Get-Date"}).examples.example.Count | Should -Be 10
        }

        It "Should have the proper number of inputs" {
            $xml2.SelectNodes('//command:command', $ns2).Where({$_.details.name -eq "Get-Date"}).inputTypes.inputType.Count | Should -Be 1
        }

        It "Should have the proper number of outputs" {
            $xml2.SelectNodes('//command:command', $ns2).Where({$_.details.name -eq "Get-Date"}).returnValues.returnValue.Count | Should -Be 2
        }

        It "Should have the proper number of relatedLinks" {
            $xml2.SelectNodes('//command:command', $ns2).Where({$_.details.name -eq "Get-Date"}).relatedLinks.navigationLink.Count | Should -Be 6
        }

        It "Should have the same content for the description" {
            # reconstruct the description from the objects, also remove white space as maml can add/remove
            $expected = $chObjects.Where({$_.title -eq "Get-Date"}).Description
            $observed = $xml2.SelectNodes('//command:command', $ns2).Where({$_.details.name -eq "Get-Date"}).description.para
            $expectedStr = ($expected -join "") -replace "[`n`r ]"
            $observedStr = ($observed -join "") -replace "[`n`r ]"
            $observedStr | Should -Be $expectedStr
        }
    }
}
