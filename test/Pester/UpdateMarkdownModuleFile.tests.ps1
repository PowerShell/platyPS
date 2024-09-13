# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

Describe "Update-MarkdownModuleFile tests" {
    BeforeAll {
        $testModule = "testModule"

        # scriptblock for creating modules
        $m2 = {
            function get-thing1 {
                [CmdletBinding()]
                param ([Parameter()][string]$p1)
                END { "p1 is $ps1"}
            }
            function get-thing2 {
                [CmdletBinding()]
                param ([Parameter()][string]$p2)
                END { "p2 is $ps2"}
            }
        }

        $m = new-module -ScriptBlock $m2 -Name $testModule | Import-Module -Pass
        $ch = Get-Command -module $testModule | New-CommandHelp
        New-MarkdownCommandHelp -withModulePage -module $m -outputfolder ${testDrive}
        $mf = Import-MarkdownModuleFile "${testDrive}/testModule/testModule.md"
        $mfDescription = "This is a test description for a module file"
        $mf.Description = $mfDescription
        $mf.Metadata['ms.date'] = "01/01/2002"
        $null = $mf | Export-MarkdownModuleFile -outputfolder "${testDrive}/testModule" -Force
        remove-module $testModule
    }

    Context "Setup tests" {
        BeforeAll {
            $lmf = Import-MarkdownModuleFile "${testDrive}/testModule/testModule.md"
        }

        It "the Module file is properly created with property '<name>'" -TestC @(
            @{ Name = "module description"; Observed = $lmf.Description; Expected = $mfDescription }
            @{ Name = "Command Group Count"; Observed = $lmf.CommandGroups.Count; Expected = 1 }
            @{ Name = "Command Count"; Observed = $lmf.CommandGroups[0].Commands.Count; Expected = 2 }
            @{ Name = "Command 1 description"; Observed = $lmf.CommandGroups[0].Commands[0].Description; Expected = "{{ Fill in the Synopsis }}" }
            @{ Name = "Command 2 description"; Observed = $lmf.CommandGroups[0].Commands[1].Description; Expected = "{{ Fill in the Synopsis }}" }
            @{ Name = "ms.data metadata"; Observed = $lmf.Metadata['ms.date']; Expected = '01/01/2002' }
            ) {
            param ($name, $observed, $expected)
            $observed | Should -Be $expected

        }
    }

    Context "Update tests" {
        BeforeAll {
            $d1 = "get-thing1 description"
            $d2 = "get-thing2 description"
            $ch[0].Synopsis = $d1
            $ch[1].Synopsis = $d2
            $p = Update-MarkdownModuleFile -Path "${testDrive}/testModule/testModule.md" -Command $ch
            $lmf = Import-MarkdownModuleFile -Path $p.FullName
            $moduleFilePath = "${testDrive}/testModule/testModule.md"
            $moduleFileBackupPath = "${testDrive}/testModule/testModule.md.bak"
        }

        It "Creates a backup file" {
            $moduleFileBackupPath | Should -Exist
        }

        It "Properly updates the module file property '<name>'" -TestC @(
            @{ Name = "Module Description"; Observed = $lmf.Description; Expected = $mfDescription }
            @{ Name = "First command description"; Observed = $lmf.CommandGroups[0].Commands[0].Description; Expected = $d1 }
            @{ Name = "Second command description"; Observed = $lmf.CommandGroups[0].Commands[1].Description; Expected = $d2 }
            @{ Name = "Metadata ms.date"; Observed = $lmf.Metadata['ms.date']; Expected = [datetime]::Now.ToString("MM/dd/yyyy")}
        ) {
            param ( $name, $observed, $expected)
            $observed | Should -Be $expected
        }

        It "will produce an error if the backup file already exists" {
            {
                Update-MarkdownModuleFile -Path $moduleFilePath -Command $ch -ErrorAction Stop
            } | Should -Throw -ErrorId "BackupExists,Microsoft.PowerShell.PlatyPS.UpdateMarkdownModuleFileCommand"
        }

        It "will produce an error if the module file exists and -force is not used" {
            {
                Update-MarkdownModuleFile -Path $moduleFilePath -Command $ch -ErrorAction Stop -NoBackup
            } | Should -Throw -ErrorId "ModuleFileExists,Microsoft.PowerShell.PlatyPS.UpdateMarkdownModuleFileCommand"
        }

        It "will update the backup file, when -force is used" {
            $moduleFileInfoReference = (Get-ChildItem $moduleFilePath).LastWriteTime.Ticks
            $backupModuleFileInfoReference = (Get-ChildItem $moduleFileBackupPath).LastWriteTime.Ticks
            Start-Sleep 3
            Update-MarkdownModuleFile -Path $moduleFilePath -Command $ch -ErrorAction Stop -Force
            $observedModuleFileInfo = (Get-ChildItem $moduleFilePath).LastWriteTime.Ticks
            $observedBackupModuleFileInfo = (Get-ChildItem $moduleFileBackupPath).LastWriteTime.Ticks
            $observedModuleFileInfo | Should -BeGreaterThan $moduleFileInfoReference
            $observedBackupModuleFileInfo | Should -BeGreaterOrEqual $backupModuleFileInfoReference
        }
    }
}
