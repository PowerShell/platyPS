$gnon = cat .\out\SMA.generated.txt | ? {-not ($_ -match "Default Value     ") }
$onon = cat .\out\SMA.original.txt | ? {-not ($_ -match "Default Value     ") }

$onon > out\onon.txt
$gnon > out\gnon.txt

windiff .\out\onon.txt .\out\gnon.txt