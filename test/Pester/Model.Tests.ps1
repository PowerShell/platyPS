Describe "Model type tests" {
    BeforeAll {
        $cmdInfo = get-command new-markdownhelp
        $commandHelpType = $cmdInfo.ImplementingType.Assembly.GetType("Microsoft.PowerShell.PlatyPS.Model.CommandHelp")     
        $ObjectProperties = @(
            @{ type = 'System.Globalization.CultureInfo'; Nullable = $false; Name = "Locale" }
            @{ type = 'System.Nullable`1[System.Guid]'; Nullable = $true; Name = "ModuleGuid" }
            @{ type = 'System.String'; Nullable = $true;  Name = "ExternalHelpFile" }
            @{ type = 'System.String'; Nullable = $true;  Name = "OnlineVersionUrl" }
            @{ type = 'System.String'; Nullable = $true;  Name = "SchemaVersion" }
            @{ type = 'System.String'; Nullable = $false; Name = "ModuleName" }
            @{ type = 'System.String'; Nullable = $false; Name = "Title" }
            @{ type = 'System.String'; Nullable = $false; Name = "Synopsis" }
            @{ type = 'System.Collections.Generic.List`1[Microsoft.PowerShell.PlatyPS.Model.SyntaxItem]'; Nullable = $false; Name = "Syntax" }
            @{ type = 'System.Collections.Generic.List`1[System.String]'; Nullable = $true; Name = "Aliases" }
            @{ type = 'System.String'; Nullable = $true; Name = "Description" }
            @{ type = 'System.Collections.Generic.List`1[Microsoft.PowerShell.PlatyPS.Model.Example]'; Nullable = $true; Name = "Examples" }
            @{ type = 'System.Collections.Generic.List`1[Microsoft.PowerShell.PlatyPS.Model.Parameter]'; Nullable = $false; Name = "Parameters" }
            @{ type = 'System.Collections.Generic.List`1[Microsoft.PowerShell.PlatyPS.Model.InputOutput]'; Nullable = $true; Name = "Inputs" }
            @{ type = 'System.Collections.Generic.List`1[Microsoft.PowerShell.PlatyPS.Model.InputOutput]'; Nullable = $true; Name = "Outputs" }
            @{ type = 'System.Collections.Generic.List`1[Microsoft.PowerShell.PlatyPS.Model.Links]'; Nullable = $true; Name = "RelatedLinks" }
            @{ type = 'System.Boolean'; Name = "HasCmdletBinding" }
            @{ type = 'System.String'; Nullable = $true;  Name = "Notes" }
        )
        $BindingFlags = [System.Reflection.BindingFlags]"Instance,NonPublic"
    }

    It "CommandHelp has the correct type information for '<name>'" -TestCases $ObjectProperties {
        param ( $name, $type, $nullable)
        $property = $commandHelpType.GetProperty($name, $BindingFlags)
        $property.PropertyType.ToString() | Should -BeExactly $type
    }
    
    
}
