# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

Describe "Import-ModuleFile tests" {
    BeforeAll {
        $modFiles = Get-ChildItem -File "${PSScriptRoot}/assets" | Where-Object { $_.extension -eq ".md" -and $_.name -notmatch "-" }
    }

    Context "File creation" {
        It "Should be able to read module files" {
            $results = $modFiles | Import-MarkdownModuleFile
            $results.Count | Should -Be 12
        }

        It "Should produce the correct type of object" {
            $results = $modFiles | Import-MarkdownModuleFile
            $results[0] | Should -BeOfType Microsoft.PowerShell.PlatyPS.ModuleFileInfo
        }
    }

    Context "Object properties" {
        BeforeAll {
            $mf0 = $modFiles[0] | Import-MarkdownModuleFile
            $testcases = @(
                @{ PropertyName = "Metadata"; PropertyType = "ordered" }
                @{ PropertyName = "Title"; PropertyType = "String" }
                @{ PropertyName = "Module"; PropertyType = "String" }
                @{ PropertyName = "ModuleGuid"; PropertyType = "Guid" }
                @{ PropertyName = "Description"; PropertyType = "String" }
                @{ PropertyName = "Locale"; PropertyType = "CultureInfo" }
                @{ PropertyName = "OptionalElement"; PropertyType = "String" }
                @{ PropertyName = "Commands"; PropertyType = "System.Collections.Generic.List[Microsoft.PowerShell.PlatyPS.ModuleCommandInfo]" }
                @{ PropertyName = "Diagnostics"; PropertyType = "Microsoft.PowerShell.PlatyPS.Model.Diagnostics" }
            )
        }

        It "The property <PropertyName> should exist and be of type <PropertyType>" -TestCases $testCases {
            param ($propertyName, $propertyType)
            $mf0.PSObject.Properties[$propertyName] | Should -BeOfType System.Management.Automation.PSProperty
            $t = $mf0.PSObject.Properties[$propertyName].Value.GetType()
            [Microsoft.PowerShell.ToStringCodeMethods]::Type($t) | Should -Be $propertyType
        }

        It "The property '<PropertyName>' should exist and be the correct value" -testCases $(
            @{ PropertyName = "Title"; Value = "CimCmdlets Module" }
            @{ PropertyName = "Module"; Value = "CimCmdlets" }
            @{ PropertyName = "ModuleGuid"; Value = "fb6cc51d-c096-4b38-b78d-0fed6277096a" }
            @{ PropertyName = "Description"; Value = "Contains cmdlets that interact with Common Information Model (CIM) Servers like the Windows","Management Instrumentation (WMI) service.","This module is only available on the Windows platform." }
            @{ PropertyName = "Locale"; Value = "en-US" }
            @{ PropertyName = "OptionalElement"; Value = "" }
        ) {
            param ($propertyName, $value)
            $rawValue = $mf0.$propertyName
            if ($rawValue -match "`r" -or $rawValue -match "`n")
            {
                $expectedValue = $RawValue.Split([char[]]("`r","`n")).Where({$_})
            }
            else {
                $expectedValue = $mf0.$propertyName
            }

            $expectedValue | Should -Be $value

        }

        It "The commands property should have the correct count" {
            $mf0.Commands.Count | Should -Be 14
        }

        It "The command '<Offset>' properties should have the correct values" -TestCases $(
            @{ Offset = 0; Name = "Export-BinaryMiLog"; Link = "Export-BinaryMiLog.md"; Description = "Creates a binary encoded representation of an object or objects and stores it in a file." }
            @{ Offset = 1; Name = "Get-CimAssociatedInstance"; Link = "Get-CimAssociatedInstance.md"; Description = "Retrieves the CIM instances that are connected to a specific CIM instance by an association." }
            @{ Offset = 2; Name = "Get-CimClass"; Link = "Get-CimClass.md"; Description = "Gets a list of CIM classes in a specific namespace." }
            @{ Offset = 3; Name = "Get-CimInstance"; Link = "Get-CimInstance.md"; Description = "Gets the CIM instances of a class from a CIM server." }
            @{ Offset = 4; Name = "Get-CimSession"; Link = "Get-CimSession.md"; Description = "Gets the CIM session objects from the current session." }
            @{ Offset = 5; Name = "Import-BinaryMiLog"; Link = "Import-BinaryMiLog.md"; Description = "Used to re-create the saved objects based on the contents of an export file." }
            @{ Offset = 6; Name = "Invoke-CimMethod"; Link = "Invoke-CimMethod.md"; Description = "Invokes a method of a CIM class." }
            @{ Offset = 7; Name = "New-CimInstance"; Link = "New-CimInstance.md"; Description = "Creates a CIM instance." }
            @{ Offset = 8; Name = "New-CimSession"; Link = "New-CimSession.md"; Description = "Creates a CIM session." }
            @{ Offset = 9; Name = "New-CimSessionOption"; Link = "New-CimSessionOption.md"; Description = "Specifies advanced options for the ``New-CimSession`` cmdlet." }
            @{ Offset = 10; Name = "Register-CimIndicationEvent"; Link = "Register-CimIndicationEvent.md"; Description = "Subscribes to indications using a filter expression or a query expression." }
            @{ Offset = 11; Name = "Remove-CimInstance"; Link = "Remove-CimInstance.md"; Description = "Removes a CIM instance from a computer." }
            @{ Offset = 12; Name = "Remove-CimSession"; Link = "Remove-CimSession.md"; Description = "Removes one or more CIM sessions." }
            @{ Offset = 13; Name = "Set-CimInstance"; Link = "Set-CimInstance.md"; Description = "Modifies a CIM instance on a CIM server by calling the **ModifyInstance** method of the CIM class." }
        ) {
            param ($offset, $Name, $Link, $description )
            $mf0.Commands[$offset].Name | Should -Be $Name
            $mf0.Commands[$offset].Link | Should -Be $Link
            $mf0.Commands[$offset].Description | Should -Be $Description

        }
    }

    Context "Metadata content" {
        BeforeAll {
            $cimCmdletFile = $modFiles | Where-Object { $_.Name -eq "CimCmdlets.md" }
            $mf = Import-MarkdownModuleFile $cimCmdletFile
        }
            
        It "Should have non null metadata" {
            $mf.Metadata | Should -Not -BeNullOrEmpty
        }

        It "Should have the proper key '<key>' and value '<value>'" -TestCases $(
            @{ Key = "Download Help Link"; Value = "https://aka.ms/powershell75-help" }
            @{ Key = "Help Version"; Value = "7.5.0.0" }
            @{ Key = "Locale"; Value = "en-US" }
            @{ Key = "Module Guid"; Value = "fb6cc51d-c096-4b38-b78d-0fed6277096a" }
            @{ Key = "Module Name"; Value = "CimCmdlets" }
            @{ Key = "ms.date"; Value = "02/20/2019" }
            @{ Key = "PlatyPS schema version"; Value = "2024-05-01" }
            @{ Key = "title"; Value = "CimCmdlets Module" }
            @{ Key = "document type"; Value = "module" }
        ) {
            param ($key, $value)
            $mf.Metadata[$key] | Should -Be $value
        }
        
        It "The key 'schema' should not be present" {
            $mf.Metadata.Contains("schema") | Should -Be $false
        }
    }
}