$configFiles = Get-ChildItem . *.csproj -rec
foreach ($file in $configFiles)
{
    (Get-Content $file.PSPath) |
    Foreach-Object { $_ -replace "build00338", "build00339" } |
    Set-Content $file.PSPath
}