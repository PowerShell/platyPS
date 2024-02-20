Describe "Test IEquatable" {
    BeforeAll {
        $assetDir = Join-Path $PSScriptRoot 'assets'
        $markdownPath1 = Join-Path $assetDir 'get-date.md'
        $markdownPath2 = Join-Path $assetDir 'Out-Null.md'
        $CommandHelpObject1 = Import-MarkdownCommandHelp -Path $markdownPath1
        $CommandHelpObject2 = Import-MarkdownCommandHelp -Path $markdownPath1
        $CommandHelpObject3 = Import-MarkdownCommandHelp -Path $markdownPath2
    }

    It "Should be equal to itself" {
        $CommandHelpObject1 -eq $CommandHelpObject1 | Should -Be $true
    }

    It "Should be equal to a different instance generated from the same object" {
        $CommandHelpObject1 -eq $CommandHelpObject2 | Should -Be $true
    }

    It "Should be false for different files" {
        $CommandHelpObject2 -eq $CommandHelpObject3 | Should -Be $false
    }

    It "Small changes in markdown file '<FileName>' should result in inequality" -TestCases @(
        @{ FileName = "get-date.alt01.md" ; expectedResult = $false }
        @{ FileName = "get-date.alt02.md" ; expectedResult = $false }
        @{ FileName = "get-date.alt03.md" ; expectedResult = $false }
        @{ FileName = "get-date.alt04.md" ; expectedResult = $false }
        @{ FileName = "get-date.alt05.md" ; expectedResult = $false }
        @{ FileName = "get-date.alt06.md" ; expectedResult = $false }
        @{ FileName = "get-date.alt07.md" ; expectedResult = $false }
        @{ FileName = "get-date.alt08.md" ; expectedResult = $false }
        @{ FileName = "get-date.alt09.md" ; expectedResult = $false }
        @{ FileName = "get-date.alt10.md" ; expectedResult = $false }
        @{ FileName = "get-date.alt11.md" ; expectedResult = $false }
        @{ FileName = "get-date.alt12.md" ; expectedResult = $false }
        @{ FileName = "get-date.alt13.md" ; expectedResult = $false }
        @{ FileName = "get-date.alt14.md" ; expectedResult = $false }
        @{ FileName = "get-date.alt15.md" ; expectedResult = $false }
        @{ FileName = "get-date.alt16.md" ; expectedResult = $false }
        @{ FileName = "get-date.alt17.md" ; expectedResult = $false }
        @{ FileName = "get-date.alt18.md" ; expectedResult = $false }
        @{ FileName = "get-date.alt19.md" ; expectedResult = $false }
        @{ FileName = "get-date.alt20.md" ; expectedResult = $false }
        @{ FileName = "get-date.alt21.md" ; expectedResult = $false }
        @{ FileName = "get-date.alt22.md" ; expectedResult = $false }
        @{ FileName = "get-date.alt23.md" ; expectedResult = $false }
        @{ FileName = "get-date.alt24.md" ; expectedResult = $true } # extra lines before or after header does not cause inequality
        @{ FileName = "get-date.alt25.md" ; expectedResult = $true } # extra whitespace after header text does not cause inequality
    ) {
        param ($FileName, $expectedResult)
        $badMarkdown = Import-MarkdownCommandHelp -Path (Join-Path $assetDir $FileName)
        $CommandHelpObject1 -eq $badMarkdown | Should -Be $expectedResult
    }


    Context "Reflection based testing for individual properties because CommandHelp object is not public" {
    
        It "Should have the same Synopsis" {
            $CommandHelpObject1.Synopsis -eq $CommandHelpObject2.Synopsis | Should -Be $true
        }

        It "Should have the same Syntax" {
            $syntax1 = $CommandHelpObject1.Syntax
            $syntax2 = $CommandHelpObject2.Syntax
            $syntax1.Count | Should -Be $syntax2.Count
            for($i = 0; $i -lt $syntax1.Count; $i++) {
                $syntax1[$i] -eq $syntax2[$i] | Should -Be $true
            }
        }

        It "Should have the same Aliases" {
            $alias1 = $CommandHelpObject1.Aliases
            $alias2 = $CommandHelpObject2.Aliases
            $alias1.Count | Should -Be $alias2.Count
            for($i = 0; $i -lt $alias1.Count; $i++) {
                $alias1[$i] -eq $alias2[$i] | Should -Be $true
            }
        }

        It "Should have the same Description" {
            $CommandHelpObject1.Description | Should -Be $CommandHelpObject2.Description
        }

        It "Should have the same Examples" {
            $example1 = $CommandHelpObject1.Examples
            $example2 = $CommandHelpObject2.Examples
            $example1.Count | Should -Be $example2.Count
            for($i = 0; $i -lt $example1.Count; $i++) {
                $example1[$i] -eq $example2[$i] | Should -Be $true
            }
        }

        It "Should have the same Parameters" {
            $parameter1 = $CommandHelpObject1.Parameters
            $parameter2 = $CommandHelpObject2.Parameters
            $parameter1.Count | Should -Be $parameter2.Count
            for($i = 0; $i -lt $parameter1.Count; $i++) {
                $parameter1[$i] -eq $parameter2[$i] | Should -Be $true
            }
        }

        It "Should have the same Inputs" {
            $input1 = $CommandHelpObject1.Inputs
            $input2 = $CommandHelpObject2.Inputs
            $input1.Count | Should -Be $input2.Count
            for($i = 0; $i -lt $input1.Count; $i++) {
                $input1[$i] -eq $input2[$i] | Should -Be $true
            }
        }

        It "Should have the same Outputs" {
            $output1 = $CommandHelpObject1.Outputs
            $output2 = $CommandHelpObject2.Outputs
            $output1.Count | Should -Be $output2.Count
            for($i = 0; $i -lt $output1.Count; $i++) {
                $output1[$i] -eq $output2[$i] | Should -Be $true
            }
        }

        It "Should have the same Notes" {
            $CommandHelpObject1.Notes | Should -BeExactly $CommandHelpObject2.Notes
        }

        It "Should have the same related links" {
            $link1 = $CommandHelpObject1.RelatedLinks
            $link2 = $CommandHelpObject2.RelatedLinks
            $link1.Count | Should -Be $link2.Count
            for($i = 0; $i -lt $link1.Count; $i++) {
                $link1[$i] -eq $link2[$i] | Should -Be $true
            }
        }

        It "Altering the description in a parameter will cause the objects to be different" {
            $parameter1 = $CommandHelpObject1.Parameters
            $parameter2 = $CommandHelpObject2.Parameters
            $parameter2[0].Description = "New Description"
            $parameter1[0] | Should -Not -Be $parameter2[0]
            $CommandHelpObject1 | Should -Not -Be $CommandHelpObject2
        }
    }
}
