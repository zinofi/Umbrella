$configFiles = Get-ChildItem . *.csproj -rec
$affectedFiles = New-Object "System.Collections.Generic.HashSet``1[string]"

# Manually alter the build number before pushing to NuGet feeds
$previousBuild = "3.0.0-preview-0131"
$currentBuild =  "3.0.0-preview-0132"

foreach ($file in $configFiles)
{
	$hasChanged = $false
	$content = Get-Content $file.PSPath

	if($content -like "*<Version>" + $previousBuild + "</Version>*")
	{
		$hasChanged = $true
		$affectedFiles.Add($file.Name)
		$content = $content -replace ("<Version>" + $previousBuild + "</Version>"), ("<Version>" + $currentBuild + "</Version>")
	}

	# Ensure Copyright year is correct
	if($content -like "*<Copyright>Zinofi Digital Ltd 2019</Copyright>*")
	{
		$hasChanged = $true
		$affectedFiles.Add($file.Name)
		$content = $content -replace ("<Copyright>Zinofi Digital Ltd 2019</Copyright>"), ("<Copyright>Zinofi Digital Ltd 2020</Copyright>")
	}

	# Ensure Authors is correct
	if($content -like "*<Authors>Richard Edwards</Authors>*")
	{
		$hasChanged = $true
		$affectedFiles.Add($file.Name)
		$content = $content -replace ("<Authors>Richard Edwards</Authors>"), ("<Authors>Zinofi Digital Ltd</Authors>")
	}

	# Ensure License is correct
	if($content -like "*<PackageLicenseUrl>https://github.com/zinofi/Umbrella/blob/master/LICENSE</PackageLicenseUrl>*")
	{
		$hasChanged = $true
		$affectedFiles.Add($file.Name)
		$content = $content -replace ("<PackageLicenseUrl>https://github.com/zinofi/Umbrella/blob/master/LICENSE</PackageLicenseUrl>"), ("<PackageLicenseExpression>MIT</PackageLicenseExpression>")
	}

	# Ensure PackageId is converted to AssemblyName and all Umbrella references are changed to Zinofi
	# Leaving as Umbrella for now - TBD
	#if($content -like "*<PackageId>Umbrella*")
	#{
	#	$hasChanged = $true
	#	$affectedFiles.Add($file.Name)
	#	$content = $content -replace ("<PackageId>Umbrella"), ("<PackageId>Zinofi")
	#	$content = $content -replace ("<PackageId>"), ("<AssemblyName>")
	#	$content = $content	-replace ("</PackageId>"), ("</AssemblyName>")
	#}
	
	if($hasChanged)
	{
		$content | Set-Content $file.PSPath
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