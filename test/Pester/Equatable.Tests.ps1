Describe "Test IEquatable" {
    BeforeAll {
        $assetDir = Join-Path $PSScriptRoot 'assets'
        $markdownPath1 = Join-Path $assetDir 'Get-Date.md'
        $markdownPath2 = Join-Path $assetDir 'Out-Null.md'
        $CommandHelpObject1 = Import-MarkdownCommandHelp -Path $markdownPath1
        $CommandHelpObject2 = Import-MarkdownCommandHelp -Path $markdownPath1
        $CommandHelpObject3 = Import-MarkdownCommandHelp -Path $markdownPath2
    }

    It "Should be equal to itself" {
        $CommandHelpObject1 -eq $CommandHelpObject1 | Should -Be $true
    }

    It "Should be equal to different instance of the same object" {
        $CommandHelpObject1 -eq $CommandHelpObject2 | Should -Be $true
    }

    It "Should be false for different files" {
        $CommandHelpObject2 -eq $CommandHelpObject3 | Should -Be $false
    }

    It "Small changes in markdown file '<FileName>' should result in inequality" -TestCases @(
        @{ FileName = "get-date.alt01.md" }
        @{ FileName = "get-date.alt02.md" }
        @{ FileName = "get-date.alt03.md" }
        @{ FileName = "get-date.alt04.md" }
        @{ FileName = "get-date.alt05.md" }
        @{ FileName = "get-date.alt06.md" }
        @{ FileName = "get-date.alt07.md" }
        @{ FileName = "get-date.alt08.md" }
        @{ FileName = "get-date.alt09.md" }
        @{ FileName = "get-date.alt10.md" }
        @{ FileName = "get-date.alt11.md" }
        @{ FileName = "get-date.alt12.md" }
        @{ FileName = "get-date.alt13.md" }
        @{ FileName = "get-date.alt14.md" }
        @{ FileName = "get-date.alt15.md" }
        @{ FileName = "get-date.alt16.md" }
        @{ FileName = "get-date.alt17.md" }
        @{ FileName = "get-date.alt18.md" }
        @{ FileName = "get-date.alt19.md" }
        @{ FileName = "get-date.alt20.md" }
        @{ FileName = "get-date.alt21.md" }
        @{ FileName = "get-date.alt22.md" }
        @{ FileName = "get-date.alt23.md" }
    ) {
        param ($FileName)
        $badMarkdown = Import-MarkdownCommandHelp -Path (Join-Path $assetDir $FileName)
        $CommandHelpObject1 -eq $badMarkdown | Should -Be $false
    }


    Context "Reflection based testing for individual properties because CommandHelp object is not public" {
        BeforeAll {
            $SynopsisProperty = $CommandHelpObject1.GetType().GetProperty('Synopsis', [System.Reflection.BindingFlags]'NonPublic,Instance')
            $SyntaxProperty = $CommandHelpObject1.GetType().GetProperty('Syntax', [System.Reflection.BindingFlags]'NonPublic,Instance')
            $AliasesProperty = $CommandHelpObject1.GetType().GetProperty('Aliases', [System.Reflection.BindingFlags]'NonPublic,Instance')
            $DescriptionProperty = $CommandHelpObject1.GetType().GetProperty('Description', [System.Reflection.BindingFlags]'NonPublic,Instance')
            $ExampleProperty = $CommandHelpObject1.GetType().GetProperty('Examples', [System.Reflection.BindingFlags]'NonPublic,Instance')
            $ParametersProperty = $CommandHelpObject1.GetType().GetProperty('Parameters', [System.Reflection.BindingFlags]'NonPublic,Instance')
            $InputProperty = $CommandHelpObject1.GetType().GetProperty('Inputs', [System.Reflection.BindingFlags]'NonPublic,Instance')
            $OutputProperty = $CommandHelpObject1.GetType().GetProperty('Outputs', [System.Reflection.BindingFlags]'NonPublic,Instance')
            $NotesProperty = $CommandHelpObject1.GetType().GetProperty('Notes', [System.Reflection.BindingFlags]'NonPublic,Instance')
            $RelatedLinksProperty = $CommandHelpObject1.GetType().GetProperty('RelatedLinks', [System.Reflection.BindingFlags]'NonPublic,Instance')
        }
    
        It "Should have the same Synopsis" {
            $SynopsisProperty.GetValue($CommandHelpObject1, $null) -eq $SynopsisProperty.GetValue($CommandHelpObject2, $null) | Should -Be $true
        }

        It "Should have the same Syntax" {
            $syntax1 = $SyntaxProperty.GetValue($CommandHelpObject1, $null)
            $syntax2 = $SyntaxProperty.GetValue($CommandHelpObject2, $null)
            $syntax1.Count | Should -Be $syntax2.Count
            for($i = 0; $i -lt $syntax1.Count; $i++) {
                $syntax1[$i] -eq $syntax2[$i] | Should -Be $true
            }
        }

        It "Should have the same Aliases" {
            $alias1 = $AliasesProperty.GetValue($CommandHelpObject1, $null)
            $alias2 = $AliasesProperty.GetValue($CommandHelpObject2, $null)
            $alias1.Count | Should -Be $alias2.Count
            for($i = 0; $i -lt $alias1.Count; $i++) {
                $alias1[$i] -eq $alias2[$i] | Should -Be $true
            }
        }

        It "Should have the same Description" {
            $DescriptionProperty.GetValue($CommandHelpObject1, $null) -eq $DescriptionProperty.GetValue($CommandHelpObject2, $null) | Should -Be $true
        }

        It "Should have the same Examples" {
            $example1 = $ExampleProperty.GetValue($CommandHelpObject1, $null)
            $example2 = $ExampleProperty.GetValue($CommandHelpObject2, $null)
            $example1.Count | Should -Be $example2.Count
            for($i = 0; $i -lt $example1.Count; $i++) {
                $example1[$i] -eq $example2[$i] | Should -Be $true
            }
        }

        It "Should have the same Parameters" {
            $parameter1 = $ParametersProperty.GetValue($CommandHelpObject1, $null)
            $parameter2 = $ParametersProperty.GetValue($CommandHelpObject2, $null)
            $parameter1.Count | Should -Be $parameter2.Count
            for($i = 0; $i -lt $parameter1.Count; $i++) {
                $parameter1[$i] -eq $parameter2[$i] | Should -Be $true
            }
        }

        It "Should have the same Inputs" {
            $input1 = $InputProperty.GetValue($CommandHelpObject1, $null)
            $input2 = $InputProperty.GetValue($CommandHelpObject2, $null)
            $input1.Count | Should -Be $input2.Count
            for($i = 0; $i -lt $input1.Count; $i++) {
                $input1[$i] -eq $input2[$i] | Should -Be $true
            }
        }

        It "Should have the same Outputs" {
            $output1 = $OutputProperty.GetValue($CommandHelpObject1, $null)
            $output2 = $OutputProperty.GetValue($CommandHelpObject2, $null)
            $output1.Count | Should -Be $output2.Count
            for($i = 0; $i -lt $output1.Count; $i++) {
                $output1[$i] -eq $output2[$i] | Should -Be $true
            }
        }

        It "Should have the same Notes" {
            $NotesProperty.GetValue($CommandHelpObject1, $null) -eq $NotesProperty.GetValue($CommandHelpObject2, $null) | Should -Be $true
        }

        It "Should have the same related links" {
            $link1 = $RelatedLinksProperty.GetValue($CommandHelpObject1, $null)
            $link2 = $RelatedLinksProperty.GetValue($CommandHelpObject2, $null)
            $link1.Count | Should -Be $link2.Count
            for($i = 0; $i -lt $link1.Count; $i++) {
                $link1[$i] -eq $link2[$i] | Should -Be $true
            }
        }

        It "Altering the description in a parameter will cause the objects to be different" {
            $parameter1 = $ParametersProperty.GetValue($CommandHelpObject1, $null)
            $parameter2 = $ParametersProperty.GetValue($CommandHelpObject2, $null)
            $parameter2[0].GetType().GetProperty("Description", [Reflection.BindingFlags]"NonPublic,Instance").SetValue($parameter2[0], "New Description")
            $parameter1[0] -eq $parameter2[0] | Should -Be $false
            $CommandHelpObject1 -eq $CommandHelpObject2 | Should -Be $false
        }
    }
}