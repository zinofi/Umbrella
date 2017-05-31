$configFiles = Get-ChildItem . *.csproj -rec
foreach ($file in $configFiles)
{
    (Get-Content $file.PSPath) |
    Foreach-Object { $_ -replace "build00349", "build00350" } |
    Set-Content $file.PSPath
}