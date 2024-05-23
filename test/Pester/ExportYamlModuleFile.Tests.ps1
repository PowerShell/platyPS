Describe "Export-YamlModuleFile tests" {
    BeforeAll {
        $moduleFileNames = "Microsoft.PowerShell.Archive.md", "Microsoft.PowerShell.Core.md",
            "Microsoft.PowerShell.Diagnostics.md", "Microsoft.PowerShell.Host.md", "Microsoft.PowerShell.Management.md", "Microsoft.PowerShell.Security.md",
            "Microsoft.PowerShell.Utility.md", "Microsoft.WSMan.Management.md", "PSDiagnostics.md", "PSReadLine.md", "ThreadJob.md"
        $moduleFileList = $moduleFileNames.ForEach({"${PSScriptRoot}/assets/$_"})
        $moduleFileInfos = Import-MarkdownModuleFile -Path $moduleFileList
    }

    Context "Basic tests" {
        It "Can export yaml module file '<name>'" -TestCases @(
            $moduleFileList.ForEach({@{ name = ([io.path]::GetFileName($_)); path = "$_" }})
        )  {
            param ($path, $name)
            $outputFile = Import-MarkdownModuleFile -Path $path | Export-YamlModuleFile -OutputFolder ${TestDrive}
            $outputFile | Should -Exist
        }

        It "Will add additional metadata if supplied" {
            $outputFolder = "${TestDrive}/new0"
            $newMetadata = @{ newkey1 = "new metadata 1"; "new key 2" = "new metadata[withspecialchar]"}
            $outFile = $moduleFileInfos[0] | Export-YamlModuleFile -OutputFolder ${outputFolder} -Metadata $newMetadata
            $readModuleFile = Import-YamlModuleFile $outFile
            $readModuleFile.Metadata['newkey1'] | Should -BeExactly $newMetadata['newkey1']
            $readModuleFile.Metadata['new key 2'] | Should -BeExactly $newMetadata['new key 2']
        }

        It "Will not create a new file if one already exists" {
            $outputFolder = "${TestDrive}/new1"
            $outFile = $moduleFileInfos | Export-YamlModuleFile -OutputFolder ${outputFolder}
            $outFile | Should -Exist
            Set-Content $outFile[0].FullName -Value $null
            $result = $moduleFileInfos[0] | Export-YamlModuleFile -OutputFolder ${outputFolder} 3>&1
            $result | Should -BeOfType [System.Management.Automation.WarningRecord] 
            $result.Message | Should -Be "skipping Microsoft.PowerShell.Archive"
            (Get-ChildItem $outFile[0]).Length | Should -Be 0
        }

        It 'Can export a group of module files when piped' {
            $outputFolder = "${TestDrive}/new2"
            $moduleFileInfos | Export-YamlModuleFile -OutputFolder ${outputFolder}
            (Get-ChildItem $outputFolder).Count | Should -Be $moduleFileInfos.Count
        }

        It 'Can export a group of module files when provided as argument' {
            $outputFolder = "${TestDrive}/new3"
            Export-YamlModuleFile $moduleFileInfos -OutputFolder ${outputFolder}
            (Get-ChildItem $outputFolder).Count | Should -Be $moduleFileInfos.Count
        }
    }

    Context "Round trip tests (export and then import)" {
        BeforeAll {
            $yamlFiles = $moduleFileInfos | Export-YamlModuleFile -output "${TestDrive}/new4"
        }

        It "Should be able to round trip module info file '<name>'" -TestCases $(
            for($i = 0; $i -lt $moduleFileInfos.Count; $i++) {
                @{ Source = $moduleFileInfos[$i]; Target = $yamlFiles[$i].FullName; Name = $yamlFiles[$i].Name}
            }
        ) {
            param ( $source, $target, $name )
            $expected = $source
            $observed = Import-YamlModuleFile -Path $target
            $observed | Should -Be $expected
        }
    }
}