$configFiles = Get-ChildItem . *.csproj -rec
foreach ($file in $configFiles)
{
    (Get-Content $file.PSPath) |
    Foreach-Object { $_ -replace "build00354", "build00355" } |
    Set-Content $file.PSPath
}