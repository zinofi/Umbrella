# bootstrap DotNet CLI tools into this session.
$script = {$Branch='rel/1.0.0';iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/dotnet/cli/rel/1.0.0/scripts/obtain/dotnet-install.ps1'))}

# load up the global.json so we can find the version
$globalJson = Get-Content -Path $PSScriptRoot\global.json -Raw -ErrorAction Ignore | ConvertFrom-Json -ErrorAction Ignore

if($globalJson)
{
    $sdkVersion = $globalJson.sdk.version
}
else
{
    Write-Warning "Unable to locate global.json to determine using 'latest'"
    $sdkVersion = "latest"
}

&$script -version $sdkVersion

 # run dotnet restore on all project.json files in the folder structure including 2>1 to redirect stderr to stdout for badly behaved tools
Get-ChildItem -Path $PSScriptRoot -Filter project.json -Recurse | ForEach-Object { & dotnet restore $_.FullName 2>1 }