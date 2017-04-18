$configFiles = Get-ChildItem . *.csproj -rec
foreach ($file in $configFiles)
{
    (Get-Content $file.PSPath) |
    Foreach-Object { $_ -replace "build00313", "build00314" } |
    Set-Content $file.PSPath
}