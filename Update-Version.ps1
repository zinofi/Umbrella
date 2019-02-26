$configFiles = Get-ChildItem . *.csproj -rec
$affectedFiles = New-Object "System.Collections.Generic.List``1[string]"

# Manually alter the build number before pushing to NuGet feeds
$previousBuild = "2.1.0-rc-00025"
$currentBuild =  "2.1.0"

foreach ($file in $configFiles)
{
	$content = Get-Content $file.PSPath

	if($content -like "*" + $previousBuild + "*")
	{
		$affectedFiles.Add($file.Name)
		$content -replace $previousBuild, $currentBuild | Set-Content $file.PSPath
	}
}

Write-Output ""
Write-Output "Updating from $($previousBuild) to $($currentBuild)"
Write-Output ""
Write-Output "Total files: $($configFiles.Length)"
Write-Output "Total affected files: $($affectedFiles.Count)"
Write-Output ""

if(!$affectedFiles.Count.Equals(0))
{
	Write-Output "Affected Files:"
	$affectedFiles | ForEach-Object { Write-Output $_ }
}