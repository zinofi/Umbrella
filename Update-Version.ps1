$configFiles = Get-ChildItem . *.csproj -rec
$affectedFiles = New-Object "System.Collections.Generic.List``1[string]"

# Manually alter the build number before pushing to NuGet feeds
$previousBuild = "3.0.0-preview-0046"
$currentBuild =  "3.0.0-preview-0047"

foreach ($file in $configFiles)
{
	$content = Get-Content $file.PSPath

	if($content -like "*<Version>" + $previousBuild + "</Version>*")
	{
		$affectedFiles.Add($file.Name)
		$content -replace ("<Version>" + $previousBuild + "</Version>"), ("<Version>" + $currentBuild + "</Version>") | Set-Content $file.PSPath
	}

	# Ensure Copyright year is correct
	if($content -like "*<Copyright>Zinofi Digital Ltd 2019</Copyright>*")
	{
		$affectedFiles.Add($file.Name)
		$content -replace ("<Copyright>Zinofi Digital Ltd 2019</Copyright>"), ("<Copyright>Zinofi Digital Ltd 2020</Copyright>") | Set-Content $file.PSPath
	}

	# Ensure Authors is correct
	if($content -like "*<Authors>Richard Edwards</Authors>*")
	{
		$affectedFiles.Add($file.Name)
		$content -replace ("<Authors>Richard Edwards</Authors>"), ("<Authors>Zinofi Digital Ltd</Authors>") | Set-Content $file.PSPath
	}

	# Ensure License is correct
	if($content -like "*<PackageLicenseUrl>https://github.com/zinofi/Umbrella/blob/master/LICENSE</PackageLicenseUrl>*")
	{
		$affectedFiles.Add($file.Name)
		$content -replace ("<PackageLicenseUrl>https://github.com/zinofi/Umbrella/blob/master/LICENSE</PackageLicenseUrl>"), ("<PackageLicenseExpression>MIT</PackageLicenseExpression>") | Set-Content $file.PSPath
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