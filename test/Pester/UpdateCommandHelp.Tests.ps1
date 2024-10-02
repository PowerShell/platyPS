# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

Describe "Tests for Update-CommandHelp" {
    BeforeAll {
        function test-update {
            <#
            .SYNOPSIS
            does something
            .DESCRIPTION
            a description
            .PARAMETER process
            process parameter description
            .EXAMPLE

            This is an example

            PS> get-process | test-update

            this is additional text.
            .LINK
            https://github.com/PowerShell/PowerShell
            #>
            [CmdletBinding()]
            [OutputType("System.Diagnostics.Process")]
            [OutputType("System.IO.FileInfo")]
            param (
                [Parameter(Position=0, ParameterSetName="process", ValueFromPipeline=$true)]
                [System.Diagnostics.Process]$process,
                [Parameter(Position=0, ParameterSetName="file", ValueFromPipelineByPropertyName=$true)]
                [System.IO.FileInfo]$file
            )

            PROCESS {
                if ($PSCmdlet.ParameterSetName -eq "process") {
                    $process
                }
                else {
                    $file
                }
            }
        }

        $tuCmdlet = Get-Command Test-Update
        $ch = New-CommandHelp -command $tuCmdlet
    }

    It "Missing parameters will be added" {
        $chCopy = [Microsoft.PowerShell.PlatyPS.Model.CommandHelp]::new($ch)
        $chCopy.Parameters.RemoveAt(0)
        $chCopy.Parameters.Count | Should -Be 1
        $chCopy.Parameters[0].Name | Should -Be "process"
        $helpFile = $chCopy | Export-MarkdownCommandHelp -output $TESTDRIVE -Force
        $chUpdate = $helpFile | Update-CommandHelp
        $chUpdate.Parameters.Count | Should -Be 2
        $chUpdate.Parameters.Name | Should -Be @("file", "process")
    }

    It "Missing Inputs should be added" {
        $chCopy = [Microsoft.PowerShell.PlatyPS.Model.CommandHelp]::new($ch)
        $chCopy.Inputs.RemoveAt(0)
        $chCopy.Inputs.Count | Should -Be 1
        $helpFile = $chCopy | Export-MarkdownCommandHelp -output $TESTDRIVE -Force
        $chUpdate = $helpFile | Update-CommandHelp
        $chUpdate.Inputs.Count | Should -Be 2
        $chUpdate.Outputs.Typename | Should -Be @("System.Diagnostics.Process", "System.IO.FileInfo")
    }

    It "Missing Outputs should be added" {
        $chCopy = [Microsoft.PowerShell.PlatyPS.Model.CommandHelp]::new($ch)
        $chCopy.Inputs.Clear()
        $chCopy.Inputs.Count | Should -Be 0
        $helpFile = $chCopy | Export-MarkdownCommandHelp -output $TESTDRIVE -Force
        $chUpdate = $helpFile | Update-CommandHelp
        $chUpdate.Inputs.Count | Should -Be 2
        $chUpdate.Outputs.Typename | Should -Be @("System.Diagnostics.Process", "System.IO.FileInfo")
    }

    It "No updates should have no changes" -skip:($IsWindows -or $PSVersionTable.PSVersion.Major -eq 5) {
        $chCopy = [Microsoft.PowerShell.PlatyPS.Model.CommandHelp]::new($ch)
        $chCopy | Should -Be $ch
        $helpFile = $chCopy | Export-MarkdownCommandHelp -output $TESTDRIVE -Force
        $chUpdate = $helpFile | Update-CommandHelp
        $chUpdate | Should -Be $ch
    }

    It "Should not update the aliases element with boiler plate" {
        $ch = Update-CommandHelp -Path ([io.path]::Combine($PSScriptRoot, "assets", "get-date.md"))
        $ch.AliasHeaderFound | Should -Be $true
    }

    Context "Parameter attribute updates" {
        BeforeAll {
                function test-update {
                <#
                .SYNOPSIS
                does something
                .DESCRIPTION
                a description
                .PARAMETER process
                process parameter description
                .EXAMPLE

                This is an example

                PS> get-process | test-update

                this is additional text.
                .LINK
                https://github.com/PowerShell/PowerShell
                #>
                [CmdletBinding()]
                [OutputType("System.Diagnostics.Process")]
                [OutputType("System.IO.FileInfo")]
                param (
                    [Parameter(Position=0, ParameterSetName="process", ValueFromPipeline=$true)]
                    [System.Diagnostics.Process]$process,
                    [Parameter(Position=0, ParameterSetName="file", ValueFromPipelineByPropertyName=$true)]
                    [SupportsWildcards()]
                    [System.IO.FileInfo]$file
                )

                PROCESS {
                    if ($PSCmdlet.ParameterSetName -eq "process") {
                        $process
                    }
                    else {
                        $file
                    }
                }
            }
        }

        It "Should update the parameter attribute with SupportsWildcards" {
            $oCH = $tuCmdlet | New-CommandHelp
            $oCH.Parameters.Where({$_.Name -eq "file"}).SupportsWildCards -eq $false
            $testUpdate = $oCH | Export-MarkdownCommandHelp -output "${TESTDRIVE}/testupdate"
            $upch = Update-CommandHelp -Path $testUpdate
            $upch.Parameters.Where({$_.Name -eq "file"}).SupportsWildCards -eq $true
        }
    }
}