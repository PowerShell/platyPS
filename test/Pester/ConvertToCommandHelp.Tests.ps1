# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

Describe "New-CommandHelp tests" {
    BeforeAll {
        $result = New-CommandHelp New-CommandHelp
        $commonParameters = 'Debug', 'ErrorAction', 'ErrorVariable', 'InformationAction', 'InformationVariable',
            'OutBuffer', 'OutVariable', 'PipelineVariable', 'ProgressAction', 'Verbose', 'WarningAction', 'WarningVariable'
    }

    It "Should return a command help object" {
        $result | Should -BeOfType "Microsoft.PowerShell.PlatyPS.Model.CommandHelp"
    }

    It "Should have the appropriate metadata properties and values '<key> = <value>'" -TestCases @(
        @{ Key = "document type"; Value = "cmdlet" }
        @{ Key = "title"; Value = "New-CommandHelp" }
        @{ Key = "Module Name"; Value = "microsoft.powershell.platyPS" }
        @{ Key = "Locale" ; Value = "en-US" }
        @{ Key = "PlatyPS schema version"; Value = "2024-05-01" }
        @{ Key = "HelpUri"; Value = "" }
        @{ Key = "ms.date"; Value = Get-Date -Format "MM/dd/yyyy" }
        @{ Key = "external help file"; Value = "Microsoft.PowerShell.PlatyPS.dll-Help.xml" }
    ) {
        param ($key, $value)
        $result.Metadata[$key] | Should -Be $value
    }

    It "Should be possible to retrieve from multiple commands" {
        $cmdInfo = Get-Command -Module Microsoft.PowerShell.PlatyPS
        $chs = New-CommandHelp $cmdInfo
        $chs.Count | Should -Be $cmdInfo.Count
        $chs.title | Should -Be $cmdInfo.Name
    }

    It "Should be possible to retrieve from multiple commands via pipeline" {
        $cmdInfo = Get-Command -Module Microsoft.PowerShell.PlatyPS
        $chs = $cmdInfo | New-CommandHelp
        $chs.Count | Should -Be $cmdInfo.Count
        $chs.title | Should -Be $cmdInfo.Name
    }

    Context "Parameter validation" {
        BeforeAll {
            $cmd = Get-Command Get-Command
            $ch = $cmd | New-CommandHelp
            $parameters = $ch.Parameters
        }

        It "should have the proper number of parameters" {
            $parameters.Count | Should -Be 17
        }

        # list retrieved from get-command
        It "should have the '<Name>' parameter with the property type '<type>'" -TestCases @(
            @{ Name = 'All'; Type = 'System.Management.Automation.SwitchParameter' },
            @{ Name = 'ArgumentList'; Type = 'System.Object[]' },
            @{ Name = 'CommandType'; Type = 'System.Management.Automation.CommandTypes' },
            @{ Name = 'FullyQualifiedModule'; Type = 'Microsoft.PowerShell.Commands.ModuleSpecification[]' },
            @{ Name = 'FuzzyMinimumDistance'; Type = 'System.UInt32' },
            @{ Name = 'ListImported'; Type = 'System.Management.Automation.SwitchParameter' },
            @{ Name = 'Module'; Type = 'System.String[]' },
            @{ Name = 'Name'; Type = 'System.String[]' },
            @{ Name = 'Noun'; Type = 'System.String[]' },
            @{ Name = 'ParameterName'; Type = 'System.String[]' },
            @{ Name = 'ParameterType'; Type = 'System.Management.Automation.PSTypeName[]' },
            @{ Name = 'ShowCommandInfo'; Type = 'System.Management.Automation.SwitchParameter' },
            @{ Name = 'Syntax'; Type = 'System.Management.Automation.SwitchParameter' },
            @{ Name = 'TotalCount'; Type = 'System.Int32' },
            @{ Name = 'UseAbbreviationExpansion'; Type = 'System.Management.Automation.SwitchParameter' },
            @{ Name = 'UseFuzzyMatching'; Type = 'System.Management.Automation.SwitchParameter' },
            @{ Name = 'Verb'; Type = 'System.String[]' }
        ) {
            param ($Name, $type)
            $ch.Parameters.Name | Should -Contain $Name
            $ch.Parameters.Where({$_.name -eq $Name}).Type | Should -Be $type
        }

        # list retrieved from get-command
        It "should have the '<Name>' parameter with the proper alias '<alias>'" -TestCases @(
            @{ Name = 'All'; alias = '' },
            @{ Name = 'ArgumentList'; alias = 'Args' },
            @{ Name = 'CommandType'; alias = 'Type' },
            @{ Name = 'FullyQualifiedModule'; alias = '' },
            @{ Name = 'FuzzyMinimumDistance'; alias = '' },
            @{ Name = 'ListImported'; alias = '' },
            @{ Name = 'Module'; alias = 'PSSnapin' },
            @{ Name = 'Name'; alias = '' },
            @{ Name = 'Noun'; alias = '' },
            @{ Name = 'ParameterName'; alias = '' },
            @{ Name = 'ParameterType'; alias = '' },
            @{ Name = 'ShowCommandInfo'; alias = '' },
            @{ Name = 'Syntax'; alias = '' },
            @{ Name = 'TotalCount'; alias = '' },
            @{ Name = 'UseAbbreviationExpansion'; alias = '' },
            @{ Name = 'UseFuzzyMatching'; alias = '' },
            @{ Name = 'Verb'; alias = '' }
        ) {
            param ($Name, $alias)
            [string]($ch.Parameters.Where({$_.name -eq $Name}).Aliases) | Should -Be $alias
        }

        It "Should have the same parameter sets (excluding '(All)')" {
            $expected = $cmd.ParameterSets.Name | Sort-Object
            $observed = $ch.Parameters.ParameterSets.Name | Sort-Object -Unique | Where-Object { $_ -ne "(All)" }
            $observed | Should -Be $expected
        }

        It "Should have the proper parameters in parameterset '<ParameterSetName>'"  -TestCases @(
            @{ ParameterSetName = 'CmdletSet';     Parameters = 'All:ArgumentList:FullyQualifiedModule:ListImported:Module:Noun:ParameterName:ParameterType:ShowCommandInfo:Syntax:TotalCount:Verb' }
            @{ ParameterSetName = 'AllCommandSet'; Parameters = 'All:ArgumentList:CommandType:FullyQualifiedModule:FuzzyMinimumDistance:ListImported:Module:Name:ParameterName:ParameterType:ShowCommandInfo:Syntax:TotalCount:UseAbbreviationExpansion:UseFuzzyMatching' }
        ) {
            param ($ParameterSetName, $Parameters)
            $observedParameters = ($ch.parameters.Where({$_.parametersets.Name -match "$ParameterSetName|\(All\)"})|sort-object name).name -join ":"
            $observedParameters | Should -Be $Parameters
        }

        # taken from get-command output and translated property names
        It "Value for Parameter All '<PropertyName>' should be '<ExpectedValue>'" -TestCases @(
            @{ PropertyName = "Position"; ExpectedValue = "Named" }
            @{ PropertyName = "IsRequired"; ExpectedValue = $false } # IsMandatory
            @{ PropertyName = "ValueFromPipeline"; ExpectedValue = $false }
            @{ PropertyName = "ValueFromPipelineByPropertyName"; ExpectedValue = $true }
            @{ PropertyName = "ValueFromRemainingArguments"; ExpectedValue = $False }
        ) {
            param ($PropertyName, $ExpectedValue )
            $pSet = $ch.parameters.Where({$_.Name -eq "All"}).ParameterSets[0]
            $pSet.$propertyName | Should -Be $ExpectedValue
        }
    }

    Context "Input/Output Validation" {
        BeforeAll {
            $cmd = Get-Command Get-Command
            $ch = $cmd | New-CommandHelp
        }

        It "Should have the proper number of output types" {
            $ch.outputs.Count | should be 8
        }

        It "Should have the output type '<type>'" -TestCases @(
            @{ Type = 'System.Management.Automation.AliasInfo' }
            @{ Type = 'System.Management.Automation.ApplicationInfo' }
            @{ Type = 'System.Management.Automation.FunctionInfo' }
            @{ Type = 'System.Management.Automation.CmdletInfo' }
            @{ Type = 'System.Management.Automation.ExternalScriptInfo' }
            @{ Type = 'System.Management.Automation.FilterInfo' }
            @{ Type = 'System.String' }
            @{ Type = 'System.Management.Automation.PSObject' }
        ) {
            param ($type)
            $ch.outputs.typename | Should -Contain $type
        }

        It "Should have the proper number of input types" {
            Set-ItResult -Pending -Because "Get-Help erroneously includes parameters which have FromRemainingArguments as pipeline input"
            $ch.inputs.count | Should -Be 5 -Because "$($ch.inputs.typename -join ':')"
        }

        It "Should have the input type '<type>'" -TestCases @(
            @{ Type = "System.Management.Automation.CommandTypes" }
            @{ Type = "System.Int32" }
            @{ Type = "Microsoft.PowerShell.Commands.ModuleSpecification" }
            @{ Type = "System.String" }
            @{ Type = "System.Management.Automation.SwitchParameter" }
        ) {
            param ($type)
            $ch.inputs.typename | Should -Contain $type
        }
    }

}