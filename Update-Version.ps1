$configFiles = Get-ChildItem . *.csproj -rec
$affectedFiles = New-Object "System.Collections.Generic.List``1[string]"

$previousBuild = "build00359"
$currentBuild = "build00360"

foreach ($file in $configFiles)
{
	$content = Get-Content $file.PSPath

	if($content -like "*" + $previousBuild + "*")
	{
		$affectedFiles.Add($file.Name)
		$content -replace $previousBuild, $currentBuild | Set-Content $file.PSPath
	}
}

Write-Output "Total files: " $configFiles.Length
Write-Output "Total affected files: " $affectedFiles.Count

if(!$affectedFiles.Count.Equals(0))
{
	Write-Output ""
	Write-Output "Affected Files: "
	$affectedFiles | ForEach-Object { Write-Output $_ }
}