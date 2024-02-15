#using namespace yamldotnet
#using namespace yamldotnet.Serialization
#using namespace System.Collections.Generic
#using namespace System.Collections.Specialized
#using namespace System.Management.Automation

$repoRoot = git rev-parse --show-toplevel
$modRoot = Join-Path $repoRoot "out/PlatyPS"
$depRoot = Join-Path $modRoot "Dependencies"
$markDigAsm = Join-Path $depRoot "Markdig.Signed.dll"
$yamlDotNetAsm = Join-Path $depRoot "YamlDotNet.dll"
import-Module $markDigAsm
import-Module $yamlDotNetAsm

class inputOutput {
    [string]$name
    [string]$description
}

class parameter {
    [string]$name
    [string]$type
    [string]$description
    [string]$defaultValue
    [string]$pipelineInput
    [string]$position
    [string]$aliases
    [string]$parameterValueGroup
}

class example {
    [string]$title
    [string]$description
    [string]$summary
}

class link {
    [string]$href
    [string]$text

}
class ch {
    [System.Collections.Specialized.OrderedDictionary]$metadata
    [Globalization.CultureInfo]$Locale
    [Guid]$ModuleGuid
    [string]$ExternalHelpfile
    [string]$OnlineVersionUrl
    [string]$SchemaVersion
    [string]$ModuleName
    [string]$title
    [bool]$HasCmdletBinding
    [bool]$HasWorkflowCommonParameters
    [string]$synopsis
    [System.Collections.Generic.List[string]]$syntaxes
    [string]$aliases
    [string]$description
    [System.Collections.Generic.list[example]]$examples
    [System.Collections.Generic.list[parameter]]$parameters
    [System.Collections.Generic.list[inputOutput]]$inputs
    [System.Collections.Generic.list[inputOutput]]$outputs
    [string]$notes
    [System.Collections.Generic.list[link]]$links

    ch() {
        $this.metadata = [System.Collections.Specialized.OrderedDictionary]::new()
        $this.Syntaxes = [System.Collections.Generic.List[string]]::new()
        $this.examples = [System.Collections.Generic.List[example]]::new()
        $this.parameters = [System.Collections.Generic.List[parameter]]::new()
        $this.inputs = [System.Collections.Generic.List[inputOutput]]::new()
        $this.inputs = [System.Collections.Generic.List[inputOutput]]::new()
        $this.links = [System.Collections.Generic.List[link]]::new()
    }

    [string]ToString() {
        return $this.title
    }
}

$script:yamldes = [YamlDotNet.Serialization.DeserializerBuilder]::new().Build()
[type[]]$tlist = @( [string] )
$script:YamlDeserializeMethod = $yamldes.GetType().GetMethod("Deserialize", $tlist).MakeGenericMethod([ch])

function Import-CommandYaml  {
    [CmdletBinding()]
    param (
        [System.Management.Automation.ParameterAttribute(ValueFromPipelineByPropertyName=$true,ValueFromPipeline=$true,Position=0,Mandatory=$true)]
        [string[]]$fullname
    )

    PROCESS {
        foreach($yamlFile in $fullname) {
            $yaml = Get-Content -raw $yamlFile
            try {
                $result = $YamlDeserializeMethod.Invoke($yamldes, $yaml)
            }
            catch {
                Write-Warning "Failed to parse $yamlFile ($_)"
                return
            }

            # fix up some of the object elements
            $result.parameters.where({$_.name -eq "CommonParameters"}).Foreach({$result.parameters.Remove($_); $result.HasCmdletBinding = $true})
            $result.Locale = $result.Metadata['Locale'] ?? "en-US"
            $result.ExternalHelpfile = $result.Metadata['external help file']
            $result
        }
    }
}
