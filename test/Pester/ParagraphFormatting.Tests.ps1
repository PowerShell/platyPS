# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

using namespace Microsoft.PowerShell.PlatyPS

Describe "ParagraphFormatHelper" {
    It "Formats paragraph with default settings" {
        $settings = [ParagraphFormatSettings]::new()
        $line = "This is a test paragraph. This is a test paragraph. This is a test paragraph. This is a test paragraph. This is a test paragraph."
        $expected = "This is a test paragraph. This is a test paragraph. This is a test paragraph.`n" +
                    "This is a test paragraph. This is a test paragraph."

        $result = [ParagraphFormatHelper]::FormatParagraph($line, $settings)

        $result | Should -Be $expected
    }

    It "Formats paragraph with indentation" {
        $settings = [ParagraphFormatSettings]::new(80, 4, 0)
        $line = "This is a test paragraph. This is a test paragraph. This is a test paragraph. This is a test paragraph. This is a test paragraph."
        $expected = "    This is a test paragraph. This is a test paragraph. This is a test`n" +
                    "    paragraph. This is a test paragraph. This is a test paragraph."

        $result = [ParagraphFormatHelper]::FormatParagraph($line, $settings)

        $result | Should -Be $expected
    }

    It "Formats paragraph with first line indentation" {
        $settings = [ParagraphFormatSettings]::new(80, 0, 4)
        $line = "This is a test paragraph. This is a test paragraph. This is a test paragraph. This is a test paragraph. This is a test paragraph."
        $expected = "    This is a test paragraph. This is a test paragraph. This is a test`n" +
                    "paragraph. This is a test paragraph. This is a test paragraph."
        $result = [ParagraphFormatHelper]::FormatParagraph($line, $settings)

        $result | Should -Be $expected
    }

    It "Formats paragraph with a very long word" {
        $settings = [ParagraphFormatSettings]::new(50, 4, 4)
        $line = "This is a test paragraph. This is a test paragraph. ThisIsAVeryLongWordWhichWillBeLongerThanTheAllowedLineLengthItShouldBeFormattedProperly. This is a test paragraph. This is a test paragraph. This is a test paragraph."
        $expected = "        This is a test paragraph. This is a test`n" + 
                    "    paragraph.`n" + 
                    "    ThisIsAVeryLongWordWhichWillBeLongerThanTheAllowedLineLengthItShouldBeFormattedProperly.`n" + 
                    "    This is a test paragraph. This is a test`n" + 
                    "    paragraph. This is a test paragraph."
        $result = [ParagraphFormatHelper]::FormatParagraph($line, $settings)
        $result | Should -Be $expected
    }

    It "Formats an array of strings into a paragraph" {
        $settings = [ParagraphFormatSettings]::new(50, 4, 4)
        $lines = "This is a test paragraph.", "This is a test paragraph.", "This is a test paragraph.", "This is a test paragraph.", "This is a test paragraph."
        $expected = "        This is a test paragraph. This is a test`n" +
                    "    paragraph. This is a test paragraph. This is`n" +
                    "    a test paragraph. This is a test paragraph." 
        $result = [ParagraphFormatHelper]::FormatParagraph($lines, $settings)
        $result | Should -Be $expected

    }
}