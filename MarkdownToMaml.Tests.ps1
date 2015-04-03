Set-StrictMode -Version Latest

enum TokenType
{
    HeaderL1
    HeaderL2
    HeaderL3
    HeaderL4

    Code

    Word

    HyperLink
}

function GetHeaderByLevel([int]$level)
{
    switch ($level)
    {
        1 { return [TokenType]::HeaderL1 }
        2 { return [TokenType]::HeaderL2 }
        3 { return [TokenType]::HeaderL3 }
        4 { return [TokenType]::HeaderL4 }
        default {
            throw "Illegal header level: $level"
        }
    }
}

class Extent
{
    [int]$startOffset
    [int]$endOffset
}

class Token
{
    [TokenType] $tokenType
    [string] $text
    [Extent] $extent
}

class Tokenizer
{
    hidden [string] $inputText;
    hidden [int] $offset;

    Tokenizer([string]$inputText) 
    {
        $this.inputText = $inputText;
        $this.offset = 0;
    }

    hidden [void] skipLineBreak()
    {
        #todo
    }

    hidden [string] readWord()
    {
        $startOffset = $this.offset
        while ($this.offset -lt $this.inputText.Length -and (-not ($this.inputText[$this.offset] -match '\s'))) 
        {
            $this.offset++
        }
        while ($this.offset -lt $this.inputText.Length -and ($this.inputText[$this.offset] -match '\s')) 
        {
            $this.offset++
        }

        $word = $this.inputText.Substring($startOffset, $this.offset - $startOffset)
        return $word.TrimEnd(@("`r", "`n"))
    }

    hidden [Token] readHeader()
    {
        $startOffset = $this.offset
        $level = 0
        while($this.inputText[$this.offset] -eq '#')
        {
            $level++
            $this.offset++
        }
        return [Token] @{
            TokenType = GetHeaderByLevel $level
            Text = $this.readWord()
            Extent = @{
                startOffset = $startOffset
                endOffset = $this.offset
            } 
        }
    }

    [Token] nextToken()
    {
        $startOffset = $this.offset
        
        if ($startOffset -ge $this.inputText.Length) {
            return $null
        }

        switch($this.inputText[$this.offset])
        {
            # TODO: This is a cheap man version. We need more graceful handling for line-endings. 
            #"`r" {
            #    $this.offset++
            #    return $this.nextToken()
            #}
            #"`n" {
            #    $this.offset++
            #    return $this.nextToken()
            #}

            '#' { return $this.readHeader() }
            '`' {
                if ($this.inputText.Substring($this.offset, 3) -eq '```') {
                    return $this.readCode()
                } 
            }
            '[' {
                return $this.readHyperLink()
            }
            default {
                return [Token] @{
                    TokenType = [TokenType]::Word
                    Text = $this.readWord()
                    Extent = @{
                        startOffset = $startOffset
                        endOffset = $this.offset
                    } 
                }
            }
        }
    }
}

Describe 'Tokenizer' {
    Context 'Simple inputs' {
        It 'should parse HeaderL1' {
            $inputText = '#Hello'
            $tokenizer = [Tokenizer]::new($inputText);
            $t = $tokenizer.nextToken()
            $t.tokenType | Should be ([TokenType]::HeaderL1)   
            $t.text | Should be 'Hello'
            $t.extent.startOffset | Should be 0
            $t.extent.endOffset | Should be 6
            $tokenizer.nextToken() | Should be $null   
        }

        It 'should parse name and SYNOPSIS' {
            $inputText = @'
##Get-Foo
###SYNOPSIS
This is synopsis.
'@
            $tokenizer = [Tokenizer]::new($inputText);
            $t = $tokenizer.nextToken()
            $t.tokenType | Should be ([TokenType]::HeaderL2)   
            $t.text | Should be 'Get-Foo'
            
            $t = $tokenizer.nextToken()
            $t.text | Should be 'SYNOPSIS'
            $t.tokenType | Should be ([TokenType]::HeaderL3)   
            
            $t = $tokenizer.nextToken()
            $t.tokenType | Should be ([TokenType]::Word)   
            $t.text | Should be 'This '
            
            $t = $tokenizer.nextToken()
            $t.tokenType | Should be ([TokenType]::Word)   
            $t.text | Should be 'is '
            
            $t = $tokenizer.nextToken()
            $t.tokenType | Should be ([TokenType]::Word)   
            $t.text | Should be 'synopsis.'
            
            $tokenizer.nextToken() | Should be $null      
        }
    }
}