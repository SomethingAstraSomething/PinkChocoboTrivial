$Uri = "https://github.com/SomethingAstraSomething/PinkChocoboTrivial/releases/latest/download/PinkChocoboTrivial.zip"
$OutFile = "PinkChocoboTrivial.zip"
Invoke-WebRequest -Uri $Uri -OutFile $OutFile
Expand-Archive -Path $OutFile -DestinationPath "PinkChocoboTrivial_extracted" -Force
Get-ChildItem -Path "PinkChocoboTrivial_extracted" -Recurse | Select-Object FullName
