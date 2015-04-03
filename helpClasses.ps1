class HelpBase
{
    [string]ToMD() {return [string]::Empty }
    [string]ToCBH() {return [string]::Empty }
    [string]ToMaml() {return [string]::Empty }
    [string]ToHtml() { return [string]::Empty }
    [string]ToText() { return [string]::Empty }
    [string[]]Render([RenderType]$type)
    {
        switch ($type)
        {
            "Maml"             { return $this.ToMD();   break }
            "CommentBasedHelp" { return $this.ToCBH();  break }
            "MarkDown"         { return $this.ToMaml(); break }
            "Html"             { return $this.ToHtml(); break }
            "Text"             { return $this.ToText(); break }
        }
        return $null
            
    }
}
class CommandName : HelpBase
{
    [string]$Data
    CommandName ([string]$name )
    {
        $this.Data = $name
    }
    [string[]]ToText()
    {
        return "COMMAND",("    " + $this.Data)
    }
}
class Synopsis : HelpBase
{
    [string[]]$Data
    [string[]]ToText()
    {
        return ($this.Data | %{"SYNOPSIS"}{"    $_"})
    }
}
class Syntax: HelpBase
{
    [string[]]$Data
    [string[]]ToText()
    {
        return ($this.Data|%{"SYNTAX"}{"    $_"})
    }
}
class Description : HelpBase
{
    [string]$Data
}
class Parameter : HelpBase
{
    [string]$Name
    [string]$Type
    [string[]]$Data
}
class Input : HelpBase
{
    [Type]$Type
    [string]$Data
}
class Output : HelpBase
{
    [Type]$Type
    [string]$Data
}

Class Notes : HelpBase
{
    [string[]]$Data
}
Class Hyperlink 
{
    [string]$Data
    [string]$Link
    [string]ToString() { return "<a href='$this.Link'>$this.Data</a>" }
}
Class Example : HelpBase
{
    [string]$Command
    [string]$Description
    [string]$Output
}

enum RenderType
{
    Maml
    CommentBasedHelp
    MarkDown
    Html
    Text
}

class Help : HelpBase
{
    [CommandName]$Name
    [synopsis]$Synopsis
    [Syntax]$Syntax
    [Description]$Description
    [Parameter[]]$Parameters
    [Input[]]$Input
    [Output[]]$Output
    [HyperLink[]]$Links
    [string]Render([RenderType]$type)
    {
        $sb= new-object System.Text.StringBuilder
        if ( $type -eq "Maml" )
        {
            # Maml Header
        }
        
        $Sections = "Name","Synopsis","Syntax","Description","Parameters",
            "Input","Output","Links"

        foreach($section in $Sections)
        {
            if ( $this.$section -ne $null )
            {
                $this.$section.Render($type) |%{ } { $sb.AppendLine($_) } { $sb.AppendLine("") }
            }
        }

        if ( $type -eq "Maml" )
        {
            # Maml Footer
        }
        return $sb.ToString()
        
    }
}

$help = [Help]::New()
$help.Name = [CommandName]::New("Get-Help")
$help.Synopsis = [Synopsis]::New();
$help.Synopsis.Data = "Displays information about Windows PowerShell commands and concepts."
$help.Syntax = [Syntax]::New()
$help.Syntax.Data = "Get-Help [[-Name] <String>] [-Category <String[]>] [-Component <String[]>] [-Full] [-Functionality <String[]>] [-Path <String>] [-Role <String[]>] [<CommonParameters>]",
    "Get-Help [[-Name] <String>] [-Category <String[]>] [-Component <String[]>] [-Functionality <String[]>] [-Path <String>] [-Role <String[]>] -Detailed [<CommonParameters>]",
    "Get-Help [[-Name] <String>] [-Category <String[]>] [-Component <String[]>] [-Functionality <String[]>] [-Path <String>] [-Role <String[]>] -Examples [<CommonParameters>]",
    "Get-Help [[-Name] <String>] [-Category <String[]>] [-Component <String[]>] [-Functionality <String[]>] [-Path <String>] [-Role <String[]>] -Online [<CommonParameters>]",
    "Get-Help [[-Name] <String>] [-Category <String[]>] [-Component <String[]>] [-Functionality <String[]>] [-Path <String>] [-Role <String[]>] -Parameter <String> [<CommonParameters>]",
    "Get-Help [[-Name] <String>] [-Category <String[]>] [-Component <String[]>] [-Functionality <String[]>] [-Path <String>] [-Role <String[]>] -ShowWindow [<CommonParameters>]",
    "Get-Help [[-Name] <String>] [-Category <String[]>] [-Component <String[]>] [-Full] [-Functionality <String[]>] [-Path <String>] [-Role <String[]>] [<CommonParameters>]",
    "Get-Help [[-Name] <String>] [-Category <String[]>] [-Component <String[]>] [-Functionality <String[]>] [-Path <String>] [-Role <String[]>] -Detailed [<CommonParameters>]",
    "Get-Help [[-Name] <String>] [-Category <String[]>] [-Component <String[]>] [-Functionality <String[]>] [-Path <String>] [-Role <String[]>] -Examples [<CommonParameters>]",
    "Get-Help [[-Name] <String>] [-Category <String[]>] [-Component <String[]>] [-Functionality <String[]>] [-Path <String>] [-Role <String[]>] -Online [<CommonParameters>]",
    "Get-Help [[-Name] <String>] [-Category <String[]>] [-Component <String[]>] [-Functionality <String[]>] [-Path <String>] [-Role <String[]>] -Parameter <String> [<CommonParameters>]",
    "Get-Help [[-Name] <String>] [-Category <String[]>] [-Component <String[]>] [-Functionality <String[]>] [-Path <String>] [-Role <String[]>] -ShowWindow [<CommonParameters>]"

$help.Render("Text")

